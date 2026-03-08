/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name Logic assertions`;

`#description This script checks if UDBScript's logic (like vector math operations) work correctly.`;

let assertion_log = '';

function assert(condition, expectation, comp_result = true)
{
    let result = Function('return (' + condition + ' == ' + expectation + ') == ' + comp_result.toString())();
    let actual = Function('return (' + condition + ')')();
    let expectation_actual = Function('return (' + expectation + ')')();

    if(result == false)
    {
        assertion_log += condition + ' -> ' + actual + ' --> FAILED\n';
        throw 'Assertion failed for condition:\n\n' + condition + '\n\nExpected:\n\n' + expectation_actual + "\n\nGot:\n\n" + actual;
    }
    else
    {
        assertion_log += condition + ' -> ' + actual + ' --> OK\n';
    }
}

try
{
    // Addition - Vector2D
    assert('new UDB.Vector2D(2, 3) + new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(4, 5)');
    assert('[ 2, 3 ] + new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(4, 5)');
    assert('{ x: 2, y: 3 } + new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(4, 5)');
    assert('new UDB.Vector2D(2, 3) + [ 2, 2 ]', 'new UDB.Vector2D(4, 5)');
    assert('new UDB.Vector2D(2, 3) + { x: 2,  y: 2 }', 'new UDB.Vector2D(4, 5)');
    assert('new UDB.Vector2D(2, 3) + 2', 'new UDB.Vector2D(4, 5)');
    assert('2 + new UDB.Vector2D(2, 3)', 'new UDB.Vector2D(4, 5)');
    assert('new UDB.Vector2D(2, 3) + "#"', '"2, 3#"');
    assert('"#" + new UDB.Vector2D(2, 3)', '"#2, 3"');
    assert('"#" + new UDB.Vector2D(2, 3) + "#"', '"#2, 3#"');    

    // Subtraction - Vector2D
    assert('new UDB.Vector2D(2, 3) - new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(0, 1)');
    assert('[ 2, 3 ] - new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(0, 1)');
    assert('{ x: 2, y: 3 } - new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(0, 1)');
    assert('new UDB.Vector2D(2, 3) - [ 2, 2 ]', 'new UDB.Vector2D(0, 1)');
    assert('new UDB.Vector2D(2, 3) - { x: 2, y: 2 }', 'new UDB.Vector2D(0, 1)');
    assert('new UDB.Vector2D(2, 3) - 2', 'new UDB.Vector2D(0, 1)');
    assert('2 - new UDB.Vector2D(2, 3)', 'new UDB.Vector2D(0, -1)');
    assert('-(new UDB.Vector2D(1, 2))', 'new UDB.Vector2D(-1, -2)');

    // Multiplication - Vector2D
    assert('new UDB.Vector2D(2, 3) * new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(4, 6)');
    assert('[ 2, 3 ] * new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(4, 6)');
    assert('{ x: 2, y: 3 } * new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(4, 6)');
    assert('new UDB.Vector2D(2, 2) * [ 2, 3 ]', 'new UDB.Vector2D(4, 6)');
    assert('new UDB.Vector2D(2, 2) * { x: 2, y: 3 }', 'new UDB.Vector2D(4, 6)');
    assert('new UDB.Vector2D(2, 3) * 2', 'new UDB.Vector2D(4, 6)');
    assert('2 * new UDB.Vector2D(2, 3)', 'new UDB.Vector2D(4, 6)');

    // Division - Vector2D
    assert('new UDB.Vector2D(4, 6) / new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(2, 3)');
    assert('[ 4, 6 ] / new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(2, 3)');
    assert('{ x: 4, y: 6 } / new UDB.Vector2D(2, 2)', 'new UDB.Vector2D(2, 3)');
    assert('new UDB.Vector2D(4, 6) / [ 2, 2 ]', 'new UDB.Vector2D(2, 3)');
    assert('new UDB.Vector2D(4, 6) / { x: 2, y: 2 }', 'new UDB.Vector2D(2, 3)');
    assert('new UDB.Vector2D(2, 8) / 4', 'new UDB.Vector2D(0.5, 2)');
    assert('4 / new UDB.Vector2D(2, 8)', 'new UDB.Vector2D(0.5, 2)');

    // Equality - Vector2D
    assert('new UDB.Vector2D(1, 2)', 'new UDB.Vector2D(1, 2)');
    assert('new UDB.Vector2D(1, 2)', '[ 1, 2 ]');
    assert('new UDB.Vector2D(1, 2)', '{ x: 1, y: 2 }');
    assert('[ 1, 2 ]', 'new UDB.Vector2D(1, 2)');
    assert('{ x: 1, y: 2 }', 'new UDB.Vector2D(1, 2)');
    assert('[ 2, 1 ]', 'new UDB.Vector2D(1, 2)', false);
    assert('{ x: 2, y: 1 }', 'new UDB.Vector2D(1, 2)', false);
    assert('new UDB.Vector2D(2, 1)', '[ 1, 2 ]', false);
    assert('new UDB.Vector2D(2, 1)', '{ x: 1, y: 2 }', false);

    // Addition - Vector3D
    assert('new UDB.Vector3D(2, 3, 4) + new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 5, 6)');
    assert('[ 2, 3, 4 ] + new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 5, 6)');
    assert('{ x: 2, y: 3, z: 4 } + new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 5, 6)');
    assert('new UDB.Vector3D(2, 3, 4) + [ 2, 2, 2 ]', 'new UDB.Vector3D(4, 5, 6)');
    assert('new UDB.Vector3D(2, 3, 4) + { x: 2,  y: 2, z: 2 }', 'new UDB.Vector3D(4, 5, 6)');
    assert('new UDB.Vector3D(2, 3, 4) + 2', 'new UDB.Vector3D(4, 5, 6)');
    assert('2 + new UDB.Vector3D(2, 3, 4)', 'new UDB.Vector3D(4, 5, 6)');
    assert('new UDB.Vector2D(2, 3) + new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 5, 2)');
    assert('new UDB.Vector3D(2, 3, 4) + new UDB.Vector2D(2, 2)', 'new UDB.Vector3D(4, 5, 4)');
    assert('[ 2, 3 ] + new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 5, 2)');
    assert('new UDB.Vector3D(2, 3, 4) + [ 2, 2 ]', 'new UDB.Vector3D(4, 5, 4)');
    assert('{ x: 2, y: 3 } + new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 5, 2)');
    assert('new UDB.Vector3D(2, 3, 4) + { x: 2,  y: 2 }', 'new UDB.Vector3D(4, 5, 4)');
    assert('new UDB.Vector3D(2, 3, 4) + "#"', '"2, 3, 4#"');
    assert('"#" + new UDB.Vector3D(2, 3, 4)', '"#2, 3, 4"');
    assert('"#" + new UDB.Vector3D(2, 3, 4) + "#"', '"#2, 3, 4#"');

    // Subtraction - Vector3D
    assert('new UDB.Vector3D(2, 3, 4) - new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(0, 1, 2)');
    assert('[ 2, 3, 4 ] - new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(0, 1, 2)');
    assert('{ x: 2, y: 3, z: 4 } - new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(0, 1, 2)');
    assert('new UDB.Vector3D(2, 3, 4) - [ 2, 2, 2 ]', 'new UDB.Vector3D(0, 1, 2)');
    assert('new UDB.Vector3D(2, 3, 4) - { x: 2,  y: 2, z: 2 }', 'new UDB.Vector3D(0, 1, 2)');
    assert('new UDB.Vector3D(2, 3, 4) - 2', 'new UDB.Vector3D(0, 1, 2)');
    assert('2 - new UDB.Vector3D(2, 3, 4)', 'new UDB.Vector3D(0, 1, 2)');
    assert('new UDB.Vector2D(2, 3) - new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(0, 1, -2)');
    assert('new UDB.Vector3D(2, 3, 4) - new UDB.Vector2D(2, 2)', 'new UDB.Vector3D(0, 1, 4)');
    assert('[ 2, 3 ] - new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(0, 1, -2)');
    assert('new UDB.Vector3D(2, 3, 4) - [ 2, 2 ]', 'new UDB.Vector3D(0, 1, 4)');
    assert('{ x: 2, y: 3 } - new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(0, 1, -2)');
    assert('new UDB.Vector3D(2, 3, 4) - { x: 2,  y: 2 }', 'new UDB.Vector3D(0, 1, 4)');

    // Multiplication - Vector3D
    assert('new UDB.Vector3D(2, 3, 4) * new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 6, 8)');
    assert('[ 2, 3, 4 ] * new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 6, 8)');
    assert('{ x: 2, y: 3, z: 4 } * new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 6, 8)');
    assert('new UDB.Vector3D(2, 3, 4) * [ 2, 2, 2 ]', 'new UDB.Vector3D(4, 6, 8)');
    assert('new UDB.Vector3D(2, 3, 4) * { x: 2,  y: 2, z: 2 }', 'new UDB.Vector3D(4, 6, 8)');
    assert('new UDB.Vector3D(2, 3, 4) * 2', 'new UDB.Vector3D(4, 6, 8)');
    assert('2 * new UDB.Vector3D(2, 3, 4)', 'new UDB.Vector3D(4, 6, 8)');
    assert('new UDB.Vector2D(2, 3) * new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 6, 0)');
    assert('new UDB.Vector3D(2, 3, 4) * new UDB.Vector2D(2, 2)', 'new UDB.Vector3D(4, 6, 0)');
    assert('[ 2, 3 ] * new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 6, 0)');
    assert('new UDB.Vector3D(2, 3, 4) * [ 2, 2 ]', 'new UDB.Vector3D(4, 6, 0)');
    assert('{ x: 2, y: 3 } * new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(4, 6, 0)');
    assert('new UDB.Vector3D(2, 3, 4) * { x: 2,  y: 2 }', 'new UDB.Vector3D(4, 6, 0)');

    // Division - Vector3D
    assert('new UDB.Vector3D(2, 3, 4) / new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(1, 1.5, 2)');
    assert('[ 2, 3, 4 ] / new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(1, 1.5, 2)');
    assert('{ x: 2, y: 3, z: 4 } / new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(1, 1.5, 2)');
    assert('new UDB.Vector3D(2, 3, 4) / [ 2, 2, 2 ]', 'new UDB.Vector3D(1, 1.5, 2)');
    assert('new UDB.Vector3D(2, 3, 4) / { x: 2,  y: 2, z: 2 }', 'new UDB.Vector3D(1, 1.5, 2)');
    assert('new UDB.Vector3D(2, 3, 4) / 2', 'new UDB.Vector3D(1, 1.5, 2)');
    assert('2 / new UDB.Vector3D(2, 3, 4)', 'new UDB.Vector3D(1, 1.5, 2)');
    assert('new UDB.Vector2D(2, 3) / new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(1, 1.5, 0)');
    assert('new UDB.Vector3D(2, 3, 4) / new UDB.Vector2D(2, 2)', 'new UDB.Vector3D(1, 1.5, Infinity)');
    assert('[ 2, 3 ] / new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(1, 1.5, 0)');
    assert('new UDB.Vector3D(2, 3, 4) / [ 2, 2 ]', 'new UDB.Vector3D(1, 1.5, Infinity)');
    assert('{ x: 2, y: 3 } / new UDB.Vector3D(2, 2, 2)', 'new UDB.Vector3D(1, 1.5, 0)');
    assert('new UDB.Vector3D(2, 3, 4) / { x: 2,  y: 2 }', 'new UDB.Vector3D(1, 1.5, Infinity)');

    // Equality - Vector3D
    assert('new UDB.Vector3D(1, 2, 3)', 'new UDB.Vector3D(1, 2, 3)');
    assert('new UDB.Vector3D(1, 2, 3)', '[ 1, 2, 3 ]');
    assert('new UDB.Vector3D(1, 2, 3)', '{ x: 1, y: 2, z: 3 }');
    assert('[ 1, 2, 3 ]', 'new UDB.Vector3D(1, 2, 3)');
    assert('{ x: 1, y: 2, z: 3 }', 'new UDB.Vector3D(1, 2, 3)');
    assert('[ 3, 2, 1 ]', 'new UDB.Vector3D(1, 2, 3)', false);
    assert('{ x: 3, y: 2, z: 1 }', 'new UDB.Vector3D(1, 2, 3)', false);
    assert('new UDB.Vector3D(3, 2, 1)', '[ 1, 2, 3 ]', false);
    assert('new UDB.Vector3D(3, 2, 1)', '{ x: 1, y: 2, z: 3 }', false);

    let line1 = new UDB.Line2D([ 32, 32 ], [ 32, -32 ]);
    let line2 = new UDB.Line2D([ 0, 16 ], [ 64, 16 ]);
    let intersecting  = UDB.Line2D.areIntersecting(line1, line2);
    assert('UDB.Line2D.areIntersecting(new UDB.Line2D([ 32, 32 ], [ 32, -32 ]), new UDB.Line2D([ 0, 16 ], [ 64, 16 ]))', 'true');

    UDB.showMessage(assertion_log);
}
catch(e)
{
    UDB.showMessage(e);
    UDB.exit();
}