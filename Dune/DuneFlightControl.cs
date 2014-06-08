using System;
using UnityEngine;
using KSP.IO;

namespace Dune
{
    public class DuneFlightControl : PartModule, IComparable<DuneFlightControl>
    {
        private static Rect _windowPosition = new Rect();
        private GUIStyle _windowStyle, _labelStyle;
        private bool _hasInitStyles = false;
        //private DuneDataControl _dataController = new DuneDataControl();

        public override void OnStart(PartModule.StartState state)
        {
            if (state != StartState.Editor)
            {
                if (!_hasInitStyles) InitStyles();
                RenderingManager.AddToPostDrawQueue(0, OnDraw);
            }
        }

        private void OnDraw()
        {
            if (vessel.isActiveVessel && this == (DuneFlightControl)vessel.GetMasterObject<DuneFlightControl>())
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
                //ScreenMessages.PostScreenMessage(_dataController.GetCostSpice(this.vessel, 7.0).ToString(), 5.0f, ScreenMessageStyle.UPPER_CENTER);
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

        public override void OnLoad(ConfigNode node)
        {
            if (HighLogic.LoadedScene != GameScenes.EDITOR)
            {
                //Debug.LogWarning("[Dune] FlightController OnLoad loadSettings");
                //_windowPosition.x = Utilities.TryParse(SettingsManager.GetValue("FlightWindowLeft"), 250f);
                //_windowPosition.y = Utilities.TryParse(SettingsManager.GetValue("FlightWindowTop"), 250f);
            }
        }

        public override void OnSave(ConfigNode node)
        {
            // Nothing to save if in editor
            if (HighLogic.LoadedSceneIsEditor) return;

            // Only masters can save
            if (this != (DuneFlightControl)vessel.GetMasterObject<DuneFlightControl>()) return;

            // Do not save when undocking
            if (HighLogic.LoadedSceneIsFlight && vessel.vesselName == null) return;

            //Debug.LogWarning("[Dune] FlightController OnSave saveSettings");
            //SettingsManager.SetValue("FlightWindowLeft", _windowPosition.x);
            //SettingsManager.SetValue("FlightWindowTop", _windowPosition.y);
            //SettingsManager.Save();
        }

        public void OnDestroy()
        {
            if (this == (DuneFlightControl)vessel.GetMasterObject<DuneFlightControl>() && vessel.isActiveVessel)
                OnSave(null);
        }

        public int GetImportance()
        {
            if (part.State == PartStates.DEAD)
                return 0;
            else
                return GetInstanceID();
        }

        public int CompareTo(DuneFlightControl other)
        {
            if (other == null) return 1;
            return GetImportance().CompareTo(other.GetImportance());
        }
    }
}
