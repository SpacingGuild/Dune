using System;
using System.Collections.Generic;
using System.Linq;

namespace Dune
{
    //COMMENT: Wouldn't it be better for this to run on startup rather than at specific scenes ?
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

        public override void OnStart()
        {
            core.difficulty = SettingsManager.GetValue("Difficulty");
            core.hardmode = Utilities.TryParse(SettingsManager.GetValue("Hardmode"),false);
        }

        public override void OnDestroy()
        {
            SettingsManager.SetValue("Difficulty", core.difficulty);
            SettingsManager.SetValue("hardmode", core.hardmode);
            SettingsManager.Save();
        }
    }
}
