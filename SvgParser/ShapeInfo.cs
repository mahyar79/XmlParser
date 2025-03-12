using System.Numerics;

public class ShapeInfo
{
    public int Id { get; set; }
    public string Type { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public BoundingBox BoundingBox { get; set; }
    public string Points { get; set; }
    public string Class { get; set; }
    public List<Vector2> Vertices { get; set; }
    public bool IsPolygon { get; set; }

    public ShapeInfo()
    {
        Vertices = new List<Vector2>();
        BoundingBox = new BoundingBox();
    }
}