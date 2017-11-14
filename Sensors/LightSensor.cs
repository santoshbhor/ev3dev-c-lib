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
        public int Value { get { if (Mode == LightSensor_mode.RGB_RAW || Mode == LightSensor_mode.COL_COLOR || Mode == LightSensor_mode.REF_RAW || Mode == LightSensor_mode.COL_CAL) throw new InvalidOperationException(); else return int.Parse(ReadVar("value0")); } }
        public LightSensor_Color Color { get { if (Mode != LightSensor_mode.COL_COLOR) throw new InvalidOperationException(); else return Value_To_LightSensor_Color(int.Parse(ReadVar("value0"))); } }
        public short[] RGB { get { if (Mode != LightSensor_mode.RGB_RAW) throw new InvalidOperationException(); else return new short[] { short.Parse(ReadVar("value0")), short.Parse(ReadVar("value1")), short.Parse(ReadVar("value2")) }; } }
        public short[] REF_RAWValue { get { if (Mode != LightSensor_mode.REF_RAW) throw new InvalidOperationException(); else return new short[] { short.Parse(ReadVar("value0")), short.Parse(ReadVar("value0")) }; } }

        public LightSensor_mode Mode { get { return String_To_LightSensor_Mode(ReadVar("mode")); } }

        public string LightSensor_Color_To_String(LightSensor_Color x)
        {
            switch (x)
            {
                case (LightSensor_Color.Black):
                    return "Black";
                case (LightSensor_Color.Blue):
                    return "Blue";
                case (LightSensor_Color.Brown):
                    return "Brown";
                case (LightSensor_Color.Green):
                    return "Green";
                case (LightSensor_Color.NoColor):
                    return "NoColor";
                case (LightSensor_Color.Red):
                    return "Red";
                case (LightSensor_Color.White):
                    return "White";
                case (LightSensor_Color.Yellow):
                    return "Yellow";
                default:
                    throw new Exception();
            }
        }
        public LightSensor_Color String_To_LightSensor_Color(string x)
        {
            switch (x)
            {
                case ("Black"):
                    return LightSensor_Color.Black;
                case ("Blue"):
                    return LightSensor_Color.Blue;
                case ("Brown"):
                    return LightSensor_Color.Brown;
                case ("Green"):
                    return LightSensor_Color.Green;
                case ("NoColor"):
                    return LightSensor_Color.NoColor;
                case ("Red"):
                    return LightSensor_Color.Red;
                case ("White"):
                    return LightSensor_Color.White;
                case ("Yellow"):
                    return LightSensor_Color.Yellow;
                default:
                    throw new Exception();
            }
        }
        public LightSensor_Color Value_To_LightSensor_Color(int x)
        {
            switch (x)
            {
                case (0):
                    return LightSensor_Color.NoColor;
                case (1):
                    return LightSensor_Color.Black;
                case (2):
                    return LightSensor_Color.Blue;
                case (3):
                    return LightSensor_Color.Green;
                case (4):
                    return LightSensor_Color.Yellow;
                case (5):
                    return LightSensor_Color.Red;
                case (6):
                    return LightSensor_Color.White;
                case (7):
                    return LightSensor_Color.Brown;
                default:
                    throw new ArgumentOutOfRangeException(x + " is not a known color id");
            }
        }

        public LightSensor_mode String_To_LightSensor_Mode(string x)
        {
            switch (x)
            {
                case ("COL-REFLECT"):
                    return LightSensor_mode.COL_REFLECT;
                case ("COL-AMBIENT"):
                    return LightSensor_mode.COL_AMBIENT;
                case ("COL-COLOR"):
                    return LightSensor_mode.COL_COLOR;
                case ("REF-RAW"):
                    return LightSensor_mode.REF_RAW;
                case ("RGB-RAW"):
                    return LightSensor_mode.RGB_RAW;
                case ("COL-CAL"):
                    return LightSensor_mode.COL_CAL;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public string LightSensor_Mode_To_String(LightSensor_mode x)
        {
            switch (x)
            {
                case (LightSensor_mode.COL_REFLECT):
                    return "COL_REFLECT";
                case (LightSensor_mode.COL_AMBIENT):
                    return "COL_AMBIENT";
                case (LightSensor_mode.COL_COLOR):
                    return "COL_COLOR";
                case (LightSensor_mode.REF_RAW):
                    return "REF_RAW";
                case (LightSensor_mode.RGB_RAW):
                    return "RGB_RAW";
                case (LightSensor_mode.COL_CAL):
                    return "COL_CAL";
                default:
                    throw new ArgumentOutOfRangeException();
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
                    throw new InvalidOperationException("this device is not a LightSensor if it is and is failing to detect it then please notify me on git");
            PORT = new LegoSensor(D);
        }

        public void ChangeMode(LightSensor_mode mode)
        {
            WriteVar("mode", LightSensor_Mode_To_String(mode));
        }
    }
}