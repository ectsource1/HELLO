// Copyright (c) Microsoft Corporation. All rights reserved. 
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using System.IO;
using System.Windows.Markup;
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
        private readonly DelegateCommand mp3Command;
        private readonly DelegateCommand speakCommand;
        private readonly DelegateCommand pauseCommand;
        private readonly DelegateCommand resumeCommand;
        private readonly DelegateCommand stopCommand;
        private readonly DelegateCommand updateCommand;
        private readonly DelegateCommand saveCommand;
        private readonly DelegateCommand saveAsCommand;
        private readonly DelegateCommand speakWordCommand;

        private readonly DelegateCommand aCommand;
        private readonly DelegateCommand bCommand;
        private readonly DelegateCommand cCommand;
        private readonly DelegateCommand dCommand;
        private readonly DelegateCommand eCommand;
        private readonly DelegateCommand fCommand;
        private readonly DelegateCommand gCommand;
        private readonly DelegateCommand hCommand;
        
        private readonly DelegateCommand iCommand;
        private readonly DelegateCommand jCommand;
        private readonly DelegateCommand kCommand;
        private readonly DelegateCommand lCommand;
        private readonly DelegateCommand mCommand;
        private readonly DelegateCommand nCommand;
        private readonly DelegateCommand oCommand;
        private readonly DelegateCommand pCommand;

        private readonly DelegateCommand qCommand;
        private readonly DelegateCommand rCommand;
        private readonly DelegateCommand sCommand;
        private readonly DelegateCommand tCommand;
        private readonly DelegateCommand uCommand;
        private readonly DelegateCommand vCommand;
        private readonly DelegateCommand wCommand;
        private readonly DelegateCommand xCommand;
        private readonly DelegateCommand yCommand;
        private readonly DelegateCommand zCommand;

        private readonly ITTService ttsService;
        private TextDocument textDocument;
        private IRegionNavigationJournal navigationJournal;
        private const string TextIdKey = "TextId";
        private List<string> voiceOptions;
        private List<int> fontSizeOptions;
        private string selectedVoice="Male";
        private int volume = 100;
        private int rate = 10;

        private bool speakClickable = true;
        private bool stopClickable = false;
        private bool resumeClickable = false;

        private string selectedText;
        private string selectedText2;
        private List<int> repeatOptions;
        private int repeat = 5;
        private int repeatCnt = 0;
        private int fontSize = 14;

        private Boolean aVisible = false;
        private Boolean bVisible = false;
        private Boolean cVisible = false;
        private Boolean dVisible = false;
        private Boolean eVisible = false;
        private Boolean fVisible = false;
        private Boolean gVisible = false;
        private Boolean hVisible = false;
        private Boolean iVisible = false;
        private Boolean jVisible = false;
        private Boolean kVisible = false;
        private Boolean lVisible = false;

        private Boolean mVisible = false;
        private Boolean nVisible = false;
        private Boolean oVisible = false;
        private Boolean pVisible = false;
        private Boolean qVisible = false;
        private Boolean rVisible = false;
        private Boolean sVisible = false;
        private Boolean tVisible = false;
        private Boolean uVisible = false;
        private Boolean vVisible = false;
        private Boolean wVisible = false;
        private Boolean xVisible = false;
        private Boolean yVisible = false;
        private Boolean zVisible = false;

        private string aLable = "";
        private string bLable = "";
        private string cLable = "";
        private string dLable = "";
        private string eLable = "";
        private string fLable = "";

        private string gLable = "";
        private string hLable = "";
        private string iLable = "";
        private string jLable = "";
        private string kLable = "";
        private string lLable = "";

        private string mLable = "";
        private string nLable = "";
        private string oLable = "";
        private string pLable = "";
        private string qLable = "";
        private string rLable = "";

        private string sLable = "";
        private string tLable = "";
        private string uLable = "";
        private string vLable = "";
        private string wLable = "";
        private string xLable = "";
        private string yLable = "";
        private string zLable = "";

        private int pageIdx = 0;
        SpeechSynthesizer voice;
        private bool wordSpeak = false;
        private string message = "";
        FsRichTextBox editBox;


        [ImportingConstructor]
        public TTSViewModel(ITTService ttsService)
        {
            #region commands
            this.goBackCommand = new DelegateCommand(this.GoBack);
            this.resetCommand = new DelegateCommand(this.Reset);
            this.mp3Command = new DelegateCommand(this.Mp3);
            this.speakCommand = new DelegateCommand(this.Speak);
            this.pauseCommand = new DelegateCommand(this.Pause);
            this.resumeCommand = new DelegateCommand(this.Resume);
            this.stopCommand = new DelegateCommand(this.Stop);
            this.updateCommand = new DelegateCommand(this.Update);
            this.saveCommand = new DelegateCommand(this.Save);
            this.saveAsCommand = new DelegateCommand(this.SaveAs);

            this.aCommand = new DelegateCommand(this.goPageA);
            this.bCommand = new DelegateCommand(this.goPageB);
            this.cCommand = new DelegateCommand(this.goPageC);
            this.dCommand = new DelegateCommand(this.goPageD);
            this.eCommand = new DelegateCommand(this.goPageE);
            this.fCommand = new DelegateCommand(this.goPageF);
            this.gCommand = new DelegateCommand(this.goPageG);
            this.hCommand = new DelegateCommand(this.goPageH);

            this.iCommand = new DelegateCommand(this.goPageI);
            this.jCommand = new DelegateCommand(this.goPageJ);
            this.kCommand = new DelegateCommand(this.goPageK);
            this.lCommand = new DelegateCommand(this.goPageL);
            this.mCommand = new DelegateCommand(this.goPageM);
            this.nCommand = new DelegateCommand(this.goPageN);
            this.oCommand = new DelegateCommand(this.goPageO);
            this.pCommand = new DelegateCommand(this.goPageP);

            this.qCommand = new DelegateCommand(this.goPageQ);
            this.rCommand = new DelegateCommand(this.goPageR);
            this.sCommand = new DelegateCommand(this.goPageS);
            this.tCommand = new DelegateCommand(this.goPageT);
            this.uCommand = new DelegateCommand(this.goPageU);
            this.vCommand = new DelegateCommand(this.goPageV);
            this.wCommand = new DelegateCommand(this.goPageW);
            this.xCommand = new DelegateCommand(this.goPageX);
            this.yCommand = new DelegateCommand(this.goPageY);
            this.zCommand = new DelegateCommand(this.goPageZ);

            this.speakWordCommand = new DelegateCommand(this.SpeakWord);
            #endregion
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
                20,
                25,
                50,
                100
            };

            fontSizeOptions = new List<int>
            {
                12,
                14,
                16,
                18,
                20,
                22,
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
            selectedText2 = "";
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

        #region goPages
        private void goPage(int idx)
        {
            TextDocument tmp = (TextDocument)this.TextDocument.Clone();
            tmp.Text = tmp.TxtList[idx];
            TextDocument = tmp;
            pageIdx = idx;
        }

        private void goPageA()
        {
            goPage(0);
        }

        private void goPageB()
        {
            goPage(1);
        }

        private void goPageC()
        {
            goPage(2);
        }

        private void goPageD()
        {
            goPage(3);
        }

        private void goPageE()
        {
            goPage(4);
        }

        private void goPageF()
        {
            goPage(5);
        }

        private void goPageG()
        {
            goPage(6);
        }

        private void goPageH()
        {
            goPage(7);
        }

        private void goPageI()
        {
            goPage(8);
        }

        private void goPageJ()
        {
            goPage(9);
        }

        private void goPageK()
        {
            goPage(10);
        }

        private void goPageL()
        {
            goPage(11);
        }

        private void goPageM()
        {
            goPage(12);
        }

        private void goPageN()
        {
            goPage(13);
        }

        private void goPageO()
        {
            goPage(14);
        }

        private void goPageP()
        {
            goPage(15);
        }

        private void goPageQ()
        {
            goPage(16);
        }

        private void goPageR()
        {
            goPage(17);
        }

        private void goPageS()
        {
            goPage(18);
        }

        private void goPageT()
        {
            goPage(19);
        }

        private void goPageU()
        {
            goPage(20);
        }

        private void goPageV()
        {
            goPage(21);
        }

        private void goPageW()
        {
            goPage(22);
        }

        private void goPageX()
        {
            goPage(23);
        }

        private void goPageY()
        {
            goPage(24);
        }

        private void goPageZ()
        {
            goPage(25);
        }
        #endregion 

        public void setEditbox(FsRichTextBox txtBox)
        {
            editBox = txtBox;
            editBox.FontSize = fontSize;
        }

        void OnWord(object sender, SpeakProgressEventArgs e)
        {
            if (!wordSpeak)
                editBox.HighlightWordInRichTextBox(e.Text);
        }

        #region pageCommands
        public ICommand ACommand
        {
            get { return this.aCommand; }
        }

        public ICommand BCommand
        {
            get { return this.bCommand; }
        }

        public ICommand CCommand
        {
            get { return this.cCommand; }
        }

        public ICommand DCommand
        {
            get { return this.dCommand; }
        }

        public ICommand ECommand
        {
            get { return this.eCommand; }
        }

        public ICommand FCommand
        {
            get { return this.fCommand; }
        }

        public ICommand GCommand
        {
            get { return this.gCommand; }
        }

        public ICommand HCommand
        {
            get { return this.hCommand; }
        }

        public ICommand ICommand
        {
            get { return this.iCommand; }
        }

        public ICommand JCommand
        {
            get { return this.jCommand; }
        }

        public ICommand KCommand
        {
            get { return this.kCommand; }
        }

        public ICommand LCommand
        {
            get { return this.lCommand; }
        }

        public ICommand MCommand
        {
            get { return this.mCommand; }
        }

        public ICommand NCommand
        {
            get { return this.nCommand; }
        }

        public ICommand OCommand
        {
            get { return this.oCommand; }
        }

        public ICommand PCommand
        {
            get { return this.pCommand; }
        }

        public ICommand QCommand
        {
            get { return this.qCommand; }
        }

        public ICommand RCommand
        {
            get { return this.rCommand; }
        }

        public ICommand SCommand
        {
            get { return this.sCommand; }
        }

        public ICommand TCommand
        {
            get { return this.tCommand; }
        }

        public ICommand UCommand
        {
            get { return this.uCommand; }
        }

        public ICommand VCommand
        {
            get { return this.vCommand; }
        }

        public ICommand WCommand
        {
            get { return this.wCommand; }
        }

        public ICommand XCommand
        {
            get { return this.xCommand; }
        }

        public ICommand YCommand
        {
            get { return this.yCommand; }
        }

        public ICommand ZCommand
        {
            get { return this.zCommand; }
        }
        #endregion

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

        public List<int> FontSizeOptions
        {
            get
            {
                return this.fontSizeOptions;
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
                editBox.FontSize = fontSize;
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

        #region visbles
        public bool AVisible
        {
            get
            {
                return this.aVisible;
            }

            set
            {
                this.SetProperty(ref this.aVisible, value);
            }
        }

        public bool BVisible
        {
            get
            {
                return this.bVisible;
            }

            set
            {
                this.SetProperty(ref this.bVisible, value);
            }
        }

        public bool CVisible
        {
            get
            {
                return this.cVisible;
            }

            set
            {
                this.SetProperty(ref this.cVisible, value);
            }
        }

        public bool DVisible
        {
            get
            {
                return this.dVisible;
            }

            set
            {
                this.SetProperty(ref this.dVisible, value);
            }
        }

        public bool EVisible
        {
            get
            {
                return this.eVisible;
            }

            set
            {
                this.SetProperty(ref this.eVisible, value);
            }
        }

        public bool FVisible
        {
            get
            {
                return this.fVisible;
            }

            set
            {
                this.SetProperty(ref this.fVisible, value);
            }
        }

        public bool GVisible
        {
            get
            {
                return this.gVisible;
            }

            set
            {
                this.SetProperty(ref this.gVisible, value);
            }
        }

        public bool HVisible
        {
            get
            {
                return this.hVisible;
            }

            set
            {
                this.SetProperty(ref this.hVisible, value);
            }
        }

        public bool IVisible
        {
            get
            {
                return this.iVisible;
            }

            set
            {
                this.SetProperty(ref this.iVisible, value);
            }
        }

        public bool JVisible
        {
            get
            {
                return this.jVisible;
            }

            set
            {
                this.SetProperty(ref this.jVisible, value);
            }
        }

        public bool KVisible
        {
            get
            {
                return this.kVisible;
            }

            set
            {
                this.SetProperty(ref this.kVisible, value);
            }
        }

        public bool LVisible
        {
            get
            {
                return this.lVisible;
            }

            set
            {
                this.SetProperty(ref this.lVisible, value);
            }
        }

        public bool MVisible
        {
            get
            {
                return this.mVisible;
            }

            set
            {
                this.SetProperty(ref this.mVisible, value);
            }
        }

        public bool NVisible
        {
            get
            {
                return this.nVisible;
            }

            set
            {
                this.SetProperty(ref this.nVisible, value);
            }
        }

        public bool OVisible
        {
            get
            {
                return this.oVisible;
            }

            set
            {
                this.SetProperty(ref this.oVisible, value);
            }
        }

        public bool PVisible
        {
            get
            {
                return this.pVisible;
            }

            set
            {
                this.SetProperty(ref this.pVisible, value);
            }
        }

        public bool QVisible
        {
            get
            {
                return this.qVisible;
            }

            set
            {
                this.SetProperty(ref this.qVisible, value);
            }
        }

        public bool RVisible
        {
            get
            {
                return this.rVisible;
            }

            set
            {
                this.SetProperty(ref this.rVisible, value);
            }
        }

        public bool SVisible
        {
            get
            {
                return this.sVisible;
            }

            set
            {
                this.SetProperty(ref this.sVisible, value);
            }
        }

        public bool TVisible
        {
            get
            {
                return this.tVisible;
            }

            set
            {
                this.SetProperty(ref this.tVisible, value);
            }
        }

        public bool UVisible
        {
            get
            {
                return this.uVisible;
            }

            set
            {
                this.SetProperty(ref this.uVisible, value);
            }
        }

        public bool VVisible
        {
            get
            {
                return this.vVisible;
            }

            set
            {
                this.SetProperty(ref this.vVisible, value);
            }
        }

        public bool WVisible
        {
            get
            {
                return this.wVisible;
            }

            set
            {
                this.SetProperty(ref this.wVisible, value);
            }
        }

        public bool XVisible
        {
            get
            {
                return this.xVisible;
            }

            set
            {
                this.SetProperty(ref this.xVisible, value);
            }
        }

        public bool YVisible
        {
            get
            {
                return this.yVisible;
            }

            set
            {
                this.SetProperty(ref this.yVisible, value);
            }
        }

        public bool ZVisible
        {
            get
            {
                return this.zVisible;
            }

            set
            {
                this.SetProperty(ref this.zVisible, value);
            }
        }
#endregion

        #region labels
        public string ALabel
        {
            get
            {
                return this.aLable;
            }

            set
            {
                this.SetProperty(ref this.aLable, value);
            }
        }

        public string BLabel
        {
            get
            {
                return this.bLable;
            }

            set
            {
                this.SetProperty(ref this.bLable, value);
            }
        }

        public string CLabel
        {
            get
            {
                return this.cLable;
            }

            set
            {
                this.SetProperty(ref this.cLable, value);
            }
        }

        public string DLabel
        {
            get
            {
                return this.dLable;
            }

            set
            {
                this.SetProperty(ref this.dLable, value);
            }
        }

        public string ELabel
        {
            get
            {
                return this.eLable;
            }

            set
            {
                this.SetProperty(ref this.eLable, value);
            }
        }

        public string FLabel
        {
            get
            {
                return this.fLable;
            }

            set
            {
                this.SetProperty(ref this.fLable, value);
            }
        }

        public string GLabel
        {
            get
            {
                return this.gLable;
            }

            set
            {
                this.SetProperty(ref this.gLable, value);
            }
        }

        public string HLabel
        {
            get
            {
                return this.hLable;
            }

            set
            {
                this.SetProperty(ref this.hLable, value);
            }
        }

        public string ILabel
        {
            get
            {
                return this.iLable;
            }

            set
            {
                this.SetProperty(ref this.iLable, value);
            }
        }

        public string JLabel
        {
            get
            {
                return this.jLable;
            }

            set
            {
                this.SetProperty(ref this.jLable, value);
            }
        }

        public string KLabel
        {
            get
            {
                return this.kLable;
            }

            set
            {
                this.SetProperty(ref this.kLable, value);
            }
        }

        public string LLabel
        {
            get
            {
                return this.lLable;
            }

            set
            {
                this.SetProperty(ref this.lLable, value);
            }
        }

        public string MLabel
        {
            get
            {
                return this.mLable;
            }

            set
            {
                this.SetProperty(ref this.mLable, value);
            }
        }

        public string NLabel
        {
            get
            {
                return this.nLable;
            }

            set
            {
                this.SetProperty(ref this.nLable, value);
            }
        }

        public string OLabel
        {
            get
            {
                return this.oLable;
            }

            set
            {
                this.SetProperty(ref this.oLable, value);
            }
        }

        public string PLabel
        {
            get
            {
                return this.pLable;
            }

            set
            {
                this.SetProperty(ref this.pLable, value);
            }
        }

        public string QLabel
        {
            get
            {
                return this.qLable;
            }

            set
            {
                this.SetProperty(ref this.qLable, value);
            }
        }

        public string RLabel
        {
            get
            {
                return this.rLable;
            }

            set
            {
                this.SetProperty(ref this.rLable, value);
            }
        }

        public string SLabel
        {
            get
            {
                return this.sLable;
            }

            set
            {
                this.SetProperty(ref this.sLable, value);
            }
        }

        public string TLabel
        {
            get
            {
                return this.tLable;
            }

            set
            {
                this.SetProperty(ref this.tLable, value);
            }
        }

        public string ULabel
        {
            get
            {
                return this.uLable;
            }

            set
            {
                this.SetProperty(ref this.uLable, value);
            }
        }

        public string VLabel
        {
            get
            {
                return this.vLable;
            }

            set
            {
                this.SetProperty(ref this.vLable, value);
            }
        }

        public string WLabel
        {
            get
            {
                return this.wLable;
            }

            set
            {
                this.SetProperty(ref this.wLable, value);
            }
        }

        public string XLabel
        {
            get
            {
                return this.xLable;
            }

            set
            {
                this.SetProperty(ref this.xLable, value);
            }
        }

        public string YLabel
        {
            get
            {
                return this.yLable;
            }

            set
            {
                this.SetProperty(ref this.yLable, value);
            }
        }

        public string ZLabel
        {
            get
            {
                return this.zLable;
            }

            set
            {
                this.SetProperty(ref this.zLable, value);
            }
        }
        #endregion
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

        private void Mp3()
        {
            wordSpeak = false;
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

            voice.Volume = volume;
            voice.Rate = rate - 10;

            voice.SetOutputToDefaultAudioDevice();  
            editBox.ResetPointer();
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

        public void changeVisible()
        {
            repeatCnt = 1000;
            Stop();
        }

        private void Update()
        {
            Stop();
            editBox.UpdateDocumentBindings();   
        }

        private void Save()
        {
            string txt = TTService.TITLE_KEY + textDocument.Subject + "\n";
            textDocument.TxtList[pageIdx] = PlainText();
            int cnt = textDocument.TxtList.Count;
            string pHeader = null;
            for (int i = 0; i < cnt; i++)
            {
                pHeader = "\nPAGE::" + textDocument.SubjectList[i] + "\n";
                txt += pHeader;
                txt += textDocument.TxtList[i];
            } 
            
            File.WriteAllText(textDocument.FileName, txt);
        }


        private void SaveAs()
        {
            Stop();
            
            SaveFileDialog saveFile1 = new SaveFileDialog();

            saveFile1.Filter = "Notes Files|*" + TTService.STORY;

            string txt = TTService.TITLE_KEY + textDocument.Subject + "\n";

            // Determine whether the user selected a file name from the saveFileDialog. 
            if (saveFile1.ShowDialog() == DialogResult.OK &&
                   saveFile1.FileName.Length > 0) {

                textDocument.TxtList[pageIdx] = PlainText();

                int cnt = textDocument.TxtList.Count;
                string pHeader = null;
                for (int i = 0; i < cnt; i++)
                {
                    pHeader = "\nPAGE::" + textDocument.SubjectList[i] + "\n";
                    txt += pHeader;
                    txt += textDocument.TxtList[i];
                }
                File.WriteAllText(saveFile1.FileName, txt);
            }
            
        }

        string PlainText()
        {
            //return editBox.To;
            return editBox.GetPlainText();
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

        private void setVisibles(TextDocument doc, int numPages)
        {
            AVisible = false;
            BVisible = false;
            CVisible = false;
            DVisible = false;
            EVisible = false;
            FVisible = false;
            GVisible = false;
            HVisible = false;
            IVisible = false;
            JVisible = false;
            KVisible = false;
            LVisible = false;

            MVisible = false;
            NVisible = false;
            OVisible = false;
            PVisible = false;
            QVisible = false;
            RVisible = false;
            SVisible = false;
            TVisible = false;
            UVisible = false;
            VVisible = false;
            WVisible = false;
            XVisible = false;
            YVisible = false;
            ZVisible = false;

            if (numPages > 1)
            {
                AVisible = true;
                BVisible = true;
                ALabel = doc.SubjectList[0];
                BLabel = doc.SubjectList[1];
            }
                
            if (numPages > 2)
            {
                CVisible = true;
                CLabel = doc.SubjectList[2];
            }
                
                
            
            if (numPages > 3)
            {
                DVisible = true;
                DLabel = doc.SubjectList[3];
            }
                

            if (numPages > 4)
            {
                EVisible = true;
                ELabel = doc.SubjectList[4];
            }
                

            if (numPages > 5)
            {
                FVisible = true;
                FLabel = doc.SubjectList[5];
            }
                

            if (numPages > 6)
            {
                GVisible = true;
                GLabel = doc.SubjectList[6];
            }
                

            if (numPages > 7)
            {
                HVisible = true;
                HLabel = doc.SubjectList[7];
            }
                

            if (numPages > 8)
            {
                IVisible = true;
                ILabel = doc.SubjectList[8];
            }
                

            if (numPages > 9)
            {
                JVisible = true;
                JLabel = doc.SubjectList[9];
            }
                

            if (numPages > 10)
            {
                KVisible = true;
                KLabel = doc.SubjectList[10];
            }
                

            if (numPages > 11)
            {
                LVisible = true;
                LLabel = doc.SubjectList[11];
            }
                

            if (numPages > 12)
            {
                MVisible = true;
                MLabel = doc.SubjectList[12];
            }
                

            if (numPages > 13)
            {
                NVisible = true;
                NLabel = doc.SubjectList[13];
            }
                

            if (numPages > 14)
            {
                OVisible = true;
                OLabel = doc.SubjectList[14];
            }
                

            if (numPages > 15)
            {
                PVisible = true;
                PLabel = doc.SubjectList[15];
            }
                

            if (numPages > 16)
            {
                QVisible = true;
                QLabel = doc.SubjectList[16];
            }
                

            if (numPages > 17)
            {
                RVisible = true;
                RLabel = doc.SubjectList[17];
            }
                

            if (numPages > 18)
            {
                SVisible = true;
                SLabel = doc.SubjectList[18];
            }
                

            if (numPages > 19)
            {
                TVisible = true;
                TLabel = doc.SubjectList[19];
            }
                

            if (numPages > 20)
            {
                UVisible = true;
                ULabel = doc.SubjectList[20];
            }
                

            if (numPages > 21)
            {
                VVisible = true;
                VLabel = doc.SubjectList[21];
            }
               

            if (numPages > 22)
            {
                WVisible = true;
                WLabel = doc.SubjectList[22];
            }
                

            if (numPages > 23)
            {
                XVisible = true;
                XLabel = doc.SubjectList[23];
            }
               

            if (numPages > 24)
            {
                YVisible = true;
                YLabel = doc.SubjectList[24];
            }
                

            if (numPages > 25)
            {
                ZVisible = true;
                ZLabel = doc.SubjectList[25];
            }
                

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
                TextDocument temp = this.ttsService.GetFunDocument(textId.Value);
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("Must set CustomPrincipal object on startup.");
                //Authenticate the user
                CustomIdentity identity = customPrincipal.Identity;
                temp.StudentId = identity.Name;
                temp.From = identity.Fullname;
                int numPages = temp.SubjectList.Count;
                if (numPages > 0) setVisibles(temp, numPages);
                this.TextDocument = temp;
            }

            this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        #endregion
    }
}
