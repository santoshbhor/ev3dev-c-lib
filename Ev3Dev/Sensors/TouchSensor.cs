using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ev3DevLib.Sensors
{
    //Last Updated on 8.4.2018 (DD/MM/YYYY)
    public class TouchSensor : InPort
    {
        public LegoSensor PORT { get; internal set; }
        public bool IsPressed { get { return (ReadVar("value0") == "1") ? true : false; } }

        public string RootToDir { get; internal set; }
        public string[] _Options;
        public override string[] Options => _Options;

        private string ReadVar(string var)
        {
            return IO.ReadValue(RootToDir + "/" + var);
        }
        private void WriteVar(string var, string value)
        {
            IO.WriteValue(RootToDir + "/" + var, value);
        }

        public override void ExecuteWriteOption(string Option, string[] Args)
        {
            switch (Option)
            {
                case ("IsPressed"):
                    throw new InvalidOperationException("ReadOnly");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public override string ExecuteReadOption(string Option)
        {
            switch(Option)
            {
                case ("IsPressed"):
                    return (IsPressed) ? "True" : "False";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public TouchSensor(Device D) : base(D)
        {
            RootToDir = D.RootToDir;
            if (D._type == DeviceType.lego_ev3_Touch)
                if (ReadVar("modes") != "TOUCH")
                    throw new InvalidOperationException("this device is not a touch sensor if please notify me on git");
            _Options = new string[] { "IsPressed" };
            PORT = new LegoSensor(D);
        }
    }
}
