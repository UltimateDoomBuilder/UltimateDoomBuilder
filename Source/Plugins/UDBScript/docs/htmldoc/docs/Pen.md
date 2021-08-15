# Pen

The `Pen` class is an built-in library. It can be used to to create geometry [Turtle graphics](https://en.wikipedia.org/wiki/Turtle_graphics) style.

The class is inspired of the `Pen` class in Doom Builder X.

Example:

```js
// Draw a regular pentagon
let p = new Pen([0, 0]);

for(let i=0; i < 5; i++)
{
	p.drawVertex();
	p.moveForward(128);
	p.turnRight(72);
}

p.finishDrawing();
```

## Constructors

### Pen(pos)
Creates an instance of the `Pen` class. The position can be a `Vector2D` or an `Array` of two numbers.
#### Parameters
* pos: start position of the pen (optional)
#### Return value
An instance of the `Pen` class

## Methods

### moveForward(distance)
Moves the pen by a distance at the current angle.
#### Parameters
* distance: number of units to move

### moveTo(pos)
Moves the pen to the given position. The position can be a `Vector2D` or an `Array` of two numbers.
#### Parameters
* pos: position to move the pen to

### setAngle(degrees)
Sets the angle to the given degrees.
#### Parameters
* degrees: degrees to set the angle to

### setAngleRadians(radians)
Sets the angle to the given radians.
#### Parameters
* radians: radians to set the angle to

### turnLeft(degrees)
Turns the pen left by the given degrees.
#### Parameters
* degrees: degrees to turn left by. If omitted it will turn by 90°

### turnLeftRadians(radians)
Turns the pen left by the given radians.
#### Parameters
* radians: radians to turn left by. If omitted it will turn by Pi/2

### turnRight(degrees)
Turns the pen right by the given degrees.
#### Parameters
* degrees: degrees to turn right by. If omitted it will turn by 90°

### turnRightRadians(radians)
Turns the pen right by the given radians.
#### Parameters
* radians: radians to turn right by. If omitted it will turn by Pi/2

### drawVertex()
Draws a `Vertex` at the current position.

### finishDrawing()
Finishes the drawing, actually creating the geometry. Also resets the vertices of this instance of `Pen`.