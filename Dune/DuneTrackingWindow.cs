using UnityEngine;

namespace Dune
{
    public class DuneTrackingWindow : DisplayModule
    {
        public DuneDebrisControl debrisControl;

        public DuneTrackingWindow(DuneCore core) : base(core) 
        {
            priority = 400;
            runModuleInScenes.Add(GameScenes.TRACKSTATION);
        }

        public override void OnStart()
        {
            debrisControl = core.GetControlModule<DuneDebrisControl>();
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
            if (GUILayout.Button("Execute"))
            {
                ScreenMessages.PostScreenMessage("Removing all debris!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                debrisControl.RemoveAll();
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
