using IPA;
using IPA.Config.Stores;
using HarmonyLib;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;
using VolumeMixer.Installers;
using VolumeMixer.Configuration;
using System.Reflection;

namespace VolumeMixer
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Harmony harmony;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, IPA.Config.Config config, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            PluginConfig.Instance = config.Generated<PluginConfig>();

            harmony = new Harmony("nmkmn.BeatSaber.VolumeMixer");

            zenjector.UseLogger(logger);
            zenjector.Install<MenuInstaller>(Location.Menu);
        }

        [OnEnable]
        public void OnEnable()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony.UnpatchSelf();
        }
    }
}
