/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name Connect Nodes`;

`#description Connects nodes to/from the first selected PathNode.`;

`#scriptoptions

direction
{
	description = "Assignment Direction";
	default = 2;
	type = 11; // Enum
	enumvalues {
		0 = "To First";
		1 = "From First";
		2 = "Both";
	}
}

doclear
{
	description = "Clear first thing's arguments";
	default = "False";
	type = 3; // Boolean
}

`;

let things = UDB.Map.getSelectedThings();

if(things.length < 2)
    UDB.die('You have to select at least 2 things.');

let dir = UDB.ScriptOptions.direction;
let clr = UDB.ScriptOptions.doclear;

let receiver = things[0];

let pos = 0;

let i = 0;
if (clr)
{
	for (i; i < 5; i++)
		receiver.args[i] = 0;
}

things.forEach(n =>
{
	if (n != receiver)
	{
		if (dir > 0)
		{
			// look for the tid first, make sure it's not already assigned.
			let found = false;
			for (i = 0; i < 5; i++)
			{
				if (n.args[i] == receiver.tag)
				{
					found = true;
					break;
				}
			}
			// Not found, so assign it.
			if (!found) for (i = 0; i < 5; i++)
			{
				
				if (n.args[i] == 0)
				{
					n.args[i] = receiver.tag;
					break;
				}
			}
			
		}
		if ((dir == 0 || dir == 2) && (pos < 5 && n.tag != 0))
		{
			
			if (clr)	// No special management necessary.
			{
				receiver.args[pos] = n.tag;
				pos++;
			}
			else // Look for a free spot.
			{
				let found = false;
				for (i = 0; i < 5; i++)
				{
					if (receiver.args[i] == n.tag)
					{
						found = true;
						break;
					}
				}
				if (!found) 					
				{
					for (i = pos; i < 5; i++)
					{
						if (receiver.args[i] == 0)
						{
							receiver.args[i] = n.tag;
							pos = i;
							found = true;
							break;
						}
					}
				}
			}
		}
	}
});