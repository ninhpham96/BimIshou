using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimIshou.Commands;

namespace BimIshou.Services
{
    class ProjectInfoService : IProjectInfoService
    {
        Document Document;
        public ProjectInfoService(UIApplication uiapp) 
        {
            Document = uiapp.ActiveUIDocument.Document;
        }

        public string GetProjectInfo(string Guid, string name)
        {
            return Document.ProjectInformation.LoadEntity<string>(Guid.GetGuid(), name);
        }
    }

    public interface IProjectInfoService
    {
        public string GetProjectInfo(string Guid, string name);
    }
}
