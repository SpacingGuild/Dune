

namespace Dune
{
    public class DuneDataController
    {
        private double techTier()
        {
            double techTier = 0;
            return techTier;
        }
        private double GetCoEfficiency()
        {
            return techTier() + 0;
        }

        public double GetCostSpice(Vessel vessel, double distance)
        {
            float mass = 0f;
            foreach (Part part in vessel.parts)
            {
                mass = mass + part.mass + part.GetResourceMass();
            }

            return mass * System.Math.Pow(1 + GetCoEfficiency(), distance);
        }
        public double DistanceDifficulty(double distance)
        {
            return distance*(185.75/100.0);
        }
    }
}