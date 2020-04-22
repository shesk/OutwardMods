using System;
using Partiality.Modloader;
using UnityEngine;
using UnityEngine.UI;

namespace OutwardMod_CoopUIScaler
{
    public class CoopUIScalerMod : PartialityMod
    {
        public CoopUIScalerMod()
        {
            ModID = "CoopUIScaler";
            Version = "0004";
            author = "SirMuffin+Laymain";
        }

        public override void OnEnable()
        {
            base.OnEnable();
            On.MapDisplay.Show_CharacterUI += MapDisplay_Show_CharacterUI;
            On.OptionsPanel.StartInit += OptionsPanel_StartInit;
            On.SplitScreenManager.Update += SplitScreenManager_Update;
            On.SplitScreenManager.DelayedRefreshSplitScreen += SplitScreenManager_DelayedRefreshSplitScreen;
        }

        private static int lastScreenHeight;
        private static int lastScreenWidth;
        private static bool moveGlobalUiToPlayer1 = true;
        private static Vector2 mapOrigAnchoredPosition;
        private static Vector2 mapOrigsizeDelta;
        private static float scaleFactor = 1f;

        public static void MapDisplay_Show_CharacterUI(On.MapDisplay.orig_Show_CharacterUI orig, MapDisplay self, CharacterUI owner)
        {
            orig.Invoke(self, owner);
            if (moveGlobalUiToPlayer1)
            {
                RectTransform privatePart = Utils.GetPrivatePart<RectTransform, CharacterUI>(owner, "m_rectTransform");
                self.RectTransform.anchoredPosition = privatePart.anchoredPosition;
                self.RectTransform.sizeDelta = privatePart.sizeDelta;
                return;
            }

            self.RectTransform.anchoredPosition = mapOrigAnchoredPosition;
            self.RectTransform.sizeDelta = mapOrigsizeDelta;
        }

        public static void OptionsPanel_StartInit(On.OptionsPanel.orig_StartInit orig, OptionsPanel self)
        {
            orig(self);
            Slider privatePart = Utils.GetPrivatePart<Slider, OptionsPanel>(self, "m_sldFoVSplit");
            if (privatePart != null)
            {
                privatePart.maxValue = 90f;
            }
        }

        public static void SplitScreenManager_Update(On.SplitScreenManager.orig_Update orig, SplitScreenManager self)
        {
            if (Input.GetKey(KeyCode.Home) && Input.GetKeyUp(KeyCode.V))
            {
                moveGlobalUiToPlayer1 = false;
                self.CurrentSplitType = SplitScreenManager.SplitType.Vertical;
                scaleFactor = 0.9f;
            }

            if (Input.GetKey(KeyCode.Home) && Input.GetKeyUp(KeyCode.H))
            {
                moveGlobalUiToPlayer1 = true;
                self.CurrentSplitType = SplitScreenManager.SplitType.Horizontal;
                scaleFactor = 1f;
            }

            if (Input.GetKey(KeyCode.Home) && Input.GetKeyUp(KeyCode.M))
            {
                moveGlobalUiToPlayer1 = !moveGlobalUiToPlayer1;
            }

            if (Input.GetKey(KeyCode.Home) && Input.GetKeyUp(KeyCode.KeypadPlus))
            {
                scaleFactor += 0.05f;
                self.ForceRefreshRatio = true;
                orig(self);
            }

            if (Input.GetKey(KeyCode.Home) && Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                scaleFactor -= 0.05f;
                self.ForceRefreshRatio = true;
                orig(self);
            }

            if (Input.GetKeyUp(KeyCode.Home))
            {
                self.ForceRefreshRatio = true;
            }

            if (Screen.height != lastScreenHeight)
            {
                lastScreenHeight = Screen.height;
                self.ForceRefreshRatio = true;
            }

            if (Screen.width != lastScreenWidth)
            {
                lastScreenWidth = Screen.width;
                self.ForceRefreshRatio = true;
            }

            orig(self);
        }

        public static void SplitScreenManager_DelayedRefreshSplitScreen(On.SplitScreenManager.orig_DelayedRefreshSplitScreen orig, SplitScreenManager self)
        {
            if (self.CurrentSplitType == SplitScreenManager.SplitType.Horizontal)
            {
                SplitScreenManager_DelayedRefreshSplitScreen_Horizontal(orig, self);
            }
            else if (self.CurrentSplitType == SplitScreenManager.SplitType.Vertical)
            {
                SplitScreenManager_DelayedRefreshSplitScreen_Vertical(orig, self);
            }

            CanvasScaler[] componentsInChildren = MenuManager.Instance.GetComponentsInChildren<CanvasScaler>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].matchWidthOrHeight = ((Screen.height > Screen.width) ? 0f : 1f);
            }

