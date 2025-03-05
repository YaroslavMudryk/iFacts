using Extensions.DeviceDetector.Models;
using iFacts.Data.Audit;
using System.ComponentModel.DataAnnotations;

namespace iFacts.Data.Entities;

public class Session : AuditableSoftDeletedBaseModelWithIdentity<Guid>
{
    public AppModel App { get; set; }
    public ClientInfo Client { get; set; }
    public Location Location { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public string Type { get; set; }
    public bool ViaMFA { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public Guid? DeactivatedBySessionId { get; set; }
    [StringLength(5000, MinimumLength = 5)]
    public string RefreshToken { get; set; }
    public DateTime ExpiredAt { get; set; }
    public string PushFcmToken { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}

public class AppModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public string Image { get; set; }
}

public class Location
{
    public string Country { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string Provider { get; set; }
    public string IP { get; set; }
}
