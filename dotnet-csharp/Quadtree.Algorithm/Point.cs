namespace Quadtree;

public readonly record struct Point(float X, float Y);

public readonly record struct Rectangle(float X, float Y, float Width, float Height) {
    public bool Contains(Point point) {
        return point.X >= X && point.X < X + Width &&
               point.Y >= Y && point.Y < Y + Height;
    }

    public bool Intersects(Rectangle other) {
        return !(other.X > X + Width ||
                 other.X + other.Width < X ||
                 other.Y > Y + Height ||
                 other.Y + other.Height < Y);
    }
}

public interface IQuadtree {
    bool Insert(Point point);
    List<Point> Query(Rectangle range, List<Point>? found = null);
}



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

public class QuadtreeIterative {
    private readonly int _capacity;
    private readonly Rectangle _boundary;
    private readonly Point[] _points;
    private int _pointCount;
    private bool _divided;

    private QuadtreeIterative _northWest, _northEast, _southWest, _southEast;

    public QuadtreeIterative(Rectangle boundary, int capacity) {
        _boundary = boundary;
        _capacity = capacity;
        _points = new Point[capacity];
        _pointCount = 0;
        _divided = false;
    }

    public bool Insert(Point point) {
       var stack = new Stack<QuadtreeIterative>();
        stack.Push(this);

        while (stack.Count > 0) {
            var current = stack.Pop();

            if (!current._boundary.Contains(point)) continue;

            if (current._pointCount < current._capacity) {
                current._points[current._pointCount++] = point;
                return true;
            }

            if (!current._divided) current.Subdivide();

            stack.Push(current._northWest);
            stack.Push(current._northEast);
            stack.Push(current._southWest);
            stack.Push(current._southEast);
        }

        return false;
    }

    public List<Point> Query(Rectangle range, List<Point>? found = null) {
        found ??= [];

        if (!_boundary.Intersects(range)) return found;

        var stack = new Stack<QuadtreeIterative>();
        stack.Push(this);

        while (stack.Count > 0) {
            var current = stack.Pop();

            for (var i = 0; i < current._pointCount; i++) {
                var point = current._points[i];
                if (range.Contains(point)) {
                    found.Add(point);
                }
            }

            if (!current._divided) continue;

            if (current._northWest._boundary.Intersects(range)) stack.Push(current._northWest);
            if (current._northEast._boundary.Intersects(range)) stack.Push(current._northEast);
            if (current._southWest._boundary.Intersects(range)) stack.Push(current._southWest);
            if (current._southEast._boundary.Intersects(range)) stack.Push(current._southEast);
        }

        return found;
    }
    
    private void Subdivide() {
        var x = _boundary.X;
        var y = _boundary.Y;
        var w = _boundary.Width / 2;
        var h = _boundary.Height / 2;

        var ne = new Rectangle(x + w, y - h, w, h);
        _northEast = new QuadtreeIterative(ne, _capacity);

        var nw = new Rectangle(x, y, w, h);
        _northWest = new QuadtreeIterative(nw, _capacity);

        var se = new Rectangle(x + w, y + h, w, h);
        _southEast = new QuadtreeIterative(se, _capacity);

        var sw = new Rectangle(x, y + h, w, h);
        _southWest = new QuadtreeIterative(sw, _capacity);

        _divided = true;
    }
}