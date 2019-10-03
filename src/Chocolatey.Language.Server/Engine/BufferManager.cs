using System;
using System.Collections.Concurrent;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Buffer = Microsoft.Language.Xml.Buffer;

namespace Chocolatey.Language.Server.Engine
{
    public class BufferManager
    {
        public EventHandler<DocumentUpdatedEventArgs> BufferUpdated;

        public BufferManager()
        {
        }

        private ConcurrentDictionary<Uri, Buffer> _buffers = new ConcurrentDictionary<Uri, Buffer>();

        public void UpdateBuffer(Uri uri, Buffer buffer)
        {
            _buffers.AddOrUpdate(uri, buffer, (k, v) => buffer);

            BufferUpdated?.Invoke(this, new DocumentUpdatedEventArgs(uri));
        }

        public Buffer GetBuffer(Uri uri)
        {
            return _buffers.TryGetValue(uri, out var buffer) ? buffer : null;
        }
    }

    public class DocumentUpdatedEventArgs : EventArgs
    {
        public Uri Uri { get; }

        public DocumentUpdatedEventArgs(Uri uri)
        {
            Uri = uri;
        }
    }
}
