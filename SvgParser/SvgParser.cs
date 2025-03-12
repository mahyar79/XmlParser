using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public List<ShapeInfo> ParseShapes()
    {
        List<ShapeInfo> shapes = new List<ShapeInfo>();

        if (_svgDocument == null)
        {
            Console.WriteLine("[Error] SVG document is not loaded.");
            return shapes;
        }

        // Track used IDs to avoid duplicates
        HashSet<int> usedIds = new HashSet<int>();

        // Extract polygons
        foreach (var element in _svgDocument.GetElementsByTagName("polygon"))
        {
            if (element is SvgPolygonElement polygonElement)
            {
                ShapeInfo shape = new ShapeInfo
                {
                    Id = GenerateUniqueId(polygonElement.Id, usedIds),
                    Class = polygonElement.GetAttribute("class"),
                    IsPolygon = true
                };

                // Convert ISvgPointList to List<Vector2>
                for (uint i = 0; i < polygonElement.Points.NumberOfItems; i++)
                {
                    var point = polygonElement.Points.GetItem(i);
                    shape.Vertices.Add(new Vector2((float)point.X, (float)point.Y));
                }

                // Validate shape (minimum 4 vertices, distinct points)
                if (shape.Vertices.Count >= 4 && HasDistinctVertices(shape, 0.1f))
                {
                    Console.WriteLine($"[INFO] Polygon parsed: {shape.Id} with {shape.Vertices.Count} vertices.");
                    shapes.Add(shape);
                }
                else
                {
                    Console.WriteLine($"[Warning] Skipping degenerate polygon {shape.Id} with {shape.Vertices.Count} vertices.");
                }
            }
        }

        // Extract paths
        foreach (var element in _svgDocument.GetElementsByTagName("path"))
        {
            if (element is SvgPathElement pathElement && pathElement.PathSegList.NumberOfItems > 0)
            {
                ShapeInfo shape = new ShapeInfo
                {
                    Id = GenerateUniqueId(pathElement.Id, usedIds),
                    Class = pathElement.GetAttribute("class"),
                    IsPolygon = false
                };

                ParsePathData(pathElement, shape);

                if (shape.Vertices.Count > 1) // Ensure path has enough points
                {
                    Console.WriteLine($"[INFO] Path parsed: {shape.Id} with {shape.Vertices.Count} points.");
                    shapes.Add(shape);
                }
                else
                {
                    Console.WriteLine($"[Warning] Skipping path {shape.Id} with insufficient points ({shape.Vertices.Count}).");
                }
            }
        }

        // Deduplicate shapes
        shapes = DeduplicateShapes(shapes);

        return shapes;
    }

    private int GenerateUniqueId(string elementId, HashSet<int> usedIds)
    {
        if (int.TryParse(elementId, out int parsedId) && !usedIds.Contains(parsedId))
        {
            usedIds.Add(parsedId);
            return parsedId;
        }

        // Generate a unique ID
        int newId;
        do
        {
            //newId = (int)(HashCode.Combine(elementId, DateTime.Now.Ticks) % 4000);
            newId = Math.Abs(HashCode.Combine(elementId, DateTime.Now.Ticks) % 4000);

        } while (usedIds.Contains(newId));
        usedIds.Add(newId);
        return newId;
    }

    private bool HasDistinctVertices(ShapeInfo shape, float minDistance)
    {
        for (int i = 0; i < shape.Vertices.Count - 1; i++)
        {
            for (int j = i + 1; j < shape.Vertices.Count; j++)
            {
                if (Vector2.Distance(shape.Vertices[i], shape.Vertices[j]) < minDistance)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private List<ShapeInfo> DeduplicateShapes(List<ShapeInfo> shapes)
    {
        return shapes
            .GroupBy(s => new { s.Id, s.Class, s.IsPolygon, Vertices = string.Join(",", s.Vertices) })
            .Select(g => g.First())
            .ToList();
    }

    private void ParsePathData(SvgPathElement path, ShapeInfo shape)
    {
        try
        {
            PathHandler handler = new PathHandler(shape);
            SvgPathParser parser = new SvgPathParser(handler);
            parser.Parse(path.GetAttribute("d")); // Use the 'd' attribute directly
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Warning] Error parsing path {shape.Id}: {ex.Message}");
            shape.Vertices.Clear(); // Clear any partial data
        }
    }

    public List<ShapeInfo> ConvertToMillimeters(List<ShapeInfo> shapes)
    {
        // Convert to millimeters
        foreach (ShapeInfo shape in shapes)
        {
            for (int i = 0; i < shape.Vertices.Count; i++)
            {
                shape.Vertices[i] = new Vector2(
                    shape.Vertices[i].X * 0.3528f,
                    shape.Vertices[i].Y * 0.3528f
                );
            }
        }

        // Normalize to positive coordinates
        if (shapes.Any())
        {
            float minX = shapes.Min(s => s.Vertices.Any() ? s.Vertices.Min(v => v.X) : float.MaxValue);
            float minY = shapes.Min(s => s.Vertices.Any() ? s.Vertices.Min(v => v.Y) : float.MaxValue);

            foreach (ShapeInfo shape in shapes)
            {
                for (int i = 0; i < shape.Vertices.Count; i++)
                {
                    shape.Vertices[i] = new Vector2(
                        shape.Vertices[i].X - minX,
                        shape.Vertices[i].Y - minY
                    );
                }
            }
        }

        return shapes;
    }

    public void CheckShapeSizes(List<ShapeInfo> shapes, float maxWidth = 1000f)
    {
        foreach (ShapeInfo shape in shapes)
        {
            if (!shape.Vertices.Any()) continue;

            float minX = shape.Vertices.Min(v => v.X);
            float maxX = shape.Vertices.Max(v => v.X);
            float width = maxX - minX;

            float minY = shape.Vertices.Min(v => v.Y);
            float maxY = shape.Vertices.Max(v => v.Y);
            float height = maxY - minY;

            // Update BoundingBox for potential use in nesting
            shape.BoundingBox = new BoundingBox
            {
                X = minX,
                Y = minY,
                Width = width,
                Height = height
            };

            if (width > maxWidth)
            {
                Console.WriteLine($"[Warning] Shape {shape.Id} exceeds width limit: {width:F2} mm (height: {height:F2} mm)");
            }
        }
    }
}