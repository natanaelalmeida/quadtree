use crate::geometry::{Point, Rectangle};
use rayon::prelude::*;
use smallvec::SmallVec;

const CAPACITY: usize = 4;


pub struct QuadtreeRayon {
    boundary: Rectangle,
    points: SmallVec<[Point; CAPACITY]>,
    divided: bool,
    children: Option<[Box<QuadtreeRayon>; CAPACITY]>
}

impl QuadtreeRayon {
    pub fn new(boundary: Rectangle) -> Self {
        Self {
            boundary,
            points: SmallVec::new(),
            divided: false,
            children: None
        }
    }

    pub fn insert(&mut self, point: Point) -> bool {
        if !self.boundary.contains(&point) {
            return false;
        }

        if self.points.len() < CAPACITY {
            self.points.push(point);
            return true;
        }

        if !self.divided {
            self.subdivide();
        }

        if let Some(ref mut children) = self.children.as_mut() {
            return children.iter_mut().any(|child| child.insert(point));
        }

        false
    }

    fn subdivide(&mut self) {
        let x = self.boundary.x;
        let y = self.boundary.y;
        let w = self.boundary.width / 2.0;
        let h = self.boundary.height / 2.0;

        let ne = Rectangle { x: x + y, y, width: w, height: h };
        let north_east = Box::new(QuadtreeRayon::new(ne));

        let nw = Rectangle { x, y, width: w, height: h };
        let north_west = Box::new(QuadtreeRayon::new(nw));

        let se = Rectangle { x: x + w, y: y + h, width: w, height: h };
        let south_east = Box::new(QuadtreeRayon::new(se));

        let sw = Rectangle { x, y: y + h, width: w, height: h };
        let south_west = Box::new(QuadtreeRayon::new(sw));

        self.children = Some([north_east, north_west, south_east, south_west]);
        self.divided = true;
    }

    pub fn query(&self, range: &Rectangle) -> Vec<Point> {
        if !self.boundary.intersects(range) {
            return Vec::new();
        }

        let mut found: Vec<Point> = self.points.iter()
            .copied()
            .filter(|point| range.contains(point))
            .collect();

        if self.divided {
            if let Some(children) = &self.children {
                let child_result: Vec<Vec<Point>> = children
                    .par_iter()
                    .map(|child| child.query(range))
                    .collect();

                for mut result in child_result {
                    found.append(&mut result);
                }
            }
        }

        found
    }
}

pub struct QuadtreeBuffer {
    boundary: Rectangle,
    points: SmallVec<[Point; CAPACITY]>,
    divided: bool,
    children: Option<[Box<QuadtreeBuffer>; CAPACITY]>
}

impl QuadtreeBuffer {
    pub fn new(boundary: Rectangle) -> Self {
        Self {
            boundary,
            points: SmallVec::new(),
            divided: false,
            children: None
        }
    }

    pub fn insert(&mut self, point: Point) -> bool {
        if !self.boundary.contains(&point) {
            return false;
        }

        if self.points.len() < CAPACITY {
            self.points.push(point);
            return true;
        }

        if !self.divided {
            self.subdivide();
        }

        if let Some(ref mut children) = self.children.as_mut() {
            return children.iter_mut().any(|child| child.insert(point));
        }

        false
    }

    fn subdivide(&mut self) {
        let x = self.boundary.x;
        let y = self.boundary.y;
        let w = self.boundary.width / 2.0;
        let h = self.boundary.height / 2.0;

        let ne = Rectangle { x: x + y, y, width: w, height: h };
        let north_east = Box::new(QuadtreeBuffer::new(ne));

        let nw = Rectangle { x, y, width: w, height: h };
        let north_west = Box::new(QuadtreeBuffer::new(nw));

        let se = Rectangle { x: x + w, y: y + h, width: w, height: h };
        let south_east = Box::new(QuadtreeBuffer::new(se));

        let sw = Rectangle { x, y: y + h, width: w, height: h };
        let south_west = Box::new(QuadtreeBuffer::new(sw));

        self.children = Some([north_east, north_west, south_east, south_west]);
        self.divided = true;
    }

    pub unsafe  fn query(&self, range: &Rectangle, buffer: &mut Vec<Point>) {
        let mut count = buffer.len();

        if !self.boundary.intersects(range) {
            return;
        }

        for &point in &self.points{
            if range.contains(&point){
                let dest = buffer.as_mut_ptr().add(count);
                std::ptr::write(dest, point);
                count += 1;
            }
        }

        if self.divided {
            if let Some(ref children) = self.children{
                for child in children.iter(){
                    child.query(range, buffer);
                }
            }
        }

        buffer.set_len(count);
    }
}