extends Node2D

@export_group("Marching Squares Settings")
@export var resolution: int = 8
@export var smooth:bool = true

@export_group("Noise3D")
@export var noise3d: FastNoiseLite

var grid = []

var height: int = 600
var width: int = 800

var cols: int
var rows: int

func _ready() -> void:
	cols = 1 + width / resolution
	rows = 1 + height / resolution
	grid.resize(cols * rows)

	for y in range(rows):
		for x in range(cols):
			var val = noise3d.get_noise_3d(x, y, 0)
			grid[x + (y * cols)] = val

func _process(delta: float) -> void:
	for y in range(rows):
		for x in range(cols):
			var val = noise3d.get_noise_3d(x, y, Engine.get_frames_drawn())
			grid[x + (y * cols)] = val
	queue_redraw()
	return

func get_state(a: float, b: float, c: float, d: float) -> int:
	var state = 0
	if a > 0: state += 8
	if b > 0: state += 4
	if c > 0: state += 2
	if d > 0: state += 1
	return state

func l(a: float, b: float) -> float:
	if !smooth:
		return 0.5
	
	if abs(a - b) < 0.00001:
		return 0.5
	return (0 - a) / (b - a)

	
func _draw() -> void:
	for y:int in range(rows - 1):
		for x:int in range(cols - 1):
			var i: int = x * resolution
			var j: int = y * resolution

			var p1: float = grid[x + y * cols]
			var p2: float = grid[x + 1 + y * cols]
			var p3: float = grid[x + 1 + (y + 1) * cols]
			var p4: float = grid[x + (y + 1) * cols]

			var state = get_state(p1, p2, p3, p4)

			var a: Vector2 = Vector2(i + l(p1, p2) * resolution, j)
			var b: Vector2 = Vector2(i + resolution, j + l(p2, p3) * resolution)
			var c: Vector2 = Vector2(i + l(p4, p3) * resolution, j + resolution)
			var d: Vector2 = Vector2(i, j + l(p1, p4) * resolution)

			match state:
				1: drawln(c, d)
				2: drawln(b, c)
				3:
					drawln(b, d)
				4: drawln(a, b)
				5:
					drawln(a, d)
					drawln(b, c)
				6: drawln(a, c)
				7: drawln(a, d)
				8: drawln(a, d)
				9: drawln(a, c)
				10:
					drawln(a, b)
					drawln(c, d)
				11: drawln(a, b)
				12: drawln(b, d)
				13: drawln(b, c)
				14: drawln(c, d)

func drawln(a: Vector2, b: Vector2) -> void:
	draw_line(a, b, Color.WHITE, 1)
