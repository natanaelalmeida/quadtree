// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Quadtree;

BenchmarkRunner.Run<QuadtreeBenchmark>();

Console.WriteLine("Hello, World!");