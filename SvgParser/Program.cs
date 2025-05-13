using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

class Program
{
    static void Main(string[] args)
    {
        string svgFilePath = "E:\\Downloads\\Nesting files\\sample.svg";
        SvgParser parser = new SvgParser(svgFilePath);

        if (parser.LoadSvg())
        {
            List<ShapeInfo> shapes = parser.ParseShapes();
            

            // Validate shape sizes
            parser.CheckShapeSizes(shapes, 1000f);

            // Output shapes to console (limited for brevity)
            foreach (ShapeInfo shape in shapes.Take(10)) // Limit to first 10 shapes to avoid console overflow
            {
                Console.WriteLine($"[RESULT] Shape ID: {shape.Id}, Type: {(shape.IsPolygon ? "Polygon" : "Path")}");
                foreach (Vector2 vertex in shape.Vertices)
                {
                    Console.WriteLine($"  ({vertex.X:F5}, {vertex.Y:F5}) ");
                }
            }

            // Save all shapes to file
            SaveShapesToFile(shapes, "output_shapes.txt");
        }
    }

    static void SaveShapesToFile(List<ShapeInfo> shapes, string filePath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(false), 65536)) // Large buffer
            {
                foreach (ShapeInfo shape in shapes)
                {
                    writer.WriteLine($"Shape ID: {shape.Id}, Class: {shape.Class}, IsPolygon: {shape.IsPolygon}");
                    writer.Write("Vertices:");
                    //foreach (Vector2 vertex in shape.Vertices)
                    //{
                    //    writer.WriteLine($"  ({vertex.X:F2}, {vertex.Y:F2})");
                    //}
                    //writer.WriteLine();

                    string verticesLine = string.Join(";", shape.Vertices.Select(v => $"{v.X:F5},{v.Y:F5}"));
                    writer.WriteLine(verticesLine);
                    writer.WriteLine();
                }
            }
            Console.WriteLine($"Shapes saved to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to save shapes to file: {ex.Message}");
        }
    }
}