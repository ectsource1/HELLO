// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// http://englishharmony.com/phrases-to-use-at-home/
using System;
using System.IO;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Input;
using SpeechInfrastructure;
using SpeechTTS.Model;

namespace Parents.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ParentViewModel : BindableBase, INavigationAware
    {
        private readonly DelegateCommand resetCommand;
        private readonly DelegateCommand speakCommand;
        private readonly DelegateCommand stopCommand;
        private readonly DelegateCommand speakWordCommand;

        private bool readmeIsChecked    = true;
        private bool dialog1IsChecked   = false;
        private bool dialog2IsChecked   = false;
        private bool translateIsChecked = false;

        private bool momChecked = false;
        private bool dadChecked = true;

        private bool isDialog = false;

        private string message = "";
        private int dialogIdx = 0;
        private int dialogCnt = 0;
        private bool repeatSelected = false;
        private bool speakSelected = false;

        private string readme    = "";
        private string dialog1   = "";
        private string dialog2   = "";
        private string translate = "";

        private List<string> genderList;
        private List<string> sentenceList;

        private List<string> voiceOptions;
        private string selectedVoice = "Male";
        private int volume = 100;
        private int rate = 10;
    
        private bool speakClickable = true;
        private bool stopClickable = false;
        private bool resumeClickable = false;

        private string selectedText;
        private List<int> repeatOptions;
        private int repeat = 5;
        private int repeatCnt = 0;
        private int at20 = 0;
        private ITTService ttsService;

        SpeechSynthesizer voice;

        [ImportingConstructor]
        public ParentViewModel(ITTService ttsService1)
        {
            ttsService = ttsService1;
            voice = new SpeechSynthesizer();
            voice.SpeakCompleted += OnSpeakCompleted;

            this.resetCommand = new DelegateCommand(this.Reset);
            this.speakCommand = new DelegateCommand(this.Speak);
            this.stopCommand = new DelegateCommand(this.Stop);
            this.speakWordCommand = new DelegateCommand(this.SpeakWord);
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
                50
            };

            genderList = new List<string>();
            sentenceList = new List<string>();

            SelectedText = "";

            addReadme();
            add50();
            addTranslate();

            string fileName = ttsService.getDefaultUserPath();
            fileName = fileName + Personal.PERSON_BIN;

            string greeting = "Hello, my friend";
            if (File.Exists(fileName))
            {
                Personal person = Personal.read(fileName);
                string nickname = person.Nickname;
                if (!string.IsNullOrEmpty(nickname))
                   greeting = "Hello, " + person.Nickname;
                
            }

            //greeting += ", Welcome to the Parents-Must-Read Module!";
            //voice.SpeakAsync(greeting);
           
        }

        public ICommand ResetCommand
        {
            get { return this.resetCommand; }
        }

        public ICommand SpeakCommand
        {
            get { return this.speakCommand; }
        }

        public ICommand StopCommand
        {
            get { return this.stopCommand; }
        }

        public ICommand SpeakWordCommand
        {
            get { return this.speakWordCommand; }
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

                    this.StopClickable = false;
                    this.SpeakClickable = true;
                }
            }

            if (speakSelected)
            {
                if (dialogIdx < dialogCnt)
                {
                    this.StopClickable = true;
                    this.SpeakClickable = false;
                    this.Speak();
                }
                else
                {
                    speakSelected = false;

                    if (dialog1IsChecked)
                       this.DialogIdx = 0;
                    else
                       this.DialogIdx = at20;

                    this.StopClickable = false;
                    this.ResumeClickable = false;
                    this.SpeakClickable = true;
                }
            }


            this.Message = "Done Reading!!";
        }

        private void Reset()
        {
            this.Rate = 10;
            this.Volume = 100;
        }

        private void Speak()
        {
            speakSelected = true;

            voice.Volume = volume;
            voice.Rate = Rate - 10;

            if (dialogIdx > dialogCnt) return;

            string gender = GenderList[dialogIdx];
            string sentence = SentenceList[dialogIdx];
            if (dadChecked)
            {
                if (gender.Contains("A"))

                    voice.SelectVoiceByHints(VoiceGender.Male);
                else
                    voice.SelectVoiceByHints(VoiceGender.Female);
            } else
            {
                if (gender.Contains("B"))
                    voice.SelectVoiceByHints(VoiceGender.Male);
                else
                    voice.SelectVoiceByHints(VoiceGender.Female);
            }
            
            voice.SpeakAsync(sentence);

            DialogIdx += 1;
        }

        private void SpeakWord()
        {
            RepeatSelected = true;
            if (selectedText.Length < 2) return;

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

        private void Stop()
        {
            voice.SpeakAsyncCancelAll();

            DialogIdx = 1000;

            this.StopClickable = false;
            this.SpeakClickable = true;
            this.ResumeClickable = false;
        }

        public void changeVisible()
        {
            if (dialogIdx != 0) {
                Stop();
            }
        }
         
        public List<string> VoiceOptions
        {
            get
            {
                return this.voiceOptions;
            }
        }

        public string SelectedVoice
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

        public bool ReadmeIsChecked
        {
            get
            {
                return this.readmeIsChecked;
            }

            set
            {
                this.SetProperty(ref this.readmeIsChecked, value);
            }
        }

        public bool IsDialog
        {
            get
            {
                return this.isDialog;
            }

            set
            {
                this.SetProperty(ref this.isDialog, value);

            }
        }

        public bool Dialog1IsChecked
        {
            get
            {
                return this.dialog1IsChecked;
            }

            set
            {
                this.SetProperty(ref this.dialog1IsChecked, value);
                IsDialog = dialog1IsChecked || dialog2IsChecked;
                if (dialog1IsChecked)
                {
                    dialogIdx = 0;
                    dialogCnt = at20;
                }
            }
        }

        public bool Dialog2IsChecked
        {
            get
            {
                return this.dialog2IsChecked;
            }

            set
            {
                this.SetProperty(ref this.dialog2IsChecked, value);
                IsDialog = dialog1IsChecked || dialog2IsChecked;
                if (dialog2IsChecked)
                {
                    dialogIdx = at20;
                    dialogCnt = genderList.Count;
                }
            }
        }

        public bool TranslateIsChecked
        {
            get
            {
                return this.translateIsChecked;
            }

            set
            {
                this.SetProperty(ref this.translateIsChecked, value);
            }
        }

        public bool MomChecked
        {
            get
            {
                return this.momChecked;
            }

            set
            {
                this.SetProperty(ref this.momChecked, value);
            }
        }

        public bool DadChecked
        {
            get
            {
                return this.dadChecked;
            }

            set
            {
                this.SetProperty(ref this.dadChecked, value);
            }
        }

        public string Readme
        {
            get
            {
                return this.readme;
            }

            set
            {
                this.SetProperty(ref this.readme, value);
            }
        }

        public List<string> GenderList
        {
            get { return this.genderList; }
        }

        public List<string> SentenceList
        {
            get { return this.sentenceList; }
        }

        public string Dialog1
        {
            get { return this.dialog1; }
        }

        public string Dialog2
        {
            get { return this.dialog2; }
        }

        public string Translate
        {
            get { return this.translate; }
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
            //this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        #endregion

        void addReadme()
        {
            readme = "学好英文必须具备几个要素：正确的学习方法，好的语言环境，有自我纠正发音的机会，日常生活中能有练习的场合，";
            readme += "另外学生要自我感觉到学英语是有用的，有主观理由去学外语，充分发挥其积极性。\n\n";
            readme += "生活在母语国家去学第二语言，没有良好的语言环境，无法纠正自己的发音，也没有练习的机会。";
            readme += "这就是为什么大部分中国孩子努力了十几年，最终还是哑巴英语。有些有经济条件好的家庭每年花费十几万";
            readme += "送孩子去国际学校读书，效果也不一定很明显，因为小孩自然倾向于选择讲中文的同学交流和玩耍。\n\n";
            readme += "ECT艾琳国际英语学校针对所有学生在学习英语方面的种种问题，设计了多方位的课堂教学内容（琴棋书画），";
            readme += "并自主开发了一套提高听说读写能力的电子私人外教软件,以此建立良好的英语学习环境，每个英语单词，";
            readme += "每个英语句子都有标准的美国发音，学生可反复模仿来提高语感和音准度，软件包括视频，卡通, 情景对话，";
            readme += "美国成语（俚语），英文笔记等8个模块. ECT的软件相当于你的私人外教，只要坚持学习，一定能学到完美英语。\n\n";
            readme += "家长需要意识到的是，光凭课堂教学一点时间是远远不够的，学生需要反复练习。父母也要参与孩子的学习中。";
            readme += "在家里也要尽可能地创造一些英语学习和巩固的环境。为此ECT 制作了父母英语50句，家长可以在家庭温馨的氛围 ";
            readme += "与自己的子女用英语交流。要求家长把这50句学得流利标准。\n\n";
            readme += "考虑到有的家长自己不喜欢英语，我们把50句分两部分，Part 1: 20句, Part 2: 30句， 父母必须学会前";
            readme += "20句，给孩子一个榜样的力量，更能提高您儿女的兴趣。\n\n";

            readme += "ECT 会对入学家长的孩子免费培训英语50句，并同时培训如何使用电子私人外教软件。";
            readme += "建议父母熟练掌握和流利讲出所有 A 句， B 句是你孩子讲的，场景不同会有不同的回答。请每天与孩子交流几句，有利于我们整体教学的快速进展！\n\n";

            readme += "关于学习方法，学生需要注意几点：\n";
            readme += "1. 不要过早学习语法，掌握一定词汇并能简单交流后再慢慢引进语法概念。我们希望孩子能学到以英语思维的地道英语，";
            readme += "语法知识会对英语考试有帮助，但早期实际上是有害的。\n";
            readme += "2. 不要过早根据音标发音，而是要跟标准发音模仿。有的学生本身音标发音都很不标准，怎么会正确对单词和句子发音呢。模仿";
            readme += "多了，再学一些基本发音规则，就会对所有单词正确发音。所有新词新句都必须先经过耳朵过滤几十遍.\n";
            readme += "3. 不要强调记单个词，要记单词关联的短语，这样既对单词有深刻理解，并且马上可以在口语和写作中用上，用词方式还是没有语法错误的标准表达形式。\n";
            readme += "4. 如果要记单词，要同时找一些近义词和反义词一起记忆，这样既能扩大词汇量，又能加深理解，提供更多口语和写作的素材。\n";
            readme += "5. 多模仿，多练习，反复听，重复讲。";


        }

        void addTranslate()
        {
            translate = "如家长需要，ECT会对50句做详细注解.\n\n";

            translate += "Take out the trash (American)\n";
            translate += "Put out the (waste) bins (British)\n";

            translate += "Can you please do the dishes?\n";
            translate += "Do the laundry\n";
            translate += "Tidy up you room\n";
            translate += "Clean it up\n";
            translate += "Make the bed \n";
            translate += "Get dressed\n";
            translate += "Did you lock the door?\n";
            translate += "When's your homework due？\n";
            translate += "You have to be ready for school in… minutes!\n";
            translate += "You’re off school for a week\n";
            translate += "What's for dinner?\n";
            translate += "Who’s cooking tonight?\n";
            translate += "Put the kettle on!\n";
            translate += "Take a few more bites!\n";
            translate += "Close your eyes and count till ten!\n";
            translate += "Want a piggyback ride?\n";
            translate += "Go easy on him (her)\n";
            translate += "That’s my boy (girl)!\n";
            translate += "Common, you can do it!\n";

            translate += "You beat me again! \n";
            translate += "Gotcha!\n";
            translate += "Let me tuck you in!\n";
            translate += "Sleep tight!\n";

            translate += "Sleep in\n";
            translate += "Time to get up!\n";
            translate += "Rise and shine!\n";
            translate += "Sleep well? \n";

            translate += "Are you warm enough?\n";
            translate += "Are you hurt?\n";
            translate += "Where are you hurt?\n";
            translate += "Where did you get hurt?  \n";

            translate += "Be nice to your (mom, sister, etc.\n";
            translate += "Where are your manners?\n";
            translate += "Don’t do that, it’s not nice! \n";
            translate += "How dare you speak to me like that?! \n";

            translate += "Hurry up!\n";
            translate += "Get ready!\n";
        }

        void add50()
        {
            // 1
            genderList.Add("A");
            sentenceList.Add("How're you doing today?");
            genderList.Add("B");
            sentenceList.Add("I'm fine, but I feel a bit tired.");

            // 2
            genderList.Add("\nA");
            sentenceList.Add("What's your teacher's name Sweetie ?");
            genderList.Add("B");
            sentenceList.Add("Which one? English or Math teacher?");

            // 3
            genderList.Add("\nA");
            sentenceList.Add("How many students are in your class?");
            genderList.Add("B");
            sentenceList.Add("About 50 students");

            // 4
            genderList.Add("\nA");
            sentenceList.Add("I love you, Sweetie !");
            genderList.Add("B");
            sentenceList.Add("Love you too !");

            // 5
            genderList.Add("\nA");
            sentenceList.Add("Why do you look so sad？");
            genderList.Add("B");
            sentenceList.Add("Grandpa didn't give me a birthday present");

            // 6
            genderList.Add("\nA");
            sentenceList.Add("Wow, You look really happy today!");
            genderList.Add("B");
            sentenceList.Add("I sure am. Today at school, I won all my chess games in the tournament");
            genderList.Add("A");
            sentenceList.Add("Awesome, I'm really proud of you");

            // 7
            genderList.Add("\nA");
            sentenceList.Add("You look very tired!");
            genderList.Add("B");
            sentenceList.Add("Yea, been working on my homework all day");

            // 8
            genderList.Add("\nA");
            sentenceList.Add("Close your eyes sweetie, I have a surprise for you");
            genderList.Add("B");
            sentenceList.Add("OK!");
            genderList.Add("A");
            sentenceList.Add("Ta-da, surprise!");
            genderList.Add("B");
            sentenceList.Add("Oh, my goodness. Thanks!, I have been dreaming about this for a while!");

            // 9
            genderList.Add("\nA");
            sentenceList.Add("Did you do your homework last night?");
            genderList.Add("B");
            sentenceList.Add("Yep, sure did!");

            // 10
            genderList.Add("\nA");
            sentenceList.Add("Have you done your homework yet?");
            genderList.Add("B");
            sentenceList.Add("No, not yet. I'll get to it later!");

            // 11
            genderList.Add("\nA");
            sentenceList.Add("When's your homework due?");
            genderList.Add("B");
            sentenceList.Add("Well, It's due on Friday so I've plenty of time to finish. There's no rush");

            // 12
            genderList.Add("\nA");
            sentenceList.Add("I always believed in you. In my eyes, you are the greatest thing that has happeded to me in my life.");
            genderList.Add("B");
            sentenceList.Add("Thanks, I will try my best and make you proud");


            // 13
            genderList.Add("\nA");
            sentenceList.Add("Do you like Eileen English School?");
            genderList.Add("B");
            sentenceList.Add("Yes, It's awesome!");

            // 14
            genderList.Add("\nA");
            sentenceList.Add("Alright, hurry up and get ready for school. we have to leave in 30 minutes");
            genderList.Add("B");
            sentenceList.Add("Yea, I know! I'm almost done !");

            // 15
            genderList.Add("\nA");
            sentenceList.Add("Are you hungry?");
            genderList.Add("B");
            sentenceList.Add("No, I had a big Lunch!");
            genderList.Add("B");
            sentenceList.Add("Yea, I'm starving.");

            genderList.Add("\nA");
            sentenceList.Add("Are you thirsty?  you want anything to drink, water, soda?");
            genderList.Add("B");
            sentenceList.Add("Yea, sure!, can I have a can of soda? Thanks!");

            // 16
            genderList.Add("\nA");
            sentenceList.Add("You enjoying the long holiday?");
            genderList.Add("B");
            sentenceList.Add("Not really, got nothing to do and I'm bored to death.");

            // 17
            genderList.Add("\nA");
            sentenceList.Add("Can you take out trash?");
            genderList.Add("B");
            sentenceList.Add("Yea sure, I'll do it on my way out");

            // 18
            genderList.Add("\nA");
            sentenceList.Add("Can you please do the dishes?");
            genderList.Add("B");
            sentenceList.Add("Yea sure, I'll get on it right away");

            // 19
            genderList.Add("\nA");
            sentenceList.Add("Can you do the laundry tonight? I'll do the dishes!");
            genderList.Add("B");
            sentenceList.Add("Sure thing, I'll do it right after I finish my homework!");

            // 20
            genderList.Add("\nA");
            sentenceList.Add("Could you tidy up your room, please?");
            genderList.Add("B");
            sentenceList.Add("Ok, I'll get on it soon!");

            at20 = 46;

            // 21
            genderList.Add("A");
            sentenceList.Add("Hi Sweetie, I just spilled my drink, can you clean it up?");
            genderList.Add("B");
            sentenceList.Add("Yes, where is Mob?");

            // 22
            genderList.Add("\nA");
            sentenceList.Add("Sweetie, can you make your bed before we leave?");
            genderList.Add("B");
            sentenceList.Add("Sure, I'll get right to it!");

            // 23
            genderList.Add("\nA");
            sentenceList.Add("Honey, I think it's time to get dressed? we have to go in an hour!");
            genderList.Add("B");
            sentenceList.Add("Ok, I'm putting on my coat!");

            // 24
            genderList.Add("\nA");
            sentenceList.Add("Did you lock the door!");
            genderList.Add("B");
            sentenceList.Add("Oh, my god, I forgot. The door is wide open. I'll go lock it right now");

            // 25
            genderList.Add("\nA");
            sentenceList.Add("Since you're off for a week, isn't it great to sleep in every morning?");
            genderList.Add("B");
            sentenceList.Add("Yes, I love long holidays");

            // 26
            genderList.Add("\nA");
            sentenceList.Add("What's for dinner");
            genderList.Add("B");
            sentenceList.Add("We are having noodles tonight");

            // 27
            genderList.Add("\nA");
            sentenceList.Add("Who's cooking tonight?");
            genderList.Add("B");
            sentenceList.Add("Grandma's cooking tonight! she has the best recipes");

            // 28
            genderList.Add("\nA");
            sentenceList.Add("Sweetie, Put the kettle on!");
            genderList.Add("B");
            sentenceList.Add("ok");

            // 29
            genderList.Add("\nA");
            sentenceList.Add("Sweetie, just take a few more bites and then you're free to go!");
            genderList.Add("B");
            sentenceList.Add("Ugh, I'm not hungry. Can't I eat it later?");

            // 30
            genderList.Add("\nA");
            sentenceList.Add("Close your eyes and count till ten!");
            genderList.Add("B");
            sentenceList.Add("ok, are you ready?");

            // 31
            genderList.Add("\nA");
            sentenceList.Add("Sweetie, want a piggyback ride?");
            genderList.Add("B");
            sentenceList.Add("Yea, sure !");

            // 32
            // 33
            genderList.Add("\nA");
            sentenceList.Add("Go easy on him, don't push too hard!");
            genderList.Add("B");
            sentenceList.Add("ok !");

            // 34
            genderList.Add("\nA");
            sentenceList.Add("That's my boy");
            genderList.Add("B");
            sentenceList.Add("Yea, He's very talented.");

            // 35
            genderList.Add("\nA");
            sentenceList.Add("Come on, you can do it !");
            genderList.Add("B");
            sentenceList.Add("Hold on, I'm trying!");

            // 36
            genderList.Add("\nA");
            sentenceList.Add("Wow, you beat me again !");
            genderList.Add("B");
            sentenceList.Add("Thanks, I've been practicing a lot lately");

            // 37
            genderList.Add("\nA");
            sentenceList.Add("Gotcha !");
            genderList.Add("B");
            sentenceList.Add("Can we play one more time! Please");

            // 38
            genderList.Add("\nA");
            sentenceList.Add("It's time to brush your teeth!");
            genderList.Add("B");
            sentenceList.Add("I already did it.");

            // 39
            genderList.Add("\nA");
            sentenceList.Add("Alright, time for bed! Let me tuck you in!");
            genderList.Add("B");
            sentenceList.Add("ok");

            // 40
            genderList.Add("\nA");
            sentenceList.Add("Did you sleep tight last night!");
            genderList.Add("B");
            sentenceList.Add("Yes, I slept really well last night");

            // 40-1
            genderList.Add("\nA");
            sentenceList.Add("Honey, you can sleep in tomorrow, the schools out for the next few days due to the bad weather!");
            genderList.Add("B");
            sentenceList.Add("Alright, cool!");

            // 41
            genderList.Add("\nA");
            sentenceList.Add("It's time to get up!");
            genderList.Add("B");
            sentenceList.Add("ok, ok, I'm still tired. 5 more minutes");
            genderList.Add("A");
            sentenceList.Add("Rise and shine!");

            // 42
            genderList.Add("\nA");
            sentenceList.Add("How do you feel? Hot or cold ?");
            genderList.Add("B");
            sentenceList.Add("Still a bit cold. Can you turn up the heater?");

            // 43
            genderList.Add("\nA");
            sentenceList.Add("Are you hurt?");
            genderList.Add("B");
            sentenceList.Add("No, I'm fine!");

            // 44
            genderList.Add("\nA");
            sentenceList.Add("Where are you hurt?");
            genderList.Add("B");
            sentenceList.Add("Right here, on my knee. It's bleeding.");

            // 45
            genderList.Add("\nA");
            sentenceList.Add("Where did you get hurt?");
            genderList.Add("B");
            sentenceList.Add("At the playground. I accidentally fell off the swings");

            // 46
            genderList.Add("\nA");
            sentenceList.Add("Try to be a bit nicer to Grandpa");
            genderList.Add("B");
            sentenceList.Add("Sorry, it won't happen again!");

            // 47
            genderList.Add("\nA");
            sentenceList.Add("Where are your manners?");
            genderList.Add("B");
            sentenceList.Add("sorry!");

            // 48
            genderList.Add("\nA");
            sentenceList.Add("Don't do that, that's not nice!");
            genderList.Add("B");
            sentenceList.Add("ok, sorry for that!");

            // 49
            genderList.Add("\nA");
            sentenceList.Add("How dare you speak to me like that?!");
            genderList.Add("B");
            sentenceList.Add("sorry, I didn't mean to.");

            // 50
            genderList.Add("\nA");
            sentenceList.Add("Hurry up! we gonna miss the school bus");
            genderList.Add("B");
            sentenceList.Add("Ok, I'm ready, let's go");

            int cnt = genderList.Count;

            string hstr = "Hello My Friend, Welcome to ECT English School!\n";
            hstr += "ECT wants you to be sucessfull in speaking fluent English!\n";
            hstr += "Lets work together to achieve what you want to be !!\n";
            hstr += "You need to login before you use this software.\n\n";

            dialog1 = hstr  + genderList[0] + ": " + sentenceList[0] + "\n";
            for (int i = 1; i < at20; i++)
            {
                dialog1 += genderList[i] + ": " + sentenceList[i] + "\n";
            }

            dialog2 = hstr + genderList[at20] + ": " + sentenceList[at20] + "\n";
            for (int i = at20 + 1; i < cnt; i++)
            {
                dialog2 += genderList[i] + ": " + sentenceList[i] + "\n";
            }
        }
    }
}
