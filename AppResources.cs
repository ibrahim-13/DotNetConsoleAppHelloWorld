using System;
using System.Drawing;
using System.IO;

namespace ConsoleAppHelloWorld
{
    public static class AppResources
    {
        private static readonly string ResourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources\\");
        private static readonly object LockObject = new ();
        private static byte[]? _serviceLogData;
        private static Bitmap? _checkboxChecked;
        private static Bitmap? _checkboxUnchecked;
        public static byte[] ServiceLogSample
        {
            get
            {
                if (_serviceLogData != null) return _serviceLogData;
                lock (LockObject)
                {
                    _serviceLogData = GetFileByName("ServiceLogSample.xlsx");
                }
                return _serviceLogData;
            }
        }

        public static Bitmap CheckboxChecked
        {
            get
            {
                if (_checkboxChecked != null) return _checkboxChecked;
                lock (LockObject)
                {
                    _checkboxChecked = GetBitmapByName("CheckboxChecked.jpg");
                }
                return _checkboxChecked;
            }
        }

        public static Bitmap CheckboxUnchecked
        {
            get
            {
                if (_checkboxUnchecked != null) return _checkboxUnchecked;
                lock (LockObject)
                {
                    _checkboxUnchecked = GetBitmapByName("CheckboxUnchecked.jpg");
                }
                return _checkboxUnchecked;
            }
        }

        private static byte[] GetFileByName(string name)
        {
            try
            {
                return File.ReadAllBytes(Path.Combine(ResourcePath, name));
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        private static Bitmap GetBitmapByName(string name)
        {
            var bytes = GetFileByName(name);
            using MemoryStream ms = new(bytes);
            var img = Image.FromStream(ms);
            return new Bitmap(img);
        }
    }
}
