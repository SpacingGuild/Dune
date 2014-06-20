using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dune
{
    public class DuneDataControl : ControlModule
    {
        public DuneDataControl(DuneCore core)
            : base(core)
        {
            runModuleInScenes.Add(GameScenes.FLIGHT);
        }

        private Vessel vessel;
        public List<Resource> resourceList = new List<Resource>();

        public override void OnUpdate()
        {
            if (!FlightGlobals.ActiveVessel.IsNull())
            {
                vessel = FlightGlobals.ActiveVessel;
                LoadResourceList();            }
        }

        public int GetHoltzmanTechEfficiency()
        {
            if (core.IsCareer)
            {
                if (ResearchAndDevelopment.GetTechnologyState("start") == RDTech.State.Available)
                    return 5;

                if (ResearchAndDevelopment.GetTechnologyState("scienceTech") == RDTech.State.Available)
                    return 25;

                if (ResearchAndDevelopment.GetTechnologyState("fieldScience") == RDTech.State.Available)
                    return 50;

                if (ResearchAndDevelopment.GetTechnologyState("advScienceTech") == RDTech.State.Available)
                    return 75;

                if (ResearchAndDevelopment.GetTechnologyState("experimentalScience") == RDTech.State.Available)
                    return 100;
            }

            return 100;
        }

        private double GetSpacefolderEfficiency()
        {
            return 0;
        }

        public double GetCostSpice(double distance)
        {
            return vessel.GetTotalMass() * System.Math.Pow(1 + GetSpacefolderEfficiency() + GetHoltzmanTechEfficiency(), distance);
        }

        public double DistanceDifficulty(double distance)
        {
            return distance * (185.75 / 100.0);
        }

        private void LoadResourceList()
        {
            foreach (var r in vessel.GetActiveResources())
            {
                resourceList.Add(new Resource(r.GetType().Name, r.amount, r.maxAmount));
            }
        }
    }

    public class Resource
    {
        public string resourceName;
        public double amount;
        public double maxAmount;

        public Resource() { }

        public Resource(string resourceName, double amount, double maxAmount)
        {
            this.resourceName = resourceName;
            this.amount = amount;
            this.maxAmount = maxAmount;
        }
    }
}