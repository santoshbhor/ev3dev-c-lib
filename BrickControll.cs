using System;
using System.Threading;
using System.IO;

namespace Ev3Dev
{
    public struct LCDIMG//for BrickControll.LCD.Graphics
    {
        public byte[] Buffer;
        public int LineLength;
    }

    //NO Updates within a static class
    //this is only for controll and on point information
    //ie we read as information is given
    public static class BrickControll
    {
        public static void LoadPorts()
        {
            InputPorts.In1 = new Device() { RootToDir = "/sys/class/lego-port/port0" };
            InputPorts.In2 = new Device() { RootToDir = "/sys/class/lego-port/port1" };
            InputPorts.In3 = new Device() { RootToDir = "/sys/class/lego-port/port2" };
            InputPorts.In4 = new Device() { RootToDir = "/sys/class/lego-port/port3" };
            OutputPorts.OutA = new Device() { RootToDir = "/sys/class/lego-port/port4" };
            OutputPorts.OutB = new Device() { RootToDir = "/sys/class/lego-port/port5" };
            OutputPorts.OutC = new Device() { RootToDir = "/sys/class/lego-port/port6" };
            OutputPorts.OutD = new Device() { RootToDir = "/sys/class/lego-port/port7" };

            InputPorts.In1 = UpdateDev(InputPorts.In1, "in1");
            InputPorts.In2 = UpdateDev(InputPorts.In2, "in3");
            InputPorts.In3 = UpdateDev(InputPorts.In3, "in2");
            InputPorts.In4 = UpdateDev(InputPorts.In4, "in4");

            OutputPorts.OutA = UpdateDev(OutputPorts.OutA, "outA");
            OutputPorts.OutB = UpdateDev(OutputPorts.OutB, "outB");
            OutputPorts.OutC = UpdateDev(OutputPorts.OutC, "outC");
            OutputPorts.OutD = UpdateDev(OutputPorts.OutD, "outD");
        }
        private static Device UpdateDev(Device D, string Addr)
        {
            if (D.RootToDir != null)
                if (D.RootToDir.StartsWith("/sys/class/lego-port/port"))
                {
                    string r = D.RootToDir;
                    string[] Directorys = Directory.GetDirectories(r);
                    if (Directorys.Length > 0)
                    {
                        int x = 0;
                        for (; x < Directorys.Length; x++)
                        {
                            if (Directorys[x].Contains(":"))
                                break;
                        }
                        if (x < Directorys.Length)
                        {
                            r = Directorys[x];
                            if (r.Contains(":"))
                            {
                                Directorys = Directory.GetDirectories(r);
                                x = 0;
                                for (; x < Directorys.Length; x++)
                                    for (int y = 0; y < Ev3Dev.Classes.Length; y++)
                                    {
                                        if (Directorys[x].Substring(Directorys[x].LastIndexOf("/") + 1) + "/" == Ev3Dev.Classes[y])
                                        {
                                            D._type = Ev3Dev.String_To_DeviceType(Ev3Dev.Classes[y]);
                                            r = Directorys[x];
                                            break;
                                        }
                                    }
                                Directorys = Directory.GetDirectories("/sys/class/" + Ev3Dev.DeviceType_To_String(D._type));
                                for (x = 0; x < Directorys.Length; x++)
                                {
                                    if (ReadVar(Directorys[x] + "/address") == Addr)
                                    {
                                        D.RootToDir = Directorys[x];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            return D;
        }
        private static string ReadVar(string path)
        {
            return IO.ReadValue(path);
        }

        //TODO add some Controlls for the Brick
        public static class Battery//info on the battery
        {
            public static string CurrentFlow { get { return IO.ReadValue("/sys/class/power_supply/legoev3-battery/current_now"); } }
            public static string Type { get { return IO.ReadValue("/sys/class/power_supply/legoev3-battery/technology"); } }
            public static string voltage_max { get { return IO.ReadValue("/sys/class/power_supply/legoev3-battery/voltage_max_design"); } }
            public static string voltage_min { get { return IO.ReadValue("/sys/class/power_supply/legoev3-battery/voltage_min_design"); } }
            public static string voltage_now { get { return IO.ReadValue("/sys/class/power_supply/legoev3-battery/voltage_now"); } }
        }
        public static class LCD//LCD controlls **uses deffult linux graphical subsystem
        {
            public const string DevLocation = "/dev/fb0";
            private static FileStream FS;
            private static bool HOOKED = false;
            //TODO find out how to do a memorymap to LCD type device
            //then find out a way of locking the device for private use
            public static void DRAW()
            {
                if (HOOKED)
                    FS.Write(Graphics.IMG.Buffer, 0, Graphics.BufferSize);
                else
                    (new FileStream(DevLocation, FileMode.Open)).Write(Graphics.IMG.Buffer, 0, Graphics.BufferSize);
            }

            public static bool HOOK()
            {
                try
                {
                    FS = new FileStream(DevLocation, FileMode.Open);
                    HOOKED = true;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            public static bool UNHOOK()
            {
                try
                {
                    FS.Close();
                    HOOKED = false;
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static class Graphics
            {
                internal static LCDIMG IMG;
                public static int LineLength { get { return X; } }
                public static int BufferSize { get; private set; }
                private static int X, Y;

                public static void WriteBuffer(LCDIMG img)
                {
                    if (img.Buffer.Length != BufferSize) throw new ArgumentOutOfRangeException();
                    else IMG = img;
                }

                /*format Data
                 * 1: X -4byte int32
                 * 2: BS -4byte int32
                 * 3: DATA
                 */
                public static LCDIMG LoadIMG(string path)
                {
                    FileStream FS = new FileStream(path, FileMode.Open);
                    byte[] buf = new byte[8];
                    FS.Read(buf, 0, 8);
                    LCDIMG img = new LCDIMG();
                    img.LineLength = BitConverter.ToInt32(buf, 0);
                    img.Buffer = new byte[BitConverter.ToInt32(buf, 4)];
                    FS.Read(img.Buffer, 0, img.Buffer.Length);
                    FS.Close();
                    return img;
                }
                public static void SaveIMG(string path, LCDIMG img)
                {
                    FileStream FS = new FileStream(path, FileMode.OpenOrCreate);
                    byte[] header = new byte[8];

                    BitConverter.GetBytes(img.LineLength).CopyTo(header, 0);
                    BitConverter.GetBytes(img.Buffer.Length).CopyTo(header, 4);

                    FS.Write(header, 0, 8);
                    FS.Write(img.Buffer, 0, img.Buffer.Length);

                    FS.Close();
                }

                public static void SetPixel(int y, int x, bool OnOff)
                {
                    if (x < 0 || x > X || y < 0 || y > Y) throw new ArgumentOutOfRangeException();

                    int IND = (y * LineLength) + x;
                    int BIND = (IND * 1) % 8;
                    IND = IND % 8;
                    IMG.Buffer[BIND] = SetBit(IMG.Buffer[BIND], IND, OnOff);
                }
                private static byte SetBit(byte b, int pos, bool value)
                {
                    byte Mask = (byte)(1 << pos);
                    byte c = b;
                    if (value)
                    {
                        c |= Mask;
                    }
                    else
                    {
                        c &= (byte)~Mask;
                    }
                    return c;
                }

                internal static class LCDINFO
                {
                    internal static int Hight = 0, Width = 0;
                    internal static int DataSize = 1;//1 = STD
                    internal static bool PrivateUse = false;
                }

                public static void INIT()
                {
                    var R = TermController.RunCommand("fbset", "-i");
                    Thread.Sleep(100);
                    while (true)
                    {

                        string line = "";
                        try
                        {
                            line = R.ReadLine();
                        }
                        catch
                        {
                            throw new MissingFieldException();
                        }

                        if (line.StartsWith("mode \""))
                        {
                            string[] info = line.Substring(6).Split(new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);
                            info[1].Substring(0, info[1].Length - 1);
                            X = int.Parse(info[0]);
                            Y = int.Parse(info[1]);
                            BufferSize = X * Y;
                            IMG = new LCDIMG() { Buffer = new byte[BufferSize], LineLength = X };
                            break;
                        }
                    }
                    R.Close();
                }
            }

            /*gen info on LCD
             * fbset -i ::returns info on LCD
             * Image data is Written to 'LCD.DevLocation' as type File
             * Image Format : bw indexer
             * On 'Geometry ? ? ? ? x' by 'fbset -i' = num of bit per pixel
             * ^^Ie xnum of bits on = pixel is on
             * ^^this can be used as a bool array
             * ^^and a custom Graphics unit to then
             * ^^controll the img data
             */
        }
        public static class Speeker//Speekers Controlls
        {
            public static void Beep()
            {
                PlayTone(1000, 1);
            }
            public static void PlayTone(int hz, int timeMS)
            {
                TermController.RunCommand("beep", $"-f {hz} -l {timeMS}");
            }
            public static void Say(string text)
            {
                TermController.RunCommandWithPipe(TermController.RunCommand("espeak", $"\"{text}\" --stdout"), "aplay", "");
            }
            public static void PlayWav(string path)
            {
                if (path.Contains(".wav"))
                    TermController.RunCommand("aplay", path);
                else throw new ArgumentOutOfRangeException();
            }
        }
        public static class BrickInput//the leds/buttons on the birck its self
        {
            public const string KeyMapFile = "??";
            public static class KeyMaps
            {
                //STD values for brand new un edited device settings
                //for new after i known how to decode KeyMapFile
                //ill create a function to update the static values
                public static byte Back = 14;
                public static byte Center = 28;
                public static byte Up = 103;
                public static byte Left = 105;
                public static byte Right = 106;
                public static byte Down = 108;
            }

            //TODO findout how to controll brick leds 

        }
        public static class InputPorts//all Inputports ... so the devices them self's
        {
            //do descovery then display devices here
            //warning hot swaping devices will not work as expected
            //if i dont update input every so often
            //mnt points 'In<1-4>:' 

            //WARNING UNTESTED
            public static Device In1 { get; internal set; }
            public static Device In2 { get; internal set; }
            public static Device In3 { get; internal set; }
            public static Device In4 { get; internal set; }
        }
        public static class OutputPorts//all Outputports ... so the devices them self's
        {
            //do descovery then display devices here
            //warning hot swaping devices will not work as expected
            //if i dont update input every so often
            //mnt points 'Out<A-D>:'

            //WARNING UNTESTED
            public static Device OutA { get; internal set; }
            public static Device OutB { get; internal set; }
            public static Device OutC { get; internal set; }
            public static Device OutD { get; internal set; }

        }
    }
}
