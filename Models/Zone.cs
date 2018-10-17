namespace RouteAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Zone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "NVARCHAR(50)")]
        public string zoneId { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string zoneName { get; set; }
        //public ICollection<Car> cars {get; set;}
        //public virtual Customer customer {get; set;}
        public string warehouseId { get; set; }
    }
}