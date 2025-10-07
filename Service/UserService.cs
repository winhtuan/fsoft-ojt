using System;
using System.Threading.Tasks;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Repository;
using Plantpedia.ViewModel;

namespace Plantpedia.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserAccount?> LoginAsync(string username, string password)
        {
            LoggerHelper.Info($"Login attempt for username: '{username}'.");
            try
            {
                var userLoginData = await _userRepository.GetUserLoginDataByUsernameAsync(username);

                if (userLoginData == null)
                {
                    LoggerHelper.Warn($"Login failed for '{username}': User not found.");
                    return null;
                }

                bool isPasswordValid = PasswordHelper.VerifyPassword(
                    password,
                    userLoginData.PasswordHash,
                    userLoginData.PasswordSalt
                );

                if (!isPasswordValid)
                {
                    LoggerHelper.Warn($"Login failed for '{username}': Invalid password.");
                    return null;
                }

                LoggerHelper.Info(
                    $"User '{username}' authenticated successfully. Updating last login time."
                );
                userLoginData.LastLoginAt = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync();

                LoggerHelper.Info($"User '{username}' logged in successfully.");
                return userLoginData.User;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"An unexpected error occurred during login for username: '{username}'."
                );
                throw;
            }
        }

        public async Task<UserAccount?> GetUserByIdAsync(int userId)
        {
            LoggerHelper.Info($"Attempting to retrieve user by ID: {userId}.");
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    LoggerHelper.Warn($"No user found for ID: {userId}.");
                }
                else
                {
                    LoggerHelper.Info($"Successfully retrieved user for ID: {userId}.");
                }
                return user;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"An error occurred while retrieving user by ID: {userId}.");
                throw;
            }
        }

        public async Task<UserAccount?> GetUserByUsernameAsync(string username)
        {
            LoggerHelper.Info($"Attempting to retrieve user by username: '{username}'.");
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    LoggerHelper.Warn($"No user found for username: '{username}'.");
                }
                else
                {
                    LoggerHelper.Info($"Successfully retrieved user for username: '{username}'.");
                }
                return user;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"An error occurred while retrieving user by username: '{username}'."
                );
                throw;
            }
        }

        public async Task<bool> UpdateProfileAsync(
            int userId,
            string lastName,
            char gender,
            DateTime dateOfBirth,
            string avatarUrl
        )
        {
            LoggerHelper.Info($"Attempting to update profile for user ID: {userId}.");
            try
            {
                var user = await _userRepository.GetUserAccountByIdAsync(userId);
                if (user == null)
                {
                    LoggerHelper.Warn($"Profile update failed: User with ID {userId} not found.");
                    return false;
                }

                user.LastName = lastName;
                user.Gender = gender;
                user.DateOfBirth = dateOfBirth;
                user.AvatarUrl = avatarUrl;

                await _userRepository.SaveChangesAsync();
                LoggerHelper.Info($"Successfully updated profile for user ID: {userId}.");
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"An error occurred while updating profile for user ID: {userId}."
                );
                throw;
            }
        }

        public async Task<UserAccount> RegisterAsync(RegisterViewModel model)
        {
            return await _userRepository.RegisterNewUserAsync(
                model.Name,
                model.Email,
                model.Password
            );
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.IsGmailExistsAsync(email);
        }
    }
}
