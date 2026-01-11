use std::{fs, vec};
use std::collections::VecDeque;
use std::collections::HashSet;
use std::collections::HashMap;
use itertools::Itertools;
use std::mem;

// Note: using a lot of raw unwraps here.
// Input is well defined by the author and gives certain guarantees.
// For this project, it's fine to just panic on failed parsing etc

fn main() {

    // Since we are not allowed to share our personal input publically
    // you are supposed to create a file next to main.rs with the input from https://adventofcode.com/2025

    let contents = fs::read_to_string("src/Input.txt")
        .expect("Should have been able to read the file");

    day12(&contents);
}

fn day01(contents: &String) {
    let mut zero_count_end = 0;
    let mut zero_count_total = 0;
    let mut current_rotation = 50;
    for line in contents.lines() {
        let rotation: i32 = line[1..].parse().unwrap();

        let laps = rotation / 100;
        zero_count_total += laps;
        let rotation_leftover = rotation % 100;

        if line.starts_with("R") {
            current_rotation += rotation_leftover;

            if current_rotation > 99 {
                current_rotation -= 100;
                if current_rotation != 0 { // this case is already counted at the end
                    zero_count_total += 1;
                }
            }
        }
        else {
            let was_already_zero = current_rotation == 0;
            current_rotation -= rotation_leftover;

            if current_rotation < 0 {
                current_rotation += 100;
                // note cannot be 0 zero here, min -99+100=1
                // we could however went Left when already at 0, this was already counted when we reached 0
                if !was_already_zero {
                    zero_count_total += 1;
                }
            }
        }

        if current_rotation == 0 {
            zero_count_end += 1;
            zero_count_total += 1;
        }
    }

    println!("Zero count part 1: {zero_count_end}");
    println!("Zero count part 2: {zero_count_total}");
}

fn day02(contents: &String) {
    let mut sum_invalid_ids_part1: i64 = 0;
    let mut sum_invalid_ids_part2: i64 = 0;
    for range in contents.split(",") {
        let boundaries: Vec<i64> = range
            .split("-")
            .map(|x| x.parse().unwrap())
            .collect();

        let start = boundaries.get(0).unwrap().clone();
        let end  = boundaries.get(1).unwrap().clone();

        for id_to_check in start..(end + 1) {
            let id_to_check_str = id_to_check.to_string();
            let digits_count = id_to_check_str.len();

            for sequence_count in 2..(digits_count + 1) {
                if digits_count % sequence_count != 0 {
                    continue; // cannot divide length of id by number of sequences
                }
                let sequence_length = digits_count / sequence_count;

                let mut is_repeated = true;
                for sequence_idx in 1..sequence_count {
                    if !id_to_check_str.chars().skip(sequence_idx * sequence_length).take(sequence_length).eq(id_to_check_str.chars().take(sequence_length)) {
                        is_repeated = false;
                        break;
                    }
                }

                if is_repeated {
                    sum_invalid_ids_part2 += id_to_check;

                    if sequence_count == 2 {
                        sum_invalid_ids_part1 += id_to_check;
                    }

                    // no need to check other sequence counts when its mirroring for a less sequence count
                    // eg 2222 mirrors with 2 and 4 sequences, only count it once
                    break; 
                }
            }
        }
    }
    println!("Part1, sum of all invalid ids: {sum_invalid_ids_part1}");
    println!("Part2, sum of all invalid ids: {sum_invalid_ids_part2}");
}

fn day03(contents: &String) {
    let mut sum_joltage_part1 = 0u64;
    let mut sum_joltage_part2 = 0u64;

    for line in contents.lines() {
        let batteries: Vec<u64> = line.chars().map(|i| u64::from(i.to_digit(10).unwrap())).collect();
        sum_joltage_part1 += day03_joltage(&batteries, 2);
        sum_joltage_part2 += day03_joltage(&batteries, 12);

    }

    println!("Part1: sum of max joltages {sum_joltage_part1}");
    println!("Part2: sum of max joltages {sum_joltage_part2}");
}

