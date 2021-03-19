using System;
using System.Linq;
using ASPbb.DataAccess.Data;
using ASPbb.DataAccess.Repository.IRepository;
using ASPbb.Models;
 
namespace ASPbb.DataAccess.Repository
{
    public class ForumRepository : Repository<Forum>, IForumRepository
    {
        private readonly ApplicationDbContext _db;

        public ForumRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Forum forum)
        {
            var objFromDb = _db.Forums.FirstOrDefault(s => s.Id == forum.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = forum.Name;
                objFromDb.Description = forum.Description;
                objFromDb.TopicsCount = forum.TopicsCount;
                objFromDb.UpdatedDate = DateTime.UtcNow;
            }
        }
    }
}
