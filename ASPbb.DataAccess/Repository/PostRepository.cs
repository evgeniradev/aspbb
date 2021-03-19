using System;
using System.Linq;
using ASPbb.DataAccess.Data;
using ASPbb.DataAccess.Repository.IRepository;
using ASPbb.Models;

namespace ASPbb.DataAccess.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly ApplicationDbContext _db;

        public PostRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Post post)
        {
            var objFromDb = _db.Posts.FirstOrDefault(s => s.Id == post.Id);

            if (objFromDb != null)
            {
                objFromDb.Content = post.Content;
                objFromDb.UpdatedDate = DateTime.UtcNow;
            }
        }
    }
}
