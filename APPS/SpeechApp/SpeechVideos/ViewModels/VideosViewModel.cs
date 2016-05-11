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
using System.Windows.Controls;


namespace SpeechVideos.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class VideosViewModel : BindableBase, INavigationAware
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

        private readonly DelegateCommand playVideoCommand;
        private readonly DelegateCommand pauseVideoCommand;
        private readonly DelegateCommand resumeVideoCommand;
        private readonly DelegateCommand stopVideoCommand;

        private readonly ITTService ttsService;
        private TextDocument textDocument;
        private IRegionNavigationJournal navigationJournal;
        private const string TextIdKey = "TextId";
        private List<string> voiceOptions;
        private string selectedVoice="Male";
        private int volume = 100;
        private int rate = 10;
        private double videoVolume = 1;
        private double maxTime=1;
        private double videoProgress = 0;

        private bool preClickable = false;
        private bool nextClickable = false;

        private bool speakClickable = true;
        private bool stopClickable = false;
        private bool resumeClickable = false;

        private bool videoPlayClickable = true;
        private bool videoStopClickable = false;
        private bool videoResumeClickable = false;

        private bool transcriptIsChecked = true;
        private bool dialogIsChecked = false;
        private bool vocabIsChecked = false;
        private bool allIsChecked = false;
        private bool notTranscript = false;

        private bool repeatSelected = false;
        private bool dialogSelected = false;

        private string selectedText;
        private string selectedText2;
        private List<int> repeatOptions;
        private List<int> fontSizeOptions;
        private int repeat = 5;
        private int fontSize = 16;
        private int repeatCnt = 0;

        private int dialogIdx = 0;
        private int dialogCnt = 0;
        private string gender = "";
        private string sentence = "";

        SpeechSynthesizer voice; 
        private bool wordSpeak = false;
        private string message = "";
        
        FsRichTextBox fsRichTextBox;
        FsRichTextBox fsRichTextBox2;
        MediaElement viewer;

        

        [ImportingConstructor]
        public VideosViewModel(ITTService ttsService)
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

            this.playVideoCommand  = new DelegateCommand(this.PlayVideo);
            this.pauseVideoCommand = new DelegateCommand(this.PauseVideo);
            this.resumeVideoCommand = new DelegateCommand(this.ResumeVideo);
            this.stopVideoCommand  = new DelegateCommand(this.StopVideo); 

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

            //viewer.MediaEnded += OnVideoEnded;

            selectedText = "";
        }

        public void MediaEnded()
        {
            viewer.Position = TimeSpan.FromSeconds(0);
            wordSpeak = false;
            this.PreClickable = true;
            this.NextClickable = true;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = true;

            this.VideoPlayClickable = true;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = false;

            viewer.Stop();

        }

        public void setEditbox(FsRichTextBox txtBox, FsRichTextBox txtBox2)
        {
            fsRichTextBox = txtBox;
            fsRichTextBox.FontSize = fontSize;

            fsRichTextBox2 = txtBox2;
            fsRichTextBox2.FontSize = fontSize;
        }

        public void setVideoPlayer(System.Windows.Controls.MediaElement view)
        {
            viewer = view;
        }

        void OnSpeakCompleted(object sender, EventArgs e)
        {
            this.Stop();

            if (repeatSelected)
            {
                if (repeatCnt < repeat)
                {
                    this.SpeakWord();
                }
                else
                {
                    this.RepeatCnt = 0;
                    RepeatSelected = false;
                }
            }

            if (dialogSelected)
            {
                if (dialogIdx < dialogCnt )
                {
                    this.SpeakDialog();
                }
                else
                {
                    this.DialogIdx = 0;
                    DialogSelected = false;
                    this.PreClickable = true;
                    this.NextClickable = true;
                    this.StopClickable = false;
                    this.ResumeClickable = false;
                    this.SpeakClickable = true;

                    this.VideoPlayClickable = true;
                    this.VideoStopClickable = false;
                    this.VideoResumeClickable = false;
                }
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

        public ICommand PlayVideoCommand
        {
            get { return this.playVideoCommand; }
        }

        public ICommand PauseVideoCommand
        {
            get { return this.pauseVideoCommand; }
        }

        public ICommand ResumeVideoCommand
        {
            get { return this.resumeVideoCommand; }
        }

        public ICommand StopVideoCommand
        {
            get { return this.stopVideoCommand; }
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
                string txt = value;
                if (txt.Length > 50)
                {
                    txt = txt.Substring(50);  
                }

                this.SetProperty(ref this.selectedText, txt);

            }
        }

        public string SelectedText2
        {
            get
            {
                return this.selectedText2;
            }

            set
            {
                string txt = value;
                if (txt.Length > 50)
                {
                    txt = txt.Substring(50);
                }

                this.SetProperty(ref this.selectedText2, txt);
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

        public double VideoVolume
        {
            get
            {
                return this.videoVolume;
            }

            set
            {
                this.SetProperty(ref this.videoVolume, value);
                viewer.Volume = videoVolume;
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

        public double VideoProgress
        {
            get
            {
                return this.videoProgress;
            }

            set
            {
                this.SetProperty(ref this.videoProgress, value);
                viewer.Position = TimeSpan.FromSeconds(videoProgress);
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

        public bool VideoPlayClickable
        {
            get
            {
                return this.videoPlayClickable;
            }

            set
            {
                this.SetProperty(ref this.videoPlayClickable, value);
            }
        }

        public bool VideoStopClickable
        {
            get
            {
                return this.videoStopClickable;
            }

            set
            {
                this.SetProperty(ref this.videoStopClickable, value);
            }
        }

        public bool VideoResumeClickable
        {
            get
            {
                return this.videoResumeClickable;
            }

            set
            {
                this.SetProperty(ref this.videoResumeClickable, value);
            }
        }

        

        public bool TranscriptIsChecked
        {
            get
            {
                return this.transcriptIsChecked;
            }

            set
            {
                this.SetProperty(ref this.transcriptIsChecked, value);
                NotTranscript = !transcriptIsChecked;
            }
        }

        public bool NotTranscript
        {
            get
            {
                return notTranscript;
            }

            set
            {
                this.SetProperty(ref this.notTranscript, value);
            }
        }

        public bool DialogIsChecked
        {
            get
            {
                return this.dialogIsChecked;
            }

            set
            {
                this.SetProperty(ref this.dialogIsChecked, value);
            }
        }

        public bool VocabIsChecked
        {
            get
            {
                return this.vocabIsChecked;
            }

            set
            {
                this.SetProperty(ref this.vocabIsChecked, value);
            }
        }

        public bool AllIsChecked
        {
            get
            {
                return this.allIsChecked;
            }

            set
            {
                this.SetProperty(ref this.allIsChecked, value);
            }
        }

        public bool RepeatSelected
        {
            get
            {
                return this.repeatSelected;
            }

            set
            {
                this.SetProperty(ref this.repeatSelected, value);
            }
        }

        public bool DialogSelected
        {
            get
            {
                return this.dialogSelected;
            }

            set
            {
                this.SetProperty(ref this.dialogSelected, value);
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

        public string Gender
        {
            get
            {
                return this.gender;
            }

            set
            {
                this.SetProperty(ref this.gender, value);
            }
        }

        public string Sentence
        {
            get
            {
                return this.sentence;
            }

            set
            {
                this.SetProperty(ref this.sentence, value);
            }
        }

        public int DialogIdx
        {
            get
            {
                return this.dialogIdx;
            }

            set
            {
                this.SetProperty(ref this.dialogIdx, value);
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

            this.VideoPlayClickable = false;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = false;

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

            this.VideoPlayClickable = false;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = false;

            this.Message = "Reading Text...";
            voice.Volume = volume;
            voice.Rate = rate - 10;
            voice.SetOutputToDefaultAudioDevice();

            if (transcriptIsChecked)
            {
                switch (selectedVoice)
                {
                    case "Male":
                        voice.SelectVoiceByHints(VoiceGender.Male);
                        break;
                    case "Female":
                        voice.SelectVoiceByHints(VoiceGender.Female);
                        break;
                }

                fsRichTextBox.ResetPointer();
                voice.SpeakAsync(PlainText());

            } else if (vocabIsChecked) {

                switch (selectedVoice)
                {
                    case "Male":
                        voice.SelectVoiceByHints(VoiceGender.Male);
                        break;
                    case "Female":
                        voice.SelectVoiceByHints(VoiceGender.Female);
                        break;
                }
                voice.SpeakAsync(textDocument.Vocalburay);

            } else if (dialogIsChecked) {

                DialogSelected = true;
                dialogCnt = textDocument.SentenceList.Count;
                if (dialogCnt > 0)
                {
                    SpeakDialog();
                }   
            }
        }

        private void SpeakDialog()
        {
            Gender = textDocument.GenderList[dialogIdx];
            Sentence = textDocument.SentenceList[dialogIdx];
            if (gender.Equals("M"))
                 voice.SelectVoiceByHints(VoiceGender.Male);
            else
                 voice.SelectVoiceByHints(VoiceGender.Female);

            voice.SpeakAsync(sentence);
            DialogIdx += 1;
        }

        private void SpeakWord()
        {
            RepeatSelected = true;
            string tmp = selectedText;
            if (notTranscript) tmp = selectedText2;
            if (tmp.Length < 2) return;

            if (repeat == 0) Repeat = 1;

            wordSpeak = true;
            this.PreClickable = false;
            this.NextClickable = false;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            this.VideoPlayClickable = false;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = false;

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
            voice.SpeakAsync(tmp);
        }

        private void Pause()
        {
            voice.Pause();
            this.StopClickable = false;
            this.SpeakClickable = false;
            this.ResumeClickable = true;

            this.VideoPlayClickable = false;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = false;
        }

        private void Resume()
        {
            voice.Resume();
            this.StopClickable = true;
            this.SpeakClickable = false;
            this.ResumeClickable = false;

            this.VideoPlayClickable = false;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = false;
        }

        private void Stop()
        {
            voice.SpeakAsyncCancelAll();

            this.PreClickable = true;
            this.NextClickable = true;
            this.StopClickable = false;
            this.SpeakClickable = true;
            this.ResumeClickable = false;

            this.VideoPlayClickable = true;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = false;
        }

        private void Pre()
        {
            TextDocument tmp = (TextDocument)this.TextDocument.Clone();
            if (tmp.Idx > 0 )
            {
                tmp.Idx -= 1;
                tmp.Text = tmp.TxtList[tmp.Idx];

                string folderName = tmp.FileName.Substring(0, tmp.FileName.LastIndexOf(@"\") + 1);
                string imgName = folderName + tmp.ImgList[tmp.Idx];

                viewer.Source = new Uri(imgName);
                viewer.LoadedBehavior = MediaState.Manual;
                viewer.UnloadedBehavior = MediaState.Manual;
                viewer.Volume = videoVolume;
                this.TextDocument = tmp;
                this.PreClickable = false;
                this.NextClickable = false;
                this.StopClickable = false;
                this.ResumeClickable = false;
                this.SpeakClickable = false;

                this.VideoPlayClickable = false;
                this.VideoStopClickable = true;
                this.VideoResumeClickable = false;
                viewer.Play();
            }
            
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
                string folderName = tmp.FileName.Substring(0, tmp.FileName.LastIndexOf(@"\")+1);
                string imgName = folderName + tmp.ImgList[tmp.Idx];

                viewer.Source = new Uri(imgName);
                viewer.LoadedBehavior = MediaState.Manual;
                viewer.UnloadedBehavior = MediaState.Manual;
                viewer.Volume = videoVolume;
                this.TextDocument = tmp;
                this.PreClickable = false;
                this.NextClickable = false;
                this.StopClickable = false;
                this.ResumeClickable = false;
                this.SpeakClickable = false;

                this.VideoPlayClickable = false;
                this.VideoStopClickable = true;
                this.VideoResumeClickable = false;
                viewer.Play();

            }
            
        }

        private void PlayVideo()
        {
            viewer.LoadedBehavior = MediaState.Manual;
            wordSpeak = false;
            this.PreClickable = false;
            this.NextClickable = false;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            this.VideoPlayClickable = false;
            this.VideoStopClickable = true;
            this.VideoResumeClickable = false;
            viewer.Play();
        }

        private void PauseVideo()
        {
            viewer.Pause();
            this.VideoPlayClickable = false;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = true;
        }

        private void ResumeVideo()
        {
            viewer.Play();
            this.VideoPlayClickable = false;
            this.VideoStopClickable = true;
            this.VideoResumeClickable = false;
        }

        private void StopVideo()
        {
            viewer.Stop();
            
            this.PreClickable = true;
            this.NextClickable = true;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = true;

            this.VideoPlayClickable = true;
            this.VideoStopClickable = false;
            this.VideoResumeClickable = false;
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
                TextDocument temp = this.ttsService.GetActivitiesDocument(textId.Value);
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("Must set CustomPrincipal object on startup.");
                //Authenticate the user
                CustomIdentity identity = customPrincipal.Identity;
                temp.StudentId = identity.Name;
                temp.From = identity.Fullname;

                string folderName = temp.FileName.Substring(0, temp.FileName.LastIndexOf(@"\") + 1);
                string imgName = folderName + temp.ImgList[0];
                viewer.Source = new Uri(imgName);
                viewer.LoadedBehavior   = MediaState.Stop;
                viewer.UnloadedBehavior = MediaState.Manual;
                viewer.Volume = videoVolume;
                this.TextDocument = temp;
                //viewer.Play();
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        #endregion
    }
}
