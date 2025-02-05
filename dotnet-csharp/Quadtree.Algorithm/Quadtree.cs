namespace Quadtree;

public class Quadtree : IQuadtree {
    private readonly int _capacity;
    private readonly Rectangle _boundary;
    private readonly List<Point> _points;
    private bool _divided;

    private Quadtree _northWest;
    private Quadtree _northEast;
    private Quadtree _southWest;
    private Quadtree _southEast;

    public Quadtree(Rectangle boundary, int capacity) {
        _boundary = boundary;
        _capacity = capacity;
        _points = new List<Point>(capacity);
        _divided = false;
    }

    public bool Insert(Point point) {
        if (!_boundary.Contains(point)) return false;

        if (_points.Count < _capacity) {
            _points.Add(point);
            return true;
        }

        if (!_divided) Subdivide();

        return _northWest.Insert(point) ||
               _northEast.Insert(point) ||
               _southWest.Insert(point) ||
               _southEast.Insert(point);
    }

    public List<Point> Query(Rectangle range, List<Point>? found = null) {
        found ??= [];

        if (!_boundary.Intersects(range)) return found;

        foreach (var point in _points) {
            if (range.Contains(point)) {
                found.Add(point);
            }
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
        _northEast = new Quadtree(ne, _capacity);

        var nw = new Rectangle(x, y, w, h);
        _northWest = new Quadtree(nw, _capacity);

        var se = new Rectangle(x + w, y + h, w, h);
        _southEast = new Quadtree(se, _capacity);

        var sw = new Rectangle(x, y + h, w, h);
        _southWest = new Quadtree(sw, _capacity);

        _divided = true;
    }
}