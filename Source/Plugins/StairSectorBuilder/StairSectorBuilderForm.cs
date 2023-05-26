using System;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.StairSectorBuilderMode
{
	public partial class StairSectorBuilderForm : DelayedForm
	{
		private bool fullyloaded;
		private bool loadingprefab;
		private int stepmultiplier = 1;
		private int originalfloorbase;
		private int originalceilingbase;


		#region ================== Properties

		public int OriginalFloorBase
		{
			set { originalfloorbase = value; }
			get { return originalfloorbase; }
		}

		public int OriginalCeilingBase
		{
			set { originalceilingbase = value; }
			get { return originalceilingbase; }
		}

		public int StepMultiplier
		{
			set { stepmultiplier = value; }
		}

		public int StairType
		{
			get { return tabcontrol.SelectedIndex; }
		}

		public uint NumberOfSectors
		{
			// get { return (uint)System.Convert.ToUInt32(numberofsectors.Text); }
			get { return (uint)numberofsectors.GetResult(1); }
			set { numberofsectors.Text = value.ToString(); }
		}

		public uint SectorDepth
		{
			// get { return (uint)System.Convert.ToUInt32(sectordepth.Text); }
			get { return (uint)sectordepth.GetResult(32); }
			set { sectordepth.Text = value.ToString(); }
		}

        public int Spacing
        {
            get { return spacing.GetResult(0); }
            set { spacing.Text = value.ToString(); }
        }

		public int InnerVertexMultiplier
		{
			get { return autocurveinnervertexmultiplier.GetResult(1); }
			set { autocurveinnervertexmultiplier.Text = value.ToString(); }
		}

		public int OuterVertexMultiplier
		{
			get { return autocurveoutervertexmultiplier.GetResult(1); }
			set { autocurveoutervertexmultiplier.Text = value.ToString(); }
		}

		public bool SideFront
		{
			get { return sidefront.Checked; }
            set { sidefront.Checked = value; }
		}

		public bool SideBack
		{
			get { return sideback.Checked; }
			set { sideback.Checked = value; }
		}

		public CheckBox DistinctBaseHeights
		{
			get { return distinctbaseheights; }
			set { distinctbaseheights = value; }
		}

		public CheckBox SingleSteps
		{
			get { return singlesteps; }
			set { singlesteps = value; }
		}

		public CheckBox DistinctSectors
		{
			get { return distinctsectors; }
			set { distinctsectors = value; }
		}

		public CheckBox SingleDirection
		{
			get { return singledirection; }
			set { singledirection = value; }
		}

		public TabControl Tabs
		{
			get { return tabcontrol; }
		}

		public int NumControlPoints
		{
			get { return numberofcontrolpoints.GetResult(1) + 2; }
            set { numberofcontrolpoints.Text = value.ToString(); }
		}

        public bool FloorHeight
        {
            get { return floorheightmodification.Checked; }
            set { floorheightmodification.Checked = value; }
        }

		public bool CeilingHeight
		{
			get { return ceilingheightmodification.Checked; }
            set { ceilingheightmodification.Checked = value; }
		}


        public int FloorHeightModification
        {
            get { return floorheightmod.GetResult(0); }
            set { floorheightmod.Text = value.ToString(); }
        }

        public int CeilingHeightModification
        {
            get { return ceilingheightmod.GetResult(0); }
            set { ceilingheightmod.Text = value.ToString(); }
        }

        public bool FloorFlat
        {
            get { return floorflat.Checked; }
            set { floorflat.Checked = value; }
        }

        public string FloorFlatTexture
        {
            get { return floorflattexture.TextureName; }
            set { floorflattexture.TextureName = value; }
        }

        public bool CeilingFlat
        {
            get { return ceilingflat.Checked; }
            set { ceilingflat.Checked = value; }
        }

        public string CeilingFlatTexture
        {
            get { return ceilingflattexture.TextureName; }
            set { ceilingflattexture.TextureName = value; }
        }

        public bool UpperTexture
        {
            get { return uppertexture.Checked; }
            set { uppertexture.Checked = value; }
        }

		public bool MiddleTexture
		{
			get { return middletexture.Checked; }
			set { middletexture.Checked = value; }
		}

        public bool LowerTexture
        {
            get { return lowertexture.Checked; }
            set { lowertexture.Checked = value; }
        }

        public string UpperTextureTexture
        {
            get { return uppertexturetexture.TextureName; }
            set { uppertexturetexture.TextureName = value; }
        }

		public string MiddleTextureTexture
		{
			get { return middletexturetexture.TextureName; }
			set { middletexturetexture.TextureName = value; }
		}

        public string LowerTextureTexture
        {
            get { return lowertexturetexture.TextureName; }
            set { lowertexturetexture.TextureName = value; }
        }

		public int FloorBase
		{
			set { floorbase.Text = value.ToString(); }
			get { return floorbase.GetResult(0); }
		}

		public int CeilingBase
		{
			set { ceilingbase.Text = value.ToString(); }
			get { return ceilingbase.GetResult(0); }
		}

		public bool FullyLoaded
		{
			get { return fullyloaded; }
		}

		public int Flipping
		{
			get { return autocurveflipping.SelectedIndex; }
			set { autocurveflipping.SelectedIndex = value; }
		}

		public bool UpperUnpegged
		{
			get { return upperunpegged.Checked; }
			set { upperunpegged.Checked = value; }
		}

		public bool LowerUnpegged
		{
			get { return lowerunpegged.Checked; }
			set { lowerunpegged.Checked = value; }
		}

		#endregion

		public StairSectorBuilderForm()
		{
			InitializeComponent();

            foreach(BuilderPlug.Prefab p in BuilderPlug.Me.Prefabs)
            {
                ListViewItem lvi = new ListViewItem();
                ListViewItem.ListViewSubItem lvisi = new ListViewItem.ListViewSubItem();

                lvi.Text = p.name;
                lvisi.Text = tabcontrol.TabPages[p.stairtype].Text;

                lvi.SubItems.Add(lvisi);
                prefabs.Items.Add(lvi);
            }
		}

        // This shows the window
        public void Show(Form owner)
        {
            // Position at left-top of owner
            this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90);

			// Set the default name for the prefab
			for(int i = 1; i < int.MaxValue; i++)
			{
				string defname = "Prefab #" + i;
				bool validname = true;

				foreach(BuilderPlug.Prefab p in BuilderPlug.Me.Prefabs)
				{
					if(p.name == defname)
					{
						validname = false;
						break;
					}
				}

				if(validname)
				{
					prefabname.Text = defname;
					break;
				}
			}

			// Show window
			base.Show(owner);
        }

		private void ComputeHeights()
		{
			if(!fullyloaded) return;

			if(floorbase.Enabled == false)
			{
				floorfirst.Text = "--";
				floorlast.Text = "--";
			}
			else
			{
				floorfirst.Text = (Int32.Parse(floorbase.Text) + Int32.Parse(floorheightmod.Text)).ToString();
				floorlast.Text = (Int32.Parse(floorbase.Text) + Int32.Parse(floorheightmod.Text) * (Int32.Parse(numberofsectors.Text) * stepmultiplier)).ToString();
			}

			if(ceilingbase.Enabled == false)
			{
				ceilingfirst.Text = "--";
				ceilinglast.Text = "--";
			}
			else
			{
				ceilingfirst.Text = (Int32.Parse(ceilingbase.Text) + Int32.Parse(ceilingheightmod.Text)).ToString();
				ceilinglast.Text = (Int32.Parse(ceilingbase.Text) + Int32.Parse(ceilingheightmod.Text) * (Int32.Parse(numberofsectors.Text) * stepmultiplier)).ToString();
			}
		}

		// Wrap redrawing display so that it will not get called multiple
		// times while loading a prefab
		private void DoRedrawDisplay()
		{
			if(loadingprefab == false) General.Interface.RedrawDisplay();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			int position = 0;

			// We always want to have the "[Default]" prefab on top, so look for it and add "[Previous]" after that
			for (int i = 0; i < BuilderPlug.Me.Prefabs.Count; i++)
			{
				BuilderPlug.Prefab p = BuilderPlug.Me.Prefabs[i];

				if (p.name == "[Default]")
				{
					position = i+1;
					break;
				}
			}

			SavePrefab("[Previous]", true, position);

			General.Editing.AcceptMode();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			General.Editing.CancelMode();
		}

		private void tbSectorDepth_TextChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void rdbFront_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void rdbBack_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void tabcontrol_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(General.Map.Map.GetSelectedLinedefs(true).Count == 1)
			{
				tabcontrol.SelectedTab = tabPage1;
			}

			if(tabcontrol.SelectedTab != tabPage1)
			{
				floorbase.Enabled = true;
				ceilingbase.Enabled = true;
			}
			else
			{
				if(distinctbaseheights.Checked)
				{
					floorbase.Enabled = false;
					ceilingbase.Enabled = false;
				}
				else
				{
					floorbase.Enabled = true;
					ceilingbase.Enabled = true;
				}
			}

			DoRedrawDisplay();
			ComputeHeights();
		}

		/*private void tbAutoNumSectors_TextChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}*/

		private void numberofsectors_WhenTextChanged(object sender, EventArgs e)
		{
			if(numberofsectors.Text == "" || numberofsectors.GetResult(1) == 0) numberofsectors.Text = "1";
			ComputeHeights();
			DoRedrawDisplay();
		}

		private void sectordepth_WhenTextChanged(object sender, EventArgs e)
		{
			if(sectordepth.GetResult(32) == 0) sectordepth.Text = "1";
			DoRedrawDisplay();
		}

		private void StairSectorBuilderForm_Load(object sender, EventArgs e)
		{
			sectordepth.Text = "32";
            spacing.Text = "0";
			numberofsectors.Text = "1";
			autocurveinnervertexmultiplier.Text = "1";
			autocurveoutervertexmultiplier.Text = "1";
			splineinnervertexmultiplier.Text = "1";
			splineoutervertexmultiplier.Text = "1";
			numberofcontrolpoints.Text = "1";
            floorheightmod.Text = "0";
            ceilingheightmod.Text = "0";
			floorbase.Text = "0";
			ceilingbase.Text = "0";
			autocurveflipping.SelectedIndex = 0;
			MiddleTextureTexture = "-";

			// Check if there's a "[Default]" prefab and load it if so
			foreach (BuilderPlug.Prefab p in BuilderPlug.Me.Prefabs)
			{
				if (p.name == "[Default]")
				{
					LoadPrefab(p);
					break;
				}
			}

			fullyloaded = true;

			ComputeHeights();
		}

		private void autocurveinnervertexmultiplier_WhenTextChanged(object sender, EventArgs e)
		{
			if(autocurveinnervertexmultiplier.GetResult(1) == 0) autocurveinnervertexmultiplier.Text = "1";

			if(splineinnervertexmultiplier.Text != autocurveinnervertexmultiplier.Text)
				splineinnervertexmultiplier.Text = autocurveinnervertexmultiplier.Text;

			DoRedrawDisplay();
		}

		private void autocurveoutervertexmultiplier_WhenTextChanged(object sender, EventArgs e)
		{
			if(autocurveoutervertexmultiplier.GetResult(1) == 0) autocurveoutervertexmultiplier.Text = "1";

			if(splineoutervertexmultiplier.Text != autocurveoutervertexmultiplier.Text)
				splineoutervertexmultiplier.Text = autocurveoutervertexmultiplier.Text;

			DoRedrawDisplay();

		}

		private void splineinnervertexmultiplier_WhenTextChanged(object sender, EventArgs e)
		{
			if(splineinnervertexmultiplier.GetResult(1) == 0) splineinnervertexmultiplier.Text = "1";

			if(autocurveinnervertexmultiplier.Text != splineinnervertexmultiplier.Text)
				autocurveinnervertexmultiplier.Text = splineinnervertexmultiplier.Text;

			DoRedrawDisplay();
		}

		private void splineoutervertexmultiplier_WhenTextChanged(object sender, EventArgs e)
		{
			if(splineoutervertexmultiplier.GetResult(1) == 0) splineoutervertexmultiplier.Text = "1";

			if(splineoutervertexmultiplier.Text != autocurveoutervertexmultiplier.Text)
				autocurveoutervertexmultiplier.Text = splineoutervertexmultiplier.Text;

			DoRedrawDisplay();
		}

		private void onecontrolpoint_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void StairSectorBuilderForm_Shown(object sender, EventArgs e)
		{
			if(General.Map.Map.GetSelectedLinedefs(true).Count == 1 || General.Map.Map.SelectedSectorsCount > 0)
			{
				tabcontrol.TabPages.Remove(tabPage2);
				tabcontrol.TabPages.Remove(tabPage3);
			}

			if(General.Map.Map.SelectedSectorsCount > 0)
			{
				singledirection.Checked = false;
				singledirection.Enabled = false;

				singlesteps.Checked = true;
				singlesteps.Enabled = false;
			}
		}

		private void singleseteps_CheckedChanged(object sender, EventArgs e)
		{
			if (singlesteps.Checked)
			{
				singledirection.Enabled = true;
				distinctsectors.Enabled = true;
			}
			else
			{
				singledirection.Enabled = false;
				distinctsectors.Enabled = false;
			}

			DoRedrawDisplay();
		}

		private void numberofcontrolpoints_WhenTextChanged(object sender, EventArgs e)
		{
			if(numberofcontrolpoints.GetResult(1) == 0) numberofcontrolpoints.Text = "1";
			DoRedrawDisplay();
		}

		private void sidefront_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void sideback_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void singledirection_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

        private void floorheightmod_WhenTextChanged(object sender, EventArgs e)
        {
            if(floorheightmod.GetResult(0) == 0) floorheightmod.Text = "0";
			ComputeHeights();
        }

        private void ceilingheightmod_WhenTextChanged(object sender, EventArgs e)
        {
            if(ceilingheightmod.GetResult(0) == 0) ceilingheightmod.Text = "0";
			ComputeHeights();
        }

        private void floorflat_CheckedChanged(object sender, EventArgs e)
        {
            if(floorflat.Checked)
                floorflattexture.Enabled = true;
            else
                floorflattexture.Enabled = false;
        }

        private void ceilingflat_CheckedChanged(object sender, EventArgs e)
        {
            if(ceilingflat.Checked)
                ceilingflattexture.Enabled = true;
            else
                ceilingflattexture.Enabled = false;
        }

        private void spacing_WhenTextChanged(object sender, EventArgs e)
        {
            if(spacing.GetResult(0) == 0) spacing.Text = "0";
            DoRedrawDisplay();
        }

        private void uppertexture_CheckedChanged(object sender, EventArgs e)
        {
			if(uppertexture.Checked)
			{
				uppertexturetexture.Enabled = true;
				upperunpegged.Enabled = true;
			}
			else
			{
				uppertexturetexture.Enabled = false;
				upperunpegged.Enabled = false;
			}
        }

		private void middletexture_CheckedChanged(object sender, EventArgs e)
		{
			if(middletexture.Checked)
				middletexturetexture.Enabled = true;
			else
				middletexturetexture.Enabled = false;
		}

        private void lowertexture_CheckedChanged(object sender, EventArgs e)
        {
			if(lowertexture.Checked)
			{
				lowertexturetexture.Enabled = true;
				lowerunpegged.Enabled = true;
			}
			else
			{
				lowertexturetexture.Enabled = false;
				lowerunpegged.Enabled = false;
			}
        }

		private void firstisback_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void StairSectorBuilderForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// User closing the window?
			if(e.CloseReason == CloseReason.UserClosing)
			{
				// Just cancel
				General.Editing.CancelMode();
				e.Cancel = true;
			}
		}

		private void lastisback_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void acflipping1_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void acflipping2_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void acflipping3_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}

		private void floorbase_WhenTextChanged(object sender, EventArgs e)
		{
			if(floorbase.GetResult(0) == 0) floorbase.Text = "0";
			ComputeHeights();
		}

		private void ceilingbase_WhenTextChanged(object sender, EventArgs e)
		{
			if(ceilingbase.GetResult(0) == 0) ceilingbase.Text = "0";
			ComputeHeights();
		}

        private void prefabsave_Click(object sender, EventArgs e)
        {
			string name = prefabname.Text.Trim();

			if(name == "[Previous]")
				MessageBox.Show(Owner, "The prefab name \"[Previous]\" is reserved and can not be overwritten.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else if (name == "[Default]")
				MessageBox.Show(Owner, "The prefab name \"[Default]\" is reserved and can not be overwritten manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
				SavePrefab(prefabname.Text.Trim(), false, -1);
		}


		private void prefabdefault_Click(object sender, EventArgs e)
		{
			SavePrefab("[Default]", true, 0);
		}

		private void SavePrefab(string name, bool forceoverwrite, int position)
		{
            int overwrite = -1;

            // Prefab name may not be empty
            if(name == "")
            {
                MessageBox.Show(this.Owner, "Please enter a name for the prefab", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if there's already a prefab with the given name
			for(int i = 0; i < BuilderPlug.Me.Prefabs.Count; i++)
			{
				BuilderPlug.Prefab p = BuilderPlug.Me.Prefabs[i];

				if(p.name == name)
				{
					if(forceoverwrite == false && MessageBox.Show(this.Owner, "A prefab with that name already exists. Overwrite?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
						return;
					else
						overwrite = i;
				}
			}

            ListViewItem lvi = new ListViewItem();
            ListViewItem.ListViewSubItem lvisi = new ListViewItem.ListViewSubItem();

            lvi.Text = name;
            lvisi.Text = tabcontrol.TabPages[tabcontrol.SelectedIndex].Text;

            lvi.SubItems.Add(lvisi);

            BuilderPlug.Prefab pf = new BuilderPlug.Prefab();

            pf.name = name;

            pf.numberofsectors = (int)NumberOfSectors;
            pf.outervertexmultiplier = OuterVertexMultiplier;
            pf.innervertexmultiplier = InnerVertexMultiplier;

            pf.stairtype = tabcontrol.SelectedIndex;

            // Straight stairs
            pf.sectordepth = (int)SectorDepth;
            pf.spacing = Spacing;
            pf.frontside = SideFront;
            pf.singlesteps = SingleSteps.Checked;
			pf.distinctsectors = distinctsectors.Checked;
            pf.singledirection = SingleDirection.Checked;
			pf.distinctbaseheights = DistinctBaseHeights.Checked;

            // Auto curve
			pf.flipping = Flipping;

            // Catmull Rom spline
            pf.numberofcontrolpoints = NumControlPoints - 2;

            // Height info
            pf.applyfloormod = FloorHeight;
            pf.floormod = FloorHeightModification;
			//pf.floorbase = FloorBase;
            pf.applyceilingmod = CeilingHeight;
            pf.ceilingmod = CeilingHeightModification;
			//pf.ceilingbase = CeilingBase;

            // Textures
            pf.applyfloortexture = FloorFlat;
            pf.floortexture = FloorFlatTexture;

            pf.applyceilingtexture = CeilingFlat;
            pf.ceilingtexture = CeilingFlatTexture;

            pf.applyuppertexture = UpperTexture;
            pf.uppertexture = UpperTextureTexture;
			pf.upperunpegged = UpperUnpegged;

			pf.applymiddletexture = MiddleTexture;
			pf.middletexture = MiddleTextureTexture;

            pf.applylowertexture = LowerTexture;
            pf.lowertexture = LowerTextureTexture;
			pf.lowerunpegged = LowerUnpegged;

			if(overwrite == -1)
			{
				if(position == -1)
				{
					BuilderPlug.Me.Prefabs.Add(pf);
					prefabs.Items.Add(lvi);
				}
				else
				{
					BuilderPlug.Me.Prefabs.Insert(position, pf);
					prefabs.Items.Insert(position, lvi);
				}
			}
			else
			{
				BuilderPlug.Me.Prefabs.RemoveAt(overwrite);
				BuilderPlug.Me.Prefabs.Insert(overwrite, pf);

				prefabs.Items.RemoveAt(overwrite);
				prefabs.Items.Insert(overwrite, lvi);
			}
        }

		private void LoadPrefab(int position)
		{
			LoadPrefab(BuilderPlug.Me.Prefabs[position]);
		}

		private void LoadPrefab(BuilderPlug.Prefab p)
		{
			loadingprefab = true;

			prefabname.Text = p.name;

			NumberOfSectors = (uint)p.numberofsectors;
			OuterVertexMultiplier = p.outervertexmultiplier;
			InnerVertexMultiplier = p.innervertexmultiplier;

			tabcontrol.SelectedIndex = p.stairtype;

			// Straight stairs
			SectorDepth = (uint)p.sectordepth;
			Spacing = p.spacing;
			SideFront = p.frontside;
			SingleSteps.Checked = p.singlesteps;
			distinctsectors.Checked = p.distinctsectors;
			SingleDirection.Checked = p.singledirection;
			DistinctBaseHeights.Checked = p.distinctbaseheights;

			if (!SideFront)
				SideBack = true;

			// Auto curve TODO
			Flipping = p.flipping;

			// Catmull Rom spline
			NumControlPoints = p.numberofcontrolpoints;

			// Height info
			FloorHeight = p.applyfloormod;
			FloorHeightModification = p.floormod;
			//FloorBase = p.floorbase;
			CeilingHeight = p.applyceilingmod;
			CeilingHeightModification = p.ceilingmod;
			//CeilingBase = p.ceilingbase;

			// Textures
			FloorFlat = p.applyfloortexture;
			FloorFlatTexture = p.floortexture;
			CeilingFlat = p.applyceilingtexture;
			CeilingFlatTexture = p.ceilingtexture;

			UpperTexture = p.applyuppertexture;
			UpperTextureTexture = p.uppertexture;
			UpperUnpegged = p.upperunpegged;

			MiddleTexture = p.applymiddletexture;
			MiddleTextureTexture = p.middletexture;

			LowerTexture = p.applylowertexture;
			LowerTextureTexture = p.lowertexture;
			LowerUnpegged = p.lowerunpegged;

			loadingprefab = false;
		}

        private void prefabload_Click(object sender, EventArgs e)
        {
			if(prefabs.SelectedIndices.Count == 0) return;

			LoadPrefab(prefabs.SelectedIndices[0]);

			DoRedrawDisplay();
        }

		private void prefabdelete_Click(object sender, EventArgs e)
		{
			if(prefabs.SelectedIndices.Count == 0) return;

			BuilderPlug.Me.Prefabs.RemoveAt(prefabs.SelectedIndices[0]);
			prefabs.Items.RemoveAt(prefabs.SelectedIndices[0]);
		}

		private void distinctbaseheights_CheckedChanged(object sender, EventArgs e)
		{
			if(distinctbaseheights.Checked)
			{
				floorbase.Enabled = false;
				ceilingbase.Enabled = false;
			}
			else
			{
				if(floorheightmodification.Checked) floorbase.Enabled = true;
				if(ceilingheightmodification.Checked) ceilingbase.Enabled = true;
			}

			ComputeHeights();
		}

		private void autocurveflipping_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(autocurveflipping.SelectedIndex != splineflipping.SelectedIndex)
				splineflipping.SelectedIndex = autocurveflipping.SelectedIndex;

			DoRedrawDisplay();
		}

		private void splineflipping_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(splineflipping.SelectedIndex != autocurveflipping.SelectedIndex)
				autocurveflipping.SelectedIndex = splineflipping.SelectedIndex;

			DoRedrawDisplay();
		}

		private void floorheightmodification_CheckedChanged(object sender, EventArgs e)
		{
			if(floorheightmodification.Checked)
			{
				floorheightmod.Enabled = true;

				if(StairType != 0 || distinctbaseheights.Checked == false)	floorbase.Enabled = true;
			}
			else
			{
				floorheightmod.Enabled = false;

				if(StairType != 0 || distinctbaseheights.Checked == false)	floorbase.Enabled = false;
			}
		}

		private void ceilingheightmodification_CheckedChanged(object sender, EventArgs e)
		{
			if(ceilingheightmodification.Checked)
			{
				ceilingheightmod.Enabled = true;

				if(StairType != 0 || distinctbaseheights.Checked == false) ceilingbase.Enabled = true;
			}
			else
			{
				ceilingheightmod.Enabled = false;

				if(StairType != 0 || distinctbaseheights.Checked == false) ceilingbase.Enabled = false;
			}
		}

		private void floorbasegetter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			floorbase.Text = originalfloorbase.ToString();
		}

		private void ceilingbasegetter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ceilingbase.Text = originalceilingbase.ToString();
		}

		private void prefabs_DoubleClick(object sender, EventArgs e)
		{
			if (prefabs.SelectedIndices.Count == 0) return;

			LoadPrefab(prefabs.SelectedIndices[0]);

			DoRedrawDisplay();
		}

		private void distinctsectors_CheckedChanged(object sender, EventArgs e)
		{
			DoRedrawDisplay();
		}
	}
}
