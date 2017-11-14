using System;
using System.Threading;

namespace Ev3DevLib.Motors
{
    //added on 21.10.2017 (DD/MM/YYYY)
    public enum TachoMotor_StopActions
    {
        coast,
        _break,
        hold
    }
    public enum TachoMotor_Commands
    {
        unknown =0,//deffult
        run_forever,
        run_to_abs_pos,
        run_to_rel_pos,
        run_timed,
        run_direct,
        stop,
        reset
    }
    public enum TachoMotor_States
    {
        running,
        ramping,
        holding,
        overloaded,
        stalled
    }
    public enum TachoMotor_Polarity
    {
        normal,
        inversed
    }
    public enum TachoMotor_Args
    {
        duty_cycle_sp,
        position_sp,
        speed_sp,
        ramp_up_sp,
        ramp_down_sp,
        time_sp
    }
    //--------------------------------

    public class TachoMotor
    {

        //values
        //my Rule not matter what all of the values must be updated inside UPDATE
        //unless there NON-READ ie 'onlywriteable'(w)
        public string Address                       { get { try { return ReadVar("address");  } catch { return "N/A"; } } }
        public int Max_Speed                        { get { try { return int.Parse(ReadVar("max_speed")); } catch { return -1; } } }
        public int Speed                            { get { try { return int.Parse(ReadVar("speed")); } catch { return -1; } } }
        public int Position                         { get { try { return int.Parse(ReadVar("position")); } catch { return -1; } } }
        public int Dutycycle                        { get { try { return int.Parse(ReadVar("duty_cycle")); } catch { return -1; } } }
        public int RampUpSpeed                      { get { try { return int.Parse(ReadVar("ramp_up_sp")); } catch { return -1; } } }
        public int RampDownSpeed                    { get { try { return int.Parse(ReadVar("ramp_down_sp")); } catch { return -1; } } }
        public TachoMotor_Polarity Polarity         { get { try { return String_To_TachoMotor_Polarity(ReadVar("polarity")); } catch { return TachoMotor_Polarity.normal; } } }
        public int RPM                              { get { try { return int.Parse(ReadVar("count_per_rot")); } catch { return -1; } } }
        public string DriverName                    { get { try { return ReadVar("driver_name"); } catch { return "N/A"; } } }
        public TachoMotor_Commands LastCommand      { get; private set; }//NON-READ
        public TachoMotor_States State              { get { try { return String_To_TachoMotor_States(ReadVar("state")); } catch { return TachoMotor_States.stalled; } } }
        public TachoMotor_StopActions StopAction    { get { try { return String_To_TachoMotor_StopActions(ReadVar("stop_action")); } catch { return TachoMotor_StopActions.coast; } } }
        

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
            if(var == "command")
            {
                LastCommand = String_To_TachoMotor_Commands(value);
            }
        }

        //constructor
        public TachoMotor(Device dev)
        {
            if (dev._type != DeviceType.tacho_motor)
                throw new InvalidOperationException("this device is not a tachno motor");

            RootToDir = dev.RootToDir;
            if (RootToDir.Contains("tacho-motor"))
                MountPoint = ReadVar("address");
            else if(RootToDir.Contains(":"))
                MountPoint = RootToDir;
            else throw new InvalidOperationException("this uses the wrong class please re initulize the device and then try agen");
            Options = new string[] {// r=readonly | rw=read+write | w=writeonly
                    "address:r",            "command:w",        "commands:r",       "count_per_rot:r",  "count_per_m:r",
                    "full_travel_count:r",  "driver_name:r",    "duty_cycle:r",     "duty_cycle_sp:rw", "polarity:rw",
                    "position:rw",          "hold_pid/kd:rw",   "hold_pid/ki:rw",   "hold_pid/kp:rw",   "max_speed:r",
                    "position_sp:rw",       "speed:r",          "speed_sp:rw",      "ramp_up_sp:rw",    "ramp_down_sp:rw",
                    "speed_pid/kd:rw",      "speed_pid/ki:rw",  "speed_pid/kp:rw",  "state:r",          "stop_action:rw",
                    "stop_actions:r",       "time_sp:rw"
                };
        }

