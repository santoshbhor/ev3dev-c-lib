using System;
using Ev3DevLib;

namespace Ev3DevLib.Sensors
{
    public enum UltraSonic_Modes
    {
        DIST_CM,
        DIST_IN,
        LISTEN,
        SI_CM,
        SI_IN,
        DC_CM,
        DC_IN
    }
    public class UltraSonic
    {
        public LegoSensor PORT { get; internal set; }
        public int Value { get { return int.Parse(ReadVar("value0")); } }
        public UltraSonic_Modes Mode { get { return String_To_UltraSonic_Modes(ReadVar("mode")); } }

        public string RootToDir { get; internal set; }

        public UltraSonic_Modes String_To_UltraSonic_Modes(string x)
        {
            switch (x)
            {
                case ("US-DIST_CM"):
                    return UltraSonic_Modes.DIST_CM;
                case ("US-DIST_IN"):
                    return UltraSonic_Modes.DIST_IN;
                case ("US-LISTEN"):
                    return UltraSonic_Modes.LISTEN;
                case ("US-SI_CM"):
                    return UltraSonic_Modes.SI_CM;
                case ("US-SI_IN"):
                    return UltraSonic_Modes.SI_IN;
                case ("US-DC_CM"):
                    return UltraSonic_Modes.DC_CM;
                case ("US-DC_IN"):
                    return UltraSonic_Modes.DC_IN;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public string UltraSonic_Modes_To_String(UltraSonic_Modes x)
        {
            switch (x)
            {
                case (UltraSonic_Modes.DIST_CM):
                    return "US-DIST_CM";
                case (UltraSonic_Modes.DIST_IN):
                    return "US-DIST_IN";
                case (UltraSonic_Modes.LISTEN):
                    return "US-LISTEN";
                case (UltraSonic_Modes.SI_CM):
                    return "US-SI_CM";
                case (UltraSonic_Modes.SI_IN):
                    return "US-SI_IN";
                case (UltraSonic_Modes.DC_CM):
                    return "US-DC_CM";
                case (UltraSonic_Modes.DC_IN):
                    return "US-DC_IN";
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

        public UltraSonic(Device D)
        {
            RootToDir = D.RootToDir;
            if (D._type == DeviceType.lego_ev3_UltraSound)
                if (ReadVar("modes") != "US-DIST-CM US-DIST-IN US-LISTEN US-SI-CM US-SI-IN US-DC-CM US-DC-IN")
                    throw new InvalidOperationException("this device is not a touch sensor if please notify me on git");
            PORT = new LegoSensor(D);
        }

        public void ChangeMode(UltraSonic_Modes x)
        {
            WriteVar("mode", UltraSonic_Modes_To_String(x));
        }
    }
}
