namespace ProjectDepsVisualizer.Core.Exporters
{
  public interface IProjectDependenciesModelExporter
  {
    void Export(ProjectDependenciesModel projectDependenciesModel, string filePath);

    string ExportFormatName { get; }
    
    string ExportedFileExtension { get; }
  }
}
