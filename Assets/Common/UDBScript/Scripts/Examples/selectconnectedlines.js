/// <reference path="../../udbscript.d.ts" />

`#version 4`;

`#name Select Connected Linedefs`;

`#description Recursively selects all linedefs that are connected to the currently selected linedef(s).`;

let lines = UDB.Map.getSelectedOrHighlightedLinedefs();

if(lines.length == 0)
    UDB.die('You have to select at least one linedef!');

let vertices_to_check = [];
let checked_vertices = [];

// Add vertices of the selected linedefs to the list of vertices to check
lines.forEach(ld => {
    if(!vertices_to_check.includes(ld.start))
        vertices_to_check.push(ld.start);

    if(!vertices_to_check.includes(ld.end))
        vertices_to_check.push(ld.end);
});

// Go through all vertices
while(vertices_to_check.length != 0)
{
    // Get the last vertex in the list
    let v = vertices_to_check.pop();

    // Add vertex to checked vertices
    checked_vertices.push(v);

    // Loop through all linedefs that are connected to the vertex
    v.getLinedefs().forEach(ld => {
        ld.selected = true;

        // Add start vertex of the linedef to the list of vertices to check, but only if
        // it has not been checked before and isn't already in the list of vertices to check.
        // If we don't do that we might loop through vertices indefinitely
        if(!vertices_to_check.includes(ld.start) && !checked_vertices.includes(ld.start))
            vertices_to_check.push(ld.start);

        // Do the same for the end vertex of the linedef
        if(!vertices_to_check.includes(ld.end) && !checked_vertices.includes(ld.end))
            vertices_to_check.push(ld.end);
    });
}
