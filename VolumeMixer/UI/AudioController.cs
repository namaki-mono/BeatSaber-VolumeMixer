using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using BeatSaberMarkupLanguage.Components.Settings;
using System.Threading.Tasks;

namespace VolumeMixer.UI
{
    [HotReload(RelativePathToLayout = @"Views\AudioView.bsml")]
    [ViewDefinition("VolumeMixer.UI.Views.AudioView.bsml")]
    internal class AudioController : BSMLAutomaticViewController
    {
        private readonly Timer delayTimer;
        private readonly int delayTime = 75; // Debounce timer in ms

        private bool firstRun = true;

        [UIValue("active")]
        private bool Active { get; set; } = true;

        [UIValue("volume")]
        private float Volume { get; set; } = 0.0f;
        private float prevVolume;

        [UIValue("muted")]
        private string MutedText { get; set; } = "🔈";

        [UIValue("devices")]
        private List<object> Devices { get; set; } = new object[] { "" }.ToList();
        private List<object> devicesClfID = new object[] { "" }.ToList();

        [UIComponent("source-dropdown")]
        private readonly DropDownListSetting sourceDropdown;

        [UIValue("device")]
        private string Device { get; set; } = "";
        private string clfID;

        public AudioController()
        {
            GetAllDevices();

            // Debounce trick to prevent hundreds of calls on slider change
            delayTimer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite); 
        }

        private void Callback(object state)
        {
            // Only change the volume if a volume change has not been detected within delayTime ms
            if (prevVolume != Volume)
            {
                prevVolume = Volume;
                delayTimer.Change(delayTime, Timeout.Infinite);
                return;
            }

            SetVolume(clfID, Volume);
        }

        [UIAction("volume-slider-change")]
        private void VolumeSliderChange(float val)
        {
            Volume = val;
            delayTimer.Change(delayTime, Timeout.Infinite);
        }

        [UIAction("mute-button-click")]
        private void MuteButtonClick()
        {
            if (MutedText.Equals("🔈") == true)
            {
                SetMute(clfID, false);
            }
            else if (MutedText.Equals("🔊") == true)
            {
                SetMute(clfID, true);
            }
        }

        [UIAction("device-dropdown-change")]
        private void DeviceDropdownChange(string val)
        {
            // Hide the volume slider if the blank entry is selected
            Active = !val.Equals("");
            NotifyPropertyChanged("Active");

            // Devices and clfID lists will always match
            int index = Devices.IndexOf(val);
            clfID = (string) devicesClfID[index];

            GetVolume(clfID);
            GetMuted(clfID);

            GetAllDevices(); // Refresh the list on change
        }

