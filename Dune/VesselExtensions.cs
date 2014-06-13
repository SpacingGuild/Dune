using System;
using System.Collections.Generic;
using System.Linq;
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

        public static float GetTotalMass(this Vessel thisVessel)
        {
            float mass = 0f;
            foreach (Part part in thisVessel.parts)
            {
                mass = mass + part.TotalMass();
            }

            return mass;
        }

        public static List<T> GetParts<T>(this Vessel thisVessel) where T : Part
        {
            if (HighLogic.LoadedSceneIsEditor) return EditorLogic.SortedShipList.OfType<T>().ToList();
            if (thisVessel == null) return new List<T>();
            return thisVessel.Parts.OfType<T>().ToList();
        }

        public static List<T> GetModules<T>(this Vessel thisVessel) where T : PartModule
        {
            List<Part> parts;
            if (HighLogic.LoadedSceneIsEditor) parts = EditorLogic.SortedShipList;
            else if (thisVessel == null) return new List<T>();
            else parts = thisVessel.Parts;

            return (from p in parts from module in p.Modules.OfType<T>() select module).ToList();
        }

        private static float lastFixedTime = 0;
        private static Dictionary<Guid, PartModule> masterObject = new Dictionary<Guid, PartModule>();

        public static PartModule GetMasterObject<T>(this Vessel thisVessel) where T : PartModule
        {
            if (thisVessel == null) return thisVessel.GetModules<T>().Max();

            if(lastFixedTime != Time.fixedTime)
            {
                masterObject = new Dictionary<Guid, PartModule>();
                lastFixedTime = Time.fixedTime;
            }

            if(!masterObject.ContainsKey(thisVessel.id))
            {
                T mo = thisVessel.GetModules<T>().Max();
                if (mo != null) masterObject.Add(thisVessel.id, mo);
                return mo;
            }

            return masterObject[thisVessel.id];
        }

        public static double TotalReourceMass(this Vessel thisVessel, string resourceName)
        {
            List<Part> parts = (HighLogic.LoadedSceneIsEditor ? EditorLogic.SortedShipList : thisVessel.parts);
            PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition(resourceName);

            if (definition == null) return 0;

            double amount = 0;
            foreach (Part part in parts)
            {
                foreach (PartResource partResource in part.Resources)
                {
                    if (partResource.info.id == definition.id) amount += partResource.amount;
                }
            }

            return amount * definition.density;
        }
    }
}
