using System;
using UnityEngine;

namespace Spine.Unity
{
	public static class SkeletonExtensions
	{
		private const float ByteToFloat = 0.003921569f;

		public static Color GetColor(this Skeleton s)
		{
			return new Color(s.r, s.g, s.b, s.a);
		}

		public static Color GetColor(this RegionAttachment a)
		{
			return new Color(a.r, a.g, a.b, a.a);
		}

		public static Color GetColor(this MeshAttachment a)
		{
			return new Color(a.r, a.g, a.b, a.a);
		}

		public static Color GetColor(this Slot s)
		{
			return new Color(s.r, s.g, s.b, s.a);
		}

		public static Color GetColorTintBlack(this Slot s)
		{
			return new Color(s.r2, s.g2, s.b2, 1f);
		}

		public static void SetColor(this Skeleton skeleton, Color color)
		{
			skeleton.A = color.a;
			skeleton.R = color.r;
			skeleton.G = color.g;
			skeleton.B = color.b;
		}

		public static void SetColor(this Skeleton skeleton, Color32 color)
		{
			skeleton.A = (float)(int)color.a * 0.003921569f;
			skeleton.R = (float)(int)color.r * 0.003921569f;
			skeleton.G = (float)(int)color.g * 0.003921569f;
			skeleton.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this Slot slot, Color color)
		{
			slot.A = color.a;
			slot.R = color.r;
			slot.G = color.g;
			slot.B = color.b;
		}

		public static void SetColor(this Slot slot, Color32 color)
		{
			slot.A = (float)(int)color.a * 0.003921569f;
			slot.R = (float)(int)color.r * 0.003921569f;
			slot.G = (float)(int)color.g * 0.003921569f;
			slot.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this RegionAttachment attachment, Color color)
		{
			attachment.A = color.a;
			attachment.R = color.r;
			attachment.G = color.g;
			attachment.B = color.b;
		}

		public static void SetColor(this RegionAttachment attachment, Color32 color)
		{
			attachment.A = (float)(int)color.a * 0.003921569f;
			attachment.R = (float)(int)color.r * 0.003921569f;
			attachment.G = (float)(int)color.g * 0.003921569f;
			attachment.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this MeshAttachment attachment, Color color)
		{
			attachment.A = color.a;
			attachment.R = color.r;
			attachment.G = color.g;
			attachment.B = color.b;
		}

		public static void SetColor(this MeshAttachment attachment, Color32 color)
		{
			attachment.A = (float)(int)color.a * 0.003921569f;
			attachment.R = (float)(int)color.r * 0.003921569f;
			attachment.G = (float)(int)color.g * 0.003921569f;
			attachment.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetLocalScale(this Skeleton skeleton, Vector2 scale)
		{
			skeleton.ScaleX = scale.x;
			skeleton.ScaleY = scale.y;
		}

		public static Matrix4x4 GetMatrix4x4(this Bone bone)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.m00 = bone.a;
			result.m01 = bone.b;
			result.m03 = bone.worldX;
			result.m10 = bone.c;
			result.m11 = bone.d;
			result.m13 = bone.worldY;
			result.m33 = 1f;
			return result;
		}

		public static void SetLocalPosition(this Bone bone, Vector2 position)
		{
			bone.X = position.x;
			bone.Y = position.y;
		}

		public static void SetLocalPosition(this Bone bone, Vector3 position)
		{
			bone.X = position.x;
			bone.Y = position.y;
		}

		public static Vector2 GetLocalPosition(this Bone bone)
		{
			return new Vector2(bone.x, bone.y);
		}

		public static Vector2 GetSkeletonSpacePosition(this Bone bone)
		{
			return new Vector2(bone.worldX, bone.worldY);
		}

		public static Vector2 GetSkeletonSpacePosition(this Bone bone, Vector2 boneLocal)
		{
			Vector2 o = default(Vector2);
			bone.LocalToWorld(boneLocal.x, boneLocal.y, out o.x, out o.y);
			return o;
		}

