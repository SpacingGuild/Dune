using System;
using System.Collections.Generic;
using System.Linq;
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

        private float lastSettingsRetrieved = 0;
        public bool SettingsRetrieved { get; private set; }
        public string engineName { get; private set; }
        public double engineEfficiency { get; private set; }
        public double engineFailure { get; private set; }

        public bool spacefoldInProgress = false;
        public bool spacefolderModuleExists = false;
        public bool navigatorModuleExists = false;

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
                    spacefolderModuleExists = true;
                }
                else
                {
                    spacefolderModules = null;
                    spacefolderModuleExists = false;
                }

                // Validate the presence of a navigatorModule.
                if (vessel.GetModules<DuneNavigatorModule>().Count > 0)
                {
                    navigatorModule = (DuneNavigatorModule)vessel.GetMasterObject<DuneNavigatorModule>();
                    navigatorModuleExists = true;
                }
                else
                {
                    navigatorModule = null;
                    navigatorModuleExists = false;
                }
            }

            // Collect spacefolderModule data every 5 seconds.
            if (Time.time > lastSettingsRetrieved + 5)
            {
                //TODO: Figure out how to handle multiple SpacefolderModules.
                lastSettingsRetrieved = Time.time;
                SettingsRetrieved = true;
                foreach (var r in spacefolderModules.OrderBy(p => p.engineEfficiency).Take(1))
                {
                    engineName = r.engineName;
                    engineEfficiency = r.engineEfficiency;
                    engineFailure = r.engineFailure;
                }
            }
            else
            {
                SettingsRetrieved = false;
            }
        }

        public SpacefoldState PreliminarySpacefoldProcedure(bool activate, int targetBodyId)
        {
            Debug.Log("[Dune] NavigatorControl PreliminarySpacefoldProcedure() Started!" + activate + " " + targetBodyId);
            GUIContent[] tempList = planetEntries();

            if(!spacefolderModuleExists)
            {
                return SpacefoldState.NO_SPACEFOLDER;
            }
            if(!navigatorModuleExists)
            {
                return SpacefoldState.NO_NAVIGATOR;
            }

            return setOrbit(FlightGlobals.Bodies.FirstOrDefault(p => p.name == tempList[targetBodyId].text));
        }

        public enum SpacefoldState { SUCCESS, NO_FUEL, ABOVE_SOI, FAILURE, NOT_IN_ORBIT, NO_SPACEFOLDER, NO_NAVIGATOR };

        public GUIContent[] planetEntries()
        {
            List<CelestialBody> planets = FlightGlobals.Bodies;
            CelestialBody theSun = planets.FirstOrDefault(p => p.name == "Sun");
            GUIContent[] planetEntries = new GUIContent[planets.Count(p => p.referenceBody == theSun && p != theSun)];

            int count = 0;
            foreach (var r in planets)
            {
                if (r.referenceBody == theSun && r != theSun)
                {
                    planetEntries[count] = new GUIContent(r.name, new Texture());
                    count++;
                }
            }

            return planetEntries;
        }

        public SpacefoldState setOrbit(CelestialBody targetBody)
        {
            // Current orbit
            Orbit currentOrbit = vessel.orbit;

            // Tech Efficiency
            int techEfficiency = core.dataControl.GetHoltzmanTechEfficiency();

            System.Random rand = new System.Random();
            Orbit newOrbit = new Orbit();
            // New values
            newOrbit.inclination = Convert.ToDouble(rand.Next((int)currentOrbit.inclination, (int)20));
            newOrbit.eccentricity = Convert.ToDouble(rand.Next((int)currentOrbit.eccentricity, (int)10));
            newOrbit.semiMajorAxis = targetBody.Radius + (targetBody.sphereOfInfluence / techEfficiency);
            newOrbit.LAN = 90;
            newOrbit.argumentOfPeriapsis = 90;
            newOrbit.meanAnomalyAtEpoch = 0;
            newOrbit.epoch = 0;
            newOrbit.referenceBody = targetBody;

            Debug.Log("inclination: " + newOrbit.inclination);
            Debug.Log("eccentricity: " + newOrbit.eccentricity);
            Debug.Log("semiMajorAxis: " + newOrbit.semiMajorAxis);


            if (newOrbit.getRelativePositionAtUT(Planetarium.GetUniversalTime()).magnitude > newOrbit.referenceBody.sphereOfInfluence)
            {
                Debug.LogError("Destination position was above the sphere of influence");
                return SpacefoldState.ABOVE_SOI;
            }

            vessel.Landed = false;
            vessel.Splashed = false;
            vessel.landedAt = string.Empty;

            try
            {
                OrbitPhysicsManager.HoldVesselUnpack(60);
            }
            catch (Exception e)
            {
                Debug.LogError("[Dune] NavigatorControl setOrbit() exception on physicsManager: " + e);
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

            spacefoldInProgress = false;
            return SpacefoldState.SUCCESS;
        }

        //public void CalculateHoltzmanEffectEngine()
        //{
        //    //TODO: Calculate new ISP and Thrust depending on ship total mass.
        //}
    }
}
