namespace RouteAPI.Helpers
{
    public class MergeOrder
    {
      public double totalDistance { get; set; }
      public double nearestPoint { get; set; }
      public MergeOrder(double totalDistance, double nearestPoint)
      {
          this.totalDistance = totalDistance;
          this.nearestPoint = nearestPoint;
      }
    }
}