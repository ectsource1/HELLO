using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace VoiceRecorder.Audio
{
    public interface IAudioRecorder
    {
        event EventHandler Stopped;

        void BeginMonitoring(int recordingDevice);
        void BeginRecording(string path);
        void Stop();
        double MicrophoneLevel { get; set; }
        RecordingState RecordingState { get; }
        SampleAggregator SampleAggregator { get; }
        WaveFormat RecordingFormat { get; set; }
        TimeSpan RecordedTime { get; }
    }
}
