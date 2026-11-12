using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace EditorTool
{
    public class HtmlParser
    {
        private string _html;
        private int _position;
        private string _doctype;

        public HtmlParser(string html)
        {
            _html = html;
            _position = 0;
        }

        public HtmlNode Parse()
        {
            // Handle <!DOCTYPE html>
            HandleDoctype();

            return ParseNode();
        }

        private void HandleDoctype()
        {
            SkipWhitespace();

            if (_position < _html.Length && _html.Substring(_position).StartsWith("<!DOCTYPE"))
            {
                var doctypeMatch = Regex.Match(_html.Substring(_position), @"^<!DOCTYPE\s+[^>]+>");
                if (doctypeMatch.Success)
                {
                    _doctype = doctypeMatch.Value;
                    _position += doctypeMatch.Length;
                }
            }

            SkipWhitespace();
        }


        private HtmlNode ParseNode()
        {
            SkipWhitespace();

            if (_position >= _html.Length || _html[_position] != '<')
            {
                return null;
            }

            var startTagMatch = Regex.Match(_html.Substring(_position), @"^<(\w+)([^>]*?)>", RegexOptions.Singleline);
            if (!startTagMatch.Success)
            {
                return null;
            }

            var node = new HtmlNode
            {
                TagName = startTagMatch.Groups[1].Value
            };

            // Updated regex to handle attributes with and without values
            var attributes = startTagMatch.Groups[2].Value;
            var attrMatches = Regex.Matches(attributes, @"(\w[-\w]*)(?:=""(.*?)"")?", RegexOptions.Singleline);
            foreach (Match match in attrMatches)
            {
                if (match.Groups[2].Success)
                {
                    node.Attributes.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
                else
                {
                    // For boolean attributes
                    node.Attributes.Add(match.Groups[1].Value, null);
                }
            }

            _position += startTagMatch.Length;

            if (_html[_position - 1] == '/' || IsSelfClosingTag(node.TagName))
            {
                return node;
            }

            // Special handling for script tags
            if (node.TagName.ToLower() == "script")
            {
                var scriptEnd = _html.IndexOf($"</{node.TagName}>", _position, StringComparison.OrdinalIgnoreCase);
                if (scriptEnd == -1)
                {
                    scriptEnd = _html.Length;
                }

                node.InnerText = _html.Substring(_position, scriptEnd - _position);
                _position = scriptEnd + node.TagName.Length + 3; // Move past </script>
                return node;
            }

            while (_position < _html.Length && !_html.Substring(_position).StartsWith($"</{node.TagName}>"))
            {
                SkipWhitespace();

                if (_html[_position] == '<' && _html[_position + 1] != '/')
                {
                    var child = ParseNode();
                    if (child != null)
                    {
                        node.Children.Add(child);
                    }
                }
                else
                {
                    var innerTextEnd = _html.IndexOf('<', _position);
                    if (innerTextEnd == -1)
                    {
                        innerTextEnd = _html.Length;
                    }

                    node.InnerText += _html.Substring(_position, innerTextEnd - _position);
                    _position = innerTextEnd;
                }
            }

            var endTagMatch = Regex.Match(_html.Substring(_position), $@"^</{node.TagName}>", RegexOptions.Singleline);
            if (endTagMatch.Success)
            {
                _position += endTagMatch.Length;
            }

            return node;
        }

        private void SkipWhitespace()
        {
            while (_position < _html.Length && char.IsWhiteSpace(_html[_position]))
            {
                _position++;
            }
        }

        public static bool IsSelfClosingTag(string tagName)
        {
            var selfClosingTags = new HashSet<string>
                { "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr", "path" };
            return selfClosingTags.Contains(tagName);
        }
    }

    public class HtmlNode
    {
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<HtmlNode> Children { get; set; } = new List<HtmlNode>();
        public string InnerText { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"<{TagName}>";
        }

        public static HtmlNode Create(string tagName, Dictionary<string, string> attributes = null, string innerText = "")
        {
            return new HtmlNode
            {
                TagName = tagName,
                Attributes = attributes ?? new Dictionary<string, string>(),
                InnerText = innerText
            };
        }

        public HtmlNode FindNode(HtmlNode currentNode, string tagName, string innerText)
        {
            return FindNodeRecursive(currentNode, tagName, innerText);
        }

        private HtmlNode FindNodeRecursive(HtmlNode currentNode, string tagName, string innerText)
        {
            if (currentNode.TagName == tagName && currentNode.InnerText.Contains(innerText))
            {
                return currentNode;
            }

            foreach (var child in currentNode.Children)
            {
                var findNodeRecursive = FindNodeRecursive(child, tagName, innerText);
                if (findNodeRecursive != null)
                {
                    return findNodeRecursive;
                }
            }

            return null;
        }


        public void InsertNode(HtmlNode rootNode, string tagName, HtmlNode node)
        {
            if (rootNode != null)
            {
                InsertNodeRecursive(rootNode, tagName, node);
            }
        }

        private bool InsertNodeRecursive(HtmlNode currentNode, string tagName, HtmlNode nodeToInsert)
        {
            if (currentNode.TagName == tagName)
            {
                currentNode.Children.Add(nodeToInsert);
                return true;
            }

            foreach (var child in currentNode.Children)
            {
                if (InsertNodeRecursive(child, tagName, nodeToInsert))
                {
                    return true;
                }
            }

            return false;
        }


        public bool DeleteNode(HtmlNode currentNode, string tagName, string innerText, HtmlNode parentNode = null)
        {
            return DeleteNodeRecursive(currentNode, tagName, innerText, parentNode);
        }

        private bool DeleteNodeRecursive(HtmlNode currentNode, string tagName, string innerText, HtmlNode parentNode = null)
        {
            if (currentNode.TagName == tagName && currentNode.InnerText.Contains(innerText))
            {
                if (parentNode != null)
                {
                    parentNode.Children.Remove(currentNode);
                }

                return true;
            }

            foreach (var child in currentNode.Children)
            {
                if (DeleteNodeRecursive(child, tagName, innerText, currentNode))
                {
                    return true;
                }
            }

            return false;
        }

        public bool DeleteNodeWithAttribute(HtmlNode currentNode, string tagName, Dictionary<string, string> attributes, HtmlNode parentNode = null)
        {
            return DeleteNodeWithAttributeRecursive(currentNode, tagName, attributes, parentNode);
        }

        private bool DeleteNodeWithAttributeRecursive(HtmlNode currentNode, string tagName, Dictionary<string, string> attributes, HtmlNode parentNode = null)
        {
            if (currentNode.TagName == tagName)
            {
                foreach (var (key, value) in attributes)
                {
                    if (currentNode.Attributes.ContainsKey(key) && currentNode.Attributes[key] == value)
                    {
                        if (parentNode != null)
                        {
                            parentNode.Children.Remove(currentNode);
                            return true;
                        }
                    }
                }

                return false;
            }

            foreach (var child in currentNode.Children)
            {
                if (DeleteNodeWithAttributeRecursive(child, tagName, attributes, currentNode))
                {
                    return true;
                }
            }

            return false;
        }


        public string GenerateHtml()
        {
            var htmlBuilder = new StringWriter();
            AppendHtml(this, htmlBuilder);
            return htmlBuilder.ToString();
        }

        public string GenerateHtmlWithDoctype()
        {
            var htmlBuilder = new StringWriter();
            htmlBuilder.Write("<!DOCTYPE html>");
            AppendHtml(this, htmlBuilder);
            return htmlBuilder.ToString();
        }

        public void AppendHtml(HtmlNode node, StringWriter writer, bool closeSelfClosingTags = false)
        {
            writer.Write("<" + node.TagName);

            foreach (var attribute in node.Attributes)
            {
                if (attribute.Value == null) // boolean attribute
                {
                    writer.Write($" {attribute.Key}");
                }
                else
                {
                    writer.Write($" {attribute.Key}=\"{attribute.Value}\"");
                }
            }

            if (HtmlParser.IsSelfClosingTag(node.TagName))
            {
                writer.Write(closeSelfClosingTags ? " />" : ">");
                return;
            }

            writer.Write(">");

            if (!string.IsNullOrEmpty(node.InnerText))
            {
                writer.Write(node.InnerText);
            }

            foreach (var child in node.Children)
            {
                AppendHtml(child, writer, closeSelfClosingTags);
            }

            writer.Write($"</{node.TagName}>");
        }
    }

    public class HtmlToDictionaryConverter
    {
        public Dictionary<string, object> Convert(HtmlNode node)
        {
            var result = new Dictionary<string, object>
            {
                { "TagName", node.TagName },
                { "Attributes", node.Attributes }
            };

            if (node.Children.Count > 0)
            {
                var children = new List<Dictionary<string, object>>();
                foreach (var child in node.Children)
                {
                    children.Add(Convert(child));
                }

                result.Add("Children", children);
            }
            else
            {
                result.Add("InnerText", node.InnerText);
            }

            return result;
        }
    }

    public class HtmlReader
    {
        public static string ReadHtmlFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                throw new FileNotFoundException("The specified HTML file was not found.", filePath);
            }
        }


        public static void WriteHtmlToFile(string fileDirectory, string html)
        {
            if (Directory.Exists(fileDirectory))
            {
                Directory.Delete(fileDirectory, true);
            }

            Directory.CreateDirectory(fileDirectory);
            var filePath = Path.Combine(fileDirectory, "index.html");
            File.WriteAllText(filePath, html);
        }
    }
}
