use quadtree::geometry::{Point, Rectangle};
use quadtree::query_optimization::{QuadtreeBuffer, QuadtreeRayon};

use criterion::{black_box, criterion_group, criterion_main, Criterion};
use rand::Rng;

fn setup_quadtree_rayon(num_points: usize) -> QuadtreeRayon {
    let mut rec = Rectangle { x: 0.0, y: 0.0, width: 500.0, height: 500.0 };
    let mut qt = QuadtreeRayon::new(rec);
    let mut rng = rand::thread_rng();
    for _ in 0..num_points {
        let p = Point {
            x: rng.gen_range(0.0..500.0),
            y: rng.gen_range(0.0..500.0),
        };
        qt.insert(p);
    }
    qt
}

fn setup_quadtree_buffer(num_points: usize) -> QuadtreeBuffer {
    let mut rec = Rectangle { x: 0.0, y: 0.0, width: 500.0, height: 500.0 };
    let mut qt = QuadtreeBuffer::new(rec);
    let mut rng = rand::thread_rng();
    for _ in 0..num_points {
        let p = Point {
            x: rng.gen_range(0.0..500.0),
            y: rng.gen_range(0.0..500.0),
        };
        qt.insert(p);
    }
    qt
}

fn benchmark_safe_query(c: &mut Criterion) {
    let qt = setup_quadtree_rayon(10_000);
    let search_area = Rectangle { x: 200.0, y: 200.0, width: 100.0, height: 100.0 };
    c.bench_function("safe_parallel_query", |b| {
        b.iter(|| {
            let res = qt.query(&search_area);
            black_box(res);
        })
    });
}

fn benchmark_unsafe_query(c: &mut Criterion) {
    let qt = setup_quadtree_buffer(10_000);
    let search_area = Rectangle { x: 200.0, y: 200.0, width: 100.0, height: 100.0 };
    let mut buffer = Vec::with_capacity(10_000);
    c.bench_function("unsafe_query_to_buffer", |b| {
        b.iter(|| {
            buffer.clear();
            unsafe {
                qt.query(&search_area, &mut buffer);
            }
            black_box(&buffer);
        })
    });
}

criterion_group!(benches, benchmark_safe_query, benchmark_unsafe_query);
criterion_main!(benches);