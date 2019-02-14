using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteAPI.Models
{
    public class AdditionalTruck
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "NVARCHAR(50)")]
        public string truckCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string drivingLicenseNo { get; set; }
        public string carLicenseNo { get; set; }
        public string status { get; set; }
    }
}