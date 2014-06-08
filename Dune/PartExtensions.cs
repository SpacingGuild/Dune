using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dune
{
    public static class PartExtensions
    {
        public static bool HasModule<T>(this Part thisPart) where T : PartModule
        {
            return thisPart.Modules.OfType<T>().Count() > 0;
        }

        public static float TotalMass(this Part thisPart)
        {
            return thisPart.mass + thisPart.GetResourceMass();
        }

        public static bool IsEngine(this Part thisPart)
        {
            return (thisPart is SolidRocket ||
                thisPart is LiquidEngine ||
                thisPart is LiquidFuelEngine ||
                thisPart is AtmosphericEngine ||
                thisPart.HasModule<ModuleEngines>() ||
                thisPart.HasModule<ModuleEnginesFX>());
        }
    }
}
