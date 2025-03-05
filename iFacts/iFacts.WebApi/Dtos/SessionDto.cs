using Extensions.DeviceDetector.Models;
using iFacts.Data.Entities;

namespace iFacts.WebApi.Dtos;

public class SessionDto
{
    public string Id { get; set; }
    public AppModel App { get; set; }
    public ClientInfo Device { get; set; }
    public Location Location { get; set; }
    public bool IsCurrent { get; set; }
}
