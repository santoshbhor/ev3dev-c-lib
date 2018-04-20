using System;
using System.Threading;

namespace Ev3DevLib.Motors
{
    //Last Updated on 5.4.2018 (DD/MM/YYYY)
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

    public class TachoMotor : OutPort
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
        public string[] _Options;
        public override string[] Options => _Options;

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
        public TachoMotor(Device dev) : base(dev)
        {
            if (dev._type != DeviceType.tacho_motor)
                throw new InvalidOperationException("this device is not a tachno motor");

            RootToDir = dev.RootToDir;
            if (RootToDir.Contains("tacho-motor"))
                MountPoint = ReadVar("address");
            else if(RootToDir.Contains(":"))
                MountPoint = RootToDir;
            else throw new InvalidOperationException("this uses the wrong class please re initulize the device and then try agen");
            _Options = new string[] {// r=readonly | rw=read+write | w=writeonly
                    "MoveTo","MoveRelativeTo","MoveFor","RunOn","RunAt","Stop","Reset", "Address", "MaxSpeed","Speed","Position","Dutycycle","RampUpSpeed","RampDownSpeed","Polarity","RPM","DriverName","LastCommand","State","StopAction"
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
                    if (value < Max_Speed*-1) return false;
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

        public override void ExecuteWriteOption(string Option, string[] Args)
        {
            switch (Option)
            {
                case ("MoveTo"):
                    MoveTo(int.Parse(Args[0]), int.Parse(Args[1]));
                    break;

                case ("MoveRelativeTo"):
                    MoveRelativeTo(int.Parse(Args[0]), int.Parse(Args[1]));
                    break;

                case ("MoveFor"):
                    MoveFor(int.Parse(Args[0]), int.Parse(Args[1]));
                    break;

                case ("RunOn"):
                    RunOn(int.Parse(Args[0]), int.Parse(Args[1]));
                    break;

                case ("RunAt"):
                    RunAt(int.Parse(Args[0]));
                    break;

                case ("Stop"):
                    Stop(String_To_TachoMotor_StopActions(Args[0]));
                    break;

                case ("Reset"):
                    Reset();
                    break;

                case ("Address"):
                    throw new InvalidOperationException("ReadOnly");

                case ("MaxSpeed"):
                    throw new InvalidOperationException("ReadOnly");

                case ("Speed"):
                    ChangeArg(TachoMotor_Args.speed_sp, Args[0]);
                    break;

                case ("Position"):
                    throw new InvalidOperationException("ReadOnly");

                case ("Dutycycle"):
                    ChangeArg(TachoMotor_Args.duty_cycle_sp, Args[0]);
                    break;

                case ("RampUpSpeed"):
                    ChangeArg(TachoMotor_Args.ramp_up_sp, Args[0]);
                    break;

                case ("RampDownSpeed"):
                    ChangeArg(TachoMotor_Args.ramp_down_sp, Args[0]);
                    break;

                case ("Polarity"):
                    ChangePolarity(String_To_TachoMotor_Polarity(Args[0]));
                    break;

                case ("RPM"):
                    throw new InvalidOperationException("ReadOnly");

                case ("DriverName"):
                    throw new InvalidOperationException("ReadOnly");

                case ("LastCommand"):
                    throw new InvalidOperationException("ReadOnly");

                case ("State"):
                    throw new InvalidOperationException("ReadOnly");

                case ("StopAction"):
                    ChangeStopAction(String_To_TachoMotor_StopActions(Args[0]));
                    break;

                case ("Command"):
                    SetCommand(String_To_TachoMotor_Commands(Args[0]));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ExecuteReadOption(string Option)
        {
            switch (Option)
            {
                case ("MoveTo"):
                    throw new InvalidOperationException("Executeable");

                case ("MoveRelativeTo"):
                    throw new InvalidOperationException("Executeable");

                case ("MoveFor"):
                    throw new InvalidOperationException("Executeable");

                case ("RunOn"):
                    throw new InvalidOperationException("Executeable");

                case ("RunAt"):
                    throw new InvalidOperationException("Executeable");

                case ("Stop"):
                    throw new InvalidOperationException("Executeable");

                case ("Reset"):
                    throw new InvalidOperationException("Executeable");

                case ("Address"):
                    return Address;

                case ("MaxSpeed"):
                    return Max_Speed.ToString();

                case ("Speed"):
                    return Speed.ToString();

                case ("Position"):
                    return Position.ToString();

                case ("Dutycycle"):
                    return Dutycycle.ToString();
                    
                case ("RampUpSpeed"):
                    return RampUpSpeed.ToString();

                case ("RampDownSpeed"):
                    return RampDownSpeed.ToString();

                case ("Polarity"):
                    return TachoMotor_Polarity_To_String(Polarity);

                case ("RPM"):
                    return RPM.ToString();

                case ("DriverName"):
                    return DriverName;

                case ("LastCommand"):
                    return TachoMotor_Commands_To_String(LastCommand);

                case ("State"):
                    return TachoMotor_States_To_String(State);

                case ("StopAction"):
                    return TachoMotor_StopActions_To_String(StopAction);

                case ("Command"):
                    throw new InvalidOperationException("WriteOnly");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
