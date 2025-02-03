using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using Perfolizer.Horology;

namespace Quadtree;

public class BenchmarkConfig : ManualConfig {
    public BenchmarkConfig() {
        AddColumn(StatisticColumn.Mean);
        AddColumn(StatisticColumn.Error);
        AddColumn(StatisticColumn.StdDev);
        AddColumn(StatisticColumn.Median);
        AddColumn(StatisticColumn.Min);
        AddColumn(StatisticColumn.Max);
        AddColumn(StatisticColumn.P90);
        AddColumn(StatisticColumn.P95);
        AddColumn(StatisticColumn.Iterations);
    }
}

[Config(typeof(BenchmarkConfig))]
[MemoryDiagnoser]
public class QuadtreeBenchmark {
    private Quadtree _quadtree;
    private QuadtreeSpan _quadtreeSpan;
    private QuadtreeIterative _quadtreeIterative;
    private List<Point> _points;

    [GlobalSetup]
    public void Setup() {
        var boundary = new Rectangle(0, 0, 100, 100);
        _quadtree = new Quadtree(boundary, 4);
        _quadtreeSpan = new QuadtreeSpan(boundary, 4);
        _quadtreeIterative = new QuadtreeIterative(boundary, 4);

        _points = new List<Point>();
        for (int i = 0; i < 1000; i++) {
            _points.Add(new Point(i, i));
        }
    }

    [Benchmark]
    public void InsertQuadtree() {
        foreach (var point in _points) {
            _quadtree.Insert(point);
        }
    }

    [Benchmark]
    public void InsertQuadtreeSpan() {
        foreach (var point in _points) {
            _quadtreeSpan.Insert(point);
        }
    }

    [Benchmark]
    public void InsertQuadtreeIterative() {
        foreach (var point in _points) {
            _quadtreeIterative.Insert(point);
        }
    }

    [Benchmark]
    public void QueryQuadtree() {
        var range = new Rectangle(0, 0, 50, 50);
        _quadtree.Query(range);
    }

    [Benchmark]
    public void QueryQuadtreeSpan() {
        var range = new Rectangle(0, 0, 50, 50);
        _quadtreeSpan.Query(range, new Point[1000]);
    }

    [Benchmark]
    public void QueryQuadtreeIterative() {
        var range = new Rectangle(0, 0, 50, 50);
        _quadtreeIterative.Query(range);
    }
}