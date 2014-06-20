using System;
using System.Linq;
using UnityEngine;

namespace Dune
{
    // TODO: Rename all displaymodule names... edit: eh, what ?
    public class DisplayModule : ControlModule
    {
        [Persistent(pass = (int)Pass.configGlobal)]
        public bool windowIsHidden = true;

        [Persistent(pass = (int)Pass.configGlobal)]
        public Vector4 windowVector = new Vector4(10, 40, 0, 0);

        [Persistent(pass = (int)Pass.configGlobal)]
        public Vector4 windowVectorTrack = new Vector4(10, 40, 0, 0);

        [Persistent(pass = (int)Pass.configGlobal)]
        public bool hideInToolbar = false;

        public int Id;
        public static int nextId = 6451535;

        public Rect windowPosition
        {
            get
            {
                if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
                    return new Rect(windowVectorTrack.x, windowVectorTrack.y, windowVectorTrack.z, windowVectorTrack.w);
                else
                    return new Rect(windowVector.x, windowVector.y, windowVector.z, windowVector.w);
            }
            set
            {
                if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
                {
                    windowVectorTrack = new Vector4
                    (
                        Math.Min(Math.Max(value.x, 0), Screen.width - value.width),
                        Math.Min(Math.Max(value.y, 0), Screen.height - value.height),
                        value.width, value.height
                    );
                    windowVectorTrack.x = Mathf.Clamp(windowVectorTrack.x, 10 - value.width, Screen.width - 10);
                    windowVectorTrack.y = Mathf.Clamp(windowVectorTrack.y, 10 - value.height, Screen.height - 10);
                }
                else
                {
                    windowVector = new Vector4
                    (
                        Math.Min(Math.Max(value.x, 0), Screen.width - value.width),
                        Math.Min(Math.Max(value.y, 0), Screen.height - value.height),
                        value.width, value.height
                    );
                    windowVector.x = Mathf.Clamp(windowVector.x, 10 - value.width, Screen.width - 10);
                    windowVector.y = Mathf.Clamp(windowVector.y, 10 - value.height, Screen.height - 10);
                }
            }
        }

        public DisplayModule(DuneCore core)
            : base(core)
        {
            Id = nextId;
            nextId++;

            InputLockManager.RemoveControlLock("DuneLockPart" + Id);
        }

        public virtual GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(250), GUILayout.Height(50) };
        }

        protected virtual void WindowGUI(int windowId)
        {
            if (GUI.Button(new Rect(windowPosition.width - 18, 2, 16, 16), ""))
            {
                windowIsHidden = true;
                InputLockManager.RemoveControlLock("DuneLockPart" + Id);
            }
            GUI.DragWindow();
        }
        //private readonly int id = new System.Random().Next(int.MaxValue);
        private bool isEditorLocked = false;
        public virtual void DrawGUI()
        {
            if (runModuleInScenes.Contains(HighLogic.LoadedScene) && !windowIsHidden)
            {
                windowPosition = GUILayout.Window(Id, windowPosition, WindowGUI, GetName(), WindowOptions());

                if (HighLogic.LoadedSceneIsEditor)
                {
                    // Lock parts while mouse is over the window.
                    if (windowPosition.Contains(Input.mousePosition) && !isEditorLocked)
                    {
                        EditorTooltip.Instance.HideToolTip();
                        EditorLogic.fetch.Lock(true, true, true, "DuneLockPart" + Id);
                        isEditorLocked = true;
                    }
                    else if (!windowPosition.Contains(Input.mousePosition) && isEditorLocked)
                    {
                        EditorLogic.fetch.Unlock("DuneLockPart" + Id);
                        isEditorLocked = false;
                    }
                }
                else
                {
                    // Hide part rightclick menu.
                    if (!GUIUtility.hotControl.IsNull())
                    {
                        if (windowPosition.Contains(Input.mousePosition) && GUIUtility.hotControl == 0) // use && Input.GetMouseButton(0)) to not hide the window, but prevent button click.
                        {
                            foreach (var window in GameObject.FindObjectsOfType(typeof(UIPartActionWindow)).OfType<UIPartActionWindow>().Where(p => p.Display == UIPartActionWindow.DisplayType.Selected))
                            {
                                window.enabled = false;
                                window.displayDirty = true;
                            }
                        }
                    }
                }
            }
        }

        public virtual string GetName()
        {
            return "Display Module";
        }
    }
}
