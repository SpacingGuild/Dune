using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dune
{
    public class ControlModule : IComparable<ControlModule>
    {
        public DuneCore core = null;
        public int priority = 0;

        public List<GameScenes> runModuleInScenes = new List<GameScenes>();

        public int CompareTo(ControlModule other)
        {
            if (other == null) return 1;
            return priority.CompareTo(other.priority);
        }

        protected bool _enabled = false;
        public bool enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    if (_enabled)
                        OnControllerEnabled();
                    else
                        OnControllerDisabled();
                }
            }
        }

        public ControlModule(DuneCore core)
        {
            this.core = core;
        }

        public virtual void OnControllerEnabled()
        {
            Debug.Log("[Dune] " + this.GetType().Name + " Enabled: " + enabled);
        }

        public virtual void OnControllerDisabled()
        {
            Debug.Log("[Dune] " + this.GetType().Name + " Enabled: " + enabled);
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnActive()
        {
        }

        public virtual void OnInactive()
        {
        }

        public virtual void OnAwake()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnLoad(ConfigNode configGlobal, ConfigNode configVessel, ConfigNode configLocal)
        {
            try
            {
                if (configGlobal != null) ConfigNode.LoadObjectFromConfig(this, configGlobal, (int)Pass.configGlobal);
                if (configVessel != null) ConfigNode.LoadObjectFromConfig(this, configVessel, (int)Pass.configVessel);
                if (configLocal != null) ConfigNode.LoadObjectFromConfig(this, configLocal, (int)Pass.configLocal);
            }
            catch (Exception e)
            {
                Debug.Log("[Dune] ControlModule:Base Exception for Onload of: " + this.GetType().Name + ":" + e);
            }
        }

        public virtual void OnSave(ConfigNode configGlobal, ConfigNode configVessel, ConfigNode configLocal)
        {
            try
            {
                if (configGlobal != null) ConfigNode.CreateConfigFromObject(this, (int)Pass.configGlobal, null).CopyTo(configGlobal);
                if (configVessel != null) ConfigNode.CreateConfigFromObject(this, (int)Pass.configVessel, null).CopyTo(configVessel);
                if (configLocal != null) ConfigNode.CreateConfigFromObject(this, (int)Pass.configLocal, null).CopyTo(configLocal);
            }
            catch (Exception e)
            {
                Debug.Log("[Dune] ControlModule:Base Exception for OnSave of: " + this.GetType().Name + ":" + e);
            }
        }

        public virtual void OnDestroy()
        {
        }
    }

    [Flags]
    public enum Pass
    {
        configGlobal = 1,
        configVessel = 2,
        configLocal = 4
    }

}
