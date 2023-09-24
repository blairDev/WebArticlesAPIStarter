namespace WebArticlesAPI.Models
{
    /// <summary>
    /// DTO for Article does not include the Id
    /// </summary>
    public class ArticleDTO
    {
        public string Title { get; set; }
        public string UserComment { get; set; }
        public string ArticleUrl { get; set; }

    }
}
