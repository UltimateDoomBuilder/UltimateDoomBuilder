/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name Create PolyObject`;

`#description Creates a poliyobject from a selected line. The anchor will be placed at the mouse cursor.`;

`#scriptoptions

startspottype
{
	description = "Start spot type";
	type = 11;
	default = 9301;
	enumvalues {
		9301 = "Regular";
		9302 = "Crushing";
		9303 = "Hurts to touch";
	}
}
`;

// Check if the mouse is in the map
if(!UDB.Map.mousePosition.isFinite())
	UDB.die('Mouse cursor is not at a valid map position');

// Get the mouse position in the map, snapped to the grid
var cursorpos = UDB.Map.snappedToGrid(UDB.Map.mousePosition);

// The polyobject number that will be used
var polyobjectnumber = -1;

// Get selected linedefs
var lines = UDB.Map.getSelectedLinedefs();

// Make sure exactly one linedef is selected
if(lines.length != 1)
	throw 'You have to select exactly 1 line';

// This stores polyobject numbers that are already used
var usednumbers = []

// Find all polyobject things and get their polyobject numbers
UDB.Map.getThings().filter(o => o.type >= 9300 && o.type <= 9303).forEach(o => {
		usednumbers.push(o.angle);
});

// Find the first free polyobject number
for(var i=1; i < 360; i++) {
	if(!usednumbers.includes(i)) {
		polyobjectnumber = i;
		break;
	}
}

// Make sure we actually found a free polyobject number
if(polyobjectnumber == -1)
	throw 'No free Polyobject numbers!';

// Set the line action and argument, and get the position where the
// polyobject anchor thing will be placed
lines[0].action = 1; // Polyobject Start Line
lines[0].args[0] = polyobjectnumber;
var anchorpos = lines[0].line.getCoordinatesAt(0.5); // Center of line

// Create the polyobject start spot thing
var t = UDB.Map.createThing(cursorpos, UDB.ScriptOptions.startspottype);
t.angle = polyobjectnumber;

// Create the polyobject anchor thing
t = UDB.Map.createThing(anchorpos, 9300); // 9300 = Polyobject Anchor
t.angle = polyobjectnumber