using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MNISTDataReader
{
    public sealed class LabelDataReader : IDataReader<int>, IDisposable
    {
        private readonly FileStream stream;
        private readonly long headerOffset;

        public LabelDataReader(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Не указан файл.");
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Файл не найден.", fileName);
            }

            stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            _ = stream.ReadInt32(); // unused data
            ItemsCount = stream.ReadInt32();
            headerOffset = stream.Position;
        }
        
        public void Dispose() => stream?.Dispose();
        
        public readonly int ItemsCount;

        public IEnumerator<int> GetEnumerator()
        {
            if (ItemsCount == 0)
            {
                yield break;
            }

            SeekByIndex(0);
            for (var i = 0; i < ItemsCount; ++i)
            {
                yield return Read();
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Read(int index)
        {
            SeekByIndex(index);

            return Read();
        }

        IEnumerable<int> IDataReader<int>.GetScope(int begin, int end)
        {
            CheckIndex(end);
            SeekByIndex(begin);

            for (var i = begin; i <= end; ++i)
            {
                yield return Read();
            }
        }

        private int Read() => stream.ReadByte();

        private void CheckIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (index >= ItemsCount)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private void SeekByIndex(int index)
        {
            CheckIndex(index);

            var offset = headerOffset + index;
            if (stream.Position != offset)
            {
                stream.Seek(offset, SeekOrigin.Begin);
            }
        }
    }
}