fn day03_joltage(batteries: &Vec<u64>, batteries_count: usize) -> u64 {
        // current best digits
        let mut digits = vec![0u64; batteries_count];

        for (battery_idx, battery_val) in batteries.iter().enumerate() {

            // go over all digits and check if this battery is better
            for digit_idx in 0..digits.len() {
                let digit_val = digits.get_mut(digit_idx).unwrap();

                if battery_idx > batteries.len() - batteries_count + digit_idx {
                    // for this digit, the battery cannot match as we wouldn't be able to find the remaining digits (not enough batteries left)
                    continue;
                }

                if battery_val > digit_val {
                    // found better digit, replace value in vec
                    *digit_val = *battery_val;

                    // reset the remaining ones to the right
                    for reset_idx in digit_idx + 1 .. digits.len() {
                        digits[reset_idx] = 0;
                    }

                    break; //battery used, cannot be used for another digit
                }
            }
        }

        // [1, 2, 3] => 3 + 20 + 100 => 123 
        return digits.iter().rev().enumerate().map(|(idx, digit)| 10u64.pow(idx as u32) * digit).sum();
}

fn day04(contents: &String) { 
    // convert content to grid
    let mut grid: Vec<Vec<char>> = contents
        .lines()
        .map(|line| line.chars().collect())
        .collect();

    // add padding so we dont need bound checking below

    // pad left & right
    for row in &mut grid {
        row.splice(0..0, ['.']);
        row.push('.');
    }
    // pad top & bot
    let col_count = grid[0].len();
    grid.splice(0..0, [vec!['.'; col_count]]);
    grid.push(vec!['.'; col_count]);

    let mut roll_count_part1 = day04_prune(&mut grid);

    let mut roll_count_part2 = roll_count_part1;
    let mut roll_count_diff = 1;
    while roll_count_diff != 0 {
        roll_count_diff = day04_prune(&mut grid);
        roll_count_part2 += roll_count_diff;
    }

    println!("Part1: roll count for forklift {roll_count_part1}");
    println!("Part2: roll count for forklift {roll_count_part2}");
}

fn day04_prune(grid: &mut Vec<Vec<char>>) -> i32 {
    let row_count = grid.len();
    let col_count = grid[0].len();
    let mut roll_count = 0;

    let mut rolls_to_remove: Vec<(usize, usize)> = vec![];

    for row_idx in 1..row_count - 1 {
        for col_idx in 1..col_count - 1 {

            // check if this spot has a roll
            if grid[row_idx][col_idx] != '@' {
                continue;
            }

            let mut neighbour_count = 0;
            'diff: for row_idx_diff in -1..=1isize {
                for col_idx_diff in -1..=1isize {
                    
                    // do not count yourself
                    if row_idx_diff == 0 && col_idx_diff == 0 {
                        continue;
                    }

                    if grid[row_idx.checked_add_signed(row_idx_diff).unwrap()][col_idx.checked_add_signed(col_idx_diff).unwrap()] == '@' {
                        neighbour_count += 1;

                        if neighbour_count >= 4 {
                            break 'diff;
                        }
                    }
                }
            }

            if neighbour_count < 4 {
                roll_count += 1;

                // queue to remove from grid (can affect neighbours if immediately removed)
                rolls_to_remove.push((row_idx, col_idx));
            }
        }
    }

    // actually remove from grid
    for (row_idx, col_idx) in rolls_to_remove {
        grid[row_idx][col_idx] = '.';
    }

    return roll_count;
}

