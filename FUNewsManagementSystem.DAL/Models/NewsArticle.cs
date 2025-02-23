using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.DAL.Models;

public partial class NewsArticle
{
    public string NewsArticleId { get; set; } = null!;

    [Display(Name = "Title of News")]
    public string? NewsTitle { get; set; }

    [Display(Name = "Headline")]
    public string Headline { get; set; } = null!;

    [Display(Name = "Created Date")]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Content of News")]
    public string? NewsContent { get; set; }

    [Display(Name = "Source of News")]
    public string? NewsSource { get; set; }

    [Display(Name = "Category")]
    public short? CategoryId { get; set; }

    [Display(Name = "Status")]
    public bool NewsStatus { get; set; } = true;

    [Display(Name = "Created By")]
    public short? CreatedById { get; set; }

    [Display(Name = "Updated By")]
    public short? UpdatedById { get; set; }

    [Display(Name = "Modified Date")]
    public DateTime? ModifiedDate { get; set; }

    public virtual Category? Category { get; set; }

    public virtual SystemAccount? CreatedBy { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
