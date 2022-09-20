﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Identity;

namespace SelfIdent;

/// <summary>
/// Represents a user in the public scope.
/// </summary>
public class User
{
    /// <summary>
    /// Unique ID
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// User Email
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// Username
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Locked out for some reason (Banned/Too many failed attempts etc)
    /// </summary>
    public bool Locked { get; set; }
    /// <summary>
    /// Key to use to unlock this user, if Locked == true
    /// </summary>
    public string? LockKey { get; set; }
    /// <summary>
    /// Date and Time of registration
    /// </summary>
    public DateTime RegistrationTime { get; set; }
    /// <summary>
    /// Date and Time of last login
    /// </summary>
    public DateTime LastLogin { get; set; }
    /// <summary>
    /// All Roles the User belongs to
    /// </summary>
    public List<Roles.Role> Roles { get; set; }

    public User()
    {
        // For Model Binding
    }

    internal User(UserIdentity identity)
    {
        this.Id = identity.Id;
        this.Email = identity.Email ?? String.Empty;
        this.Name = identity.Name ?? String.Empty;
        this.RegistrationTime = identity.AccountData.RegistrationTime ?? DateTime.MinValue;
        this.Locked = identity.AccountData.Locked;
        this.LastLogin = identity.AccountData.LastLogin ?? DateTime.MinValue;
        this.Roles = identity.Roles;
        this.LockKey = identity.AccountData.LockKey;
    }
}
