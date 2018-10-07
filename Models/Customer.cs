using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteAPI.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "NVARCHAR(50)")]
        public string cusCode { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string cusType { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string cusCond { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string title { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string firstName { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string lastName { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string houseNo { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string building { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string road { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string soi { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string subDistrict { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string district { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string city { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string portalCode { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string day { get; set; }
        public int depBottle { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string gps { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string status { get; set; }
        [ForeignKey("Zone")]
        [Column(TypeName = "NVARCHAR(50)")]
        //public string zoneId { get; set; }
        public virtual Zone zone { get; set; }
        public ICollection<Delivery> deliveries { get; set; }
    }
}