fn day05(contents: &String) {

    // parse input
    let mut ranges: Vec<(u64, u64)> = vec![];
    let mut ids: Vec<u64> = vec![];
    let mut is_parsing_ranges = true;

    for line in contents.lines() {
        if line.is_empty() {
            is_parsing_ranges = false;
            continue;
        }

        if is_parsing_ranges {
            let boundaries: Vec<u64> = line.split("-").map(|i| i.parse().unwrap()).collect();
            ranges.push((boundaries[0], boundaries[1]));
        }
        else {
            ids.push(line.parse().unwrap());
        }
    }

    let mut fresh_count_part1 = 0;

    'id_loop: for id in ids {
        for range in &ranges {
            if range.0 <= id && id <= range.1 {
                fresh_count_part1 += 1;
                continue 'id_loop;
            }
        }
    }

    println!("Part1: fresh count {fresh_count_part1}");

    let mut non_overlapping_ranges: Vec<(u64, u64)> = vec![];
    let mut queue = VecDeque::from(ranges);
    let mut next = queue.pop_front();
    while let Some(range_candidate) = &mut next {

        // go over existing non-overlapping ranges and check if we do
        // if so, cut up this candidate in pieces and keep checking

        for range_existing in &non_overlapping_ranges {

            // 2 ranges can overlap in 4 different ways

            // Case 1
            //  ------------   (existing)
            //     -------     (candidate)
            if range_candidate.0 >= range_existing.0 && range_candidate.1 <= range_existing.1 {
                // this candidate is entirely encased in already existing so do nothing
                *range_candidate = (0, 0);
                break;
            }

            // Case 2
            //     -------     (existing)
            //  ------------   (candidate)
            if range_existing.0 >= range_candidate.0 && range_existing.1 <= range_candidate.1 {
                // split candidate in 3 pieces and queue left and right piece for re evaluation on their own
                
                // Left
                if range_candidate.0 != range_existing.0 {
                    queue.push_back((range_candidate.0, range_existing.0 - 1));
                }

                // Right
                if range_candidate.1 != range_existing.1 {
                    queue.push_back((range_existing.1 + 1, range_candidate.1));
                }

                // throw away remainder
                *range_candidate = (0, 0);
                break;
            }

            // Case 3
            //     -------     (existing)
            //  ------         (candidate)
            if range_candidate.0 < range_existing.0 && range_candidate.1 >= range_existing.0 {
                // cut off right part and keep searching for more on the left part
                *range_candidate = (range_candidate.0, range_existing.0 - 1);
                continue; // keep processing candidate
            }

            // Case 4
            //  ------         (existing)
            //     -------     (candidate)
            if range_candidate.1 > range_existing.1 && range_candidate.0 <= range_existing.1 {
                // cut off left part and keep searching for more on the right part
                *range_candidate = (range_existing.1 + 1, range_candidate.1);
                continue; // keep processing candidate
            }

        }

        if range_candidate.0 != 0 && range_candidate.1 != 0 {
            non_overlapping_ranges.push(*range_candidate);
        }

        next = queue.pop_front();
    }

    let mut fresh_count_part2 = 0;
    for range in &non_overlapping_ranges {
        fresh_count_part2 += range.1 - range.0 + 1; // add one because bounds are inclusive
    }

    println!("Part2: fresh count {fresh_count_part2}");
}

