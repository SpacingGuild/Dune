using UnityEngine;

namespace Dune
{
    public class DuneThrottleControl : ControlModule
    {
        public DuneThrottleControl(DuneCore core) : base(core) { }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                ThrottleControl();
            }
        }

        private void ThrottleControl()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.X))
            {
                //Debug.LogWarning("[Dune] FlightMaxThrottle");
                FlightGlobals.ActiveVessel.MaxThrottle();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.X))
            {
                //Debug.LogWarning("[Dune] FlightMinThrottle");
                FlightGlobals.ActiveVessel.MinThrottle();
            }
        }
    }
}
