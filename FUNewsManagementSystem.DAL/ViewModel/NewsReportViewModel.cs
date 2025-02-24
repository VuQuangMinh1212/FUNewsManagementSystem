using FUNewsManagementSystem.DAL.Models;

namespace FUNewsManagementSystem.BLL.ViewModels
{
    public class NewsReportViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<NewsArticle> NewsArticles { get; set; }
    }
}
