using iFacts.Data.Audit;

namespace iFacts.Data.Entities;

public class User : AuditableSoftDeletedBaseModelWithIdentity<int>
{
    public User(string firstName, string middleName, string lastName, string login)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Login = login;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime? BirthDate { get; set; }

    public string UserName { get; set; }
    public string Image { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }

    public bool CanBeBlocked { get; set; }
    public DateTime? BlockedUntil { get; set; }
    public int FailedLoginAttempts { get; set; }
    public bool Mfa { get; set; }
    public string MfaSecretKey { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public List<PasswordHistory> PasswordHistories { get; set; }
    public List<UserRole> UserRoles { get; set; }
    public List<App> Apps { get; set; }
}
