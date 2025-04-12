﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using App1.Models;
using Windows.System;

namespace App1.Repositories
{
    public class UserRepo : IUserRepository
    {
        private readonly List<User> _usersList;

        public UserRepo()
        {
            List<Role> basicUserRoles = new List<Role>
            {
                new Role(RoleType.User, "user")
            };
            List<Role> adminRoles = new List<Role>
            {
                new Role(RoleType.User, "user"),
                new Role(RoleType.Admin, "admin")
            };
            List<Role> managerRoles = new List<Role>
            {
                new Role(RoleType.User, "user"),
                new Role(RoleType.Admin, "admin"),
                new Role(RoleType.Manager, "manager")
            };
            List<Role> bannedUserRoles = new List<Role>
            {
                new Role(RoleType.Banned, "banned")
            };
            _usersList = new List<User>
            {
                new User(
                    userId: 1,
                    emailAddress: "bianca.georgiana.cirnu@gmail.com",
                    fullName: "Bianca Georgiana Cirnu",
                    numberOfDeletedReviews: 2,
                    HasSubmittedAppeal: true,
                    assignedRoles: basicUserRoles
                    ),
                new User(
                    userId: 2,
                    emailAddress: "alexiabortos@gmail.com",
                    fullName: "Alexia Bortos",
                    numberOfDeletedReviews: 2,
                    HasSubmittedAppeal: true,
                    assignedRoles: adminRoles
                ),
            };
        }

        public List<User> GetUsersWhoHaveSubmittedAppeals()
        {
            try
            {
                return _usersList.Where(user => user.HasSubmittedAppeal).ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve users who have submitted appeals.", ex);
            }
        }

        public List<User> GetUsersByRoleType(RoleType roleType)
        {
            try
            {
                return _usersList.Where(user => user.AssignedRoles.Any(role => role.RoleType == roleType)).ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve users with role type '{roleType}'.", ex);
            }
        }

        public RoleType GetHighestRoleTypeForUser(int userId)
        {
            try
            {
                User user = _usersList.FirstOrDefault(user => user.UserId == userId);

                if (user == null)
                {
                    throw new ArgumentException($"No user found with ID {userId}");
                }

                if (user.AssignedRoles.Count == 0)
                {
                    return RoleType.Banned;
                }

                return user.AssignedRoles.Max(role => role.RoleType);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve the highest role type for user with ID {userId}.", ex);
            }
        }

        public User GetUserByID(int userId)
        {
            try
            {
                User user = _usersList.FirstOrDefault(user => user.UserId == userId);
                if (user == null)
                {
                    throw new ArgumentException($"No user found with ID {userId}");
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve user with ID {userId}.", ex);
            }
        }

        public List<User> GetBannedUsersWhoHaveSubmittedAppeals()
        {
            try
            {
                return _usersList.Where(user => user.HasSubmittedAppeal && user.AssignedRoles.Any(role => role.RoleType == RoleType.Banned)).ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve banned users who have submitted appeals.", ex);
            }
        }
        public void AddRoleToUser(int userId, Role roleToAdd)
        {
            try
            {
                User user = _usersList.FirstOrDefault(user => user.UserId == userId);
                if (user == null)
                {
                    throw new ArgumentException($"No user found with ID {userId}");
                }
                user.AssignedRoles.Add(roleToAdd);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to add role to user with ID {userId}.", ex);
            }
        }
        public List<User> GetAllUsers()
        {
            try
            {
                return _usersList;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve all users.", ex);
            }
        }

        public class RepositoryException : Exception
        {
            public RepositoryException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}