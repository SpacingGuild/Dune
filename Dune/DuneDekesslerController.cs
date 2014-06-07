using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using KSP.IO;

namespace Dune
{
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class DuneDekesslerController : ScenarioModule
    {
        private static Rect _windowPosition = new Rect();
        private GUIStyle _windowStyle, _labelStyle;
        private bool _hasInitStyles, _autoDekessle, _windowIsVisible;
        private IButton btnTrackingController;

        public void Update()
        {
            if (_autoDekessle)
            {
                // Add auto dekessler
                Debug.LogWarning("[Dune] DekesslerAuto");
            }
        }
        public void Start()
        {
            if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
            {
                Debug.LogWarning("[Dune] Start DekesslerControl");
                
                if (!_hasInitStyles) InitStyles();

                _windowPosition.x = Utilities.TryParse(SettingsManager.GetValue("TrackingStationWindowLeft"), 250f);
                _windowPosition.y = Utilities.TryParse(SettingsManager.GetValue("TrackingStationWindowTop"), 250f);
                _windowIsVisible = Utilities.TryParse(SettingsManager.GetValue("TrackingStationWindowShow"), false);
                _autoDekessle = Utilities.TryParse(SettingsManager.GetValue("AutoDekessle"), false);
                
                if (ToolbarManager.ToolbarAvailable)
                    createToolbarButton();

                RenderingManager.AddToPostDrawQueue(0, OnDraw);
            }
        }

        private void createToolbarButton()
        {
            var btnTrackingController = ToolbarManager.Instance.add("Dune", "trackingControllerBtn");
            btnTrackingController.TexturePath = "SpacingGuild/Dune/toolbar";
            refreshTooltip(btnTrackingController);
            btnTrackingController.Visibility = new GameScenesVisibility(GameScenes.TRACKSTATION);
            btnTrackingController.OnClick += e =>
            {
                _windowIsVisible = !_windowIsVisible;
                refreshTooltip(btnTrackingController);
            };
        }

        private void refreshTooltip(IButton btn)
        {
            btn.ToolTip = (_windowIsVisible ? "Hide" : "Show") + " dekessler window";
        }

        private void OnDraw()
        {
            if(_windowIsVisible)
                _windowPosition = GUILayout.Window(11, _windowPosition, OnWindow, "Dune Tracking Window", _windowStyle);
        }
        private void OnWindow(int windowId)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Dekessle debris: ", _labelStyle);
            if (GUILayout.Button("Dekessle"))
            {
                ScreenMessages.PostScreenMessage("Dekessling all debris!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                Dekessle();
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }
        private void Dekessle()
        {
            int count = 0;
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.vesselType == VesselType.Debris)
                {
                    try
                    {
                        count = count + 1;
                        FlightGlobals.Vessels.Remove(vessel);
                        vessel.Die();

                        foreach(ProtoCrewMember crewMember in vessel.GetVesselCrew())
                        {
                            crewMember.rosterStatus = ProtoCrewMember.RosterStatus.MISSING;
                            crewMember.Die();
                            Debug.LogWarning("[Dune] crewMember: " + crewMember.name + " was reported missing!");
                        }

                        Debug.LogWarning("[Dune] Vessel ID: " + vessel.id + " was destroyed!");
                    }
                    catch (System.Exception e)
                    {
                        //InvalidOperationException: Collection was modified; enumeration operation may not execute.
                        Debug.LogError("[Dune] Vessel ID" + vessel.id + "couldn't be destroyed: " + e);
                    }
                }
            }
            if (count > 0)
            {
                Debug.LogWarning("[Dune] Reload tracking station");
                HighLogic.LoadScene(GameScenes.TRACKSTATION);
                ScreenMessages.PostScreenMessage("Dekessled: " + count, 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            else
            {
                Debug.LogWarning("[Dune] Reload postponed");
                ScreenMessages.PostScreenMessage("No debris found!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
        }
        private void InitStyles()
        {
            _windowStyle = new GUIStyle(HighLogic.Skin.window);
            _windowStyle.fixedWidth = 250f;

            _labelStyle = new GUIStyle(HighLogic.Skin.label);
            _labelStyle.stretchWidth = true;

            _hasInitStyles = true;
        }

        public void OnDestroy()
        {
            if (HighLogic.LoadedScene != GameScenes.LOADING)
            {
                Debug.LogError("[Dune] TrackingController OnDestroy saveSettings");
                SettingsManager.SetValue("TrackingStationWindowLeft", _windowPosition.x);
                SettingsManager.SetValue("TrackingStationWindowTop", _windowPosition.y);
                SettingsManager.SetValue("TrackingStationWindowShow", _windowIsVisible);
                SettingsManager.SetValue("AutoDekessle", _autoDekessle);
                SettingsManager.Save();

                //KeyNotFoundException: The given key was not present in the dictionary.
                // Occurs because there are some keys(FlightWindowLeft and FlightWindowTop) 
                // that haven't been loaded once during the specific session.

                // Fix: Use of Persistent attributes on keys in class DuneSettingsController
            }
        }
    }
}
