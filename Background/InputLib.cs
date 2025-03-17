using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
using Valve.VR;
// Valve.VR;

namespace AVTemp.Background
{
    internal class InputLib
    {
        public static bool rTrigger
        {
            get
            {
                return getButton.rightControllerIndexFloat == 1f;
            }
        }
        public static bool lTrigger
        {
            get
            {
                return getButton.leftControllerIndexFloat == 1f;
            }
        }
        public static bool rGrip
        {
            get
            {
                return getButton.rightGrab;
            }
        }
        public static bool lGrip
        {
            get
            {
                return getButton.leftGrab;
            }
        }
        public static bool xButton
        {
            get
            {
                return getButton.leftControllerPrimaryButton;
            }
        }
        public static bool yButton
        {
            get
            {
                return getButton.leftControllerSecondaryButton;
            }
        }
        public static bool bButton
        {
            get
            {
                return getButton.rightControllerSecondaryButton;
            }
        }
        public static bool aButton
        {
            get
            {
                return getButton.rightControllerPrimaryButton;
            }
        }
        /*public static bool rJoystick
        {
            get
            {
                return SteamVR_Actions.gorillaTag_RightJoystickClick.state;
            }
        }
        public static bool lJoystick
        {
            get
            {
                return SteamVR_Actions.gorillaTag_LeftJoystickClick.state;
            }
        }*/

        public static Vector2 lJoystick
        {
            get
            {
                return SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
            }
        }

        public static Vector2 rJoystick
        {
            get
            {
                return SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
            }
        }

        public static bool lJoystickClick
        {
            get
            {
                return SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
            }
        }

        public static bool rJoystickClick
        {
            get
            {
                return SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.RightHand);
            }
        }

        private static ControllerInputPoller getButton
        {
            get
            {
                return ControllerInputPoller.instance;
            }
        }
    }
}
