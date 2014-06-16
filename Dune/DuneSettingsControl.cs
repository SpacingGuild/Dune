using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dune
{
    public class DuneSettingsControl : ControlModule
    {
        public DuneSettingsControl(DuneCore core) : base(core)
        {
            priority = 1000;
            runModuleInScenes.Add(GameScenes.SPACECENTER);
            runModuleInScenes.Add(GameScenes.TRACKSTATION);
            runModuleInScenes.Add(GameScenes.FLIGHT);
            runModuleInScenes.Add(GameScenes.EDITOR);
        }

        public string difficulty = "normal";
        public bool hardmode = false;

        public override void OnStart()
        {
            difficulty = SettingsManager.GetValue("Difficulty");
            hardmode = Utilities.TryParse(SettingsManager.GetValue("Hardmode"),false);
        }

        public override void OnDestroy()
        {
            SettingsManager.SetValue("Difficulty", difficulty);
            SettingsManager.SetValue("hardmode", hardmode);
            SettingsManager.Save();
        }
    }
}
