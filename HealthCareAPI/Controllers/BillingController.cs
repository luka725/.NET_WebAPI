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
    /// <summary>
    /// Controller for managing billing operations.
    /// </summary>
    [RoutePrefix("api/billings")]
    [TokenAuthenticationFilter]
    public class BillingController : ApiController
    {
        private readonly DatabaseHelper _dbHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillingController"/> class.
        /// </summary>
        public BillingController()
        {
            _dbHelper = DatabaseHelper.Instance;
        }

        /// <summary>
        /// Retrieves billing records for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>Billing records for the user.</returns>
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

        /// <summary>
        /// Retrieves a billing record by its ID.
        /// </summary>
        /// <param name="billingId">The ID of the billing record.</param>
        /// <returns>The billing record.</returns>
        [HttpGet]
        [Route("{billingId}")]
        public async Task<IHttpActionResult> GetBillingById(int billingId)
        {
            try
            {
                var billingRecord = await _dbHelper.GetBillingByIdAsync(billingId);
                return Ok(billingRecord);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving billing record: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new billing record.
        /// </summary>
        /// <param name="billing">The billing record to create.</param>
        /// <returns>The created billing record.</returns>
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

        /// <summary>
        /// Updates an existing billing record by its ID.
        /// </summary>
        /// <param name="billingId">The ID of the billing record to update.</param>
        /// <param name="billing">The updated billing details.</param>
        /// <returns>The updated billing record.</returns>
        [HttpPut]
        [Route("{billingId}")]
        public async Task<IHttpActionResult> UpdateBilling(int billingId, BillingDTO billing)
        {
            try
            {
                var updatedBilling = await _dbHelper.UpdateBillingAsync(billingId, billing);
                return Ok(updatedBilling);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating billing record: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a billing record by its ID.
        /// </summary>
        /// <param name="billingId">The ID of the billing record to delete.</param>
        /// <returns>Success message if deletion is successful.</returns>
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
            catch (KeyNotFoundException)
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
