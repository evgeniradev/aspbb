using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ASPbb.DataAccess.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ASPbb.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository ApplicationUser { get; }
        IForumRepository Forum { get; }
        ITopicRepository Topic { get; }
        IPostRepository Post { get; }
 
        void Save();
    }
}
