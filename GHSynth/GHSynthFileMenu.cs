﻿using System;
using System.Windows.Forms;

namespace GHSynth
{
    class GHSynthFileMenu
    {
        Eto.Forms.UITimer _timer;

        public void AddToMenu()
        {
            if (_timer != null)
                return;
            _timer = new Eto.Forms.UITimer();
            _timer.Interval = 1;
            _timer.Elapsed += SetupMenu;
            _timer.Start();
        }

        void SetupMenu(object sender, EventArgs e)
        {
            var editor = Grasshopper.Instances.DocumentEditor;
            if (null == editor || editor.Handle == IntPtr.Zero)
                return;

            var controls = editor.Controls;
            if (null == controls || controls.Count == 0)
                return;

            _timer.Stop();
            foreach (var ctrl in controls)
            {
                var menu = ctrl as Grasshopper.GUI.GH_MenuStrip;
                if (menu == null)
                    continue;

                for (int i = 0; i < menu.Items.Count; i++)
                {
                    var menuitem = menu.Items[i] as ToolStripMenuItem;
                    if (menuitem != null && menuitem.Text == "GHSynth") return;
                }
                var ghSynthMenu = new ToolStripMenuItem("GHSynth");
                menu.Items.Add(ghSynthMenu);

                var button_setSampleRate = new ToolStripMenuItem() {
                    Text = "Sample Rate",
                    Checked = false,
                    Image = Properties.Resources.Hz
                };
                button_setSampleRate.Click += SampleRateClicked;
                ghSynthMenu.DropDownItems.Add(button_setSampleRate);

            }
        }

        private void SampleRateClicked(object sender, EventArgs e)
        {
            double newRate = GHSynthSettings.SampleRate;
            var dialog_result = Rhino.UI.Dialogs.ShowNumberBox(
                "Sample Rate",
                "Sample Rate",
                ref newRate,
                8000, 
                44100);
            if ((int)newRate != GHSynthSettings.SampleRate) 
                Grasshopper.Instances.ActiveCanvas.Document.ExpireSolution();
            GHSynthSettings.SampleRate = (int)newRate;
        }
    }
}