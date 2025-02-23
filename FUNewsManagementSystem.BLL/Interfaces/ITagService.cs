using FUNewsManagementSystem.DAL.Models;

namespace FUNewsManagementSystem.BLL.Interfaces
{
    public interface ITagService
    {
        IEnumerable<Tag> GetAllTags();
        IEnumerable<Tag> GetTagsByIds(IEnumerable<int> tagIds);
        Tag GetTagById(int id);
        void AddTag(Tag tag);
        void UpdateTag(Tag tag);
        void DeleteTag(int id);
    }
}
