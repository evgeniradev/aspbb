using System;
using System.Linq;
using ASPbb.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace ASPbb.DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private ApplicationDbContext _db;

        public DbInitializer(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            } catch(Exception ex)
            {

            }
        }
    }
}
