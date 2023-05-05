using System;
using System.IO;

namespace MNISTDataReader
{
    internal static class ExtensionMethods
    {
        private const int SizeOfInt32 = sizeof(int);

        public static int ReadInt32(this Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Поток не поддерживает чтение.");
            }

            var bytes = new byte[SizeOfInt32];

            if (stream.Read(bytes, 0, SizeOfInt32) != SizeOfInt32)
            {
                throw new FileLoadException("Не удалось выполнить операцию чтения.");
            }

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}