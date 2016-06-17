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

namespace SpeechWords.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WordsViewModel : BindableBase, INavigationAware
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

        private readonly ITTService ttsService;
        private TextDocument textDocument;
        private IRegionNavigationJournal navigationJournal;
        private const string TextIdKey = "TextId";
        private List<string> voiceOptions;
        private string selectedVoice="Male";
        private int volume = 100;
        private int rate = 10;

        private bool preClickable = true;
        private bool nextClickable = true;

        private bool speakClickable = true;
        private bool stopClickable = false;
        private bool resumeClickable = false;

        private bool transcriptIsChecked = true;
        private bool dialogIsChecked = false;
        private bool vocabIsChecked = false;
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
        private bool audioReady;
        MediaElement audio;
        Image image;


        [ImportingConstructor]
        public WordsViewModel(ITTService ttsService)
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

            this.ttsService = ttsService;

            voiceOptions = new List<string>
            {
                "Male",
                "Female"
            };

            repeatOptions = new List<int>
            {
                3,
                5,
                10,
                15,
                20,
                50
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

            audioReady = false;
            selectedText = "";
            selectedText2 = "";
        }

        public void MediaEnded()
        {
            audio.Position = TimeSpan.FromSeconds(0);
            wordSpeak = false;
            this.PreClickable = true;
            this.NextClickable = true;
            this.StopClickable = false;
            this.ResumeClickable = false;
            this.SpeakClickable = true;
            audio.Stop();
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
            audio = maudio;
        }

        void OnSpeakCompleted(object sender, EventArgs e)
        {
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
                    this.SpeakClickable = true;
                }
            }

            if (dialogIsChecked)
            {
                if (dialogIdx < dialogCnt)
                {
                    this.SpeakDialog();
                }
                else
                {
                    this.DialogIdx = 0;
                    this.PreClickable = true;
                    this.NextClickable = true;
                    this.StopClickable = false;
                    this.ResumeClickable = false;
                    this.SpeakClickable = true;
                }
            }

            if (transcriptIsChecked || vocabIsChecked)
            {
                this.PreClickable = true;
                this.NextClickable = true;
                this.StopClickable = false;
                this.ResumeClickable = false;
                this.SpeakClickable = true;
            }

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

        public string SelectedText2
        {
            get
            {
                return this.selectedText2;
            }

            set
            {
                this.SetProperty(ref this.selectedText2, value);
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
                NotTranscript = dialogIsChecked || vocabIsChecked;
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
                NotTranscript = dialogIsChecked || vocabIsChecked;
            }
        }

        public bool NotTranscript
        {
            get
            {
                return this.notTranscript;
            }

            set
            {
                this.SetProperty(ref this.notTranscript, value);
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
            Stop();
            SelectedText = "";
            SelectedText2 = "";
            wordSpeak = false;
            this.StopClickable = true;
            this.ResumeClickable = false;
            this.SpeakClickable = false;
            this.Message = "Reading Text...";

            if (selectedVoice.Equals("MyVoice"))
            {
                audio.Play();
                return;
            }

            voice.Volume = volume;
            voice.Rate = rate - 10;
            voice.SetOutputToDefaultAudioDevice();

            switch (selectedVoice)
            {
                case "Male":
                    voice.SelectVoiceByHints(VoiceGender.Male);
                    break;
                case "Female":
                    voice.SelectVoiceByHints(VoiceGender.Female);
                    break;
            }


            if (transcriptIsChecked)
            {
                fsRichTextBox.ResetPointer();
                voice.SpeakAsync(PlainText());
            }
            else if (vocabIsChecked)
            {
                voice.SpeakAsync(textDocument.Vocalburay);
            }
            else if (dialogIsChecked)
            {

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
            if (gender.Contains("A"))
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
            voice.SpeakAsync(tmp);
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

            if (dialogIsChecked)
                DialogIdx = 1000;

            this.PreClickable = true;
            this.NextClickable = true;
            this.StopClickable = false;
            this.SpeakClickable = true;
            this.ResumeClickable = false;
        }

        public void changeVisible()
        {
            repeatCnt = 1000;
            Stop();
            if (audioReady ) audio.Stop();
        }

        private void Pre()
        {
            Stop();
            TranscriptIsChecked = true;
            TextDocument tmp = (TextDocument)this.TextDocument.Clone();
            if (tmp.Idx > 0 )
            {
                tmp.Idx -= 1;
                tmp.Text = tmp.TxtList[tmp.Idx];
                tmp.SubSubject = tmp.SubjectList[tmp.Idx];
                string folderName = tmp.FileName.Substring(0, tmp.FileName.LastIndexOf(@"\") + 1);
                string imgName = tmp.ImgList[tmp.Idx];
                if ( !imgName.Contains("\\"))
                    imgName = folderName + tmp.ImgList[tmp.Idx];
                if (!File.Exists(imgName))
                {
                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    imgName = appPath + TTService.MISSED_IMG;
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imgName);
                bitmap.EndInit();
                image.Source = bitmap;
                this.TextDocument = tmp;

                audioReady = false;
                if (tmp.MediaList.Count > tmp.Idx)
                {
                    audioReady = true;
                    string audioName = tmp.MediaList[tmp.Idx];
                    if ( !audioName.Contains("\\"))
                        audioName = folderName + tmp.MediaList[tmp.Idx];
                    if (!File.Exists(audioName))
                    {
                        string appPath = AppDomain.CurrentDomain.BaseDirectory;
                        audioName = appPath + TTService.MISSED_MP3;
                    }
                    audio.Source = new Uri(audioName);
                    audio.LoadedBehavior = MediaState.Manual;
                    audio.UnloadedBehavior = MediaState.Manual;
                } 
                
                Speak();
            }
            
        }

        private void Next()
        {
            Stop();
            TranscriptIsChecked = true;
            TextDocument tmp = (TextDocument)this.TextDocument.Clone();
            int cnt = tmp.TxtList.Count;
            if (cnt > 0)
            {
                tmp.Idx += 1;
                if (tmp.Idx >= cnt) tmp.Idx = 0;

                tmp.Text = tmp.TxtList[tmp.Idx];
                tmp.SubSubject = tmp.SubjectList[tmp.Idx];
                string folderName = tmp.FileName.Substring(0, tmp.FileName.LastIndexOf(@"\")+1);

                string imgName = tmp.ImgList[tmp.Idx];
                if ( !imgName.Contains("\\"))
                    imgName = folderName + tmp.ImgList[tmp.Idx];
                if (!File.Exists(imgName))
                {
                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    imgName = appPath + TTService.MISSED_IMG;
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imgName);
                bitmap.EndInit();
                image.Source = bitmap;

                this.TextDocument = tmp;

                audioReady = false;
                if (tmp.MediaList.Count > tmp.Idx)
                {
                    audioReady = true;
                    string audioName = tmp.MediaList[tmp.Idx];
                    if (!audioName.Contains("\\"))
                        audioName = folderName + tmp.MediaList[tmp.Idx];
                    if (!File.Exists(audioName))
                    {
                        string appPath = AppDomain.CurrentDomain.BaseDirectory;
                        audioName = appPath + TTService.MISSED_MP3;
                    }
                    audio.Source = new Uri(audioName);
                    audio.LoadedBehavior = MediaState.Manual;
                    audio.UnloadedBehavior = MediaState.Manual;
                }

                Speak();
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
                TextDocument temp = this.ttsService.GetCardsDocument(textId.Value);
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("Must set CustomPrincipal object on startup.");
                //Authenticate the user
                CustomIdentity identity = customPrincipal.Identity;
                temp.StudentId = identity.Name;
                temp.From = identity.Fullname;

                string folderName = temp.FileName.Substring(0, temp.FileName.LastIndexOf(@"\") + 1);
                string imgName = temp.ImgList[0];
                if (!imgName.Contains("\\"))
                    imgName = folderName + temp.ImgList[0];
                if (!File.Exists(imgName))
                {
                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    imgName = appPath + TTService.MISSED_IMG;
                }
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imgName);
                bitmap.EndInit();
                image.Source = bitmap;

                if (temp.MediaList.Count > 0)
                {
                    audioReady = true;
                    string audioName = temp.MediaList[0];
                    if ( !audioName.Contains("\\")) 
                         audioName = folderName + temp.MediaList[0];
                    if (!File.Exists(audioName))
                    {
                        string appPath = AppDomain.CurrentDomain.BaseDirectory;
                        audioName = appPath + TTService.MISSED_MP3;
                    }
                    audio.Source = new Uri(audioName);
                    audio.LoadedBehavior = MediaState.Manual;
                    audio.UnloadedBehavior = MediaState.Manual;
                    if (voiceOptions.Count < 3)
                    {
                        VoiceOptions.Add("MyVoice");
                    } 

                } else {
                    if (voiceOptions.Count > 2)
                        VoiceOptions.Remove("MyVoice");
                }
                
                this.TextDocument = temp;
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        #endregion
    }
}
