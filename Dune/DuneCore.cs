using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.IO;
using System.Reflection;

namespace Dune
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class DuneCore : ScenarioModule, IComparable<DuneCore>
    {
        //Dune Controllers
        public DuneMenuControl _duneMenuControl;
        public DuneTrackingControl _duneTrackingControl;
        public DuneThrottleControl _duneThrottleControl;

        private List<ControlModule> controlModules = new List<ControlModule>();
        private List<ControlModule> controlModulesToLoad = new List<ControlModule>();
        private bool controlModulesUpdated = false;

        private static List<Type> moduleRegistry;

        public ConfigNode partSettings;
        public static float lastSettingsSaveTime;
        public static bool IsCareer;

        public RenderingManager renderingManager = null;
        public GUIStyle _windowStyle, _labelStyle;
        public bool showGui = true;



        public void Start()
        {
            //COMMENT: Monitor Core Start()
            Debug.Log("[Dune] Core Start()");
            IsCareer = (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) ? true : false;

            if (controlModules.Count == 0)
            {
                OnLoad(null);
            }

            lastSettingsSaveTime = Time.time;

            foreach (ControlModule module in controlModules)
            {
                try
                {
                    module.OnStart();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] module" + module.GetType().Name + " threw an exception in OnStart:" + e);
                }
            }
        }

        public void OnActive()
        {
            foreach (ControlModule module in controlModules)
            {
                try
                {
                    module.OnActive();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in OnActive:" + e);
                }
            }
        }

        public void OnInactive()
        {
            foreach (ControlModule module in controlModules)
            {
                try
                {
                    module.OnInactive();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in OnInactive:" + e);
                }
            }
        }

        public override void OnAwake()
        {
            MonoBehaviour.DontDestroyOnLoad(this);

            //COMMENT: Monitor Core OnAwake()
            Debug.Log("[Dune] Core OnAwake()");

            if (HighLogic.LoadedScene == GameScenes.LOADING || HighLogic.LoadedScene == GameScenes.MAINMENU) return;

            foreach (ControlModule module in controlModules)
            {
                try
                {
                    module.OnAwake();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in OnAwake:" + e);
                }
            }
        }

        public void FixedUpdate()
        {
            LoadDelayedModules();

            foreach (ControlModule module in controlModules)
            {
                try
                {
                    if (module.enabled) module.OnFixedUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in FixedUpdate:" + e);
                }
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedScene != GameScenes.TRACKSTATION || HighLogic.LoadedSceneIsFlight) return;

            if (renderingManager == null)
            {
                renderingManager = (RenderingManager)GameObject.FindObjectOfType(typeof(RenderingManager));
            }

            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene == GameScenes.TRACKSTATION && renderingManager != null)
            {
                if (renderingManager.uiElementsToDisable.Length >= 1) showGui = renderingManager.uiElementsToDisable[0].activeSelf;
            }

            if (controlModulesUpdated)
            {
                controlModules.Sort();
                controlModulesUpdated = false;
            }

            if (HighLogic.LoadedScene == GameScenes.TRACKSTATION || HighLogic.LoadedSceneIsFlight)
            {
                if (Time.time > lastSettingsSaveTime + 5)
                {
                    OnSave(null);
                    lastSettingsSaveTime = Time.time;
                }
            }

            foreach (ControlModule module in controlModules)
            {
                try
                {
                    if (module.enabled) module.OnUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in OnUpdate:" + e);
                }
            }
        }

        void LoadControlModules()
        {
            if (moduleRegistry == null)
            {
                moduleRegistry = new List<Type>();
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (var module in ass.GetTypes().Where(p => p.IsSubclassOf(typeof(ControlModule))).ToList()) //(from t in ass.GetTypes() where t.IsSubclassOf(typeof(ControlModule)) select t).ToList())
                        {
                            //COMMENT: Monitor in log which Assemblies gets loaded.
                            Debug.LogWarning("[Dune] Core Module loaded: " + module.FullName);
                            moduleRegistry.Add(module);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] moduleRegistry creation threw an exception in LoadControlModules loading " + ass.FullName + ": " + e);
                    }
                }
            }

            try
            {
                foreach (Type t in moduleRegistry)
                {
                    if (t != typeof(ControlModule) && (t != typeof(DisplayModule)) && (GetControlModule(t.Name) == null))
                        AddControlModule((ControlModule)(t.GetConstructor(new Type[] { typeof(DuneCore) }).Invoke(new object[] { this })));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Dune] moduleRegistry loading threw an exception in LoadControlModules: " + e);
            }

            //TODO: Add new controlModules here.
            _duneMenuControl = GetControlModule<DuneMenuControl>();
            _duneTrackingControl = GetControlModule<DuneTrackingControl>();
            _duneThrottleControl = GetControlModule<DuneThrottleControl>();
        }

        void LoadDelayedModules()
        {
            if (controlModulesToLoad.Count > 0)
            {
                controlModules.AddRange(controlModulesToLoad);
                controlModulesUpdated = true;
                controlModulesToLoad.Clear();
            }
        }

        public override void OnLoad(ConfigNode sfsNode)
        {
            //COMMENT: Monitor Core OnLoad()
            Debug.Log("[Dune] Core OnLoad()");

            if (false) //(GUIsomething.skin == null)
            {
                //GameObject something = new GameObject("somethingGUILoader", typeof(SomethingGUILoader));
            }

            try
            {
                if (partSettings == null && sfsNode != null)
                {
                    partSettings = sfsNode;
                }

                LoadControlModules();

                ConfigNode configGlobal = new ConfigNode("DuneGlobalSettings");
                if (File.Exists<DuneCore>("dune_global_settings.cfg"))
                {
                    try
                    {
                        configGlobal = ConfigNode.Load(IOUtils.GetFilePathFor(this.GetType(), "dune_global_settings.cfg"));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] global exception when trying to load dune_global_settings.cfg: " + e);
                    }
                }

                ConfigNode configTechTier = new ConfigNode("DuneTechTierSettings");
                if (File.Exists<DuneCore>("dune_techtier_settings.cfg"))
                {
                    try
                    {
                        configTechTier = ConfigNode.Load(IOUtils.GetFilePathFor(this.GetType(), "dune_techtier_settings.cfg"));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] techTier exception when trying to load dune_techtier_settings.cfg: " + e);
                    }
                }

                ConfigNode configVessel = new ConfigNode("DuneVesselSettings");
                String vesselName = FlightGlobals.ActiveVessel != null ? string.Join("_", FlightGlobals.ActiveVessel.vesselName.Split(System.IO.Path.GetInvalidFileNameChars())) : ""; //Remove illegal chars from filename.
                if ((FlightGlobals.ActiveVessel != null) && File.Exists<DuneCore>("dune_settings_type_" + vesselName + ".cfg"))
                {
                    try
                    {
                        configVessel = ConfigNode.Load(IOUtils.GetFilePathFor(this.GetType(), "dune_settings_type_" + vesselName + ".cfg"));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune.Onload] caught an exception trying to load dune_settings_type_" + vesselName + ".cfg: " + e);
                    }
                }

                ConfigNode configLocal = new ConfigNode("DuneLocalSettings");
                if (sfsNode != null && sfsNode.HasNode("DuneLocalSettings"))
                {
                    configLocal = sfsNode.GetNode("DuneLocalSettings");
                }
                else if (partSettings != null && partSettings.HasNode("DuneLocalSettings"))
                {
                    configLocal = partSettings.GetNode("DuneLocalSettings");
                }
                else if (sfsNode == null)
                {
                    foreach (ControlModule module in controlModules)
                    {
                        try
                        {
                            module.OnSave(null, null, null, configLocal.AddNode(module.GetType().Name));
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in OnLoad: " + e);
                        }
                    }
                }


                foreach (ControlModule module in controlModules)
                {
                    try
                    {
                        string name = module.GetType().Name;
                        ConfigNode moduleGlobal = configGlobal.HasNode(name) ? configGlobal.GetNode(name) : null;
                        ConfigNode moduleTechTier = configTechTier.HasNode(name) ? configTechTier.GetNode(name) : null;
                        ConfigNode moduleVessel = configVessel.HasNode(name) ? configVessel.GetNode(name) : null;
                        ConfigNode moduleLocal = configLocal.HasNode(name) ? configLocal.GetNode(name) : null;
                        module.OnLoad(moduleGlobal, moduleTechTier, moduleVessel, moduleLocal);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in OnLoad: " + e);
                    }
                }

                LoadDelayedModules();
            }
            catch (Exception e)
            {
                Debug.LogError("[Dune] caught exception in core OnLoad: " + e);
            }
        }

        public override void OnSave(ConfigNode sfsNode)
        {
            //COMMENT: Monitor Core OnSave()
            Debug.Log("[Dune] Core OnSave()");

            // Dont save if not in flight or tracking station
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER) return;
            // Dont save empty settings
            if (controlModules.Count == 0) return;
            // Dont save when undocking
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel.vesselName == null) return;

            try
            {
                //Add delayedModules so they get saved
                LoadDelayedModules();

                ConfigNode configGlobal = new ConfigNode("DuneGlobalSettings");
                ConfigNode configTechTier = new ConfigNode("DuneTechTierSettings");
                ConfigNode configVessel = new ConfigNode("DuneVesselSettings");
                ConfigNode configLocal = new ConfigNode("DuneLocalSettings");

                foreach (ControlModule module in controlModules)
                {
                    try
                    {
                        string name = module.GetType().Name;
                        module.OnSave(configGlobal.AddNode(name), configTechTier.AddNode(name), configVessel.AddNode(name), configLocal.AddNode(name));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in OnSave: " + e);
                    }
                }

                //COMMENT: Monitor Core OnSave() configNode values
                /*
                Debug.Log("[Dune] Core OnSave() configNode values:");
                Debug.Log("configGlobal:");
                Debug.Log(configGlobal.ToString());
                Debug.Log("configTechTier:");
                Debug.Log(configTechTier.ToString());
                Debug.Log("configVessel:");
                Debug.Log(configVessel.ToString());
                Debug.Log("configLocal:");
                Debug.Log(configLocal.ToString());//*/

                //Debug.Log("[Dune] Core OnSave() 1 Called by: " + new System.Diagnostics.StackFrame(1).GetMethod().Name);

                //TODO: Fix sfsnode NRE
                /*
                try
                {
                    Debug.Log("SfsNode:");
                    Debug.Log(sfsNode.ToString());
                    if (sfsNode != null) sfsNode.nodes.Add(configLocal);
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] Core OnSave() sfsnode/configLocal: " + e);
                }*/

                if (HighLogic.LoadedSceneIsFlight)
                {
                    string vesselName = FlightGlobals.ActiveVessel.vesselName;
                    vesselName = string.Join("_", vesselName.Split(System.IO.Path.GetInvalidFileNameChars())); // Remove illegal chars from the filename
                    configVessel.Save(IOUtils.GetFilePathFor(this.GetType(), "dune_settings_type_" + vesselName + ".cfg"));
                }

                if (true)
                {
                    configGlobal.Save(IOUtils.GetFilePathFor(this.GetType(), "dune_settings_global.cfg"));
                    configTechTier.Save(IOUtils.GetFilePathFor(this.GetType(), "dune_settings_techtier.cfg"));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Dune] caught an exception in core OnSave: " + e);
            }

        }

        public void OnDestroy()
        {
            //COMMENT: Monitor Core OnDestroy()
            Debug.LogWarning("[Dune] Core OnDestroy()");
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene == GameScenes.TRACKSTATION || FlightGlobals.ActiveVessel.isActiveVessel)
            {
                OnSave(null);
            }

            foreach (ControlModule module in controlModules)
            {
                try
                {
                    module.OnDestroy();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] module " + module.GetType().Name + " threw an exception in OnDestroy " + e);
                }
            }
        }

        private void OnGUI()
        {
            if (!showGui) return;

            //Check GUI init

            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene == GameScenes.TRACKSTATION)
            {
                foreach (DisplayModule module in GetControlModules<DisplayModule>())
                {
                    try
                    {
                        if (module.enabled) module.DrawGUI(HighLogic.LoadedScene == GameScenes.TRACKSTATION);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] Core OnGUI: module " + module.GetType().Name + " threw an exception in DrawGUI: " + e);
                    }
                }
            }
        }

        public T GetControlModule<T>() where T : ControlModule
        {
            return (T)controlModules.FirstOrDefault(p => p is T); // Null if no matches
        }

        public List<T> GetControlModules<T>() where T : ControlModule
        {
            return controlModules.FindAll(p => p is T).Cast<T>().ToList();
        }

        public ControlModule GetControlModule(string type)
        {
            return controlModules.FirstOrDefault(p => p.GetType().Name.ToLowerInvariant() == type.ToLowerInvariant()); // Null if no matches
        }

        public void AddControlModule(ControlModule control)
        {
            controlModules.Add(control);
            controlModulesUpdated = true;
        }

        public void AddControlModuleListener(ControlModule module)
        {
            controlModulesToLoad.Add(module);
        }

        public void RemoveControlModule(ControlModule module)
        {
            controlModules.Remove(module);
            controlModulesUpdated = true;
        }

        public void ReloadAllControlModules()
        {
            foreach (ControlModule module in controlModules) module.OnDestroy();
            controlModules.Clear();

            OnLoad(null);
            Start();
        }

        public int priority = 0;

        public int CompareTo(DuneCore other)
        {
            if (other == null) return 1;
            return priority.CompareTo(other.priority);
        }
    }
}
