using System;
using System.Collections.Generic;

namespace PBX
{
    using DictionaryList = List<KeyValuePair<string, dynamic>>;

    partial class File
    {
        DictionaryList parse(Reader sr)
        {
            while (!sr.EndOfStream)
            {
                var c = sr.Peek();
                if (c == '{')
                    break;
                sr.Skip();
            }

            return parseDictionary(sr);
        }

        DictionaryList parseDictionary(Reader sr)
        {
            var res = new List<KeyValuePair<string, dynamic>>();

            var cur = sr.Read();
            if (cur != '{')
                throw new Exception("Dictionary must start with {");

            skipPadding(sr);

            while (!sr.EndOfStream)
            {
                cur = sr.Peek();
                if (cur == '}')
                {
                    sr.Skip();
                    break;
                }

                parseDictionary(sr, res);
                skipPadding(sr);
            }

            return res;
        }

        string parseKey(Reader sr)
        {
            skipPadding(sr);

            string res = "";
            while (!sr.EndOfStream)
            {
                var c = sr.Peek();
                if (isWhitespace(c) || c == ';')
                {
                    break;
                }
                sr.Skip();
                res += c;
            }

            skipPadding(sr);

            if (res.Length > 0 && res.StartsWith("\""))
                res = res.Substring(1);

            if (res.Length > 0 && res.EndsWith("\""))
                res = res.Substring(0, res.Length - 1);

            return res;
        }

        string parseLiteral(Reader sr)
        {
            skipPadding(sr);

            string res = "";
            var c = sr.Peek();
            var prev = c;
            if (c == '"')
            {
                sr.Skip();
                while (!sr.EndOfStream)
                {
                    c = sr.Read();
                    if (c == '"')
                    {
                        if (prev == '\\')
                            res += c;
                        else
                            break;
                    }
                    else
                    {
                        res += c;
                    }
                    prev = c;
                }
            }
            else
            {
                while (!sr.EndOfStream)
                {
                    c = sr.Peek();
                    if (isWhitespace(c) || c == ';' || c == ',')
                        break;
                    res += c;
                    sr.Skip();
                }
            }

            skipPadding(sr);

            res = res.Replace("\\\"", "\"");

            return res;
        }

        dynamic parseValue(Reader sr)
        {
            var c = sr.Peek();
            if (c == '{')
                return parseDictionary(sr);
            else if (c == '(')
                return parseArray(sr);
            return parseLiteral(sr);
        }

        void parseDictionary(Reader sr, DictionaryList res)
        {
            var key = parseKey(sr);

            var c = sr.Read();
            if (c != '=')
                throw new Exception("Expected assignment");

            skipPadding(sr);

            dynamic val = parseValue(sr);

            c = sr.Peek();
            if (c != ';')
                throw new Exception("Expected semicolon after dictionary");
            sr.Skip();

            res.Add(new KeyValuePair<string, dynamic>(key, val));
        }

        void parseArray(Reader sr, List<dynamic> res)
        {
            var val = parseValue(sr);

            var c = sr.Peek();
            if (c != ',')
                throw new Exception("Expected comma after value");
            sr.Skip();

            res.Add(val);
        }

        List<dynamic> parseArray(Reader sr)
        {
            var res = new List<dynamic>();

            var c = sr.Peek();
            if (c != '(')
                throw new Exception("Array must start with (");

            sr.Read();
            skipPadding(sr);

            while (!sr.EndOfStream)
            {
                c = sr.Peek();
                if (c == ')')
                {
                    sr.Skip();
                    break;
                }

                parseArray(sr, res);
                skipPadding(sr);
            }

            return res;
        }

        bool isWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }

        void skipWhitespace(Reader sr)
        {
            while (!sr.EndOfStream)
            {
                var cur = sr.Peek();
                if (!isWhitespace(cur))
                    break;
                sr.Skip();
            }
        }

        void skipComment(Reader sr)
        {
            var offset = sr.Position;

            var a = sr.Read();
            var b = sr.Read();

            if (a != '/' || b != '*')
            {
                sr.Position = offset;
                return;
            }

            while (!sr.EndOfStream)
            {
                a = sr.Read();
                if (a == '*')
                {
                    b = sr.Peek();
                    if (b == '/')
                    {
                        sr.Skip();
                        return;
                    }
                }
            }

            sr.Position = offset;
        }

        void skipPadding(Reader sr)
        {
            skipWhitespace(sr);
            skipComment(sr);
            skipWhitespace(sr);
        }
    }
}
