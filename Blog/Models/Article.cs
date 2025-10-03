using System.ComponentModel.DataAnnotations;
using Blog.Models;
using Microsoft.EntityFrameworkCore;
namespace Blog.Models
{
    /// <summary>
    /// Represents a blog article
    /// </summary>
    /// 

        public class Article
        {
            public int Id { get; set; }
            public string AuthorName { get; set; }
            public string AuthorEmail { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public DateTimeOffset PublishedDate { get; set; }
            public ICollection<Comment>? Comments { get; set; }
    }
    //public class Article
    //{
        /// <summary>
        /// The unique identifier for the article. Assigned at creation.
        /// </summary>
        //public int Id { get; set; }

        /// <summary>
        /// The name of the author who wrote the article.
        /// </summary>
        /// 
        //[Required]
        
        //public string AuthorName { get; set; }

        /// <summary>
        /// The email of the author who wrote the article.
        /// </summary>
        /// 
        //[Required]
        //[EmailAddress]
        //public string AuthorEmail { get; set; }

        /// <summary>
        /// The title of the article. Specified by the user.
        /// It is limited to 100 characters.
        /// </summary>
        /// 
        //[Required]
        //[MaxLength(100)]
        //public string Title { get; set; }

        /// <summary>
        /// The full content of the article. 
        /// </summary>
        /// 
        //[Required]
        //[MaxLength(100)]
        //public string Content { get; set; }

        /// <summary>
        /// Represents the moment the article was published
        /// </summary>
        //public DateTimeOffset PublishedDate { get; set; }
    //}
}
