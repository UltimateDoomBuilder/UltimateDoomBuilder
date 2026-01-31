/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name Create Line Portal`;

`#description Creates a quick portal (line action 301) between the selected lines. Only works in UDMF. Only works when exactly two lines are selected. Both lines must have exactly the same length. Draws a sector at the back of the selected lines.`;

`#scriptoptions

depth
{
	description = "Depth of space behind portal";
	default = 64;
	type = 0; // Integer
}

texture
{
	description = "Filler texture";
	default = "FIREBLU1";
	type = 6; // Texture
}
`;

// Function to draw the space behind the portal line
function drawPortalSector(line, depth)
{
    // The origin is the bottom left point of the rectanble
    let origin = line.v1;

    // The base points of the rectangle. There are five because
    // the first and last point need to be at the same position
    // to get a complete drawing
    let points = [
        new UDB.Vector2D(0, 0),
        new UDB.Vector2D(0, depth),
        new UDB.Vector2D(line.getLength(), depth),
        new UDB.Vector2D(line.getLength(), 0),
        new UDB.Vector2D(0, 0)
    ];

    // The points need to be rotated by the line's angle and moved
    // by the origin's position
    for(let i=0; i < points.length; i++)
    {
        let v = points[i].getRotated(line.getAngle() - 90);
        points[i] = origin + v;
    }

    // Draw the lines
    if(!UDB.Map.drawLines(points))
        throw 'Failed drawing space behind line ' + line;
    
    // Set the front middle texture for the new 1-sided linedefs
    UDB.Map.getMarkedLinedefs().filter(ld => ld.back == null).forEach(ld => ld.front.middleTexture = UDB.ScriptOptions.texture)
}

// The line action for portals only works in UDMF (GZDoom and Eternity Engine)
if(!UDB.Map.isUDMF)
    throw 'This script only works in UDMF maps';

// Get selected linedefs
let lines = UDB.Map.getSelectedLinedefs();

// Make sure exactly two lines are selected
if(lines.length != 2)
    throw 'You need to select exactly two lines';

// Both lines have to have the same length, otherwise the portal will
// be broken
if(lines[0].line.getLength() != lines[1].line.getLength())
    throw 'Both lines need to have the same length';

// Get a new tag to use for the portal
let newtag = UDB.Map.getNewTag();

// Set the action, arg, and tag of both lines
lines[0].action = lines[1].action = 301; // Line_QuickPortal
for(let i=0; i <= 4; i++) lines[0].args[i] = lines[1].args[i] = 0;
lines[0].tag = lines[1].tag = newtag;

// Draw the sectors behind the portal
drawPortalSector(lines[0].line, UDB.ScriptOptions.depth);
drawPortalSector(lines[1].line, UDB.ScriptOptions.depth);