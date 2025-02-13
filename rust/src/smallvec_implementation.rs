use smallvec::SmallVec;
use crate::geometry::{Point, Rectangle};
use crate::unsafe_implementation::QuadtreeUnsafe;

const CAPACITY: usize = 4;

pub struct QuadtreeSmallVec {
    boundary: Rectangle,
    points: SmallVec<[Point; 4]>,
    divided: bool,
    children: Option<[Box<QuadtreeSmallVec>; 4]>
}

impl QuadtreeSmallVec {
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
        let north_east = Box::new(QuadtreeSmallVec::new(ne));

        let nw = Rectangle { x, y, width: w, height: h };
        let north_west = Box::new(QuadtreeSmallVec::new(nw));

        let se = Rectangle { x: x + w, y: y + h, width: w, height: h };
        let south_east = Box::new(QuadtreeSmallVec::new(se));

        let sw = Rectangle { x, y: y + h, width: w, height: h };
        let south_west = Box::new(QuadtreeSmallVec::new(sw));

        self.children = Some([north_east, north_west, south_east, south_west]);
        self.divided = true;
    }

    pub fn query(&self, range: &Rectangle, found: &mut Vec<Point>) {
        if !self.boundary.intersects(range) {
            return;
        }

        for &point in &self.points {
            if range.contains(&point) {
                found.push(point);
            }
        }

        if self.divided {
            if let Some(children) = &self.children {
                for child in children.iter() {
                    child.query(range, found);
                }
            }
        }
    }
}

