// index.js - УПРОЩЕННАЯ ВЕРСИЯ
const express = require('express');
const { Pool } = require('pg');

const app = express();

// Подключение к PostgreSQL Render
const pool = new Pool({
    connectionString: "postgresql://keysshopdb_user:TXFgDCd1SnDxmEOnjTrR6RZCXSLzjkzq@dpg-d5chq9ili9vc73cibbdg-a/keysshopdb",
    ssl: {
        rejectUnauthorized: false
    }
});

// АВТОМАТИЧЕСКОЕ СОЗДАНИЕ ТАБЛИЦЫ ПРИ ЗАПУСКЕ
async function createTableIfNotExists() {
    try {
        console.log('🔍 Проверяю наличие таблицы games...');

        // Создаем таблицу если ее нет
        await pool.query(`
      CREATE TABLE IF NOT EXISTS games (
        id SERIAL PRIMARY KEY,
        название VARCHAR(100) NOT NULL,
        год_выпуска VARCHAR(10),
        описание TEXT,
        разработчик VARCHAR(100),
        изображение VARCHAR(200),
        цена DECIMAL(10,2) NOT NULL,
        жанры VARCHAR(200),
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
      )
    `);

        console.log('✅ Таблица games готова');

        // Проверяем есть ли данные
        const check = await pool.query('SELECT COUNT(*) as count FROM games');
        const count = parseInt(check.rows[0].count);

        if (count === 0) {
            console.log('📝 Добавляю тестовые игры...');

            await pool.query(`
        INSERT INTO games (название, год_выпуска, описание, разработчик, изображение, цена, жанры) VALUES
        ('Counter-Strike 2', '2023', 'Бесплатный тактический шутер', 'Valve', 'https://cdn.cloudflare.steamstatic.com/steam/apps/730/header.jpg', 0.00, 'Шутер, Мультиплеер'),
        ('Dota 2', '2013', 'Бесплатная MOBA игра', 'Valve', 'https://cdn.cloudflare.steamstatic.com/steam/apps/570/header.jpg', 0.00, 'MOBA, Стратегия'),
        ('Apex Legends', '2019', 'Бесплатный королевская битва', 'Respawn Entertainment', 'https://cdn.cloudflare.steamstatic.com/steam/apps/1172470/header.jpg', 0.00, 'Баттл-рояль, Шутер'),
        ('Warframe', '2013', 'Бесплатный кооперативный экшен', 'Digital Extremes', 'https://cdn.cloudflare.steamstatic.com/steam/apps/230410/header.jpg', 0.00, 'Экшен, РПГ'),
        ('Path of Exile', '2013', 'Бесплатный action/RPG', 'Grinding Gear Games', 'https://cdn.cloudflare.steamstatic.com/steam/apps/238960/header.jpg', 0.00, 'РПГ, Экшен')
      `);

            console.log('✅ Тестовые игры добавлены');
        }

    } catch (error) {
        console.error('❌ Ошибка создания таблицы:', error.message);
    }
}

// Запускаем создание таблицы при старте
createTableIfNotExists();

