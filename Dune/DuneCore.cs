﻿using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dune
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class DuneCore : ScenarioModule
    {
        // Reference point for all ControlModules.
        internal DuneDisplayControl displayControl;
        internal DuneDebrisControl debrisControl;
        internal DuneThrottleControl throttleControl;
        internal DuneDataControl dataControl;
        internal DuneNavigatorControl navigatorControl;
        internal DuneSettingsControl settingsControl;

        // Internal list of ControlModules.
        private List<ControlModule> controlModules = new List<ControlModule>();
        private List<ControlModule> controlModulesToLoad = new List<ControlModule>();
        private bool controlModulesUpdated = false;
        private static List<Type> moduleRegistry;

        // ConfigNode Gloabl and (hopefully) Local.
        internal ConfigNode partSettings;
        internal static float lastSettingsSaveTime;

        // Global settings.
        internal string difficulty = "normal";
        internal bool hardmode = false;
        internal bool IsCareer = HighLogic.CurrentGame.Mode == Game.Modes.CAREER;

        internal RenderingManager renderingManager = null;
        internal bool showGui = true;

        // 3rd Party plugin check;
        internal bool kethaneExists = Utilities.TryParse(AssemblyLoader.loadedAssemblies.Any(a => a.name == "Kethane").ToString(), false);
        internal bool kasExists = Utilities.TryParse(AssemblyLoader.loadedAssemblies.Any(a => a.name == "KAS").ToString(), false);
        internal bool mechjeb = Utilities.TryParse(AssemblyLoader.loadedAssemblies.Any(a => a.name == "MuMechLib").ToString(), false);
        internal bool exLaunchpadExists = Utilities.TryParse(AssemblyLoader.loadedAssemblies.Any(a => a.name == "Launchpad").ToString(), false);

        //TODO: Long-term Goal; Rearrange ControlModules, DisplayModules and PartModules. More control over where they exist.

        public void Start()
        {
            Debug.Log("[Dune] Core Start()");

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
                    Debug.LogError("[Dune] Core Start() Module: " + module.GetType().Name + " Exception: " + e);
                }
            }
        }

        public override void OnAwake()
        {
            MonoBehaviour.DontDestroyOnLoad(this);

            foreach (ControlModule module in controlModules)
            {
                try
                {
                    module.OnAwake();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] Core OnAwake() Module: " + module.GetType().Name + " Exception: " + e);
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
                    Debug.LogError("[Dune] Core FixedUpdate() Module: " + module.GetType().Name + " Exception: " + e);
                }
            }
        }

        public void Update()
        {
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
                    if (!(module is DisplayModule))
                    {
                        if (module.runModuleInScenes.Contains(HighLogic.LoadedScene))
                        {
                            module.enabled = true;
                        }
                        else
                        {
                            module.enabled = false;
                        }
                    }

                    if (module.enabled) module.OnUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] Core OnUpdate() Module: " + module.GetType().Name + " Exception: " + e);
                }
            }
        }

        private void LoadControlModules()
        {
            if (moduleRegistry == null)
            {
                moduleRegistry = new List<Type>();
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (var module in ass.GetTypes().Where(p => p.IsSubclassOf(typeof(ControlModule))).ToList())
                        {
                            Debug.LogWarning("[Dune] Core assembly loaded: " + module.FullName);
                            moduleRegistry.Add(module);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] Core LoadControlModules() moduleRegistry loading: " + ass.FullName + " Exception: " + e);
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
                Debug.LogError("[Dune] Core LoadControlModules() moduleRegistry: " + e);
            }

            displayControl = GetControlModule<DuneDisplayControl>();
            debrisControl = GetControlModule<DuneDebrisControl>();
            throttleControl = GetControlModule<DuneThrottleControl>();
            dataControl = GetControlModule<DuneDataControl>();
            navigatorControl = GetControlModule<DuneNavigatorControl>();
            settingsControl = GetControlModule<DuneSettingsControl>();
        }

        private void LoadDelayedModules()
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
            Debug.Log("[Dune] Core OnLoad()");

            if (GUIDune.skin == null)
            {
                GameObject guiLoader = new GameObject("GUILoader", typeof(GUILoader));
            }

            try
            {
                if (partSettings == null && sfsNode != null)
                {
                    partSettings = sfsNode;
                }

                LoadControlModules();

                ConfigNode configGlobal = new ConfigNode("DuneGlobalSettings");
                if (File.Exists<DuneCore>("dune_settings_global.cfg"))
                {
                    try
                    {
                        configGlobal = ConfigNode.Load(IOUtils.GetFilePathFor(this.GetType(), "dune_settings_global.cfg"));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] Core OnLoad() dune_settings_global.cfg: " + e);
                    }
                }

                //TODO: Change vesselName to vesselID and make it into 1 file, rather than 1 per vessel.
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
                        Debug.LogError("[Dune] Core OnLoad() dune_settings_type_" + vesselName + ".cfg: " + e);
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
                            module.OnSave(configGlobal.AddNode(module.GetType().Name), configVessel.AddNode(module.GetType().Name), configLocal.AddNode(module.GetType().Name));
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("[Dune] Core OnLoad() configLocal Module: " + module.GetType().Name + " Exception: " + e);
                        }
                    }
                }


                foreach (ControlModule module in controlModules)
                {
                    try
                    {
                        string name = module.GetType().Name;
                        ConfigNode moduleGlobal = configGlobal.HasNode(name) ? configGlobal.GetNode(name) : null;
                        ConfigNode moduleVessel = configVessel.HasNode(name) ? configVessel.GetNode(name) : null;
                        ConfigNode moduleLocal = configLocal.HasNode(name) ? configLocal.GetNode(name) : null;
                        module.OnLoad(moduleGlobal, moduleVessel, moduleLocal);
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
                Debug.LogError("[Dune] Core OnLoad(): " + e);
            }
        }

        public override void OnSave(ConfigNode sfsNode)
        {
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
                ConfigNode configVessel = new ConfigNode("DuneVesselSettings");
                ConfigNode configLocal = new ConfigNode("DuneLocalSettings");

                foreach (ControlModule module in controlModules)
                {
                    try
                    {
                        string name = module.GetType().Name;
                        module.OnSave(configGlobal.AddNode(name), configVessel.AddNode(name), configLocal.AddNode(name));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Dune] Core OnSave() module " + module.GetType().Name + " Exception: " + e);
                    }
                }

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

                try
                {
                    if (sfsNode != null) sfsNode.nodes.Add(configLocal);
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] Core OnSave() sfsnode/configLocal: " + e);
                }

                if (HighLogic.LoadedSceneIsFlight)
                {
                    string vesselName = FlightGlobals.ActiveVessel.vesselName;
                    vesselName = string.Join("_", vesselName.Split(System.IO.Path.GetInvalidFileNameChars())); // Remove illegal chars from the filename
                    configVessel.Save(IOUtils.GetFilePathFor(this.GetType(), "dune_settings_type_" + vesselName + ".cfg"));
                }

                if (true)
                {
                    configGlobal.Save(IOUtils.GetFilePathFor(this.GetType(), "dune_settings_global.cfg"));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Dune] Core OnSave() Exception: " + e);
            }

        }

        private void OnGUI()
        {
            if (!showGui) return;

            GUIDune.LoadSkin(GetControlModule<DuneSettingsWindow>().skinType);

            GUI.skin = GUIDune.skin;


            foreach (DisplayModule module in GetControlModules<DisplayModule>())
            {
                try
                {
                    if (module.enabled) module.DrawGUI();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] Core OnGUI() module " + module.GetName() + " Exception: " + e);
                }
            }
        }

        public void OnDestroy()
        {
            Debug.Log("[Dune] Core Destroy()");

            OnSave(null);

            foreach (ControlModule module in controlModules)
            {
                try
                {
                    module.OnDestroy();
                }
                catch (Exception e)
                {
                    Debug.LogError("[Dune] Core OnDestroy() module " + module.GetType().Name + " Exception: " + e);
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
    }
}
