using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gentech_services.Data;
using gentech_services.Models;
using gentech_services.Repositories;

namespace gentech_services.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string username, string pin)
        {
            return await _userRepository.AuthenticateAsync(username, pin);
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> CreateUserAsync(string fullName, string username, string pin, string role)
        {
            // Validate username doesn't exist
            if (await _userRepository.UsernameExistsAsync(username))
            {
                throw new InvalidOperationException($"Username '{username}' already exists.");
            }

            var user = new User
            {
                FullName = fullName,
                Username = username,
                Pin = pin,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            return await _userRepository.AddAsync(user);
        }

        public async Task<User> UpdateUserAsync(int userId, string fullName, string username, string pin, string role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            // Check if username is being changed and if it already exists
            if (user.Username != username && await _userRepository.UsernameExistsAsync(username))
            {
                throw new InvalidOperationException($"Username '{username}' already exists.");
            }

            user.FullName = fullName;
            user.Username = username;
            user.Pin = pin;
            user.Role = role;

            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task DeactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
        }

        public async Task ActivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            user.IsActive = true;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            await _userRepository.DeleteAsync(user);
        }
    }
}
