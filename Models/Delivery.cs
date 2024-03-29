namespace RouteAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Delivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "NVARCHAR(50)")]        
        public string deliveryId { get; set; }
        public DateTime transDate { get; set; }
        public int quantity { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string status { get; set; }
        // public Car Car {get; set;}
        public Customer Customer {get; set;}
        //public string gps { get; set; }
        public string cusCode { get; set; }
        public string truckCode {get; set;}
        //public string deliveryOrder {get; set;}
        public string trip { get; set; }
        public string reason { get; set; }
        public Truck Truck { get; set; }
        public string giveback { get; set; }
        public string coupon { get; set; }
    }
}