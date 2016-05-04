// Copyright (c) Microsoft Corporation. All rights reserved. 
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using System.IO;
using System.Windows.Forms;
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

namespace SpeechTTS.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TTSViewModel : BindableBase, INavigationAware
    {
        private readonly DelegateCommand goBackCommand;
        private readonly DelegateCommand resetCommand;
        private readonly DelegateCommand<object> mp3Command;
        private readonly DelegateCommand<object> speakCommand;
        private readonly DelegateCommand pauseCommand;
        private readonly DelegateCommand resumeCommand;
        private readonly DelegateCommand stopCommand;
        private readonly DelegateCommand<object> updateCommand;
        //private readonly DelegateCommand loadDocCommand;
        private readonly DelegateCommand<object> saveCommand;
        private readonly DelegateCommand<object> saveAsCommand;
        private readonly DelegateCommand speakWordCommand;

        private readonly ITTService ttsService;
        private TextDocument textDocument;
        private IRegionNavigationJournal navigationJournal;
        private const string TextIdKey = "TextId";
        private List<string> voiceOptions;
        private string selectedVoice="Male";
        private int volume = 100;
        private int rate = 10;

        private bool speakClickable = true;
        private bool stopClickable = false;
        private bool resumeClickable = false;

        private string selectedText;
        private List<int> repeatOptions;
        private int repeat = 5;
        private int repeatCnt = 0;

        SpeechSynthesizer voice;
        FsRichTextBox fsRichTextBox;
        private bool wordSpeak = false;
        private string message = "";

        [ImportingConstructor]
        public TTSViewModel(ITTService ttsService)
        {
            this.goBackCommand = new DelegateCommand(this.GoBack);
            this.resetCommand = new DelegateCommand(this.Reset);
            this.mp3Command = new DelegateCommand<object>(this.Mp3);
            this.speakCommand = new DelegateCommand<object>(this.Speak);
            this.pauseCommand = new DelegateCommand(this.Pause);
            this.resumeCommand = new DelegateCommand(this.Resume);
            this.stopCommand = new DelegateCommand(this.Stop);
            this.updateCommand = new DelegateCommand<object>(this.Update);
            this.saveCommand = new DelegateCommand<object>(this.Save);
            this.saveAsCommand = new DelegateCommand<object>(this.SaveAs);
            //this.loadDocCommand = new DelegateCommand(this.LoadDocument);

            this.speakWordCommand = new DelegateCommand(this.SpeakWord);

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

            voice = new SpeechSynthesizer();
            voice.SpeakCompleted += OnSpeakCompleted;
            voice.SpeakProgress += OnWord;

            selectedText = "";
        }

        void OnSpeakCompleted(object sender, EventArgs e)
        {
            this.Stop();
            this.SpeakClickable = true;
            this.StopClickable = false;
            this.ResumeClickable = false;

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

        public ICommand UpdateCommand
        {
            get { return this.updateCommand; }
        }

        public ICommand SaveCommand
        {
            get { return this.saveCommand; }
        }

        public ICommand SaveAsCommand
        {
            get { return this.saveAsCommand; }
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

        private void Mp3(object control)
        {
            wordSpeak = false;
            this.StopClickable = true;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            if (control != null)
            {
                fsRichTextBox = (FsRichTextBox)control;
                //fsRichTextBox.FontSize = 16;
            }

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

        private void Speak(object control)
        {
            wordSpeak = false;
            this.StopClickable = true;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            if (control != null) 
            {
                fsRichTextBox = (FsRichTextBox)control;
                fsRichTextBox.FontSize = 16;
                //this.richTextBox1.rosoftSelectionFont = newFont; 20
            }

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

            voice.Volume = volume;
            voice.Rate = rate - 10;

            voice.SetOutputToDefaultAudioDevice();  
            fsRichTextBox.ResetPointer();
            voice.SpeakAsync(PlainText());
        }

        private void SpeakWord()
        {
            if (selectedText.Length < 2)
                return;

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
            this.StopClickable = false;
            this.SpeakClickable = true;
            this.ResumeClickable = false;
        }

        private void Update(object control)
        {
            Stop();
            if (control != null)
            {
                fsRichTextBox = (FsRichTextBox)control;
                fsRichTextBox.UpdateDocumentBindings();
            }
        }

        private void Save(object control)
        {
            FsRichTextBox textBox = (FsRichTextBox)control;
            File.WriteAllText(textDocument.FileName, textBox.GetPlainText());
        }

        private void SaveAs(object control)
        {
            Stop();
            if (control != null)
            {
                fsRichTextBox = (FsRichTextBox)control;
                SaveFileDialog saveFile1 = new SaveFileDialog();

                // Initialize the SaveFileDialog to specify the RTF extention for the file.
                saveFile1.DefaultExt = "*.txt";
                saveFile1.Filter = "TXT Files|*.txt";

                // Determine whether the user selected a file name from the saveFileDialog. 
                if (saveFile1.ShowDialog() == DialogResult.OK &&
                   saveFile1.FileName.Length > 0)
                {
                    File.WriteAllText(saveFile1.FileName, PlainText());
                    // Save the contents of the RichTextBox into the file.
                    //fsRichTextBox.SaveAsRTEText(saveFile1.FileName, "");
                    //fsRichTextBox.
                }
            }
        }

        string PlainText()
        {
            return fsRichTextBox.GetPlainText();
        }

        /*
        private void LoadDocument()
        {
            Stop();
            TextDocument temp = (TextDocument)textDocument.Clone();
            temp.Type = "RTF";
            temp.Text = "<FlowDocument PagePadding=\"5,0,5,0\" AllowDrop=\"True\" "
                + "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">"
                + "<Paragraph>Text generated by app back-end</Paragraph></FlowDocument>";

            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal == null)
                throw new ArgumentException("Must set CustomPrincipal object on startup.");

            //Authenticate the user
            CustomIdentity identity = customPrincipal.Identity;
            temp.StudentId = identity.Name;
            temp.From = identity.Fullname;
            this.TextDocument = temp;
        }*/

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
            // todo: 13 - Determining if a view or view model handles the navigation request
            //
            // By implementing IsNavigationTarget, this view model can let the region
            // navigation service know that it is the item sought for navigation. 
            // 
            // If this view model is the one that was built to display the requested
            // EmailId (as a result of a prior navigation request), then this
            // method will return true.  
            //
            // Otherwise, it will return false and if no other EmailViewModel type returns true 
            // then the navigation service knows that no EmailView is already available that 
            // shows that email.  In this case, the navigation service will request a new one 
            // be constructed and added to the region.
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
                TextDocument temp = this.ttsService.GetFunDocument(textId.Value);
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("Must set CustomPrincipal object on startup.");
                //Authenticate the user
                CustomIdentity identity = customPrincipal.Identity;
                temp.StudentId = identity.Name;
                temp.From = identity.Fullname;
                this.TextDocument = temp;
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        #endregion
    }
}
