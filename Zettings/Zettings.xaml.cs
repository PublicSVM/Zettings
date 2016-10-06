using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;

namespace Zettings
{
    public partial class ZettingWindow : Window
    {
        public string GameName { get { return gameName.Text; } }

        public ZettingWindow()
        {
            InitializeComponent();
        }

        private void gameName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            status.Text = "";
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Invoke(Save);
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Invoke(Load);
        }

        private void Invoke(Action<string, string> action)
        {
            if (string.IsNullOrWhiteSpace(GameName))
            {
                status.Text = "Err: Name empty";
                return;
            }

            try
            {
                status.Text = "";
                action.Invoke(GetLivesplitPath(), GameName);
                status.Text = $"Success: { action.Method.Name }";
            }
            catch (Exception ex)
            {
                status.Text = $"Ex: {ex.Message}";
            }
        }

        private static string GetLivesplitPath()
        {
            string path = ConfigurationManager.AppSettings["LiveSplitPath"];

            if (path == "")
                path = GetPathBy(Environment.SpecialFolder.ProgramFiles);
            if (path == "")
                path = GetPathBy(Environment.SpecialFolder.CommonProgramFilesX86);
            if (path == "")
                path = GetPathBy(Environment.SpecialFolder.UserProfile, "Downloads");

            return path;
        }

        private static string GetPathBy(Environment.SpecialFolder folder, string directory = "")
        {
            var userProfile = Environment.GetFolderPath(folder);
            var path = Path.Combine(userProfile, directory);

            var directories = Directory.GetDirectories(path, "LiveSplit*");
            return directories?.Max() ?? "";
        }

        private static void Save(string path, string game)
        {
            var settingsPath = Path.Combine(path, "settings");
            Directory.CreateDirectory(settingsPath);
            var oldPath = Path.Combine(path, "settings.cfg");
            var newPath = Path.Combine(settingsPath, $"{ game }.settings.cfg");
            File.Copy(oldPath, newPath, true);
        }

        private static void Load(string path, string game)
        {
            var settingsPath = Path.Combine(path, "settings");
            Directory.CreateDirectory(settingsPath);
            var oldPath = Path.Combine(settingsPath, $"{ game }.settings.cfg");
            var newPath = Path.Combine(path, "settings.cfg");
            File.Copy(oldPath, newPath, true);
        }
    }
}
