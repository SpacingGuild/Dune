using System.Collections.Generic;

namespace Dune
{
    public class DuneDataControl : ControlModule
    {
        public DuneDataControl(DuneCore core) : base(core) { }

        public override void OnAwake()
        {

        }

        private double techTier()
        {
            List<TechLimit> _techLimits = new List<TechLimit>();

            double dtechTier = 0;

            if (HighLogic.CurrentGame == null || HighLogic.CurrentGame.Mode != Game.Modes.CAREER) return dtechTier;

            if (ResearchAndDevelopment.Instance == null)
            {
                return dtechTier;
            }

            foreach (TechLimit limit in _techLimits)
            {
                if (ResearchAndDevelopment.GetTechnologyState(limit.name) != RDTech.State.Available) continue;

                //if (limit.allowCurveTweaking)
                //    allowCurveTweaking = true;
            }
            return dtechTier;
        }

        private double GetCoEfficiency()
        {
            return techTier() + 0;
        }

        public double GetCostSpice(Vessel vessel, double distance)
        {
            return vessel.GetTotalMass() * System.Math.Pow(1 + GetCoEfficiency(), distance);
        }

        public double DistanceDifficulty(double distance)
        {
            return distance * (185.75 / 100.0);
        }
    }
    public sealed class TechLimit
    {
        public int id { get; set; }
        public string name { get; set; }
        public double value { get; set; }
    }
}