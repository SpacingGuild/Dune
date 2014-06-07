using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Dune
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DuneThrottleController : ScenarioModule
    {
        public void Start()
        {
            Debug.LogWarning("[Dune] Start ThrottleControl");
        }

        public void Update()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                MonitorThrottleControl();
            }
        }

        private void MonitorThrottleControl()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.X))
            {
                Debug.LogWarning("[Dune] FlightMaxThrottle");
                FlightGlobals.ActiveVessel.MaxThrottle();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.X))
            {
                Debug.LogWarning("[Dune] FlightMinThrottle");
                FlightGlobals.ActiveVessel.MinThrottle();
            }
        }
    }
}
