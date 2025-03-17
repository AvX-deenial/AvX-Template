using System;
using BepInEx;
using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;
using static QRCoder.PayloadGenerator.SwissQrCode;
using UnityEngine;
using AVTemp;
using static AVTemp.Background.InputLib;
using AVTemp.Background;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using UnityEngine.InputSystem;
using AVTemp.Patches;
using System.Linq;
using static AVTemp.Background.Methods.Mods;

namespace AVTemp.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    public class WristMenu : MonoBehaviour
    {
        #region Ignore Unless You're Experienced
        public static void Prefix()
        {
            try
            {
                bool toOpen = (!rightHandMenu && ControllerInputPoller.instance.leftControllerSecondaryButton) || (rightHandMenu && ControllerInputPoller.instance.rightControllerSecondaryButton);
                bool keyboardOpen = UnityInput.Current.GetKey(PCmenuOpener);

                if (wristMenu == null)
                {
                    if (toOpen || keyboardOpen)
                    {
                        drawww();
                        Recenter(rightHandMenu, keyboardOpen);
                        if (reference == null)
                        {
                            makereference(rightHandMenu);
                        }
                    }
                }
                else
                {
                    if ((toOpen || keyboardOpen))
                    {
                        Recenter(rightHandMenu, keyboardOpen);
                    }
                    else
                    {
                        GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(true);

                        Rigidbody comp = wristMenu.AddComponent(typeof(Rigidbody)) as Rigidbody;
                        if (rightHandMenu)
                        {
                            comp.velocity = GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                        }
                        else
                        {
                            comp.velocity = GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                        }

                        UnityEngine.Object.Destroy(wristMenu, 2);
                        wristMenu = null;

                        UnityEngine.Object.Destroy(reference);
                        reference = null;
                    }
                }

                if (wristMenu != null)
                {
                    if (fybvsfdhvuhsurusugthsuhuihsdfbvbygfysdgyrgygsgdfs == 2)
                    {
                        if (lGrip == true && plastLeftGrip == false)
                        {
                            MakeButtonSound(rightHandMenu, buttonSound);
                            Toggle("PreviousPage");
                        }
                        plastLeftGrip = lGrip;

                        if (rGrip == true && plastRightGrip == false)
                        {
                            MakeButtonSound(rightHandMenu, buttonSound);
                            Toggle("NextPage");
                        }
                        plastRightGrip = rGrip;
                    }

                    if (fybvsfdhvuhsurusugthsuhuihsdfbvbygfysdgyrgygsgdfs == 3)
                    {
                        if (lTrigger && plastLeftGrip == false)
                        {
                            MakeButtonSound(rightHandMenu, buttonSound);
                            Toggle("PreviousPage");
                        }
                        plastLeftGrip = lTrigger;

                        if (rTrigger && plastRightGrip == false)
                        {
                            MakeButtonSound(rightHandMenu, buttonSound);
                            Toggle("NextPage");
                        }
                        plastRightGrip = rTrigger;
                    }
                }
            }
            catch (Exception exc)
            {
                UnityEngine.Debug.LogError(string.Format("{0} // Error initializing at {1}: {2}", PlugInfo.Name, exc.StackTrace, exc.Message));
            }

            // Constant
            try
            {
                // Execute Enabled mods
                foreach (ButtonCL[] buttonlist in buttons)
                {
                    foreach (ButtonCL v in buttonlist)
                    {
                        if (v.enabled)
                        {
                            if (v.mod != null)
                            {
                                try
                                {
                                    v.mod.Invoke();
                                }
                                catch (Exception exc)
                                {
                                    UnityEngine.Debug.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PlugInfo.Name, v.text, exc.StackTrace, exc.Message));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                UnityEngine.Debug.LogError(string.Format("{0} // Error with executing mods at {1}: {2}", PlugInfo.Name, exc.StackTrace, exc.Message));
            }
        }
        #endregion
        #region Variables
        public static GameObject wristMenu;
        public static GameObject wristBG;
        public static GameObject reference;
        public static GameObject canvaS;
        public static Camera TPC;
        public static int buttonTab = 0;
        public static int pageNumber = 0;
        public static SphereCollider buttonCollider;
        public static bool plastLeftGrip;
        public static bool plastRightGrip;
        #endregion
        #region Customization
        // changes the menu title
        public static string MenuName = "AvX Template";

        // bg colors
        public static Color FirstColor = new Color32(228, 72, 64, 255);
        public static Color SecondColor = new Color32(228, 72, 64, 255);

        // button colors
        public static Color disableButtonColor = Color.black;
        public static Color enableButtonColor = Color.black;

        // text colors
        public static Color titleColor = Color.white;
        public static Color disableTextColor = Color.white;
        public static Color enableTextColor = Color.red;

        // outline color
        public static Color outlineColor = Color.black;

        // max amount of buttons per page
        public static int buttonCount = 7;

        // switches the menu from the left hand to the right hand
        public static bool rightHandMenu = false;

        // changes if the menu has an outline or not
        public static bool bgOutline = true;

        // changes the text font
        public static Font currentFont = Font.CreateDynamicFontFromOSFont("Cascadia Code", 24);

        // changes what type of page buttons you want
        public static string pageButtonType = ":Bottom:"; // you can change it to :Bottom: :Sides: :Triggers: and :Grips:

        // changes what sound plays when you press a button
        public static int buttonSound = 67; // the wardrobe button sound

        // enable and disables the disconnect and home button
        public static bool dcButton = true;
        public static bool homeButton = true;

        // enable and disables the notifications
        public static bool disableNotifs = false;

        // switches the buttons you have to press to open the menu
        public static bool menuOpener = yButton;
        public static KeyCode PCmenuOpener = KeyCode.Q; // PC menu opener

        // changes the menu bg size
        public static Vector3 menuSize = new Vector3(0.1f, 1f, 1f); // depth width hight
        #endregion
        #region Buttons
        public static void EnterTab(int tabNum) { buttonTab = tabNum; pageNumber = 0; }

        // yes you can remove tabs it'll still work
        public static ButtonCL[][] buttons = new ButtonCL[][]
        {
            new ButtonCL[] // Home [0]
            {
                new ButtonCL("Settings", ()=>EnterTab(1), null, null, false, false, "Brings you to the settings tab."),
                new ButtonCL("Tab 2", ()=>EnterTab(2), null, null, false, false, "Brings you to tab 2."),
                new ButtonCL("Tab 3", ()=>EnterTab(3), null, null, false, false, "Brings you to tab 3."),
                new ButtonCL("Tab 4", ()=>EnterTab(4), null, null, false, false, "Brings you to tab 4."),
                new ButtonCL("Tab 5", ()=>EnterTab(5), null, null, false, false, "Brings you to tab 5."),
                new ButtonCL("Tab 6", ()=>EnterTab(6), null, null, false, false, "Brings you to tab 6."),
                new ButtonCL("Tab 7", ()=>EnterTab(7), null, null, false, false, "Brings you to tab 7."),

                new ButtonCL("Tab 8", ()=>EnterTab(8), null, null, false, false, "Brings you to tab 8."),
                new ButtonCL("Tab 9", ()=>EnterTab(9), null, null, false, false, "Brings you to tab 9."),
                new ButtonCL("Tab 10", ()=>EnterTab(10), null, null, false, false, "Brings you to tab 10."),
                new ButtonCL("Tab 11", ()=>EnterTab(11), null, null, false, false, "Brings you to tab 11."),
                new ButtonCL("Tab 12", ()=>EnterTab(12), null, null, false, false, "Brings you to tab 12."),
            },
            new ButtonCL[] // Settings [1]
            {
                new ButtonCL("Exit Settings", ()=>EnterTab(0), null, null, false, false, "Exits the settings tab."),
                new ButtonCL("Disconnect Button", ()=>{ dcButton = true; RecrateMenu(); }, null, ()=>{ dcButton = false; RecrateMenu(); }, true, true, "Toggles the disconnect button."),
                new ButtonCL("Home Button", ()=>{ homeButton = true; RecrateMenu(); }, null, ()=>{ homeButton = false; RecrateMenu(); }, true, true, "Toggles the home button."),
                new ButtonCL("Change Page Buttons", ()=>ChangePageButtons(), null, null, false, false, "Changes the page buttons."),
            },
            new ButtonCL[] // Tab 2 [2]
            {
                new ButtonCL("Exit Tab 2", ()=>EnterTab(0), null, null, false, false, "Exits tab 2."),
            },
            new ButtonCL[] // Tab 3 [3]
            {
                new ButtonCL("Exit Tab 3", ()=>EnterTab(0), null, null, false, false, "Exits tab 3."),
            },
            new ButtonCL[] // Tab 4 [4]
            {
                new ButtonCL("Exit Tab 4", ()=>EnterTab(0), null, null, false, false, "Exits tab 4."),
            },
            new ButtonCL[] // Tab 5 [5]
            {
                new ButtonCL("Exit Tab 5", ()=>EnterTab(0), null, null, false, false, "Exits tab 5."),
            },
            new ButtonCL[] // Tab 6 [6]
            {
                new ButtonCL("Exit Tab 6", ()=>EnterTab(0), null, null, false, false, "Exits tab 6."),
            },
            new ButtonCL[] // Tab 7 [7]
            {
                new ButtonCL("Exit Tab 7", ()=>EnterTab(0), null, null, false, false, "Exits tab 7."),
            },
            new ButtonCL[] // Tab 8 [8]
            {
                new ButtonCL("Exit Tab 8", ()=>EnterTab(0), null, null, false, false, "Exits tab 8."),
            },
            new ButtonCL[] // Tab 9 [9]
            {
                new ButtonCL("Exit Tab 9", ()=>EnterTab(0), null, null, false, false, "Exits tab 9."),
            },
            new ButtonCL[] // Tab 10 [10]
            {
                new ButtonCL("Exit Tab 10", ()=>EnterTab(0), null, null, false, false, "Exits tab 10."),
            },
            new ButtonCL[] // Tab 11 [11]
            {
                new ButtonCL("Exit Tab 11", ()=>EnterTab(0), null, null, false, false, "Exits tab 11."),
            },
            new ButtonCL[] // Tab 12 [12]
            {
                new ButtonCL("Exit Tab 12", ()=>EnterTab(0), null, null, false, false, "Exits tab 12."),
            },
        };
        #endregion
        #region Actual Menu
        public static void drawww()
        {
            wristMenu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(wristMenu.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(wristMenu.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(wristMenu.GetComponent<Renderer>());
            wristMenu.transform.localScale = new Vector3(0.1f, 0.3f, .4f) * 1f;

            wristBG = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(wristBG.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(wristBG.GetComponent<BoxCollider>());
            wristBG.transform.parent = wristMenu.transform;
            wristBG.transform.rotation = Quaternion.identity;
            wristBG.transform.localScale = menuSize;
            wristBG.transform.localPosition = new Vector3(0.50f, 0f, 0f);

            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = FirstColor;
            array[0].time = 0f;
            array[1].color = SecondColor;
            array[1].time = 0.5f;
            array[2].color = FirstColor;
            array[2].time = 1f;
            ColorChanger colorChanger = wristBG.AddComponent<ColorChanger>();
            colorChanger.colors = new Gradient
            {
                colorKeys = array
            };
            colorChanger.Start();

            canvaS = new GameObject();
            canvaS.transform.parent = wristMenu.transform;
            Canvas canvas = canvaS.AddComponent<Canvas>();
            CanvasScaler canvasScaler = canvaS.AddComponent<CanvasScaler>();
            canvaS.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;

            Text text = new GameObject
            {
                transform =
                    {
                        parent = canvaS.transform
                    }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = MenuName + " <color=grey>[</color><color=white>" + (pageNumber + 1).ToString() + "</color><color=grey>]</color>";
            text.fontSize = 1;
            text.color = titleColor;
            text.supportRichText = true;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.3f, 0.065f);
            component.position = new Vector3(0.0551f, 0f, 0.175f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (bgOutline)
            {
                // top side outline
                CreateOutline(depthscale: 0.15f, widthscale: 1.01f, heightscale: 0.007f, depth: 0.5f, width: 0f, height: 0.5f); // depth: width: and height: is the position
                CreateOutline(depthscale: 0.15f, widthscale: 1.01f, heightscale: 0.007f, depth: 0.5f, width: 0f, height: -0.5f);

                // bottom side outline
                CreateOutline(depthscale: 0.15f, widthscale: 0.01f, heightscale: 1f, depth: 0.5f, width: 0.5f, height: 0f);
                CreateOutline(depthscale: 0.15f, widthscale: 0.01f, heightscale: 1f, depth: 0.5f, width: -0.5f, height: 0f);
            }

            if (dcButton)
            {
                GameObject dcButtonn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    dcButtonn.layer = 2;
                }
                UnityEngine.Object.Destroy(dcButtonn.GetComponent<Rigidbody>());
                dcButtonn.GetComponent<BoxCollider>().isTrigger = true;
                dcButtonn.transform.parent = wristMenu.transform;
                dcButtonn.transform.rotation = Quaternion.identity;
                dcButtonn.transform.localScale = new Vector3(0.1f, 0.8f, 0.08f);
                dcButtonn.transform.localPosition = new Vector3(0.5f, 0, 0.555f);//-0.7f);//-0.6f);//-0.7f);
                dcButtonn.AddComponent<Background.Button>().relatedText = "Disconnect";
                dcButtonn.GetComponent<Renderer>().material.color = disableButtonColor;
                text = new GameObject
                {
                    transform =
                            {
                                parent = canvaS.transform
                            }
                }.AddComponent<Text>();
                text.font = currentFont;
                text.text = "Leave";
                text.color = disableTextColor;
                text.supportRichText = true;
                text.fontSize = 1;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontStyle = FontStyle.Normal;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(.2f, .03f);
                component.localPosition = new Vector3(.0551f, 0f, .225f);//-.281f);//-.241f);//-.281f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            if (homeButton)
            {
                if (buttonTab > 0)
                {
                    GameObject Returner = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (!UnityInput.Current.GetKey(KeyCode.Q))
                    {
                        Returner.layer = 2;
                    }
                    UnityEngine.Object.Destroy(Returner.GetComponent<Rigidbody>());
                    Returner.GetComponent<BoxCollider>().isTrigger = true;
                    Returner.transform.parent = wristMenu.transform;
                    Returner.transform.rotation = Quaternion.identity;
                    Returner.transform.localScale = new Vector3(0.1f, 0.8f, 0.08f);
                    Returner.transform.localPosition = new Vector3(0.5f, 0, -0.555f);//-0.7f);//-0.6f);//-0.7f);
                    Returner.AddComponent<Background.Button>().relatedText = "ReturnToHome";
                    Returner.GetComponent<Renderer>().material.color = disableButtonColor;
                    text = new GameObject
                    {
                        transform =
                            {   
                                parent = canvaS.transform
                            }
                    }.AddComponent<Text>();
                    text.font = currentFont;
                    text.text = "Exit To Home";
                    text.color = disableTextColor;
                    text.supportRichText = true;
                    text.fontSize = 1;
                    text.alignment = TextAnchor.MiddleCenter;
                    text.fontStyle = FontStyle.Normal;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 0;
                    component = text.GetComponent<RectTransform>();
                    component.localPosition = Vector3.zero;
                    component.sizeDelta = new Vector2(.2f, .03f);
                    component.localPosition = new Vector3(.0551f, 0f, -.225f);//-.281f);//-.241f);//-.281f);
                    component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                }
            }

            if (pageButtonType == ":Bottom:")
            {
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = wristMenu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.38f, .08f);
                gameObject.transform.localPosition = new Vector3(0.56f, 0.22f, -0.43f);
                gameObject.GetComponent<Renderer>().material.color = disableButtonColor;
                gameObject.AddComponent<Background.Button>().relatedText = "PreviousPage";

                text = new GameObject
                {
                    transform =
                        {
                            parent = canvaS.transform
                        }
                }.AddComponent<Text>();
                text.font = currentFont;
                text.text = "<";
                text.fontSize = 1;
                text.color = disableTextColor;
                text.fontStyle = FontStyle.Normal;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                component.localPosition = new Vector3(0.061f, 0.0665f, -0.169f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = wristMenu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.38f, .08f);
                gameObject.transform.localPosition = new Vector3(0.56f, -0.22f, -0.43f);
                gameObject.GetComponent<Renderer>().material.color = disableButtonColor;
                gameObject.AddComponent<Background.Button>().relatedText = "NextPage";

                text = new GameObject
                {
                    transform =
                        {
                            parent = canvaS.transform
                        }
                }.AddComponent<Text>();
                text.font = currentFont;
                text.text = ">";
                text.fontSize = 1;
                text.color = disableTextColor;
                text.fontStyle = FontStyle.Normal;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                component.localPosition = new Vector3(0.061f, -0.0665f, -0.169f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            if (pageButtonType == ":Sides:")
            {
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = wristMenu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.1f, .9f);
                gameObject.transform.localPosition = new Vector3(0.56f, 0.6f, 0f);
                gameObject.GetComponent<Renderer>().material.color = disableButtonColor;
                gameObject.AddComponent<Background.Button>().relatedText = "PreviousPage";

                text = new GameObject
                {
                    transform =
                        {
                            parent = canvaS.transform
                        }
                }.AddComponent<Text>();
                text.font = currentFont;
                text.text = "<";
                text.fontSize = 1;
                text.color = disableTextColor;
                text.fontStyle = FontStyle.Normal;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                component.localPosition = new Vector3(0.061f, 0.18f, 0f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = wristMenu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.1f, .9f);
                gameObject.transform.localPosition = new Vector3(0.56f, -0.6f, 0f);
                gameObject.GetComponent<Renderer>().material.color = disableButtonColor;
                gameObject.AddComponent<Background.Button>().relatedText = "NextPage";

                text = new GameObject
                {
                    transform =
                        {
                            parent = canvaS.transform
                        }
                }.AddComponent<Text>();
                text.font = currentFont;
                text.text = ">";
                text.fontStyle = FontStyle.Normal;
                text.fontSize = 1;
                text.color = disableTextColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                component.localPosition = new Vector3(0.061f, -0.18f, 0f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            if (fybvsfdhvuhsurusugthsuhuihsdfbvbygfysdgyrgygsgdfs == 2 && fybvsfdhvuhsurusugthsuhuihsdfbvbygfysdgyrgygsgdfs == 3)
            {

            }

            ButtonCL[] activeButtons = buttons[buttonTab].Skip(pageNumber * buttonCount).Take(buttonCount).ToArray();
            for (int i = 0; i < activeButtons.Length; i++)
            {
                maekbutton(i * 0.11f, activeButtons[i]);
            }
        }
        public static void maekbutton(float offset, ButtonCL method)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q))
            {
                gameObject.layer = 2;
            }
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = wristMenu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.82f, 0.08f);
            gameObject.transform.localPosition = new Vector3(0.54f, 0f, 0.34f - offset);
            gameObject.AddComponent<Background.Button>().relatedText = method.text;

            if (method.enabled)
            {
                gameObject.GetComponent<Renderer>().material.color = enableButtonColor;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = disableButtonColor;
            }

            Text text = new GameObject
            {
                transform =
                {
                    parent = canvaS.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = method.text;
            text.supportRichText = true;
            text.fontSize = 1;
            if (method.enabled)
            {
                text.color = enableTextColor;
            }
            else
            {
                text.color = disableTextColor;
            }
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Normal;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.1925f, .0225f);
            component.localPosition = new Vector3(.061f, 0f, .135f - offset / 2.54f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }
        public static void makereference(bool righthanded)
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (righthanded)
            {
                reference.transform.parent = GorillaTagger.Instance.leftHandTransform;
            }
            else
            {
                reference.transform.parent = GorillaTagger.Instance.rightHandTransform;
            }
            reference.GetComponent<Renderer>().material.color = Color.cyan;
            reference.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            buttonCollider = reference.GetComponent<SphereCollider>();
        }
        public static void Recenter(bool righthanded, bool PCopen)
        {
            if (!PCopen)
            {
                if (!righthanded)
                {
                    wristMenu.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    wristMenu.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                }
                else
                {
                    wristMenu.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    Vector3 rotation = GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles;
                    rotation += new Vector3(0f, 0f, 180f);
                    wristMenu.transform.rotation = Quaternion.Euler(rotation);
                }
            }
            else
            {
                try
                {
                    TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
                }
                catch { }

                GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(false);

                if (TPC != null)
                {
                    TPC.transform.position = new Vector3(-999f, -999f, -999f);
                    TPC.transform.rotation = Quaternion.identity;
                    /*GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bg.transform.localScale = new Vector3(10f, 10f, 0.01f);
                    bg.transform.transform.position = TPC.transform.position + TPC.transform.forward;
                    bg.GetComponent<Renderer>().material.color = new Color32((byte)(FirstColor.r * 50), (byte)(FirstColor.g * 50), (byte)(FirstColor.b * 50), 255);
                    GameObject.Destroy(bg, Time.deltaTime);*/
                    wristMenu.transform.parent = TPC.transform;
                    wristMenu.transform.position = (TPC.transform.position + (Vector3.Scale(TPC.transform.forward, new Vector3(0.5f, 0.5f, 0.5f)))) + (Vector3.Scale(TPC.transform.up, new Vector3(-0.02f, -0.02f, -0.02f)));
                    Vector3 rot = TPC.transform.rotation.eulerAngles;
                    rot = new Vector3(rot.x - 90, rot.y + 90, rot.z);
                    wristMenu.transform.rotation = Quaternion.Euler(rot);

                    if (reference != null)
                    {
                        if (Mouse.current.leftButton.isPressed)
                        {
                            Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                            RaycastHit hit;
                            bool worked = Physics.Raycast(ray, out hit, 100);
                            if (worked)
                            {
                                Background.Button collide = hit.transform.gameObject.GetComponent<Background.Button>();
                                if (collide != null)
                                {
                                    collide.OnTriggerEnter(buttonCollider);
                                }
                            }
                        }
                        else
                        {
                            reference.transform.position = new Vector3(999f, -999f, -999f);
                        }
                    }
                }
            }
        }
        public static void RecrateMenu()
        {
            if (wristMenu != null)
            {
                UnityEngine.Object.Destroy(wristMenu);
                wristMenu = null;

                drawww();
                Recenter(rightHandMenu, UnityInput.Current.GetKey(PCmenuOpener));
            }
        }
        public static void Toggle(string buttonText)
        {
            int lastPage = ((buttons[buttonTab].Length + buttonCount - 1) / buttonCount) - 1;
            if (buttonText == "Disconnect")
            {
                PhotonNetwork.Disconnect();
            }
            else
            if (buttonText == "ReturnToHome")
            {
                EnterTab(0);
                pageNumber = 0;
            }
            else
            if (buttonText == "PreviousPage")
            {
                pageNumber--;
                if (pageNumber < 0)
                {
                    pageNumber = lastPage;
                }
            }
            else
            {
                if (buttonText == "NextPage")
                {
                    pageNumber++;
                    if (pageNumber > lastPage)
                    {
                        pageNumber = 0;
                    }
                }
                else
                {


                    ButtonCL target = GetButtonCL(buttonText);
                    if (target != null)
                    {
                        if (target.toggle)
                        {
                            target.enabled = !target.enabled;
                            if (target.enabled)
                            {
                                Notifs.SendNotification("[<color=cyan>" + MenuName.ToUpper() + "</color>] Enabled " + target.text + "<color=magenta> : " + target.tooltipNotif + "</color>");
                                if (target.enabledMod != null)
                                {
                                    try { target.enabledMod.Invoke(); } catch { }
                                }
                            }
                            else
                            {
                                Notifs.SendNotification("[<color=cyan>" + MenuName.ToUpper() + "</color>] Disabled " + target.text + "<color=magenta> : " + target.tooltipNotif + "</color>");
                                if (target.disablemethod != null)
                                {
                                    try { target.disablemethod.Invoke(); } catch { }
                                }
                            }
                        }
                        else
                        {
                            Notifs.SendNotification("[<color=cyan>" + MenuName.ToUpper() + "</color>] Toggled " + target.text + "<color=magenta> : " + target.tooltipNotif + "</color>");
                            if (target.mod != null)
                            {
                                try { target.mod.Invoke(); } catch { }
                            }
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(buttonText + " does not exist");
                    }
                }

            }
            RecrateMenu();
        }
        public static ButtonCL GetButtonCL(string buttonText)
        {
            foreach (ButtonCL[] buttons in buttons)
            {
                foreach (ButtonCL button in buttons)
                {
                    if (button.text == buttonText)
                    {
                        return button;
                    }
                }
            }

            return null;
        }
        #endregion
        #region Stuff Besides Making The Menu
        public static Color bgcolor(float offset)
        {
            Color result = FirstColor;
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = FirstColor;
            array[0].time = 0f;
            array[1].color = SecondColor;
            array[1].time = 0.5f;
            array[2].color = FirstColor;
            array[2].time = 1f;
            Gradient gradient = new Gradient
            {
                colorKeys = array
            };
            result = gradient.Evaluate((Time.time / 2f + offset) % 1f);
            return result;
        }

        public static void MakeButtonSound(bool righthanded, int buttonsound)
        {
            GorillaTagger.Instance.StartVibration(righthanded, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(buttonsound/*the wood in canyons*/, righthanded, 0.7f);
        }

        public static void CreateOutline(float depthscale, float widthscale, float heightscale, float depth, float width, float height)
        {
            GameObject gameObject69 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q))
            {
                gameObject69.layer = 2;
            }
            UnityEngine.Object.Destroy(gameObject69.GetComponent<Rigidbody>());
            gameObject69.GetComponent<BoxCollider>().isTrigger = true;
            gameObject69.transform.parent = wristMenu.transform;
            gameObject69.transform.rotation = Quaternion.identity;
            gameObject69.transform.localScale = new Vector3(depthscale, widthscale, heightscale);
            gameObject69.transform.localPosition = new Vector3(depth, width, height);
            gameObject69.GetComponent<Renderer>().material.color = outlineColor;
        }
        #endregion
    }
}
