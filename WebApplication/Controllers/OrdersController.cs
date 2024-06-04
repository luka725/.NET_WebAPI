using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApplication.Attributes;
using WebApplication.DBHelper;


namespace WebApplication.Controllers
{
    [BasicAuthentication]
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Orders()
        {
            return Ok();
        }

        [HttpGet]
        [Route("user")]
        public async Task<IHttpActionResult> GetUserOrders(int id, int limit = 10)
        {
            int userId = id;

            try
            {
                var orders = await DatabaseHelper.Instance.GetOrdersForUserAsync(userId, limit);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}