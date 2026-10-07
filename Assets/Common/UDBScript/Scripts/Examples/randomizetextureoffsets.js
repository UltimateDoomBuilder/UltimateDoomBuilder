/// <reference path="../../udbscript.d.ts" />

`#version 4`;

`#name Randomize Texture Offsets`;

`#description Randomized texture offsets. Distinct upper, middle, and lower offsets only work if the game configuration supports those local offsets.`;

`#scriptoptions

global_x
{
    description = "Global X Offset";
    default = "False";
    type = 3; // Boolean
}

global_y
{
    description = "Global Y Offset";
    default = "False";
    type = 3; // Boolean
}

upper_x
{
	description = "Upper X Offset";
	default = "True";
	type = 3; // Boolean
}

upper_y
{
	description = "Upper Y Offset";
	default = "True";
	type = 3; // Boolean
}

middle_x
{
	description = "Middle X Offset";
	default = "True";
	type = 3; // Boolean
}

middle_y
{
	description = "Middle Y Offset";
	default = "True";
	type = 3; // Boolean
}

lower_x
{
	description = "Lower X Offset";
	default = "True";
	type = 3; // Boolean
}

lower_y
{
	description = "Lower Y Offset";
	default = "True";
	type = 3; // Boolean
}
`;

// Gets a random number
function getRandomOffset(max)
{
    return Math.floor(Math.random() * max);
}

// Checks if the given name is a proper texture name and if the texture exists
function isValidTexture(texture)
{
    return texture != '-' && UDB.Data.textureExists(texture);
}

function randomizeSidedefOffsets(sd)
{
    // Global X texture offset
    if(UDB.ScriptOptions.global_x && (isValidTexture(sd.upperTexture) || isValidTexture(sd.middleTexture) || isValidTexture(sd.lowerTexture)))
    {
        let widths = [];

        if(isValidTexture(sd.upperTexture))
            widths.push(UDB.Data.getTextureInfo(sd.upperTexture).width);

        if(isValidTexture(sd.middleTexture))
            widths.push(UDB.Data.getTextureInfo(sd.middleTexture).width);

        if(isValidTexture(sd.lowerTexture))
            widths.push(UDB.Data.getTextureInfo(sd.lowerTexture).width);

        if(widths.length > 0)
            sd.offsetX = getRandomOffset(Math.max(widths));
    }

    // Global Y texture offset
    if(UDB.ScriptOptions.global_y && (isValidTexture(sd.upperTexture) || isValidTexture(sd.middleTexture) || isValidTexture(sd.lowerTexture)))
    {
        let heights = [];

        if(isValidTexture(sd.upperTexture))
            heights.push(UDB.Data.getTextureInfo(sd.upperTexture).height);

        if(isValidTexture(sd.middleTexture))
            heights.push(UDB.Data.getTextureInfo(sd.middleTexture).height);

        if(isValidTexture(sd.lowerTexture))
            heights.push(UDB.Data.getTextureInfo(sd.lowerTexture).height);

        if(heights.length > 0)
            sd.offsetY = getRandomOffset(Math.max(heights));
    }    

    // Local X texture offsets
    if(UDB.GameConfiguration.hasLocalSidedefTextureOffsets)
    {
        if(UDB.ScriptOptions.upper_x && isValidTexture(sd.upperTexture))
            sd.fields.offsetx_top = getRandomOffset(UDB.Data.getTextureInfo(sd.upperTexture).height);

        if(UDB.ScriptOptions.middle_x && isValidTexture(sd.middleTexture))
            sd.fields.offsetx_mid = getRandomOffset(UDB.Data.getTextureInfo(sd.middleTexture).height);

        if(UDB.ScriptOptions.lower_x && isValidTexture(sd.lowerTexture))
            sd.fields.offsetx_bottom = getRandomOffset(UDB.Data.getTextureInfo(sd.lowerTexture).height);
    }

    // Local Y texture offsets
    if(UDB.GameConfiguration.hasLocalSidedefTextureOffsets)
    {
        if(UDB.ScriptOptions.upper_y && isValidTexture(sd.upperTexture))
            sd.fields.offsety_top = getRandomOffset(UDB.Data.getTextureInfo(sd.upperTexture).height);

        if(UDB.ScriptOptions.middle_y && isValidTexture(sd.middleTexture))
            sd.fields.offsety_mid = getRandomOffset(UDB.Data.getTextureInfo(sd.middleTexture).height);

        if(UDB.ScriptOptions.lower_y && isValidTexture(sd.lowerTexture))
            sd.fields.offsety_bottom = getRandomOffset(UDB.Data.getTextureInfo(sd.lowerTexture).height);
    }    
}

// Randomize offset of front and back sidedefs of all selected linedefs
UDB.Map.getSelectedLinedefs().forEach(ld => {
    if(ld.front != null)
        randomizeSidedefOffsets(ld.front);
    
    if(ld.back != null)
        randomizeSidedefOffsets(ld.back);
});