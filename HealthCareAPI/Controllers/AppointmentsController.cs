using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using HealthCareAPI.DTOs;
using HealthCareAPI.Helpers;

/// <summary>
/// Controller for managing appointments.
/// </summary>
[EnableCors(origins: "*", headers: "*", methods: "*")]
[TokenAuthenticationFilter]
[RoutePrefix("api/appointments")]
public class AppointmentsController : ApiController
{
    private readonly DatabaseHelper _dbHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentsController"/> class.
    /// </summary>
    public AppointmentsController()
    {
        _dbHelper = DatabaseHelper.Instance;
    }

    /// <summary>
    /// Retrieves a paginated list of appointments based on doctor name, patient name, appointment type, and payment status.
    /// </summary>
    /// <param name="doctorName">Name of the doctor.</param>
    /// <param name="patientName">Name of the patient.</param>
    /// <param name="appointmentType">Type of the appointment.</param>
    /// <param name="paymentStatus">Status of the payment.</param>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page for pagination.</param>
    /// <returns>A paginated list of appointments.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IHttpActionResult> GetUserDoctorAppointmentBilling(string doctorName = "", string patientName = "", string appointmentType = "", string paymentStatus = "", int page = 1, int pageSize = 10)
    {
        var data = await _dbHelper.GetUserDoctorAppointmentBillingAsync(doctorName, patientName, appointmentType, paymentStatus, page, pageSize);
        return Ok(data);
    }

    /// <summary>
    /// Retrieves details of a specific appointment by its ID.
    /// </summary>
    /// <param name="appointmentId">ID of the appointment.</param>
    /// <returns>Details of the appointment.</returns>
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

    /// <summary>
    /// Creates a new appointment.
    /// </summary>
    /// <param name="newAppointment">Details of the new appointment.</param>
    /// <returns>Created appointment with ID.</returns>
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

    /// <summary>
    /// Updates an existing appointment by its ID.
    /// </summary>
    /// <param name="appointmentId">ID of the appointment to update.</param>
    /// <param name="updatedAppointment">Updated details of the appointment.</param>
    /// <returns>Status of the update operation.</returns>
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

    /// <summary>
    /// Cancels an existing appointment by its ID.
    /// </summary>
    /// <param name="appointmentId">ID of the appointment to cancel.</param>
    /// <returns>Status of the cancelation operation.</returns>
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
}
