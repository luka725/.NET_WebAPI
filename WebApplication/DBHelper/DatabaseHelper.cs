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
        public async Task<List<OrdersDTO>> GetOrdersForUserAsync(int userId, int page, int limit)
        {
            int skip = (page - 1) * limit;

            var orders = await dbContext.Set<Order>()
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            return AutoMapperConfig.Mapper.Map<List<OrdersDTO>>(orders);
        }
        public async Task<int?> RegisterUserAsync(UsersDTO registerUser)
        {
            User user = AutoMapperConfig.Mapper.Map<User>(registerUser);
            try
            {
                bool userExists = await dbContext.Set<User>().AnyAsync(u => u.Email == user.Email);
                if (userExists)
                {
                    return null;
                }
                user.RoleID = 1;
                dbContext.Set<User>().Add(user);
                await dbContext.SaveChangesAsync();

                return user.ID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<List<OrderDetailProductDTO>> GetOrderForUserAsync(int userId, int page, int limit)
        {
            int skip = (page - 1) * limit;

            var orderDetails = await dbContext.Set<Order>()
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .Skip(skip)
                .Take(limit)
                .Select(o => new OrderDetailProductDTO
                {
                    OrderID = o.ID,
                    OrderDate = o.OrderDate,
                    OrderDetailID = o.OrderDetail.ID,
                    ProductID = o.OrderDetail.Product.ID,
                    ProductName = o.OrderDetail.Product.Name,
                    Quantity = o.OrderDetail.Quantity,
                    ProductPrice = o.OrderDetail.Product.Price
                })
                .ToListAsync();

            return orderDetails;
        }
    }
}