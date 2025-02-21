using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using System.Collections.Generic;

namespace FUNewsManagementSystem.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return _tagRepository.GetAllTags();
        }

        public IEnumerable<Tag> GetTagsByIds(IEnumerable<int> tagIds)
        {
            return _tagRepository.GetTagsByIds(tagIds);
        }

        public Tag GetTagById(int id)
        {
            return _tagRepository.GetTagById(id);
        }

        public void AddTag(Tag tag)
        {
            _tagRepository.AddTag(tag);
        }

        public void UpdateTag(Tag tag)
        {
            _tagRepository.UpdateTag(tag);
        }

        public void DeleteTag(int id)
        {
            _tagRepository.DeleteTag(id);
        }
    }
}
