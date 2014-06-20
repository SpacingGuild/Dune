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

        new public bool windowIsHidden = true;

        private GUIContent[] menuContent;
        private GUIStyle listStyle = new GUIStyle();
        private int selectedPlanetIndex;
        private int selectedMenuIndex = 0;
        private Vector2 scrollPosition = Vector2.zero;

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
                GUIDune.Label("Holtzman Efficiency: ", (core.dataControl.GetHoltzmanTechEfficiency()) + "%");
            }
            if (selectedMenuIndex == 1)
            {
                GUIDune.Title("Navigator Controls");

                // Show Navigator Controls if not activated.

                if (core.navigatorControl.spacefoldInProgress)
                {
                    GUIDune.Title("Spacefold in progress!");
                }
                else
                {
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                    GUILayout.BeginHorizontal();
                    selectedPlanetIndex = GUILayout.SelectionGrid(selectedPlanetIndex, core.navigatorControl.planetEntries(), 1);

                    GUILayout.BeginVertical();
                    GUIDune.Label("some text", "some value");
                    GUIDune.Label("some text", "some value");
                    GUIDune.Label("some text", "some value");
                    GUIDune.Label("some text", "some value");
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();
                    if (!HighLogic.LoadedSceneIsFlight)
                    {
                        GUIDune.Warning("Warning: You can't initiate a spacefold, when you are not in a vessel!");
                    }
                    else if (!core.navigatorControl.spacefolderModuleExists)
                    {
                        GUIDune.Warning("Warning: You can't initiate a spacefold, when your vessel does not contain a Spacefolder!");
                    }
                    else if (!core.navigatorControl.navigatorModuleExists)
                    {
                        GUIDune.Warning("Warning: You can't initiate a spacefold, when your vessel does not contain a Navigator Core!");
                    }
                    else if (core.navigatorControl.planetEntries()[selectedPlanetIndex].text == FlightGlobals.currentMainBody.name)
                    {
                        GUIDune.Warning("Warning: You can't initiate a spacefold to the same planet that you are at!");
                    }
                    else
                    {
                        if (GUILayout.Button("Activate Spacefold", new GUIStyle(GUI.skin.button) { margin = new RectOffset(10, 10, 5, 20) }))
                        {
                           var state = core.navigatorControl.PreliminarySpacefoldProcedure(true, selectedPlanetIndex);
                        }
                    }
                }
            }
            if (selectedMenuIndex == 2)
            {
                GUIDune.Title("Vessel information!");

                // Show engine layout.
                GUIDune.Title("Engine Data");
                if (core.navigatorControl.SettingsRetrieved)
                {
                    GUIDune.Label("Engine Name: ", core.navigatorControl.engineName);
                    GUIDune.Label("Engine Efficency: ", core.navigatorControl.engineEfficiency + "%");
                    GUIDune.Label("Engine Failure: ", core.navigatorControl.engineFailure + "%");
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

            base.WindowGUI(windowId);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(700), GUILayout.Height(500) };
        }

        public override string GetName()
        {
            return String.Empty;
        }
    }
}
