namespace Quadtree.Tests;

public class QuadtreeTests {
    private readonly Quadtree _quadtree;
    // private readonly QuadtreeSpan _quadtree;
    //private readonly QuadtreeIterative _quadtree;

    public QuadtreeTests() {
        var boundary = new Rectangle(0, 0, 100, 100);
        _quadtree = new Quadtree(boundary, 4);
        // _quadtree = new QuadtreeSpan(boundary, 4);
        // _quadtree = new QuadtreeIterative(boundary, 4);
    }
    
    [Fact]
    public void Insert_PointWithinBoundary_ShouldReturnTrue() {
        var point = new Point(50, 50);

        var result = _quadtree.Insert(point);

        Assert.True(result);
    }

    [Fact]
    public void Insert_PointOutsideBoundary_ShouldReturnFalse() {
        var point = new Point(150, 150);

        var result = _quadtree.Insert(point);

        Assert.False(result);
    }

    [Fact]
    public void Query_RangeWithinBoundary_ShouldReturnPoints() {
        var points = new List<Point> {
            new Point(10, 10),
            new Point(20, 20),
            new Point(30, 30)
        };

        foreach (var point in points) {
            _quadtree.Insert(point);
        }

        var range = new Rectangle(0, 0, 50, 50);
        var found = new List<Point>();
        _quadtree.Query(range, found);

        Assert.Equal(3, found.Count);
    }

    [Fact]
    public void Query_ShouldReturnExpectedPoints() {
        var points = new List<Point>() {
            new Point(10, 10), new Point(20, 20), new Point(30, 30), new Point(40, 40),
            new Point(50, 50), new Point(60, 60), new Point(70, 70), new Point(80, 80),
            new Point(90, 90), new Point(25, 25)
        };
        
        foreach (var point in points) {
            _quadtree.Insert(point);
        }
        
        var range = new Rectangle(20, 20, 30, 30);
        var found = _quadtree.Query(range);

        var expected = points.Where(point => range.Contains(point));
        
        Assert.Equal(expected, found);
    }
    
    [Fact]
    public void Query_RangeOutsideBoundary_ShouldReturnNoPoints() {
        var points = new List<Point> {
            new Point(10, 10),
            new Point(20, 20),
            new Point(30, 30)
        };

        foreach (var point in points) {
            _quadtree.Insert(point);
        }

        var range = new Rectangle(150, 150, 50, 50);
        var found = new List<Point>();
        _quadtree.Query(range, found);

        Assert.Empty(found);
    }
}