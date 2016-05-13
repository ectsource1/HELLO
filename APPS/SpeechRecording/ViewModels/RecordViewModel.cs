// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using Microsoft.Win32;
using System.Windows;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NAudio.Wave;
using System.IO;
using SpeechRecording.Properties;
using VoiceRecorder.Audio;

namespace SpeechRecording.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RecordViewModel : BindableBase, INavigationAware
    {
        private readonly DelegateCommand goBackCommand;
        private IRegionNavigationJournal navigationJournal;

        // recording
        private readonly IAudioRecorder recorder;

        private readonly DelegateCommand stopCommand;
        private readonly DelegateCommand beginRecordingCommand;

        private int selectedRecordingDeviceIndex;
        private ObservableCollection<string> recordingDevices;

        private float lastPeak;
        private string waveFileName;
        bool stopClickable = false;
        bool startClickable = true;
        bool showWaveForm = false;
        float currentInputLevel = 0;
        double microphoneLevel = 0;
        string recordedTime = "";
        String status = "RECORDING READY ...";

        // play recordings
        private IAudioPlayer audioPlayer;

        private readonly DelegateCommand playCommand;
        private readonly DelegateCommand stopPlayCommand;
        private readonly DelegateCommand saveCommand;
        private readonly DelegateCommand autoTuneCommand;
        private readonly DelegateCommand selectAllCommand;

        private SampleAggregator playSampleAggregator;
        private int leftPosition;
        private int rightPosition;
        private int totalWaveFormSamples;
        bool playClickable = false;
        bool playStopClickable = false;

        private int samplesPerSecond;
        //private VoiceRecorderState voiceRecorderState;

        /// Enum used by the combobox and the view model.
        public enum PlayOptions
        {
            All     = 1,
            Trimmed = 2,
        }

        List<ComboBoxItemPlay> playTypeEnum;

        [ImportingConstructor]
        //public RecordViewModel(IAudioRecorder recorder)
        public RecordViewModel()
        {
            // recording
            this.goBackCommand = new DelegateCommand(this.GoBack);
            this.beginRecordingCommand = new DelegateCommand(this.BeginRecording);
            this.stopCommand = new DelegateCommand(this.Stop);
            this.recordingDevices = new ObservableCollection<string>();
            for (int n = 0; n < WaveIn.DeviceCount; n++){
                this.recordingDevices.Add(WaveIn.GetCapabilities(n).ProductName);
            }
            this.recorder = new AudioRecorder(); ; 
            this.recorder.Stopped += OnRecorderStopped;
            recorder.SampleAggregator.MaximumCalculated += OnRecorderMaximumCalculated;

            // Play recording
            this.PlaySampleAggregator = new SampleAggregator();
            playSampleAggregator.NotificationCount = 800; // gets set correctly later on
            this.audioPlayer = new AudioPlayer();
            this.audioPlayer.Stopped += OnPlaybackStopped;
            this.playCommand = new DelegateCommand(this.Play);
            this.stopPlayCommand = new DelegateCommand(this.StopPlay);
            this.saveCommand = new DelegateCommand(this.Save);
            this.autoTuneCommand = new DelegateCommand(OnAutoTune);
            this.selectAllCommand = new DelegateCommand(this.SelectAll);

            playTypeEnum = new List<ComboBoxItemPlay>() {
               new ComboBoxItemPlay(){ ValuePlayEnum = RecordViewModel.PlayOptions.All,
                                       ValuePlayString = "All" },
               new ComboBoxItemPlay(){ ValuePlayEnum = RecordViewModel.PlayOptions.Trimmed,
                                       ValuePlayString = "Trimmed" },
            };
        }

        private RecordViewModel.PlayOptions playType = RecordViewModel.PlayOptions.All;
        public RecordViewModel.PlayOptions PlayType
        {
            get { return playType; }
            set
            {
                this.SetProperty(ref this.playType, value);
            }
        }

        public List<ComboBoxItemPlay> PlayTypeEnum
        {
            get { return playTypeEnum; }
        }

        void OnRecorderStopped(object sender, EventArgs e)
        {
            
            this.Status = "Recording Stopped";
            this.StartClickable = true;
            this.stopClickable = false;
            this.PlayClickable = true;
            this.playStopClickable = false;
        }

        void OnPlaybackStopped(object sender, EventArgs e)
        {
            StopPlay();
        }

        void OnRecorderMaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            lastPeak = Math.Max(e.MaxSample, Math.Abs(e.MinSample));
            this.CurrentInputLevel = lastPeak * 100;
            this.RecordedTime = currTime();
        }

        public string RecordedTime
        {
            get
            {
                return currTime();
            }
            set
            {
                this.SetProperty(ref this.recordedTime, value);
            }
        }

        private string currTime()
        {
            var current = recorder.RecordedTime;
            return String.Format("{0:D2}:{1:D2}.{2:D3}",
                current.Minutes, current.Seconds, current.Milliseconds);
        }

        private string fileBase()
        {
            var current = DateTime.Now;
            return ("F" + current.Day + current.Hour + current.Minute);
        }

        public ObservableCollection<string> RecordingDevices
        {
            get { return recordingDevices; }
        }

        public int SelectedIndex
        {
            get
            {
                return selectedRecordingDeviceIndex;
            }
            set
            {
                if (selectedRecordingDeviceIndex != value)
                {
                    this.SetProperty(ref this.selectedRecordingDeviceIndex, value);
                }
            }
        }


        public bool StartClickable
        {
            get
            {
                return this.startClickable;
            }

            set
            {
                this.SetProperty(ref this.startClickable, value);
            }
        }

        public bool StopClickable
        {
            get
            {
                return this.stopClickable;
            }

            set
            {
                this.SetProperty(ref this.stopClickable, value);
            }
        }

        public string Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.SetProperty(ref this.status, value);
            }
        }

        public double MicrophoneLevel
        {
            get { return recorder.MicrophoneLevel; }
            set
            {
                recorder.MicrophoneLevel = value;
                this.SetProperty(ref this.microphoneLevel, value);
            }
        }

        public bool ShowWaveForm
        {
            get
            {
                return recorder.RecordingState == RecordingState.Recording ||
                       recorder.RecordingState == RecordingState.RequestedStop;
            }
            set
            {
                this.SetProperty(ref this.showWaveForm, value);
            }

        }

        // multiply by 100 because the Progress bar's default maximum value is 100
        public float CurrentInputLevel
        {
            get { return lastPeak * 100; }
            set
            {
                this.SetProperty(ref this.currentInputLevel, value);
            }
        }

        public SampleAggregator SampleAggregator
        {
            get
            {
                return recorder.SampleAggregator;
            }
        }

        public ICommand GoBackCommand
        {
            get { return this.goBackCommand; }
        }

        public ICommand BeginRecordingCommand
        {
            get { return this.beginRecordingCommand; }
        }

        public ICommand StopCommand
        {
            get { return this.stopCommand; }
        }

        public ICommand PlayCommand
        {
            get { return this.playCommand; }
        }

        public ICommand StopPlayCommand
        {
            get { return this.stopPlayCommand; }
        }

        public ICommand SaveCommand
        {
            get { return this.saveCommand; }
        }

        public ICommand AutoTuneCommand
        {
            get { return this.autoTuneCommand; }
        }

        public ICommand SelectAllCommand
        {
            get { return this.selectAllCommand; }
        }


        private void GoBack()
        {
            // todo: 15 - Using the journal to navigate back.
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }
        private void BeginRecording()
        {
            if (selectedRecordingDeviceIndex < 0) return;

            if (recorder.RecordingState == RecordingState.Stopped ||
                recorder.RecordingState == RecordingState.Monitoring)
            {
                if (recorder.RecordingState == RecordingState.Stopped)
                      BeginMonitoring(selectedRecordingDeviceIndex);

                this.WaveFileName = Path.Combine(Path.GetTempPath(), fileBase() + ".wav");
                recorder.BeginRecording(waveFileName);
                this.ShowWaveForm = recorder.RecordingState == RecordingState.Recording ||
                                    recorder.RecordingState == RecordingState.RequestedStop;
                this.StartClickable = false;
                this.StopClickable = true;
                this.PlayClickable = false;
                this.playStopClickable = false;
            }
        }

        private void BeginMonitoring(int recordingDevice)
        {
            recorder.BeginMonitoring(recordingDevice);
            this.MicrophoneLevel = recorder.MicrophoneLevel;
        }

        private void Stop()
        {
            if (recorder.RecordingState == RecordingState.Recording)
            {
                recorder.Stop();
            }        
        }

        private void Play()
        {
            if (playType == PlayOptions.All) RenderFile();
            audioPlayer.CurrentPosition = PositionToTimeSpan(leftPosition);
            audioPlayer.StartPosition = PositionToTimeSpan(leftPosition);
            audioPlayer.EndPosition = PositionToTimeSpan(rightPosition);
            audioPlayer.Play();

            this.StartClickable = false;
            this.StopClickable = false;
            this.PlayClickable = false;
            this.PlayStopClickable = true;
        }

        private void StopPlay()
        {
            audioPlayer.Stop();

            this.StartClickable = true;
            this.StopClickable = false;
            this.PlayClickable = true;
            this.PlayStopClickable = false;
        }

        private void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "WAV file (.wav)|*.wav|MP3 file (.mp3)|.mp3";
            saveFileDialog.DefaultExt = ".wav";
            bool? result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                SaveAs(saveFileDialog.FileName);
            }
        }

        private void OnAutoTune()
        {
            audioPlayer.Dispose(); // needed to relinquish the file as it may get deleted
            //Messenger.Default.Send(new NavigateMessage("AutoTuneView", this.voiceRecorderState));
        }

        private void SelectAll()
        {
            this.LeftPosition = 0;
            this.RightPosition = TotalWaveFormSamples;
        }

        private void SaveAs(string fileName)
        {
            AudioSaver saver = new AudioSaver(waveFileName);
            saver.TrimFromStart = PositionToTimeSpan(LeftPosition);
            saver.TrimFromEnd = PositionToTimeSpan(TotalWaveFormSamples - RightPosition);

            if (fileName.ToLower().EndsWith(".wav"))
            {
                saver.SaveFileFormat = SaveFileFormat.Wav;
                saver.SaveAudio(fileName);
            }
            else if (fileName.ToLower().EndsWith(".mp3"))
            {
                string lameExePath = LocateLame();
                if (lameExePath != null)
                {
                    saver.SaveFileFormat = SaveFileFormat.Mp3;
                    saver.LameExePath = lameExePath;
                    saver.SaveAudio(fileName);
                }
            }
            else
            {
                MessageBox.Show("Please select a supported output format");
            }
        }

        private TimeSpan PositionToTimeSpan(int position)
        {
            int samples = SampleAggregator.NotificationCount * position;
            return TimeSpan.FromSeconds((double)samples / samplesPerSecond);
        }

        public string LocateLame()
        {
            string lameExePath = Settings.Default.LameExePath;
            //string lameExePath = "C:\\APPS\\bin\\Lame";

            //if (String.IsNullOrEmpty(lameExePath) || !File.Exists(lameExePath))
            if (!File.Exists(lameExePath))
            {
                if (MessageBox.Show("To save as MP3 requires LAME.exe, please locate",
                    "Save as MP3",
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.FileName = "lame.exe";
                    bool? result = ofd.ShowDialog();
                    if (result != null && result.HasValue)
                    {
                        if (File.Exists(ofd.FileName) && ofd.FileName.ToLower().EndsWith("lame.exe"))
                        {
                            Settings.Default.LameExePath = ofd.FileName;
                            Settings.Default.Save();
                            return ofd.FileName;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return lameExePath;
        }

        private void RenderFile()
        {
            //string tmpFile = "C:\\ECTData\\Cartoons\\LionAndMouse\\test1.mp3";
            playSampleAggregator.RaiseRestart();
            using (WaveFileReader reader = new WaveFileReader(this.waveFileName))
            {
                this.samplesPerSecond = reader.WaveFormat.SampleRate;
                SampleAggregator.NotificationCount = reader.WaveFormat.SampleRate / 10;

                byte[] buffer = new byte[1024];
                WaveBuffer waveBuffer = new WaveBuffer(buffer);
                waveBuffer.ByteBufferCount = buffer.Length;
                int bytesRead;
                do
                {
                    bytesRead = reader.Read(waveBuffer, 0, buffer.Length);
                    int samples = bytesRead / 2;
                    for (int sample = 0; sample < samples; sample++)
                    {
                        if (bytesRead > 0)
                        {
                            playSampleAggregator.Add(waveBuffer.ShortBuffer[sample] / 32768f);
                        }
                    }
                } while (bytesRead > 0);
                int totalSamples = (int)reader.Length / 2;
                TotalWaveFormSamples = totalSamples / playSampleAggregator.NotificationCount;
                SelectAll();
            }
            audioPlayer.LoadFile(this.waveFileName);
        }


        //Play Recording
        public SampleAggregator PlaySampleAggregator
        {
            get
            {
                return playSampleAggregator;
            }
            set
            {
                if (playSampleAggregator != value)
                {
                    this.SetProperty(ref this.playSampleAggregator, value);
                }
            }
        }

        public int LeftPosition
        {
            get
            {
                return leftPosition;
            }
            set
            {
                if (leftPosition != value)
                {
                    this.SetProperty(ref this.leftPosition, value);
                }
            }
        }

        public int RightPosition
        {
            get
            {
                return rightPosition;
            }
            set
            {
                if (rightPosition != value)
                {
                    this.SetProperty(ref this.rightPosition, value);
                }
            }
        }

        public int TotalWaveFormSamples
        {
            get
            {
                return totalWaveFormSamples;
            }
            set
            {
                if (totalWaveFormSamples != value)
                {
                    this.SetProperty(ref this.totalWaveFormSamples, value);
                }
            }
        }

        public string WaveFileName
        {
            get
            {
                return this.waveFileName;
            }

            set
            {
                this.SetProperty(ref this.waveFileName, value);
            }
        }

        public bool PlayStopClickable
        {
            get
            {
                return this.playStopClickable;
            }

            set
            {
                this.SetProperty(ref this.playStopClickable, value);
            }
        }

        public bool PlayClickable
        {
            get
            {
                return this.playClickable;
            }

            set
            {
                this.SetProperty(ref this.playClickable, value);
            }
        }

        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Intentionally not implemented.
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        #endregion
    }
}
