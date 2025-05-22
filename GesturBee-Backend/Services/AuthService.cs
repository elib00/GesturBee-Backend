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

            //create a roadmap progress that defaults to stage 1 level 1
            RoadmapProgress roadmapProgress = new RoadmapProgress
            {
                UserId = newUser.Id,
            };

            await _authRepository.CreateRoadmapProgress(roadmapProgress);

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

        public async Task<ApiResponseDTO<UserDetailsDTO>> ProcessGoogleAuth(Dictionary<string, string> userInfo)
        {
            string userEmail = userInfo["Email"];
            string userFirstName = userInfo["FirstName"];
            string userLastName = userInfo["LastName"];

            User existingUser = await _authRepository.GetUserByEmail(userEmail);

            //log in immediately
            // user already has a local account
            if(existingUser != null)
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
                        LastLogin = existingUser.LastLogin
                    }
                };
            }

            //no local acc, we create one
            User newUser = ConstructUser(new UserRegistrationDTO
            {
                Email = userEmail,
                FirstName = userFirstName,
                LastName = userLastName,
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
                }
            };
        }

        public async Task<ApiResponseDTO<UserDetailsDTO>> ProcessFacebookAuth(FacebookUserInfoDTO userInfo)
        {
            string userEmail = userInfo.Email;
            string[] nameParts = userInfo.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string userFirstName = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : nameParts.FirstOrDefault() ?? "";
            string userLastName = nameParts.Length > 1 ? nameParts.Last() : "";

            User existingUser = await _authRepository.GetUserByEmail(userEmail);

            //log in immediately
            // user already has a local account
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
                    }
                };
            }

            //no local acc, we create one
            User newUser = ConstructUser(new UserRegistrationDTO
            {
                Email = userEmail,
                FirstName = userFirstName,
                LastName = userLastName,
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
                    LastLogin = userFromDb.LastLogin
                }
            };
        }

        public async Task<ApiResponseDTO> ResetPassword(ResetPasswordDTO resetDetails)
        {
            string email = resetDetails.Email;
            string newPassword = resetDetails.NewPassword;
            string confirmPassword = resetDetails.NewPassword;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                return new ApiResponseDTO
                {
                    Success = false,
                    ResponseType = ResponseType.MissingInput,
                    Data = null
                };

            if(newPassword != confirmPassword)
            {
                return new ApiResponseDTO
                {
                    Success = false,
                    ResponseType = ResponseType.ResetPasswordMismatch
                };
            }


            //hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _authRepository.ResetPassword(email, hashedPassword);

            return new ApiResponseDTO
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
