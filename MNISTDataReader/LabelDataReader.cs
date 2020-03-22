using System;
using System.Collections.Generic;
using System.IO;

namespace MNISTDataReader
{
    public partial class LabelDataReader : IDataReader<int>, IDisposable
    {
        public readonly int ItemsCount;

        const int HeaderOffset = 8;
        const int MagicNumberOffset = 4;

        FileStream stream;

        int currentIteratorIndex = 0;

        public LabelDataReader(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Не указан файл");

            if (!File.Exists(fileName))
                throw new FileNotFoundException("Файл не найден", fileName);

            stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            stream.Seek(MagicNumberOffset, SeekOrigin.Begin);

            ItemsCount = stream.ReadInt32();
        }

        public IEnumerator<int> GetEnumerator()
        {
            if (currentIteratorIndex == ItemsCount)
            {
                currentIteratorIndex = 0;

                yield break;
            }

            ++currentIteratorIndex;

            yield return read();
        }

        public int Read(int index)
        {
            seekByIndex(index);

            return read();
        }

        public IEnumerable<int> GetScope(int begin, int end)
        {
            seekByIndex(begin);

            checkIndex(end);

            for (int i = begin; i <= end; i++)
                yield return read();
        }

        public void Dispose()
        {
            stream?.Dispose();
            stream = null;
        }

        int read()
        {
            return stream.ReadByte();
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

            var offset = HeaderOffset + index;

            if (stream.Position != offset)
                stream.Seek(offset, SeekOrigin.Begin);
        }
    }
}
