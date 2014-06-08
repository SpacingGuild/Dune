using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using KSP.IO;

namespace Dune
{
    public class DuneTrackingControl : ControlModule
    {
        public DuneTrackingControl(DuneCore core) : base(core) { priority = 500; }

        public bool _autoDekessle = false;

        public override void OnAwake()
        {
            //COMMENT: Monitor DuneTrackingControl OnAwake()
            Debug.Log("[Dune] DuneTrackingControl OnAwake()");
        }

        public override void OnFixedUpdate()
        {
            if (_autoDekessle)
            {
                // Add auto dekessler
                //Debug.LogWarning("[Dune] DekesslerAuto");
            }
        }

        public void Dekessle()
        {
            int count = 0;
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.vesselType == VesselType.Debris)
                {
                    try
                    {
                        count = count + 1;
                        FlightGlobals.Vessels.Remove(vessel);
                        vessel.Die();

                        foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew())
                        {
                            crewMember.rosterStatus = ProtoCrewMember.RosterStatus.MISSING;
                            crewMember.Die();
                            Debug.LogWarning("[Dune] crewMember: " + crewMember.name + " was reported missing!");
                        }

                        Debug.LogWarning("[Dune] Vessel ID: " + vessel.id + " was destroyed!");
                    }
                    catch (System.Exception e)
                    {
                        //InvalidOperationException: Collection was modified; enumeration operation may not execute.
                        Debug.LogError("[Dune] Vessel ID" + vessel.id + "couldn't be destroyed: " + e);
                    }
                }
            }
            if (count > 0)
            {
                Debug.LogWarning("[Dune] Reload tracking station");
                HighLogic.LoadScene(GameScenes.TRACKSTATION);
                ScreenMessages.PostScreenMessage("Dekessled: " + count, 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            else
            {
                Debug.LogWarning("[Dune] Reload postponed");
                ScreenMessages.PostScreenMessage("No debris found!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
        }
    }
}
