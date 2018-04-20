using System;
namespace Ev3DevLib.Sensors
{
    //Last Updated on 8.4.2018 (DD/MM/YYYY)
    public abstract class InPort
    {
        public InPort(Device D)
        {

        }
        public abstract string[] Options { get; }
        public abstract void ExecuteWriteOption(string Option, string[] Args);
        public abstract string ExecuteReadOption(string Option);
    }
    public class AutoInPort
    {
        InPort Port;
        public AutoInPort(Device D)
        {
            switch (D._type)
            {
                case (DeviceType.lego_ev3_Gyro):
                    Port = new GyroSensor(D);
                    break;

                case (DeviceType.lego_ev3_Light):
                    Port = new LightSensor(D);
                    break;

                case (DeviceType.lego_ev3_Touch):
                    Port = new TouchSensor(D);
                    break;

                case (DeviceType.lego_ev3_UltraSound):
                    Port = new UltraSonic(D);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public string[] Options => Port.Options;
        public void ExecuteWriteOption(string Option, string[] Args) => Port.ExecuteWriteOption(Option, Args);
        public string ExecuteReadOption(string Option) => Port.ExecuteReadOption(Option);
    }

}
