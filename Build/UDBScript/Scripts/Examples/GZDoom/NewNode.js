/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name New Path Node`;

`#description Creates a new node and assigns it a TID. If a Path Node is already selected, connects the two automatically.`;

`#scriptoptions

gridsnap
{
	description = "Grid Snap";
	default = 1;
	type = 11; // enum
	enumvalues {
		0 = "Disabled";
		1 = "Enabled";
	}
}
`;


let mpos = UDB.Map.mousePosition;

if(!mpos.isFinite())
	UDB.die('Mouse cursor must be inside the map');


let nodetype = 9022;
let ntag = UDB.Map.getNewTag();
let things = UDB.Map.getSelectedThings();
	
let node = UDB.Map.createThing(mpos, nodetype);
if (UDB.ScriptOptions.gridsnap > 0)
	node.snapToGrid();
node.tag = ntag;

let i = 0;
let count = 0;
if(things.length > 0)
{
	things.forEach(n =>
	{	
		if (n.type == nodetype)
		{
			// For the new node, assign the path IDs to it, up to max arguments.
			if (n.tag != 0 && count < 5)
			{
				node.args[count] = n.tag;
				count++;
			}
			// For the selected nodes, check for an empty slot.
			let pos = -1;
			
			for(i = 0; i < 5; i++)
			{
				if (n.args[i] != 0)
					continue;
				
				pos = i;
				break;
			}
			
			
			
			// Check if the tag is already used first.
			if (pos >= 0) 
			{
				for (i = 0; i < 5; i++)
				{
					if (n.args[i] == ntag)
					{
						pos = -1;
						break;
					}
				}
			}
			// Free slot, and unpresent. Set it in.
			if (pos >= 0)
				n.args[pos] = ntag;
		}
	});
}

UDB.Map.clearSelectedThings();
node.selected = true;
