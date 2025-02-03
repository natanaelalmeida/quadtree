import random
import unittest

from matplotlib import pyplot as plt

from src.helper import plot_points
from src.simple import Rectangle, Quadtree, Point


class TestQuadtree(unittest.TestCase):
    def test_insert_and_query(self):
        boundary = Rectangle(0, 0, 100, 100)
        quadtree = Quadtree(boundary, 4)

        points = [
            Point(10, 10), Point(20, 20), Point(30, 30), Point(40, 40), Point(50, 50),
            Point(60, 60), Point(70, 70), Point(80, 80), Point(90, 90), Point(25, 25)
        ]

        for p in points:
            quadtree.insert(p)

        query_rect = Rectangle(20, 20, 30, 30)
        found = quadtree.query(query_rect)
        expected_points = [p for p in points if query_rect.contains(p)]

        fig, ax = plt.subplots(figsize=(6, 6))
        quadtree.plot(ax)

        # Uncomment this line to plot the points
        # plot_points(points, ax, query_patch=query_rect, found_points=found)

        self.assertEqual(expected_points, found)

    def test_query_partial_overlap(self):
        boundary = Rectangle(0, 0, 100, 100)
        quadtree = Quadtree(boundary, 4)

        points = [
            Point(10, 10),
            Point(20, 20),
            Point(30, 30),
            Point(40, 40),
            Point(50, 50)
        ]

        for p in points:
            quadtree.insert(p)

        query_rect = Rectangle(25, 25, 50, 50)
        found = quadtree.query(query_rect)

        expected_points = [p for p in points if query_rect.contains(p)]
        self.assertEqual(found, expected_points)

    def test_insert_outside_boundary(self):
        boundary = Rectangle(0, 0, 100, 100)
        quadtree = Quadtree(boundary, 4)

        points = [
            Point(-10, -10),
            Point(110, 110),
            Point(50, -10),
            Point(-10, 50)
        ]

        for p in points:
            result = quadtree.insert(p)
            self.assertFalse(result)



if __name__ == '__main__':
    unittest.main()
