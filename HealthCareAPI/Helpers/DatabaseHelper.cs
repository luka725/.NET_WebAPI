using AutoMapper;
using HealthCareAPI.Common;
using HealthCareAPI.DTOs;
using HealthCareDAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace HealthCareAPI.Helpers
{
    /// <summary>
    /// Helper class to manage database operations for HealthCareAPI.
    /// </summary>
    public class DatabaseHelper
    {
        private static DatabaseHelper _instance;
        private static readonly object _lock = new object();
        private readonly HealthCareModel _context;

        private DatabaseHelper()
        {
            _context = new HealthCareModel();
        }

        /// <summary>
        /// Singleton instance of the DatabaseHelper class.
        /// </summary>
        public static DatabaseHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DatabaseHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Retrieves all roles from the database asynchronously.
        /// </summary>
        /// <returns>A list of RoleDTOs.</returns>
        public async Task<List<RoleDTO>> GetRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return AutoMapperConfig.Mapper.Map<List<Role>, List<RoleDTO>>(roles);
        }
        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user DTO corresponding to the provided ID.</returns>
        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.UserID == userId);
            return AutoMapperConfig.Mapper.Map<UserDTO>(user);
        }

        /// <summary>
        /// Updates a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="updatedUser">The updated user DTO.</param>
        /// <returns>True if the user was successfully updated; otherwise, false.</returns>

        public async Task<bool> UpdateUserAsync(int userId, UserDTO updatedUser)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return false; // User not found
            }

            AutoMapperConfig.Mapper.Map(updatedUser, user);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true; // Successfully updated user
        }

        /// <summary>
        /// Deletes a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>True if the user was successfully deleted; otherwise, false.</returns>

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return false; // User not found
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted user
        }


        /// <summary>
        /// Retrieves an appointment by its ID asynchronously.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to retrieve.</param>
        /// <returns>The appointment DTO corresponding to the provided ID.</returns>

        // Retrieve details of a specific appointment
        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            return AutoMapperConfig.Mapper.Map<AppointmentDTO>(appointment);
        }

        /// <summary>
        /// Retrieves all appointments for a given user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user for whom appointments are retrieved.</param>
        /// <returns>A list of appointment DTOs.</returns>

        // Retrieve all appointments for the authenticated user
        public async Task<List<AppointmentDTO>> GetAllAppointmentsAsync(int userId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PatientID == userId || a.DoctorID == userId)
                .ToListAsync();

            return AutoMapperConfig.Mapper.Map<List<AppointmentDTO>>(appointments);
        }

        // Create a new appointment
        /// <summary>
        /// Creates a new appointment asynchronously.
        /// </summary>
        /// <param name="newAppointment">The appointment DTO containing details of the new appointment.</param>
        /// <returns>The ID of the newly created appointment.</returns>
        public async Task<int> CreateAppointmentAsync(AppointmentDTO newAppointment)
        {
            var appointment = AutoMapperConfig.Mapper.Map<Appointment>(newAppointment);

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return appointment.AppointmentID;
        }
        /// <summary>
        /// Updates details of an existing appointment asynchronously.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to update.</param>
        /// <param name="updatedAppointment">The updated appointment DTO.</param>
        /// <returns>True if the appointment was successfully updated; otherwise, false.</returns>

        // Update details of an existing appointment
        public async Task<bool> UpdateAppointmentAsync(int appointmentId, AppointmentDTO updatedAppointment)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment == null)
            {
                return false; // Appointment not found
            }

            AutoMapperConfig.Mapper.Map(updatedAppointment, appointment);

            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true; // Successfully updated appointment
        }
        /// <summary>
        /// Cancels an appointment asynchronously.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to cancel.</param>
        /// <returns>True if the appointment was successfully canceled; otherwise, false.</returns>

        // Cancel an appointment
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment == null)
            {
                return false; // Appointment not found
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return true; // Successfully canceled appointment
        }
        /// <summary>
        /// Retrieves billing records for a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user for whom billing records are retrieved.</param>
        /// <returns>A list of billing DTOs.</returns>
        public async Task<List<BillingDTO>> GetBillingForUserAsync(int userId)
        {
            var billings = await _context.Billings
                                        .Where(b => b.PatientID == userId || b.DoctorID == userId)
                                        .ToListAsync();

            return billings.Select(b => AutoMapperConfig.Mapper.Map<BillingDTO>(b)).ToList();
        }
        /// <summary>
        /// Retrieves a billing record by its ID asynchronously.
        /// </summary>
        /// <param name="billingId">The ID of the billing record to retrieve.</param>
        /// <returns>The billing DTO corresponding to the provided ID.</returns>
        public async Task<BillingDTO> GetBillingByIdAsync(int billingId)
        {
            var billing = await _context.Billings.FindAsync(billingId);

            if (billing == null)
            {
                throw new KeyNotFoundException($"Billing record with ID {billingId} not found.");
            }

            return AutoMapperConfig.Mapper.Map<BillingDTO>(billing);
        }
        /// <summary>
        /// Creates a new billing record asynchronously.
        /// </summary>
        /// <param name="billing">The billing DTO containing details of the new billing record.</param>
        /// <returns>The newly created billing DTO.</returns>
        public async Task<BillingDTO> CreateBillingAsync(BillingDTO billing)
        {
            var newBilling = AutoMapperConfig.Mapper.Map<Billing>(billing);

            _context.Billings.Add(newBilling);
            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<BillingDTO>(newBilling);
        }
        /// <summary>
        /// Updates a billing record asynchronously.
        /// </summary>
        /// <param name="billingId">The ID of the billing record to update.</param>
        /// <param name="billing">The updated billing DTO.</param>
        /// <returns>The updated billing DTO.</returns>
        public async Task<BillingDTO> UpdateBillingAsync(int billingId, BillingDTO billing)
        {
            var billingToUpdate = await _context.Billings.FindAsync(billingId);

            if (billingToUpdate == null)
            {
                throw new KeyNotFoundException($"Billing record with ID {billingId} not found.");
            }

            // Update properties based on billing DTO
            billingToUpdate.Amount = billing.Amount;
            billingToUpdate.PaymentStatus = billing.PaymentStatus;
            billingToUpdate.PaymentMethod = billing.PaymentMethod;

            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<BillingDTO>(billingToUpdate);
        }
        /// <summary>
        /// Deletes a billing record asynchronously.
        /// </summary>
        /// <param name="billingId">The ID of the billing record to delete.</param>
        public async Task DeleteBillingAsync(int billingId)
        {
            var billingToDelete = await _context.Billings.FindAsync(billingId);

            if (billingToDelete == null)
            {
                throw new KeyNotFoundException($"Billing record with ID {billingId} not found.");
            }

            _context.Billings.Remove(billingToDelete);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a user with associated roles by their ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user DTO with associated roles.</returns>
        public async Task<UserWithRolesDTO> GetUserWithRolesByIdAsync(int userId)
        {
            var user = await _context.Set<User>()
                                     .Include(u => u.Role)
                                     .FirstOrDefaultAsync(u => u.UserID == userId);
            return AutoMapperConfig.Mapper.Map<UserWithRolesDTO>(user);
        }
        /// <summary>
        /// Retrieves a user with associated roles and tokens by their ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user DTO with associated roles and tokens.</returns>
        public async Task<UserWithRolesAndTokensDTO> GetUserWithRolesAndTokensByIdAsync(int userId)
        {
            var user = await _context.Set<User>()
                                     .Include(u => u.Role)
                                     .Include(u => u.Tokens)
                                     .FirstOrDefaultAsync(u => u.UserID == userId);

            return AutoMapperConfig.Mapper.Map<UserWithRolesAndTokensDTO>(user);
        }
        /// <summary>
        /// Retrieves all doctors asynchronously.
        /// </summary>
        /// <returns>A list of doctor DTOs.</returns>
        public async Task<List<DoctorDTO>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return doctors.Select(d => AutoMapperConfig.Mapper.Map<DoctorDTO>(d)).ToList();
        }
        /// <summary>
        /// Retrieves a doctor by their ID asynchronously.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor to retrieve.</param>
        /// <returns>The doctor DTO corresponding to the provided ID.</returns>
        public async Task<DoctorDTO> GetDoctorByIdAsync(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);

            if (doctor == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");
            }

            return AutoMapperConfig.Mapper.Map<DoctorDTO>(doctor);
        }
        /// <summary>
        /// Cleans up expired tokens asynchronously.
        /// </summary>
        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = _context.Tokens.Where(t => t.ExpiryDateTime <= DateTime.UtcNow);
            _context.Tokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Creates a new doctor asynchronously.
        /// </summary>
        /// <param name="doctor">The doctor DTO containing details of the new doctor.</param>
        /// <returns>The newly created doctor DTO.</returns>
        public async Task<DoctorDTO> CreateDoctorAsync(DoctorDTO doctor)
        {
            var newDoctor = AutoMapperConfig.Mapper.Map<Doctor>(doctor);

            _context.Doctors.Add(newDoctor);
            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<DoctorDTO>(newDoctor);
        }
        /// <summary>
        /// Updates a doctor asynchronously.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor to update.</param>
        /// <param name="doctor">The updated doctor DTO.</param>
        /// <returns>The updated doctor DTO.</returns>
        public async Task<DoctorDTO> UpdateDoctorAsync(int doctorId, DoctorDTO doctor)
        {
            var doctorToUpdate = await _context.Doctors.FindAsync(doctorId);

            if (doctorToUpdate == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");
            }

            // Update properties based on doctor DTO
            doctorToUpdate.FirstName = doctor.FirstName;
            doctorToUpdate.LastName = doctor.LastName;
            doctorToUpdate.Specialization = doctor.Specialization;
            doctorToUpdate.ContactNumber = doctor.ContactNumber;
            doctorToUpdate.Email = doctor.Email;

            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<DoctorDTO>(doctorToUpdate);
        }

        /// <summary>
        /// Validates a user asynchronously by username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The validated user entity if successful; otherwise, null.</returns>
        public async Task<User> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Set<User>().Include(u => u.Tokens).FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var hashedPassword = ComputeSha256Hash(password);
            if (user.PasswordHash.Trim() != hashedPassword) return null;

            return user;
        }
        /// <summary>
        /// Generates a token for a user asynchronously.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>The generated token entity.</returns>
        public async Task<Token> GenerateTokenAsync(User user)
        {
            var tokenValue = GenerateTokenValue();
            var existingToken = user.Tokens.FirstOrDefault(t => t.ExpiryDateTime <= DateTime.UtcNow);

            Token token;

            if (existingToken != null)
            {
                existingToken.TokenValue = tokenValue;
                existingToken.ExpiryDateTime = DateTime.UtcNow.AddHours(1);
                existingToken.TokenType = "Bearer";
                token = existingToken;
            }
            else
            {
                token = new Token
                {
                    UserID = user.UserID,
                    TokenValue = tokenValue,
                    ExpiryDateTime = DateTime.UtcNow.AddHours(1),
                    TokenType = "Bearer"
                };

                _context.Tokens.Add(token);
            }

            await _context.SaveChangesAsync();
            return token;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string GenerateTokenValue()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="username">The username for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="firstName">The first name of the new user.</param>
        /// <param name="lastName">The last name of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="phoneNumber">The phone number of the new user.</param>
        /// <param name="roleId">The ID of the role assigned to the new user.</param>
        /// <returns>The registered user DTO with roles and tokens.</returns>
        public async Task<UserWithRolesAndTokensDTO> RegisterUser(string username, string password, string firstName, string lastName, string email, string phoneNumber, int roleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(firstName) ||
                    string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(phoneNumber))
                {
                    throw new ArgumentException("All fields are required.");
                }

                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    throw new ArgumentException("Username is already taken.");
                }

                string hashedPassword = ComputeSha256Hash(password);

                Role role = await _context.Roles.FindAsync(roleId) ?? throw new ArgumentException($"Role with ID {roleId} not found.");

                User newUser = new User
                {
                    Username = username,
                    PasswordHash = hashedPassword,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    RegistrationDate = DateTime.Today,
                    IsActive = true,
                    RoleID = roleId
                };

                try
                {
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException;
                    while (innerException != null)
                    {
                        innerException = innerException.InnerException;
                    }

                    throw;
                }

                var registeredUser = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Tokens)
                    .FirstOrDefaultAsync(u => u.UserID == newUser.UserID);

                var userDTO = new UserWithRolesAndTokensDTO
                {
                    UserID = registeredUser.UserID,
                    Username = registeredUser.Username,
                    FirstName = registeredUser.FirstName,
                    LastName = registeredUser.LastName,
                    Email = registeredUser.Email,
                    PhoneNumber = registeredUser.PhoneNumber,
                    RegistrationDate = registeredUser.RegistrationDate ?? DateTime.MinValue,
                    IsActive = registeredUser.IsActive ?? false,
                    Roles = new RoleDTO
                    {
                        RoleID = registeredUser.Role.RoleID,
                        RoleName = registeredUser.Role.RoleName
                    },
                    Token = registeredUser.Tokens.Select(t => new TokenDTO
                    {
                        TokenID = t.TokenID,
                        TokenValue = t.TokenValue
                    }).ToList()
                };

                return userDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Retrieves a user by their token asynchronously.
        /// </summary>
        /// <param name="token">The token value associated with the user.</param>
        /// <returns>The user entity corresponding to the provided token.</returns>
        public async Task<User> GetUserByTokenAsync(string token)
        {
            try
            {
                using (var context = new HealthCareModel()) // Replace with your DbContext initialization logic
                {
                    // Fetch the user by matching the token
                    var user = await context.Users
                        .Include(u => u.Tokens)
                        .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.TokenValue == token && t.ExpiryDateTime > DateTime.UtcNow));

                    return user;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace Console.WriteLine with your logging mechanism)
                Console.WriteLine(ex.Message);
                throw; // Re-throw the exception to propagate it up the call stack
            }
        }

        /// <summary>
        /// Retrieves a token asynchronously based on its value.
        /// </summary>
        /// <param name="tokenValue">The value of the token to retrieve.</param>
        /// <returns>The token entity corresponding to the provided token value.</returns>
        public async Task<Token> GetTokenAsync(string tokenValue)
        {
            // Retrieve token from database
            var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenValue == tokenValue);

            if (token != null)
            {
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == token.UserID);

                if (user != null)
                {
                    var principal = new UserPrincipal(user.Username);
                    if (user.Role != null)
                    {
                        principal.Roles.Add(user.Role.RoleName); // Assuming RoleName is stored in Role entity
                    }
                    // Set principal for current request context
                    Thread.CurrentPrincipal = principal;
                    return token;
                }
            }

            return null;
        }
        /// <summary>
        /// Retrieves the roles associated with a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of roles assigned to the user.</returns>
        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            return await _context.Roles
                                   .Where(r => r.Users.Any(u => u.UserID == userId))
                                   .ToListAsync();
        }
        /// <summary>
        /// Deletes a doctor asynchronously.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor to delete.</param>
        /// <returns>Task completion.</returns>
        public async Task DeleteDoctorAsync(int doctorId)
        {
            var doctorToDelete = await _context.Doctors.FindAsync(doctorId);

            if (doctorToDelete == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");
            }

            _context.Doctors.Remove(doctorToDelete);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Retrieves a patient asynchronously by their ID.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <returns>The patient DTO corresponding to the provided ID.</returns>
        public async Task<PatientDTO> GetPatientByIdAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);

            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
            }

            return AutoMapperConfig.Mapper.Map<PatientDTO>(patient);
        }
        /// <summary>
        /// Retrieves all patients asynchronously.
        /// </summary>
        /// <returns>A list of all patients as DTOs.</returns>
        public async Task<List<PatientDTO>> GetAllPatientsAsync()
        {
            var patients = await _context.Patients.ToListAsync();
            return patients.Select(p => AutoMapperConfig.Mapper.Map<PatientDTO>(p)).ToList();
        }

        /// <summary>
        /// Creates a new patient asynchronously.
        /// </summary>
        /// <param name="patient">The patient DTO containing information of the new patient.</param>
        /// <returns>The created patient DTO.</returns>
        public async Task<PatientDTO> CreatePatientAsync(PatientDTO patient)
        {
            var newPatient = AutoMapperConfig.Mapper.Map<Patient>(patient);

            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<PatientDTO>(newPatient);
        }
        /// <summary>
        /// Updates an existing patient asynchronously.
        /// </summary>
        /// <param name="patientId">The ID of the patient to update.</param>
        /// <param name="patient">The patient DTO containing updated information.</param>
        /// <returns>The updated patient DTO.</returns>
        public async Task<PatientDTO> UpdatePatientAsync(int patientId, PatientDTO patient)
        {
            var patientToUpdate = await _context.Patients.FindAsync(patientId);

            if (patientToUpdate == null)
            {
                throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
            }

            // Update properties based on patient DTO
            patientToUpdate.FirstName = patient.FirstName;
            patientToUpdate.LastName = patient.LastName;
            patientToUpdate.DateOfBirth = patient.DateOfBirth;
            patientToUpdate.Gender = patient.Gender;
            patientToUpdate.ContactNumber = patient.ContactNumber;
            patientToUpdate.Email = patient.Email;
            patientToUpdate.Address = patient.Address;

            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<PatientDTO>(patientToUpdate);
        }
        /// <summary>
        /// Deletes a patient asynchronously.
        /// </summary>
        /// <param name="patientId">The ID of the patient to delete.</param>
        /// <returns>Task completion.</returns>
        public async Task DeletePatientAsync(int patientId)
        {
            var patientToDelete = await _context.Patients.FindAsync(patientId);

            if (patientToDelete == null)
            {
                throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
            }

            _context.Patients.Remove(patientToDelete);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Retrieves the ID of a user associated with a token asynchronously.
        /// </summary>
        /// <param name="token">The token value associated with the user.</param>
        /// <returns>The ID of the user corresponding to the provided token.</returns>
        public async Task<int?> GetUserIdByTokenAsync(string token)
        {
            var userId = await _context.Users
                .Where(u => u.Tokens.Any(t => t.TokenValue == token))
                .Select(u => u.UserID)
                .FirstOrDefaultAsync();

            return userId;
        }
        /// <summary>
        /// Retrieves doctor appointment billing information asynchronously with optional filters and pagination.
        /// </summary>
        /// <param name="doctorName">Optional. Filter by doctor's first or last name.</param>
        /// <param name="patientName">Optional. Filter by patient's first or last name.</param>
        /// <param name="appointmentType">Optional. Filter by appointment type.</param>
        /// <param name="paymentStatus">Optional. Filter by payment status.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page for pagination.</param>
        /// <returns>A list of doctor appointment billing DTOs matching the filters and pagination criteria.</returns>

        public async Task<List<UserDoctorAppointmentBillingDTO>> GetUserDoctorAppointmentBillingAsync(
    string doctorName, string patientName, string appointmentType, string paymentStatus, int page, int pageSize)
        {
            try
            {
                var query = _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Include(a => a.Billings) // Include Billings if it's a collection
                    .Select(a => new UserDoctorAppointmentBillingDTO
                    {
                        AppointmentID = a.AppointmentID,
                        PatientID = a.PatientID,
                        PatientFirstName = a.Patient.FirstName,
                        PatientLastName = a.Patient.LastName,
                        DoctorID = a.DoctorID,
                        DoctorFirstName = a.Doctor.FirstName,
                        DoctorLastName = a.Doctor.LastName,
                        AppointmentDateTime = a.AppointmentDateTime,
                        AppointmentType = a.AppointmentType,
                        Status = a.Status,
                        Notes = a.Notes,
                        // Retrieve Billing information using navigation property
                        BillingID = a.Billings.OrderBy(b => b.BillingDateTime).Select(b => b.BillingID).FirstOrDefault(),
                        BillingDateTime = a.Billings.OrderBy(b => b.BillingDateTime).Select(b => b.BillingDateTime).FirstOrDefault(),
                        Amount = a.Billings.OrderBy(b => b.BillingDateTime).Select(b => b.Amount).FirstOrDefault(),
                        PaymentStatus = a.Billings.OrderBy(b => b.BillingDateTime).Select(b => b.PaymentStatus).FirstOrDefault(),
                        PaymentMethod = a.Billings.OrderBy(b => b.BillingDateTime).Select(b => b.PaymentMethod).FirstOrDefault()
                    });

                // Apply filters
                if (!string.IsNullOrEmpty(doctorName))
                {
                    query = query.Where(a => (a.DoctorFirstName + " " + a.DoctorLastName).Contains(doctorName));
                }

                if (!string.IsNullOrEmpty(patientName))
                {
                    query = query.Where(a => (a.PatientFirstName + " " + a.PatientLastName).Contains(patientName));
                }

                if (!string.IsNullOrEmpty(appointmentType))
                {
                    query = query.Where(a => a.AppointmentType.Contains(appointmentType));
                }

                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    query = query.Where(a => a.PaymentStatus.Contains(paymentStatus));
                }

                // Apply pagination after all filtering and ordering
                query = query.OrderBy(a => a.AppointmentDateTime)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize);

                return await query.ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log the exception with more context
                Console.WriteLine($"An error occurred while fetching the appointments: {ex.Message}");
                throw; // Re-throw the exception to propagate it up the call stack
            }
        }

        /// <summary>
        /// Calculates the total count of doctors with an optional specialization filter.
        /// </summary>
        /// <param name="specialization">The specialization to filter doctors by. If null or empty, no filtering is applied.</param>
        /// <returns>The total count of doctors matching the provided criteria.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while counting doctors.</exception>
        public async Task<int> GetTotalDoctorsCountAsync(string specialization)
        {
            try
            {
                IQueryable<Doctor> query = _context.Doctors;

                if (!string.IsNullOrEmpty(specialization))
                {
                    query = query.Where(d => d.Specialization.ToLower().Contains(specialization.ToLower()));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while counting doctors: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw; // Re-throw the exception to be handled by the controller
            }
        }
        /// <summary>
        /// Retrieves a list of doctors with an optional specialization filter and pagination.
        /// </summary>
        /// <param name="specialization">The specialization to filter doctors by. If null or empty, no filtering is applied.</param>
        /// <param name="page">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
        /// <returns>A list of doctors matching the provided criteria.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching doctors.</exception>
        public async Task<List<DoctorDTO>> GetDoctorsWithSpecializationAsync(string specialization, int page = 1, int pageSize = 10)
        {
            try
            {
                Console.WriteLine($"Starting GetDoctorsWithSpecializationAsync with specialization: {specialization}, page: {page}, pageSize: {pageSize}");

                // Ensure _context is not null and properly configured
                if (_context == null)
                {
                    Console.WriteLine("_context is null");
                    throw new Exception("_context is not initialized.");
                }

                IQueryable<Doctor> query = _context.Doctors;

                if (!string.IsNullOrEmpty(specialization))
                {
                    query = query.Where(d => d.Specialization.ToLower().Contains(specialization.ToLower()));
                }

                // Order the query by DoctorID (or another suitable property)
                query = query.OrderBy(d => d.DoctorID);

                var doctors = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Console.WriteLine($"Fetched {doctors.Count} doctors");

                var doctorDTOs = doctors.Select(d => AutoMapperConfig.Mapper.Map<DoctorDTO>(d)).ToList();
                Console.WriteLine($"Mapped {doctorDTOs.Count} doctors to DTOs");

                return doctorDTOs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching doctors: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw; // Re-throw the exception to be handled by the controller
            }
        }
    }
}