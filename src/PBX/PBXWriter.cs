using System.IO;

namespace PBX
{
    class Writer
    {
        StreamWriter sw;
        int indent = 0;
        bool inlineScope = false;

        public Writer(StreamWriter stream)
        {
            sw = stream;
        }

        public void WriteLine(string line)
        {
            sw.WriteLine(line);
        }

        public void WriteLine(string format, params object[] args)
        {
            sw.WriteLine(format, args);
        }
        public void Write(string val)
        {
            sw.Write(val);
        }
        public void Write(string format, params object[] args)
        {
            sw.Write(format, args);
        }

        public void WriteIndent()
        {
            if (inlineScope)
                return;
            for (int i = 0; i < indent; ++i)
            {
                sw.Write('\t');
            }
        }

        public void NewLine()
        {
            if (!inlineScope)
                sw.Write("\n");
            else
                sw.Write(" ");
        }

        public void BeginScope()
        {
            sw.Write("{");
            if (!inlineScope)
                NewLine();
            indent++;
        }

        public void EndScope()
        {
            indent--;
            WriteIndent();
            sw.Write("}");
        }

        public void BeginArray()
        {
            sw.Write("(");
            if (!inlineScope)
                NewLine();
            indent++;
        }

        public void EndArray()
        {
            indent--;
            WriteIndent();
            sw.Write(")");
        }

        public void BeginDict()
        {
            BeginScope();
        }

        public void EndDict()
        {
            EndScope();
        }
        public void SetInline(bool val)
        {
            inlineScope = val;
        }
    }
}
