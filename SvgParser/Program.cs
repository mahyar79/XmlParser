using System.Numerics;
using System.Windows.Shapes;

class Program
{
    static void Main(string[] args)
    {
        string svgFilePath = "E:\\Downloads\\Test.svg"; 
        SvgParser parser = new SvgParser(svgFilePath);

        if (parser.LoadSvg())
        {
            List<Shape> shapes = parser.ParseShapes();
            shapes = parser.ConvertToMillimeters(shapes);

            foreach (Shape shape in shapes)
            {
                Console.WriteLine($"[RESULT] Shape ID: {shape.Id}, Type: {(shape.IsPolygon ? "Polygon" : "Path")}");
                foreach (Vector2 vertex in shape.Vertices)
                    Console.WriteLine($"  ({vertex.X:F2}, {vertex.Y:F2}) mm");
            }

            SaveShapesToFile(shapes, "output_shapes.txt");

        }



        static void SaveShapesToFile(List<Shape> shapes, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Shape shape in shapes)
                {
                    writer.WriteLine($"Shape ID: {shape.Id}, Class: {shape.Class}, IsPolygon: {shape.IsPolygon}");
                    writer.WriteLine("Vertices (in mm):");
                    foreach (Vector2 vertex in shape.Vertices)
                    {
                        writer.WriteLine($"  ({vertex.X:F2}, {vertex.Y:F2})");
                    }
                    writer.WriteLine();
                }
            }
            Console.WriteLine($"Shapes saved to {filePath}");
        }
    }
}
