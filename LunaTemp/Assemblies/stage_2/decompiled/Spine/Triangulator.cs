using System;

namespace Spine
{
	public class Triangulator
	{
		private readonly ExposedList<ExposedList<float>> convexPolygons = new ExposedList<ExposedList<float>>();

		private readonly ExposedList<ExposedList<int>> convexPolygonsIndices = new ExposedList<ExposedList<int>>();

		private readonly ExposedList<int> indicesArray = new ExposedList<int>();

		private readonly ExposedList<bool> isConcaveArray = new ExposedList<bool>();

		private readonly ExposedList<int> triangles = new ExposedList<int>();

		private readonly Pool<ExposedList<float>> polygonPool = new Pool<ExposedList<float>>();

		private readonly Pool<ExposedList<int>> polygonIndicesPool = new Pool<ExposedList<int>>();

		public ExposedList<int> Triangulate(ExposedList<float> verticesArray)
		{
			float[] vertices = verticesArray.Items;
			int vertexCount = verticesArray.Count >> 1;
			ExposedList<int> indicesArray = this.indicesArray;
			indicesArray.Clear();
			int[] indices = indicesArray.Resize(vertexCount).Items;
			for (int k = 0; k < vertexCount; k++)
			{
				indices[k] = k;
			}
			ExposedList<bool> isConcaveArray = this.isConcaveArray;
			bool[] isConcave = isConcaveArray.Resize(vertexCount).Items;
			int j = 0;
			for (int l = vertexCount; j < l; j++)
			{
				isConcave[j] = IsConcave(j, vertexCount, vertices, indices);
			}
			ExposedList<int> triangles = this.triangles;
			triangles.Clear();
			triangles.EnsureCapacity(Math.Max(0, vertexCount - 2) << 2);
			while (vertexCount > 3)
			{
				int previous = vertexCount - 1;
				int i = 0;
				int next = 1;
				while (true)
				{
					if (!isConcave[i])
					{
						int p1 = indices[previous] << 1;
						int p2 = indices[i] << 1;
						int p3 = indices[next] << 1;
						float p1x = vertices[p1];
						float p1y = vertices[p1 + 1];
						float p2x = vertices[p2];
						float p2y = vertices[p2 + 1];
						float p3x = vertices[p3];
						float p3y = vertices[p3 + 1];
						for (int ii = (next + 1) % vertexCount; ii != previous; ii = (ii + 1) % vertexCount)
						{
							if (!isConcave[ii])
							{
								continue;
							}
							int v = indices[ii] << 1;
							float vx = vertices[v];
							float vy = vertices[v + 1];
							if (!PositiveArea(p3x, p3y, p1x, p1y, vx, vy) || !PositiveArea(p1x, p1y, p2x, p2y, vx, vy) || !PositiveArea(p2x, p2y, p3x, p3y, vx, vy))
							{
								continue;
							}
							goto IL_01b1;
						}
						break;
					}
					goto IL_01b1;
					IL_01b1:
					if (next == 0)
					{
						while (isConcave[i])
						{
							i--;
							if (i <= 0)
							{
								break;
							}
						}
						break;
					}
					previous = i;
					i = next;
					next = (next + 1) % vertexCount;
				}
				triangles.Add(indices[(vertexCount + i - 1) % vertexCount]);
				triangles.Add(indices[i]);
				triangles.Add(indices[(i + 1) % vertexCount]);
				indicesArray.RemoveAt(i);
				isConcaveArray.RemoveAt(i);
				vertexCount--;
				int previousIndex = (vertexCount + i - 1) % vertexCount;
				int nextIndex = ((i != vertexCount) ? i : 0);
				isConcave[previousIndex] = IsConcave(previousIndex, vertexCount, vertices, indices);
				isConcave[nextIndex] = IsConcave(nextIndex, vertexCount, vertices, indices);
			}
			if (vertexCount == 3)
			{
				triangles.Add(indices[2]);
				triangles.Add(indices[0]);
				triangles.Add(indices[1]);
			}
			return triangles;
		}

