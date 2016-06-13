// Copyright (c) Microsoft Corporation. All rights reserved. 
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using System.IO;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using SpeechTTS.Model;
using SpeechControls;
using SpeechInfrastructure;
using NAudio.Wave;
using NAudio.Lame;
using System.Threading;
using SpeechTTS.Auth;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Pronunciation.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SoundViewModel : BindableBase, INavigationAware
    {
        private readonly DelegateCommand goBackCommand;
        private readonly DelegateCommand resetCommand;
        private readonly DelegateCommand mp3Command;
        private readonly DelegateCommand speakCommand;
        private readonly DelegateCommand pauseCommand;
        private readonly DelegateCommand resumeCommand;
        private readonly DelegateCommand stopCommand;
        private readonly DelegateCommand preCommand;
        private readonly DelegateCommand nextCommand;
        private readonly DelegateCommand speakWordCommand;

        private readonly DelegateCommand playMediaCommand;
        private readonly DelegateCommand pauseMediaCommand;
        private readonly DelegateCommand resumeMediaCommand;
        private readonly DelegateCommand stopMediaCommand;

        private readonly ITTService ttsService;
        private TextDocument textDocument;
        private IRegionNavigationJournal navigationJournal;
        private const string TextIdKey = "TextId";
        private List<string> voiceOptions;
        private string selectedVoice="Male";
        private int volume = 100;
        private int rate = 10;

        private bool preClickable = false;
        private bool nextClickable = false;

        private bool speakClickable = true;
        private bool stopClickable = false;
        private bool resumeClickable = false;

        private bool mediaPlayClickable = true;
        private bool mediaStopClickable = false;
        private bool mediaResumeClickable = false;

        private bool videoLoaded = false;
        private bool videoNotLoaded = true;
        private bool mediaLoaded = false;

        private string selectedText;
        private List<int> repeatOptions;
        private List<int> fontSizeOptions;
        private int repeat = 5;
        private int fontSize = 16;
        private int repeatCnt = 0;

        SpeechSynthesizer voice; 
        private bool wordSpeak = false;
        private string message = "";

        private double mediaVolume = 1;
        private double maxTime = 1;
        private double mediaProgress = 0;

        FsRichTextBox fsRichTextBox;
        MediaElement media;
        Image image;


        [ImportingConstructor]
        public SoundViewModel(ITTService ttsService)
        {
            this.goBackCommand = new DelegateCommand(this.GoBack);
            this.resetCommand = new DelegateCommand(this.Reset);
            this.mp3Command = new DelegateCommand(this.Mp3);
            this.speakCommand = new DelegateCommand(this.Speak);
            this.pauseCommand = new DelegateCommand(this.Pause);
            this.resumeCommand = new DelegateCommand(this.Resume);
            this.stopCommand = new DelegateCommand(this.Stop);
            this.preCommand = new DelegateCommand(this.Pre);
            this.nextCommand = new DelegateCommand(this.Next);
            this.speakWordCommand = new DelegateCommand(this.SpeakWord);

            this.playMediaCommand = new DelegateCommand(this.PlayMedia);
            this.pauseMediaCommand = new DelegateCommand(this.PauseMedia);
            this.resumeMediaCommand = new DelegateCommand(this.ResumeMedia);
            this.stopMediaCommand = new DelegateCommand(this.StopMedia);

            this.ttsService = ttsService;

            voiceOptions = new List<string>
            {
                "Male",
                "Female"
            };

            repeatOptions = new List<int>
            {
                5,
                10,
                15,
                20
            };

            fontSizeOptions = new List<int>
            {
                12,
                16,
                20,
                24,
                26,
                28,
                30,
                32
            };

            voice = new SpeechSynthesizer();
            voice.SpeakCompleted += OnSpeakCompleted;
            voice.SpeakProgress += OnWord;

            selectedText = "";
        }

        public void MediaEnded()
        {
            media.Position = TimeSpan.FromSeconds(0);
            wordSpeak = false;
            this.PreClickable = true;
            this.NextClickable = true;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = true;
            media.Stop();
        }

        public void setEditbox(FsRichTextBox txtBox)
        {
            fsRichTextBox = txtBox;
            fsRichTextBox.FontSize = fontSize;
        }

        public void setImage(System.Windows.Controls.Image img)
        {
            image = img;
        }

        public void setAudio(MediaElement maudio)
        {
            media = maudio;
        }

        void OnSpeakCompleted(object sender, EventArgs e)
        {
            this.Stop();

            if (repeatCnt < repeat)
            {
                this.SpeakWord();
            }
            else
            {
                this.RepeatCnt = 0;
            }

            this.Message = "Done Reading!!";
        }

        void OnWord(object sender, SpeakProgressEventArgs e)
        {
            if (!wordSpeak)
                fsRichTextBox.HighlightWordInRichTextBox(e.Text);
        }

        public ICommand GoBackCommand
        {
            get { return this.goBackCommand; }
        }

        public ICommand ResetCommand
        {
            get { return this.resetCommand; }
        }

        public ICommand Mp3Command
        {
            get { return this.mp3Command; }
        }

        public ICommand SpeakCommand
        {
            get { return this.speakCommand; }
        }

        public ICommand SpeakWordCommand
        {
            get { return this.speakWordCommand; }
        }

        public ICommand PauseCommand
        {
            get { return this.pauseCommand; }
        }

        public ICommand ResumeCommand
        {
            get { return this.resumeCommand; }
        }

        public ICommand StopCommand
        {
            get { return this.stopCommand; }
        }

        public ICommand PreCommand
        {
            get { return this.preCommand; }
        }

        public ICommand NextCommand
        {
            get { return this.nextCommand; }
        }

        public ICommand PlayMeddiaCommand
        {
            get { return this.playMediaCommand; }
        }

        public ICommand PauseMediaCommand
        {
            get { return this.pauseMediaCommand; }
        }

        public ICommand ResumeMediaCommand
        {
            get { return this.resumeMediaCommand; }
        }

        public ICommand StopMediaCommand
        {
            get { return this.stopMediaCommand; }
        }


        public List<string> VoiceOptions
        {
            get
            {
                return this.voiceOptions;
            }
        }

        public String SelectedVoice
        {
            get
            {
                return this.selectedVoice;
            }

            set
            {
                this.SetProperty(ref this.selectedVoice, value);
            }
        }

        public List<int> RepeatOptions
        {
            get
            {
                return this.repeatOptions;
            }
        }

        public List<int> FontSizeOptions
        {
            get
            {
                return this.fontSizeOptions;
            }
        }

        public TextDocument TextDocument
        {
            get
            {
                return this.textDocument;
            }

            set
            {
                this.SetProperty(ref this.textDocument, value);
            }
        }

        public int Repeat
        {
            get
            {
                return this.repeat;
            }

            set
            {
                this.SetProperty(ref this.repeat, value);
            }
        }

        public int FontSize
        {
            get
            {
                return this.fontSize;
            }

            set
            {
                this.SetProperty(ref this.fontSize, value);
                fsRichTextBox.FontSize = fontSize;
            }
        }

        public int RepeatCnt
        {
            get
            {
                return this.repeatCnt;
            }

            set
            {
                this.SetProperty(ref this.repeatCnt, value);
            }
        }

        public string SelectedText
        {
            get
            {
                return this.selectedText;
            }

            set
            {
                this.SetProperty(ref this.selectedText, value);
            }
        }

        public int Volume
        {
            get
            {
                return this.volume;
            }

            set
            {
                this.SetProperty(ref this.volume, value);
            }
        }

        public int Rate
        {
            get
            {
                return this.rate;
            }

            set
            {
                this.SetProperty(ref this.rate, value);
            }
        }

        public bool PreClickable
        {
            get
            {
                return this.preClickable;
            }

            set
            {
                this.SetProperty(ref this.preClickable, value);
            }
        }

        public bool NextClickable
        {
            get
            {
                return this.nextClickable;
            }

            set
            {
                this.SetProperty(ref this.nextClickable, value);
            }
        }

        public bool SpeakClickable
        {
            get
            {
                return this.speakClickable;
            }

            set
            {
                this.SetProperty(ref this.speakClickable, value);
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

        public bool ResumeClickable
        {
            get
            {
                return this.resumeClickable;
            }

            set
            {
                this.SetProperty(ref this.resumeClickable, value);
            }
        }

        public bool MediaPlayClickable
        {
            get
            {
                return this.mediaPlayClickable;
            }

            set
            {
                this.SetProperty(ref this.mediaPlayClickable, value);
            }
        }

        public bool MediaStopClickable
        {
            get
            {
                return this.mediaStopClickable;
            }

            set
            {
                this.SetProperty(ref this.mediaStopClickable, value);
            }
        }

        public bool MediaResumeClickable
        {
            get
            {
                return this.mediaResumeClickable;
            }

            set
            {
                this.SetProperty(ref this.mediaResumeClickable, value);
            }
        }

        public bool VideoLoaded
        {
            get
            {
                return this.videoLoaded;
            }

            set
            {
                this.SetProperty(ref this.videoLoaded, value);
            }
        }

        public bool VideoNotLoaded
        {
            get
            {
                return this.videoNotLoaded;
            }

            set
            {
                this.SetProperty(ref this.videoNotLoaded, value);
            }
        }

        public bool MediaLoaded
        {
            get
            {
                return this.mediaLoaded;
            }

            set
            {
                this.SetProperty(ref this.mediaLoaded, value);
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                this.SetProperty(ref this.message, value);
            }
        }

        public double MediaVolume
        {
            get
            {
                return this.mediaVolume;
            }

            set
            {
                this.SetProperty(ref this.mediaVolume, value);
                media.Volume = mediaVolume;
            }
        }

        public double MaxTime
        {
            get
            {
                return this.maxTime;
            }

            set
            {
                this.SetProperty(ref this.maxTime, value);
            }
        }

        public double MediaProgress
        {
            get
            {
                return this.mediaProgress;
            }

            set
            {
                this.SetProperty(ref this.mediaProgress, value);
                media.Position = TimeSpan.FromSeconds(mediaProgress);
            }
        }


        private void GoBack()
        {
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }

        private void Reset()
        {
            this.Rate = 10;
            this.Volume = 100;
        }

        private void Mp3()
        {
            wordSpeak = false;
            this.preClickable = false;
            this.nextClickable = false;
            this.StopClickable = true;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            this.Message = "Saving To MP3...";

            switch (selectedVoice)
            {
                case "Male":
                    voice.SelectVoiceByHints(VoiceGender.Male);
                    break;
                case "Female":
                    voice.SelectVoiceByHints(VoiceGender.Female);
                    break;
            }

            voice.Volume = volume;
            voice.Rate = rate - 10;

            //save to memory stream
            MemoryStream ms = new MemoryStream();
            voice.SetOutputToWaveStream(ms);
            voice.Speak(PlainText());

            //now convert to mp3 using LameEncoder or shell out to audiograbber
            string fileName = ttsService.getAudioPath() + 
                              DateTime.Now.ToString("yyyyMMddhh") + ".mp3";
            ConvertWavStreamToMp3File(ref ms, fileName);

            Stop();
            this.Message = "Saved MP3!!";
        }

        public static void ConvertWavStreamToMp3File(ref MemoryStream ms, string savetofilename)
        {
            //rewind to beginning of stream
            
            ms.Seek(0, SeekOrigin.Begin);

            using (var retMs = new MemoryStream())
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(savetofilename, rdr.WaveFormat, LAMEPreset.VBR_90))
            {
                rdr.CopyTo(wtr);
            }
        }

        private void Speak()
        {
            wordSpeak = false;
            this.PreClickable = false;
            this.NextClickable = false;
            this.StopClickable = true;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            this.Message = "Reading Text...";

            switch (selectedVoice)
            {
                case "Male":
                    voice.SelectVoiceByHints(VoiceGender.Male);
                    break;
                case "Female":
                    voice.SelectVoiceByHints(VoiceGender.Female);
                    break;
            }

            if (selectedVoice.Equals("MyVoice"))
            {
                media.Play();

            } else {
                voice.Volume = volume;
                voice.Rate = rate - 10;
                voice.SetOutputToDefaultAudioDevice();
                fsRichTextBox.ResetPointer();
                voice.SpeakAsync(PlainText());
            }
            
        }

        private void SpeakWord()
        {
            if (selectedText.Length < 2)
                return;

            if (repeat == 0) Repeat = 1;

            wordSpeak = true;
            this.PreClickable = false;
            this.NextClickable = false;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            this.RepeatCnt += 1;

            switch (selectedVoice)
            {
                case "Male":
                    voice.SelectVoiceByHints(VoiceGender.Male);
                    break;
                case "Female":
                    voice.SelectVoiceByHints(VoiceGender.Female);
                    break;
            }

            voice.Volume = volume;
            voice.Rate = Rate - 10;
            voice.SpeakAsync(selectedText);
        }

        private void Pause()
        {
            voice.Pause();
            this.StopClickable = false;
            this.SpeakClickable = false;
            this.ResumeClickable = true;
        }

        private void Resume()
        {
            voice.Resume();
            this.StopClickable = true;
            this.SpeakClickable = false;
            this.ResumeClickable = false;
        }

        private void Stop()
        {
            voice.SpeakAsyncCancelAll();

            this.PreClickable = true;
            this.NextClickable = true;
            this.StopClickable = false;
            this.SpeakClickable = true;
            this.ResumeClickable = false;
        }

        public void changeVisible()
        {
            media.Stop();
            Stop();   
        }

        private void PlayMedia()
        {
            media.LoadedBehavior = MediaState.Manual;
            wordSpeak = false;
            this.PreClickable = false;
            this.NextClickable = false;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            this.MediaPlayClickable = false;
            this.MediaStopClickable = true;
            this.MediaResumeClickable = false;
            media.Play();
        }

        private void PauseMedia()
        {
            media.Pause();
            this.MediaPlayClickable = false;
            this.MediaStopClickable = false;
            this.MediaResumeClickable = true;
        }

        private void ResumeMedia()
        {
            media.Play();
            this.MediaPlayClickable = false;
            this.MediaStopClickable = true;
            this.MediaResumeClickable = false;
        }

        private void StopMedia()
        {
            media.Stop();

            this.PreClickable = true;
            this.NextClickable = true;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = true;

            this.MediaPlayClickable = true;
            this.MediaStopClickable = false;
            this.MediaResumeClickable = false;
        }

        private void Pre()
        {
            TextDocument tmp = (TextDocument)this.TextDocument.Clone();
            tmp.Idx -= 1;
            if (tmp.Idx < 0) tmp.Idx = tmp.TxtList.Count - 1;
            
            tmp.Text = tmp.TxtList[tmp.Idx];
            tmp.SubSubject = tmp.SubjectList[tmp.Idx];

            string folderName = tmp.FileName.Substring(0, tmp.FileName.LastIndexOf(@"\") + 1);
            string imgName = folderName + tmp.ImgList[tmp.Idx];

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imgName);
            bitmap.EndInit();
            image.Source = bitmap;
            this.TextDocument = tmp;

            string audioName = folderName + tmp.MediaList[tmp.Idx];
            media.Source = new Uri(audioName);
            media.LoadedBehavior = MediaState.Manual;
            media.UnloadedBehavior = MediaState.Manual;
            if (audioName.Contains("mp4"))
            {
                 VideoLoaded = true;
                 VideoNotLoaded = false;
            } else
            {
                 VideoLoaded = false;
                 VideoNotLoaded = true;
            }
                     
            MediaLoaded = true;
                
            media.Play();  
        }

        private void Next()
        {
            TextDocument tmp = (TextDocument)this.TextDocument.Clone();
            int cnt = tmp.TxtList.Count;
            if (cnt > 0)
            {
                tmp.Idx += 1;
                if (tmp.Idx >= cnt) tmp.Idx = 0;

                tmp.Text = tmp.TxtList[tmp.Idx];
                tmp.SubSubject = tmp.SubjectList[tmp.Idx];

                string folderName = tmp.FileName.Substring(0, tmp.FileName.LastIndexOf(@"\")+1);
                string imgName = folderName + tmp.ImgList[tmp.Idx];

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imgName);
                bitmap.EndInit();
                image.Source = bitmap;

                string mediaName = folderName + tmp.MediaList[tmp.Idx];
                if (mediaName.Contains(".mp4"))
                {
                    VideoLoaded = true;
                    VideoNotLoaded = false;
                }
                else
                {
                    VideoLoaded = false;
                    VideoNotLoaded = true;
                }
                MediaLoaded = true;
                media.Source = new Uri(mediaName);
                media.LoadedBehavior = MediaState.Manual;
                media.UnloadedBehavior = MediaState.Manual;
                media.Volume = mediaVolume;
                this.TextDocument = tmp;
                
                media.Play();
  
            }
            
        }

        string PlainText()
        {
            return fsRichTextBox.GetPlainText();
        }

        private Guid? GetRequestedTextId(NavigationContext navigationContext)
        {
            var text = navigationContext.Parameters[TextIdKey];
            Guid textId;
            if (text != null)
            {
                if (text is Guid)
                {
                    textId = (Guid)text;
                }
                else
                {
                    textId = Guid.Parse(text.ToString());
                }

                return textId;
            }

            return null;
        }

        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            if (this.textDocument == null)
            {
                return true;
            }

            var requestedTextId = GetRequestedTextId(navigationContext);

            return requestedTextId.HasValue && requestedTextId.Value == this.textDocument.Id;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Intentionally not implemented.
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            var textId = GetRequestedTextId(navigationContext);
            if (textId.HasValue)
            {
                TextDocument temp = this.ttsService.GetSoundDocument(textId.Value);
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("Must set CustomPrincipal object on startup.");
                //Authenticate the user
                CustomIdentity identity = customPrincipal.Identity;
                temp.StudentId = identity.Name;
                temp.From = identity.Fullname;

                string folderName = temp.FileName.Substring(0, temp.FileName.LastIndexOf(@"\") + 1);
                string imgName = folderName + temp.ImgList[0];
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imgName);
                bitmap.EndInit();
                image.Source = bitmap;

                string mediaName = folderName + temp.MediaList[0];
                media.Source = new Uri(mediaName);
                media.LoadedBehavior = MediaState.Manual;
                media.UnloadedBehavior = MediaState.Manual;
                media.Volume = mediaVolume;
                this.TextDocument = temp;
                if (mediaName.Contains(".mp4"))
                {
                    VideoLoaded = true;
                    videoNotLoaded = false;
                } else
                {
                    VideoLoaded = false;
                    VideoNotLoaded = true;
                }

                MediaLoaded = true;

                media.Play();

                this.TextDocument = temp;
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        #endregion
    }
}
