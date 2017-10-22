using System;
using System.Threading;

namespace Ev3Dev.Motors
{
    //added on 21.10.2017 (DD/MM/YYYY)
    public enum DcMotor_StopActions
    {
        unknown = 0,//deffult
        coast,
        _break,
    }
    public enum DcMotor_States
    {
        running,
        ramping,
    }
    public enum DcMotor_Polarity
    {
        normal,
        inversed
    }
    public enum DcMotor_Commands
    {
        unknown = 0,//deffult
        run_forever,
        run_timed,
        run_direct,
        stop
    }
    public enum DcMotor_Args
    {
        duty_cycle_sp,
        ramp_down_sp,
        ramp_up_sp,
        time_sp,
    }
    //--------------------------------

    public class DcMotor
    {
        //values
        //my Rule not matter what all of the values must be updated inside UPDATE
        //unless there NON-READ ie 'onlywriteable'(w)
        public string Address { get; private set; }
        public DcMotor_Commands LastCommand { get; private set; }//NON-READ
        public int Dutycycle { get; private set; }
        public int RampUpSpeed { get; private set; }
        public int RampDownSpeed { get; private set; }
        public DcMotor_Polarity Polarity { get; private set; }
        public DcMotor_States State { get; private set; }
        public DcMotor_StopActions StopAction { get; private set; }//NON-READ

        //my members
        public bool NoDeley = false;//this says if we should NOT sleep while updating args
        public int Deley_MS = 30;//this is the time to sleep if !NoDeley

        private Thread UpdateTHR;
        private void UPDATE()
        {
            while (true)
            {
                Address = ReadVar("address");
                Dutycycle = int.Parse(ReadVar("duty_cycle"));
                Polarity = String_To_DcMotor_Polarity(ReadVar("polarity"));
                State = String_To_DcMotor_States(ReadVar("state"));

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
            if (var == "stop_action")
            {
                StopAction = String_To_DcMotor_StopActions(value);
            }
            else if (var == "command")
            {
                LastCommand = String_To_DcMotor_Commands(value);
            }
        }

        //constructor
        public DcMotor(Device dev)
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

            Options = new string[] {// r=readonly | rw=read+write | w=writeonly
                    "address:r",    "command:w",        "commands:r",       "driver_name:r",
                    "duty_cycle:r", "duty_cycle_sp:rw", "polarity:rw",      "state:r",
                    "stop_action:w","stop_actions:r",   "ramp_down_sp:rw",  "ramp_up_sp:rw",
                    "time_sp:rw"
                };
            UpdateTHR.Start();
        }

