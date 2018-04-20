using System.Collections.Generic;
using System.IO;
using System;

namespace Ev3DevLib
{
    public enum DeviceType
    {
        Unknown,
        tacho_motor,
        dc_motor,
        servo_motor,
        lego_port,
        lego_ev3_Touch,
        lego_ev3_Gyro,
        lego_ev3_Light,
        lego_ev3_UltraSound,
        lego_sensor
    }
    public static class Ev3Dev//this is for creating links to devices
    {
        public static bool DebuggText = false;
        internal static readonly string[] Classes = new string[]
            {
                "dc-motor/",
                "servo-motor/",
                "tacho-motor/",
                "lego-port/",
                "lego-sensor/",
                "ev3-uart-host/",
            };
        public static Dictionary<string, string> Items { get; private set; }
        public static void INIT()
        {
            Items = new Dictionary<string, string>();
            for (int x = 0; x < Classes.Length; x++)
            {
                string[] addrs = Directory.GetFiles("/sys/class/"+Classes[x]);
                for (int y = 0; y < addrs.Length; y++)
                    Items.Add(Classes[x] + addrs[y], "/sys/class/" + Classes[x] + addrs[y]);
            }
        }

        private static string GetRootDir(DeviceType type)
        {
            switch (type)
            {
                case (DeviceType.dc_motor):
                    return "/sys/class/dc-motor/";
                case (DeviceType.servo_motor):
                    return "/sys/class/servo-motor/";
                case (DeviceType.tacho_motor):
                    return "/sys/class/tacho-motor/";
                case (DeviceType.lego_port):
                    return "/sys/class/lego-port/";
                default:
                    throw new ArgumentNullException();
            }
        }
        public static DeviceType String_To_DeviceType(string x)
        {
            x = x.Replace("/", "");
            switch (x)
            {
                case ("dc-motor"):
                    return DeviceType.dc_motor;
                case ("servo-motor"):
                    return DeviceType.servo_motor;
                case ("tacho-motor"):
                    return DeviceType.tacho_motor;
                case ("lego-port"):
                    return DeviceType.lego_port;
                case ("lego-ev3-touch"):
                    return DeviceType.lego_ev3_Touch;
                case ("lego-ev3-gyro"):
                    return DeviceType.lego_ev3_Gyro;
                case ("lego-sensor"):
                case ("ev3-uart-host"):
                    return DeviceType.lego_sensor;
                case ("lego-ev3-ultrasound"):
                    return DeviceType.lego_ev3_UltraSound;
                case ("lego-ev3-light"):
                    return DeviceType.lego_ev3_Light;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static string DeviceType_To_String(DeviceType x)
        {
            switch (x)
            {
                case (DeviceType.dc_motor):
                    return "dc-motor";
                case (DeviceType.servo_motor):
                    return "servo-motor";
                case (DeviceType.tacho_motor):
                    return "tacho-motor";
                case (DeviceType.lego_port):
                    return "lego-port";
                case (DeviceType.lego_ev3_Touch):
                    return "lego-ev3-touch";
                case (DeviceType.lego_ev3_Gyro):
                    return "lego-ev3-gyro";
                case (DeviceType.lego_ev3_UltraSound):
                    return "lego-ev3-ultrasound";
                case (DeviceType.lego_ev3_Light):
                    return "lego-ev3-light";
                case (DeviceType.lego_sensor):
                    return "lego-sensor";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string[] GetAddressesOf(DeviceType type)
        {
            return Directory.GetDirectories(GetRootDir(type));
        }
        public static Device CreateDeviceInstanceOf(string address, DeviceType type)
        {
            string RootB = GetRootDir(type);
            address = address.Replace("/", "").Replace("\\", "");//no new directorys just directory name
            if (Directory.Exists(RootB + address))
            {
                Device D = new Device
                {
                    RootToDir = RootB + address,
                    _type = type,
                    Options = Directory.GetFiles(RootB + address),
                };
                return D;
            }
            else throw new ArgumentOutOfRangeException();
        }
        public static Device CreateDeviceInstanceByMountPt(string MountPt,DeviceType type)
        {
            return new Device()
            {
                Options = Directory.GetFiles(MountPt),
                RootToDir = MountPt,
                _type = type,
            };
        }
        public static Device CreateDeviceInstanceByItemName(string name)
        {
            if (Items.ContainsKey(name))
            {
                string Root = Items[name];
                Device D = new Device()
                {
                    RootToDir = Root,
                    _type = String_To_DeviceType(name.Substring(0, name.IndexOf('/'))),
                    Options = Directory.GetFiles(Root),
                };
                return D;
            }
            else
                throw new ArgumentOutOfRangeException();
        }
    }
    public static class KnownDrivers
    {
        //need to add these
        public const string LegoEv3LMotor = "lego-ev3-l-motor";
        public const string LegoEv3MMotor = "lego-ev3-m-motor";
        public const string NxtI2cSensor  = "nxt-i2c-sensor";
        public const string LegoEv3Color  = "lego-ev3-color";
        public const string LegoEv3Us     = "lego-ev3-us";
        public const string LegoNxtUs     = "lego-nxt-us";
        public const string LegoEv3Gyro   = "lego-ev3-gyro";
        public const string LegoEv3Ir     = "lego-ev3-ir";
        public const string LegoNxtSound  = "lego-nxt-sound";
        public const string LegoNxtLight  = "lego-nxt-light";
        public const string LegoEv3Touch  = "lego-ev3-touch";
        public const string LegoNxtTouch  = "lego-nxt-touch";
    }
    public struct Device
    {
        public string RootToDir { get; internal set; }
        public string[] Options { get; internal set; }
        public DeviceType _type { get; internal set; }
        public string DriverName { get; internal set; }
        public bool Connected { get { try { File.OpenWrite(RootToDir + "/command").Close(); return true; } catch { return false; } }}
    }
}
