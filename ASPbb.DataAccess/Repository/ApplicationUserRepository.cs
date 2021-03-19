using System;
using System.Linq;
using ASPbb.DataAccess.Data;
using ASPbb.DataAccess.Repository.IRepository;
using ASPbb.Models;

namespace ASPbb.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
