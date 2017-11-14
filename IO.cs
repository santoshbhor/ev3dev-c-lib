using System.IO;
using System;

namespace Ev3DevLib
{
    public static class IO
    {
        public static string ReadValue(string path)
        {
            if (Ev3Dev.DebuggText)
                Console.WriteLine("Reading File:" + path);

            if(File.Exists(path))
            {
                using (StreamReader R = new StreamReader(File.OpenRead(path)))
                {
                    return R.ReadToEnd().Replace("\n","");
                }
            }else throw new FileNotFoundException();
        }
        public static void WriteValue(string path,string value)
        {
            if (Ev3Dev.DebuggText)
                Console.WriteLine("Writting (" + value + ") to " + path);
            if (File.Exists(path))
            {
                using (StreamWriter W = new StreamWriter(File.OpenWrite(path)))
                {
                    W.BaseStream.SetLength(0);//null file
                    W.Write(value);//make file only this value
                }
            }
            else throw new FileNotFoundException();
        }
    }
}
