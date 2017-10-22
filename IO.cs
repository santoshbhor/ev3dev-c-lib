using System.IO;

namespace Ev3Dev
{
    public static class IO
    {
        public static string ReadValue(string path)
        {
            if(File.Exists(path))
            {
                using (StreamReader R = new StreamReader(new FileStream(path, FileMode.Open)))
                {
                    return R.ReadToEnd();
                }
            }else throw new FileNotFoundException();
        }
        public static void WriteValue(string path,string value)
        {
            if (File.Exists(path))
            {
                using (StreamWriter W = new StreamWriter(new FileStream(path, FileMode.Open)))
                {
                    W.BaseStream.SetLength(0);//null file
                    W.Write(value);//make file only this value
                }
            }
            else throw new FileNotFoundException();
        }
    }
}