		public static Vector3 GetWorldPosition(this Bone bone, Transform spineGameObjectTransform)
		{
			return spineGameObjectTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY));
		}

		public static Vector3 GetWorldPosition(this Bone bone, Transform spineGameObjectTransform, float positionScale)
		{
			return spineGameObjectTransform.TransformPoint(new Vector3(bone.worldX * positionScale, bone.worldY * positionScale));
		}

		public static Quaternion GetQuaternion(this Bone bone)
		{
			float halfRotation = Mathf.Atan2(bone.c, bone.a) * 0.5f;
			return new Quaternion(0f, 0f, Mathf.Sin(halfRotation), Mathf.Cos(halfRotation));
		}

		public static Quaternion GetLocalQuaternion(this Bone bone)
		{
			float halfRotation = bone.rotation * (3.14159265f / 180f) * 0.5f;
			return new Quaternion(0f, 0f, Mathf.Sin(halfRotation), Mathf.Cos(halfRotation));
		}

		public static Vector2 GetLocalScale(this Skeleton skeleton)
		{
			return new Vector2(skeleton.ScaleX, skeleton.ScaleY);
		}

		public static void GetWorldToLocalMatrix(this Bone bone, out float ia, out float ib, out float ic, out float id)
		{
			float a = bone.a;
			float b = bone.b;
			float c = bone.c;
			float d = bone.d;
			float invDet = 1f / (a * d - b * c);
			ia = invDet * d;
			ib = invDet * (0f - b);
			ic = invDet * (0f - c);
			id = invDet * a;
		}

		public static Vector2 WorldToLocal(this Bone bone, Vector2 worldPosition)
		{
			Vector2 o = default(Vector2);
			bone.WorldToLocal(worldPosition.x, worldPosition.y, out o.x, out o.y);
			return o;
		}

		public static Vector2 SetPositionSkeletonSpace(this Bone bone, Vector2 skeletonSpacePosition)
		{
			if (bone.parent == null)
			{
				bone.SetLocalPosition(skeletonSpacePosition);
				return skeletonSpacePosition;
			}
			Bone parent = bone.parent;
			Vector2 parentLocal = parent.WorldToLocal(skeletonSpacePosition);
			bone.SetLocalPosition(parentLocal);
			return parentLocal;
		}

		public static Material GetMaterial(this Attachment a)
		{
			object rendererObject = null;
			if (a is IHasRendererObject renderableAttachment)
			{
				rendererObject = renderableAttachment.RendererObject;
			}
			if (rendererObject == null)
			{
				return null;
			}
			return (Material)((AtlasRegion)rendererObject).page.rendererObject;
		}

		public static Vector2[] GetLocalVertices(this VertexAttachment va, Slot slot, Vector2[] buffer)
		{
			int floatsCount = va.worldVerticesLength;
			int bufferTargetSize = floatsCount >> 1;
			buffer = buffer ?? new Vector2[bufferTargetSize];
			if (buffer.Length < bufferTargetSize)
			{
				throw new ArgumentException($"Vector2 buffer too small. {va.Name} requires an array of size {floatsCount}. Use the attachment's .WorldVerticesLength to get the correct size.", "buffer");
			}
			if (va.bones == null)
			{
				float[] localVerts = va.vertices;
				for (int j = 0; j < bufferTargetSize; j++)
				{
					int l = j * 2;
					buffer[j] = new Vector2(localVerts[l], localVerts[l + 1]);
				}
			}
			else
			{
				float[] floats = new float[floatsCount];
				va.ComputeWorldVertices(slot, floats);
				Bone sb = slot.bone;
				float bwx = sb.worldX;
				float bwy = sb.worldY;
				sb.GetWorldToLocalMatrix(out var ia, out var ib, out var ic, out var id);
				for (int i = 0; i < bufferTargetSize; i++)
				{
					int k = i * 2;
					float x = floats[k] - bwx;
					float y = floats[k + 1] - bwy;
					buffer[i] = new Vector2(x * ia + y * ib, x * ic + y * id);
				}
			}
			return buffer;
		}

		public static Vector2[] GetWorldVertices(this VertexAttachment a, Slot slot, Vector2[] buffer)
		{
			int worldVertsLength = a.worldVerticesLength;
			int bufferTargetSize = worldVertsLength >> 1;
			buffer = buffer ?? new Vector2[bufferTargetSize];
			if (buffer.Length < bufferTargetSize)
			{
				throw new ArgumentException($"Vector2 buffer too small. {a.Name} requires an array of size {worldVertsLength}. Use the attachment's .WorldVerticesLength to get the correct size.", "buffer");
			}
			float[] floats = new float[worldVertsLength];
			a.ComputeWorldVertices(slot, floats);
			int i = 0;
			for (int k = worldVertsLength >> 1; i < k; i++)
			{
				int j = i * 2;
				buffer[i] = new Vector2(floats[j], floats[j + 1]);
			}
			return buffer;
		}

		public static Vector3 GetWorldPosition(this PointAttachment attachment, Slot slot, Transform spineGameObjectTransform)
		{
			Vector3 skeletonSpacePosition = default(Vector3);
			skeletonSpacePosition.z = 0f;
			attachment.ComputeWorldPosition(slot.bone, out skeletonSpacePosition.x, out skeletonSpacePosition.y);
			return spineGameObjectTransform.TransformPoint(skeletonSpacePosition);
		}

		public static Vector3 GetWorldPosition(this PointAttachment attachment, Bone bone, Transform spineGameObjectTransform)
		{
			Vector3 skeletonSpacePosition = default(Vector3);
			skeletonSpacePosition.z = 0f;
			attachment.ComputeWorldPosition(bone, out skeletonSpacePosition.x, out skeletonSpacePosition.y);
			return spineGameObjectTransform.TransformPoint(skeletonSpacePosition);
		}
	}
}
