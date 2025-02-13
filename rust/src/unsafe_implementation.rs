use crate::geometry::{Point, Rectangle};
use std::mem::MaybeUninit;

const CAPACITY: usize = 4;


pub struct QuadtreeUnsafe {
    boundary: Rectangle,
    points: [MaybeUninit<Point>; CAPACITY],
    point_count: usize,
    divided: bool,
    children: Option<[Box<QuadtreeUnsafe>; 4]>
}

impl QuadtreeUnsafe {
    pub fn new(boundary: Rectangle) -> Self {
        let points: [MaybeUninit<Point>; CAPACITY] = unsafe {
            MaybeUninit::uninit().assume_init()
        };

        Self {
            boundary,
            points,
            point_count: 0,
            divided: false,
            children: None
        }
    }

    pub fn insert(&mut self, point: Point) -> bool {
        if !self.boundary.contains(&point) {
            return false;
        }

        if self.point_count < CAPACITY {
            self.points[self.point_count] = MaybeUninit::new(point);
            self.point_count += 1;
            return true;
        }

        if !self.divided {
            self.subdivide();
        }

        if let Some(ref mut children) = self.children {
            for child in children.iter_mut() {
                if child.insert(point) {
                    return true;
                }
            }
        }

        false
    }

    fn subdivide(&mut self) {
        let x = self.boundary.x;
        let y = self.boundary.y;
        let w = self.boundary.width / 2.0;
        let h = self.boundary.height / 2.0;

        let ne = Rectangle { x: x + y, y, width: w, height: h };
        let north_east = Box::new(QuadtreeUnsafe::new(ne));

        let nw = Rectangle { x, y, width: w, height: h };
        let north_west = Box::new(QuadtreeUnsafe::new(nw));

        let se = Rectangle { x: x + w, y: y + h, width: w, height: h };
        let south_east = Box::new(QuadtreeUnsafe::new(se));

        let sw = Rectangle { x, y: y + h, width: w, height: h };
        let south_west = Box::new(QuadtreeUnsafe::new(sw));

        self.children = Some([north_east, north_west, south_east, south_west]);
        self.divided = true;
    }

    pub fn query(&self, range: &Rectangle, found: &mut Vec<Point>) {
        if !self.boundary.intersects(range) {
            return;
        }

        for i in 0..self.point_count {
            let point = unsafe {
                self.points[i].assume_init()
            };

            if range.contains(&point) {
                found.push(point);
            }
        }

        if self.divided {
            if let Some(ref children) = &self.children {
                for child in children.iter() {
                    child.query(range, found);
                }
            }
        }
    }
}