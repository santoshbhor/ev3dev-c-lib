using System;
using System.Threading;
using Ev3DevLib;
using System.IO;

namespace Ev3DevLib.Sensors
{
    //added on 21.10.2017 (DD/MM/YYYY)
    public enum LegoSensor_Args
    {
        command,
        direct,
        mode,
        poll_ms
    }
    public enum LegoSensor_BinFormats
    {
        Byte,
        SByte,
        UShort,
        Short,
        Short_BigEndian,
        Int,
        Int_BigEndian,
        Float,
    }
    //--------------------------------
    public class LegoSensor
    {
        //values
        //my Rule not matter what all of the values must be updated inside UPDATE
        //unless there NON-READ ie 'onlywriteable'(w)
        public int[] Values                             { get { try { int[] Vs = new int[NumValues]; for (int x = 0; x < NumValues; x++){ Vs[x] = int.Parse(ReadVar("value" + x)); };return Vs; } catch { return null; } } }
        public string TextValue                         { get { try { return ReadVar("text_value"); } catch { return "N/A"; } } }
        public string Units                             { get { try { return ReadVar("units"); } catch { return "N/A"; } } }
        public int Poll                                 { get { try { return int.Parse(ReadVar("poll_ms")); } catch { return -1; } } }
        public bool PollSupported                       { get { try { return (ReadVar("poll_ms") == "-EOPNOTSUPP") ? false : true; } catch { return false; } } }
        public int NumValues                            { get { try { return int.Parse(ReadVar("num_values")); } catch { return -1; } } }
        public string Modes                             { get { try { return ReadVar("modes"); } catch { return "N/A"; } } }
        public string Mode                              { get { try { return ReadVar("mode"); } catch { return "N/A"; } } }
        public string FWVersion                         { get { try { return ReadVar("fw_version"); } catch { return "N/A"; } } }
        public string DriverName                        { get { try { return ReadVar("driver_name"); } catch { return "N/A"; } } }
        public int Decimals                             { get { try { return int.Parse(ReadVar("decimals")); } catch { return -1; } } }
        public bool DirectSupported                     { get { try { return (ReadVar("direct") == "-EOPNOTSUPP") ? false : true; } catch { return false; } } }
        public string Commands                          { get { try { return ReadVar("commands"); } catch { return "N/A"; } } }
        public string LastKnownCommand                  { get; private set; }//NON-READ
        public bool CommandsSupported                   { get { try { return (ReadVar("commands") == "-EOPNOTSUPP") ? false : true; ; } catch { return false; } } }
        public LegoSensor_BinFormats BinDataFormat      { get { try { return String_To_LegoSensor_BinFormats(ReadVar("bin_data_format")); } catch { return LegoSensor_BinFormats.Int; } } }
        public string BinDataPath                       { get { return RootToDir + "/bin_data"; } }
        public string Address                           { get { try { return ReadVar("address"); } catch { return "N/A"; } } }
        
        //my header info
        public string RootToDir { get; private set; }
        public string MountPoint { get; private set; }
        public string[] Options { get; private set; }

        //base functions
        private string ReadVar(string var)
        {
            return IO.ReadValue(RootToDir + "/" + var);
        }
        private void WriteVar(string var, string value)
        {
            IO.WriteValue(RootToDir + "/" + var, value);
        }

        //constructor
        public LegoSensor(Device dev)
        {
            if(Ev3Dev.DebuggText)
                Console.WriteLine("Device Type Is:" + dev._type);

            if (dev._type != DeviceType.lego_ev3_UltraSound && dev._type != DeviceType.lego_ev3_Gyro && dev._type != DeviceType.lego_ev3_Light && dev._type != DeviceType.lego_ev3_Touch)
                throw new InvalidOperationException("this device is not a sensor");

            RootToDir = dev.RootToDir;
            if (RootToDir.Contains("lego-sensor"))
                MountPoint = ReadVar("address");
            else if (RootToDir.Contains(":"))
                MountPoint = RootToDir;
            else throw new InvalidOperationException("this uses the wrong class please re initulize the device and then try agen");

            Options = new string[] {// r=readonly | rw=read+write | w=writeonly | b=byte output
                    "address:r",    "bin_data:r #b",    "bin_data_format:r",    "command:w",
                    "comands:r",    "direct:rw",        "decimals:r",           "driver_name:r",
                    "fw_version",   "mode:rw",          "modes:r",              "num_values:r",
                    "poll_ms:rw",   "units:r",          "value<0-9>:r",         "text_value"
                };
        }

