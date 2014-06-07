using UnityEngine;

namespace Dune
{
    public static class VesselExtensions
    {
        public static void MaxThrottle(this Vessel thisVessel)
        {
            thisVessel.ctrlState.mainThrottle = 1;
            if (thisVessel == FlightGlobals.ActiveVessel)
            {
                FlightInputHandler.state.mainThrottle = 1;
            }
        }

        public static void MinThrottle(this Vessel thisVessel)
        {
            thisVessel.ctrlState.mainThrottle = 0;
            if (thisVessel == FlightGlobals.ActiveVessel)
            {
                FlightInputHandler.state.mainThrottle = 0;
            }
        }
    }
}
