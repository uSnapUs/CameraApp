namespace Camera.Helpers
{
    public interface ILocation
    {
        string Address { get; set; }
        Coordinate Coordinate { get; set; }
    }
}