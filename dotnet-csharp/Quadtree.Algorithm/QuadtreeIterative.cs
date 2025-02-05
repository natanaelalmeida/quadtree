namespace Quadtree;

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