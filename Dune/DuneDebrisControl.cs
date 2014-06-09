using UnityEngine;

namespace Dune
{
    public class DuneDebrisControl : ControlModule
    {
        public DuneDebrisControl(DuneCore core) : base(core) { priority = 500; }

        [KSPField(isPersistant = true),Persistent(pass = (int)Pass.configGlobal)]
        public bool autoRemoveAll = false;

        [Persistent(pass = (int)Pass.configGlobal)]
        public float timeBetweenRemoves = 30;
        private float lastRemove;

        public override void OnFixedUpdate()
        {
            if (autoRemoveAll && (lastRemove + timeBetweenRemoves < Time.time))
            {
                lastRemove = Time.time;

                Debug.Log("[Dune] DebrisControl autoRemove Time: "+ Time.time);
                RemoveAll();
            }
        }

        public void RemoveAll()
        {
            int count = 0;
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.vesselType == VesselType.Debris)
                {
                    try
                    {
                        count = count + 1;

                        foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew())
                        {
                            crewMember.rosterStatus = ProtoCrewMember.RosterStatus.MISSING;
                            crewMember.Die();
                            Debug.LogWarning("[Dune] DebrisControl crewMember: " + crewMember.name + " was reported missing!");
                        }

                        FlightGlobals.Vessels.Remove(vessel);
                        vessel.Die();

                        Debug.Log("[Dune] DebrisControl Vessel ID: " + vessel.id + " was destroyed!");
                        //InvalidOperationException: Collection was modified; enumeration operation may not execute.
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("[Dune] DebrisControl Vessel ID" + vessel.id + "couldn't be destroyed: " + e);
                    }
                }
            }
            Debug.Log("[Dune] DebrisControl RemovedCount: " + count);
            
            if (count > 0)
            {
                Debug.Log("[Dune] DebrisControl Reload tracking station");
                HighLogic.LoadScene(GameScenes.TRACKSTATION);
                ScreenMessages.PostScreenMessage("Removed: " + count+ " debris...", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            else
            {
                Debug.Log("[Dune] DebrisControl Reload not needed");
                ScreenMessages.PostScreenMessage("No debris found!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
        }
    }
}
