﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinDynamicDesktop
{
    class UwpHelper : PlatformHelper
    {
        private bool startOnBoot;

        public override string GetLocalFolder()
        {
            return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        }

        public override async void CheckStartOnBoot()
        {
            var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync(
                "WinDynamicDesktopUwp");

            switch (startupTask.State)
            {
                case Windows.ApplicationModel.StartupTaskState.Disabled:
                    startOnBoot = false;
                    break;
                case Windows.ApplicationModel.StartupTaskState.DisabledByUser:
                    startOnBoot = false;
                    MainMenu.startOnBootItem.Enabled = false;
                    break;
                case Windows.ApplicationModel.StartupTaskState.Enabled:
                    startOnBoot = true;
                    break;
            }

            MainMenu.startOnBootItem.Checked = startOnBoot;
        }

        public override async void ToggleStartOnBoot()
        {
            var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync(
                "WinDynamicDesktopUwp");

            if (!startOnBoot)
            {
                var state = await startupTask.RequestEnableAsync();

                switch (state)
                {
                    case Windows.ApplicationModel.StartupTaskState.DisabledByUser:
                        startOnBoot = false;
                        break;
                    case Windows.ApplicationModel.StartupTaskState.Enabled:
                        startOnBoot = true;
                        break;
                }
            }
            else
            {
                startupTask.Disable();
                startOnBoot = false;
            }

            MainMenu.startOnBootItem.Checked = startOnBoot;
        }

        public override async void OpenUpdateLink()
        {
            await Windows.System.Launcher.LaunchUriAsync(
                new Uri("ms-windows-store://downloadsandupdates"));
        }

        public static async void SetLockScreenImage(string imageFilename)
        {
            var uri = new Uri("ms-appdata:///local/themes/" + ThemeManager.currentTheme.themeId +
                "/" + imageFilename);
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);

            var profileSettings =
                Windows.System.UserProfile.UserProfilePersonalizationSettings.Current;
            await profileSettings.TrySetLockScreenImageAsync(file);
        }
    }
}
