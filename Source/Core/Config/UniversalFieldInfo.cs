
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

using System;
using System.Collections;
using System.Collections.Generic;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public enum UDMFFieldAssociationModifier
	{
		None,
		Absolute
	}

	public struct UDMFFieldAssociation
	{
		public string Property;
		public UDMFFieldAssociationModifier Modify;
		public bool NeverShowEventLines;
		public bool ConsolidateEventLines;

		public UDMFFieldAssociation(string property, UDMFFieldAssociationModifier modify, bool nevershoweventlines, bool consolidateeventlines)
		{
			Property = property;
			Modify = modify;
			NeverShowEventLines = nevershoweventlines;
			ConsolidateEventLines = consolidateeventlines;
		}
	}

	public class UniversalFieldInfo : IComparable<UniversalFieldInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Properties
		private string name;
		private int type;
		private object defaultvalue;
		private bool thingtypespecific;
		private bool managed;
		private EnumList enumlist;
		private Dictionary<string, UDMFFieldAssociation> associations;

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public int Type { get { return type; } }
		public object Default { get { return defaultvalue; } }
		public bool ThingTypeSpecific { get { return thingtypespecific; } }
		public bool Managed { get { return managed; } }
		public EnumList Enum { get { return enumlist; } }
		public Dictionary<string, UDMFFieldAssociation> Associations { get { return associations; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal UniversalFieldInfo(string path, string name, string configname, Configuration cfg, IDictionary<string, EnumList> enums)
		{
			string setting = "universalfields." + path + "." + name;

			// Initialize
			this.name = name.ToLowerInvariant();
			associations = new Dictionary<string, UDMFFieldAssociation>();

			// Read type
			type = cfg.ReadSetting(setting + ".type", int.MinValue);
			defaultvalue = cfg.ReadSettingObject(setting + ".default", null);
			thingtypespecific = cfg.ReadSetting(setting + ".thingtypespecific", false);
			managed = cfg.ReadSetting(setting + ".managed", true);

			// Read enum
			object enumsetting = cfg.ReadSettingObject(setting + ".enum", null);
			if (enumsetting != null)
			{
				// Reference to existing enums list?
				if (enumsetting is string)
				{
					// Link to it
					enumlist = enums[enumsetting.ToString()];
				}
				else if (enumsetting is IDictionary)
				{
					// Make list
					enumlist = new EnumList(enumsetting as IDictionary);
				}
			}

			//mxd. Check type
			if (this.type == int.MinValue)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "No type is defined for universal field \"" + name + "\" defined in \"" + configname + "\". Integer type will be used.");
				this.type = (int)UniversalType.Integer;
			}

			if(type == (int)UniversalType.EnumOption && enumsetting == null)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "Universal field \"" + name + "\" defined in \"" + configname + "\" is of type enum (" + this.type + "), but has no enum values set. Falling back to integer type");
				type = (int)UniversalType.Integer;
			}

			TypeHandler th = General.Types.GetFieldHandler(this);
			if(th is NullHandler)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "Universal field \"" + name + "\" defined in \"" + configname + "\" has unknown type " + this.type + ". String type will be used instead.");
				this.type = (int)UniversalType.String;
				if(this.defaultvalue == null) this.defaultvalue = "";
			}

			//mxd. Default value is missing? Get it from typehandler
			if(this.defaultvalue == null) this.defaultvalue = th.GetDefaultValue();

			// Read associations
			IDictionary assocdict = cfg.ReadSetting(setting + ".associations", new Hashtable());
			foreach (DictionaryEntry section in assocdict)
			{
				string property = cfg.ReadSetting(setting + ".associations." + section.Key + ".property", string.Empty);
				string modifystr = cfg.ReadSetting(setting + ".associations." + section.Key + ".modify", string.Empty);
				bool nevershoweventlines = cfg.ReadSetting(setting + ".associations." + section.Key + ".nevershoweventlines", false);
				bool consolidateeventlines = cfg.ReadSetting(setting + ".associations." + section.Key + ".consolidateeventlines", false);
				UDMFFieldAssociationModifier ufam = UDMFFieldAssociationModifier.None;

				if(!string.IsNullOrWhiteSpace(property))
				{
					switch (modifystr)
					{
						case "abs":
							ufam = UDMFFieldAssociationModifier.Absolute;
							break;
					}

					associations[property] = new UDMFFieldAssociation(property, ufam, nevershoweventlines, consolidateeventlines);
				}
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		internal UniversalFieldInfo(string name, int type, object defaultvalue)
		{
			this.name = name.ToLowerInvariant();
			this.type = type;
			this.defaultvalue = defaultvalue;
		}

		#endregion

		#region ================== Methods

		// This presents the item as string
		public override string ToString()
		{
			return name;
		}

		// This compares against another field
		public int CompareTo(UniversalFieldInfo other)
		{
			return string.Compare(this.name, other.name);
		}

		#endregion
	}
}
