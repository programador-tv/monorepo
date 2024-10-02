using Domain.Entities;

namespace Domain.Repositories
{
    public interface ITagRepository
    {
        public Task<List<Tag>> GetAllByFreetimeIdAsync(string id);
        public Task<List<Tag>> GetTagRelationByKeyword(string keyword);

        public Task CreateTagForLiveAndFreeTime(Tag tag);
    }
}
