using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.DAL.Models;

public partial class Tag
{
    public int TagId { get; set; }

    [Display(Name = "Tag Name")]
    public string? TagName { get; set; }

    [Display(Name = "Note")]
    public string? Note { get; set; }

    public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();

}
