
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using VolumeMixer.UI;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace VolumeMixer.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool Enabled { get; set; } = true;
        public virtual bool ShowHandle { get; set; } = true;
        public virtual Vector3 UIPosition { get; set; } = new Vector3(2f, 0.2f, 2.8f);
        public virtual Quaternion UIRotation { get; set; } = Quaternion.Euler(new Vector3(8, 33, 0));

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do nothing
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            if (UIWindow.floatingScreen != null)
            {
                if (Enabled == true)
                {
                    UIWindow.floatingScreen.ShowHandle = ShowHandle;
                    AudioController.Show();
                }
                else
                {
                    AudioController.Hide();
                }
            }
        }
    }
}
