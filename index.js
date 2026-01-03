const express = require('express');
const { Pool } = require('pg');

const app = express();

// Подключение к PostgreSQL на Render
const pool = new Pool({
    connectionString: "postgresql://keysshopdb_user:TXFgDCd1SnDxmEOnjTrR6RZCXSLzjkzq@dpg-d5chq9ili9vc73cibbdg-a.frankfurt-postgres.render.com/keysshopdb",
    ssl: { rejectUnauthorized: false }
});

// Автоматическое создание таблицы
async function setupDatabase() {
    try {
        console.log('🚀 Настраиваю базу данных...');

        // Создаем таблицу если её нет
        await pool.query(`
            CREATE TABLE IF NOT EXISTS games (
                id SERIAL PRIMARY KEY,
                название VARCHAR(100) NOT NULL,
                год_выпуска VARCHAR(10),
                описание TEXT,
                разработчик VARCHAR(100),
                изображение VARCHAR(200),
                цена DECIMAL(10,2) NOT NULL DEFAULT 0,
                жанры VARCHAR(200),
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )
        `);
        console.log('✅ Таблица games создана/проверена');

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
            console.log('✅ 5 тестовых игр добавлены');
        } else {
            console.log(`📊 В базе уже есть ${count} игр`);
        }

    } catch (error) {
        console.error('❌ Ошибка настройки БД:', error.message);
    }
}

// Настраиваем БД при запуске
setupDatabase();

// Главная страница - каталог игр
app.get('/', async (req, res) => {
    try {
        // Получаем игры из базы
        const result = await pool.query(`
            SELECT * FROM games 
            ORDER BY id
        `);

        // Создаем HTML страницу
        const html = `
        <!DOCTYPE html>
        <html>
        <head>
            <title>KeysShop - Каталог игр</title>
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <style>
                body {
                    font-family: Arial, sans-serif;
                    max-width: 1200px;
                    margin: 0 auto;
                    padding: 20px;
                    background: #f5f7fa;
                }
                .header {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    padding: 30px;
                    border-radius: 10px;
                    text-align: center;
                    margin-bottom: 30px;
                }
                .games-grid {
                    display: grid;
                    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
                    gap: 25px;
                }
                .game-card {
                    background: white;
                    border-radius: 10px;
                    padding: 20px;
                    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
                    transition: transform 0.3s;
                }
                .game-card:hover {
                    transform: translateY(-5px);
                }
                .game-title {
                    font-size: 18px;
                    font-weight: bold;
                    margin: 10px 0;
                    color: #333;
                }
                .game-price {
                    font-size: 22px;
                    font-weight: bold;
                    color: #2ecc71;
                    margin: 10px 0;
                }
                .game-info {
                    color: #666;
                    font-size: 14px;
                    margin: 5px 0;
                }
                .game-desc {
                    color: #777;
                    font-size: 13px;
                    margin: 10px 0;
                    line-height: 1.4;
                }
                .status {
                    text-align: center;
                    padding: 50px;
                    color: #666;
                }
            </style>
        </head>
        <body>
            <div class="header">
                <h1>🎮 KeysShop - Каталог игр</h1>
                <p>Лицензионные ключи для Steam игр</p>
                <p>Найдено игр: ${result.rows.length}</p>
            </div>
            
            <div class="games-grid">
                ${result.rows.map(game => `
                <div class="game-card">
                    <div class="game-title">${game.название}</div>
                    <div class="game-price">${game.цена === 0 ? 'БЕСПЛАТНО' : game.цена + ' ₽'}</div>
                    <div class="game-info">📅 ${game.год_выпуска || 'Не указан'}</div>
                    <div class="game-info">👨‍💻 ${game.разработчик || 'Неизвестно'}</div>
                    <div class="game-info">🎮 ${game.жанры || 'Жанры не указаны'}</div>
                    <div class="game-desc">
                        ${game.описание ? (game.описание.length > 100 ? game.описание.substring(0, 100) + '...' : game.описание) : 'Описание отсутствует'}
                    </div>
                </div>
                `).join('')}
            </div>
            
            ${result.rows.length === 0 ? `
            <div class="status">
                <h3>📭 Каталог пуст</h3>
                <p>База данных настраивается...</p>
                <script>
                    setTimeout(() => location.reload(), 5000);
                </script>
            </div>
            ` : ''}
            
            <div style="text-align: center; margin-top: 40px; color: #888; font-size: 14px;">
                <p>KeysShop © 2024 | Все игры лицензионные</p>
            </div>
        </body>
        </html>
        `;

        res.send(html);

    } catch (error) {
        res.send(`
            <html>
            <body style="font-family: Arial; padding: 40px;">
                <h1>Ошибка загрузки каталога</h1>
                <p>${error.message}</p>
                <p>Попробуйте обновить страницу через 30 секунд.</p>
            </body>
            </html>
        `);
    }
});

// Проверка здоровья
app.get('/health', async (req, res) => {
    try {
        await pool.query('SELECT 1');
        res.json({
            status: 'ok',
            message: 'KeysShop работает',
            timestamp: new Date().toISOString()
        });
    } catch (error) {
        res.status(500).json({
            status: 'error',
            error: error.message
        });
    }
});

// Добавление больше игр (для теста)
app.get('/add-more', async (req, res) => {
    try {
        await pool.query(`
            INSERT INTO games (название, год_выпуска, описание, разработчик, цена, жанры) VALUES
            ('Hogwarts Legacy', '2023', 'Ролевая игра по вселенной Гарри Поттера', 'Avalanche Software', 3999, 'РПГ'),
            ('Cyberpunk 2077', '2020', 'Футуристический экшен-RPG', 'CD Projekt RED', 1999, 'РПГ')
            ON CONFLICT DO NOTHING
        `);
        res.send('<h1>✅ Игры добавлены!</h1><a href="/">Вернуться в каталог</a>');
    } catch (error) {
        res.send('<h1>Ошибка: ' + error.message + '</h1>');
    }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`🚀 KeysShop запущен на порту ${PORT}`);
    console.log(`👉 Откройте: http://localhost:${PORT}`);
});
