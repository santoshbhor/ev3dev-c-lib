using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ev3DevLib.Sensors
{
    public class TouchSensor
    {
        public LegoSensor PORT { get; internal set; }
        public bool IsPressed { get { return (ReadVar("value0") == "1") ? true : false; } }

        public string RootToDir { get; internal set; }

        private string ReadVar(string var)
        {
            return IO.ReadValue(RootToDir + "/" + var);
        }
        private void WriteVar(string var, string value)
        {
            IO.WriteValue(RootToDir + "/" + var, value);
        }

        public TouchSensor(Device D)
        {
            RootToDir = D.RootToDir;
            if (D._type == DeviceType.lego_ev3_Touch)
                if (ReadVar("modes") != "TOUCH")
                    throw new InvalidOperationException("this device is not a touch sensor if please notify me on git");
            PORT = new LegoSensor(D);
        }
    }
}
