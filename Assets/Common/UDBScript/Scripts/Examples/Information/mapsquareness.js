/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name Show Map Squareness`;

`#description Shows how square a map is. Computes the percentage of orthogonal lines, based on their length.`;

let totalLength = 0;
let orthogonalLength = 0;

UDB.Map.getLinedefs().forEach(ld => {
    totalLength += ld.length;
    
    if(ld.angle % 90 == 0)
        orthogonalLength += ld.length;
});

orthogonalPercentage = (orthogonalLength / totalLength * 100).toFixed(2);

UDB.showMessage('The map is ' + orthogonalPercentage + '% square.');