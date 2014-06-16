using UnityEngine;

namespace Dune
{
    //TODO: Move this to persistent.cfg rather than plugin.
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class SettingsManager : MonoBehaviour
    {
        private static ConfigNode node;

        public static string GetValue(string key)
        {
            load();
            return node.GetValue(key);
        }

        public static void SetValue(string key, object value)
        {
            load();
            if (node.HasValue(key))
            {
                node.RemoveValue(key);
                node.AddValue(key, value);
            }
        }

        public static void Save()
        {
            load();
            Debug.Log("[Dune] Saving Settings " + HighLogic.LoadedScene);
            node.Save(settingsFile);
        }

        private static void load()
        {
            if (node != null) { return; }
            Debug.Log("[Dune] Loading Settings " + HighLogic.LoadedScene);
            node = ConfigNode.Load(settingsFile) ?? new ConfigNode();
        }

        private static string settingsFile
        {
            get { return KSPUtil.ApplicationRootPath + "GameData/SpacingGuild/Dune/settings.cfg"; }
        }

        public void Awake()
        {
            MonoBehaviour.DontDestroyOnLoad(this);
        }

        public void OnDestroy()
        {
            Debug.Log("[Dune] SettingsManager OnDestroy saveSettings");
            Save();
        }
    }
}