fn day06(contents: &String) {
    // parse input
    let mut numbers: Vec<Vec<u64>> = vec![];
    let mut operations: Vec<char> = vec![];
    for line in contents.lines() {
        // checked input and number rows always start with digit, ideally this is more robust
        if line.trim_start().chars().nth(0).unwrap().is_digit(10) {
            numbers.push(line
                .split(" ")
                .filter(|s| !s.is_empty())
                .map(|n| n.parse().unwrap())
                .collect());
        }
        else {
            operations = line
                .split(" ")
                .filter(|s| !s.is_empty())
                .map(|s| s.chars().nth(0).unwrap())
                .collect();
        }
    }

    // transpose vec of vec
    let len = numbers[0].len();
    let mut iters: Vec<_> = numbers.into_iter().map(|n| n.into_iter()).collect();
    let numbers_transposed: Vec<Vec<u64>> = (0..len)
        .map(|_| {
            iters
                .iter_mut()
                .map(|n| n.next().unwrap())
                .collect::<Vec<u64>>()
        })
        .collect();

    let mut sum_part1 = 0u64;
    for (idx, row) in numbers_transposed.iter().enumerate() {
        if operations[idx] == '+' {
            sum_part1 += &row.iter().sum();
        }
        else {
            sum_part1 += &row.iter().product();
        }
    }

    println!("Part1: sum {sum_part1}");

    // for part 2 we will flip the rows and remove the operation row
    // this makes it easier to keep track of which digit we are in big number
    // a + b * 10 + c * 100 + d * 1000 etc

    let lines_flipped: Vec<Vec<char>> = contents
        .lines()
        .rev()
        .skip(1)
        .map(|s| s.chars().collect())
        .collect();

    let mut sum_part2 = 0u64;
    let mut number_queue: Vec<u64> = vec![];
    let height = lines_flipped.len();
    let width = lines_flipped[0].len();
    let mut op_idx = 0;
    for col_idx in 0..width {
        let mut digit_queue: Vec<u64> = vec![];
        for row_idx in 0..height {
            let digit_candidate= lines_flipped[row_idx][col_idx];
            if digit_candidate == ' ' {
                continue;
            }
            digit_queue.push(u64::from(digit_candidate.to_digit(10).unwrap()));
        }

        if !digit_queue.is_empty() {
            let number: u64 = digit_queue.iter().enumerate().map(|(idx, digit)| 10u64.pow(idx as u32) * digit).sum();
            number_queue.push(number);
        }

        if digit_queue.is_empty() || col_idx == width - 1 {
            // empty column or last column => finish equation
            if operations[op_idx] == '+' {
                sum_part2 += &number_queue.iter().sum();
            }
            else {
                sum_part2 += &number_queue.iter().product();
            }

            // reset for next equation
            number_queue.clear();
            op_idx += 1;
        }
    }
    println!("Part2: sum {sum_part2}");
}

fn day07(contents: &String) { 
    let grid: Vec<Vec<char>> = contents
        .lines()
        .map(|line| line.chars().collect())
        .collect();

    let width = grid[0].len();
    let mut split_count = 0;
    let mut beams = vec![0u64; width];
    let start = grid[0].iter().position(|c| *c == 'S').unwrap();
    beams[start] = 1;

    // start at 2 because we already did first 2 rows (S and beam under it)
    for row_idx in 2..grid.len() {
        let mut beams_next = vec![0u64; width];

        for col_idx in 0..width {
            let beam_count = beams[col_idx];

            if beam_count == 0 {
                // no beam above, nothing to do
                continue;
            }

            if grid[row_idx][col_idx] == '^' {
                // splitter, create new beams, left & right
                if col_idx != 0 {
                    beams_next[col_idx - 1] += beam_count;
                }
                if col_idx != width - 1 {
                    beams_next[col_idx + 1] += beam_count;
                }

                split_count += 1;
            }
            else {
                // empty space, beam falls through (if any)
                beams_next[col_idx] += beam_count;
            }
        }

        beams = beams_next;
    }

    let timeline_count: u64 = beams.iter().sum();

    println!("Part1, split count {split_count}");
    println!("Part2, timeline count {timeline_count}");
}

