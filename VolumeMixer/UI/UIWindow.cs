using BeatSaberMarkupLanguage.FloatingScreen;
using UnityEngine;
using HMUI;
using VolumeMixer.Configuration;

namespace VolumeMixer.UI
{
    internal class UIWindow
    {
        private readonly AudioController audioController;
        public static FloatingScreen floatingScreen = null;

        public UIWindow(AudioController ac)
        {
            audioController = ac;
        }

        public void CreateFloatingScreen(Vector3 pos, Quaternion rot)
        {
            floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(150.0f, 25.0f), true, pos, rot);
            floatingScreen.SetRootViewController(audioController, ViewController.AnimationType.None);
            floatingScreen.gameObject.name = "VolumeMixer";

            floatingScreen.HandleSide = FloatingScreen.Side.Bottom;
            floatingScreen.ShowHandle = PluginConfig.Instance.ShowHandle;
            floatingScreen.HighlightHandle = PluginConfig.Instance.ShowHandle; // Why does this setter change ShowHandle...? BSML bug
            floatingScreen.handle.transform.localScale = Vector3.one * 5.0f;
            floatingScreen.handle.transform.localPosition = new Vector3(0.0f, -8.0f, 0.0f);
            floatingScreen.HandleReleased += OnHandleReleased;
            floatingScreen.gameObject.SetActive(PluginConfig.Instance.Enabled);

            audioController.transform.localScale = Vector3.one;
            audioController.transform.localEulerAngles = Vector3.zero;
            audioController.gameObject.SetActive(true);
        }

        private void OnHandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            if (floatingScreen.handle.transform.position.y < 0)
            {
                floatingScreen.transform.position += new Vector3(0.0f, 0.5f, 0.0f);
            }

            PluginConfig.Instance.UIPosition = floatingScreen.transform.position;
            PluginConfig.Instance.UIRotation = floatingScreen.transform.rotation;
        }
    }
}