        //helpers
        public static string TachoMotor_StopActions_To_String(TachoMotor_StopActions x)
        {
            switch(x)
            {
                case (TachoMotor_StopActions.coast):
                    return "coast";

                case (TachoMotor_StopActions.hold):
                    return "hold";

                case (TachoMotor_StopActions._break):
                    return "break";


                default:
                    return "INVALID";
            }
        }
        public static string TachoMotor_Commands_To_String(TachoMotor_Commands x)
        {
            switch (x)
            {
                case (TachoMotor_Commands.reset):
                    return "reset";
                case (TachoMotor_Commands.run_direct):
                    return "run-direct";
                case (TachoMotor_Commands.run_forever):
                    return "run-forever";
                case (TachoMotor_Commands.run_timed):
                    return "run-timed";
                case (TachoMotor_Commands.run_to_abs_pos):
                    return "run-to-abs-pos";
                case (TachoMotor_Commands.run_to_rel_pos):
                    return "run-to-rel-pos";
                case (TachoMotor_Commands.stop):
                    return "stop";

                default:
                    return "INVALID";
            }
        }
        public static string TachoMotor_States_To_String(TachoMotor_States x)
        {
            switch (x)
            {
                case (TachoMotor_States.holding):
                    return "holding";
                case (TachoMotor_States.overloaded):
                    return "overloaded";
                case (TachoMotor_States.ramping):
                    return "ramping";
                case (TachoMotor_States.running):
                    return "running";
                case (TachoMotor_States.stalled):
                    return "stalled";
                default:
                    return "INVALID";
            }
        }
        public static string TachoMotor_Polarity_To_String(TachoMotor_Polarity x)
        {
            switch (x)
            {
                case (TachoMotor_Polarity.inversed):
                    return "inversed";
                case (TachoMotor_Polarity.normal):
                    return "normal";

                default:
                    return "INVALID";
            }
        }
        public static string TachoMotor_Args_To_String(TachoMotor_Args x)
        {
            switch(x)
            {
                case (TachoMotor_Args.duty_cycle_sp):
                    return "duty_cycle_sp";
                case (TachoMotor_Args.position_sp):
                    return "position_sp";
                case (TachoMotor_Args.ramp_down_sp):
                    return "ramp_down_sp";
                case (TachoMotor_Args.ramp_up_sp):
                    return "ramp_up_sp";
                case (TachoMotor_Args.speed_sp):
                    return "speed_sp";
                case (TachoMotor_Args.time_sp):
                    return "time_sp";


                default:
                    return "INVALID";
            }
        }

