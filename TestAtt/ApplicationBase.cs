using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.Helpers;
using System.ComponentModel;
using System.Reflection;

namespace BimIshou.TestAtt;

public abstract class ApplicationBase : IExternalApplication
{
    private UIApplication _uiApplication;

    public Result Result { get; set; }

    public UIControlledApplication Application { get; private set; }

    public UIApplication UiApplication => _uiApplication ?? (_uiApplication = (UIApplication)(Application?.GetType().GetField("m_uiapplication", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Application)));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Result OnStartup(UIControlledApplication application)
    {
        Application = application;
        AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyOnStartup;
        try
        {
            OnStartup();
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssemblyOnStartup;
        }

        return Result;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Result OnShutdown(UIControlledApplication application)
    {
        AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyOnShutdown;
        try
        {
            OnShutdown();
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssemblyOnShutdown;
        }

        return Result.Succeeded;
    }

    public abstract void OnStartup();

    public virtual void OnShutdown()
    {
    }

    private Assembly ResolveAssemblyOnStartup(object sender, ResolveEventArgs args)
    {
        return ResolveHelper.ResolveAssembly(sender,args);
    }

    private Assembly ResolveAssemblyOnShutdown(object sender, ResolveEventArgs args)
    {
        return ResolveHelper.ResolveAssembly(sender,args);
    }
}