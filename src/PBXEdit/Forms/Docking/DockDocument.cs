using DarkUI.Forms;
using ScintillaNET;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PBXEdit
{
    public partial class DockDocument : DarkUI.Docking.DarkDocument
    {
        public string File { get; set; }
        string InitialContent;
        bool modified;
        Encoding FileEncoding;

        static readonly Color BACK_COLOR = Color.FromArgb(255, 40, 40, 40);
        static readonly Color FORE_COLOR = Color.FromArgb(255, 220, 220, 220);

        public bool Modified { get => modified; }

        public DockDocument()
        {
            InitializeComponent();
            InitSyntaxColoring();
            InitNumberMargin();

            EventHandler eh = new EventHandler(OnSizeChanged);
            TextBox.TextChanged += eh;
            TextBox.ClientSizeChanged += eh;
            TextBox.InsertCheck += OnInsertCheck;
            TextBox.CharAdded += OnCharAdded;
            TextBox.TextChanged += OnTextChanged;
            TextBox.KeyDown += OnKeyDown;

            TextBox.IndentationGuides = IndentView.LookBoth;
            TextBox.WrapIndentMode = WrapIndentMode.Same;
            TextBox.IndentWidth = 4;
        }

        public DockDocument(string file, Image icon) : this()
        {
            File = file;
            Icon = icon;
            DockText = GetFileName(File);
            ReloadFileContents();
            ClearModificationState();
        }

        static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0)
                return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return new UTF32Encoding(true, true);  //UTF-32BE

            return Encoding.ASCII;
        }

        void MarkStateModified()
        {
            if (TextBox.Text == InitialContent)
            {
                ClearModificationState();
                return;
            }

            modified = true;
            DockText = "* " + GetFileName(File);
        }

        void ClearModificationState()
        {
            modified = false;
            DockText = GetFileName(File);
            InitialContent = TextBox.Text;
        }

        void InitSyntaxColoring()
        {
            // Configure the default style
            TextBox.StyleResetDefault();
            TextBox.Styles[Style.Default].Font = "Lucida Console";
            TextBox.Styles[Style.Default].Size = 9;
            TextBox.Styles[Style.Default].BackColor = BACK_COLOR;
            TextBox.Styles[Style.Default].ForeColor = Color.FromArgb(255, 220, 220, 220);
            TextBox.CaretForeColor = FORE_COLOR;
            TextBox.StyleClearAll();
            TextBox.BorderStyle = BorderStyle.None;

            // Configure the CPP (C#) lexer styles
            TextBox.Styles[Style.Cpp.Identifier].ForeColor = Color.FromArgb(255, 189, 183, 107);
            TextBox.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(255, 87, 166, 74);
            TextBox.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            TextBox.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            TextBox.Styles[Style.Cpp.Number].ForeColor = Color.FromArgb(255, 181, 206, 168);
            TextBox.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(255, 214, 157, 133);
            TextBox.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            TextBox.Styles[Style.Cpp.Preprocessor].ForeColor = Color.FromArgb(255, 86, 156, 214);
            TextBox.Styles[Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
            TextBox.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
            TextBox.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            TextBox.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
            TextBox.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
            TextBox.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            TextBox.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            TextBox.Styles[Style.Cpp.GlobalClass].ForeColor = Color.FromArgb(255, 255, 215, 0);

            TextBox.Lexer = Lexer.Cpp;

            // Cpp.Word
            TextBox.SetKeywords(0, "auto void class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield");
            // Cpp.Word2
            TextBox.SetKeywords(1, "Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Windows Forms ScintillaNET");
            // Cpp.CommentDocKeyword
            TextBox.SetKeywords(2, "");
            // Cpp.GlobalClass
            TextBox.SetKeywords(3, "uint8_t int8_t uint16_t int16_t uint32_t int32_t uint64_t int64_t string vector map unordered_map set deque uintptr_t intptr_t");
            // Cpp.Preprocessor
            TextBox.SetKeywords(4, "include");
        }

        private void InitNumberMargin()
        {
            TextBox.Styles[Style.LineNumber].BackColor = Color.FromArgb(255, 50, 53, 55);
            TextBox.Styles[Style.LineNumber].ForeColor = Color.FromArgb(255, 43, 145, 175);

            TextBox.Styles[Style.IndentGuide].ForeColor = FORE_COLOR;
            TextBox.Styles[Style.IndentGuide].BackColor = Color.FromArgb(255, 50, 53, 55);

            var nums = TextBox.Margins[1];
            nums.Width = 40;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            var nums2 = TextBox.Margins[2];
            nums2.Width = 4;
            nums2.Type = MarginType.Text;
            nums2.Sensitive = true;
            nums2.Mask = 0;
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            var tb = sender as ScintillaNET.Scintilla;

            Size tS = TextRenderer.MeasureText(tb.Text, tb.Font);

            var totalMargin = 0;
            foreach (var m in tb.Margins)
            {
                totalMargin += m.Width;
            }

            // FIXME: Need to be calculated correctly using points.
            tb.VScrollBar = (tS.Height + 200) >= tb.Height;
            tb.HScrollBar = (tS.Width + totalMargin + 200) >= tb.Width;
        }

        string GetFileName(string file)
        {
            return System.IO.Path.GetFileName(file);
        }

        void ReloadFileContents()
        {
            if (File == null || File.Length == 0)
                return;

            try
            {
                FileEncoding = GetEncoding(File);
                InitialContent = System.IO.File.ReadAllText(File, FileEncoding);
                TextBox.Text = InitialContent;
            }
            catch (System.Exception)
            {
            }
        }

        void OnInsertCheck(object sender, InsertCheckEventArgs e)
        {
            if (e.Text.EndsWith("\n") || e.Text.EndsWith("\r\n"))
            {
                int startPos = TextBox.Lines[TextBox.LineFromPosition(TextBox.CurrentPosition)].Position;
                int endPos = e.Position;
                string curLineText = TextBox.GetTextRange(startPos, (endPos - startPos));

                Match indent = Regex.Match(curLineText, "^[ \\t]*");
                e.Text = (e.Text + indent.Value);
                if (Regex.IsMatch(curLineText, "{\\s*$"))
                {
                    e.Text = (e.Text + "\t");
                }
            }
        }

        void OnCharAdded(object sender, CharAddedEventArgs e)
        {
            if (e.Char == 125 /* '}' */)
            {
                int curLine = TextBox.LineFromPosition(TextBox.CurrentPosition);

                if (TextBox.Lines[curLine].Text.Trim() == "}")
                {
                    SetIndent(TextBox, curLine, GetIndent(TextBox, curLine) - 4);
                }
            }
        }

        public void SaveFile()
        {
            try
            {
                System.IO.File.WriteAllText(File, TextBox.Text, FileEncoding);
                ClearModificationState();
            }
            catch (Exception)
            {
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                SaveFile();
                e.SuppressKeyPress = true;
            }
        }

        const int SCI_SETLINEINDENTATION = 2126;
        const int SCI_GETLINEINDENTATION = 2127;

        private void SetIndent(ScintillaNET.Scintilla scin, int line, int indent)
        {
            scin.DirectMessage(SCI_SETLINEINDENTATION, new IntPtr(line), new IntPtr(indent));
        }

        private int GetIndent(ScintillaNET.Scintilla scin, int line)
        {
            return (scin.DirectMessage(SCI_GETLINEINDENTATION, new IntPtr(line), IntPtr.Zero).ToInt32());
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            MarkStateModified();
        }

        public override void Close()
        {
            if (modified)
            {
                var result = DarkMessageBox.ShowWarning("You have unsaved changes, save before closing?", "Close", DarkDialogButton.YesNoCancel);
                if (result == DialogResult.Cancel)
                    return;

                if (result == DialogResult.Yes)
                {
                    System.IO.File.WriteAllText(File, TextBox.Text, FileEncoding);
                }
            }

            base.Close();
        }
    }
}
