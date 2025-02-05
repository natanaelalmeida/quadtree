namespace Quadtree;

public class QuadtreeSpan  {
    private readonly int _capacity;
    private readonly Rectangle _boundary;
    private readonly Point[] _points;
    private int _pointCount;
    private bool _divided;

    private QuadtreeSpan _northWest, _northEast, _southWest, _southEast;

    public QuadtreeSpan(Rectangle boundary, int capacity) {
        _boundary = boundary;
        _capacity = capacity;
        _points = new Point[capacity];
        _pointCount = 0;
        _divided = false;
    }

    public bool Insert(Point point) {
        if (!_boundary.Contains(point)) return false;

        if (_pointCount < _capacity) {
            _points[_pointCount++] = point;
            return true;
        }

        if (!_divided) Subdivide();

        return _northWest.Insert(point) ||
               _northEast.Insert(point) ||
               _southWest.Insert(point) ||
               _southEast.Insert(point);
    }

    public Span<Point> Query(Rectangle range, Span<Point> found) {
        if (!_boundary.Intersects(range)) return found;

        for (var i = 0; i < _pointCount; i++) {
            var point = _points[i];
            if (!range.Contains(point)) continue;
            found[i] = point;
        }

        if (!_divided) return found;

        _northWest.Query(range, found);
        _northEast.Query(range, found);
        _southWest.Query(range, found);
        _southEast.Query(range, found);

        return found;
    }

    private void Subdivide() {
        var x = _boundary.X;
        var y = _boundary.Y;
        var w = _boundary.Width / 2;
        var h = _boundary.Height / 2;

        var ne = new Rectangle(x + w, y - h, w, h);
        _northEast = new QuadtreeSpan(ne, _capacity);

        var nw = new Rectangle(x, y, w, h);
        _northWest = new QuadtreeSpan(nw, _capacity);

        var se = new Rectangle(x + w, y + h, w, h);
        _southEast = new QuadtreeSpan(se, _capacity);

        var sw = new Rectangle(x, y + h, w, h);
        _southWest = new QuadtreeSpan(sw, _capacity);

        _divided = true;
    }
}