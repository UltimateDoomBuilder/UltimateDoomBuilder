/// <reference path="../../../udbscript.d.ts" />

`#version 4`;

`#name Make Door`;

`#description Creates a door from a selected sector. Functionality is the same as the built-in "make door" action.`;

`#scriptoptions

doortexture {
    description = "Door texture";
    default = "BIGDOOR2";
    type = 6;
}

doortrack {
    description = "Door track texture";
    default = "DOORTRAK";
    type = 6;
}

ceilingtexture {
    description = "Door ceiling texture";
    default = "FLAT20";
    type = 7;
}

`;

let sectors = UDB.Map.getSelectedOrHighlightedSectors();

if(sectors.length == 0)
    UDB.die('You need to select at least one sector!');

sectors.forEach(s => {
    s.ceilingHeight = s.floorHeight;
    s.ceilingTexture = UDB.ScriptOptions.ceilingtexture;

    s.getSidedefs().forEach(sd => {
        if(sd.other == null) // 1-sided lines
        {
            sd.middleTexture = UDB.ScriptOptions.doortrack;

            if(UDB.Map.isUDMF)
                sd.line.flags.dontpegbottom = true;
            else
                sd.line.flags['16'] = true;
        }
        else // 2-sided lines
        {
            // If the sidedef is on the front of the linedef we need to flip the linedef
            // so that the line can be activated properly
            if(sd.isFront)
                sd.line.flip();

            sd.other.upperTexture = UDB.ScriptOptions.doortexture;

            // Set the action
            if(UDB.Map.isDoom)
                sd.line.action = 1;
            else
            {
                sd.line.action = 12;
                sd.line.args[0] = 0; // Tag
                sd.line.args[1] = 16; // Speed
                sd.line.args[2] = 150; // Close delay
                sd.line.args[3] = 0; // Light tag
                
                if(UDB.Map.isHexen)
                {
                    sd.line.activate = 1024; // Player presses use
                    sd.line.flags['512'] = true; // Can be used repeatedly
                    sd.line.flags['8192'] = true; // Can be activated by monsters
                }
                else // UDMF
                {
                    sd.line.flags.repeatspecial = true;
                    sd.line.flags.playeruse = true;
                    sd.line.flags.monsteruse = true;
                }
            }
        }
    });
});