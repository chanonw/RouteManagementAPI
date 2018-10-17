namespace RouteAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Warehouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "NVARCHAR(50)")]
        public string warehouseId { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string warehouseName { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        //public ICollection<Zone> zones { get; set; }
        public string gps { get; set; }
    }
}