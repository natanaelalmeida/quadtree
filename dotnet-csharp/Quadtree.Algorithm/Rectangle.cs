namespace Quadtree;

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