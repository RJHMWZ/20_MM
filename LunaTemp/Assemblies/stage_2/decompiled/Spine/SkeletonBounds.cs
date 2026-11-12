using System;

namespace Spine
{
	public class SkeletonBounds
	{
		private ExposedList<Polygon> polygonPool = new ExposedList<Polygon>();

		private float minX;

		private float minY;

		private float maxX;

		private float maxY;

		public ExposedList<BoundingBoxAttachment> BoundingBoxes { get; private set; }

		public ExposedList<Polygon> Polygons { get; private set; }

		public float MinX
		{
			get
			{
				return minX;
			}
			set
			{
				minX = value;
			}
		}

		public float MinY
		{
			get
			{
				return minY;
			}
			set
			{
				minY = value;
			}
		}

		public float MaxX
		{
			get
			{
				return maxX;
			}
			set
			{
				maxX = value;
			}
		}

		public float MaxY
		{
			get
			{
				return maxY;
			}
			set
			{
				maxY = value;
			}
		}

		public float Width => maxX - minX;

		public float Height => maxY - minY;

		public SkeletonBounds()
		{
			BoundingBoxes = new ExposedList<BoundingBoxAttachment>();
			Polygons = new ExposedList<Polygon>();
		}

		public void Update(Skeleton skeleton, bool updateAabb)
		{
			ExposedList<BoundingBoxAttachment> boundingBoxes = BoundingBoxes;
			ExposedList<Polygon> polygons = Polygons;
			ExposedList<Slot> slots = skeleton.slots;
			int slotCount = slots.Count;
			boundingBoxes.Clear();
			int j = 0;
			for (int k = polygons.Count; j < k; j++)
			{
				polygonPool.Add(polygons.Items[j]);
			}
			polygons.Clear();
			for (int i = 0; i < slotCount; i++)
			{
				Slot slot = slots.Items[i];
				if (slot.bone.active && slot.attachment is BoundingBoxAttachment boundingBox)
				{
					boundingBoxes.Add(boundingBox);
					Polygon polygon = null;
					int poolCount = polygonPool.Count;
					if (poolCount > 0)
					{
						polygon = polygonPool.Items[poolCount - 1];
						polygonPool.RemoveAt(poolCount - 1);
					}
					else
					{
						polygon = new Polygon();
					}
					polygons.Add(polygon);
					int count = (polygon.Count = boundingBox.worldVerticesLength);
					if (polygon.Vertices.Length < count)
					{
						polygon.Vertices = new float[count];
					}
					boundingBox.ComputeWorldVertices(slot, polygon.Vertices);
				}
			}
			if (updateAabb)
			{
				AabbCompute();
				return;
			}
			minX = -2.1474836E+09f;
			minY = -2.1474836E+09f;
			maxX = 2.1474836E+09f;
			maxY = 2.1474836E+09f;
		}

		private void AabbCompute()
		{
			float minX = 2.1474836E+09f;
			float minY = 2.1474836E+09f;
			float maxX = -2.1474836E+09f;
			float maxY = -2.1474836E+09f;
			ExposedList<Polygon> polygons = Polygons;
			int i = 0;
			for (int j = polygons.Count; i < j; i++)
			{
				Polygon polygon = polygons.Items[i];
				float[] vertices = polygon.Vertices;
				int ii = 0;
				for (int nn = polygon.Count; ii < nn; ii += 2)
				{
					float x = vertices[ii];
					float y = vertices[ii + 1];
					minX = Math.Min(minX, x);
					minY = Math.Min(minY, y);
					maxX = Math.Max(maxX, x);
					maxY = Math.Max(maxY, y);
				}
			}
			this.minX = minX;
			this.minY = minY;
			this.maxX = maxX;
			this.maxY = maxY;
		}

		public bool AabbContainsPoint(float x, float y)
		{
			return x >= minX && x <= maxX && y >= minY && y <= maxY;
		}

