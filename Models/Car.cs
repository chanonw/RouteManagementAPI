using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteAPI.Models
{
    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "NVARCHAR(50)")]
        public string carCode { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string firstName { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string lastName { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string drivingLicenseNo { get; set; }
        public string carLicenseNo { get; set; }
        public string status { get; set; }
        public string zoneId { get; set; }
        //public ICollection<Delivery> deliveries { get; set; }
    }
}