        public static TachoMotor_StopActions String_To_TachoMotor_StopActions(string x)
        {
            switch (x)
            {
                case ("coast"):
                    return  TachoMotor_StopActions.coast;

                case ("hold"):
                    return TachoMotor_StopActions.hold;

                case ("break"):
                    return TachoMotor_StopActions._break;


                default:
                    throw new ArgumentOutOfRangeException("_break = break other then that names are the same as there string counter parts");
            }
        }
        public static TachoMotor_Commands String_To_TachoMotor_Commands(string x)
        {
            switch (x)
            {
                case ("reset"):
                    return TachoMotor_Commands.reset;
                case ("run-direct"):
                    return TachoMotor_Commands.run_direct;
                case ("run-forever"):
                    return TachoMotor_Commands.run_forever;
                case ("run-timed"):
                    return TachoMotor_Commands.run_timed;
                case ("run-to-abs-pos"):
                    return TachoMotor_Commands.run_to_abs_pos;
                case ("run-to-rel-pos"):
                    return TachoMotor_Commands.run_to_rel_pos;
                case ("stop"):
                    return TachoMotor_Commands.stop;

                default:
                    throw new ArgumentOutOfRangeException("arg is not a TachoMotor_Commands 'TachoMotor_Commands strings are the same as there names just a _ becomes a -'");
            }
        }
        public static TachoMotor_States String_To_TachoMotor_States(string x)
        {
            switch (x)
            {
                case ("holding"):
                    return TachoMotor_States.holding;
                case ("overloaded"):
                    return TachoMotor_States.overloaded;
                case ("ramping"):
                    return TachoMotor_States.ramping;
                case ("running"):
                    return TachoMotor_States.running;
                case ("stalled"):
                    return TachoMotor_States.stalled;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static TachoMotor_Polarity String_To_TachoMotor_Polarity(string x)
        {
            switch (x)
            {
                case ("inversed"):
                    return TachoMotor_Polarity.inversed;
                case ("normal"):
                    return TachoMotor_Polarity.normal;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static TachoMotor_Args String_To_TachoMotor_Args(string x)
        {
            switch (x)
            {
                case ("duty_cycle_sp"):
                    return TachoMotor_Args.duty_cycle_sp;
                case ("position_sp"):
                    return TachoMotor_Args.position_sp;
                case ("ramp_down_sp"):
                    return TachoMotor_Args.ramp_down_sp;
                case ("ramp_up_sp"):
                    return TachoMotor_Args.ramp_up_sp;
                case ("speed_sp"):
                    return TachoMotor_Args.speed_sp;
                case ("time_sp"):
                    return TachoMotor_Args.time_sp;


                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        //functions
        public void MoveTo(int Position, int Speed)
        {
            if (TestArg(Position, TachoMotor_Args.position_sp) &&
                TestArg(Speed, TachoMotor_Args.speed_sp))
            {

                WriteVar("speed_sp", Speed.ToString());
                WriteVar("position_sp", Position.ToString());
                WriteVar("command", "run-to-abs-pos");
            }
            else throw new ArgumentOutOfRangeException("one or more args are invalid");
        }
        public void MoveRelativeTo(int Distance, int Speed)
        {
            if (TestArg(Distance, TachoMotor_Args.position_sp) &&
                TestArg(Speed, TachoMotor_Args.speed_sp))
            {
                WriteVar("speed_sp", Speed.ToString());
                WriteVar("position_sp", Distance.ToString());
                WriteVar("command", "run-to-rel-pos");
            }
            else throw new ArgumentOutOfRangeException();
        }
        public void MoveFor(int Speed, int Time)
        {
            if (TestArg(Speed, TachoMotor_Args.speed_sp) &&
                TestArg(Time, TachoMotor_Args.time_sp))
            {
                WriteVar("speed_sp", Speed.ToString());
                WriteVar("time_sp", Time.ToString());
                WriteVar("command", "run-timed");
            }
            else throw new ArgumentOutOfRangeException();
        }
        public void RunOn(int Dutycycle, int Speed)
        {
            if (TestArg(Dutycycle, TachoMotor_Args.duty_cycle_sp) &&
                TestArg(Speed, TachoMotor_Args.speed_sp))
            {
                WriteVar("duty_cycle_sp", Dutycycle.ToString());
                WriteVar("speed_sp", Speed.ToString());
                WriteVar("command", "run-direct");
            }
            else throw new ArgumentOutOfRangeException();
        }
        public void RunAt(int Speed)
        {
            if (TestArg(Speed, TachoMotor_Args.speed_sp))
            {
                WriteVar("speed_sp", Speed.ToString());
                WriteVar("command", "run-forever");
            }
            else throw new ArgumentOutOfRangeException();
        }
        public void Stop(TachoMotor_StopActions Action)
        {
            WriteVar("stop_action", TachoMotor_StopActions_To_String(Action));
            WriteVar("command", "stop");
        }
        public void Reset()
        {
            WriteVar("command", "reset");
        }

        //safty functions
        private bool TestArg(string value, TachoMotor_Args forArg)
        {
            return TestArg(int.Parse(value),forArg);
        }
        private bool TestArg(int value, TachoMotor_Args forArg)
        {
            switch (forArg)
            {
                case (TachoMotor_Args.duty_cycle_sp):
                    if (value < -100) return false;
                    if (value > 100) return false;
                    return true;

                case (TachoMotor_Args.position_sp)://uses full int32 range
                    return true;

                case (TachoMotor_Args.ramp_down_sp):
                case (TachoMotor_Args.ramp_up_sp):
                    if (value < 0) return false;
                    if (value > 10000) return false;
                    return true;

                case (TachoMotor_Args.speed_sp):
                    if (value < 0) return false;
                    if (value > Max_Speed) return false;
                    return true;

                case (TachoMotor_Args.time_sp):
                    if (value < 0) return false;
                    return true;
            }
            return false;//should never happen
        }
        
        //hands on for more advanced users
        public void ChangeArg(TachoMotor_Args x, string value)
        {
            if (TestArg(value, x))
            {
                WriteVar(TachoMotor_Args_To_String(x), value);
            }
            else throw new ArgumentOutOfRangeException("invalid value for this arg");
        }
        public void ChangeStopAction(TachoMotor_StopActions to)
        {
            WriteVar("stop_action", TachoMotor_StopActions_To_String(to));
        }
        public void ChangePolarity(TachoMotor_Polarity to)
        {
            WriteVar("polarity", TachoMotor_Polarity_To_String(to));
        }
        public void ChangeDutyCycle(int Cycle)
        {
            if (TestArg(Cycle, TachoMotor_Args.duty_cycle_sp))
            {
                WriteVar("duty_cycle_sp", Cycle.ToString());
            }
            else throw new ArgumentOutOfRangeException();
        }
        public void SetCommand(TachoMotor_Commands to)
        {
            if (to == TachoMotor_Commands.unknown)
                throw new InvalidOperationException("this is a filler since command cant be read from DO NOT USE THIS AS A COMMAND TO SEND");
            else
                WriteVar("command", TachoMotor_Commands_To_String(to));
        }
    }
}