fn day08(contents: &String) {
    let mut coords: Vec<Point> = contents
        .lines()
        .map(|line| {
            let vec: Vec<i64> = line.split(",").map(|n| n.parse().unwrap()).collect();
            return Point {
                x: vec[0],
                y: vec[1],
                z: vec[2],
            };
        })
        .collect();

    let desired_connection_count = if coords.len() < 100 { 10 } else { 1000 }; // example = 10, real = 1000

    let mut distances: Vec<(Point, Point, i64)> = coords
        .iter()
        .tuple_combinations()
        .map(|(p1, p2)| (p1.clone(), p2.clone(), day08_dist(p1, p2)))
        .collect();

    // sort by distance
    distances.sort_by_key(|tup| tup.2);
    
    let mut circuits: Vec<HashSet<Point>> = vec![];
    let mut point_to_circuit: HashMap<Point, usize> = HashMap::new();

    for (idx, (p1, p2, _)) in distances.iter().enumerate() {
        let p1_circuit_opt = point_to_circuit.get(p1).copied();
        let p2_circuit_opt = point_to_circuit.get(p2).copied();

        match (p1_circuit_opt, p2_circuit_opt) {
            (Some(p1_circuit), Some(p2_circuit)) => {
                // both junction boxes already part of a circuit
                if p1_circuit == p2_circuit {
                    // same circuit, nothing changes for the composition, but this is a new connection!
                    continue;
                }

                // different circuits => merge them
                let p2_elements = mem::take(circuits.get_mut(p2_circuit).unwrap());
                for p2_entry in &p2_elements {
                    *point_to_circuit.get_mut(&p2_entry).unwrap() = p1_circuit;
                }
                let p1_set = circuits.get_mut(p1_circuit).unwrap();
                p1_set.extend(p2_elements.iter());
            }

            (Some(p1_circuit), None) => {
                // add p2 to circuit of p1
                point_to_circuit.insert(p2.clone(), p1_circuit);
                let set = circuits.get_mut(p1_circuit).unwrap();
                set.insert(p2.clone());
            }

            (None, Some(p2_circuit)) => {
                // add p1 to circuit of p2
                point_to_circuit.insert(p1.clone(), p2_circuit);
                let set = circuits.get_mut(p2_circuit).unwrap();
                set.insert(p1.clone());
            }

            (None, None) => {
                // both not in set, create new set
                let set_id = circuits.len();
                let mut set = HashSet::new();
                set.insert(p1.clone());
                set.insert(p2.clone());
                circuits.push(set);
                point_to_circuit.insert(p1.clone(), set_id);
                point_to_circuit.insert(p2.clone(), set_id);
            }
        }

        // for part 1 we care about a certain number of connections
        if idx == desired_connection_count - 1 {
            let circuit_sizes: usize = circuits
                .iter()
                .map(|s| s.iter().count())
                .sorted_by(|a, b| b.cmp(a)) // descending
                .take(3)
                .product();

            println!("Part1: product of 3 biggest circuit sizes {circuit_sizes}");
        }

        // for part 2 we want every box in the same circuit
        if circuits.iter().any(|set| set.len() == coords.len()) {
            // we have a set (circuit) with all coords (junction boxes) in
            let x_product = p1.x * p2.x;
            println!("Part2: finished the circuit at {p1:?} and {p2:?} for solution {x_product}");
            return;
        }
    }

    println!("No solution found for part 2");
}

fn day08_dist(p1: &Point, p2: &Point) -> i64 {
    let dx = p1.x - p2.x;
    let dy = p1.y - p2.y;
    let dz = p1.z - p2.z;
    return dx * dx + dy * dy + dz * dz;
}

#[derive(Clone, Debug, Hash, Eq, PartialEq, Copy)]
struct Point {
    x: i64,
    y: i64,
    z: i64,
}

fn day09(contents: &String) {
    let coords: Vec<(i64, i64)> = contents
        .lines()
        .map(|line| {
            let vec: Vec<i64> = line.split(",").map(|n| n.parse().unwrap()).collect();
            return (vec[0], vec[1]);
        })
        .collect();

    let mut max_surface_part1 = 0i64;
    let mut max_surface_part2 = 0i64;
    for (p1, p2) in coords.iter().tuple_combinations() {
        let surface = ((p1.0 - p2.0).abs() + 1) * ((p1.1 - p2.1).abs() + 1);
        if surface > max_surface_part1 {
            max_surface_part1 = surface;
        }

        if surface > max_surface_part2 {

            // for part 2 we will check if any line cuts this rectangle

            let x_min_rectangle = p1.0.min(p2.0);
            let x_max_rectangle: i64 = p1.0.max(p2.0);
            let y_min_rectangle = p1.1.min(p2.1);
            let y_max_rectangle = p1.1.max(p2.1);

            let mut cuts_rectangle = false;
            for idx in 0..coords.len() {
                let p3 = coords[idx];
                let p4 = coords[(idx + 1) % coords.len()];

                let x_min_line = p3.0.min(p4.0);
                let x_max_line: i64 = p3.0.max(p4.0);
                let y_min_line = p3.1.min(p4.1);
                let y_max_line = p3.1.max(p4.1);

                // does the line between p3 & p4 cut the rectangle between p1 & p2

                if p3.0 == p4.0 {
                    // Vertical line!

                    // Check if X is within bounds
                    if p3.0 > x_min_rectangle && p3.0 < x_max_rectangle {
                        
                        // Check for Y-axis overlap
                        // We find the 'highest' start point and the 'lowest' end point
                        let overlap_start = y_min_line.max(y_min_rectangle);
                        let overlap_end = y_max_line.min(y_max_rectangle);

                        // If start is less than end, they overlap!
                        if overlap_start < overlap_end {
                            cuts_rectangle = true;
                            break;
                        }
                    }
                }
                else {
                    // Horizontal line!
                    
                    if p3.1 > y_min_rectangle && p3.1 < y_max_rectangle {
                        let overlap_start = x_min_line.max(x_min_rectangle);
                        let overlap_end = x_max_line.min(x_max_rectangle);

                        if overlap_start < overlap_end {
                            cuts_rectangle = true;
                            break;
                        }
                    }
                }
            }

            if !cuts_rectangle {
                max_surface_part2 = surface;
            }
        }
    }

    println!("Part 1 max surface area of rectangle {max_surface_part1}");
    println!("Part 2 max surface area of rectangle {max_surface_part2}");
}

