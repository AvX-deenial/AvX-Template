using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;
using UnityEngine;
using Object = UnityEngine.Object;
using static AVTemp.Background.InputLib;
using static AVTemp.Menu.WristMenu;
using GorillaGameModes;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Mono.Math.BigInteger;

namespace AVTemp.Background
{
    internal class GunLib
    {
        public static void CreateGun(Action help, bool lockOn)
        {
            if (rGrip || Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out Rayhit);
                if (Mouse.current.rightButton.isPressed)
                {
                    shouldBePC = true;
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Rayhit, 100f);
                }
                NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = ((isCopying || rTrigger || Mouse.current.leftButton.isPressed) ? enableButtonColor : disableButtonColor);
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = (isCopying ? whoCopy.transform.position : Rayhit.point);
                Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                Object.Destroy(NewPointer.GetComponent<Collider>());
                Object.Destroy(NewPointer, Time.deltaTime);
                GameObject gameObject2 = new GameObject("Line");
                LineRenderer lineRenderer = gameObject2.AddComponent<LineRenderer>();
                lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
                lineRenderer.startColor = bgcolor(0f);
                lineRenderer.endColor = bgcolor(0.5f);
                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.05f;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;
                lineRenderer.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                lineRenderer.SetPosition(1, isCopying ? whoCopy.transform.position : Rayhit.point);
                int Step = 50;
                //if (GetGunInput(true))
                //{
                lineRenderer.positionCount = Step;
                lineRenderer.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);

                for (int i = 1; i < (Step - 1); i++)
                    lineRenderer.SetPosition(i, Vector3.Lerp(GorillaTagger.Instance.rightHandTransform.position, NewPointer.transform.position, i / (Step - 1f)) + (UnityEngine.Random.Range(0f, 1f) > 0.75f ? new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)) : Vector3.zero));

                lineRenderer.SetPosition(Step - 1, NewPointer.transform.position);
                Object.Destroy(gameObject2, Time.deltaTime);
                if (lockOn)
                {
                    if (isCopying && whoCopy != null && Time.time > kgDebounce)
                    {
                        help();
                    }
                    if (rTrigger || Mouse.current.leftButton.isPressed)
                    {
                        componentInParent = Rayhit.collider.GetComponentInParent<VRRig>();
                        if (componentInParent && componentInParent != GorillaTagger.Instance.offlineVRRig)
                        {
                            isCopying = true;
                            whoCopy = componentInParent;
                        }
                    }
                }
                else
                {
                    if (rTrigger || Mouse.current.leftButton.isPressed)
                    {
                        help();
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                }
            }
        }
        public static bool isCopying = false;
        public static VRRig whoCopy = null;
        public static bool shouldBePC = false;
        public static bool makegun = false;
        public static float kgDebounce = 0f;
        public static GameObject NewPointer = null;
        public static RaycastHit Rayhit;
        public static VRRig componentInParent;

        public static Color GetLineColor(float offset, Color color1, Color color2)
        {
            Color result = color1;
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = color1;
            array[0].time = 0f;
            array[1].color = color2;
            array[1].time = 0.5f;
            array[2].color = color1;
            array[2].time = 1f;
            Gradient gradient = new Gradient
            {
                colorKeys = array
            };
            result = gradient.Evaluate((Time.time / 2f + offset) % 1f);
            return result;
        }
    }
}
