using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dune
{
    public class DuneNavigatorControl : ControlModule
    {
        public DuneNavigatorControl(DuneCore core)
            : base(core)
        {
            runModuleInScenes.Add(GameScenes.FLIGHT);
        }

        private List<DuneSpacefolderModule> spacefolderModules;
        private DuneNavigatorModule navigatorModule;

        private Vessel vessel;

        public bool settingsRetrieved { get; private set; }
        public string engineName { get; private set; }
        public double engineEfficiency { get; private set; }
        public double engineFailure { get; private set; }

        public bool initiateSpacefold = false;
        private bool spacefoldInProgress = false;

        //Variables for timing the messages, graphics and spacefold.
        private float lastFoldMessageSent = 0;
        private float messageCount = 0;

        public bool stop = false;

        public override void OnUpdate()
        {
            if (!FlightGlobals.ActiveVessel.IsNull())
            {
                vessel = FlightGlobals.ActiveVessel;
            }

            // Validate the presence of modules.
            if (!vessel.IsNull() && !stop)
            {
                // Validate the presence of a spacefolderModule.
                if (vessel.GetModules<DuneSpacefolderModule>().Count > 0)
                {
                    spacefolderModules = vessel.GetModules<DuneSpacefolderModule>();
                }
                else
                {
                    spacefolderModules = null;
                }

                // Validate the presence of a navigatorModule.
                if (vessel.GetModules<DuneNavigatorModule>().Count > 0)
                {
                    navigatorModule = (DuneNavigatorModule)vessel.GetMasterObject<DuneNavigatorModule>();
                }
                else
                {
                    navigatorModule = null;
                }
            }

            // Collect engineData if spacefolderModule is not null.
            if (!spacefolderModules.IsNull())
            {
                //TODO: Figure out how to handle multiple SpacefolderModules.
                settingsRetrieved = true;
                foreach (var r in spacefolderModules.OrderBy(p => p.engineEfficiency).Take(1))
                {
                    engineName = r.engineName;
                    engineEfficiency = r.engineEfficiency;
                    engineFailure = r.engineFailure;
                }
            }
            else
            {
                settingsRetrieved = false;
            }

            // Run checks against the navigatorModule for changes. Stop when initiatingSpacefold == true.
            if (!navigatorModule.IsNull() && !initiateSpacefold)
            {
                initiateSpacefold = navigatorModule.initiateSpacefold;
            }

            // Stop checking for updates and start the spacefold procedure.
            if (initiateSpacefold && !spacefoldInProgress)
            {
                navigatorModule.initiateSpacefold = initiateSpacefold;

                //if (Time.time > lastFoldMessageSent + 1)
                //{
                //    lastFoldMessageSent = Time.time;
                //    ScreenMessages.PostScreenMessage("Initiating Spacefold in T MINUS: " + (messageCount - 5) * -1, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                //    messageCount = messageCount + 1;
                //    if (messageCount == 5)
                //    {
                        spacefoldInProgress = true;

                        // Start the preliminary spacefold procedure.
                        PreliminarySpacefoldProcedure();
                //    }
                //}
            }
        }

        public void PreliminarySpacefoldProcedure()
        {
            Debug.Log("[Dune] NavigatorControl PreliminarySpacefoldProcedure() Started!");

            // TODO: Setup dynamic orbit.
        }

        public GUIContent[] planetEntries()
        {
            List<CelestialBody> planets = FlightGlobals.Bodies;
            CelestialBody theSun = planets.FirstOrDefault(p => p.name == "Sun");
            GUIContent[] planetEntries = new GUIContent[planets.Count(p => p.referenceBody == theSun)];

            int count = 0;
            foreach(var r in planets)
            {
                if (r.referenceBody == theSun)
                {
                    planetEntries[count] = new GUIContent(r.name, new Texture());
                    count++;
                }
            }

            return planetEntries;
        }

        public void setOrbit(CelestialBody targetBody)
        {
            CelestialBody body = targetBody;

            // New values
            Orbit newOrbit = new Orbit();
            newOrbit.inclination = 0;
            newOrbit.eccentricity = 0;
            newOrbit.semiMajorAxis = (body.Radius + body.sphereOfInfluence)/2;
            newOrbit.LAN = 90;
            newOrbit.argumentOfPeriapsis = 90;
            newOrbit.meanAnomalyAtEpoch = 0;
            newOrbit.epoch = 0;
            newOrbit.referenceBody = body;


            if (newOrbit.getRelativePositionAtUT(Planetarium.GetUniversalTime()).magnitude > newOrbit.referenceBody.sphereOfInfluence)
            {
                Debug.LogError("Destination position was above the sphere of influence");
                return;
            }

            vessel.Landed = false;
            vessel.Splashed = false;
            vessel.landedAt = string.Empty;

            try
            {
                OrbitPhysicsManager.HoldVesselUnpack(60);
            }
            catch (NullReferenceException)
            {
            }

            foreach (var v in (FlightGlobals.fetch == null ? (IEnumerable<Vessel>)new[] { vessel } : FlightGlobals.Vessels).Where(v => v.packed == false))
                v.GoOnRails();

            // Set new values.
            vessel.orbit.inclination = newOrbit.inclination;
            vessel.orbit.eccentricity = newOrbit.eccentricity;
            vessel.orbit.semiMajorAxis = newOrbit.semiMajorAxis;
            vessel.orbit.LAN = newOrbit.LAN;
            vessel.orbit.argumentOfPeriapsis = newOrbit.argumentOfPeriapsis;
            vessel.orbit.meanAnomalyAtEpoch = newOrbit.meanAnomalyAtEpoch;
            vessel.orbit.epoch = newOrbit.epoch;
            vessel.orbit.referenceBody = newOrbit.referenceBody;
            vessel.orbit.Init();
            vessel.orbit.UpdateFromUT(Planetarium.GetUniversalTime());

            vessel.orbitDriver.pos = vessel.orbit.pos.xzy;
            vessel.orbitDriver.vel = vessel.orbit.vel;
        }

        //public void CalculateHoltzmanEffectEngine()
        //{
        //    //TODO: Calculate new ISP and Thrust depending on ship total mass.
        //}
    }
}
