using System.Numerics;
using SharpVectors.Dom.Svg;

public class PathHandler : ISvgPathHandler
{
    private readonly ShapeInfo _shape;
    private Vector2 _lastPoint = Vector2.Zero;

    public PathHandler(ShapeInfo shape)
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
        // Approximate Bezier curve with linear segments
        Vector2 p0 = _lastPoint;
        Vector2 p1 = new Vector2(x1, y1);
        Vector2 p2 = new Vector2(x2, y2);
        Vector2 p3 = new Vector2(x, y);

        // Use 5 points to approximate the curve
        for (int i = 1; i <= 5; i++)
        {
            float t = i / 5.0f;
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            float xT = uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X;
            float yT = uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y;

            _shape.Vertices.Add(new Vector2(xT, yT));
        }

        _lastPoint = p3;
    }

    public void CurvetoCubicRel(float x1, float y1, float x2, float y2, float x, float y)
    {
        float absX1 = _lastPoint.X + x1;
        float absY1 = _lastPoint.Y + y1;
        float absX2 = _lastPoint.X + x2;
        float absY2 = _lastPoint.Y + y2;
        float absX = _lastPoint.X + x;
        float absY = _lastPoint.Y + y;
        CurvetoCubicAbs(absX1, absY1, absX2, absY2, absX, absY);
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
        // Simplified arc handling: add endpoint (could be improved with arc approximation)
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