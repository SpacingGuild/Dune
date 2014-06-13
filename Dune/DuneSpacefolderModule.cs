using System;
using System.Linq;
using UnityEngine;

namespace Dune
{
    public class DuneSpacefolderModule : PartModule
    {
        public string engineName { get; private set; }
        public double engineEfficiency { get; private set; }
        public double engineFailure { get; private set; }

        public ConfigNode configModule;

        public override void OnStart(PartModule.StartState state)
        {
            if (state != StartState.Editor && state != StartState.None)
            {
                if (!FlightGlobals.ActiveVessel.IsNull())
                {
                    engineName = part.name;
                    engineEfficiency = double.Parse(configModule.GetValue("efficiency"));
                    engineFailure = double.Parse(configModule.GetValue("failure"));
                }
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            if (configModule == null)
            {
                configModule = new ConfigNode();
                node.CopyTo(configModule);
            }
            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            if (node != null) base.OnSave(node);
            else base.OnSave(configModule);
        }
    }
}
