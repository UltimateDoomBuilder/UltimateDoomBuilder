
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
using System.Collections.Generic;
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.IO;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Actions
{
    public class ActionManager
    {
        #region ================== Constants

        private const string ACTIONS_RESOURCE = "Actions.cfg";

        #endregion

        #region ================== Variables

        // Actions
        private Dictionary<string, Action> actions;

        // Categories
        private SortedDictionary<string, string> categories;

        // Keys state
        private int modifiers;
        private List<int> pressedkeys;

        // Begun actions
        private List<Action> activeactions;
        private Action currentaction;

        // Exclusive invokation
        private bool exclusiverequested;

        // Disposing
        private bool isdisposed;

        #endregion

        #region ================== Properties

        internal SortedDictionary<string, string> Categories { get { return categories; } }
        internal Action this[string action] { get { if (actions.ContainsKey(action)) return actions[action]; else throw new ArgumentException("There is no such action \"" + action + "\""); } }
        public bool IsDisposed { get { return isdisposed; } }
        internal bool ExclusiveRequested { get { return exclusiverequested; } }

        /// <summary>
        /// Current executing action. This returns Null when no action is invoked.
        /// </summary>
        public Action Current { get { return currentaction; } internal set { currentaction = value; } }

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        internal ActionManager()
        {
            // Initialize
            General.WriteLogLine("Starting action manager...");
            actions = new Dictionary<string, Action>();
            pressedkeys = new List<int>();
            activeactions = new List<Action>();
            categories = new SortedDictionary<string, string>();

            // Load all actions in this assembly
            LoadActions(General.ThisAssembly);

            // We have no destructor
            GC.SuppressFinalize(this);
        }

        // Disposer
        internal void Dispose()
        {
            // Not already disposed?
            if (!isdisposed)
            {
                // Clean up

                // Done
                isdisposed = true;
            }
        }

        #endregion

        #region ================== Actions

        // This loads all actions from an assembly
        internal void LoadActions(Assembly asm)
        {
            AssemblyName asmname = asm.GetName();

            // Find a resource named Actions.cfg
            string[] resnames = asm.GetManifestResourceNames();
            foreach (string rn in resnames)
            {
                // Found one?
                if (rn.EndsWith(ACTIONS_RESOURCE, StringComparison.OrdinalIgnoreCase))
                {
                    // Get a stream from the resource
                    Stream actionsdata = asm.GetManifestResourceStream(rn);
                    StreamReader actionsreader = new StreamReader(actionsdata, Encoding.ASCII);

                    // Load configuration from stream
                    Configuration cfg = new Configuration();
                    cfg.InputConfiguration(actionsreader.ReadToEnd());
                    if (cfg.ErrorResult)
                    {
                        string errordesc = "Error in Actions configuration on line " + cfg.ErrorLine + ": " + cfg.ErrorDescription;
                        General.CancelAutoMapLoad();
                        General.ErrorLogger.Add(ErrorType.Error, "Unable to read Actions configuration from assembly " + Path.GetFileName(asm.Location));
                        General.WriteLogLine(errordesc);
                        General.ShowErrorMessage("Unable to read Actions configuration from assembly " + Path.GetFileName(asm.Location) + "!\n" + errordesc, MessageBoxButtons.OK);
                    }
                    else
                    {
                        // Read the categories structure
                        IDictionary cats = cfg.ReadSetting("categories", new Hashtable());
                        foreach (DictionaryEntry c in cats)
                        {
                            // Make the category if not already added
                            if (!categories.ContainsKey(c.Key.ToString()))
                                categories.Add(c.Key.ToString(), c.Value.ToString());
                        }

                        // Go for all objects in the configuration
                        foreach (DictionaryEntry a in cfg.Root)
                        {
                            // Get action properties
                            string shortname = a.Key.ToString();
                            string name = asmname.Name.ToLowerInvariant() + "_" + shortname;
                            bool debugonly = cfg.ReadSetting(a.Key + ".debugonly", false);

                            // Not the categories structure?
                            if (shortname.ToLowerInvariant() != "categories")
                            {
                                // Check if action should be included
                                if (General.DebugBuild || !debugonly)
                                {
                                    // Create an action
                                    CreateAction(cfg, name, shortname);
                                }
                            }
                        }
                    }

                    // Done with the resource
                    actionsreader.Dispose();
                    break; //mxd. Usually we have a single "Actions.cfg", right?
                }
            }
        }

        // This manually creates an action
        private void CreateAction(Configuration cfg, string name, string shortname)
        {
            // Action does not exist yet?
            if (!actions.ContainsKey(name))
            {
                // Read the key from configuration
                int key = General.Settings.ReadSetting("shortcuts." + name, -1);

                // Create an action
                actions.Add(name, new Action(cfg, name, shortname, key));
            }
            else
            {
                // Action already exists!
                General.ErrorLogger.Add(ErrorType.Warning, "Action \"" + name + "\" already exists. Action names must be unique.");
            }
        }

        // This binds all methods marked with this attribute
        public void BindMethods(Type type)
        {
            // Bind static methods
            BindMethods(null, type);
        }

        // This binds all methods marked with this attribute
        public void BindMethods(object obj)
        {
            // Bind instance methods
            BindMethods(obj, obj.GetType());
        }

        // This binds all methods marked with this attribute
        private void BindMethods(object obj, Type type)
        {
            if (obj == null)
                General.WriteLogLine("Binding static action methods for class " + type.Name + "...");
            else
                General.WriteLogLine("Binding action methods for " + type.Name + " object...");

            // Go for all methods on obj
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            //foreach(MethodInfo m in methods)
            int methodsCount = methods.Length;
            for (int i = 0; i < methodsCount; i++)
            {
                MethodInfo m = methods[i];
                // Check if the method has this attribute
                ActionAttribute[] attrs = (ActionAttribute[])methods[i].GetCustomAttributes(typeof(BeginActionAttribute), true);

                int attrsCount = attrs.Length;
                // Go for all attributes
                //foreach(ActionAttribute a in attrs)
                for (int j = 0; j < attrsCount; j++)
                {
                    ActionAttribute a = attrs[j];

                    // Create a delegate for this method
                    ActionDelegate del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

                    // Make proper name
                    string actionname = a.GetFullActionName(type.Assembly);

                    // Bind method to action
                    if (Exists(actionname))
                        actions[actionname].BindBegin(del);
                    else
                        throw new ArgumentException("Could not bind " + m.ReflectedType.Name + "." + m.Name + " to action \"" + actionname + "\", that action does not exist! Refer to, or edit Actions.cfg for all available application actions.");
                }

                // Check if the method has this attribute
                attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(EndActionAttribute), true);
                attrsCount = attrs.Length;
                // Go for all attributes
                //foreach (ActionAttribute a in attrs)
                for (int j = 0; j < attrsCount; j++)
                {
                    ActionAttribute a = attrs[j];

                    // Create a delegate for this method
                    ActionDelegate del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

                    // Make proper name
                    string actionname = a.GetFullActionName(type.Assembly);

                    // Bind method to action
                    if (Exists(actionname))
                        actions[actionname].BindEnd(del);
                    else
                        throw new ArgumentException("Could not bind " + m.ReflectedType.Name + "." + m.Name + " to action \"" + actionname + "\", that action does not exist. Refer to, or edit Actions.cfg for all available application actions.");
                }
            }
        }

        // This binds a delegate manually
        internal void BindBeginDelegate(Assembly asm, ActionDelegate d, BeginActionAttribute a)
        {
            // Make proper name
            string actionname = a.GetFullActionName(asm);

            // Bind delegate to action
            if (Exists(actionname))
                actions[actionname].BindBegin(d);
            else
                General.ErrorLogger.Add(ErrorType.Warning, "Could not bind delegate for " + d.Method.Name + " to action \"" + a.ActionName + "\" (" + actionname + "), that action does not exist. Refer to, or edit Actions.cfg for all available application actions.");
        }

        // This binds a delegate manually
        internal void BindEndDelegate(Assembly asm, ActionDelegate d, EndActionAttribute a)
        {
            // Make proper name
            string actionname = a.GetFullActionName(asm);

            // Bind delegate to action
            if (Exists(actionname))
                actions[actionname].BindEnd(d);
            else
                General.ErrorLogger.Add(ErrorType.Warning, "Could not bind delegate for " + d.Method.Name + " to action \"" + a.ActionName + "\" (" + actionname + "), that action does not exist. Refer to, or edit Actions.cfg for all available application actions.");
        }

        // This unbinds all methods marked with this attribute
        public void UnbindMethods(Type type)
        {
            // Unbind static methods
            UnbindMethods(null, type);
        }

        // This unbinds all methods marked with this attribute
        public void UnbindMethods(object obj)
        {
            // Unbind instance methods
            UnbindMethods(obj, obj.GetType());
        }

        // This unbinds all methods marked with this attribute
        private void UnbindMethods(object obj, Type type)
        {
            if (obj == null)
                General.WriteLogLine("Unbinding static action methods for class " + type.Name + "...");
            else
                General.WriteLogLine("Unbinding action methods for " + type.Name + " object...");

            // Go for all methods on obj
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            //foreach (MethodInfo m in methods)
            int methodsCount = methods.Length;
            for(int i = 0; i < methodsCount; i++)
            {
                MethodInfo m = methods[i];
                // Check if the method has this attribute
                ActionAttribute[] attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(BeginActionAttribute), true);

                // Go for all attributes
                //foreach (ActionAttribute a in attrs)
                int attrsCount = attrs.Length;
                for(int j = 0; j < attrsCount; j++)
                {
                    ActionAttribute a = attrs[j];
                    // Create a delegate for this method
                    ActionDelegate del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

                    // Make proper name
                    string actionname = a.GetFullActionName(type.Assembly);

                    // Unbind method from action
                    actions[actionname].UnbindBegin(del);
                }

                // Check if the method has this attribute
                attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(EndActionAttribute), true);

                // Go for all attributes
                //foreach (ActionAttribute a in attrs)
                attrsCount = attrs.Length;
                for (int j = 0; j < attrsCount; j++)
                {
                    ActionAttribute a = attrs[j];
                    // Create a delegate for this method
                    ActionDelegate del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

                    // Make proper name
                    string actionname = a.GetFullActionName(type.Assembly);

                    // Unbind method from action
                    actions[actionname].UnbindEnd(del);
                }
            }
        }

        // This unbinds a delegate manually
        internal void UnbindBeginDelegate(Assembly asm, ActionDelegate d, BeginActionAttribute a)
        {
            // Make proper name
            string actionname = a.GetFullActionName(asm);

            // Unbind delegate to action
            actions[actionname].UnbindBegin(d);
        }

        // This unbinds a delegate manually
        internal void UnbindEndDelegate(Assembly asm, ActionDelegate d, EndActionAttribute a)
        {
            // Make proper name
            string actionname = a.GetFullActionName(asm);

            // Unbind delegate to action
            actions[actionname].UnbindEnd(d);
        }

        // This checks if a given action exists
        public bool Exists(string action)
        {
            return actions.ContainsKey(action);
        }

        // This returns a list of all actions
        internal Action[] GetAllActions()
        {
            Action[] list = new Action[actions.Count];
            actions.Values.CopyTo(list, 0);
            return list;
        }

        // This returns the specified action
        public Action GetActionByName(string fullname)
        {
            return actions[fullname];
        }

        // This saves the control settings
        internal void SaveSettings()
        {
            // Go for all actions
            foreach (KeyValuePair<string, Action> a in actions)
            {
                // Write to configuration
                General.Settings.WriteSetting("shortcuts." + a.Key, a.Value.ShortcutKey);
            }
        }

        // This invokes the Begin and End of the given action
        public bool InvokeAction(string actionname)
        {
            if (Exists(actionname))
            {
                actions[actionname].Invoke();
                return true;
            }
            return false;
        }

        #endregion

        #region ================== Shortcut Keys

        // This applies default keys if they are not already in use
        internal void ApplyDefaultShortcutKeys()
        {
            // Find actions that have no key set
            foreach (KeyValuePair<string, Action> a in actions)
            {
                // Key set?
                if (a.Value.ShortcutKey == -1)
                {
                    // Check if the default key is not already used
                    bool keyused = false;
                    foreach (KeyValuePair<string, Action> d in actions)
                    {
                        // Check if the keys are the same
                        // Note that I use the mask of the source action to check if they match any combination
                        if ((d.Value.ShortcutKey & a.Value.ShortcutMask) == (a.Value.DefaultShortcutKey & a.Value.ShortcutMask))
                        {
                            // No party.
                            keyused = true;
                            break;
                        }
                    }

                    // Party?
                    if (!keyused)
                    {
                        // Apply the default key
                        a.Value.SetShortcutKey(a.Value.DefaultShortcutKey);
                    }
                    else
                    {
                        // No party.
                        a.Value.SetShortcutKey(0);
                    }
                }
            }
        }

        // This checks if a given action is active
        public bool CheckActionActive(Assembly asm, string actionname)
        {
            if (asm == null) asm = General.ThisAssembly;

            // Find active action
            string fullname = asm.GetName().Name.ToLowerInvariant() + "_" + actionname;
            foreach (Action a in activeactions)
            {
                if (a.Name == fullname) return true;
            }

            // No such active action
            return false;
        }

        // Removes all shortcut keys
        internal void RemoveShortcutKeys()
        {
            // Clear all keys
            foreach (KeyValuePair<string, Action> a in actions)
                a.Value.SetShortcutKey(0);
        }

        // This notifies a key has been pressed
        // Returns true when the key press has been absorbed
        internal bool KeyPressed(int key)
        {
            int strippedkey = key & ~((int)Keys.Alt | (int)Keys.Shift | (int)Keys.Control);
            if ((strippedkey == (int)Keys.ShiftKey) || (strippedkey == (int)Keys.ControlKey) || (strippedkey == (int)Keys.Alt)) key = strippedkey;
            bool repeat = pressedkeys.Contains(strippedkey);

            // Update pressed keys
            if (!repeat) pressedkeys.Add(strippedkey);

			if (key == 0) {
				return false;
			}

            // Add action to active list
            Action[] acts = GetActionsByKey(key);
            bool absorbed = acts.Length > 0;
            foreach (Action a in acts) if (!activeactions.Contains(a)) activeactions.Add(a);

            // Invoke actions
            absorbed |= BeginActionByKey(key, repeat);

            return absorbed;
        }

        // This notifies a key has been released
        // Returns true when the key release has been absorbed
        internal bool KeyReleased(int key)
        {
            int strippedkey = key & ~((int)Keys.Alt | (int)Keys.Shift | (int)Keys.Control);

            // Update pressed keys
            if (pressedkeys.Contains(strippedkey)) pressedkeys.Remove(strippedkey);

            // End actions that no longer match
            return EndActiveActions();
        }

        // This releases all pressed keys
        internal void ReleaseAllKeys()
        {
            // Clear pressed keys
            pressedkeys.Clear();

            // End actions
            EndActiveActions();
        }

        // This updates the modifiers
        internal void UpdateModifiers(int mods)
        {
            // Update modifiers
            modifiers = mods;

            // End actions that no longer match
            EndActiveActions();
        }

        // This will call the associated actions for a keypress
        // Returns true when the key invokes any action
        internal bool BeginActionByKey(int key, bool repeated)
        {
            bool invoked = false;

            // Get all actions for which a begin is bound
            List<Action> boundactions = new List<Action>(actions.Count);
            foreach (KeyValuePair<string, Action> a in actions)
                if (a.Value.BeginBound) boundactions.Add(a.Value);

            // Go for all actions
            foreach (Action a in boundactions)
            {
                // This action is associated with this key?
                if (a.KeyMatches(key))
                {
                    invoked = true;

                    // Allowed to repeat?
                    if (a.Repeat || !repeated)
                    {
                        // Invoke action
                        a.Begin();
                    }
                    else
                    {
                        //General.WriteLogLine("Action \"" + a.Value.Name + "\" failed because it does not support repeating activation!");
                    }
                }
            }

            return invoked;
        }

        // This will end active actions for which the pressed keys do not match
        // Returns true when actions have been ended
        private bool EndActiveActions()
        {
            bool listchanged;
            bool actionsended = false;

            do
            {
                // Go for all active actions
                listchanged = false;
                for (int i = 0; i < activeactions.Count; i++)
                {
                    Action a = activeactions[i];

                    // Go for all pressed keys
                    bool stillactive = false;
                    foreach (int k in pressedkeys)
                    {
                        if ((k == (int)Keys.ShiftKey) || (k == (int)Keys.ControlKey))
                            stillactive |= a.KeyMatches(k);
                        else
                            stillactive |= a.KeyMatches(k | modifiers);
                    }

                    // End the action if no longer matches any of the keys
                    if (!stillactive)
                    {
                        actionsended = true;
                        activeactions.RemoveAt(i);
                        listchanged = true;
                        a.End();
                        break;
                    }
                }
            }
            while (listchanged);

            return actionsended;
        }

        // This returns all action names for a given key
        public string[] GetActionNamesByKey(int key)
        {
            List<string> actionnames = new List<string>();

            // Go for all actions
            foreach (KeyValuePair<string, Action> a in actions)
            {
                // This action is associated with this key?
                if (a.Value.KeyMatches(key))
                {
                    // List short name
                    actionnames.Add(a.Value.ShortName);
                }
            }

            // Return result;
            return actionnames.ToArray();
        }

        // This returns all action names for a given key
        public Action[] GetActionsByKey(int key)
        {
            List<Action> actionnames = new List<Action>();

            // Go for all actions
            foreach (KeyValuePair<string, Action> a in actions)
            {
                // This action is associated with this key?
                if (a.Value.KeyMatches(key))
                {
                    // List short name
                    actionnames.Add(a.Value);
                }
            }

            // Return result;
            return actionnames.ToArray();
        }

        #endregion

        #region ================== Exclusive Invokation

        // This resets the exclusive request
        internal void ResetExclusiveRequest()
        {
            exclusiverequested = false;
        }

        /// <summary>
        /// This asks for exclusive invokation of the current BeginAction or EndAction.
        /// Returns true when successull, false when denied (already given to another caller)
        /// </summary>
        public bool RequestExclusiveInvokation()
        {
            if (exclusiverequested) return false; // Already given out

            // Success
            exclusiverequested = true;
            return true;
        }

        #endregion
    }
}
