using Application.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        /// <summary>
        /// Tries authentication for provided credentials.
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <param name="user">Authencticates user's object is assigned to it</param>
        /// <returns>Whether authenticated or not</returns>
        bool TryAuthentication(string email, string password, out UserDTO user);

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user">new user data</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>New user's object</returns>
        UserDTO AddNew(NewUserDTO user, int userId);

        /// <summary>
        /// Updates an user with new data
        /// </summary>
        /// <param name="user">Updated data</param>
        /// <param name="userId">Invoker user's id </param>
        /// <returns>Whether accomplished or no</returns>
        bool Update(UserEditDTO user, int userId);

        /// <summary>
        /// Gets user list
        /// </summary>
        /// <param name="skipCount">Amount of users to be skipped from beginning</param>
        /// <param name="takeCount">Amount of users to be taken</param>
        /// <param name="totalCount">Total amount of users should be assigned to this argument</param>
        /// <returns></returns>
        IEnumerable<UserDTO> GetUserList(int skipCount, int takeCount, out int totalCount);

        /// <summary>
        /// Generates and returns random password
        /// </summary>
        /// <returns></returns>
        string GeneratePassword();

        /// <summary>
        /// Creates and returns recovery token for user with specific email
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns></returns>
        string GetRecoveryToken(string email);

        /// <summary>
        /// Validates a token
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Recovers password
        /// </summary>
        /// <param name="recoverPassword">All nessecery data for password recovery</param>
        /// <returns>Whether accomplished or no</returns>
        bool RecoverPassword(RecoverPasswordDTO recoverPassword);

        /// <summary>
        /// Gets an user by id
        /// </summary>
        /// <param name="id">User's id</param>
        /// <returns></returns>
        UserDTO GetUserById(int id);

        /// <summary>
        /// Gets an user by email
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns></returns>
        UserDTO GetUserByEmail(string email);

        /// <summary>
        /// Updates an user's profile
        /// </summary>
        /// <param name="user">Updated data</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool UpdateProfile(UserProfileDTO user, int userId);

        /// <summary>
        /// Changes a password
        /// </summary>
        /// <param name="changePassword">Password changing data</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool ChangePassword(ChangePasswordDTO changePassword, int userId);

        /// <summary>
        /// Changes a password
        /// </summary>
        /// <param name="userEditId">Updating user's id</param>
        /// <param name="newPassword">New plain password</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool ChangePassword(int userEditId, string newPassword, int userId);

        /// <summary>
        /// Deletes an user
        /// </summary>
        /// <param name="id">User's id</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool Delete(int id, int userId);

        /// <summary>
        /// Activates an user
        /// </summary>
        /// <param name="id">User's id</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool Activate(int id, int userId);

        /// <summary>
        /// Deactivates an user
        /// </summary>
        /// <param name="id">User's id</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool Disactivate(int id, int userId);
    }
}
