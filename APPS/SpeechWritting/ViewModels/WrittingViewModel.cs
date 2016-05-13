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
using SpeechControls;
using SpeechInfrastructure;

namespace SpeechWritting.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WrittingViewModel : BindableBase, INavigationAware
    {
        private readonly DelegateCommand goBackCommand;
        private readonly DelegateCommand speakWordCommand;
        private readonly DelegateCommand<object> speakCommand;
        private readonly DelegateCommand pauseCommand;
        private readonly DelegateCommand resumeCommand;
        private readonly DelegateCommand stopCommand;
        private readonly DelegateCommand<object> updateCommand;
        private readonly DelegateCommand loadDocCommand;
        private readonly DelegateCommand<object> saveCommand;
        private readonly DelegateCommand<object> saveAsCommand;

        private IRegionNavigationJournal navigationJournal;

        private string selectedText;

        private List<int> repeatOptions;
        private int repeat = 1;
        private int repeatCnt = 0;

        private const string TextIdKey = "TextId";
        private TextDocument textDocument;
 
        private List<string> voiceOptions;
        private string selectedVoice="Male";

        private int rate = 10;
        private int volume = 100;
        
        private bool saveClickable = false;
        private bool speakClickable = true;
        private bool stopClickable = false;
        private bool resumeClickable = false;

        SpeechSynthesizer voice;
        FsRichTextBox fsRichTextBox;

        private bool wordSpeak = false;

        [ImportingConstructor]
        public WrittingViewModel()
        {
            this.goBackCommand = new DelegateCommand(this.GoBack);
            this.speakWordCommand = new DelegateCommand(this.SpeakWord);
            this.speakCommand = new DelegateCommand<object>(this.Speak);
            this.pauseCommand = new DelegateCommand(this.Pause);
            this.resumeCommand = new DelegateCommand(this.Resume);
            this.stopCommand = new DelegateCommand(this.Stop);
            this.updateCommand = new DelegateCommand<object>(this.Update);
            this.saveCommand = new DelegateCommand<object>(this.Save);
            this.saveAsCommand = new DelegateCommand<object>(this.SaveAs);
            this.loadDocCommand = new DelegateCommand(this.LoadDocument);

            repeatOptions = new List<int>
            {
                0,
                1,
                5,
                10
            };

            voiceOptions = new List<string>
            {
                "Male",
                "Female"
            };

            voice = new SpeechSynthesizer();
            voice.SpeakCompleted += OnSpeakCompleted;
            voice.SpeakProgress += OnWord;

            selectedText = "";
            textDocument = new TextDocument();
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
            } else
            {
                this.RepeatCnt = 0;
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

        public ICommand SpeakWordCommand
        {
            get { return this.speakWordCommand; }
        }

        public ICommand SpeakCommand
        {
            get { return this.speakCommand; }
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

        public ICommand LoadDocCommand
        {
            get { return this.loadDocCommand; }
        }

        public ICommand SaveCommand
        {
            get { return this.saveCommand; }
        }

        public ICommand SaveAsCommand
        {
            get { return this.saveAsCommand; }
        }

        public List<int> RepeatOptions
        {
            get
            {
                return this.repeatOptions;
            }
        }

        public List<string> VoiceOptions
        {
            get
            {
                return this.voiceOptions;
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

        public bool SaveClickable
        {
            get
            {
                return this.saveClickable;
            }

            set
            {
                this.SetProperty(ref this.saveClickable, value);
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


        private void GoBack()
        {
            // todo: 15 - Using the journal to navigate back.
            //
            // This view model offers a GoBack command and uses
            // the journal captured in OnNavigatedTo to
            // navigate back to the prior view.
            if (this.navigationJournal != null)
            {
                this.navigationJournal.GoBack();
            }
        }

        private void SpeakWord()
        {
            if (selectedText.Length < 2 )
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

        private void Speak(object control)
        {
            this.Repeat = 0;

            this.StopClickable = true;
            this.ResumeClickable = false;
            this.SpeakClickable = false;

            wordSpeak = false;

            if (control != null) 
            {
                fsRichTextBox = (FsRichTextBox)control;
            }

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

            fsRichTextBox.ResetPointer();
            voice.SpeakAsync(PlainText());    
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
            Stop();
            /*
            if (control != null) {
                fsRichTextBox = (FsRichTextBox)control;
                string key = TextDocument.RTE_KEY + ":"
                           + this.TextDocument.From + ":"
                           + this.TextDocument.Description + ":"
                           + this.TextDocument.Subject + ":"
                           + this.TextDocument.Id.ToString() + ":"
                           + TextDocument.RTE_KEY;
                // Save the contents of the RichTextBox into the file.
                fsRichTextBox.SaveRTEText(this.TextDocument.FileName, key);
            }*/
        }

        private void SaveAs(object control)
        {
            Stop();
            if (control != null)
            {
                fsRichTextBox = (FsRichTextBox)control;
                SaveFileDialog saveFile1 = new SaveFileDialog();

                // Initialize the SaveFileDialog to specify the RTF extention for the file.
                saveFile1.DefaultExt = "*.rte";
                saveFile1.Filter = "RTE Files|*.rte";

                // Determine whether the user selected a file name from the saveFileDialog. 
                if (saveFile1.ShowDialog() == DialogResult.OK &&
                   saveFile1.FileName.Length > 0)
                {
                    /*
                    textDocument.Id  = Guid.NewGuid();
                    string key = TextDocument.RTE_KEY + ":"
                               + this.TextDocument.From + ":" 
                               + this.TextDocument.Description + ":" 
                               + this.TextDocument.Subject + ":"
                               + textDocument.Id.ToString() + ":"
                               + TextDocument.RTE_KEY;
                    // Save the contents of the RichTextBox into the file.
                    fsRichTextBox.SaveAsRTEText(saveFile1.FileName, key);*/

                    loadFile(saveFile1.FileName);
                    
                }
            }
        }

        string PlainText()
        {
            return fsRichTextBox.GetPlainText();
        }

        private void LoadDocument()
        {
            Stop();
            
            OpenFileDialog openFile1 = new OpenFileDialog();
            openFile1.DefaultExt = "*.*";
            //openFile1.Filter = "RTF Files|*.rtf";
            if (openFile1.ShowDialog() == DialogResult.OK &&
                   openFile1.FileName.Length > 0)
            {
                loadFile(openFile1.FileName);          
            }
        }

        private void loadFile(string fileName)
        {
            TextDocument temp = new TextDocument(false);
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string txt = sr.ReadToEnd();
                    /*
                    if (txt.Contains(TextDocument.RTE_KEY) && txt.Contains("<FlowDocument"))
                    {
                        temp.Type = TextDocument.RTE;
                        int first = txt.IndexOf(TextDocument.RTE_KEY);
                        int last = txt.LastIndexOf(TextDocument.RTE_KEY);
                        string afs = txt.Substring(first, last - first);
                        string[] strs = afs.Split(':');
                        temp.FileName = fileName;
                        temp.From = strs[1];
                        temp.Description = strs[2];
                        temp.Subject = strs[3];
                        Guid guid;
                        bool isValid = Guid.TryParse(strs[4], out guid);
                        if (isValid)
                        {
                            temp.Id = guid;
                            this.SaveClickable = true;
                        }
                        else
                        {
                            this.SaveClickable = false;
                        }*/

                    }
                    else if (txt.Contains("FlowDocument"))
                    {
                        temp.Type = TextDocument.RTF;
                    }
                    else
                    {
                        temp.Type = TextDocument.TXT;
                    }

                    temp.Text = txt;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            this.TextDocument = temp;
            this.SaveClickable = true;
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

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            // todo: 15 - Orient to the right context
            //
            // When this view model is navigated to, it gathers the
            // requested EmailId from the navigation context's parameters.
            //
            // It also captures the navigation Journal so it
            // can offer a 'go back' command.
            var textId = GetRequestedTextId(navigationContext);
            if (textId.HasValue)
            {
                //this.TextDocument = this.ttsService.GetTextDocument(textId.Value);
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Intentionally not implemented.
        }

        #endregion
    }
}
