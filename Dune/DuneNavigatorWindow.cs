using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dune
{
    public class DuneNavigatorWindow : DisplayModule
    {
        public DuneNavigatorWindow(DuneCore core)
            : base(core)
        {
            priority = 100;
            runModuleInScenes.Add(GameScenes.SPACECENTER);
            runModuleInScenes.Add(GameScenes.TRACKSTATION);
            runModuleInScenes.Add(GameScenes.EDITOR);
            runModuleInScenes.Add(GameScenes.FLIGHT);
        }

        private GUIContent[] menuContent;
        private GUIStyle listStyle = new GUIStyle();
        private int selectedItemIndex;
        private int selectedMenuIndex = 0;

        public override void OnStart()
        {
            //Init Menu
            menuContent = new GUIContent[5];
            menuContent[0] = new GUIContent("Overview", new Texture());
            menuContent[1] = new GUIContent("Navigator Controls", new Texture());
            menuContent[2] = new GUIContent("Vessel Information", new Texture());
            menuContent[3] = new GUIContent("Cargo Information", new Texture());
            menuContent[4] = new GUIContent("Plugin Information", new Texture());
        }

        protected override void WindowGUI(int windowId)
        {
            // TODO: Create main window.
            GUILayout.BeginVertical();

            GUIDune.Image(GUIDune.DuneLogo);

            // Menu Design
            selectedMenuIndex = GUILayout.SelectionGrid(selectedMenuIndex, menuContent, 5);
            // End Menu Design

            // Page content
            GUILayout.BeginVertical();

            #region Main Page
            if (selectedMenuIndex == 0)
            {
                GUIDune.Title("Overview");
                GUIDune.Label("", "Higher is better");
                GUIDune.Label("Holtzman Efficiency: ", (1 - core.dataControl.GetHoltzmanTechEfficiency()) * 100 + "%");
            }
            if (selectedMenuIndex == 1)
            {
                GUIDune.Title("Navigator Controls");

                // Show Navigator Controls if not activated.

                if (core.navigatorControl.initiateSpacefold)
                {
                    GUIDune.Title("Spacefold in progress!");
                }
                else
                {
                    selectedItemIndex = GUILayout.SelectionGrid(selectedItemIndex, core.navigatorControl.planetEntries(), 5);

                    if (GUILayout.Button("Activate Spacefold"))
                    {
                        core.navigatorControl.initiateSpacefold = true;
                    }
                }
            }
            if (selectedMenuIndex == 2)
            {
                GUIDune.Title("Vessel information!");

                // Show engine layout.
                GUIDune.Title("Engine Data");
                if (core.navigatorControl.settingsRetrieved)
                {
                    GUIDune.Label("Engine Name: ", core.navigatorControl.engineName);
                    GUIDune.Label("Engine Efficency: ", (1 - core.navigatorControl.engineEfficiency) * 100 + "%");
                    GUIDune.Label("Engine Failure: ", (1 - core.navigatorControl.engineFailure) * 100 + "%");
                }
            }
            if (selectedMenuIndex == 3)
            {
                GUIDune.Title("Cargo selected!");
            }
            if (selectedMenuIndex == 4)
            {
                GUIDune.Title("Plugin information!");
            }
            #endregion

            GUILayout.EndVertical();
            // End Page content

            GUILayout.BeginArea(new Rect(0, 480, 700, 20));
            GUIDune.Footer("Copyright © Spacing Guild 2014");
            GUILayout.EndArea();

            GUILayout.EndVertical();

            // TODO: Fix the window clickthrough.
            // http://answers.unity3d.com/questions/16774/preventing-mouse-clicks-from-passing-through-gui-c.html
            // http://forum.unity3d.com/threads/gui-window-resolving-flicker-preventing-click-thru-understanding-events.170647/
            //GUIUtility.hotControl = 0;
            base.WindowGUI(windowId);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(700), GUILayout.Height(500) };
        }

        public override string GetName()
        {
            return "Navigator Window";
        }
    }
}
