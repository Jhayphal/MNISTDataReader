using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MNISTDataReader
{
    public sealed class ImageDataReader : IDataReader<byte[]>, IDisposable
    {
        private readonly FileStream stream;
        private readonly long headerOffset;

        public ImageDataReader(string fileName)
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
            Rows = stream.ReadInt32();
            Columns = stream.ReadInt32();
            headerOffset = stream.Position;
        }
        
        public void Dispose()
        {
            stream?.Dispose();
        }

        public readonly int ItemsCount;
        
        public readonly int Columns;
        
        public readonly int Rows;
        
        public int ImageBytesCount => Columns * Rows;

        public byte[] Read(int index)
        {
            SeekByIndex(index);

            return Read();
        }

        public IEnumerator<byte[]> GetEnumerator()
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

        public IEnumerable<byte[]> GetScope(int begin, int end)
        {
            CheckIndex(end);
            SeekByIndex(begin);

            for (var i = begin; i <= end; ++i)
            {
                yield return Read();
            }
        }

        private byte[] Read()
        {
            var bytes = new byte[ImageBytesCount];

            if (stream.Read(bytes, 0, ImageBytesCount) != ImageBytesCount)
            {
                throw new FileLoadException("Достигнут конец потока.");
            }

            return bytes;
        }

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

            var offset = headerOffset + ImageBytesCount * index;
            if (stream.Position != offset)
            {
                stream.Seek(offset, SeekOrigin.Begin);
            }
        }
    }
}
