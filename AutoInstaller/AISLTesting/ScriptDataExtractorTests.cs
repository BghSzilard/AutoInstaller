using AISL;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISLTesting;

public class ScriptDataExtractorTests
{
    [Fact]
    public void GetProgramDataTest()
    {
        string path = "C:\\Users\\Rares\\Dropbox\\My PC (DESKTOP-P06UPBI)\\Desktop\\SummerSchooInstaller\\AutoInstaller\\AutoInstaller\\AISLTesting\\script.aisl";
        // make relative path
        ProgramData output = ScriptDataExtractor.GetProgramData(path);

        string name = output.Name;

        Assert.Equal(name, "Simcenter Test Cloud Blueprint");
    }
}
