using ASPbb.DataAccess.Data;
using ASPbb.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ASPbb.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IApplicationUserRepository ApplicationUser { get; set; }
        public IForumRepository Forum { get; private set; }
        public ITopicRepository Topic { get; private set; }
        public IPostRepository Post { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUserRepository(_db);
            Forum = new ForumRepository(_db);
            Topic = new TopicRepository(_db);
            Post = new PostRepository(_db);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
