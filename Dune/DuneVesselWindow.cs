using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dune
{
    public class DuneVesselWindow : DisplayModule
    {
        public DuneVesselWindow(DuneCore core)
            : base(core)
        {
            priority = 100;
            runModuleInScenes.Add(GameScenes.FLIGHT);
        }

        GameObject objCoM;
        GameObject objCoT;
        GameObject objCoL;

        public bool showCoM = false;
        public bool showCoT = false;
        public bool showCoL = false;

        public override void OnStart()
        {
            objCoM = new GameObject("objectCenterMass");
            objCoT = new GameObject("objectCenterThrust");
            objCoL = new GameObject("objectCenterLift");
        }

        private void CreateCoMModel()
        {
            if (GameDatabase.Instance.ExistsModel("SpacingGuild/Dune/Textures/modelCM"))
            {
                var com = FlightGlobals.ActiveVessel.findLocalCenterOfMass();
                ScreenMessages.PostScreenMessage("COM" + com, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                objCoM = GameDatabase.Instance.GetModel("SpacingGuild/Dune/Textures/modelCM");
                objCoM.SetActive(true);
                objCoM.transform.SetParent(FlightGlobals.ActiveVessel.transform);
                objCoM.transform.localPosition = com;
                objCoM.transform.rotation = FlightGlobals.ActiveVessel.transform.rotation;
            }
            else
            {
                Debug.Log("[Dune] DuneSpacefolderWindow CreateCoMModel() model does not exist.");
                showCoM = false;
            }
        }

        private void CreateCoTModel()
        {
            if (GameDatabase.Instance.ExistsModel("SpacingGuild/Dune/Textures/modelCT"))
            {
                var com = FlightGlobals.ActiveVessel.findLocalCenterOfMass();
                var cot = FlightGlobals.ActiveVessel.findLocalCenterOfPressure();
                ScreenMessages.PostScreenMessage("COT" + (com + cot), 5.0f, ScreenMessageStyle.UPPER_CENTER);
                objCoT = GameDatabase.Instance.GetModel("SpacingGuild/Dune/Textures/modelCT");
                objCoT.SetActive(true);
                objCoT.transform.SetParent(FlightGlobals.ActiveVessel.transform);
                objCoT.transform.localPosition = (com + cot);
                //TODO: Fix rotation of the exhaust so that it points towards the ground/opposite of the heading.
                objCoT.transform.rotation = objCoT.transform.rotation.Inverse();
            }
            else
            {
                Debug.Log("[Dune] DuneSpacefolderWindow CreateCoTModel() model does not exist.");
                showCoT = false;
            }
        }

        private void CreateCoLModel()
        {
            //TODO: Figure out how to find the position of Center of Lift.
            if (GameDatabase.Instance.ExistsModel("SpacingGuild/Dune/Textures/modelCL"))
            {
                var com = FlightGlobals.ActiveVessel.findLocalCenterOfMass();
                var cot = FlightGlobals.ActiveVessel.findLocalCenterOfPressure();
                var moi = FlightGlobals.ActiveVessel.findLocalMOI(FlightGlobals.ActiveVessel.findWorldCenterOfMass());
                var moiNEW = (new Vector3(0, moi.y - com.y, 0));
                ScreenMessages.PostScreenMessage("MOI" + moiNEW, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                objCoL = GameDatabase.Instance.GetModel("SpacingGuild/Dune/Textures/modelCL");
                objCoL.SetActive(true);
                objCoL.transform.SetParent(FlightGlobals.ActiveVessel.transform);
                objCoL.transform.localPosition = moiNEW;
                objCoL.transform.rotation = FlightGlobals.ActiveVessel.transform.rotation;
            }
            else
            {
                Debug.Log("[Dune] DuneSpacefolderWindow CreateCoLModel() model does not exist.");
                showCoL = false;
            }
        }

        protected override void WindowGUI(int windowId)
        {
            GUILayout.BeginVertical();

            GUIStyle styleCenter = new GUIStyle(GUI.skin.label);
            styleCenter.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label("Vessel Centers ", styleCenter, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            if (GUILayout.Button("CoM", GUILayout.ExpandWidth(true)))
            {
                showCoM = !showCoM;
                if (showCoM)
                {
                    CreateCoMModel();
                }
                if (!showCoM)
                {
                    objCoM.SetActive(false);
                }
            }
            if (GUILayout.Button("CoT", GUILayout.ExpandWidth(true)))
            {
                showCoT = !showCoT;
                if (showCoT)
                {
                    CreateCoTModel();
                }
                if (!showCoT)
                {
                    objCoT.SetActive(false);
                }
            }
            if (GUILayout.Button("CoL", GUILayout.ExpandWidth(true)))
            {
                ScreenMessages.PostScreenMessage("Disabled until further notice.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                //showCoL = !showCoL;
                //if (showCoL)
                //{
                //    CreateCoLModel();
                //}
                //if (!showCoL)
                //{
                //    objCoL.SetActive(false);
                //}
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            base.WindowGUI(windowId);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(250), GUILayout.Height(50) };
        }

        public override string GetName()
        {
            return "Vessel Data";
        }
    }
}
