using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ev3DevLib.Misc;

namespace Ev3DevLib.Motors
{
    //Last Updated on 5.4.2018 (DD/MM/YYYY)
    public abstract class OutPort
    {
        public OutPort(Device D)
        {
        }
        public abstract string[] Options { get; }
        public abstract void ExecuteWriteOption(string Option, string[] Args);
        public abstract string ExecuteReadOption(string Option);
    }
    public class AutoOutPort
    {
        OutPort Port;
        public AutoOutPort(Device D)
        {
            switch (D._type)
            {
                case (DeviceType.tacho_motor):
                    Port = new TachoMotor(D);
                    break;

                case (DeviceType.dc_motor):
                    Port = new DcMotor(D);
                    break;

                case (DeviceType.servo_motor):
                    Port = new ServoMotor(D);
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
