using System;
using System.IO;

namespace MNISTDataReader
{
    internal static class ExstensionMethods
    {
        const int SizeOfInt32 = 4;

        public static int ToInt32(this byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();

            if (bytes.Length != SizeOfInt32)
                throw new InvalidOperationException("Количество байтов не соответствует размеру типа System.Int32");

            var result = 0;

            result += (int)bytes[0] << 24;
            result += (int)bytes[1] << 16;
            result += (int)bytes[2] << 8;
            result += (int)bytes[3];

            return result;
        }

        public static int ReadInt32(this Stream stream)
        {
            if (!stream.CanRead)
                throw new InvalidOperationException("Поток не поддерживает чтение");

            var bytes = new byte[SizeOfInt32];

            if (stream.Read(bytes, 0, SizeOfInt32) != SizeOfInt32)
                throw new FileLoadException("Не удалось выполнить операцию чтения");

            return bytes.ToInt32();
        }
    }
}
