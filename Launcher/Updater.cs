﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    public static class Updater
    {
        static readonly string version = "v1.1.1";

        public static void CheckUpdates()
        {
            var t = new Task(new Action(CompareWithGitHub));
            t.Start();
        }

        private static void CompareWithGitHub()
        {
            using (var client = new WebClient())
            {
                try
                {
                    var data = client.DownloadString("https://raw.githubusercontent.com/lyftzeigen/SnakeBattle/master/README.md");
                    var v = data.Split('\n')[0].Split(' ')[2];

                    if (version != v)
                    {
                        MessageBox.Show("Update available!\n" +
                            "Please check new version: https://github.com/lyftzeigen/SnakeBattle", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch { }
            }
        }
    }
}