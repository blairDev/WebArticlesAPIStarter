namespace WebArticlesAPI.Models
{
    public class Article
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string UserComment { get; set; }
        public string ArticleUrl { get; set; }

    }
}
