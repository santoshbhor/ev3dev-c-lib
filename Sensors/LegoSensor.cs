using System;
using System.Threading;
using Ev3Dev;
using System.IO;

namespace Ev3Dev.Sensors
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
        public string TextValue { get; private set; }
        public int[] Values { get; private set; }
        public string Units { get; private set; }
        public int Poll { get; private set; }
        public bool PollSupported { get; private set; }
        public int NumValues { get; private set; }
        public string Modes { get; private set; }
        public string Mode { get; private set; }
        public string FWVersion { get; private set; }
        public string DriverName { get; private set; }
        public int Decimals { get; private set; }
        public byte[] Direct { get; private set; }
        public bool DirectSupported { get; private set; }
        public string Commands { get; private set; }
        public string LastCommand { get; private set; }//NON-READ
        public bool CommandsSupported { get; private set; }
        public LegoSensor_BinFormats BinDataFormat { get; private set; }
        public byte[] BinData { get; private set; }
        public string Address { get; private set; }

        //my members
        public bool NoDeley = false;//this says if we should NOT sleep while updating args
        public int Deley_MS = 30;//this is the time to sleep if !NoDeley

        private Thread UpdateTHR;
        private void UPDATE()
        {
            while (true)
            {
                DirectSupported = (ReadVar("direct") == "-EOPNOTSUPP") ? false:true;
                CommandsSupported = (ReadVar("commands") == "-EOPNOTSUPP") ? false : true;
                PollSupported = (ReadVar("poll_ms") == "-EOPNOTSUPP") ? false : true;
                FWVersion = ReadVar("fw_version");
                DriverName = ReadVar("driver_name");
                Mode = ReadVar("mode");
                Modes = ReadVar("modes");
                NumValues = int.Parse(ReadVar("num_values"));
                Decimals = int.Parse(ReadVar("decimals"));
                BinDataFormat = String_To_LegoSensor_BinFormats(ReadVar("bin_data_format"));
                Address = ReadVar("address");
                Units = ReadVar("units");
                TextValue = ReadVar("text_value");

                if (PollSupported) Poll = int.Parse(ReadVar("poll_ms"));
                if (CommandsSupported) Commands = ReadVar("commands");
                if (NumValues > 0)
                {
                    Values = new int[NumValues];
                    for(int x = 0;x < NumValues; x++)
                    {
                        Values[x] = int.Parse(ReadVar("value" + x));
                    }
                }
                if (!NoDeley) Thread.Sleep(Deley_MS);
            }
        }

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
            if (dev._type != DeviceType.dc_motor)
                throw new InvalidOperationException("this device is not a tachno motor");

            RootToDir = dev.RootToDir;
            UpdateTHR = new Thread(new ThreadStart(UPDATE));

            if (RootToDir.StartsWith("/sys/class/dc-motor/motor"))
                MountPoint = "??";//ReadVar("address");
            else if (RootToDir.Contains(":"))
                MountPoint = RootToDir;
            else throw new InvalidOperationException("this uses the wrong class please re initulize the device and then try agen");

            Options = new string[] {// r=readonly | rw=read+write | w=writeonly | b=byte output
                    "address:r",    "bin_data:r #b",    "bin_data_format:r",    "command:w",
                    "comands:r",    "direct:rw",        "decimals:r",           "driver_name:r",
                    "fw_version",   "mode:rw",          "modes:r",              "num_values:r",
                    "poll_ms:rw",   "units:r",          "value<0-9>:r",         "text_value"
                };
            UpdateTHR.Start();
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
