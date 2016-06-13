//-------------------------------------------------
// Copyright (c) ECT Education Group
// Author: Charlie Jiang
// Date  : 03/02/16
//-------------------------------------------------
using System;
using System.Collections.Generic;
using SpeechInfrastructure;

namespace SpeechTTS.Model
{
    public interface ITTService
    {
        int getNumNotes();
        int getNumCarts();
        int getNumClasses();
        int getNumIdioms();
        int getNumSounds();
        int getNumGrammars();

        string getAudioPath();
        string getDefaultUserPath();

        int FunIdx(TextDocument doc);
        IAsyncResult BeginGetFunDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetFunDocuments(IAsyncResult result);
        IAsyncResult BeginSendFunDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendFunDocument(IAsyncResult result);
        TextDocument GetFunDocument(Guid id);
        void SaveFunDocuments();
        void AddFunDocument(TextDocument doc);
        void RemoveFunDocument(TextDocument doc);

        int CardIdx(TextDocument doc);
        IAsyncResult BeginGetCardsDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetCardsDocuments(IAsyncResult result);
        IAsyncResult BeginSendCardsDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendCardsDocument(IAsyncResult result);
        TextDocument GetCardsDocument(Guid id);
        void SaveCardsDocuments();
        void AddCardsDocument(TextDocument doc);
        void RemoveCardsDocument(TextDocument doc);

        int ClassIdx(TextDocument doc);
        IAsyncResult BeginGetActivitiesDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetActivitiesDocuments(IAsyncResult result);
        IAsyncResult BeginSendActivitiesDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendActivitiesDocument(IAsyncResult result);
        TextDocument GetActivitiesDocument(Guid id);
        void SaveActivitiesDocuments();
        void AddActivitiesDocument(TextDocument doc);
        void RemoveActivitiesDocument(TextDocument doc);

        int IdiomIdx(TextDocument doc);
        IAsyncResult BeginGetIdiomsDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetIdiomsDocuments(IAsyncResult result);
        IAsyncResult BeginSendIdiomsDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendIdiomsDocument(IAsyncResult result);
        TextDocument GetIdiomsDocument(Guid id);
        void SaveIdiomsDocuments();
        void AddIdiomsDocument(TextDocument doc);
        void RemoveIdiomsDocument(TextDocument doc);

        int SoundIdx(TextDocument doc);
        IAsyncResult BeginGetSoundDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetSoundDocuments(IAsyncResult result);
        IAsyncResult BeginSendSoundDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendSoundDocument(IAsyncResult result);
        TextDocument GetSoundDocument(Guid id);
        void SaveSoundDocuments();
        void AddSoundDocument(TextDocument doc);
        void RemoveSoundDocument(TextDocument doc);

        int GrammarIdx(TextDocument doc);
        IAsyncResult BeginGetGrammarDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetGrammarDocuments(IAsyncResult result);
        IAsyncResult BeginSendGrammarDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendGrammarDocument(IAsyncResult result);
        TextDocument GetGrammarDocument(Guid id);
        void SaveGrammarDocuments();
        void AddGrammarDocument(TextDocument doc);
        void RemoveGrammarDocument(TextDocument doc);

    }
}
