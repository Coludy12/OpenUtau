﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenUtau.Core;
using OpenUtau.Core.Util;
using ReactiveUI;
using static OpenUtau.Core.ResamplerDriver.DriverModels;

namespace OpenUtau.App.ViewModels {
    public class PreferencesViewModel : ViewModelBase {
        public List<AudioOutputDevice>? AudioOutputDevices {
            get => audioOutputDevices;
            set => this.RaiseAndSetIfChanged(ref audioOutputDevices, value);
        }
        public AudioOutputDevice? AudioOutputDevice {
            get => audioOutputDevice;
            set => this.RaiseAndSetIfChanged(ref audioOutputDevice, value);
        }
        public List<int> PrerenderThreadsItems { get; }
        public int PrerenderThreads {
            get => prerenderThreads;
            set => this.RaiseAndSetIfChanged(ref prerenderThreads, value);
        }
        public List<EngineInfo>? Resamplers { get; }
        public EngineInfo? PreviewResampler {
            get => previewResampler;
            set => this.RaiseAndSetIfChanged(ref previewResampler, value);
        }
        public EngineInfo? ExportResampler {
            get => exportResampler;
            set => this.RaiseAndSetIfChanged(ref exportResampler, value);
        }
        public int Theme {
            get => theme;
            set => this.RaiseAndSetIfChanged(ref theme, value);
        }

        private List<AudioOutputDevice>? audioOutputDevices;
        private AudioOutputDevice? audioOutputDevice;
        private int prerenderThreads;
        private EngineInfo? previewResampler;
        private EngineInfo? exportResampler;
        private int theme;

        public PreferencesViewModel() {
            var audioOutput = PlaybackManager.Inst.AudioOutput;
            if (audioOutput != null) {
                audioOutputDevices = audioOutput.GetOutputDevices();
                int deviceNumber = audioOutput.DeviceNumber;
                if (audioOutputDevices.Count > deviceNumber) {
                    audioOutputDevice = audioOutputDevices[deviceNumber];
                }
            }
            PrerenderThreadsItems = Enumerable.Range(1, 16).ToList();
            PrerenderThreads = Preferences.Default.PrerenderThreads;
            Resamplers = Core.ResamplerDriver.ResamplerDriver.Search(PathManager.Inst.GetEngineSearchPath());
            if (Resamplers.Count > 0) {
                int index = Resamplers.FindIndex(resampler => resampler.Name == Preferences.Default.ExternalPreviewEngine);
                if (index >= 0) {
                    previewResampler = Resamplers[index];
                } else {
                    previewResampler = null;
                }
                index = Resamplers.FindIndex(resampler => resampler.Name == Preferences.Default.ExternalExportEngine);
                if (index >= 0) {
                    exportResampler = Resamplers[index];
                } else {
                    exportResampler = null;
                }
            }
            theme = Preferences.Default.Theme;

            this.WhenAnyValue(vm => vm.AudioOutputDevice)
                .WhereNotNull()
                .Subscribe(device => {
                    if (PlaybackManager.Inst.AudioOutput != null) {
                        PlaybackManager.Inst.AudioOutput.SelectDevice(device.guid, device.deviceNumber);
                        Preferences.Default.PlaybackDevice = device.guid.ToString();
                        Preferences.Default.PlaybackDeviceNumber = device.deviceNumber;
                        Preferences.Save();
                    }
                });
            this.WhenAnyValue(vm => vm.PrerenderThreads)
                .Subscribe(threads => {
                    Preferences.Default.PrerenderThreads = threads;
                    Preferences.Save();
                });
            this.WhenAnyValue(vm => vm.PreviewResampler)
                .WhereNotNull()
                .Subscribe(resampler => {
                    Preferences.Default.ExternalPreviewEngine = resampler?.Name;
                    Preferences.Save();
                });
            this.WhenAnyValue(vm => vm.ExportResampler)
                .WhereNotNull()
                .Subscribe(resampler => {
                    Preferences.Default.ExternalExportEngine = resampler?.Name;
                    Preferences.Save();
                });
            this.WhenAnyValue(vm => vm.Theme)
                .Subscribe(theme => {
                    Preferences.Default.Theme = theme;
                    Preferences.Save();
                });
        }

        public void TestAudioOutputDevice() {
            PlaybackManager.Inst.PlayTestSound();
        }

    }
}
