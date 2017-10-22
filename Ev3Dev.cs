using System.Collections.Generic;
using System.IO;
using System;

namespace Ev3Dev
{
    public enum DeviceType
    {
        tacho_motor,
        dc_motor,
        servo_motor,
        lego_port,
    }
    public static class Ev3Dev//this is for creating links to devices
    {
        //key=name Value=RootToDirectory
        public static Dictionary<string, string> Items { get; private set; }
        public static void INIT()
        {
            Items = new Dictionary<string, string>();
            string[] Classes = new string[]
            {
                "dc-motor/",
                "servo-motor/",
                "tacho-motor/",
                "lego-port/",
            };
            for (int x = 0; x < Classes.Length; x++)
            {
                string[] addrs = Directory.GetFiles("/sys/class/"+Classes[x]);
                for (int y = 0; y < addrs.Length; y++)
                    Items.Add(Classes[x] + addrs[y], "/sys/class/" + Classes[x] + addrs[x]);
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
                    throw new ArgumentNullException();//Device type somehow doesnt have a know value
            }
        }
        public static DeviceType String_To_DeviceType(string x)
        {
            switch(x)
            {
                case ("dc-motor"):
                    return DeviceType.dc_motor;
                case ("servo-motor"):
                    return DeviceType.servo_motor;
                case ("tacho-motor"):
                    return DeviceType.tacho_motor;
                case ("lego-port"):
                    return DeviceType.lego_port;
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

    public struct Device
    {
        public string RootToDir { get; internal set; }
        public string[] Options { get; internal set; }
        public DeviceType _type { get; internal set; }
        public bool Connected { get { return Directory.Exists(RootToDir); }}
    }
}
