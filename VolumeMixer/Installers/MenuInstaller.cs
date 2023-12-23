using VolumeMixer.HarmonyPatches;
using VolumeMixer.UI;
using Zenject;

namespace VolumeMixer.Installers
{
    internal class MenuInstaller : Installer<MenuInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<AudioController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<UIWindow>().AsSingle();
            Container.BindInterfacesTo<UIPatch>().AsSingle();
        }
    }
}