fn day10(contents: &String) { 
    let mut total_presses_part1 = 0;
    let mut total_presses_part2 = 0;

    for (idx, line) in contents.lines().enumerate() {
        // parse line
        // [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
        let close_square_bracket = line.chars().position(|c| c == ']').unwrap();
        let open_curly_bracket = line.chars().position(|c| c == '{').unwrap();

        let target_lights: Vec<u32> = line[1..close_square_bracket]
            .chars()
            .map(|c| if c == '#' {1} else {0})
            .collect();
        let buttons: Vec<Vec<u32>> = line[(close_square_bracket + 2)..(open_curly_bracket - 1)]
            .trim()
            .split(" ")
            .map(|b| b[1..(b.len()-1)].split(",").map(|i| i.parse().unwrap()).collect::<Vec<usize>>())
            .map(|indices| day10_encoding(&indices, target_lights.len()))
            .collect();
        let target_joltages: Vec<u32> = line[(open_curly_bracket + 1)..(line.len() - 1)]
            .split(",")
            .map(|i| i.parse().unwrap())
            .collect();
        
        total_presses_part1 += day10_recursive(&target_lights, &buttons, true);
        total_presses_part2 += day10_recursive(&target_joltages, &buttons, false);
    }

    println!("Part 1: total presses {total_presses_part1}");
    println!("Part 2: total presses {total_presses_part2}");
}

fn day10_encoding(indices: &Vec<usize>, target_len: usize) -> Vec<u32> {
    let mut result = vec![0; target_len];

    for idx in indices {
        result[*idx] = 1;
    }

    result
}

