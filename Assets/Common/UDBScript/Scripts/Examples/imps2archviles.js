/// <reference path="../../udbscript.d.ts" />

`#version 4`;

`#name Imps to Arch-Viles`;

`#description Turns all Imps that appear on UV into Arch-Viles. To make it a bit more fair for the player it also adds a health potion to the monster's position.`;

UDB.Map.getThings().filter(t => t.type == 3001 && ((UDB.Map.isUDMF && t.flags.skill5) || (!UDB.Map.isUDMF && t.flags['4']))).forEach(t => {
    let addav = false;
    let skillsum = 0;

    // Count the number of skills the Imp appears on
    if(UDB.Map.isUDMF)
    {
        for(let i=1; i <= 5; i++)
            if(t.flags['skill' + i])
                skillsum++;
    }
    else
    {
        for(let i=0; i < 3; i++)
            if(t.flags[parseInt(1 << i)])
                skillsum++;
    }

    // If the Imp appears on more than one skill we need to add the Arch-Vile, otherwise we can
    // simply set the existing thing's type to that of the Arch-Vile
    if(skillsum != 1)
        addav = true;
    
    // Create a new health potion at the thing's position
    let hp = UDB.Map.createThing(t.position, 2014);

    if(UDB.Map.isUDMF)
    {
        hp.flags.skill5 = true;
        for(let i=1; i <= 4; i++)
            hp.flags['skill' + i] = false;
    }
    else
    {
        hp.flags['4'] = true;
        hp.flags['1'] = hp.flags['2'] = false;
    }

    if(addav)
    {
        // Create a new Arch-Vile and copy all the original thing's properties
        let av = UDB.Map.createThing(t.position);
        t.copyPropertiesTo(av);
		av.type = 64;

        // Set the skill flags. Since we're addin an Arch-Vile we have to unset the Imp's UV flag,
        // and remove the Arch-Viles (and health potion's) other skill flags
        if(UDB.Map.isUDMF)
        {
            t.flags.skill5 = false;
            for(let i=1; i <= 4; i++)
                av.flags['skill' + i] = false;
        }
        else
        {
            t.flags['4'] = false;
            av.flags['1'] = av.flags['2'] = false;
        }
    }
    else
    {
        // Modify the existing thing to be the Arch-Vile
        t.type = 64;
    }
});
