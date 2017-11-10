using System;
using System.Threading;

namespace Ev3Dev.Misc
{
    //added on 21.10.2017 (DD/MM/YYYY)
    public enum LegoPort_Args
    {
        mode,
        set_device
    }
    //--------------------------------

    //this should ONLY be used by knowing users
    //so if you dont know how to use this DONT
    public class LegoPort
    {
        //values
        //my Rule not matter what all of the values must be updated inside UPDATE
        //unless there NON-READ ie 'onlywriteable'(w)
        public string Address { get { try { return ReadVar("address"); } catch { return "N/A"; } } }
        public string DriverName { get { try { return ReadVar("driver_name"); } catch { return "N/A"; } } } 
        public string Modes { get { try { return ReadVar("modes"); } catch { return "N/A"; } } }
        public string Mode { get { try { return ReadVar("mode"); } catch { return "N/A"; } } }
        public string Status { get { try { return ReadVar("status"); } catch { return "N/A"; } } }
        public string LastKnownDeviceType { get; private set; }//NON-READ

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
            if (var == "set_device")
            {
                LastKnownDeviceType = var;
            }
        }

        //constructor
        public LegoPort(Device dev)
        {
            if (dev._type != DeviceType.lego_port)
                throw new InvalidOperationException("this device is not a tachno motor");

            RootToDir = dev.RootToDir;

            if (RootToDir.Contains("lego-port"))
                MountPoint = "??";//ReadVar("address");
            else if (RootToDir.Contains(":"))
                MountPoint = RootToDir;
            else throw new InvalidOperationException("this uses the wrong class please re initulize the device and then try agen");

            Options = new string[] {// r=readonly | rw=read+write | w=writeonly
                    "address:r", "driver_name:r", "modes:r", "set_device:w", "status:r"
                };
        }

        //functions
        public void SetMode(string mode)
        {
            if (TestMode(mode))
            {
                WriteVar("mode", mode);
            }
        }
        public void SetDeviceType(DeviceType x)
        {
            WriteVar("set_device", Ev3Dev.DeviceType_To_String(x));
        }

        //safty functions
        private bool TestMode(string value)
        {
            string[] modes = Modes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string x in modes)
                if (value == x)
                    return true;
            return false;
        }

        //hands on for more advanced users
        public void SetArg(string value,LegoPort_Args x)
        {
            if(x == LegoPort_Args.mode)
            {
                if(TestMode(value))
                {
                    WriteVar("mode", value);
                }
            }
            else//set_device
            {
                WriteVar("set_device", value);
            }
        }
    }
}
