using UnityEngine;

namespace Dune
{
    public class DuneThrottleControl : ControlModule
    {
        public DuneThrottleControl(DuneCore core) : base(core)
        {
            runModuleInScenes.Add(GameScenes.FLIGHT);
        }

        public override void OnUpdate()
        {
            if(!FlightGlobals.ActiveVessel.IsNull())
                ThrottleControl();
        }

        private void ThrottleControl()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Z))
            {
                Debug.Log("[Dune] Throttle Max");
                FlightGlobals.ActiveVessel.MaxThrottle();
            }
        }
    }
}