		public ExposedList<ExposedList<float>> Decompose(ExposedList<float> verticesArray, ExposedList<int> triangles)
		{
			float[] vertices = verticesArray.Items;
			ExposedList<ExposedList<float>> convexPolygons = this.convexPolygons;
			int i = 0;
			for (int n4 = convexPolygons.Count; i < n4; i++)
			{
				polygonPool.Free(convexPolygons.Items[i]);
			}
			convexPolygons.Clear();
			ExposedList<ExposedList<int>> convexPolygonsIndices = this.convexPolygonsIndices;
			int m = 0;
			for (int n3 = convexPolygonsIndices.Count; m < n3; m++)
			{
				polygonIndicesPool.Free(convexPolygonsIndices.Items[m]);
			}
			convexPolygonsIndices.Clear();
			ExposedList<int> polygonIndices = polygonIndicesPool.Obtain();
			polygonIndices.Clear();
			ExposedList<float> polygon = polygonPool.Obtain();
			polygon.Clear();
			int fanBaseIndex = -1;
			int lastWinding = 0;
			int[] trianglesItems = triangles.Items;
			int l = 0;
			for (int n2 = triangles.Count; l < n2; l += 3)
			{
				int t1 = trianglesItems[l] << 1;
				int t2 = trianglesItems[l + 1] << 1;
				int t3 = trianglesItems[l + 2] << 1;
				float x1 = vertices[t1];
				float y1 = vertices[t1 + 1];
				float x2 = vertices[t2];
				float y2 = vertices[t2 + 1];
				float x3 = vertices[t3];
				float y3 = vertices[t3 + 1];
				bool merged = false;
				if (fanBaseIndex == t1)
				{
					int o = polygon.Count - 4;
					float[] p = polygon.Items;
					int winding2 = Winding(p[o], p[o + 1], p[o + 2], p[o + 3], x3, y3);
					int winding4 = Winding(x3, y3, p[0], p[1], p[2], p[3]);
					if (winding2 == lastWinding && winding4 == lastWinding)
					{
						polygon.Add(x3);
						polygon.Add(y3);
						polygonIndices.Add(t3);
						merged = true;
					}
				}
				if (!merged)
				{
					if (polygon.Count > 0)
					{
						convexPolygons.Add(polygon);
						convexPolygonsIndices.Add(polygonIndices);
					}
					else
					{
						polygonPool.Free(polygon);
						polygonIndicesPool.Free(polygonIndices);
					}
					polygon = polygonPool.Obtain();
					polygon.Clear();
					polygon.Add(x1);
					polygon.Add(y1);
					polygon.Add(x2);
					polygon.Add(y2);
					polygon.Add(x3);
					polygon.Add(y3);
					polygonIndices = polygonIndicesPool.Obtain();
					polygonIndices.Clear();
					polygonIndices.Add(t1);
					polygonIndices.Add(t2);
					polygonIndices.Add(t3);
					lastWinding = Winding(x1, y1, x2, y2, x3, y3);
					fanBaseIndex = t1;
				}
			}
			if (polygon.Count > 0)
			{
				convexPolygons.Add(polygon);
				convexPolygonsIndices.Add(polygonIndices);
			}
			int k = 0;
			for (int n = convexPolygons.Count; k < n; k++)
			{
				polygonIndices = convexPolygonsIndices.Items[k];
				if (polygonIndices.Count == 0)
				{
					continue;
				}
				int firstIndex = polygonIndices.Items[0];
				int lastIndex = polygonIndices.Items[polygonIndices.Count - 1];
				polygon = convexPolygons.Items[k];
				int o2 = polygon.Count - 4;
				float[] p2 = polygon.Items;
				float prevPrevX = p2[o2];
				float prevPrevY = p2[o2 + 1];
				float prevX = p2[o2 + 2];
				float prevY = p2[o2 + 3];
				float firstX = p2[0];
				float firstY = p2[1];
				float secondX = p2[2];
				float secondY = p2[3];
				int winding = Winding(prevPrevX, prevPrevY, prevX, prevY, firstX, firstY);
				for (int ii = 0; ii < n; ii++)
				{
					if (ii == k)
					{
						continue;
					}
					ExposedList<int> otherIndices = convexPolygonsIndices.Items[ii];
					if (otherIndices.Count != 3)
					{
						continue;
					}
					int otherFirstIndex = otherIndices.Items[0];
					int otherSecondIndex = otherIndices.Items[1];
					int otherLastIndex = otherIndices.Items[2];
					ExposedList<float> otherPoly = convexPolygons.Items[ii];
					float x4 = otherPoly.Items[otherPoly.Count - 2];
					float y4 = otherPoly.Items[otherPoly.Count - 1];
					if (otherFirstIndex == firstIndex && otherSecondIndex == lastIndex)
					{
						int winding3 = Winding(prevPrevX, prevPrevY, prevX, prevY, x4, y4);
						int winding5 = Winding(x4, y4, firstX, firstY, secondX, secondY);
						if (winding3 == winding && winding5 == winding)
						{
							otherPoly.Clear();
							otherIndices.Clear();
							polygon.Add(x4);
							polygon.Add(y4);
							polygonIndices.Add(otherLastIndex);
							prevPrevX = prevX;
							prevPrevY = prevY;
							prevX = x4;
							prevY = y4;
							ii = 0;
						}
					}
				}
			}
			for (int j = convexPolygons.Count - 1; j >= 0; j--)
			{
				polygon = convexPolygons.Items[j];
				if (polygon.Count == 0)
				{
					convexPolygons.RemoveAt(j);
					polygonPool.Free(polygon);
					polygonIndices = convexPolygonsIndices.Items[j];
					convexPolygonsIndices.RemoveAt(j);
					polygonIndicesPool.Free(polygonIndices);
				}
			}
			return convexPolygons;
		}

		private static bool IsConcave(int index, int vertexCount, float[] vertices, int[] indices)
		{
			int previous = indices[(vertexCount + index - 1) % vertexCount] << 1;
			int current = indices[index] << 1;
			int next = indices[(index + 1) % vertexCount] << 1;
			return !PositiveArea(vertices[previous], vertices[previous + 1], vertices[current], vertices[current + 1], vertices[next], vertices[next + 1]);
		}

		private static bool PositiveArea(float p1x, float p1y, float p2x, float p2y, float p3x, float p3y)
		{
			return p1x * (p3y - p2y) + p2x * (p1y - p3y) + p3x * (p2y - p1y) >= 0f;
		}

		private static int Winding(float p1x, float p1y, float p2x, float p2y, float p3x, float p3y)
		{
			float px = p2x - p1x;
			float py = p2y - p1y;
			return (p3x * py - p3y * px + px * p1y - p1x * py >= 0f) ? 1 : (-1);
		}
	}
}