// Главная страница - каталог игр
app.get('/', async (req, res) => {
    try {
        // Получаем все игры из базы
        const result = await pool.query('SELECT * FROM games ORDER BY id');

        // Простой HTML каталог
        const html = `
    <!DOCTYPE html>
    <html>
    <head>
        <title>KeysShop - Каталог</title>
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <style>
            * { margin: 0; padding: 0; box-sizing: border-box; }
            body { font-family: Arial; background: #f0f2f5; padding: 20px; }
            .container { max-width: 1200px; margin: 0 auto; }
            .header { 
                background: linear-gradient(135deg, #667eea, #764ba2); 
                color: white; 
                padding: 30px; 
                border-radius: 10px; 
                margin-bottom: 30px;
                text-align: center;
            }
            .games { 
                display: grid; 
                grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); 
                gap: 20px; 
            }
            .game { 
                background: white; 
                border-radius: 10px; 
                padding: 20px; 
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                transition: transform 0.3s;
            }
            .game:hover { transform: translateY(-5px); }
            .game img { 
                width: 100%; 
                height: 160px; 
                object-fit: cover; 
                border-radius: 5px; 
                margin-bottom: 15px;
                background: #eee;
            }
            .game h3 { color: #333; margin-bottom: 10px; }
            .game .price { 
                font-size: 22px; 
                font-weight: bold; 
                color: #2ecc71; 
                margin: 10px 0;
            }
            .game .desc { 
                color: #666; 
                font-size: 14px; 
                line-height: 1.5; 
                margin: 10px 0;
                max-height: 60px;
                overflow: hidden;
            }
            .game .info { 
                color: #888; 
                font-size: 12px; 
                margin: 5px 0;
            }
            .buy-btn { 
                background: #667eea; 
                color: white; 
                border: none; 
                padding: 12px; 
                width: 100%; 
                border-radius: 5px; 
                margin-top: 15px;
                font-size: 16px;
                cursor: pointer;
            }
            .buy-btn:hover { background: #764ba2; }
            .status { 
                text-align: center; 
                padding: 50px; 
                color: #666; 
                font-size: 18px;
            }
        </style>
    </head>
    <body>
        <div class="container">
            <div class="header">
                <h1>🎮 KeysShop - Каталог игр</h1>
                <p>Покупайте лицензионные ключи для Steam</p>
                <p style="margin-top: 10px; font-size: 14px;">Игр в каталоге: ${result.rows.length}</p>
            </div>
            
            ${result.rows.length === 0 ? `
            <div class="status">
                <h3>📭 Каталог пуст</h3>
                <p>Таблица создается автоматически...</p>
                <p>Обновите страницу через 10 секунд</p>
                <script>
                    setTimeout(() => location.reload(), 10000);
                </script>
            </div>
            ` : `
            <div class="games">
                ${result.rows.map(game => `
                <div class="game">
                    <img src="${game.изображение || 'https://via.placeholder.com/300x150/667eea/ffffff?text=Игра'}" 
                         alt="${game.название}"
                         onerror="this.src='https://via.placeholder.com/300x150/667eea/ffffff?text=${game.название.substring(0, 15)}'">
                    <h3>${game.название}</h3>
                    <div class="price">
                        ${game.цена === 0 ? 'БЕСПЛАТНО' : game.цена.toFixed(2) + ' ₽'}
                    </div>
                    <div class="info">📅 ${game.год_выпуска || 'Не указан'}</div>
                    <div class="info">👨‍💻 ${game.разработчик || 'Неизвестно'}</div>
                    <div class="info">🎮 ${game.жанры || 'Жанр не указан'}</div>
                    <div class="desc">
                        ${game.описание ? (game.описание.length > 80 ? game.описание.substring(0, 80) + '...' : game.описание) : 'Нет описания'}
                    </div>
                    <button class="buy-btn" 
                            onclick="alert('Вы выбрали: ${game.название}\\n\\nФункция покупки в разработке 🚀')">
                        ${game.цена === 0 ? 'СКАЧАТЬ БЕСПЛАТНО' : 'КУПИТЬ ЗА ' + game.цена.toFixed(2) + ' ₽'}
                    </button>
                </div>
                `).join('')}
            </div>
            `}
            
            <div style="text-align: center; margin-top: 40px; color: #888; font-size: 14px;">
                <p>KeysShop © 2024 | Все игры лицензионные</p>
                <p style="margin-top: 10px;">
                    <a href="/add-more" style="color: #667eea;">Добавить больше игр</a> | 
                    <a href="/reload" style="color: #667eea;">Обновить каталог</a>
                </p>
            </div>
        </div>
    </body>
    </html>
    `;

        res.send(html);

    } catch (error) {
        res.send(`
      <html>
      <body style="font-family: Arial; padding: 40px;">
        <h1>🚀 KeysShop запущен!</h1>
        <p>Создаю базу данных... Пожалуйста, подождите.</p>
        <p>Ошибка: ${error.message}</p>
        <script>
          setTimeout(() => location.reload(), 5000);
        </script>
      </body>
      </html>
    `);
    }
});

// Страница для добавления больше игр
app.get('/add-more', async (req, res) => {
    try {
        await pool.query(`
      INSERT INTO games (название, год_выпуска, описание, разработчик, изображение, цена, жанры) VALUES
      ('Hogwarts Legacy', '2023', 'Ролевая игра по вселенной Гарри Поттера', 'Avalanche Software', 'https://cdn.cloudflare.steamstatic.com/steam/apps/990080/header.jpg', 3999.00, 'РПГ, Приключение'),
      ('Cyberpunk 2077', '2020', 'Футуристический экшен-RPG', 'CD Projekt RED', 'https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/header.jpg', 1999.00, 'РПГ, Экшен'),
      ('Baldur's Gate 3', '2023', 'Лучшая RPG 2023 года', 'Larian Studios', 'https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/header.jpg', 2999.00, 'РПГ, Стратегия'),
      ('Grand Theft Auto V', '2015', 'Экшен в открытом мире', 'Rockstar Games', 'https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg', 1499.00, 'Экшен, Приключение'),
      ('The Witcher 3', '2015', 'Культовая RPG игра', 'CD Projekt RED', 'https://cdn.cloudflare.steamstatic.com/steam/apps/292030/header.jpg', 899.00, 'РПГ, Приключение')
      ON CONFLICT DO NOTHING
    `);

        res.send(`
      <html>
      <body style="font-family: Arial; padding: 40px; text-align: center;">
        <h1>✅ Игры добавлены!</h1>
        <p>В каталог добавлено 5 популярных игр</p>
        <p><a href="/" style="color: blue; font-size: 18px;">Вернуться в каталог</a></p>
      </body>
      </html>
    `);
    } catch (error) {
        res.send(`<h1>Ошибка: ${error.message}</h1>`);
    }
});

// Просто перезагрузить
app.get('/reload', (req, res) => {
    res.redirect('/');
});

// Статус приложения
app.get('/health', async (req, res) => {
    try {
        await pool.query('SELECT 1');
        res.json({
            status: '✅ OK',
            message: 'KeysShop работает',
            timestamp: new Date().toISOString()
        });
    } catch (error) {
        res.status(500).json({
            status: '❌ ERROR',
            error: error.message
        });
    }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`🚀 KeysShop запущен на порту ${PORT}`);
    console.log(`👉 Откройте: http://localhost:${PORT}`);
});
