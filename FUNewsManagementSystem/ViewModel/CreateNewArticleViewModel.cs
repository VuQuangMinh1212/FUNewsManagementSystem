using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.ViewModel
{
    public class CreateNewArticleViewModel
    {
        public string NewsArticleId { get; set; } = null!;
        public short? CreatedById { get; set; }

        [Required(ErrorMessage = "NewsTitle is required")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        [Required(ErrorMessage = "NewsContents is required")]
        public string? NewsContent { get; set; }

        [Required(ErrorMessage = "NewsSource is required")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Please choose category")]
        [DisplayName("Category")]
        public short? CategoryId { get; set; }

        public bool NewsStatus { get; set; } = true;

        [Required(ErrorMessage = "Please choose tags")]
        [DisplayName("Tags")]
        public List<int>? TagsId { get; set; }
    }
}
