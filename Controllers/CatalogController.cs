using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeysShop.Data;
using System.Linq;
using System.Threading.Tasks;
using KeysShop.Models;

namespace KeysShop.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext _context;

        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Catalog
        /*public async Task<IActionResult> Index()
        {
            // Получаем игры с жанрами
            var games = await _context.Games
                .Include(g => g.Developer)
                .Include(g => g.GameGenres)
                    .ThenInclude(gg => gg.Genre)
                .Select(g => new GameViewModel
                {
                    Id = g.id_игры,
                    Title = g.название,
                    Description = g.описание,
                    Price = g.цена,
                    ImageUrl = g.ImageUrl,
                    ReleaseYear = g.год_выпуска,
                    Developer = g.Developer != null ? g.Developer.разработчик : "Не указан",
                    AvailableKeys = _context.Keys
                        .Count(k => k.id_игры == g.id_игры &&
                                   (k.продан == false || k.продан == null)),
                    Genres = g.GameGenres.Select(gg => gg.Genre.жанр).ToList() // ← ДОБАВЬ ЭТО
                })
                .Where(g => g.AvailableKeys > 0)
                .Take(20)
                .ToListAsync();

            return View(games);
        }*/
        public async Task<IActionResult> Index()
        {
            // Временно возвращаем пустой список
            var games = await _context.Games.ToListAsync();
            return View(games);

            // ИЛИ для теста создаем тестовые данные
            // var testGames = new List<Game>
            // {
            //     new Game { Id = 1, название = "Тестовая игра 1", год_выпуска = "2024" },
            //     new Game { Id = 2, название = "Тестовая игра 2", год_выпуска = "2024" }
            // };
            // return View(testGames);
        }

        // GET: /Catalog/Details/5 - УПРОЩЕННАЯ ВЕРСИЯ
        public async Task<IActionResult> Details(int id)
        {
            // ПЕРВЫЙ ШАГ: Найди игру БЕЗ всех связей
            var game = await _context.Games
                .Include(g => g.Developer) // Только разработчика
                .FirstOrDefaultAsync(g => g.id_игры == id);

            if (game == null)
            {
                return NotFound();
            }

            // ВТОРОЙ ШАГ: Отдельно посчитай ключи
            var availableKeys = await _context.Keys
                .CountAsync(k => k.id_игры == id &&
                                (k.продан == false || k.продан == null));

            var totalKeys = await _context.Keys
                .CountAsync(k => k.id_игры == id);

            // ТРЕТИЙ ШАГ: Получи жанры отдельным запросом
            var genres = await _context.GameGenres
                .Where(gg => gg.id_игры == id)
                .Include(gg => gg.Genre)
                .Select(gg => gg.Genre.жанр)
                .ToListAsync();

            // Создай ViewModel
            var viewModel = new GameDetailsViewModel
            {
                Id = game.id_игры,
                Title = game.название,
                Description = game.описание,
                Price = game.цена,
                ImageUrl = game.ImageUrl,
                ReleaseYear = game.год_выпуска,
                Developer = game.Developer != null ? game.Developer.разработчик : "Не указан",
                AvailableKeys = availableKeys,
                TotalKeys = totalKeys,
                Genres = genres
            };

            return View(viewModel);
        }

        // Простая модель для отображения
        public class GameViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public decimal? Price { get; set; }
            public string ImageUrl { get; set; }
            public string ReleaseYear { get; set; }
            public string Developer { get; set; }
            public int AvailableKeys { get; set; }
            public List<string> Genres { get; set; } // ← ДОБАВЬ ЭТУ СТРОЧКУ!
        }

        // Модель для детальной страницы
        public class GameDetailsViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public decimal? Price { get; set; }
            public string ImageUrl { get; set; }
            public string ReleaseYear { get; set; }
            public string Developer { get; set; }
            public int AvailableKeys { get; set; }
            public int TotalKeys { get; set; }
            public List<string> Genres { get; set; }

            public string FormattedPrice
            {
                get
                {
                    return Price?.ToString("C") ?? "Цена не указана";
                }
            }

            public bool IsInStock
            {
                get { return AvailableKeys > 0; }
            }
        }
    }
}