fn day10_recursive(targets: &Vec<u32>, buttons: &Vec<Vec<u32>>, oddness_only: bool) -> u32 {
    let mut best_button_count = u32::MAX;

    if targets.iter().all(|&t| t == 0) {
        return 0;
    }

    let mut reduce_factor = 1u32;
    let mut reduced_targets = targets.clone();
    while reduced_targets.iter().all(|&i|i % 2 == 0) {
        reduce_factor *= 2;
        reduced_targets = reduced_targets.iter().map(|i| i / 2).collect();
    }

    let target_mask: Vec<bool> = reduced_targets.iter().map(|u| u % 2 == 1).collect();
    let choices = buttons.iter().map(|x| vec![Some(x), None]);

    for combination in choices.multi_cartesian_product() {
        let mut somes = combination.iter().flatten().copied();
        let mut button_count = 1u32; // first vec is not iterated
        if let Some(first_vec) = somes.next() {
            let result: Vec<u32> = somes.fold(first_vec.clone(), |mut acc, next_vec| {
                button_count += 1;
                for (a, b) in acc.iter_mut().zip(next_vec.iter()) {
                    *a += b;
                }
                acc
            });

            let result_mask: Vec<bool> = result.iter().map(|u| u % 2 == 1).collect();

            if result_mask != target_mask {
                continue;
            }

            // valid combination to get the correct oddness (mask)

            if oddness_only {
                if button_count < best_button_count {
                    best_button_count = button_count;
                }
                continue;
            }

            // not interested in oddness only, need exact match
            // reduce the problem with this combination and recursively solve

            if result.iter().zip(&reduced_targets).any(|(res, tar)| res > tar) {
                // not a real sln as we are already overshooting
                continue;
            }

            let sub_targets = reduced_targets
                .iter()
                .zip(&result)
                .map(|(tar, res)| tar - res)
                .collect();

            let sub_button_count = day10_recursive(&sub_targets, buttons, oddness_only);

            if sub_button_count == u32::MAX {
                continue; // no sln for sub problem
            }

            button_count += sub_button_count;

            if button_count < best_button_count {
                best_button_count = button_count;
            }
        }
    }

    if best_button_count == u32::MAX {
        return best_button_count;
    }
    else {
        return reduce_factor * best_button_count;
    }
}

fn day11(contents: &String) {
    let mut node_ids: HashMap<String, u32> = HashMap::new();
    let mut next_node_id = 0;

    let mut get_id = |name: &str| -> u32 {
        *node_ids.entry(name.to_string()).or_insert_with(|| {
            let id = next_node_id;
            next_node_id += 1;
            id
        })
    };

    let mut connections: HashMap<u32, HashSet<u32>> = HashMap::new();
    let mut connections_reverse: HashMap<u32, HashSet<u32>> = HashMap::new();

    for line in contents.lines() {
        let mut parts: VecDeque<u32> = line
            .replace(":", "")
            .split(" ")
            .map(|s| get_id(s))
            .collect();
        
        let src = parts.pop_front().unwrap();
        let set: HashSet<u32> = parts.into_iter().collect();

        // reverse connections
        for &dst in &set {
            connections_reverse
                .entry(dst)
                .or_insert(HashSet::new())
                .insert(src);
        }
        
        // regular connection
        connections.insert(src, set);

        // in case this has no in connections, also add with empty set to reverse
        connections_reverse.entry(src).or_insert(HashSet::new());
    }

    // do Topological sort with Kahn’s Algorithm
    let mut indegrees: HashMap<u32, u32> = connections_reverse
        .iter()
        .map(|(&node_id, inset)| (node_id, inset.len() as u32))
        .collect();

    let mut nodes_sorted = vec![];
    let mut queue_sort: VecDeque<u32> = VecDeque::new();
    for (&node_id, &indegree) in &indegrees {
        if indegree == 0 {
            queue_sort.push_back(node_id);
        }
    }

    while let Some(next_node) = queue_sort.pop_front() { 
        nodes_sorted.push(next_node);

        if let Some(set) = connections.get(&next_node) {
            for &connection in set {
                if let Some(count) = indegrees.get_mut(&connection) {
                    *count -= 1;
                    if *count == 0 {
                        queue_sort.push_back(connection);
                    }
                }
            }
        }
    }

    let paths_part1 = day11_path_count(
        &nodes_sorted, 
        &connections_reverse, 
        get_id("you"),
        get_id("out"));

    println!("Part 1: #paths {paths_part1}");

    // for part 2 we need not all paths from start to finish (here svr and out)
    // but they must also pass by dac & fft in any order.
    // this can only happen in 2 combinations
    // svr -> fft -> dac -> out
    // svr -> dac -> fft -> out
    // for both options we can use the same algorithm to find the number of paths between each step and then multiply

    let svr_fft = day11_path_count(
        &nodes_sorted, 
        &connections_reverse, 
        get_id("svr"),
        get_id("fft"));

    let svr_dac = day11_path_count(
        &nodes_sorted, 
        &connections_reverse, 
        get_id("svr"),
        get_id("dac"));

    let fft_dac = day11_path_count(
        &nodes_sorted, 
        &connections_reverse, 
        get_id("fft"),
        get_id("dac"));

    let dac_fft = day11_path_count(
        &nodes_sorted, 
        &connections_reverse, 
        get_id("dac"),
        get_id("fft"));

    let fft_out = day11_path_count(
        &nodes_sorted, 
        &connections_reverse, 
        get_id("fft"),
        get_id("out"));

    let dac_out = day11_path_count(
        &nodes_sorted, 
        &connections_reverse, 
        get_id("dac"),
        get_id("out"));

    let paths_part2 = svr_fft * fft_dac * dac_out + svr_dac * dac_fft * fft_out;

    println!("Part 2: #paths {paths_part2}");
}

