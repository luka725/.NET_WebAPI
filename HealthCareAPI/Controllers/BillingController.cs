using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HealthCareAPI.Helpers;
using HealthCareAPI.DTOs;

namespace HealthCareAPI.Controllers
{
    [RoutePrefix("api/billings")]
    [TokenAuthenticationFilter]
    public class BillingController : ApiController
    {
        private readonly DatabaseHelper _dbHelper;

        public BillingController()
        {
            _dbHelper = DatabaseHelper.Instance;
        }

        [HttpGet]
        [Route("api/billing")]
        public async Task<IHttpActionResult> GetBillingForUser(int userId)
        {
            try
            {
                var billingRecords = await _dbHelper.GetBillingForUserAsync(userId);
                return Ok(billingRecords);
            }
            catch (UnauthorizedAccessException)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized access."));
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving billing records: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{billingId}")]
        public async Task<IHttpActionResult> GetBillingById(int billingId)
        {
            try
            {
                var billingRecord = await _dbHelper.GetBillingByIdAsync(billingId);
                return Ok(billingRecord);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving billing record: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateBilling(BillingDTO billing)
        {
            try
            {
                var createdBilling = await _dbHelper.CreateBillingAsync(billing);
                return Created($"api/billing/{createdBilling.BillingID}", createdBilling);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating billing record: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{billingId}")]
        public async Task<IHttpActionResult> UpdateBilling(int billingId, BillingDTO billing)
        {
            try
            {
                var updatedBilling = await _dbHelper.UpdateBillingAsync(billingId, billing);
                return Ok(updatedBilling);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating billing record: {ex.Message}");
            }
        }

        [RoleAuthenticationFilter("Administrator")]
        [HttpDelete]
        [Route("{billingId}")]
        public async Task<IHttpActionResult> DeleteBilling(int billingId)
        {
            try
            {
                await _dbHelper.DeleteBillingAsync(billingId);
                return Ok($"Billing record with ID {billingId} deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting billing record: {ex.Message}");
            }
        }

    }
}