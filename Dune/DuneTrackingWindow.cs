using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dune
{
    public class DuneTrackingWindow : DisplayModule
    {
        public DuneTrackingControl tracking;

        public DuneTrackingWindow(DuneCore core) : base(core) 
        {
            priority = 400;
            showInFlight = false;
            showInTrack = true;
        }

        public override void OnStart()
        {
            //COMMENT: Monitor TrackingWindow OnStart()
            Debug.Log("[Dune] TrackingWindow OnStart()");
            tracking = core.GetControlModule<DuneTrackingControl>();
        }

        public override string GetName()
        {
            return "Tracking Window";
        }

        protected override void WindowGUI(int windowId)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label("Remove debris:");
            if (GUILayout.Button("Dekessle"))
            {
                ScreenMessages.PostScreenMessage("Dekessling all debris!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                //Dekessle();
            }
            GUILayout.EndHorizontal();
            //Add more to the window here..
            GUILayout.EndVertical();

            base.WindowGUI(windowId);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(250), GUILayout.Height(50) };
        }
    }
}
