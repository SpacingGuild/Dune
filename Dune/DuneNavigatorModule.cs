using System;
using System.Linq;
using UnityEngine;

namespace Dune
{
    public class DuneNavigatorModule : PartModule
    {
        public ConfigNode configModule;
        public bool initiateSpacefold = false;

        //[KSPField(isPersistant = false, guiActive = true, guiName = "Spacefold Cooldown: ")]
        //public string spacefoldCooldown = "Standby";

        //[KSPEvent(guiActive = true, guiName = "Activate", active = true, guiActiveUnfocused = true)]
        //public void ActivateSpacefold()
        //{
        //    initiateSpacefold = true;
        //    Events["ActivateSpacefold"].active = false;
        //    Events["DeactivateSpacefold"].active = true;
        //}

        //[KSPEvent(guiActive = true, guiName = "Deactivate", active = false, guiActiveUnfocused = true)]
        //public void DeactivateSpacefold()
        //{
        //    initiateSpacefold = false;
        //    Events["DeactivateSpacefold"].active = false;
        //    Events["ActivateSpacefold"].active = true;
        //}

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
