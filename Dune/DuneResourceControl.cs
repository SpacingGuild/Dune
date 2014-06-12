using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dune
{
    public class DuneResourceControl : ControlModule
    {
        public DuneResourceControl(DuneCore core) : base(core)
        {
            runModuleInScenes.Add(GameScenes.FLIGHT);
        }

        //COMMENT: Would it be better to have a list of ships and store all the resource totals ?
        private Vessel vessel;
        public List<Resource> resourceList;

        public override void OnStart()
        {
            resourceList = new List<Resource>();
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                vessel = FlightGlobals.ActiveVessel;
                LoadResourceList();
            }
        }

        private void LoadResourceList()
        {
            foreach (Part part in vessel.parts)
            {
                foreach (PartResource res in part.Resources)
                {
                    if (resourceList.Exists(p => p.resourceName == res.resourceName))
                    {
                        foreach(Resource r in resourceList)
                        {
                            r.amount = r.amount + res.amount;
                            r.maxAmount = r.maxAmount + res.maxAmount;
                        }
                    }
                    else
                    {
                        resourceList.Add(new Resource(res.resourceName, res.amount, res.maxAmount));
                    }
                }
            }
        }
    }
    //TODO: Is there a better way of doing this ?
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
