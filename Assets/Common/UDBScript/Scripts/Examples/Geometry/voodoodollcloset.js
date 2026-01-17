/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name Draw Voodoo Doll Closet`;

`#description Draws a voodoo doll closet from the mouse cursor's position. Requires Boom actions. If linedefs are selected when the script is run, those linedefs will have actions assigned that unblock the voodoo doll: if they have a SW1/SW2 texture a S1 action will be applied, if they are double-sided a W1 action will be applied.`;

`#scriptoptions

length
{
	description = "Length of closet";
	default = 256;
	type = 0; // Integer
}

direction
{
	description = "Direction of closet";
	default = 0;
	type = 11; // Enum
	enumvalues {
		0 = "North";
		1 = "East";
		2 = "South";
		3 = "West";
	}
}

inactive
{
	description = "Start inactive";
	default = "True";
	type = 3; // Boolean
}

looping
{
	description = "Looping";
	default = "False";
	type = 3; // Boolean
}
`;

// Make sure the has a correct minimum length
if(UDB.ScriptOptions.length < 96)
	throw 'Voodoo doll closet has to be at least 96 map units long!';

// Get the mouse position in the map, snapped to the grid
let basepos = UDB.Map.snappedToGrid(UDB.Map.mousePosition);

// Closet width is static
let closetwidth = 64;

// Get the currently selected lines. Those will get actions to release the voodoo doll
let triggerlines = UDB.Map.getSelectedLinedefs();

// The number of tags we need depend on the selected options
let numnewtags = 1;
let newtagindex = 0;

// We need an additional tag if the player is blocked at the beginning
if(UDB.ScriptOptions.inactive)
	numnewtags++;

// We need an additional tag if the closet should be looping
if(UDB.ScriptOptions.looping)
	numnewtags++;

// Get thew new tags
let tags = UDB.Map.getMultipleNewTags(numnewtags);

// Create a pen for drawing geometry
var p = new Pen();

// Draw the closet
p.setAngle(90 * UDB.ScriptOptions.direction).moveTo(basepos).drawVertex()
.moveForward(UDB.ScriptOptions.length).drawVertex().turnRight()
.moveForward(closetwidth).drawVertex().turnRight()
.moveForward(UDB.ScriptOptions.length).drawVertex();

if(!p.finishDrawing(true))
	throw "Something went wrong while drawing!";

// Get the new sector, assign a tag, and set heights
let sector = UDB.Map.getMarkedSectors()[0];
sector.tag = tags[newtagindex];
sector.floorHeight = 0;
sector.ceilingHeight = 56;

// Draw the carrying line
p.setAngle(90 * UDB.ScriptOptions.direction).moveTo(basepos).drawVertex()
.moveForward(32).drawVertex();

if(!p.finishDrawing())
	throw 'Something went wrong while drawing!';

// Assign the action and tag to the line
let carryline = UDB.Map.getMarkedLinedefs()[0];
carryline.action = 252;
carryline.tag = tags[newtagindex];

// Increment the new tag index, so that the next new tag will be used for the next step
newtagindex++;

// Create the player blocking geometry if necessary
if(UDB.ScriptOptions.inactive)
{
	// Draw the blocking sector
	p.setAngle(90 * UDB.ScriptOptions.direction).moveTo(basepos)
	.moveForward(64).turnRight().moveForward(16).drawVertex()
	.turnRight().moveForward(8).drawVertex()
	.turnLeft().moveForward(closetwidth - 32).drawVertex();
	
	if(!p.finishDrawing(true))
		throw "Something went wrong while drawing!";
	
	// Get the new sectors and assign a tag
	sector = UDB.Map.getMarkedSectors()[0];
	sector.tag = tags[newtagindex];
	sector.floorHeight = 0;
	sector.ceilingHeight = 55;

	// Assign actions to release the voodoo doll to the previosly selected lines. If the line has a texture
	// starting with SW1 or SW2 a switch action will be applied. Otherwise a walk-over action is applied (but only if
	// it's a 2-sided line)
	triggerlines.forEach(tl => {
			if(	tl.front.upperTexture.startsWith('SW1') || tl.front.upperTexture.startsWith('SW2') ||
				tl.front.middleTexture.startsWith('SW1') || tl.front.middleTexture.startsWith('SW2') ||
				tl.front.lowerTexture.startsWith('SW1') || tl.front.lowerTexture.startsWith('SW2')
				)
			{
				tl.action = 166; // S1 Ceiling Raise to Highest Ceiling
				tl.tag = tags[newtagindex];
			}
			else if(tl.back != null)
			{
				tl.action = 40; // W1 Ceiling Raise to Highest Ceiling
				tl.tag = tags[newtagindex];
			}
	});

	// Increment the new tag index, so that the next new tag will be used for the next step
	newtagindex++;
}

// Create the looping teleporter geometry if necessary
if(UDB.ScriptOptions.looping)
{
	// Create the teleport destination line
	p.setAngle(90 * UDB.ScriptOptions.direction).moveTo(basepos)
	.moveForward(32).turnRight().moveForward(8).drawVertex()
	.moveForward(closetwidth - 16).drawVertex();

	if(!p.finishDrawing())
		throw 'Something went wrong while drawing!';
	
	// The destination line only needs a tag and no action
	let line = UDB.Map.getMarkedLinedefs(true)[0];
	line.tag = tags[newtagindex];
	
	// Create the teleport line
	p.setAngle(90 * UDB.ScriptOptions.direction)
	.moveTo(basepos)
	.moveForward(UDB.ScriptOptions.length - 32).turnRight().moveForward(8).drawVertex()
	.moveForward(closetwidth - 16).drawVertex();
	
	if(!p.finishDrawing())
		throw 'Something went wrong while drawing!';
	
	// The teleport line needs a tag and an action
	line = UDB.Map.getMarkedLinedefs(true)[0];
	line.action = 263;
	line.tag = tags[newtagindex];
}

// Compute the new voodoo doll position
let newpos = new UDB.Vector2D(32, 32).getRotated(UDB.Angle2D.doomToReal(-90 * UDB.ScriptOptions.direction - 90)) + basepos;

// Get all player 1 starts
let playerthings = UDB.Map.getThings().filter(o => o.type == 1);

// The actual player always spawns on the last player 1 start that was placed, so
// we need to move the last player 1 start to the monster closet and create a new
// player 1 start at the old position
if(playerthings.length > 0)
{
	// Sort them by their index, so that the first element is that last player 1 start
	let pt = playerthings.sort((a, b) => b.index - a.index)[0];

	// Store old position and angle and move the last player 1 start to the closet
	let oldpos = pt.position;
	let oldangle = pt.angle;
	
	pt.position = newpos;
	pt.snapToAccuracy();

	// Create a new player 1 start and move it to the old position	
	let t = UDB.Map.createThing(oldpos, 1);
	t.angle = oldangle;
}
else
{
	let t = UDB.Map.createThing(newpos, 1);
	t.snapToAccuracy();
}