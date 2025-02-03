import unittest

from matplotlib import pyplot as plt

from src.helper import plot_points
from src.optimized import Rectangle, Quadtree, Point


class TestQuadtreeOptimized(unittest.TestCase):
    def test_insert_and_query(self):
        boundary = Rectangle(0, 0, 100, 100)
        quadtree = Quadtree(boundary, 4)

        points = [
            Point(10, 10), Point(20, 20), Point(30, 30), Point(40, 40), Point(50, 50),
            Point(60, 60), Point(70, 70), Point(80, 80), Point(90, 90), Point(25, 25)
        ]

        quadtree.batch_insert(points)

        query_rect = Rectangle(20, 20, 30, 30)
        found = quadtree.query(query_rect)
        expected_points = [p for p in points if query_rect.contains(p)]

        fig, ax = plt.subplots(figsize=(6, 6))
        quadtree.plot(ax)

        # Uncomment this line to plot the points
        # plot_points(points, ax, query_patch=query_rect, found_points=found)

        self.assertEqual(expected_points, found)

if __name__ == '__main__':
    unittest.main()
