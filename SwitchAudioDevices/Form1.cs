﻿using System;
using System.Windows.Forms;

namespace SwitchAudioDevices
{
    public partial class Form1 : Form
    {
        private ContextMenu menu;
        public Form1()
        {
            InitializeComponent();
            menu = new ContextMenu();
            NotifyIcon.ContextMenu = menu;
            Program.PopulateDeviceList(menu);
            AddPreferencesAndExit();
        }

        private void AddPreferencesAndExit()
        {
            // Add preferences
            var preferencesItem = new MenuItem { Text = "Preferences" };
            preferencesItem.Click += OpenPreferences;
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(preferencesItem);

            // Add an exit button
            var exitItem = new MenuItem { Text = "Exit" };
            exitItem.Click += OnExit;
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(exitItem);
        }

        private void OpenPreferences(object sender, EventArgs e)
        {
            Visible = true;

            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;

            Activate();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Icon = null;
            Dispose(true);
            Application.Exit();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenPreferences(sender, e);
        }

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = false;
            }
        }
    }
}