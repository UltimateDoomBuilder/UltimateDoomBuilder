/// <reference path="../../udbscript.d.ts" />

`#version 4`;

`#name Delete Sector Tag`;

`#description Deletes a tag from the selected sectors (or all sectors if no sectors are selected)`;

// Get the selected sectors
let sectors = UDB.Map.getSelectedSectors();

// If no sectors were selected get all sectors
if(sectors.length == 0)
    sectors = UDB.Map.getSectors();

// Prepare to ask the user for the tag to delete
let qo = new UDB.QueryOptions();
qo.addOption('tag', 'Tag to delete', 1, 0);

// Ask for the tag to delete, abort script of cancel was pressed
if(!qo.query())
    UDB.die('Script aborted');

// Abort when tag was set to 0
if(qo.options.tag == 0)
    UDB.die("Tag can't be 0");

// Delete the tag from the sectors
sectors.forEach(s => s.removeTag(qo.options.tag));