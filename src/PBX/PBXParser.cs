using System;
using System.Collections.Generic;

namespace PBX
{
    using DictionaryList = List<KeyValuePair<string, dynamic>>;

    public partial class File
    {
        private DictionaryList Parse(Reader sr)
        {
            while (!sr.EndOfStream)
            {
                var c = sr.Peek();
                if (c == '{')
                {
                    break;
                }

                sr.Skip();
            }

            return ParseDictionary(sr);
        }

        private DictionaryList ParseDictionary(Reader sr)
        {
            var res = new List<KeyValuePair<string, dynamic>>();

            var cur = sr.Read();
            if (cur != '{')
            {
                throw new Exception("Dictionary must start with {");
            }

            SkipPadding(sr);

            while (!sr.EndOfStream)
            {
                cur = sr.Peek();
                if (cur == '}')
                {
                    sr.Skip();
                    break;
                }

                ParseDictionary(sr, res);
                SkipPadding(sr);
            }

            return res;
        }

        private string ParseKey(Reader sr)
        {
            SkipPadding(sr);

            string res = "";
            while (!sr.EndOfStream)
            {
                var c = sr.Peek();
                if (IsWhitespace(c) || c == ';')
                {
                    break;
                }
                sr.Skip();
                res += c;
            }

            SkipPadding(sr);

            if (res.Length > 0 && res.StartsWith("\""))
            {
                res = res.Substring(1);
            }

            if (res.Length > 0 && res.EndsWith("\""))
            {
                res = res.Substring(0, res.Length - 1);
            }

            return res;
        }

        private string ParseLiteral(Reader sr)
        {
            SkipPadding(sr);

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
                        {
                            res += c;
                        }
                        else
                        {
                            break;
                        }
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
                    if (IsWhitespace(c) || c == ';' || c == ',')
                    {
                        break;
                    }

                    res += c;
                    sr.Skip();
                }
            }

            SkipPadding(sr);

            res = res.Replace("\\\"", "\"");

            return res;
        }

        private dynamic ParseValue(Reader sr)
        {
            var c = sr.Peek();
            if (c == '{')
            {
                return ParseDictionary(sr);
            }
            else if (c == '(')
            {
                return ParseArray(sr);
            }

            return ParseLiteral(sr);
        }

        private void ParseDictionary(Reader sr, DictionaryList res)
        {
            var key = ParseKey(sr);

            var c = sr.Read();
            if (c != '=')
            {
                throw new Exception("Expected assignment");
            }

            SkipPadding(sr);

            dynamic val = ParseValue(sr);

            c = sr.Peek();
            if (c != ';')
            {
                throw new Exception("Expected semicolon after dictionary");
            }

            sr.Skip();

            res.Add(new KeyValuePair<string, dynamic>(key, val));
        }

        private void ParseArray(Reader sr, List<dynamic> res)
        {
            var val = ParseValue(sr);

            var c = sr.Peek();
            if (c != ',')
            {
                throw new Exception("Expected comma after value");
            }

            sr.Skip();

            res.Add(val);
        }

        private List<dynamic> ParseArray(Reader sr)
        {
            var res = new List<dynamic>();

            var c = sr.Peek();
            if (c != '(')
            {
                throw new Exception("Array must start with (");
            }

            sr.Read();
            SkipPadding(sr);

            while (!sr.EndOfStream)
            {
                c = sr.Peek();
                if (c == ')')
                {
                    sr.Skip();
                    break;
                }

                ParseArray(sr, res);
                SkipPadding(sr);
            }

            return res;
        }

        private bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }

        private void SkipWhitespace(Reader sr)
        {
            while (!sr.EndOfStream)
            {
                var cur = sr.Peek();
                if (!IsWhitespace(cur))
                {
                    break;
                }

                sr.Skip();
            }
        }

        private void SkipComment(Reader sr)
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

        private void SkipPadding(Reader sr)
        {
            SkipWhitespace(sr);
            SkipComment(sr);
            SkipWhitespace(sr);
        }
    }
}
