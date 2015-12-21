using System;
using System.Collections.Generic;
using System.Text;

namespace Imaging.net.Internal
{
    /// <summary>
    /// And temp file deleter utility, from https://github.com/danielgindi/dg.Utilities
    /// </summary>
    internal class TemporaryFileDeleter : IDisposable
    {
        private string _LocalFilePath;

        public string LocalFilePath
        {
            get { return _LocalFilePath; }
        }
        public TemporaryFileDeleter(string localFilePath)
        {
            _LocalFilePath = localFilePath;
        }

        ~TemporaryFileDeleter()
        {
            Dispose(false);
        }

        public void DeleteFile()
        {
            string path = _LocalFilePath;
            _LocalFilePath = null;
            if (path == null) return;
            System.IO.File.Delete(path);
        }
        public void DoNotDelete()
        {
            _LocalFilePath = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            // Now clean up Native Resources (Pointers)
            try
            {
                DeleteFile();
            }
            catch { }
        }
    }
}
