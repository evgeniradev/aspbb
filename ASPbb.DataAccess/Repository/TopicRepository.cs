using System;
using System.Linq;
using ASPbb.DataAccess.Data;
using ASPbb.DataAccess.Repository.IRepository;
using ASPbb.Models;

namespace ASPbb.DataAccess.Repository
{
    public class TopicRepository : Repository<Topic>, ITopicRepository
    {
        private readonly ApplicationDbContext _db;

        public TopicRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Topic topic)
        {
            var objFromDb = _db.Topics.FirstOrDefault(s => s.Id == topic.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = topic.Name;
                objFromDb.PostsCount = topic.PostsCount;
                objFromDb.UpdatedDate = DateTime.UtcNow;
            }
        }
    }
}
