using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

public class SvgParser
{
    private readonly string _svgFilePath;
    private SvgDocument _svgDocument;

    public SvgParser(string svgFilePath)
    {
        _svgFilePath = svgFilePath;
    }

    public bool LoadSvg()
    {
        if (!File.Exists(_svgFilePath))
        {
            Console.WriteLine($"[Error] File not found: {_svgFilePath}");
            return false;
        }

        try
        {
            // Create an SVG window to enable parsing
            WpfSvgWindow svgWindow = new WpfSvgWindow(800, 600, null);
            _svgDocument = new SvgDocument(svgWindow);
            _svgDocument.Load(_svgFilePath);
            Console.WriteLine("[INFO] SVG file loaded successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to load SVG: {ex.Message}");
            return false;
        }
    }

    public List<Shape> ParseShapes()
    {
        List<Shape> shapes = new List<Shape>();

        if (_svgDocument == null)
        {
            Console.WriteLine("[Error] SVG document is not loaded.");
            return shapes;
        }

        // Extract polygons
        foreach (var element in _svgDocument.GetElementsByTagName("polygon"))
        {
            if (element is SvgPolygonElement polygonElement)
            {
                Shape shape = new Shape
                {
                    Id = polygonElement.Id ?? "Unknown",
                    Class = polygonElement.GetAttribute("class"),
                    IsPolygon = true
                };

                // Convert ISvgPointList to List<Vector2>
                for (uint i = 0; i < polygonElement.Points.NumberOfItems; i++)
                {
                    var point = (polygonElement.Points.GetItem(i));
                    shape.Vertices.Add(new Vector2((float)point.X, (float)point.Y));
                }

                Console.WriteLine($"[INFO] Polygon parsed: {shape.Id} with {shape.Vertices.Count} vertices.");
                shapes.Add(shape);
            }
        }

        // Extract paths
        foreach (var element in _svgDocument.GetElementsByTagName("path"))
        {
            if (element is SvgPathElement pathElement && pathElement.PathSegList.NumberOfItems > 0)
            {
                Shape shape = new Shape
                {
                    Id = pathElement.Id ?? "Unknown",
                    Class = pathElement.GetAttribute("class"),
                    IsPolygon = false
                };

                ParsePathData(pathElement, shape);

                Console.WriteLine($"[INFO] Path parsed: {shape.Id} with {shape.Vertices.Count} points.");
                shapes.Add(shape);
            }
        }

        return shapes;
    }

    private void ParsePathData(SvgPathElement path, Shape shape)
    {
        try
        {
            // Correct usage: SvgPathParser requires an ISvgPathHandler
            PathHandler handler = new PathHandler(shape);
            SvgPathParser parser = new SvgPathParser(handler);
            parser.Parse(path.ToString());

            // Shape data is now in the handler
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Warning] Error parsing path {shape.Id}: {ex.Message}");
        }
    }

    public List<Shape> ConvertToMillimeters(List<Shape> shapes)
    {
        foreach (Shape shape in shapes)
        {
            for (int i = 0; i < shape.Vertices.Count; i++)
            {
                shape.Vertices[i] = new Vector2(
                    shape.Vertices[i].X * 0.3528f,
                    shape.Vertices[i].Y * 0.3528f
                );
            }
        }
        return shapes;
    }
}
