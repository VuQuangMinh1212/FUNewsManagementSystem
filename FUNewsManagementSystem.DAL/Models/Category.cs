using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.DAL.Models;

public partial class Category
{
    public short CategoryId { get; set; }

    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = null!;

    [Display(Name = "Description of Category")]
    public string CategoryDesciption { get; set; } = null!;

    [Display(Name = "Parent Category")]
    public short? ParentCategoryId { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

    [Display(Name = "Parent Category")]
    public virtual Category? ParentCategory { get; set; }
}
