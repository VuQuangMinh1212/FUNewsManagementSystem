using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagementSystem.DAL.Models;

public partial class NewsTag
{
    [Key]
    [Column(Order = 1)]
    public string NewsArticleId { get; set; } = null!;

    [Key]
    [Column(Order = 2)]
    public int TagId { get; set; }
    public virtual NewsArticle? NewsArticle { get; set; }
    public virtual Tag? Tag { get; set; }
}
