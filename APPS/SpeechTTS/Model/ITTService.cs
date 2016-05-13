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
        string getAudioPath();

        IAsyncResult BeginGetFunDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetFunDocuments(IAsyncResult result);
        IAsyncResult BeginSendFunDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendFunDocument(IAsyncResult result);
        TextDocument GetFunDocument(Guid id);
        void SaveFunDocuments();
        void AddFunDocument(TextDocument doc);
        void RemoveFunDocument(TextDocument doc);

        IAsyncResult BeginGetCardsDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetCardsDocuments(IAsyncResult result);
        IAsyncResult BeginSendCardsDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendCardsDocument(IAsyncResult result);
        TextDocument GetCardsDocument(Guid id);
        void SaveCardsDocuments();
        void AddCardsDocument(TextDocument doc);
        void RemoveCardsDocument(TextDocument doc);

        IAsyncResult BeginGetActivitiesDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetActivitiesDocuments(IAsyncResult result);
        IAsyncResult BeginSendActivitiesDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendActivitiesDocument(IAsyncResult result);
        TextDocument GetActivitiesDocument(Guid id);
        void SaveActivitiesDocuments();
        void AddActivitiesDocument(TextDocument doc);
        void RemoveActivitiesDocument(TextDocument doc);

        IAsyncResult BeginGetIdiomsDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetIdiomsDocuments(IAsyncResult result);
        IAsyncResult BeginSendIdiomsDocument(TextDocument email, AsyncCallback callback, object userState);
        void EndSendIdiomsDocument(IAsyncResult result);
        TextDocument GetIdiomsDocument(Guid id);
        void SaveIdiomsDocuments();
        void AddIdiomsDocument(TextDocument doc);
        void RemoveIdiomsDocument(TextDocument doc);
    }
}
