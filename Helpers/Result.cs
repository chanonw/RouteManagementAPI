namespace RouteAPI.Helpers
{
    public class Result
    {
        public double totalDistance { get; set; }
        public int days { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public int seconds { get; set; }
        public Result(double totalDistance, int days, int hours, int minutes, int seconds)
        {
            this.totalDistance = totalDistance;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
        }
    }
}