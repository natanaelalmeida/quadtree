#[derive(Debug, Clone, Copy)]
struct Point {
    x: f64,
    y: f64
}

#[derive(Debug, Clone, Copy)]
struct Rectangle {
    x: f64,
    y: f64,
    width: f64,
    height: f64
}

impl Rectangle {
    fn contains(&self, point: &Point) -> bool {
        point.x >= self.x
        && point.x <= self.x + self.width
        && point.y >= self.y
        && point.y <= self.y + self.height
    }

    fn intersects(&self, other: &Rectangle) -> bool {
        other.x < self.x + self.width
        && other.x + other.width > self.x
        && other.y < self.y + self.height
        && other.y + other.height > self.y
    }
}

#[derive(Debug)]
struct Quadtree {
    boundary: Rectangle,
    capacity: usize,
    points: Vec<Point>,
    divided: bool,
    north_west: Option<Box<Quadtree>>,
    north_east: Option<Box<Quadtree>>,
    south_west: Option<Box<Quadtree>>,
    south_east: Option<Box<Quadtree>>
}

impl Quadtree {
    fn new(boundary: Rectangle, capacity: usize) -> Self {
        Self {
            boundary,
            capacity,
            points: Vec::new(),
            divided: false,
            north_west: None,
            north_east: None,
            south_west: None,
            south_east: None
        }
    }

    fn insert(&mut self, point: Point) -> bool {
        if !self.boundary.contains(&point) {
            return false;
        }

        if self.points.len() < self.capacity {
            self.points.push(point);
            return true;
        }

        if !self.divided {
            self.subdivide();
        }

        if let Some(ref mut ne) = self.north_east {
            if ne.insert(point) {
                return true;
            }
        }

        if let Some(ref mut nw) = self.north_west {
            if nw.insert(point) {
                return true;
            }
        }

        if let Some(ref mut se) = self.south_east {
            if se.insert(point) {
                return true;
            }
        }

        if let Some(ref mut sw) = self.south_west {
            if sw.insert(point) {
                return true;
            }
        }

        false
    }

    fn query(&self, range: &Rectangle, found: &mut Vec<Point>){
        if !self.boundary.intersects(range) {
            return;
        }

        for &point in &self.points{
            if range.contains(&point) {
                found.push(point);
            }
        }

        if self.divided {
            if let Some(ref ne) = self.north_east {
                ne.query(range, found);
            }

            if let Some(ref nw) = self.north_west {
                nw.query(range, found);
            }

            if let Some(ref se) = self.south_east {
                se.query(range, found);
            }

            if let Some(ref sw) = self.south_west {
                sw.query(range, found);
            }
        }
    }

    fn subdivide(&mut self){
        let x = self.boundary.x;
        let y = self.boundary.y;
        let w = self.boundary.width / 2.0;
        let h = self.boundary.height / 2.0;

        let ne = Rectangle { x: x + y, y, width: w, height: h };
        self.north_east = Some(Box::new(Quadtree::new(ne, self.capacity)));

        let nw = Rectangle { x, y, width: w, height: h };
        self.north_west = Some(Box::new(Quadtree::new(nw, self.capacity)));

        let se = Rectangle { x: x + w, y: y + h, width: w, height: h };
        self.south_east = Some(Box::new(Quadtree::new(se, self.capacity)));

        let sw = Rectangle { x, y: y + h, width: w, height: h };
        self.south_west = Some(Box::new(Quadtree::new(sw, self.capacity)));

        self.divided = true;
    }
}

fn main() {
    let boundary = Rectangle { x: 0.0, y: 0.0, width: 200.0, height: 200.0 };
    let mut qtree = Quadtree::new(boundary, 4);

    qtree.insert(Point { x: 50.0, y: 50.0 });
    qtree.insert(Point { x: 150.0, y: 150.0 });
    qtree.insert(Point { x: 75.0, y: 75.0 });
    qtree.insert(Point { x: 125.0, y: 125.0 });

    let search = Rectangle { x: 40.0, y: 40.0, width: 50.0, height: 50.0 };
    let mut found = Vec::new();

    qtree.query(&search, &mut found);

    println!("Points in range: {:?}", found);
}
