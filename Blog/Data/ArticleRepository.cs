using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace Blog.Data
{
    /// <summary>
    /// Implementation of <see cref="IArticleRepository"/> using SQLite as a persistence solution.
    /// </summary>
    public class ArticleRepository : IArticleRepository
    {
        private readonly BlogContext _context;

        public ArticleRepository(BlogContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates the necessary tables for this application if they don't exist already.
        /// Should be called once when starting the service.
        /// </summary>
        //public void EnsureCreated()
        //{
        //    using var context = new BlogContext();
        //    context.Database.EnsureCreated();
        //}

        public IEnumerable<Article> GetAll()
        {
            return _context.Articles
                .ToList()
                .OrderBy(a => a.PublishedDate)
                .ToList();
        }
        public IEnumerable<Article> GetByAuthor(string AuthorFilter)
        {
            //var result = new BlogContext();
            
            return _context.Articles
                .Where(a => a.AuthorName.ToLower() == AuthorFilter.ToLower())
                .ToList()
                .OrderBy(a => a.PublishedDate)
                .ToList();
        }
        public Article? GetByTitle(string TitleFilter)
        {
            //var result = new BlogContext();
            if (string.IsNullOrWhiteSpace(TitleFilter)) { return null; };


            var normalized = TitleFilter.Trim().ToLowerInvariant();

            return _context.Articles
                .FirstOrDefault(a => a.Title.ToLower().Contains(normalized));

        }
        public IEnumerable<Article> GetByDateRange(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            //using var range = new BlogContext();

            return _context.Articles
                .Where(a => a.PublishedDate >= startDate && a.PublishedDate <= endDate)
                .ToList()
                .OrderBy(a => a.PublishedDate)
                .ToList();
        }
        public IEnumerable<Article> GetByEmail(string FilterEmail)
        {
            //var result = new BlogContext();
            return _context.Articles
                .Where(a => a.AuthorEmail.ToLower() == FilterEmail.ToLower())
                .ToList()
                .OrderBy(a => a.PublishedDate)
                .ToList();
        }
        public Article? GetById(int id)
        {
            //using var art=new BlogContext();
            return _context.Articles.Find(id);
        }

        public Article Create(Article article)
        {
            //using var insert=new BlogContext();

            _context.Articles.Add(article);
            _context.SaveChanges();


            return article;

        }

        public void AddComment(Comment comment)
        {
            //using var insert = new BlogContext();
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public IEnumerable<Comment> GetCommentsByArticleId(int articleId)
        {
            //using var result = new BlogContext();
            return _context.Comments
            .Where(c => c.ArticleId == articleId)
            .ToList()
            .OrderBy(c => c.PublishedDate)
            .ToList();
        }
    }
}