		public bool AabbIntersectsSegment(float x1, float y1, float x2, float y2)
		{
			float minX = this.minX;
			float minY = this.minY;
			float maxX = this.maxX;
			float maxY = this.maxY;
			if ((x1 <= minX && x2 <= minX) || (y1 <= minY && y2 <= minY) || (x1 >= maxX && x2 >= maxX) || (y1 >= maxY && y2 >= maxY))
			{
				return false;
			}
			float i = (y2 - y1) / (x2 - x1);
			float y3 = i * (minX - x1) + y1;
			if (y3 > minY && y3 < maxY)
			{
				return true;
			}
			y3 = i * (maxX - x1) + y1;
			if (y3 > minY && y3 < maxY)
			{
				return true;
			}
			float x3 = (minY - y1) / i + x1;
			if (x3 > minX && x3 < maxX)
			{
				return true;
			}
			x3 = (maxY - y1) / i + x1;
			if (x3 > minX && x3 < maxX)
			{
				return true;
			}
			return false;
		}

		public bool AabbIntersectsSkeleton(SkeletonBounds bounds)
		{
			return minX < bounds.maxX && maxX > bounds.minX && minY < bounds.maxY && maxY > bounds.minY;
		}

		public bool ContainsPoint(Polygon polygon, float x, float y)
		{
			float[] vertices = polygon.Vertices;
			int nn = polygon.Count;
			int prevIndex = nn - 2;
			bool inside = false;
			for (int ii = 0; ii < nn; ii += 2)
			{
				float vertexY = vertices[ii + 1];
				float prevY = vertices[prevIndex + 1];
				if ((vertexY < y && prevY >= y) || (prevY < y && vertexY >= y))
				{
					float vertexX = vertices[ii];
					if (vertexX + (y - vertexY) / (prevY - vertexY) * (vertices[prevIndex] - vertexX) < x)
					{
						inside = !inside;
					}
				}
				prevIndex = ii;
			}
			return inside;
		}

		public BoundingBoxAttachment ContainsPoint(float x, float y)
		{
			ExposedList<Polygon> polygons = Polygons;
			int i = 0;
			for (int j = polygons.Count; i < j; i++)
			{
				if (ContainsPoint(polygons.Items[i], x, y))
				{
					return BoundingBoxes.Items[i];
				}
			}
			return null;
		}

		public BoundingBoxAttachment IntersectsSegment(float x1, float y1, float x2, float y2)
		{
			ExposedList<Polygon> polygons = Polygons;
			int i = 0;
			for (int j = polygons.Count; i < j; i++)
			{
				if (IntersectsSegment(polygons.Items[i], x1, y1, x2, y2))
				{
					return BoundingBoxes.Items[i];
				}
			}
			return null;
		}

		public bool IntersectsSegment(Polygon polygon, float x1, float y1, float x2, float y2)
		{
			float[] vertices = polygon.Vertices;
			int nn = polygon.Count;
			float width12 = x1 - x2;
			float height12 = y1 - y2;
			float det1 = x1 * y2 - y1 * x2;
			float x4 = vertices[nn - 2];
			float y4 = vertices[nn - 1];
			for (int ii = 0; ii < nn; ii += 2)
			{
				float x5 = vertices[ii];
				float y5 = vertices[ii + 1];
				float det2 = x4 * y5 - y4 * x5;
				float width13 = x4 - x5;
				float height13 = y4 - y5;
				float det3 = width12 * height13 - height12 * width13;
				float x3 = (det1 * width13 - width12 * det2) / det3;
				if (((x3 >= x4 && x3 <= x5) || (x3 >= x5 && x3 <= x4)) && ((x3 >= x1 && x3 <= x2) || (x3 >= x2 && x3 <= x1)))
				{
					float y3 = (det1 * height13 - height12 * det2) / det3;
					if (((y3 >= y4 && y3 <= y5) || (y3 >= y5 && y3 <= y4)) && ((y3 >= y1 && y3 <= y2) || (y3 >= y2 && y3 <= y1)))
					{
						return true;
					}
				}
				x4 = x5;
				y4 = y5;
			}
			return false;
		}

		public Polygon GetPolygon(BoundingBoxAttachment attachment)
		{
			int index = BoundingBoxes.IndexOf(attachment);
			return (index == -1) ? null : Polygons.Items[index];
		}
	}
}
