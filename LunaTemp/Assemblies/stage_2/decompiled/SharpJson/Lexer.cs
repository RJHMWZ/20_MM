using System;
using System.Globalization;
using System.Text;

namespace SharpJson
{
	internal class Lexer
	{
		public enum Token
		{
			None,
			Null,
			True,
			False,
			Colon,
			Comma,
			String,
			Number,
			CurlyOpen,
			CurlyClose,
			SquaredOpen,
			SquaredClose
		}

		private char[] json;

		private int index = 0;

		private bool success = true;

		private char[] stringBuffer = new char[4096];

		public bool hasError => !success;

		public int lineNumber { get; private set; }

		public bool parseNumbersAsFloat { get; set; }

		public Lexer(string text)
		{
			Reset();
			json = text.ToCharArray();
			parseNumbersAsFloat = false;
		}

		public void Reset()
		{
			index = 0;
			lineNumber = 1;
			success = true;
		}

		public string ParseString()
		{
			int idx = 0;
			StringBuilder builder = null;
			SkipWhiteSpaces();
			char c = json[index++];
			bool failed = false;
			bool complete = false;
			while (!complete && !failed && index != json.Length)
			{
				c = json[index++];
				if (c == '"')
				{
					complete = true;
					break;
				}
				if (c == '\\')
				{
					if (index == json.Length)
					{
						break;
					}
					switch (json[index++])
					{
					case '"':
						stringBuffer[idx++] = '"';
						break;
					case '\\':
						stringBuffer[idx++] = '\\';
						break;
					case '/':
						stringBuffer[idx++] = '/';
						break;
					case 'b':
						stringBuffer[idx++] = '\b';
						break;
					case 'f':
						stringBuffer[idx++] = '\f';
						break;
					case 'n':
						stringBuffer[idx++] = '\n';
						break;
					case 'r':
						stringBuffer[idx++] = '\r';
						break;
					case 't':
						stringBuffer[idx++] = '\t';
						break;
					case 'u':
					{
						int remainingLength = json.Length - index;
						if (remainingLength >= 4)
						{
							string hex = new string(json, index, 4);
							stringBuffer[idx++] = (char)Convert.ToInt32(hex, 16);
							index += 4;
						}
						else
						{
							failed = true;
						}
						break;
					}
					}
				}
				else
				{
					stringBuffer[idx++] = c;
				}
				if (idx >= stringBuffer.Length)
				{
					if (builder == null)
					{
						builder = new StringBuilder();
					}
					builder.Append(stringBuffer, 0, idx);
					idx = 0;
				}
			}
			if (!complete)
			{
				success = false;
				return null;
			}
			if (builder != null)
			{
				return builder.ToString();
			}
			return new string(stringBuffer, 0, idx);
		}

		private string GetNumberString()
		{
			SkipWhiteSpaces();
			int lastIndex = GetLastIndexOfNumber(index);
			int charLength = lastIndex - index + 1;
			string result = new string(json, index, charLength);
			index = lastIndex + 1;
			return result;
		}

		public float ParseFloatNumber()
		{
			string str = GetNumberString();
			if (!float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
			{
				return 0f;
			}
			return number;
		}

		public double ParseDoubleNumber()
		{
			string str = GetNumberString();
			if (!double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
			{
				return 0.0;
			}
			return number;
		}

		private int GetLastIndexOfNumber(int index)
		{
			int lastIndex;
			for (lastIndex = index; lastIndex < json.Length; lastIndex++)
			{
				char ch = json[lastIndex];
				if ((ch < '0' || ch > '9') && ch != '+' && ch != '-' && ch != '.' && ch != 'e' && ch != 'E')
				{
					break;
				}
			}
			return lastIndex - 1;
		}

		private void SkipWhiteSpaces()
		{
			while (index < json.Length)
			{
				char ch = json[index];
				if (ch == '\n')
				{
					lineNumber++;
				}
				if (!char.IsWhiteSpace(json[index]))
				{
					break;
				}
				index++;
			}
		}

		public Token LookAhead()
		{
			SkipWhiteSpaces();
			int savedIndex = index;
			return NextToken(json, ref savedIndex);
		}

		public Token NextToken()
		{
			SkipWhiteSpaces();
			return NextToken(json, ref index);
		}

		private static Token NextToken(char[] json, ref int index)
		{
			if (index == json.Length)
			{
				return Token.None;
			}
			switch (json[index++])
			{
			case '{':
				return Token.CurlyOpen;
			case '}':
				return Token.CurlyClose;
			case '[':
				return Token.SquaredOpen;
			case ']':
				return Token.SquaredClose;
			case ',':
				return Token.Comma;
			case '"':
				return Token.String;
			case '-':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				return Token.Number;
			case ':':
				return Token.Colon;
			default:
			{
				index--;
				int remainingLength = json.Length - index;
				if (remainingLength >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
				{
					index += 5;
					return Token.False;
				}
				if (remainingLength >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
				{
					index += 4;
					return Token.True;
				}
				if (remainingLength >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
				{
					index += 4;
					return Token.Null;
				}
				return Token.None;
			}
			}
		}
	}
}
