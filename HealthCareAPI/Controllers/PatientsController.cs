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
    [TokenAuthenticationFilter]
    [RoutePrefix("/api/patients")]
    public class PatientsController : ApiController
    {
        private readonly DatabaseHelper _dbHelper;

        public PatientsController()
        {
            _dbHelper = DatabaseHelper.Instance;
        }

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

        [HttpGet]
        [Route("{patientId}")]
        public async Task<IHttpActionResult> GetPatientById(int patientId)
        {
            try
            {
                var patient = await _dbHelper.GetPatientByIdAsync(patientId);
                return Ok(patient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving patient: {ex.Message}");
            }
        }

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

        [HttpPut]
        [Route("{patientId}")]
        public async Task<IHttpActionResult> UpdatePatient(int patientId, PatientDTO patient)
        {
            try
            {
                var updatedPatient = await _dbHelper.UpdatePatientAsync(patientId, patient);
                return Ok(updatedPatient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating patient: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{patientId}")]
        public async Task<IHttpActionResult> DeletePatient(int patientId)
        {
            try
            {
                await _dbHelper.DeletePatientAsync(patientId);
                return Ok($"Patient with ID {patientId} deleted successfully.");
            }
            catch (KeyNotFoundException ex)
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