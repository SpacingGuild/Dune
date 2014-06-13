using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dune
{
    public class DuneSpacefolderControl : ControlModule
    {
        public DuneSpacefolderControl(DuneCore core)
            : base(core)
        {
            runModuleInScenes.Add(GameScenes.FLIGHT);
        }

        private DuneSpacefolderModule spacefolderModule;

        public bool settingsRetrieved { get; private set; }
        public string engineName { get; private set; }
        public double engineEfficiency { get; private set; }
        public double engineFailure { get; private set; }

        public override void OnUpdate()
        {
            if (!FlightGlobals.ActiveVessel.IsNull())
            {
                if (FlightGlobals.ActiveVessel.GetModules<DuneSpacefolderModule>().Count > 0)
                {
                    spacefolderModule = (DuneSpacefolderModule)FlightGlobals.ActiveVessel.GetMasterObject<DuneSpacefolderModule>();
                }
                else
                {
                    spacefolderModule = null;
                }
            }

            if (!spacefolderModule.IsNull())
            {
                settingsRetrieved = true;
                engineName = spacefolderModule.engineName;
                engineEfficiency = spacefolderModule.engineEfficiency;
                engineFailure = spacefolderModule.engineFailure;
            }
            else
            {
                settingsRetrieved = false;
            }
        }
    }
}
