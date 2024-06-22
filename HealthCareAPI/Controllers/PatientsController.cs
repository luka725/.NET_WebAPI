using HealthCareAPI.DTOs;
using HealthCareAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HealthCareAPI.Controllers
{
    /// <summary>
    /// Controller for managing patient operations.
    /// </summary>
    [TokenAuthenticationFilter]
    [RoutePrefix("api/patients")]
    public class PatientsController : ApiController
    {
        private readonly DatabaseHelper _dbHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientsController"/> class.
        /// </summary>
        public PatientsController()
        {
            _dbHelper = DatabaseHelper.Instance;
        }

        /// <summary>
        /// Retrieves all patients.
        /// </summary>
        /// <returns>A list of all patients.</returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllPatients()
        {
            try
            {
                var patients = await _dbHelper.GetAllPatientsAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving patients: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a patient by their ID.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <returns>The patient details.</returns>
        [HttpGet]
        [Route("{patientId}")]
        public async Task<IHttpActionResult> GetPatientById(int patientId)
        {
            try
            {
                var patient = await _dbHelper.GetPatientByIdAsync(patientId);
                return Ok(patient);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving patient: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        /// <param name="patient">The patient details to create.</param>
        /// <returns>The created patient.</returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreatePatient(PatientDTO patient)
        {
            try
            {
                var createdPatient = await _dbHelper.CreatePatientAsync(patient);
                return Created($"api/patients/{createdPatient.PatientID}", createdPatient);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating patient: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing patient by their ID.
        /// </summary>
        /// <param name="patientId">The ID of the patient to update.</param>
        /// <param name="patient">The updated patient details.</param>
        /// <returns>The updated patient.</returns>
        [HttpPut]
        [Route("{patientId}")]
        public async Task<IHttpActionResult> UpdatePatient(int patientId, PatientDTO patient)
        {
            try
            {
                var updatedPatient = await _dbHelper.UpdatePatientAsync(patientId, patient);
                return Ok(updatedPatient);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating patient: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a patient by their ID.
        /// </summary>
        /// <param name="patientId">The ID of the patient to delete.</param>
        /// <returns>A success message if deletion is successful.</returns>
        [HttpDelete]
        [Route("{patientId}")]
        public async Task<IHttpActionResult> DeletePatient(int patientId)
        {
            try
            {
                await _dbHelper.DeletePatientAsync(patientId);
                return Ok($"Patient with ID {patientId} deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting patient: {ex.Message}");
            }
        }
    }
}
