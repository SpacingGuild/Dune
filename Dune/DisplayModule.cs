﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dune
{
    public class DisplayModule : ControlModule
    {
        public bool windowIsHidden = false;

        [Persistent(pass = (int)Pass.configGlobal)]
        public bool showInFlight = true;

        [Persistent(pass = (int)Pass.configGlobal)]
        public bool showInTrack = false;
        public bool showInCurrentScene {get {return (HighLogic.LoadedSceneIsFlight ? showInFlight : showInTrack);}}

        [Persistent(pass = (int)Pass.configGlobal)]
        public Vector4 windowVector = new Vector4(10, 40, 0, 0);

        [Persistent(pass = (int)Pass.configGlobal)]
        public Vector4 windowVectorTrack = new Vector4(10, 40, 0, 0);
        
        public int Id;
        public static int nextId = 6451535;

        public Rect windowPosition
        {
            get
            {
                if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
                    return new Rect(windowVectorTrack.x, windowVectorTrack.y, windowVectorTrack.z, windowVectorTrack.w);
                else
                    return new Rect(windowVector.x, windowVector.y, windowVector.z, windowVector.w);
            }
            set
            {
                if(HighLogic.LoadedScene == GameScenes.TRACKSTATION)
                {
                    windowVectorTrack.x = Mathf.Clamp(windowVectorTrack.x, 10 - value.width, Screen.width - 10);
                    windowVectorTrack.y = Mathf.Clamp(windowVectorTrack.y, 10 - value.height, Screen.height - 10);
                }
                else
                {
                    windowVector.x = Mathf.Clamp(windowVector.x, 10 - value.width, Screen.width - 10);
                    windowVector.y = Mathf.Clamp(windowVector.y, 10 - value.height, Screen.height - 10);
                }
            }
        }

        public DisplayModule(DuneCore core) : base(core)
        {
            Id = nextId;
            nextId++;
        }

        public virtual GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[0];
        }

        protected virtual void WindowGUI(int windowId)
        {
            if(GUI.Button(new Rect(windowPosition.width - 18,2,16,16), ""))
            {
                enabled = false;
            }

            GUI.DragWindow();
        }

        public virtual void DrawGUI(bool inTrack)
        {
            if(showInCurrentScene)
            {
                windowPosition = GUILayout.Window(Id, windowPosition, WindowGUI, GetName(), WindowOptions());
            }
        }

        public override void OnSave(ConfigNode configGlobal, ConfigNode configTechTier, ConfigNode configVessel, ConfigNode configLocal)
        {
            base.OnSave(configGlobal, configTechTier, configVessel, configLocal);

            if (configGlobal != null) configGlobal.AddValue("enabled", enabled);
        }

        public override void OnLoad(ConfigNode configGlobal, ConfigNode configTechTier, ConfigNode configVessel, ConfigNode configLocal)
        {
            base.OnLoad(configGlobal, configTechTier, configVessel, configLocal);

            if(configGlobal != null && configGlobal.HasValue("enabled"))
            {
                bool loadedEnabled;
                if (bool.TryParse(configGlobal.GetValue("enabled"), out loadedEnabled)) enabled = loadedEnabled;
            }
        }

        public virtual string GetName()
        {
            return "Display Module";
        }
    }
}