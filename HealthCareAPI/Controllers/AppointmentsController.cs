using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using HealthCareAPI.DTOs;
using HealthCareAPI.Helpers;


[RoutePrefix("api/appointments")]
public class AppointmentsController : ApiController
{
    private readonly DatabaseHelper _dbHelper;

    public AppointmentsController()
    {
        _dbHelper = DatabaseHelper.Instance;
    }

    // GET /api/appointments
    [HttpGet]
    public async Task<IHttpActionResult> GetAllAppointments()
    {
        var userId = GetAuthenticatedUserId();
        var appointments = await _dbHelper.GetAllAppointmentsAsync(userId);

        return Ok(appointments);
    }

    // GET /api/appointments/{appointmentId}
    [HttpGet]
    [Route("{appointmentId}")]
    public async Task<IHttpActionResult> GetAppointment(int appointmentId)
    {
        var appointment = await _dbHelper.GetAppointmentByIdAsync(appointmentId);

        if (appointment == null)
        {
            return NotFound(); // Appointment not found
        }

        return Ok(appointment); // Return appointment details
    }

    // POST /api/appointments
    [HttpPost]
    public async Task<IHttpActionResult> CreateAppointment(AppointmentDTO newAppointment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Invalid model state
        }

        var appointmentId = await _dbHelper.CreateAppointmentAsync(newAppointment);

        return Created($"api/appointments/{appointmentId}", new { AppointmentId = appointmentId });
    }

    // PUT /api/appointments/{appointmentId}
    [HttpPut]
    [Route("{appointmentId}")]
    public async Task<IHttpActionResult> UpdateAppointment(int appointmentId, AppointmentDTO updatedAppointment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Invalid model state
        }

        var success = await _dbHelper.UpdateAppointmentAsync(appointmentId, updatedAppointment);

        if (!success)
        {
            return NotFound(); // Appointment not found
        }

        return Ok(); // Appointment updated successfully
    }

    // DELETE /api/appointments/{appointmentId}
    [HttpDelete]
    [Route("{appointmentId}")]
    public async Task<IHttpActionResult> CancelAppointment(int appointmentId)
    {
        var success = await _dbHelper.CancelAppointmentAsync(appointmentId);

        if (!success)
        {
            return NotFound(); // Appointment not found
        }

        return Ok(); // Appointment canceled successfully
    }

    // Example method to get authenticated user's ID (replace with your authentication logic)
    private int GetAuthenticatedUserId()
    {
        var principal = RequestContext.Principal as ClaimsPrincipal;
        var userIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }


        throw new UnauthorizedAccessException("User ID not found in claims.");
    }

}
