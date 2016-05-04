// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace VoiceRecorder.Core
{
    [ModuleExport(typeof(RecordCoreModue))]
    public class RecordCoreModue : IModule
    {
       
        public void Initialize()
        {
        }
    }
}
