// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Linq;
using System.Threading;
using System.Speech.Recognition;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
//using System.Threading;

namespace SpeechSTT.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class STTViewModel : BindableBase, INavigationAware
    {
        private readonly DelegateCommand goBackCommand;
       
        private IRegionNavigationJournal navigationJournal;

        private readonly DelegateCommand startSpeechCommand;
        private readonly DelegateCommand stopSpeechCommand;

        private SpeechRecognitionEngine recognizer;

        private int recognized = 0;
        private int hypothesized = 0;
        
        bool started = false;

        string status = "READY TO START...";
        string recogText;

        private ObservableCollection<string> recognizedWords;

        

        //socket related
        public static string ipServer = "127.0.0.1";
        public static byte[] data = new byte[512];
        public static int port = 26000;
        public static double validity = 0.70f;
        Socket server;
        IPEndPoint iep;

        [ImportingConstructor]
        public STTViewModel()
        {
            this.goBackCommand = new DelegateCommand(this.GoBack);
            this.startSpeechCommand = new DelegateCommand(this.StartSpeech);
            this.stopSpeechCommand = new DelegateCommand(this.StopSpeech);

            recognizedWords = new ObservableCollection<string>();

            InitializeRecognizerSynthesizer();

            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            iep = new IPEndPoint(IPAddress.Parse(ipServer), port);
            //server.Bind(iep);
        }

        public IEnumerable<string> RecognizedWords
        {
            get { return recognizedWords; }
        }

        private void InitializeRecognizerSynthesizer()
        {
            var selectedRecognizer = (from e in SpeechRecognitionEngine.InstalledRecognizers()
                                      where e.Culture.Equals(Thread.CurrentThread.CurrentCulture)
                                      select e).FirstOrDefault();
            recognizer = new SpeechRecognitionEngine(selectedRecognizer);

            Choices texts = new Choices();
            texts.Add("Hello");
            texts.Add("You are so stupid");
            texts.Add("I am Charlie Jiang");
            texts.Add("My Name is Eileen Jiang");
            texts.Add("Who are You?");
            texts.Add("My Mom is Fengmei Xiao");
            texts.Add("My brother name is Andy");
            texts.Add("You are very pretty, What's your name ?");
            texts.Add("You look very young, Would you tell me how old are you ?");
            texts.Add("Go there to take a box for me");
            texts.Add("This computer is so slow, I want to buy a new one");
            texts.Add("iPhone is very expensive, Can you afford to buy one ?");

            texts.Add("This computer is so dumb, can not understand everything I say");
            texts.Add("I will invent a smarter computer when I grow up");
            texts.Add("My daughter loves games, Do you like to play chess ?");
            texts.Add("I'm not a good chess player, but I enjoy playing");
            texts.Add("My mom always pushes me to do everything she likes, not what I like");
            texts.Add("My daddy is short and bald, but very funny ?");
            texts.Add("I don't know much about chess, Would you please teach me how to play?");
            texts.Add("My mom likes to be manager, but never earn big money");
            texts.Add("I like to play chess with my daddy, but he never win a game against me");
            texts.Add("He is very handsome, Do you know how to make people ugly? eat eou !");

            texts.Add("End Dictate");
            Grammar wordsList = new Grammar(new GrammarBuilder(texts));
            recognizer.LoadGrammar(wordsList);

            recognizer.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>(recognizer_AudioStateChanged);
            recognizer.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(recognizer_SpeechHypothesized);
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
           
        }


        public int Hypothesized
        {
            get
            {
                return this.hypothesized;
            }

            set
            {
                this.SetProperty(ref this.hypothesized, value);
            }
        }

        public int Recognized
        {
            get
            {
                return this.recognized;
            }

            set
            {
                this.SetProperty(ref this.recognized, value);
            }
        }

        public bool Started
        {
            get
            {
                return this.started;
            }

            set
            {
                this.SetProperty(ref this.started, value);
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

        public string RecogText
        {
            get
            {
                return this.recogText;
            }

            set
            {
                this.SetProperty(ref this.recogText, value);
                recognizedWords.Insert(0, recogText);
            }
        }

        private void recognizer_AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            switch (e.AudioState)
            {
                case AudioState.Speech:
                    this.Status = "Listening";
                    break;
                case AudioState.Silence:
                    this.Status = "Idle";
                    break;
                case AudioState.Stopped:
                    this.Status = "Stopped";
                    break;
            }
        }

        private void recognizer_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            this.Hypothesized++;
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            this.Recognized++;

            if (!started) return;

            float accuracy = (float)e.Result.Confidence;

            string phrase = e.Result.Text;
            {
                if (phrase == "End Dictate")
                {
                    this.Started = false;
                    recognizer.RecognizeAsyncStop();
                }
               
                this.RecogText = phrase;

                //send
                data = Encoding.ASCII.GetBytes(phrase);
                //if (!server.Connected) server.Connect(iep);
                server.SendTo(data, iep);
            }
        }

        public ICommand GoBackCommand
        {
            get { return this.goBackCommand; }
        }

        public ICommand StartSpeechCommand
        {
            get { return this.startSpeechCommand; }
        }

        public ICommand StopSpeechCommand
        {
            get { return this.stopSpeechCommand; }
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
        private void StartSpeech()
        {
            this.Started = true;
            this.Status = "Started...";
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void StopSpeech()
        {
            this.Started = false;
            recognizer.RecognizeAsyncStop();
            //server.Close();
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
