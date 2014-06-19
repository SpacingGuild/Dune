using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dune
{
    public class DuneSettingsWindow : DisplayModule
    {
        public DuneSettingsWindow(DuneCore core)
            : base(core)
        {
            priority = 1000;
            runModuleInScenes.Add(GameScenes.SPACECENTER);
            runModuleInScenes.Add(GameScenes.TRACKSTATION);
            runModuleInScenes.Add(GameScenes.EDITOR);
            runModuleInScenes.Add(GameScenes.FLIGHT);

            skinType = GUIDune.SkinType.Dune;
        }

        new public bool windowIsHidden = true;

        internal GUIDune.SkinType skinType;
        internal bool resetAborted = false;

        protected override void WindowGUI(int windowId)
        {
            GUILayout.BeginVertical();

            GUIDune.Title("Skins");
            GUIDune.Label("Current skin: ", skinType.ToString());

            if (GUIDune.skin == null || skinType != GUIDune.SkinType.Dune)
            {
                if (GUILayout.Button("Use Dune GUI skin"))
                {
                    GUIDune.LoadSkin(GUIDune.SkinType.Dune);
                    skinType = GUIDune.SkinType.Dune;
                }
            }

            if (GUIDune.skin == null || skinType != GUIDune.SkinType.Default)
            {
                if (GUILayout.Button("Use default GUI skin"))
                {
                    GUIDune.LoadSkin(GUIDune.SkinType.Default);
                    skinType = GUIDune.SkinType.Default;
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Reset all settings to default: ", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Reset"))
            {
                core.GetControlModule<SettingsDialog>().enabled = true;
                core.GetControlModule<SettingsDialog>().windowIsHidden = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            base.WindowGUI(windowId);
        }

        public override string GetName()
        {
            return "Settings Window";
        }
    }

    internal class SettingsDialog : DisplayModule
    {
        public SettingsDialog(DuneCore core)
            : base(core)
        {
            runModuleInScenes.Add(GameScenes.SPACECENTER);
            runModuleInScenes.Add(GameScenes.TRACKSTATION);
            runModuleInScenes.Add(GameScenes.EDITOR);
            runModuleInScenes.Add(GameScenes.FLIGHT);

            hideInToolbar = true;
            windowVector = new Vector4(Screen.width/2-100, Screen.height/2-30, 0, 0);
            windowVectorTrack = new Vector4(100, 200, 0, 0);
        }

        protected override void WindowGUI(int windowId)
        {
            GUILayout.BeginVertical();

            GUIDune.Title("Are you sure you want to reset to default settings?");

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Do not reset!", GUILayout.ExpandWidth(true)))
            {
                core.GetControlModule<DuneSettingsWindow>().resetAborted = true;
                windowIsHidden = true;
                enabled = false;
            }
            GUILayout.Space(20f);
            if (GUILayout.Button("Reset!", GUILayout.ExpandWidth(true)))
            {
                windowIsHidden = true;
                enabled = false;

                KSP.IO.FileInfo.CreateForType<DuneCore>("dune_settings_global.cfg").Delete();

                if (!FlightGlobals.ActiveVessel.IsNull())
                {
                    KSP.IO.FileInfo.CreateForType<DuneCore>("dune_settings_" + FlightGlobals.ActiveVessel.vesselName + ".cfg");
                }

                ScreenMessages.PostScreenMessage("Resetting settings and reloading modules!", 5.0f, ScreenMessageStyle.UPPER_CENTER);

                core.ReloadAllControlModules();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            if (GUI.Button(new Rect(windowPosition.width - 18, 2, 16, 16), ""))
            {
                core.GetControlModule<DuneSettingsWindow>().resetAborted = true;
                windowIsHidden = true;
                enabled = false;
            }
            GUI.DragWindow();
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(60) };
        }

        public override string GetName()
        {
            return "Confirm Reset!";
        }
    }
}
