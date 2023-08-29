using System.Collections.ObjectModel;

namespace Core;

public class ProgramData
{
    public string InstallationsPath { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public List<Parameter> Parameters { get; set; } = new List<Parameter>();
}
