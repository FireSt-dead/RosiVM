using RosiCompiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace RosiStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PushParser parser;
        private List<TokenStyle> tokenStyles;
        
        public MainWindow()
        {
            this.parser = new PushParser();
            this.tokenStyles = new List<TokenStyle>()
            {
                // Classes
                new TokenStyle(PushParser.parse + "Name", Brushes.DarkRed, FontWeights.Medium),
                new TokenStyle(PushParser.parse + "Visibility", Brushes.DarkBlue, FontWeights.Normal),

                new TokenStyle(PushParser.parse + "class", Brushes.DarkBlue, FontWeights.Medium),
                new TokenStyle(PushParser.parse + "interface", Brushes.DarkBlue, FontWeights.Medium),
                new TokenStyle(PushParser.parse + "extends", Brushes.DarkBlue, FontWeights.Medium),
                new TokenStyle(PushParser.parse + "implements", Brushes.DarkBlue, FontWeights.Medium),
                new TokenStyle(PushParser.parse + "module", Brushes.DarkBlue, FontWeights.Medium),
                new TokenStyle(PushParser.parse + "method", Brushes.DarkBlue, FontWeights.Medium),
                new TokenStyle(PushParser.parse + "function", Brushes.DarkBlue, FontWeights.Medium),

                new TokenStyle(PushParser.parse + "var", Brushes.DarkBlue, FontWeights.Normal),

                // Operators
                new TokenStyle(PushParser.parse + "if", Brushes.DarkBlue, FontWeights.Normal),
                new TokenStyle(PushParser.parse + "else", Brushes.DarkBlue, FontWeights.Normal),
                
                new TokenStyle(PushParser.parse + "semicolon", Brushes.DarkGray, FontWeights.Normal),
                new TokenStyle(PushParser.parse + "colon", Brushes.DarkGray, FontWeights.Normal),

                new TokenStyle(PushParser.parse + "Identifier", Brushes.DarkCyan, FontWeights.Normal),
            };
            InitializeComponent();
        }

        private bool parsing = false;
        private void CodeView_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (parsing)
            {
                return;
            }
            try
            {

                this.Console.Document.ContentEnd.InsertTextInRun("\n\nChanged!\n");

                Stopwatch stopwatch = Stopwatch.StartNew();

                parsing = true;

                TextRange sourceRange = new TextRange(
                    this.CodeView.Document.ContentStart,
                    this.CodeView.Document.ContentEnd
                );

                this.Console.Document.ContentEnd.InsertTextInRun("Get code: " + stopwatch.Elapsed.ToString() + "\n");
                stopwatch.Restart();

                string text = sourceRange.Text;
                this.parser.Code = text;

                this.Console.Document.ContentEnd.InsertTextInRun("Parsed in: " + stopwatch.Elapsed.ToString() + "\n");
                stopwatch.Restart();

                XmlDataProvider xmlDataProvider = new XmlDataProvider();
                xmlDataProvider.Document = new System.Xml.XmlDocument();
                xmlDataProvider.Document.LoadXml(this.parser.Tree.ToString());
                this.Outline.DataContext = xmlDataProvider;

                this.Console.Document.ContentEnd.InsertTextInRun("Set Outline: " + stopwatch.Elapsed.ToString() + "\n");
                stopwatch.Restart();

                TextRange astRange = new TextRange(
                    this.AstView.Document.ContentStart,
                    this.AstView.Document.ContentEnd
                );
                astRange.Text = this.parser.Tree.ToString();

                this.Console.Document.ContentEnd.InsertTextInRun("Set AST: " + stopwatch.Elapsed.ToString() + "\n");
                stopwatch.Restart();

                // System.Console.WriteLine(text);

                sourceRange.ClearAllProperties();

                this.Console.Document.ContentEnd.InsertTextInRun("Clear formating: " + stopwatch.Elapsed.ToString() + "\n");
                stopwatch.Restart();

                List<Highlight> tags = new List<Highlight>();
                foreach (XElement node in this.parser.Tree.Descendants())
                {
                    var style = this.tokenStyles.FirstOrDefault(s => s.TokenName == node.Name);
                    if (style != null)
                    {
                        int line = int.Parse(node.Attribute("line").Value);
                        int column = int.Parse(node.Attribute("column").Value);
                        int lenght = node.Value.Length;

                        var start = this.CodeView.Document.ContentStart.GetLineStartPosition(0).GetLineStartPosition(line).GetPositionAtOffset(column + 1);
                        var end = start.GetPositionAtOffset(lenght);
                        TextRange classRange = new TextRange(start, end);
                        tags.Add(new Highlight(classRange, style));
                    }
                }

                this.Console.Document.ContentEnd.InsertTextInRun("Calculate highlight tags: " + stopwatch.Elapsed.ToString() + "\n");
                stopwatch.Restart();

                foreach (Highlight tag in tags)
                {
                    tag.Range.ApplyPropertyValue(TextElement.FontWeightProperty, tag.TokenStyle.FontWeight);
                    tag.Range.ApplyPropertyValue(TextElement.ForegroundProperty, tag.TokenStyle.Foreground);
                }

                this.Console.Document.ContentEnd.InsertTextInRun("Highlighted: " + stopwatch.Elapsed.ToString() + "\n");
                stopwatch.Restart();

            }
            finally
            {
                parsing = false;
            }
        }

        public class Highlight
        {
            public Highlight(TextRange range, TokenStyle tokenStyle)
            {
                this.Range = range;
                this.TokenStyle = tokenStyle;
            }

            public TextRange Range;
            public TokenStyle TokenStyle;
        }

        public class TokenStyle
        {
            public TokenStyle(XName tokenName, Brush foreground, FontWeight fontWeight)
            {
                this.TokenName = tokenName;
                this.Foreground = foreground;
                this.FontWeight = fontWeight;
            }

            public XName TokenName;
            public Brush Foreground;
            public FontWeight FontWeight;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.FileOk += (se, ev) =>
            {
                using (var stream = dialog.OpenFile())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var code = reader.ReadToEnd();
                        new TextRange(this.CodeView.Document.ContentStart, this.CodeView.Document.ContentEnd).Text = code;
                    }
                }
            };
            dialog.ShowDialog(this);
        }
    }
}
