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
        public async Task<IHttpActionResult> GetUserOrders(int id, int limit = 5)
        {
            int userId = (int)Request.Properties["UserID"];

            try
            {
                if(userId == id)
                {
                    var orders = await DatabaseHelper.Instance.GetOrdersForUserAsync(userId, limit);
                    return Ok(orders);
                }
                else
                {
                    return BadRequest("Dont try that!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}