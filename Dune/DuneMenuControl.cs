using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dune
{
    public class DuneMenuControl : ControlModule
    {
        public DuneMenuControl(DuneCore core) : base(core)
        {
            priority = -1000;
            runModuleInScenes.Add(GameScenes.FLIGHT);
            runModuleInScenes.Add(GameScenes.SPACECENTER);
            runModuleInScenes.Add(GameScenes.TRACKSTATION);

            if (toolbarButtons == null)
                toolbarButtons = new Dictionary<string, IButton>();

            if (missingIcons == null)
                missingIcons = new HashSet<string>();
        }

        private static Dictionary<string, IButton> toolbarButtons;
        private static HashSet<string> missingIcons;

        public override void OnStart()
        {
            SetupToolBarButtons();
        }

        public override void OnUpdate()
        {
            if(!ToolbarManager.ToolbarAvailable)
            {
                foreach (DisplayModule module in core.GetControlModules<DisplayModule>())
                {
                    if (!module.windowIsHidden && module.runModuleInScenes.Contains(HighLogic.LoadedScene))
                    {
                        module.enabled = true;
                    }
                }
            }
        }

        public void SetupToolBarButtons()
        {
            foreach(DisplayModule module in core.GetControlModules<DisplayModule>())
            {
                if(!module.windowIsHidden)
                {
                    SetupToolbarButton(module);
                }
            }
        }

        public void SetupToolbarButton(DisplayModule module)
        {
            if (!ToolbarManager.ToolbarAvailable) return;

            if(!module.windowIsHidden)
            {
                IButton btn;
                string name = CleanName(module.GetName());
                if(!toolbarButtons.ContainsKey(name))
                {
                    Debug.Log("[Dune] MenuControl Add btn for: " + name);
                    btn = ToolbarManager.Instance.add("Dune", name);
                    toolbarButtons[name] = btn;
                    btn.ToolTip = (module.enabled ? "Hide" : "Show")+ " Dune " + module.GetName();

                    btn.OnClick += (b) =>
                    {
                        DisplayModule mod = core.GetControlModules<DisplayModule>().FirstOrDefault(m => m.GetName() == module.GetName());
                        if(mod != null)
                        {
                            mod.enabled = !mod.enabled;
                        }
                    };
                }
                else
                {
                    btn = toolbarButtons[name];
                }

                btn.Visibility = new GameScenesVisibility(module.runModuleInScenes.ToArray());
                string TexturePath = "SpacingGuild/Dune/Icons/" + name;
                if(GameDatabase.Instance.GetTexture(TexturePath, false) == null)
                {
                    TexturePath = "SpacingGuild/Dune/Icons/QMark";
                    if(!missingIcons.Contains(name))
                    {
                        missingIcons.Add(name);
                        Debug.Log("[Dune] MenuControl No icon for " + name);
                    }
                }
                btn.TexturePath = TexturePath;
            }
        }

        public override void OnDestroy()
        {
            //TODO: Fix KeyNotFoundException: The given key was not present in the dictionary.
            // Neither of the Debug.Log() are run, suggests that MenuControl is either already dead, or
            // has been cleaned at some other time. But when ?
            if(ToolbarManager.ToolbarAvailable)
            {
                foreach(Button btn in toolbarButtons.Values)
                {
                    btn.Destroy();
                }
                toolbarButtons.Clear();
            }
            base.OnDestroy();
        }

        public static string CleanName(string name)
        {
            return name.Replace('.', '_').Replace(' ', '_').Replace(':', '_').Replace('/', '_');
        }
    }
}
