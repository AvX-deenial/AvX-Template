using System;
using System.Collections.Generic;
using System.Text;

namespace AVTemp.Background
{
    public class ButtonCL
    {
        public string text = "";
        public Action mod = null;
        public Action enabledMod = null;
        public Action disablemethod = null; // ignore these idfk what happened
        public bool enabled = false;
        public bool toggle = false;
        public string tooltipNotif = "";

        public ButtonCL(string buttonText, Action method, Action enableMethod, Action disableMethod, bool enabled, bool isTogglable, string toolTip)
        {
            this.text = buttonText;
            this.mod = method;
            this.enabledMod = enableMethod;
            this.disablemethod = disableMethod;
            this.enabled = enabled;
            this.toggle = isTogglable;
            this.tooltipNotif = toolTip;
        }
    }
}
