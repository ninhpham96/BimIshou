using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;

namespace BimIshou
{
    public static class Hosting
    {
        private static ServiceProvider? _serviceProvider;
        public static void StartHosting(UIApplication uiapp)
        {
            var services = new ServiceCollection();
            services.AddSingleton(uiapp);
            services.AddSingleton(uiapp.ActiveUIDocument.Document);
            _serviceProvider = services.BuildServiceProvider();
        }
        public static void StopHosting()
        {
            _serviceProvider?.Dispose();
        }
        public static T? GetService<T>() where T : class
        {
            return _serviceProvider?.GetService<T>();
        }
    }
}
