using System;
using System.Linq;
using UnityEngine;

namespace Dune
{
    public class DuneNavigatorModule : PartModule
    {
        public ConfigNode configModule;

        public override void OnStart(PartModule.StartState state)
        {
            if (state != StartState.Editor && state != StartState.None)
            {

            }
        }

        //public override void OnFixedUpdate()
        //{
        //    // TODO: Need more checks to make sure it is only running the master object. But we cant do that, as long as the Core cant handle PartModules.
        //}

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
