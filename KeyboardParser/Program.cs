using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using KeyboardParser.Model;

namespace KeyboardParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("keyboards.xml");

            var sb = new StringBuilder();
            var sbDisplay = new StringBuilder();
            var sbCodes = new StringBuilder();

            var arrayPrefix = string.Empty;

            foreach (XmlNode language in doc.DocumentElement.ChildNodes)
            {
                var rowNumber = 0;

                sbDisplay.AppendFormat("{0}\"{1}\"", arrayPrefix, language.Attributes["name"].Value);
                sbCodes.AppendFormat("{0}\"{1}\"", arrayPrefix, language.Attributes["apiCode"].Value);
                arrayPrefix = ",";

                sb.AppendFormat("case \"{0}\":" + Environment.NewLine, language.Attributes["name"].Value);
                sb.AppendFormat("self.spaceLabel = \"{0}\"" + Environment.NewLine, language.Attributes["spaceLabel"].Value);
                sb.AppendFormat("self.returnLabel = \"{0}\"\n" + Environment.NewLine, language.Attributes["returnLabel"].Value);
                sb.AppendLine();

                foreach (XmlNode row in language.ChildNodes)
                {


                    if (!row.HasChildNodes)
                    {
                        continue;
                    }

                    var prefix = "";
                    var hasSpecialKeyStatement = false;
                    var kb = new StringBuilder();

                    if (rowNumber == 2)
                    {
                        kb.AppendFormat("var keyModel = Key(.Shift){0}", Environment.NewLine);
                        kb.AppendFormat("keyModel.keyCap = \"\"{0}", Environment.NewLine);
                        kb.AppendFormat("self.addKey(keyModel, row: 2, page: 0){0}", Environment.NewLine);
                        kb.AppendLine();
                    }

                    kb.Append("for key in [");

                    var skb = new StringBuilder();
                    skb.Append("switch key {" + Environment.NewLine);

                    foreach (XmlNode key in row.ChildNodes)
                    {
                        kb.AppendFormat("{0}\"{1}\"", prefix, key.Attributes["value"].Value);
                        prefix = ",";

                        if (!key.HasChildNodes)
                        {
                            continue;
                        }

                        hasSpecialKeyStatement = true;
                        skb.AppendFormat("case \"{0}\":{1}", key.Attributes["value"].Value, Environment.NewLine);

                        var acb = new StringBuilder();
                        var acbPrefix = "";

                        foreach (XmlNode altChar in key.ChildNodes)
                        {
                            acb.AppendFormat("{0}\"{1}\"", acbPrefix, altChar.Attributes["value"].Value);
                            acbPrefix = ",";
                        }

                        skb.AppendFormat("keyModel.altChars = [{0}]{1}", acb.ToString(), Environment.NewLine);
                    }

                    skb.AppendFormat("default: break{0}", Environment.NewLine);
                    skb.Append("}");
                    skb.AppendLine();

                    kb.Append("] {" + Environment.NewLine);
                    kb.Append("var keyModel = Key(.Character)" + Environment.NewLine);
                    kb.Append("keyModel.keyCap = key" + Environment.NewLine);
                    kb.Append("keyModel.outputText = key" + Environment.NewLine);
                    kb.AppendLine();

                    if (hasSpecialKeyStatement)
                    {
                        kb.AppendFormat("{0}{1}", skb.ToString(), Environment.NewLine);
                        kb.AppendLine();
                    }

                    kb.AppendFormat("self.addKey(keyModel, row: {0}, page: 0){1}", rowNumber, Environment.NewLine);
                    kb.Append("}");
                    kb.AppendLine();

                    if (rowNumber == 2)
                    {
                        kb.AppendFormat("var keyModel2 = Key(.Backspace){0}", Environment.NewLine);
                        kb.AppendFormat("keyModel2.keyCap = \"\"{0}", Environment.NewLine);
                        kb.AppendFormat("self.addKey(keyModel2, row: 2, page: 0){0}", Environment.NewLine);
                    }

                    sb.Append(kb.ToString());
                    rowNumber++;
                }

                sb.AppendLine();
            }

            // writes text to file
            System.IO.File.WriteAllText(@"struct.txt", sb.ToString());
            System.IO.File.WriteAllText(@"display.txt", sbDisplay.ToString());
            System.IO.File.WriteAllText(@"codes.txt", sbCodes.ToString());
        }
    }
}
