using System.Collections.Generic;
using UnityEngine;

namespace Dune
{
    public static class PartExtensions
    {
        public static bool IsPrimary(this Part thisPart, List<Part> partsList, int modeuleClassId)
        {
            foreach (Part part in partsList)
            {
                if (part.Modules.Contains(modeuleClassId))
                {
                    if (part == thisPart)
                        return true;
                    else
                        break;
                }
            }

            return false;
        }
    }
}
