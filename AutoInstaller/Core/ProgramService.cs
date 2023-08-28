namespace Core;

public class ProgramService
{
    private AISLScriptBuilder _scriptBuilder = new AISLScriptBuilder();
    private string _folderPath = @"C:\Users\sziba\Desktop\Programs"; //this has to be changed
    public void SaveProgram(ProgramData programData)
    {
        string programPath = _folderPath + "\\" + programData.Name;
        if (!Directory.Exists(programPath))
        {
            Directory.CreateDirectory(programPath);
        }

        string filePath = programPath + $"\\ {programData.Version}" + ".aisl";

        
        FileStream fs = File.Create(filePath);


    }
}