using SiraUtil.Affinity;
using VolumeMixer.Configuration;
using VolumeMixer.UI;

namespace VolumeMixer.HarmonyPatches
{
    internal class UIPatch : IAffinity
    {
        private readonly UIWindow window;
        private bool debounce = false;

        public UIPatch(UIWindow uiw)
        {
            window = uiw;
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(MainMenuViewController), "DidActivate")]
        internal void Postfix()
        {
            if (debounce == false)
            {
                window.CreateFloatingScreen(PluginConfig.Instance.UIPosition, PluginConfig.Instance.UIRotation);
                debounce = true;
            }
        }
    }
}