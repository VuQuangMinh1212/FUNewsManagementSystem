using FUNewsManagementSystem.DAL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DAL.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly FunewsManagementContext _context;

        public TagRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return _context.Tags.ToList();
        }
        public IEnumerable<Tag> GetTagsByIds(IEnumerable<int> tagIds)
        {
            return _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
        }

        public Tag GetTagById(int id)
        {
            return _context.Tags.Find(id);
        }

        public void AddTag(Tag tag)
        {
            tag.TagId = (short)(_context.Tags.Max(a => (int?)a.TagId) + 1 ?? 1);
            _context.Tags.Add(tag);
            _context.SaveChanges();
        }

        public void UpdateTag(Tag tag)
        {
            _context.Tags.Update(tag);
            _context.SaveChanges();
        }

        public void DeleteTag(int id)
        {
            var tag = _context.Tags.Find(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Tag> GetAllTagsPaging(int page, int pageSize)
        {
            return _context.Tags
                .OrderBy(t => t.TagId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
