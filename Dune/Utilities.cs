using UnityEngine;

namespace Dune
{
    internal static class Utilities
    {
        public static float TryParse(string s, float defaultValue)
        {
            float value;
            if (!float.TryParse(s, out value))
            {
                value = defaultValue;
            }

            return value;
        }

        public static bool TryParse(string s, bool defaultValue)
        {
            bool value;
            if(!bool.TryParse(s, out value))
            {
                value = defaultValue;
            }

            return value;
        }
    }

    //public static DuneThrottleController Current
    //{
    //    get
    //    {
    //        Debug.LogError("[Dune] CurrentPersistent"); 

    //        var game = HighLogic.CurrentGame;
    //        if (game == null) { return null; }

    //        if (!game.scenarios.Any(p => p.moduleName == typeof(DuneThrottleController).Name))
    //        {
    //            var proto = game.AddProtoScenarioModule(typeof(DuneThrottleController), GameScenes.FLIGHT);
    //            if (proto.targetScenes.Contains(HighLogic.LoadedScene))
    //            {
    //                proto.Load(ScenarioRunner.fetch);
    //            }
    //        }

    //        return game.scenarios.Select(s => s.moduleRef).OfType<DuneThrottleController>().SingleOrDefault();
    //    }
    //}
}