            Canvas[] componentsInChildren2 = MenuManager.Instance.GetComponentsInChildren<Canvas>();
            for (int i = 0; i < componentsInChildren2.Length; i++)
            {
                componentsInChildren2[i].scaleFactor = scaleFactor;
            }
        }

        // Token: 0x06000009 RID: 9 RVA: 0x000023A0 File Offset: 0x000005A0
        public static void SplitScreenManager_DelayedRefreshSplitScreen_Horizontal(On.SplitScreenManager.orig_DelayedRefreshSplitScreen orig, SplitScreenManager self)
        {
            self.CurrentSplitType = SplitScreenManager.SplitType.Horizontal;
            orig(self);
            if (moveGlobalUiToPlayer1)
            {
                Vector2 zero = Vector2.zero;
                Vector2 zero2 = Vector2.zero;
                if (self.LocalPlayers.Count == 2)
                {
                    zero2.y = -0.5f;
                    zero.y = -0.5f;
                }

                Vector2 vector = Vector2.Scale(zero2, MenuManager.Instance.ScreenSize);
                Vector2 anchoredPosition = Vector2.Scale(zero, vector);
                LoadingFade privatePart = Utils.GetPrivatePart<LoadingFade, MenuManager>(MenuManager.Instance, "m_masterLoading");
                if (privatePart != null)
                {
                    RectTransform componentInChildren = privatePart.GetComponentInChildren<RectTransform>();
                    if (componentInChildren != null)
                    {
                        componentInChildren.sizeDelta = vector;
                        componentInChildren.anchoredPosition = anchoredPosition;
                    }
                }

                ProloguePanel privatePart2 = Utils.GetPrivatePart<ProloguePanel, MenuManager>(MenuManager.Instance, "m_prologueScreen");
                if (privatePart2 != null)
                {
                    RectTransform rectTransform = privatePart2.RectTransform;
                    rectTransform.sizeDelta = vector;
                    rectTransform.anchoredPosition = anchoredPosition;
                }
            }
        }

        public static void SplitScreenManager_DelayedRefreshSplitScreen_Vertical(On.SplitScreenManager.orig_DelayedRefreshSplitScreen orig, SplitScreenManager self)
        {
            self.CurrentSplitType = SplitScreenManager.SplitType.Vertical;
            DictionaryExt<int, SplitPlayer> privatePart = Utils.GetPrivatePart<DictionaryExt<int, SplitPlayer>, SplitScreenManager>(self, "m_localCharacterUIs");
            if (GameDisplayInUI.Instance.gameObject.activeSelf != self.RenderInImage)
            {
                GameDisplayInUI.Instance.gameObject.SetActive(self.RenderInImage);
            }

            for (int i = 0; i < privatePart.Count; i++)
            {
                SplitPlayer splitPlayer = privatePart.Values[i];
                Vector3 default_OFFSET = CharacterCamera.DEFAULT_OFFSET;
                Vector2 zero = Vector2.zero;
                Vector2 zero2 = Vector2.zero;
                Rect splitRect = new Rect(0f, 0f, 0f, 0f);
                RawImage rawImage = (!self.RenderInImage) ? null : GameDisplayInUI.Instance.Screens[i];
                float foV;
                if (privatePart.Count == 1)
                {
                    splitRect.position = Vector2.zero;
                    splitRect.size = Vector2.one;
                    foV = OptionManager.Instance.GetFoVSolo(i);
                    if (self.RenderInImage)
                    {
                        rawImage.rectTransform.localScale = Vector3.one;
                        GameDisplayInUI.Instance.Screens[1].gameObject.SetActive(false);
                    }

                    GameDisplayInUI.Instance.SetMultiDisplayActive(false);
                }
                else
                {
                    if (privatePart.Count != 2)
                    {
                        throw new NotImplementedException("Support for more than 2 players is not implemented.");
                    }

                    int num = i + 1;
                    if (self.RenderInImage)
                    {
                        splitRect.position = ((i != 0) ? new Vector2(0.5f, 0f) : Vector2.zero);
                        splitRect.size = new Vector2(0.5f, 1f);
                    }
                    else
                    {
                        splitRect.position = new Vector2(0.5f * (float) ((i != 0) ? 1 : -1), 0f);
                        splitRect.size = Vector2.one;
                    }

                    foV = OptionManager.Instance.GetFoVSplit(i);
                    default_OFFSET.z = -2.5f;
                    zero2.x = -0.5f;
                    zero.x = (float) ((num % 2 != 1) ? -1 : 1) * 0.5f;
                    if (self.RenderInImage)
                    {
                        GameDisplayInUI.Instance.Screens[1].gameObject.SetActive(true);
                    }
                }

                CameraSettings cameraSettings;
                cameraSettings.FoV = foV;
                cameraSettings.SplitRect = splitRect;
                cameraSettings.Offset = default_OFFSET;
                cameraSettings.CameraDepth = 2 * i;
                cameraSettings.Image = rawImage;
                splitPlayer.RefreshSplitScreen(zero, zero2, cameraSettings);
            }

            if (moveGlobalUiToPlayer1)
            {
                Vector2 zero3 = Vector2.zero;
                Vector2 zero4 = Vector2.zero;
                if (self.LocalPlayers.Count == 2)
                {
                    zero4.x = -0.5f;
                    zero3.x = 0.5f;
                }

                Vector2 vector = Vector2.Scale(zero4, MenuManager.Instance.ScreenSize);
                Vector2 anchoredPosition = Vector2.Scale(zero3, vector);
                LoadingFade privatePart2 = Utils.GetPrivatePart<LoadingFade, MenuManager>(MenuManager.Instance, "m_masterLoading");
                if (privatePart2 != null)
                {
                    RectTransform componentInChildren = privatePart2.GetComponentInChildren<RectTransform>();
                    if (componentInChildren != null)
                    {
                        componentInChildren.sizeDelta = vector;
                        componentInChildren.anchoredPosition = anchoredPosition;
                    }
                }

                ProloguePanel privatePart3 = Utils.GetPrivatePart<ProloguePanel, MenuManager>(MenuManager.Instance, "m_prologueScreen");
                if (privatePart3 != null)
                {
                    RectTransform rectTransform = privatePart3.RectTransform;
                    rectTransform.sizeDelta = vector;
                    rectTransform.anchoredPosition = anchoredPosition;
                }
            }
        }
    }
}
