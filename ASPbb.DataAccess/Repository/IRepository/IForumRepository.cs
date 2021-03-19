using System;
using ASPbb.Models;

namespace ASPbb.DataAccess.Repository.IRepository
{
    public interface IForumRepository : IRepository<Forum>
    {
        void Update(Forum forum);
    }
}

