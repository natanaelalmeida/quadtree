import matplotlib.pyplot as plt

class Point:
    def __init__(self, x, y):
        self.x = x
        self.y = y

    def __repr__(self):
        return f'Point({self.x}, {self.y})'

class Rectangle:
    def __init__(self, x, y, width, height):
        self.x = x
        self.y = y
        self.width = width
        self.height = height

    def contains(self, point):
        return (self.x <= point.x < self.x + self.width and
                self.y <= point.y < self.y + self.height)

    def intersects(self, other) -> bool:
        return not (other.x > self.x + self.width or
                other.x + other.width < self.x or
                other.y > self.y + self.height or
                other.y + other.height < self.y)

    def __repr__(self):
        return f'Rectangle({self.x}, {self.y}, {self.width}, {self.height})'

class Quadtree:
    def __init__(self, boundary: Rectangle, capacity):
        self._boundary = boundary
        self._capacity = capacity
        self._points = []
        self._divided = False

        self._north_east = None
        self._north_west = None
        self._south_east = None
        self._south_west = None

    def subdivide(self):
        x = self._boundary.x
        y = self._boundary.y
        w = self._boundary.width
        h = self._boundary.height

        half_width = w / 2
        half_height = h / 2

        # Create four children that divide the current region
        ne = Rectangle(x + half_width, y - half_height, half_width, half_height)
        self._north_east = Quadtree(ne, self._capacity)

        nw = Rectangle(x, y, half_width, half_height)
        self._north_west = Quadtree(nw, self._capacity)

        se = Rectangle(x + half_width, y + half_height, half_width, half_height)
        self._south_east = Quadtree(se, self._capacity)

        sw = Rectangle(x, y + half_height, half_width, half_height)
        self._south_west = Quadtree(sw, self._capacity)

        # Mark the current region as divided
        self._divided = True

    def insert(self, point):
        if not self._boundary.contains(point):
            return False # Point is not in the boundary

        if self.exceeds_capacity:
            self._points.append(point)
            return True

        if not self._divided:
            self.subdivide() # Subdivide the region if it hasn't been divided yet

        # Try to insert the point into the children
        return (self._north_east.insert(point) or
                self._north_west.insert(point) or
                self._south_east.insert(point) or
                self._south_west.insert(point))

    def query(self, range_rect, found=None):
        if found is None:
            found = []

        # Return if the range does not intersect the boundary
        if not self._boundary.intersects(range_rect):
            return found

        for point in self._points:
            if range_rect.contains(point):
                found.append(point)

        if self._divided:
            self._north_east.query(range_rect, found)
            self._north_west.query(range_rect, found)
            self._south_east.query(range_rect, found)
            self._south_west.query(range_rect, found)

        return found

    def plot(self, ax):
        rect = plt.Rectangle((self._boundary.x, self._boundary.y),
                             self._boundary.width,
                             self._boundary.height,
                             fill=False, color="black", lw=0.8)

        ax.add_patch(rect)


        if self._divided:
            self._north_east.plot(ax)
            self._north_west.plot(ax)
            self._south_east.plot(ax)
            self._south_west.plot(ax)

    def __repr__(self):
        return f'Quadtree({self._points} points, divided={self._divided})'

    @property
    def exceeds_capacity(self) -> bool:
        return len(self._points) < self._capacity