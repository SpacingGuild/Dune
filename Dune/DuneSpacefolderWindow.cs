using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dune
{
    public class DuneSpacefolderWindow : DisplayModule
    {
        public DuneSpacefolderWindow(DuneCore core)
            : base(core)
        {
            priority = 100;
            runModuleInScenes.Add(GameScenes.FLIGHT);
        }


        public override string GetName()
        {
            return "Engine Data";
        }

        protected override void WindowGUI(int windowId)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleRight;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            if (core.spacefolderControl.settingsRetrieved)
            {
                GUILayout.Label("Engine Name: ", GUILayout.ExpandWidth(true));
                GUILayout.Label(core.spacefolderControl.engineName, style);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Label("Engine Efficiency: ", GUILayout.ExpandWidth(true));
                GUILayout.Label(((1 - core.spacefolderControl.engineEfficiency) * 100).ToString() + "%", style);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Label("Engine Failure: ", GUILayout.ExpandWidth(true));
                GUILayout.Label(((1 - core.spacefolderControl.engineFailure) * 100).ToString() + "%", style);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Label("Tech Efficiency: ", GUILayout.ExpandWidth(true));
                GUILayout.Label(((1 - core.dataControl.GetHoltzmanTechEfficiency()) * 100).ToString() + "%", style);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            base.WindowGUI(windowId);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(250), GUILayout.Height(50) };
        }
    }
}
