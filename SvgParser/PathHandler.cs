using System.Collections.Generic;
using System.Numerics;
using SharpVectors.Dom.Svg;

public class PathHandler : ISvgPathHandler
{
    private readonly Shape _shape;
    private Vector2 _lastPoint = Vector2.Zero;

    public PathHandler(Shape shape)
    {
        _shape = shape;
    }

    public void StartPath() { }

    public void EndPath() { }

    public void MovetoAbs(float x, float y)
    {
        _lastPoint = new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void MovetoRel(float x, float y)
    {
        _lastPoint += new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void LinetoAbs(float x, float y)
    {
        _lastPoint = new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void LinetoRel(float x, float y)
    {
        _lastPoint += new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void CurvetoCubicAbs(float x1, float y1, float x2, float y2, float x, float y)
    {
        _lastPoint = new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void CurvetoCubicRel(float x1, float y1, float x2, float y2, float x, float y)
    {
        _lastPoint += new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void CurvetoCubicSmoothAbs(float x2, float y2, float x, float y)
    {
        _lastPoint = new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void CurvetoCubicSmoothRel(float x2, float y2, float x, float y)
    {
        _lastPoint += new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void CurvetoQuadraticAbs(float x1, float y1, float x, float y)
    {
        _lastPoint = new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void CurvetoQuadraticRel(float x1, float y1, float x, float y)
    {
        _lastPoint += new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void CurvetoQuadraticSmoothAbs(float x, float y)
    {
        _lastPoint = new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void CurvetoQuadraticSmoothRel(float x, float y)
    {
        _lastPoint += new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void LinetoHorizontalAbs(float x)
    {
        _lastPoint = new Vector2(x, _lastPoint.Y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void LinetoHorizontalRel(float x)
    {
        _lastPoint += new Vector2(x, 0);
        _shape.Vertices.Add(_lastPoint);
    }

    public void LinetoVerticalAbs(float y)
    {
        _lastPoint = new Vector2(_lastPoint.X, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void LinetoVerticalRel(float y)
    {
        _lastPoint += new Vector2(0, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void ArcAbs(float rx, float ry, float angle, bool largeArc, bool sweep, float x, float y)
    {
        _lastPoint = new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void ArcRel(float rx, float ry, float angle, bool largeArc, bool sweep, float x, float y)
    {
        _lastPoint += new Vector2(x, y);
        _shape.Vertices.Add(_lastPoint);
    }

    public void ClosePath()
    {
        if (_shape.Vertices.Count > 0)
        {
            _shape.Vertices.Add(_shape.Vertices[0]); 
        }
    }
}
