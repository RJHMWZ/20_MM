using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Spine
{
	public class Atlas : IEnumerable<AtlasRegion>, IEnumerable
	{
		private readonly List<AtlasPage> pages = new List<AtlasPage>();

		private List<AtlasRegion> regions = new List<AtlasRegion>();

		private TextureLoader textureLoader;

		public List<AtlasRegion> Regions => regions;

		public List<AtlasPage> Pages => pages;

		public IEnumerator<AtlasRegion> GetEnumerator()
		{
			return regions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return regions.GetEnumerator();
		}

		public Atlas(TextReader reader, string dir, TextureLoader textureLoader)
		{
			Load(reader, dir, textureLoader);
		}

		public Atlas(List<AtlasPage> pages, List<AtlasRegion> regions)
		{
			this.pages = pages;
			this.regions = regions;
			textureLoader = null;
		}

		private void Load(TextReader reader, string imagesDir, TextureLoader textureLoader)
		{
			if (textureLoader == null)
			{
				throw new ArgumentNullException("textureLoader", "textureLoader cannot be null.");
			}
			this.textureLoader = textureLoader;
			string[] tuple = new string[4];
			AtlasPage page = null;
			while (true)
			{
				string line = reader.ReadLine();
				if (line == null)
				{
					break;
				}
				if (line.Trim().Length == 0)
				{
					page = null;
					continue;
				}
				if (page == null)
				{
					page = new AtlasPage();
					page.name = line;
					if (ReadTuple(reader, tuple) == 2)
					{
						page.width = int.Parse(tuple[0], CultureInfo.InvariantCulture);
						page.height = int.Parse(tuple[1], CultureInfo.InvariantCulture);
						ReadTuple(reader, tuple);
					}
					page.format = (Format)Enum.Parse(typeof(Format), tuple[0], false);
					ReadTuple(reader, tuple);
					page.minFilter = (TextureFilter)Enum.Parse(typeof(TextureFilter), tuple[0], false);
					page.magFilter = (TextureFilter)Enum.Parse(typeof(TextureFilter), tuple[1], false);
					string direction = ReadValue(reader);
					page.uWrap = TextureWrap.ClampToEdge;
					page.vWrap = TextureWrap.ClampToEdge;
					switch (direction)
					{
					case "x":
						page.uWrap = TextureWrap.Repeat;
						break;
					case "y":
						page.vWrap = TextureWrap.Repeat;
						break;
					case "xy":
						page.uWrap = (page.vWrap = TextureWrap.Repeat);
						break;
					}
					textureLoader.Load(page, Path.Combine(imagesDir, line));
					pages.Add(page);
					continue;
				}
				AtlasRegion region = new AtlasRegion();
				region.name = line;
				region.page = page;
				string rotateValue = ReadValue(reader);
				if (rotateValue == "true")
				{
					region.degrees = 90;
				}
				else if (rotateValue == "false")
				{
					region.degrees = 0;
				}
				else
				{
					region.degrees = int.Parse(rotateValue);
				}
				region.rotate = region.degrees == 90;
				ReadTuple(reader, tuple);
				int x = int.Parse(tuple[0], CultureInfo.InvariantCulture);
				int y = int.Parse(tuple[1], CultureInfo.InvariantCulture);
				ReadTuple(reader, tuple);
				int width = int.Parse(tuple[0], CultureInfo.InvariantCulture);
				int height = int.Parse(tuple[1], CultureInfo.InvariantCulture);
				region.u = (float)x / (float)page.width;
				region.v = (float)y / (float)page.height;
				if (region.rotate)
				{
					region.u2 = (float)(x + height) / (float)page.width;
					region.v2 = (float)(y + width) / (float)page.height;
				}
				else
				{
					region.u2 = (float)(x + width) / (float)page.width;
					region.v2 = (float)(y + height) / (float)page.height;
				}
				region.x = x;
				region.y = y;
				region.width = Math.Abs(width);
				region.height = Math.Abs(height);
				if (ReadTuple(reader, tuple) == 4)
				{
					region.splits = new int[4]
					{
						int.Parse(tuple[0], CultureInfo.InvariantCulture),
						int.Parse(tuple[1], CultureInfo.InvariantCulture),
						int.Parse(tuple[2], CultureInfo.InvariantCulture),
						int.Parse(tuple[3], CultureInfo.InvariantCulture)
					};
					if (ReadTuple(reader, tuple) == 4)
					{
						region.pads = new int[4]
						{
							int.Parse(tuple[0], CultureInfo.InvariantCulture),
							int.Parse(tuple[1], CultureInfo.InvariantCulture),
							int.Parse(tuple[2], CultureInfo.InvariantCulture),
							int.Parse(tuple[3], CultureInfo.InvariantCulture)
						};
						ReadTuple(reader, tuple);
					}
				}
				region.originalWidth = int.Parse(tuple[0], CultureInfo.InvariantCulture);
				region.originalHeight = int.Parse(tuple[1], CultureInfo.InvariantCulture);
				ReadTuple(reader, tuple);
				region.offsetX = int.Parse(tuple[0], CultureInfo.InvariantCulture);
				region.offsetY = int.Parse(tuple[1], CultureInfo.InvariantCulture);
				region.index = int.Parse(ReadValue(reader), CultureInfo.InvariantCulture);
				regions.Add(region);
			}
		}

		private static string ReadValue(TextReader reader)
		{
			string line = reader.ReadLine();
			int colon = line.IndexOf(':');
			if (colon == -1)
			{
				throw new Exception("Invalid line: " + line);
			}
			return line.Substring(colon + 1).Trim();
		}

		private static int ReadTuple(TextReader reader, string[] tuple)
		{
			string line = reader.ReadLine();
			int colon = line.IndexOf(':');
			if (colon == -1)
			{
				throw new Exception("Invalid line: " + line);
			}
			int i = 0;
			int lastMatch = colon + 1;
			for (; i < 3; i++)
			{
				int comma = line.IndexOf(',', lastMatch);
				if (comma == -1)
				{
					break;
				}
				tuple[i] = line.Substring(lastMatch, comma - lastMatch).Trim();
				lastMatch = comma + 1;
			}
			tuple[i] = line.Substring(lastMatch).Trim();
			return i + 1;
		}

		public void FlipV()
		{
			int i = 0;
			for (int j = regions.Count; i < j; i++)
			{
				AtlasRegion region = regions[i];
				region.v = 1f - region.v;
				region.v2 = 1f - region.v2;
			}
		}

		public AtlasRegion FindRegion(string name)
		{
			int i = 0;
			for (int j = regions.Count; i < j; i++)
			{
				if (regions[i].name == name)
				{
					return regions[i];
				}
			}
			return null;
		}

		public void Dispose()
		{
			if (textureLoader != null)
			{
				int i = 0;
				for (int j = pages.Count; i < j; i++)
				{
					textureLoader.Unload(pages[i].rendererObject);
				}
			}
		}
	}
}
