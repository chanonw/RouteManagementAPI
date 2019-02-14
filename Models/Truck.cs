using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteAPI.Models
{
    public class Truck
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "NVARCHAR(50)")]
        public string truckCode { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string firstName { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string lastName { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string drivingLicenseNo { get; set; }
        public string carLicenseNo { get; set; }
        public string status { get; set; }
        public bool personalLeave { get; set; }
        public bool sickLeave { get; set; }
        public string zoneId { get; set; }
        //public ICollection<Delivery> deliveries { get; set; }
    }
}