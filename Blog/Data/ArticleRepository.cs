using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using NuGet.Protocol.Plugins;

namespace Blog.Data
{
    /// <summary>
    /// Implementation of <see cref="IArticleRepository"/> using SQLite as a persistence solution.
    /// </summary>
    public class ArticleRepository : IArticleRepository
    {
        private readonly string _connectionString;

        public ArticleRepository(DatabaseConfig _config)
        {
            //using var connection = new SqliteConnection(_connectionString);
            //connection.Open();
            _connectionString = _config.DefaultConnectionString ?? throw new ArgumentNullException("Connection string not found");
        }

        /// <summary>
        /// Creates the necessary tables for this application if they don't exist already.
        /// Should be called once when starting the service.
        /// </summary>
        public void EnsureCreated()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS Articles
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AuthorName TEXT NOT NULL,
                    AuthorEmail TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    PublishedDate TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Comments
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ArticleId INTEGER NOT NULL,
                    Content TEXT NOT NULL,
                    PublishedDate TEXT NOT NULL,
                    FOREIGN KEY (ArticleId) REFERENCES Articles(Id) ON DELETE CASCADE   
                )";
            command.ExecuteNonQuery();
        }

        public IEnumerable<Article> GetAll()
        {
            var result = new List<Article>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var all = connection.CreateCommand();
            all.CommandText = @"SELECT * FROM Articles";
            using var reader = all.ExecuteReader();
            while (reader.Read())
            {
                var article = new Article()
                {
                    Id = reader.GetInt32(0),
                    AuthorName = reader.GetString(1),
                    AuthorEmail = reader.GetString(2),
                    Title = reader.GetString(3),
                    Content = reader.GetString(4),
                    PublishedDate = DateTimeOffset.Parse(reader.GetString(5))
                };

                result.Add(article);
            }
            return result;
        }

        public IEnumerable<Article> GetByDateRange(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var result = new List<Article>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var range = connection.CreateCommand();
            range.CommandText = @"SELECT * FROM Articles WHERE PublishedDate BETWEEN $startDate AND $endDate";

            range.Parameters.AddWithValue("$startDate", startDate.ToString("o"));
            range.Parameters.AddWithValue("$endDate", endDate.ToString("o"));


            using var reader = range.ExecuteReader();
            while (reader.Read())
            {
                var article = new Article()
                {
                    Id = reader.GetInt32(0),
                    AuthorName = reader.GetString(1),
                    AuthorEmail = reader.GetString(2),
                    Title = reader.GetString(3),
                    Content = reader.GetString(4),
                    PublishedDate = DateTimeOffset.Parse(reader.GetString(5))
                };

                result.Add(article);
            }
            return result;
        }

        public Article? GetById(int id)
        {
            var result = new List<Article>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Articles WHERE Id=$id";
            command.Parameters.AddWithValue("$id", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Article()
                {
                    Id = reader.GetInt32(0),
                    AuthorName = reader.GetString(1),
                    AuthorEmail = reader.GetString(2),
                    Title = reader.GetString(3),
                    Content = reader.GetString(4),
                    PublishedDate = DateTimeOffset.Parse(reader.GetString(5))
                };
                //result.Add(article);
            }
            return null;
        }

        public Article Create(Article article)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var insert = connection.CreateCommand();
            insert.CommandText = @"INSERT INTO Articles(AuthorName,AuthorEmail,Title,Content,PublishedDate)
            VALUES($name,$email,$title,$content,$date)";
            insert.Parameters.AddWithValue("$name", article.AuthorName);
            insert.Parameters.AddWithValue("$email", article.AuthorEmail);
            insert.Parameters.AddWithValue("$title", article.Title);
            insert.Parameters.AddWithValue("$content", article.Content);
            insert.Parameters.AddWithValue("$date", article.PublishedDate.ToString("o"));
            insert.ExecuteNonQuery();
            var newId = connection.CreateCommand();
            newId.CommandText = "SELECT last_insert_rowid();";
            article.Id = Convert.ToInt32(newId.ExecuteScalar());

            return article;

        }

        public void AddComment(Comment comment)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO Comments (ArticleId,Content,PublishedDate)
                VALUES($articleId,$content,$date)";
            command.Parameters.AddWithValue("$articleId", comment.ArticleId);
            command.Parameters.AddWithValue("$content", comment.Content);
            command.Parameters.AddWithValue("$date", comment.PublishedDate.ToString("o"));
            command.ExecuteNonQuery();
        }

        public IEnumerable<Comment> GetCommentsByArticleId(int articleId)
        {
            var result = new List<Comment>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Comments WHERE ArticleId=$Id";
            command.Parameters.AddWithValue("$Id", articleId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            { 
                var comments=new Comment
                {
                    Id = reader.GetInt32(0),
                    ArticleId = reader.GetInt32(1),
                    Content = reader.GetString(2),
                    PublishedDate = DateTimeOffset.Parse(reader.GetString(3))
                };
                result.Add(comments);
            }
            return result;
        }
    }
}
