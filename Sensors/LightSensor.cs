using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ev3Dev.Sensors
{
    public enum LightSensor_Color
    {
        NoColor,
        Black,
        Blue,
        Green,
        Yellow,
        Red,
        White,
        Brown,
    }
    public enum LightSensor_mode
    {
        COL_REFLECT,
        COL_AMBIENT,
        COL_COLOR,
        REF_RAW,
        RGB_RAW,
        COL_CAL
    }
    public class LightSensor
    {
        public LegoSensor PORT { get; internal set; }
        public string RootToDir { get; internal set; }


        public string LightSensor_Color_To_String(LightSensor_Color x)
        {
            switch(x)
            {
                default:
                    throw new Exception();
            }
        }


        private string ReadVar(string var)
        {
            return IO.ReadValue(RootToDir + "/" + var);
        }
        private void WriteVar(string var, string value)
        {
            IO.WriteValue(RootToDir + "/" + var, value);
        }

        public LightSensor(Device D)
        {
            RootToDir = D.RootToDir;
            if (D._type == DeviceType.lego_ev3_light)
                if (ReadVar("modes") != "COL-REFLECT COL-AMBIENT COL-COLOR REF-RAW RGB-RAW COL-CAL")
                    throw new InvalidOperationException("this device is not a touch sensor if please notify me on git");
            PORT = new LegoSensor(D);
        }
    }
}