fn day11_path_count(nodes_sorted: &Vec<u32>, connections_reverse: &HashMap<u32, HashSet<u32>>, src: u32, dst: u32) -> u64 {
    let mut path_counts: HashMap<u32, u64> = HashMap::new();
    path_counts.insert(dst, 1);

    for &node in nodes_sorted.iter().rev() {

        if node == src {
            break; // know enough, no point in continuing
        }

        let Some(&node_count) = path_counts.get(&node) else {
            continue; // count is 0, nothing to do
        };

        if let Some(set) = connections_reverse.get(&node) {
            for &connection in set {
                *path_counts.entry(connection).or_insert(0) += node_count;
            }
        } 
    }

    *path_counts.get(&src).unwrap_or(&0)
}

fn day12(contents: &String) {
    // parse input
    let mut pieces: Vec<Piece> = vec![];
    let mut regions: Vec<(u32, u32, Vec<u32>)> = vec![];
    let mut current_piece: Vec<Vec<bool>> = vec![];
    for line in contents.lines() {
        if line.len() <= 3 {
            //eg 
            // 3:
            // .##
            // ##.
            // #..
            // 
            // 5:

            if line.is_empty() {
                let mut form_iter = current_piece.into_iter().flatten();
                let form: [bool; 9] = std::array::from_fn(|_| form_iter.next().unwrap_or(false));
                let area = form.iter().map(|&b| b as u32).sum();

                pieces.push(Piece { form, area });
                current_piece = vec![];
            }
            else if line.len() == 2 {
                continue;
            }
            else {
                let line_vec = line.chars()
                    .map(|c| c == '#')
                    .collect();

                current_piece.push(line_vec);
            }
        }
        else {
            // eg  35x43: 24 20 20 32 26 32
            let parts: Vec<_> = line.split(": ").collect();
            let bounds: Vec<u32> = parts[0].split("x").map(|f| f.parse().unwrap()).collect();
            let counts: Vec<u32> = parts[1].split(" ").map(|f| f.parse().unwrap()).collect();
            regions.push((bounds[0], bounds[1], counts));
        }
    }

    let mut region_fit_count = 0;
    for (width, height, present_counts) in &regions {
        // brute force very slow
        // there are some low effort calculations we can do for upper and lower bounds
        // 
        // 1) every present fits in a 3x3 so if we just put all presents next to eachother and not overlap any 3x3 area, we can definitely fit all presents
        // 2) if the area of every present summed up is more than the region are, it definitely does NOT fit

        let total_present_count: u32 = present_counts.iter().sum();
        let possible_non_overlapping_present_count = (width / 3) * (height / 3);

        if total_present_count <= possible_non_overlapping_present_count {
            // will always fit as we can just the presents 3x3 bounding boxes next to eachother
            region_fit_count += 1;
            continue;
        }

        let min_required_present_area: u32 = present_counts
            .iter()
            .zip(&pieces)
            .map(|(count, piece)| count * piece.area)
            .sum();

        let available_area = width * height;
        if available_area < min_required_present_area {
            // can never fit
            continue;
        }

        // Assume it fits :)
        region_fit_count += 1;

        // turns out in the real input (not the sample) there are no such cases where you need
        // to actually pack presents in an area (really complicated)
    }

    println!("Part1: count {region_fit_count}");
}

struct Piece {
    form: [bool; 9],
    area: u32,
}