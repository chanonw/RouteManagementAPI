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
        public string driverName { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string status { get; set; }
        public string zoneId { get; set; }
        //public ICollection<Delivery> deliveries { get; set; }
    }
}