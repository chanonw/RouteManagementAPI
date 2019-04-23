namespace RouteAPI.Dtos
{
    public class CustomerForUpdateDto
    {
        public string cusCode { get; set; }
        public string zoneId { get; set; }
        public string gps { get; set; }
        public string cusCond { get; set; }
        public string cusType { get; set; }
        public string day { get; set; }
        public double distanceToWh { get; set; }
    }
}