        //helpers
        public string LegoSensor_Args_To_String(LegoSensor_Args x)
        {
            switch(x)
            {
                case (LegoSensor_Args.command):
                    return "command";
                case (LegoSensor_Args.direct):
                    return "direct";
                case (LegoSensor_Args.mode):
                    return "mode";
                case (LegoSensor_Args.poll_ms):
                    return "poll_ms";

                default:
                    return "INVALID";
            }
        }
        public LegoSensor_Args String_To_LegoSensor_Args(string x)
        {
            switch(x)
            {
                case ("command"):
                    return LegoSensor_Args.command;
                case ("direct"):
                    return LegoSensor_Args.direct;
                case ("mode"):
                    return LegoSensor_Args.mode;
                case ("poll_ms"):
                    return LegoSensor_Args.poll_ms;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string LegoSensor_BinFormats_To_String(LegoSensor_BinFormats x)
        {
            switch(x)
            {
                case (LegoSensor_BinFormats.Byte):
                    return "u8";
                case (LegoSensor_BinFormats.SByte):
                    return "s8";
                case (LegoSensor_BinFormats.Float):
                    return "float";
                case (LegoSensor_BinFormats.Int):
                    return "s32";
                case (LegoSensor_BinFormats.Int_BigEndian):
                    return "s32_be";
                case (LegoSensor_BinFormats.Short):
                    return "s16";
                case (LegoSensor_BinFormats.Short_BigEndian):
                    return "s16_be";
                case (LegoSensor_BinFormats.UShort):
                    return "u16";

                default:
                    return "INVALID";
            }
        }
        public LegoSensor_BinFormats String_To_LegoSensor_BinFormats(string x)
        {
            switch (x)
            {
                case ("u8"):
                    return LegoSensor_BinFormats.Byte;
                case ("s8"):
                    return LegoSensor_BinFormats.SByte;
                case ("float"):
                    return LegoSensor_BinFormats.Float;
                case ("s32"):
                    return LegoSensor_BinFormats.Int;
                case ("s32_be"):
                    return LegoSensor_BinFormats.Int_BigEndian;
                case ("s16"):
                    return LegoSensor_BinFormats.Short;
                case ("s16_be"):
                    return LegoSensor_BinFormats.Short_BigEndian;
                case ("u16"):
                    return LegoSensor_BinFormats.UShort;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //functions
        //dont know if this works
        Stream GetDirectStream()
        {
            if (DirectSupported)
                return new FileStream(RootToDir + "/direct", FileMode.Open);
            else throw new InvalidOperationException();
        }

        //safty functions
        public bool TestArgs(string value, LegoSensor_Args x)
        {
            switch(x)
            {
                case (LegoSensor_Args.command):
                    if (CommandsSupported)
                    {
                        string[] commands = Commands.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int y = 0; y < commands.Length; y++)
                            if (commands[y] == value) return true;
                        return false;
                    }
                    else return false;
                        

                case (LegoSensor_Args.direct):
                    return false;

                case (LegoSensor_Args.mode):
                    string[] modes = Modes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int y = 0; y < modes.Length; y++)
                        if (modes[y] == value) return true;
                    return false;

                case (LegoSensor_Args.poll_ms):
                    int p;
                    if (int.TryParse(value, out p))
                    {
                        if (p > 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;

                default:
                    throw new ArgumentNullException();
            }
        }

        //hands on for more advanced users
        public void SetArg(string Value, LegoSensor_Args x)
        {
            if (x == LegoSensor_Args.direct)
                throw new InvalidOperationException("use the 'Stream' from 'GetDirectStream()' ");
            else if (TestArgs(Value, x))
            {
                WriteVar(LegoSensor_Args_To_String(x), Value);
            }
            else
                throw new ArgumentOutOfRangeException();
        }
    }
}
