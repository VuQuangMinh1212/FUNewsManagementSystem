using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.DAL.Models;

public partial class SystemAccount
{
    public short AccountId { get; set; }

    [Display(Name = "Account Name")]
    public string? AccountName { get; set; }

    [Display(Name = "Account Email")]
    public string? AccountEmail { get; set; }

    [Display(Name = "Account Role")]
    public int? AccountRole { get; set; }

    [Display(Name = "Account Password")]
    public string? AccountPassword { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
