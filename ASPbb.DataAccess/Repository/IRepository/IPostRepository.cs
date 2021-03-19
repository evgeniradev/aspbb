using System;
using ASPbb.Models;

namespace ASPbb.DataAccess.Repository.IRepository
{
    public interface IPostRepository : IRepository<Post>
    {
        void Update(Post post);
    }
}

