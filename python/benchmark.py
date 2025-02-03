import random
import time

from pympler import asizeof

from src.simple import Quadtree as QuadtreeSimple, Rectangle as RectangleSimple, Point as PointSimple
from src.optimized import Quadtree as QuadtreeOptimized, Rectangle as RectangleOptimized, Point as PointOptimized

def benchmark_quadtree_size():
    qtree_simple = QuadtreeSimple(RectangleSimple(0, 0, 100, 100), 4)
    qtree_optimized = QuadtreeOptimized(RectangleOptimized(0, 0, 100, 100), 4)

    print(f"Simple size: {asizeof.asizeof(qtree_simple) / 1024:.2f} KB")
    print(f"Optimized size: {asizeof.asizeof(qtree_optimized) / 1024:.2f} KB")

def benchmark_insert_time():
    qtree_simple = QuadtreeSimple(RectangleSimple(0, 0, 100, 100), 4)
    qtree_optimized = QuadtreeOptimized(RectangleOptimized(0, 0, 100, 100), 4)

    points = [
        PointSimple(random.randint(0, 100), random.randint(0, 100)) for _ in range(1000)
    ]


    start = time.time()
    for p in points:
        qtree_simple.insert(p)
    end = time.time()

    print(f"\nSimple: {(end - start) * 1000:.2f} ms")
    print(f"Points size: {asizeof.asizeof(points) / 1024:.2f} KB")

    points = [
        PointOptimized(random.randint(0, 100), random.randint(0, 100)) for _ in range(1000)
    ]

    start = time.time()
    for p in points:
        qtree_optimized.insert(p)
    end = time.time()

    print(f"\nOptimized: {(end - start) * 1000:.2f} ms")
    print(f"Points size: {asizeof.asizeof(points) / 1024:.2f} KB")


def benchmark_query_time():
    qtree_simple = QuadtreeSimple(RectangleSimple(0, 0, 100, 100), 4)
    qtree_optimized = QuadtreeOptimized(RectangleOptimized(0, 0, 100, 100), 4)

    points = [
        PointSimple(random.randint(0, 100), random.randint(0, 100)) for _ in range(1000)
    ]

    for p in points:
        qtree_simple.insert(p)

    for p in points:
        qtree_optimized.insert(p)

    query_rect = RectangleSimple(20, 20, 30, 30)

    start = time.time()
    qtree_simple.query(query_rect)
    end = time.time()

    print(f"\nSimple query: {(end - start) * 1000:.2f} ms")

    start = time.time()
    qtree_optimized.query(query_rect)
    end = time.time()

    print(f"Optimized query: {(end - start) * 1000:.2f} ms")


if __name__ == '__main__':
    benchmark_quadtree_size()
    benchmark_insert_time()
    benchmark_query_time()