namespace Quadtree;

public interface IQuadtree {
    bool Insert(Point point);
    List<Point> Query(Rectangle range, List<Point>? found = null);
}