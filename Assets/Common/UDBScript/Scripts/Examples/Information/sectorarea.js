/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name Calculate sector area`;

`#description Calculates the area of the selected sectors.`

let sectors = UDB.Map.getSelectedSectors();

if(sectors.length == 0)
UDB.die('You need to select at least one sector.');

let area = 0;

sectors.forEach(s => {
    s.getTriangles().forEach(t => {
        area += 0.5 * Math.abs(t[0].x * (t[1].y - t[2].y) + t[1].x * (t[2].y - t[0].y) + t[2].x * (t[0].y - t[1].y));
    });
});

UDB.showMessage('The area of the selected sectors is ' + area + ' mu².');