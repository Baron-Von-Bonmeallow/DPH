using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class ArticlesController : Controller
    {
        private IArticleRepository _articleRepository;

        public ArticlesController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        // GET: ArticlesController
        public ActionResult Index(
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate)
        {
            if (startDate.HasValue || endDate.HasValue)
            {
                var start = startDate ?? DateTimeOffset.MinValue;
                var end = endDate ?? DateTimeOffset.MaxValue;
                return View(_articleRepository.GetByDateRange(start, end));
            }

            return View(_articleRepository.GetAll());
        }
        public ActionResult Search([FromQuery] string title, [FromQuery] string email)
        {
            IEnumerable<Article> results = Enumerable.Empty<Article>();
            var article = _articleRepository.GetByTitle(title);
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (article != null)
                {
                    results = new List<Article> { article };
                }
                results = article != null ? new List<Article> { article } : Enumerable.Empty<Article>();
                ViewBag.TitleFilter = title;
            }
            else if (!string.IsNullOrWhiteSpace(email))
            {
                results = _articleRepository.GetByEmail(email);
                ViewBag.EmailFilter = email;
            }
            else
            {
                return View("Search", Enumerable.Empty<Article>());
            }

            return View(results);

        }
        // GET: ArticlesController/Details/5
        public ActionResult Details(int id)
        {
            var article = _articleRepository.GetById(id);
            if (article == null)
            {
                return NotFound();
            }

            var comments = _articleRepository.GetCommentsByArticleId(id);

            var viewModel = new ArticleDetailsViewModel(article, comments);
            return View(viewModel);
        }

        // GET: ArticlesController/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: ArticlesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article article)
        {
            if (!ModelState.IsValid)
            {
                return View(article);
            }

            article.PublishedDate = DateTimeOffset.UtcNow;
            _articleRepository.Create(article);
            //Article created = _articleRepository.Create(article);

            return RedirectToAction(nameof(Details), new { id = article.Id });
        }

        [HttpPost]
        [Route("Articles/{articleId}/AddComment")]
        public ActionResult AddComment(int articleId, Comment comment)
        {
            Article? article = _articleRepository.GetById(articleId);
            if (article == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(comment.Content))
            {
                return BadRequest();
            }

            comment.ArticleId = articleId;
            comment.PublishedDate = DateTimeOffset.UtcNow;
            _articleRepository.AddComment(comment);

            return RedirectToAction(nameof(Details), new { id = articleId });
        }
    }
}
