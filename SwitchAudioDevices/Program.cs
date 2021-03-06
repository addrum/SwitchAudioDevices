﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using SwitchAudioDevices.Properties;

namespace SwitchAudioDevices
{
    public class Program
    {
        private static int _deviceCount;
        private static int _currentDeviceId;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void RegisterHotkeys(KeyboardHook hook)
        {
            // register the control + alt + F12 combination as hot key
            var hotkeys = Settings.Default.ModifierKeys.Split(',');
            ModifierKeys modifiers = 0;
            foreach (var hotkey in hotkeys)
            {
                switch (hotkey)
                {
                    case "CTRL":
                        modifiers |= ModifierKeys.Control;
                        break;
                    case "ALT":
                        modifiers |= ModifierKeys.Alt;
                        break;
                    case "SHIFT":
                        modifiers |= ModifierKeys.Shift;
                        break;
                    case "WIN":
                        modifiers |= ModifierKeys.Win;
                        break;
                }
            }

            var key = Settings.Default.Keys;

            hook.RegisterHotKey(modifiers, key);
            //hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Alt, Keys.F12);
        }
        
        #region Tray events

        public static void PopulateDeviceList(ContextMenu menu)
        {

            // All all active devices
            foreach (var device in GetDevices())
            {
                var id = device.Item1;
                var deviceName = device.Item2;
                var isInUse = device.Item3;

                var item = new MenuItem { Checked = isInUse, Text = deviceName };
                item.Click += (s, a) => SelectDevice(id);

                menu.MenuItems.Add(item);
            }
        }

        #endregion

        #region EndPointController.exe interaction

        public static IEnumerable<Tuple<int, string, bool>> GetDevices()
        {
            _deviceCount = 0;
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = "EndPointController.exe",
                    Arguments = "-f \"%d|%ws|%d|%d\""
                }
            };
            p.Start();
            p.WaitForExit();
            var stdout = p.StandardOutput.ReadToEnd().Trim();

            var devices = new List<Tuple<int, string, bool>>();

            foreach (var line in stdout.Split('\n'))
            {
                var elems = line.Trim().Split('|');
                var deviceInfo = new Tuple<int, string, bool>(int.Parse(elems[0]), elems[1], elems[3].Equals("1"));
                devices.Add(deviceInfo);
                _deviceCount += 1;
            }

            return devices;
        }

        public static void SelectDevice(int id)
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = "EndPointController.exe",
                    Arguments = id.ToString(CultureInfo.InvariantCulture)
                }
            };
            p.Start();
            p.WaitForExit();
        }

        //Gets the ID of the next sound device in the list
        public static int NextId()
        {
            if (_currentDeviceId == _deviceCount)
            {
                _currentDeviceId = 1;
            }
            else
            {
                _currentDeviceId += 1;
            }
            return _currentDeviceId;
        }

        public static string GetCurrentPlaybackDevice()
        {
            var deviceName = "";
            foreach (var device in GetDevices())
            {
                if (device.Item1 == _currentDeviceId)
                    deviceName = device.Item2;
            }
            
            return deviceName;
        }

        #endregion
    }
}
