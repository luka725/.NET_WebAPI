using DataAccesLayer;
using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication.Common;

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
        public async Task<int> GetUserIdByEmail(string email)
        {
            try
            {
                var user = await dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    return user.ID;
                }
                else
                {
                    throw new InvalidOperationException("User not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user ID by email", ex);
            }
        }
        public async Task<UsersDTO> GetUserById(int id)
        {
            try
            {
                var user = await dbContext.Set<User>().FirstOrDefaultAsync(u => u.ID == id);
                if(user != null)
                {
                    return AutoMapperConfig.Mapper.Map<UsersDTO>(user);
                }
                else
                {
                    throw new InvalidOperationException("User not found");
                }
            }catch (Exception ex)
            {
                throw new Exception("Error getting user by ID", ex);
            }
        }
    }
}