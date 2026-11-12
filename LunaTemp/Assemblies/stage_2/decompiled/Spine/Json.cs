using System.IO;
using SharpJson;

namespace Spine
{
	public static class Json
	{
		public static object Deserialize(TextReader text)
		{
			JsonDecoder parser = new JsonDecoder();
			parser.parseNumbersAsFloat = true;
			return parser.Decode(text.ReadToEnd());
		}
	}
}
