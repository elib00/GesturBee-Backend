using GesturBee_Backend.DTO;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository;

namespace GesturBee_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<ApiResponseDTO<UserDetailsDTO>> RegisterUser(UserRegistrationDTO userDetails, AuthType authType)
        {
            string newUserEmail = userDetails.Email;

            bool isExistingEmail = await _authRepository.IsExistingEmail(newUserEmail);
           
            if(isExistingEmail)
                return new ApiResponseDTO<UserDetailsDTO>
                {
                    Success = false,
                    ResponseType = ResponseType.UserAlreadyExists,
                    Data = null
                };

            //the user profile
            UserProfile profile = new UserProfile
            {
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                ContactNumber = userDetails.ContactNumber,
                Gender = userDetails.Gender,
                BirthDate = userDetails.BirthDate
            };

            //the user account
            UserAccount account = new UserAccount
            {
                Email = newUserEmail,
            };

            if(authType == AuthType.LocalAuth)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(userDetails.Password);
            }

            //bind sa user
            User newUser = new User
            {
                Account = account,
                Profile = profile
            };
            

            await _authRepository.CreateUser(newUser);

            return new ApiResponseDTO<UserDetailsDTO>
            {
                Success = true,
                ResponseType = ResponseType.UserCreated,
                Data = new UserDetailsDTO
                {
                    Id = newUser.Id,
                    Email = newUser.Account.Email,
                    FirstName = newUser.Profile.FirstName,
                    LastName = newUser.Profile.LastName,
                    ContactNumber = newUser.Profile.ContactNumber,
                    Gender = newUser.Profile.Gender,
                    BirthDate = newUser.Profile.BirthDate,
                    LastLogin = newUser.LastLogin,
                    Roles = newUser.Roles
                }
            };

        }

        public async Task<ApiResponseDTO<UserDetailsDTO>> ValidateUser(UserValidationDTO credentials)
        {
            string? email = credentials.Email;
            string? password = credentials.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new ApiResponseDTO<UserDetailsDTO>
                {
                    Success = false,
                    ResponseType = ResponseType.MissingInput,
                    Data = null
                };

            //query for the user
            User? userFromDb = await _authRepository.GetUserByEmail(email);

            if(userFromDb == null)
                return new ApiResponseDTO<UserDetailsDTO>
                {
                    Success = false,
                    ResponseType = ResponseType.UserNotFound,
                    Data = null
                };
            
            if(!ValidatePassword(password, userFromDb.Account.Password))
            {
                return new ApiResponseDTO<UserDetailsDTO>
                {
                    Success = false,
                    ResponseType = ResponseType.InvalidUser,
                    Data = null
                };
            }

            userFromDb.Roles = await GetUserRoles(userFromDb.Id);

            return new ApiResponseDTO<UserDetailsDTO>
            {
                Success = true,
                ResponseType = ResponseType.ValidUser,
                Data = new UserDetailsDTO
                {
                    Id = userFromDb.Id,
                    Email = userFromDb.Account.Email,
                    FirstName = userFromDb.Profile.FirstName,
                    LastName = userFromDb.Profile.LastName,
                    ContactNumber = userFromDb.Profile.ContactNumber,
                    Gender = userFromDb.Profile.Gender,
                    BirthDate = userFromDb.Profile.BirthDate,
                    LastLogin = userFromDb.LastLogin,
                    Roles = userFromDb.Roles
                }
            };
        }

        private async Task<List<string>> GetUserRoles(int userId)
        {
            bool isUserAStudent = await _authRepository.IsUserAStudent(userId);
            bool isUserATeacher = await _authRepository.IsUserATeacher(userId);

            List<string> userRoles = new List<string>();

            if (isUserAStudent)
            {
                userRoles.Add("Student");
            }

            if (isUserATeacher)
            {
                userRoles.Add("Teacher");
            }

            //return new ApiResponseDTO<object>
            //{
            //    Success = true,
            //    ResponseType = ResponseType.SuccessfulRetrievalOfResource,
            //    Data = new
            //    {
            //        UserRoles = userRoles
            //    }
            //};

            return userRoles;
        }

        // TODO: Implement this function
        public async Task<ApiResponseDTO<UserDetailsDTO>> FetchUserUsingEmail(string email)
        {
            User? userFromDb = await _authRepository.GetUserByEmail(email);
            if(userFromDb == null)
            {
                return new ApiResponseDTO<UserDetailsDTO>
                {
                    Success = false,
                    ResponseType = ResponseType.UserNotFound,
                    Data = null
                };
            }

            userFromDb.Roles = await GetUserRoles(userFromDb.Id);

            return new ApiResponseDTO<UserDetailsDTO>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = new UserDetailsDTO
                {
                    Id = userFromDb.Id,
                    Email = userFromDb.Account.Email,
                    FirstName = userFromDb.Profile.FirstName,
                    LastName = userFromDb.Profile.LastName,
                    ContactNumber = userFromDb.Profile.ContactNumber,
                    Gender = userFromDb.Profile.Gender,
                    BirthDate = userFromDb.Profile.BirthDate,
                    LastLogin = userFromDb.LastLogin,
                    Roles = userFromDb.Roles
                }
            };
           
        }

        private bool ValidatePassword(string password, string pwFromDb)
        {
            return BCrypt.Net.BCrypt.Verify(password, pwFromDb);
        }
    }
}
