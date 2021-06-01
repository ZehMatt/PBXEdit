using System.IO;

namespace PBX
{
    internal class Reader
    {
        private string data;
        public int Position { get; set; }
        public bool EndOfStream { get { return Position >= data.Length; } }

        public Reader(StreamReader sr)
        {
            data = sr.ReadToEnd();
        }

        public char Read()
        {
            return data[Position++];
        }

        public void Skip(int count = 1)
        {
            Position += count;
            if (Position < 0)
            {
                Position = 0;
            }

            if (Position > data.Length)
            {
                Position = data.Length;
            }
        }

        public char Peek()
        {
            return data[Position];
        }
    }
}
