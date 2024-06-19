using AutoMapper;
using HealthCareAPI.Common;
using HealthCareAPI.DTOs;
using HealthCareDAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace HealthCareAPI.Helpers
{
    public class DatabaseHelper
    {
        private static DatabaseHelper _instance;
        private static readonly object _lock = new object();
        private readonly HealthCareModel _context;

        private DatabaseHelper()
        {
            _context = new HealthCareModel();
        }

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
        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.UserID == userId);
            return AutoMapperConfig.Mapper.Map<UserDTO>(user);
        }
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

        // Retrieve details of a specific appointment
        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            return AutoMapperConfig.Mapper.Map<AppointmentDTO>(appointment);
        }

        // Retrieve all appointments for the authenticated user
        public async Task<List<AppointmentDTO>> GetAllAppointmentsAsync(int userId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PatientID == userId || a.DoctorID == userId)
                .ToListAsync();

            return AutoMapperConfig.Mapper.Map<List<AppointmentDTO>>(appointments);
        }

        // Create a new appointment
        public async Task<int> CreateAppointmentAsync(AppointmentDTO newAppointment)
        {
            var appointment = AutoMapperConfig.Mapper.Map<Appointment>(newAppointment);

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return appointment.AppointmentID;
        }

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

        public async Task<List<BillingDTO>> GetBillingForUserAsync(int userId)
        {
            var billings = await _context.Billings
                                        .Where(b => b.PatientID == userId || b.DoctorID == userId)
                                        .ToListAsync();

            return billings.Select(b => AutoMapperConfig.Mapper.Map<BillingDTO>(b)).ToList();
        }

        public async Task<BillingDTO> GetBillingByIdAsync(int billingId)
        {
            var billing = await _context.Billings.FindAsync(billingId);

            if (billing == null)
            {
                throw new KeyNotFoundException($"Billing record with ID {billingId} not found.");
            }

            return AutoMapperConfig.Mapper.Map<BillingDTO>(billing);
        }

        public async Task<BillingDTO> CreateBillingAsync(BillingDTO billing)
        {
            var newBilling = AutoMapperConfig.Mapper.Map<Billing>(billing);

            _context.Billings.Add(newBilling);
            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<BillingDTO>(newBilling);
        }

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


        public async Task<UserWithRolesDTO> GetUserWithRolesByIdAsync(int userId)
        {
            var user = await _context.Set<User>()
                                     .Include(u => u.Role)
                                     .FirstOrDefaultAsync(u => u.UserID == userId);
            return AutoMapperConfig.Mapper.Map<UserWithRolesDTO>(user);
        }
        public async Task<UserWithRolesAndTokensDTO> GetUserWithRolesAndTokensByIdAsync(int userId)
        {
            var user = await _context.Set<User>()
                                     .Include(u => u.Role)
                                     .Include(u => u.Tokens)
                                     .FirstOrDefaultAsync(u => u.UserID == userId);

            return AutoMapperConfig.Mapper.Map<UserWithRolesAndTokensDTO>(user);
        }

        public async Task<List<DoctorDTO>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return doctors.Select(d => AutoMapperConfig.Mapper.Map<DoctorDTO>(d)).ToList();
        }

        public async Task<DoctorDTO> GetDoctorByIdAsync(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);

            if (doctor == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");
            }

            return AutoMapperConfig.Mapper.Map<DoctorDTO>(doctor);
        }

        public async Task<DoctorDTO> CreateDoctorAsync(DoctorDTO doctor)
        {
            var newDoctor = AutoMapperConfig.Mapper.Map<Doctor>(doctor);

            _context.Doctors.Add(newDoctor);
            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<DoctorDTO>(newDoctor);
        }

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


        public async Task<User> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Set<User>().Include(u => u.Tokens).FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var hashedPassword = ComputeSha256Hash(password);
            if (user.PasswordHash.Trim() != hashedPassword) return null;

            return user;
        }

        public async Task<Token> GenerateTokenAsync(User user)
        {
            var tokenValue = GenerateTokenValue();
            var token = new Token
            {
                UserID = user.UserID,
                TokenValue = tokenValue,
                ExpiryDateTime = DateTime.UtcNow.AddHours(1),
                TokenType = "Bearer"
            };

            _context.Tokens.Add(token);
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

        public async Task<User> GetUserByTokenAsync(string token)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Tokens)
                    .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.TokenValue == token));

                return user;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving user by token", ex);
            }
        }
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
        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            return await _context.Roles
                                   .Where(r => r.Users.Any(u => u.UserID == userId))
                                   .ToListAsync();
        }

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

        public async Task<PatientDTO> GetPatientByIdAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);

            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
            }

            return AutoMapperConfig.Mapper.Map<PatientDTO>(patient);
        }

        public async Task<List<PatientDTO>> GetAllPatientsAsync()
        {
            var patients = await _context.Patients.ToListAsync();
            return patients.Select(p => AutoMapperConfig.Mapper.Map<PatientDTO>(p)).ToList();
        }


        public async Task<PatientDTO> CreatePatientAsync(PatientDTO patient)
        {
            var newPatient = AutoMapperConfig.Mapper.Map<Patient>(patient);

            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            return AutoMapperConfig.Mapper.Map<PatientDTO>(newPatient);
        }

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

    }
}