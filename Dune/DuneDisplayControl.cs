using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dune
{
    public class DuneDisplayControl : ControlModule
    {
        public DuneDisplayControl(DuneCore core)
            : base(core)
        {
            priority = -1000;
            runModuleInScenes.Add(GameScenes.FLIGHT);
            runModuleInScenes.Add(GameScenes.SPACECENTER);
            runModuleInScenes.Add(GameScenes.TRACKSTATION);
            runModuleInScenes.Add(GameScenes.EDITOR);

            if (toolbarButtons == null)
                toolbarButtons = new Dictionary<string, IButton>();

            if (missingIcons == null)
                missingIcons = new HashSet<string>();
        }

        private static Dictionary<string, IButton> toolbarButtons;
        private static HashSet<string> missingIcons;

        //private static float lastTimeCheck = 0;

        public override void OnStart()
        {
            if (!ToolbarManager.ToolbarAvailable)
            {
                SetupToolBarButtons();
            }
        }

        public override void OnUpdate()
        {
            if (!ToolbarManager.ToolbarAvailable)
            {
                //if (Time.time > lastTimeCheck + 5)
                //{
                //    lastTimeCheck = Time.time;
                //    SetupToolBarButtons();
                //}
            }
        }

        public void SetupToolBarButtons()
        {
            foreach (DisplayModule module in core.GetControlModules<DisplayModule>())
            {
                if (module.enabled)
                {
                    SetupToolbarButton(module);
                }
            }
        }

        public void SetupToolbarButton(DisplayModule module)
        {
            IButton btn;
            string name = CleanName(module.GetName());
            if (!toolbarButtons.ContainsKey(name))
            {
                Debug.Log("[Dune] MenuControl Add btn for: " + name);
                btn = ToolbarManager.Instance.add("Dune", name);
                toolbarButtons[name] = btn;
                btn.ToolTip = (module.windowIsHidden ? "Show" : "Hide") + " Dune " + module.GetName();

                btn.OnClick += (b) =>
                {
                    DisplayModule mod = core.GetControlModules<DisplayModule>().FirstOrDefault(m => m.GetName() == module.GetName());
                    if (mod != null)
                    {
                        mod.windowIsHidden = !mod.windowIsHidden;
                    }
                };
            }
            else
            {
                btn = toolbarButtons[name];
            }

            btn.Visibility = new GameScenesVisibility(module.runModuleInScenes.ToArray());
            string TexturePath = "SpacingGuild/Dune/Icons/" + name;
            if (GameDatabase.Instance.GetTexture(TexturePath, false) == null)
            {
                TexturePath = "SpacingGuild/Dune/Icons/QMark";
                if (!missingIcons.Contains(name))
                {
                    missingIcons.Add(name);
                    Debug.Log("[Dune] MenuControl No icon for " + name);
                }
            }
            btn.TexturePath = TexturePath;
        }

        public override void OnDestroy()
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                foreach (Button btn in toolbarButtons.Values)
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
