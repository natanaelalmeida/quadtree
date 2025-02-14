from abc import ABC, abstractmethod

import numpy as np

from src.optimized import Rectangle
from src.optimized import Quadtree


class QueryStrategy(ABC):
    def __init__(self, quadtree: Quadtree):
        self._quadtree = quadtree

    @abstractmethod
    def query(self):
        raise NotImplementedError

class SlidingWindowQuery(QueryStrategy):
    def __init__(self, quadtree: Quadtree):
        super().__init__(quadtree)

    def query(self, **kwargs):
        window_size = kwargs.get('window_size', 10)
        step_size = kwargs.get('step_size', 1)
        density_threshold = kwargs.get('density_threshold', 3)

        found = []
        boundary = self._quadtree.boundary

        x_start, y_start = boundary.x, boundary.y
        x_end, y_end = boundary.x + boundary.width, boundary.y + boundary.height

        x_values =  np.arange(x_start, x_end - window_size + 1, step_size)
        y_values = np.arange(y_start, y_end - window_size + 1, step_size)

        for x in x_values:
            for y in y_values:
                window = Rectangle(x, y, window_size, window_size)
                points = self._quadtree.query(window)
                if len(points) >= density_threshold:
                    found.append((window, len(points), points))

        return found

from sklearn.cluster import DBSCAN

class DensityBasedQuery(QueryStrategy):
    def __init__(self, quadtree: Quadtree):
        super().__init__(quadtree)

    def query(self, **kwargs):
        epsilon = kwargs.get("epsilon", 30)
        min_samples = kwargs.get("min_samples", 5)

        data = np.array([[point.x, point.y] for point in self._quadtree.points])
        db_scan = DBSCAN(eps=epsilon, min_samples=min_samples)
        labels = db_scan.fit_predict(data)

        clusters = {}
        for label, point in zip(labels, self._quadtree.points):
            if label not in clusters:
                clusters[label] = []
            clusters[label].append(point)

        return clusters
