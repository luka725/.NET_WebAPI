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
    [RoutePrefix("/api/doctors")]
    public class DoctorsController : ApiController
    {
        private readonly DatabaseHelper _dbHelper;

        public DoctorsController()
        {
            _dbHelper = DatabaseHelper.Instance;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _dbHelper.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving doctors: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{doctorId}")]
        public async Task<IHttpActionResult> GetDoctorById(int doctorId)
        {
            try
            {
                var doctor = await _dbHelper.GetDoctorByIdAsync(doctorId);
                return Ok(doctor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving doctor: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateDoctor(DoctorDTO doctor)
        {
            try
            {
                var createdDoctor = await _dbHelper.CreateDoctorAsync(doctor);
                return Created($"api/doctors/{createdDoctor.DoctorID}", createdDoctor);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating doctor: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{doctorId}")]
        public async Task<IHttpActionResult> UpdateDoctor(int doctorId, DoctorDTO doctor)
        {
            try
            {
                var updatedDoctor = await _dbHelper.UpdateDoctorAsync(doctorId, doctor);
                return Ok(updatedDoctor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating doctor: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{doctorId}")]
        public async Task<IHttpActionResult> DeleteDoctor(int doctorId)
        {
            try
            {
                await _dbHelper.DeleteDoctorAsync(doctorId);
                return Ok($"Doctor with ID {doctorId} deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting doctor: {ex.Message}");
            }
        }


    }
}