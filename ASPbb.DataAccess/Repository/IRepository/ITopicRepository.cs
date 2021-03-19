using System;
using ASPbb.Models;

namespace ASPbb.DataAccess.Repository.IRepository
{
    public interface ITopicRepository : IRepository<Topic>
    {
        void Update(Topic topic);
    }
}

