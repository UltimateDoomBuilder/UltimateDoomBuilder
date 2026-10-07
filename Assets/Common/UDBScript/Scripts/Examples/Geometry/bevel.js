/// <reference path="../../../udbscript.d.ts" />

// Inspired by ribbiks's DBX Lua script: https://github.com/ribbiks/doom_lua/

`#version 4`;

`#name Bevel Linedefs`;

`#description Bevels linedefs at their shared vertices. Only works when the vertices have only two linedefs connected`;

`#scriptoptions

size
{
    description = "Bevel size";
    type = 0;
    default = 32;
}
`;

let lines = UDB.Map.getSelectedLinedefs();

if(lines.length < 2)
    die('You need to select at least 2 connected linedefs');

let vertices = new Set();

// Collect all vertices that has exactly 2 linedefs and both of those are selected
lines.forEach(ld => [ ld.start, ld.end ].forEach(v => {
        let vertexlines = v.getLinedefs();

        if(vertexlines.length == 2 && vertexlines.every(ld2 => ld2.selected))
            vertices.add(v);
    })
);

// Go through all collected vertices
vertices.forEach(v => {
    // Split all lines at the given size from the vertex away
    v.getLinedefs().forEach(ld => {
        if(ld.start == v)
            ld.split(ld.line.getCoordinatesAt(1.0 / ld.length * UDB.ScriptOptions.size));
        else
            ld.split(ld.line.getCoordinatesAt(1.0 - (1.0 / ld.length * UDB.ScriptOptions.size)));
    });

    // Get one of the connected linedef...
    let ld = v.getLinedefs()[0];

    // ... and join the current vertex into the linedef's closer vertex
    if(ld.start == v)
        v.join(ld.end);
    else
        v.join(ld.start);
});