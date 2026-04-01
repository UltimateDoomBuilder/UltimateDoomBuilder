namespace CodeImp.DoomBuilder.ZDoom
{
    internal sealed class DecorateStateGoto : StateGoto
    {
        #region ================== DECORATE State Goto parsing

        internal DecorateStateGoto(ActorStructure actor, ZDTextParser parser)
        {
            string firsttarget = "";
            string secondtarget = "";
            bool commentreached = false;
            bool offsetreached = false;
            string offsetstr = "";
            int cindex = 0;

            // This is a bitch to parse because for some bizarre reason someone thought it
            // was funny to allow quotes here. Read the whole line and start parsing this manually.
            string line = parser.ReadLine();

            // Skip whitespace
            while ((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
                cindex++;

            // Parse first target
            while ((cindex < line.Length) && (line[cindex] != ':'))
            {
                // When a comment is reached, we're done here
                if (line[cindex] == '/')
                {
                    if ((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
                    {
                        commentreached = true;
                        break;
                    }
                }

                // Whitespace ends the string
                if ((line[cindex] == ' ') || (line[cindex] == '\t'))
                    break;

                // + sign indicates offset start
                if (line[cindex] == '+')
                {
                    cindex++;
                    offsetreached = true;
                    break;
                }

                // Ignore quotes
                if (line[cindex] != '"')
                    firsttarget += line[cindex];

                cindex++;
            }

            if (!commentreached && !offsetreached)
            {
                // Skip whitespace
                while ((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
                    cindex++;

                // Parse second target
                while (cindex < line.Length)
                {
                    // When a comment is reached, we're done here
                    if (line[cindex] == '/')
                    {
                        if ((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
                        {
                            commentreached = true;
                            break;
                        }
                    }

                    // Whitespace ends the string
                    if ((line[cindex] == ' ') || (line[cindex] == '\t'))
                        break;

                    // + sign indicates offset start
                    if (line[cindex] == '+')
                    {
                        cindex++;
                        offsetreached = true;
                        break;
                    }

                    // Ignore quotes and semicolons
                    if ((line[cindex] != '"') && (line[cindex] != ':'))
                        secondtarget += line[cindex];

                    cindex++;
                }
            }

            // Try to find the offset if we still haven't found it yet
            if (!offsetreached)
            {
                // Skip whitespace
                while ((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
                    cindex++;

                if ((cindex < line.Length) && (line[cindex] == '+'))
                {
                    cindex++;
                    offsetreached = true;
                }
            }

            if (offsetreached)
            {
                // Parse offset
                while (cindex < line.Length)
                {
                    // When a comment is reached, we're done here
                    if (line[cindex] == '/')
                    {
                        if ((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
                        {
                            commentreached = true;
                            break;
                        }
                    }

                    // Whitespace ends the string
                    if ((line[cindex] == ' ') || (line[cindex] == '\t'))
                        break;

                    // Ignore quotes and semicolons
                    if ((line[cindex] != '"') && (line[cindex] != ':'))
                        offsetstr += line[cindex];

                    cindex++;
                }
            }

            // We should now have a first target, optionally a second target and optionally a sprite offset

            // Check if we don't have the class specified
            if (string.IsNullOrEmpty(secondtarget))
            {
                // First target is the state to go to
                classname = actor.ClassName;
                statename = firsttarget.ToLowerInvariant().Trim();
            }
            else
            {
                // First target is the base class to use
                // Second target is the state to go to
                classname = firsttarget.ToLowerInvariant().Trim();
                statename = secondtarget.ToLowerInvariant().Trim();
            }

            if (offsetstr.Length > 0)
                int.TryParse(offsetstr, out spriteoffset);

            if ((classname == "super") && (actor.BaseClass != null))
                classname = actor.BaseClass.ClassName;
        }

        #endregion
    }
}
