using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class InfoLogger
{
    private const string logFileAddress = "D:\\Games\\Castlevania\\custom-sotn\\sotn-api\\SotnApi\\custom-sotn-app\\debug\\info.ids";

    public static void Log(List<string> targets)
    {
        FileStream stream = File.Create(logFileAddress);
        StreamWriter writer = new StreamWriter(stream);

        for (int i = 0; i < targets.Count; i++)
        {
            writer.WriteLine(targets[i]);
        }

        writer.Dispose();
        stream.Dispose();
    }

    public static List<string> GetLog()
    {
        FileStream stream;

        if (!File.Exists(logFileAddress))
            stream = File.Create(logFileAddress);
        else
            stream = File.Open(logFileAddress, FileMode.Open);

        StreamReader reader = new StreamReader(stream);

        List<string> ids = new List<string>();

        while(!reader.EndOfStream)
        {
            ids.Add(reader.ReadLine());
        }

        reader.Dispose();
        stream.Dispose();

        return ids;
    }
}