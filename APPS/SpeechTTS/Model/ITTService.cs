// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
    }
}
