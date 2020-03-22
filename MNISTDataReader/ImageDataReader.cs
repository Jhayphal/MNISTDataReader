using System;
using System.Collections.Generic;
using System.IO;

namespace MNISTDataReader
{
    public partial class ImageDataReader : IDataReader<byte[]>, IDisposable
    {
        public readonly int ItemsCount;
        public readonly int NumberOfColumns;
        public readonly int NumberOfRows;

        const int HeaderOffset = 16;
        const int MagicNumberOffset = 4;

        FileStream stream;
        readonly int imageBytesCount;

        public ImageDataReader(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Не указан файл");

            if (!File.Exists(fileName))
                throw new FileNotFoundException("Файл не найден", fileName);

            stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            stream.Seek(MagicNumberOffset, SeekOrigin.Begin);

            ItemsCount = stream.ReadInt32();
            NumberOfRows = stream.ReadInt32();
            NumberOfColumns = stream.ReadInt32();

            imageBytesCount = NumberOfColumns * NumberOfRows;
        }

        public byte[] Read(int index)
        {
            seekByIndex(index);

            return read();
        }

        public IEnumerator<byte[]> GetEnumerator()
        {
            var count = ItemsCount;

            for (int i = 0; i < count; ++i)
                yield return read();
        }

        public IEnumerable<byte[]> GetScope(int begin, int end)
        {
            seekByIndex(begin);

            checkIndex(end);

            for (int i = begin; i <= end; ++i)
                yield return read();
        }

        public void Dispose()
        {
            stream?.Dispose();
            stream = null;
        }

        byte[] read()
        {
            var bytes = new byte[imageBytesCount];

            stream.Read(bytes, 0, imageBytesCount);

            return bytes;
        }

        void checkIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException();

            if (index >= ItemsCount)
                throw new ArgumentOutOfRangeException();
        }

        void seekByIndex(int index)
        {
            checkIndex(index);

            var offset = HeaderOffset + imageBytesCount * index;

            if (stream.Position != offset)
                stream.Seek(offset, SeekOrigin.Begin);
        }
    }
}