        //helpers
        public static string DcMotor_StopActions_To_String(DcMotor_StopActions x)
        {
            switch (x)
            {
                case (DcMotor_StopActions.coast):
                    return "coast";
                case (DcMotor_StopActions._break):
                    return "break";
                default:
                    return "INVALID";
            }
        }
        public static string DcMotor_States_To_String(DcMotor_States x)
        {
            switch (x)
            {
                case (DcMotor_States.ramping):
                    return "ramping";
                case (DcMotor_States.running):
                    return "running";
                default:
                    return "INVALID";
            }
        }
        public static string DcMotor_Polarity_To_String(DcMotor_Polarity x)
        {
            switch (x)
            {
                case (DcMotor_Polarity.inversed):
                    return "inversed";
                case (DcMotor_Polarity.normal):
                    return "normal";
                default:
                    return "INVALID";
            }
        }
        public static string DcMotor_Commands_To_String(DcMotor_Commands x)
        {
            switch (x)
            {
                case (DcMotor_Commands.run_direct):
                    return "run-direct";
                case (DcMotor_Commands.run_forever):
                    return "run-forever";
                case (DcMotor_Commands.run_timed):
                    return "run-timed";
                case (DcMotor_Commands.stop):
                    return "stop";
                default:
                    return "INVALID";
            }
        }
        public static string DcMotor_Args_To_String(DcMotor_Args x)
        {
            switch (x)
            {
                case (DcMotor_Args.duty_cycle_sp):
                    return "duty-cycle-sp";
                case (DcMotor_Args.ramp_down_sp):
                    return "ramp-down-sp";
                case (DcMotor_Args.ramp_up_sp):
                    return "ramp-up-sp";
                case (DcMotor_Args.time_sp):
                    return "time-sp";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DcMotor_StopActions String_To_DcMotor_StopActions(string x)
        {
            switch (x)
            {
                case ("coast"):
                    return DcMotor_StopActions.coast;
                case ("break"):
                    return DcMotor_StopActions._break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static DcMotor_States String_To_DcMotor_States(string x)
        {
            switch (x)
            {
                case ("ramping"):
                    return DcMotor_States.ramping;
                case ("running"):
                    return DcMotor_States.running;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static DcMotor_Polarity String_To_DcMotor_Polarity(string x)
        {
            switch (x)
            {
                case ("inversed"):
                    return DcMotor_Polarity.inversed;
                case ("normal"):
                    return DcMotor_Polarity.normal;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static DcMotor_Commands String_To_DcMotor_Commands(string x)
        {
            switch (x)
            {
                case ("run-direct"):
                    return DcMotor_Commands.run_direct;
                case ("run-forever"):
                    return DcMotor_Commands.run_forever;
                case ("run-timed"):
                    return DcMotor_Commands.run_timed;
                case ("stop"):
                    return DcMotor_Commands.stop;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static DcMotor_Args String_To_DcMotor_Args(string x)
        {
            switch (x)
            {
                case ("duty_cycle_sp"):
                    return DcMotor_Args.duty_cycle_sp;
                case ("ramp_down_sp"):
                    return DcMotor_Args.ramp_down_sp;
                case ("ramp_up_sp"):
                    return DcMotor_Args.ramp_up_sp;
                case ("time_sp"):
                    return DcMotor_Args.time_sp;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        //functions
        public void MoveFor(int Dutycycle, int Time)
        {
            if (TestArg(Dutycycle, DcMotor_Args.duty_cycle_sp) &&
                TestArg(Time, DcMotor_Args.time_sp))
            {
                WriteVar("duty_cycle_sp", Dutycycle.ToString());
                WriteVar("time_sp", Time.ToString());
                WriteVar("command", "run-timed");
            }
            else throw new ArgumentOutOfRangeException();
        }
        public void RunOn(int Dutycycle)
        {
            if (TestArg(Dutycycle, DcMotor_Args.duty_cycle_sp))
            {
                WriteVar("duty_cycle_sp", Dutycycle.ToString());
                WriteVar("command", "run-direct");//not using run-forever due to it having the same effect but run-direct will dynamicly update the dutycycle as the file is updated
            }
            else throw new ArgumentOutOfRangeException();
        }
        public void Stop(DcMotor_StopActions Action)
        {
            WriteVar("stop_action", DcMotor_StopActions_To_String(Action));
            WriteVar("command", "stop");
        }
        
        //safty functions
        private bool TestArg(string value, DcMotor_Args forArg)
        {
            return TestArg(int.Parse(value), forArg);
        }
        private bool TestArg(int value, DcMotor_Args forArg)
        {
            switch(forArg)
            {
                case (DcMotor_Args.duty_cycle_sp):
                    if (value < -100) return false;
                    if (value > 100) return false;
                    return true;

                case (DcMotor_Args.ramp_down_sp):
                case (DcMotor_Args.ramp_up_sp):
                    if (value < 0) return false;
                    if (value > 10000) return false;
                    return true;

                case (DcMotor_Args.time_sp):
                    if (value < 0) return false;
                    return true;
            }
            return false;//should never happen
        }

        //hands on for more advanced users
        public void ChangeArg(DcMotor_Args x, string value)
        {
            if (TestArg(value, x))
            {
                WriteVar(DcMotor_Args_To_String(x), value);
            }
            else throw new ArgumentOutOfRangeException("invalid value for this arg");
        }
        public void ChangeArg(DcMotor_Args x, int value)
        {
            if (TestArg(value, x))
            {
                WriteVar(DcMotor_Args_To_String(x), value.ToString());
            }
            else throw new ArgumentOutOfRangeException("invalid value for this arg");
        }
        public void ChangeStopAction(DcMotor_StopActions to)
        {
            WriteVar("stop_action", DcMotor_StopActions_To_String(to));
        }
        public void ChangePolarity(DcMotor_Polarity to)
        {
            WriteVar("polarity", DcMotor_Polarity_To_String(to));
        }
        public void ChangeDutyCycle(int Cycle)
        {
            WriteVar("duty_cycle_sp", Cycle.ToString());
        }
        public void SetCommand(DcMotor_Commands to)
        {
            if (to == DcMotor_Commands.unknown)
                throw new InvalidOperationException("this is a filler since command cant be read from DO NOT USE THIS AS A COMMAND TO SEND");
            else
                WriteVar("command", DcMotor_Commands_To_String(to));
        }
    }
}
