using UnityEngine;
using KSP.IO;

namespace Dune
{
    public class DuneFlightController : PartModule
    {
        private static Rect _windowPosition = new Rect();
        private GUIStyle _windowStyle, _labelStyle;
        private bool _hasInitStyles = false;
        private DuneDataController _dataController = new DuneDataController();

        public override void OnStart(PartModule.StartState state)
        {
            if (state != StartState.Editor)
            {
                if (!_hasInitStyles) InitStyles();
                _windowPosition.x = Utilities.TryParse(SettingsManager.GetValue("FlightWindowLeft"), 250f);
                _windowPosition.y = Utilities.TryParse(SettingsManager.GetValue("FlightWindowTop"), 250f);
                RenderingManager.AddToPostDrawQueue(0, OnDraw);
            }
        }

        private void OnDraw()
        {
            if (this.vessel == FlightGlobals.ActiveVessel && this.part.IsPrimary(this.vessel.parts, this.ClassID))
            {
                _windowPosition = GUILayout.Window(10, _windowPosition, OnWindow, "Dune Flight Controller", _windowStyle);
            }
        }

        private void OnWindow(int windowId)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("This is a label", _labelStyle);
            if (GUILayout.Button("Cost"))
            {
                ScreenMessages.PostScreenMessage(_dataController.GetCostSpice(this.vessel, 7.0).ToString(), 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void InitStyles()
        {
            _windowStyle = new GUIStyle(HighLogic.Skin.window);
            _windowStyle.fixedWidth = 250f;

            _labelStyle = new GUIStyle(HighLogic.Skin.label);
            _labelStyle.stretchWidth = true;

            _hasInitStyles = true;
        }

        public override void OnUpdate()
        {
            
        }

        public void OnDestroy()
        {
            if (HighLogic.LoadedScene != GameScenes.LOADING)
            {
                Debug.LogError("[Dune] FlightController OnDestroy saveSettings");
                SettingsManager.SetValue("FlightWindowLeft", _windowPosition.x);
                SettingsManager.SetValue("FlightWindowTop", _windowPosition.y);
                SettingsManager.Save();
            }
        }
    }
}
