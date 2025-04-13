using GesturBee_Backend.DTO;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository;
using Microsoft.Identity.Client;

namespace GesturBee_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<ApiResponseDTO<UserDetailsDTO>> RegisterUser(UserRegistrationDTO userDetails)
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

            //construct ang user nga object
            User newUser = ConstructUser(userDetails);
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

        private User ConstructUser(UserRegistrationDTO userDetails)
        {
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
                Email = userDetails.Email,
            };


            //only for locally created nga accounts, mng hash tag password for them
            if (userDetails.Password != null)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(userDetails.Password);
            }

            //bind sa user
            User newUser = new User
            {
                Account = account,
                Profile = profile,
                LastLogin = userDetails.LastLogin
            };

            return newUser;
        }

        public async Task<ApiResponseDTO<UserDetailsDTO>> ProcessExternalAuth(Dictionary<string, string> userInfo)
        {
            string newUserEmail = userInfo["Email"];

            User existingUser = await _authRepository.GetUserByEmail(newUserEmail);

            // if the user already has a local account
            if (existingUser != null)
            {
                // update the last login of the existing user
                await _authRepository.UpdateLastLogin(existingUser);

                return new ApiResponseDTO<UserDetailsDTO>
                {
                    Success = true,
                    ResponseType = ResponseType.ValidUser,
                    Data = new UserDetailsDTO
                    {
                        Id = existingUser.Id,
                        Email = existingUser.Account.Email,
                        FirstName = existingUser.Profile.FirstName,
                        LastName = existingUser.Profile.LastName,
                        ContactNumber = existingUser.Profile.ContactNumber,
                        Gender = existingUser.Profile.Gender,
                        BirthDate = existingUser.Profile.BirthDate,
                        LastLogin = existingUser.LastLogin,
                        Roles = existingUser.Roles
                    }
                };
            }

            // if email isn't tied to a local account yet,
            // construct the new user object
            string fullName = userInfo["Name"];
            string[] nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            User newUser = ConstructUser(new UserRegistrationDTO
            {
                Email = userInfo["Email"],
                FirstName = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : nameParts[0],
                LastName = nameParts.Length > 1 ? nameParts[^1] : "",
                LastLogin = DateTime.UtcNow // set the last login to the current date time 
            });

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

            //update the last login of the user
            await _authRepository.UpdateLastLogin(userFromDb);

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

            List<string> userRoles = new List<string>(["User"]);

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

        public async Task<ApiResponseDTO<object>> ResetPassword(ResetPasswordDTO resetDetails)
        {
            string email = resetDetails.Email;
            string newPassword = resetDetails.NewPassword;
            string confirmPassword = resetDetails.NewPassword;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                return new ApiResponseDTO<object>
                {
                    Success = false,
                    ResponseType = ResponseType.MissingInput,
                    Data = null
                };

            if(newPassword != confirmPassword)
            {
                return new ApiResponseDTO<object>
                {
                    Success = false,
                    ResponseType = ResponseType.ResetPasswordMismatch
                };
            }


            //hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _authRepository.ResetPassword(email, hashedPassword);

            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.PasswordResetSuccessful
            };
        }
        private bool ValidatePassword(string password, string pwFromDb)
        {
            return BCrypt.Net.BCrypt.Verify(password, pwFromDb);
        }
    }
}
