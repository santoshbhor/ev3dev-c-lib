using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ev3DevLib.Sensors
{
    //Last Updated on 8.4.2018 (DD/MM/YYYY)
    public enum GyroSensor_modes
    {
        GYRO_ANG,
        GYRO_RATE,
        GYRO_FAS,
        GYRO_G_and_A,
        GYRO_CAL,
        TILT_RATE,
        TILT_ANG,
    }
    public class GyroSensor : InPort
    {
        public LegoSensor PORT { get; internal set; }

        public int Value { get { if (Mode != GyroSensor_modes.GYRO_G_and_A && Mode != GyroSensor_modes.GYRO_CAL) return int.Parse(ReadVar("value0")); else throw new InvalidOperationException("wrong mode"); } }
        public int[] G_and_AValue { get { if (Mode == GyroSensor_modes.GYRO_G_and_A) return new int[] { int.Parse(ReadVar("value0")), int.Parse(ReadVar("value1")) }; else throw new InvalidOperationException("wrong mode"); } }
        public int[] CALValue { get { if(Mode == GyroSensor_modes.GYRO_CAL) return new int[] { int.Parse(ReadVar("value0")), int.Parse(ReadVar("value1")), int.Parse(ReadVar("value2")), int.Parse(ReadVar("value3")) }; else throw new InvalidOperationException("wrong mode"); } }

        public GyroSensor_modes Mode { get { return String_To_GyroSensor_modes(ReadVar("mode")); } }
        public string RootToDir { get; internal set; }
        private string[] _Options;
        public override string[] Options => _Options;

        private string ReadVar(string var)
        {
            return IO.ReadValue(RootToDir + "/" + var);
        }
        private void WriteVar(string var, string value)
        {
            IO.WriteValue(RootToDir + "/" + var, value);
        }

        public GyroSensor_modes String_To_GyroSensor_modes(string x)
        {
            switch(x)
            {
                case ("GYRO-ANG"):
                    return GyroSensor_modes.GYRO_ANG;
                case ("GYRO-CAL"):
                    return GyroSensor_modes.GYRO_CAL;
                case ("GYRO-FAS"):
                    return GyroSensor_modes.GYRO_FAS;
                case ("GYRO-G&A"):
                    return GyroSensor_modes.GYRO_G_and_A;
                case ("GYRO-RATE"):
                    return GyroSensor_modes.GYRO_RATE;
                case ("TILT-ANG"):
                    return GyroSensor_modes.TILT_ANG;
                case ("TILT-RATE"):
                    return GyroSensor_modes.TILT_RATE;

                default:
                    throw new Exception($"\"{x}\" is not a mode, Modes Are: GYRO-ANG, GYRO-CAL, GYRO-FAS, GYRO-RATE, TILT-ANG, TILT-RATE");
            }
        }
        public string GyroSensor_modes_To_String(GyroSensor_modes x)
        {
            switch (x)
            {
                case (GyroSensor_modes.GYRO_ANG):
                    return "GYRO-ANG";
                case (GyroSensor_modes.GYRO_CAL):
                    return "GYRO-CAL";
                case (GyroSensor_modes.GYRO_FAS):
                    return "GYRO-FAS";
                case (GyroSensor_modes.GYRO_G_and_A):
                    return "GYRO-G&A";
                case (GyroSensor_modes.GYRO_RATE):
                    return "GYRO-RATE";
                case (GyroSensor_modes.TILT_ANG):
                    return "TILT-ANG";
                case (GyroSensor_modes.TILT_RATE):
                    return "TILT-RATE";

                default:
                    throw new Exception("this should never happen");
            }
        }

        public GyroSensor(Device D):base(D)
        {
            RootToDir = D.RootToDir;
            if (D._type == DeviceType.lego_ev3_Gyro)
                if (ReadVar("modes") != "GYRO-ANG GYRO-RATE GYRO-FAS GYRO-G&A GYRO-CAL TILT-RATE TILT-ANG")
                    throw new InvalidOperationException("this device is not a touch sensor if please notify me on git");
            _Options = new string[] { "Value", "G_and_AValue", "CALValue", "Mode" };
            PORT = new LegoSensor(D);
        }

        public void SetModeTo(GyroSensor_modes x)
        {
            WriteVar("mode", GyroSensor_modes_To_String(x));
        }

        public override void ExecuteWriteOption(string Option, string[] Args)
        {
            switch(Option)
            {
                case ("Value"):
                case ("G_and_AValue"):
                case ("CALValue"):
                    throw new InvalidOperationException("ReadOnly");

                case ("Mode"):
                    SetModeTo(String_To_GyroSensor_modes(Args[0]));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public override string ExecuteReadOption(string Option)
        {
            switch (Option)
            {
                case ("Value"):
                    return Value.ToString();

                case ("G_and_AValue"):
                    return $"{G_and_AValue[0]}, {G_and_AValue[1]}";

                case ("CALValue"):
                    return $"{CALValue[0]}, {CALValue[1]}, {CALValue[2]}, {CALValue[3]},";

                case ("Mode"):
                    return GyroSensor_modes_To_String(Mode);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
