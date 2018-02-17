using System.Threading;
using System.Windows;
using VDManager.Utils;
using VDManager.Utils.Interfaces;

namespace VDManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            RegisterInstance();
        }

        public static void RegisterInstance()
        {
            DependencyInjectionUtil.RegisterInstance<IDispatcher>(new Dispatcher(Thread.CurrentThread));
        }
    }
}
