using DataAccesLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication.DBHelper
{
    public class DatabaseHelper
    {
        private static DatabaseHelper instance;
        private readonly DbContext dbContext;
        private DatabaseHelper(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public static DatabaseHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseHelper(new OrdersDataModel());
                }
                return instance;
            }
        }
        public async Task<bool> AuthenticateUserAsync(string email, string password)
        {
            var user = await dbContext.Set<User>().SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false;
            }
            return user.Password == password;
        }
    }
}