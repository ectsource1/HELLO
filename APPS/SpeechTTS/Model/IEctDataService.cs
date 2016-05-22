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
    public interface IEctDataService
    {
        int ProjIdx(TextDocument doc);
        IAsyncResult BeginGetProjDocuments(AsyncCallback callback, object userState);
        IEnumerable<TextDocument> EndGetProjDocuments(IAsyncResult result);
        IAsyncResult BeginSendProjDocument(TextDocument page, AsyncCallback callback, object userState);
        void EndSendProjDocument(IAsyncResult result);
        EctModule GetProjDocument(Guid id);
        void SaveProjDocuments();
        void AddProjDocument(TextDocument doc);
        void RemoveProjDocument(TextDocument doc);
    }
}
