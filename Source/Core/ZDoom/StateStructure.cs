
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System.Collections.Generic;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public class StateStructure
	{
		#region ================== FrameInfo (mxd)

		public class FrameInfo
		{
			public string Sprite;
			public string LightName;
			public bool Bright;
            public int Duration; // this is used for TrimLeft

            public bool IsEmpty()
            {
                return (Sprite.StartsWith("TNT1") || Duration == 0);
            }
		}
		
		#endregion

		#region ================== Variables
		
		// All we care about is the first sprite in the sequence
		internal List<FrameInfo> sprites;
		internal StateGoto gotostate;
		
		#endregion

		#region ================== Properties

		public int SpritesCount { get { return sprites.Count; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal StateStructure()
		{
			this.gotostate = null;
			this.sprites = new List<FrameInfo>();
		}

		//mxd
		internal StateStructure(string spritename) 
		{
			this.gotostate = null;
			this.sprites = new List<FrameInfo> { new FrameInfo { Sprite = spritename } };
		}

		#endregion

		#region ================== Methods

        // This removes useless images from the start of the state (TNT1)
        protected void TrimLeft() // :)
        {
            // postprocess: if there are only TNT1 sprites, do nothing. however, if there were normal sprites as well, use these.
            int firstNonEmpty = -1;
            for (int i = 0; i < sprites.Count; i++)
            {
                if (!sprites[i].IsEmpty())
                {
                    firstNonEmpty = i;
                    break;
                }
            }

            if (firstNonEmpty > 0)
                sprites.RemoveRange(0, firstNonEmpty);
        }
		
		// This finds the first valid sprite and returns it
		public FrameInfo GetSprite(int index)
		{
			return GetSprite(index, new HashSet<StateStructure>());
		}
		
		// This version of GetSprite uses a callstack to check if it isn't going into an endless loop
		private FrameInfo GetSprite(int index, HashSet<StateStructure> prevstates)
		{
			// If we have sprite of our own, see if we can return this index
			if(index < sprites.Count) return sprites[index];
			
			// Otherwise, continue searching where goto tells us to go
			if(gotostate != null)
			{
				// Find the class
				ActorStructure a = General.Map.Data.GetZDoomActor(gotostate.ClassName);
				if(a != null)
				{
					StateStructure s = a.GetState(gotostate.StateName);
					if((s != null) && !prevstates.Contains(s))
					{
						prevstates.Add(this);
						return s.GetSprite(gotostate.SpriteOffset, prevstates);
					}
				}
			}
			
			// If there is no goto keyword used, just give us one of our sprites if we can
			if(sprites.Count > 0)
			{
				// The following behavior should really depend on the flow control keyword (loop or stop) but who cares.
				return sprites[0];
			}
			
			return new FrameInfo();
		}
		
		#endregion
	}
}
