using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	[ExecuteAlways]
	public class Reference20GuideShade : MaskableGraphic
	{
		private Vector2 lastSize;

		private Vector3 lastScale;

		public Reference20GuideShade()
		{
			base.useLegacyMeshGeneration = false;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if ((bool)base.canvas)
			{
				RectTransform bounds = base.canvas.rootCanvas.transform as RectTransform;
				Vector3[] corners = new Vector3[4];
				bounds.GetWorldCorners(corners);
				Vector3 min = base.rectTransform.InverseTransformPoint(corners[0]);
				Vector3 max = base.rectTransform.InverseTransformPoint(corners[2]);
				float left = 284f;
				float right = 511.2f;
				float bottom = 113.6f;
				float top = 227.2f;
				Quad(vh, min.x, min.y, max.x, bottom);
				Quad(vh, min.x, top, max.x, max.y);
				Quad(vh, min.x, bottom, left, top);
				Quad(vh, right, bottom, max.x, top);
			}
		}

		private void Quad(VertexHelper vh, float x0, float y0, float x1, float y1)
		{
			if (!(x1 <= x0) && !(y1 <= y0))
			{
				int i = vh.currentVertCount;
				Color32 c = color;
				vh.AddVert(new Vector3(x0, y0), c, Vector2.zero);
				vh.AddVert(new Vector3(x0, y1), c, Vector2.zero);
				vh.AddVert(new Vector3(x1, y1), c, Vector2.zero);
				vh.AddVert(new Vector3(x1, y0), c, Vector2.zero);
				vh.AddTriangle(i, i + 1, i + 2);
				vh.AddTriangle(i, i + 2, i + 3);
			}
		}

		private void LateUpdate()
		{
			if ((bool)base.canvas)
			{
				Vector2 size = (base.canvas.rootCanvas.transform as RectTransform).rect.size;
				Vector3 scale = base.rectTransform.lossyScale;
				if (size != lastSize || scale != lastScale)
				{
					lastSize = size;
					lastScale = scale;
					SetVerticesDirty();
				}
			}
		}
	}
}
