using System.ComponentModel.DataAnnotations;

namespace RouteAPI.Dtos
{
    public class DriverForNewDto
    {
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        public string drivingLicenseNo { get; set; }
        [Required]
        public string carLicenseNo { get; set; }
        public string zoneId {get; set;}
    }
}