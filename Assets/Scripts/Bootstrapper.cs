using Assets.Scripts;

public class Bootstrapper : BootstrapperBase {
    Bootstrapper() {
        Container.Singleton<MainMenu>();
        Container.Singleton<SettingsMenu>();
        Container.Singleton<PlayerMenu>();
        Container.Singleton<Introduction>();
        Container.RegisterSingleton(typeof(Conductor), "Conductor", typeof(Conductor));

        // these are child datacontexts that we use in the playermenu:
        Container.RegisterPerRequest(typeof (ClassSetViewModel), "ClassSetViewModel", typeof(ClassSetViewModel));
    }
}