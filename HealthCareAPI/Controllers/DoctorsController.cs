using HealthCareAPI.DTOs;
using HealthCareAPI.Helpers;
using HealthCareDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HealthCareAPI.Controllers
{
    /// <summary>
    /// Controller for managing doctor operations.
    /// </summary>
    [TokenAuthenticationFilter]
    [RoutePrefix("api/doctors")]
    public class DoctorsController : ApiController
    {
        private readonly DatabaseHelper _dbHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorsController"/> class.
        /// </summary>
        public DoctorsController()
        {
            _dbHelper = DatabaseHelper.Instance;
        }

        /// <summary>
        /// Retrieves all doctors.
        /// </summary>
        /// <returns>A list of all doctors.</returns>
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

        /// <summary>
        /// Retrieves a doctor by their ID.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor.</param>
        /// <returns>The doctor details.</returns>
        [HttpGet]
        [Route("get/{doctorId}")]
        public async Task<IHttpActionResult> GetDoctorById(int doctorId)
        {
            try
            {
                var doctor = await _dbHelper.GetDoctorByIdAsync(doctorId);
                return Ok(doctor);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving doctor: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new doctor.
        /// </summary>
        /// <param name="doctor">The doctor details to create.</param>
        /// <returns>The created doctor.</returns>
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

        /// <summary>
        /// Updates an existing doctor by their ID.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor to update.</param>
        /// <param name="doctor">The updated doctor details.</param>
        /// <returns>The updated doctor.</returns>
        [HttpPut]
        [Route("{doctorId}")]
        public async Task<IHttpActionResult> UpdateDoctor(int doctorId, DoctorDTO doctor)
        {
            try
            {
                var updatedDoctor = await _dbHelper.UpdateDoctorAsync(doctorId, doctor);
                return Ok(updatedDoctor);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating doctor: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a doctor by their ID.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor to delete.</param>
        /// <returns>A success message if deletion is successful.</returns>
        [HttpDelete]
        [Route("{doctorId}")]
        public async Task<IHttpActionResult> DeleteDoctor(int doctorId)
        {
            try
            {
                await _dbHelper.DeleteDoctorAsync(doctorId);
                return Ok($"Doctor with ID {doctorId} deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting doctor: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves doctors with pagination based on their specialization.
        /// </summary>
        /// <param name="specialization">The specialization to filter doctors.</param>
        /// <param name="page">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The number of items per page for pagination (default is 10).</param>
        /// <returns>A list of doctor entities matching the specialization and pagination criteria.</returns>
        [HttpGet]
        [Route("specialization")]
        public async Task<IHttpActionResult> GetDoctorsWithSpecializationAsync(string specialization, int page = 1, int pageSize = 10)
        {
            try
            {
                var doctors = await _dbHelper.GetDoctorsWithSpecializationAsync(specialization, page, pageSize);

                // Fetch total count for pagination
                var totalCount = await _dbHelper.GetTotalDoctorsCountAsync(specialization);

                // Calculate total pages
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // Return results with pagination metadata
                return Ok(new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
                    Results = doctors
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while fetching doctors: {ex.Message}");
                return InternalServerError();
            }
        }
    }
}