        private void GetAllDevices()
        {
            // Reset lists whenever we refresh the devices list
            Devices = new object[] { "" }.ToList();
            devicesClfID = new object[] { "" }.ToList();

            StringBuilder receivedData = new StringBuilder();
            bool isIncompleteJson = false;

            var proc = new Process();
            proc.StartInfo.FileName = $"{Environment.CurrentDirectory}\\svcl.exe";
            proc.StartInfo.Arguments = $"/Stdout /sjson";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = false;
            proc.OutputDataReceived += (sender, e) =>
            {
                try
                {
                    receivedData.Append(e.Data);

                    if (e.Data.Trim().Contains("[") && isIncompleteJson == false)
                    {
                        // The JSON returned from svcl.exe is wack, this is the workaround
                        receivedData.Clear();
                        receivedData.Append("[");
                        isIncompleteJson = true;
                    }

                    if (e.Data.Trim().EndsWith("]") && isIncompleteJson == true)
                    {
                        isIncompleteJson = false;

                        var json = receivedData.ToString();
                        var jsonArray = JArray.Parse(json);

                        foreach (var item in jsonArray)
                        {
                            var direction = item["Direction"]?.ToString();
                            var name = item["Name"]?.ToString();
                            var clfID = item["Command-Line Friendly ID"]?.ToString();
                            var deviceName = item["Device Name"]?.ToString();
                            var deviceState = item["Device State"]?.ToString();
                            var processID = item["Process ID"]?.ToString();

                            // Filter out capture devices
                            if (direction != null && direction.Equals("Render") == true)
                            {
                                // Filter out inactive devices
                                if ((deviceState != null && deviceState.Equals("Active") == true) ||
                                    (processID != null && processID.Equals("") == false))
                                {
                                    // First 4 characters are a truncated device name, then a slash, then source audio name. 20 character limit
                                    string shortenedDeviceName = deviceName.Substring(0, Math.Min(deviceName.Length, 4)).PadRight(4, '-');
                                    string limitedName = $"{shortenedDeviceName}/{name}";
                                    limitedName = limitedName.Substring(0, Math.Min(limitedName.Length, 20));

                                    Devices.Add(limitedName);
                                    devicesClfID.Add(clfID);
                                }
                            }
                        }

                        if (firstRun == true)
                        {
                            firstRun = false;

                            if (Devices.Count > 1)
                            {
                                Device = (string)Devices[1];
                                NotifyPropertyChanged("Device");

                                clfID = (string)devicesClfID[1];
                                GetVolume(clfID);
                                GetMuted(clfID);
                            }
                        }
                        // NotifyPropertyChanged doesn't work for dropdowns - janky workaround
                        sourceDropdown.values = Devices;
                        sourceDropdown.UpdateChoices();
                    }
                }
                catch (Exception)
                {
                    // Do nothing
                }
            };
            proc.Start();
            proc.BeginOutputReadLine();
        }

        private void GetMuted(string process)
        {
            var proc = new Process();
            proc.StartInfo.FileName = $"{Environment.CurrentDirectory}\\svcl.exe";
            proc.StartInfo.Arguments = $"/Stdout /GetMute \"{process}\"";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = false;
            proc.OutputDataReceived += (sender, e) =>
            {
                try
                {
                    int mutedBit = int.Parse(e.Data);
                    MutedText = (mutedBit == 0) ? "🔊" : "🔈";
                    NotifyPropertyChanged("MutedText");
                }
                catch (Exception)
                {
                    // Do nothing
                }
            };
            proc.Start();
            proc.BeginOutputReadLine();
        }

        private void SetMute(string process, bool muted)
        {
            try
            {
                var proc = new Process();
                proc.StartInfo.FileName = $"{Environment.CurrentDirectory}\\svcl.exe";
                proc.StartInfo.Arguments = muted ? $"/Mute \"{process}\"" : $"/Unmute \"{process}\"";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.BeginOutputReadLine();
                proc.WaitForExit();
            }
            catch (Exception)
            {
                // Do nothing
            }
            MutedText = muted ? "🔈" : "🔊";
            NotifyPropertyChanged("MutedText");
        }

        private void GetVolume(string process)
        {
            var proc = new Process();
            proc.StartInfo.FileName = $"{Environment.CurrentDirectory}\\svcl.exe";
            proc.StartInfo.Arguments = $"/Stdout /GetPercent \"{process}\"";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = false;
            proc.OutputDataReceived += (sender, e) =>
            {
                try
                {
                    Volume = float.Parse(e.Data);
                    prevVolume = Volume;
                    NotifyPropertyChanged("Volume");
                }
                catch (Exception)
                {
                    // Do nothing
                }
            };
            proc.Start();
            proc.BeginOutputReadLine();
        }

        private void SetVolume(string process, float val)
        {
            try
            {
                var proc = new Process();
                proc.StartInfo.FileName = $"{Environment.CurrentDirectory}\\svcl.exe";
                proc.StartInfo.Arguments = $"/SetVolume \"{process}\" {val}";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.BeginOutputReadLine();
                proc.WaitForExit();

            }
            catch (Exception)
            {
                // Do nothing
            }
        }

        public static void Show()
        {
            UIWindow.floatingScreen.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            UIWindow.floatingScreen.gameObject.SetActive(false);
        }
    }
}