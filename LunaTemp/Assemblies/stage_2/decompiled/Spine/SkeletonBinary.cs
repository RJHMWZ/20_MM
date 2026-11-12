using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spine
{
	public class SkeletonBinary
	{
		internal class Vertices
		{
			public int[] bones;

			public float[] vertices;
		}

		internal class SkeletonInput
		{
			private byte[] chars = new byte[32];

			private byte[] bytesBigEndian = new byte[4];

			internal ExposedList<string> strings;

			private Stream input;

			public SkeletonInput(Stream input)
			{
				this.input = input;
			}

			public byte ReadByte()
			{
				return (byte)input.ReadByte();
			}

			public sbyte ReadSByte()
			{
				int value = input.ReadByte();
				if (value == -1)
				{
					throw new EndOfStreamException();
				}
				return (sbyte)value;
			}

			public bool ReadBoolean()
			{
				return input.ReadByte() != 0;
			}

			public float ReadFloat()
			{
				input.Read(bytesBigEndian, 0, 4);
				chars[3] = bytesBigEndian[0];
				chars[2] = bytesBigEndian[1];
				chars[1] = bytesBigEndian[2];
				chars[0] = bytesBigEndian[3];
				return BitConverter.ToSingle(chars, 0);
			}

			public int ReadInt()
			{
				input.Read(bytesBigEndian, 0, 4);
				return (bytesBigEndian[0] << 24) + (bytesBigEndian[1] << 16) + (bytesBigEndian[2] << 8) + bytesBigEndian[3];
			}

			public int ReadInt(bool optimizePositive)
			{
				int b = input.ReadByte();
				int result = b & 0x7F;
				if (((uint)b & 0x80u) != 0)
				{
					b = input.ReadByte();
					result |= (b & 0x7F) << 7;
					if (((uint)b & 0x80u) != 0)
					{
						b = input.ReadByte();
						result |= (b & 0x7F) << 14;
						if (((uint)b & 0x80u) != 0)
						{
							b = input.ReadByte();
							result |= (b & 0x7F) << 21;
							if (((uint)b & 0x80u) != 0)
							{
								result |= (input.ReadByte() & 0x7F) << 28;
							}
						}
					}
				}
				return optimizePositive ? result : ((result >> 1) ^ -(result & 1));
			}

			public string ReadString()
			{
				int byteCount = ReadInt(true);
				switch (byteCount)
				{
				case 0:
					return null;
				case 1:
					return "";
				default:
				{
					byteCount--;
					byte[] buffer = chars;
					if (buffer.Length < byteCount)
					{
						buffer = new byte[byteCount];
					}
					ReadFully(buffer, 0, byteCount);
					return Encoding.UTF8.GetString(buffer, 0, byteCount);
				}
				}
			}

			public string ReadStringRef()
			{
				int index = ReadInt(true);
				return (index == 0) ? null : strings.Items[index - 1];
			}

			public void ReadFully(byte[] buffer, int offset, int length)
			{
				while (length > 0)
				{
					int count = input.Read(buffer, offset, length);
					if (count <= 0)
					{
						throw new EndOfStreamException();
					}
					offset += count;
					length -= count;
				}
			}

			public string GetVersionString()
			{
				try
				{
					int byteCount = ReadInt(true);
					if (byteCount > 1)
					{
						input.Position += byteCount - 1;
					}
					byteCount = ReadInt(true);
					if (byteCount > 1)
					{
						byteCount--;
						byte[] buffer = new byte[byteCount];
						ReadFully(buffer, 0, byteCount);
						return Encoding.UTF8.GetString(buffer, 0, byteCount);
					}
					throw new ArgumentException("Stream does not contain a valid binary Skeleton Data.", "input");
				}
				catch (Exception ex)
				{
					throw new ArgumentException("Stream does not contain a valid binary Skeleton Data.\n" + ex, "input");
				}
			}
		}

		public const int BONE_ROTATE = 0;

		public const int BONE_TRANSLATE = 1;

		public const int BONE_SCALE = 2;

		public const int BONE_SHEAR = 3;

		public const int SLOT_ATTACHMENT = 0;

		public const int SLOT_COLOR = 1;

		public const int SLOT_TWO_COLOR = 2;

		public const int PATH_POSITION = 0;

		public const int PATH_SPACING = 1;

		public const int PATH_MIX = 2;

		public const int CURVE_LINEAR = 0;

		public const int CURVE_STEPPED = 1;

		public const int CURVE_BEZIER = 2;

		private AttachmentLoader attachmentLoader;

		private List<SkeletonJson.LinkedMesh> linkedMeshes = new List<SkeletonJson.LinkedMesh>();

		public static readonly TransformMode[] TransformModeValues = new TransformMode[5]
		{
			TransformMode.Normal,
			TransformMode.OnlyTranslation,
			TransformMode.NoRotationOrReflection,
			TransformMode.NoScale,
			TransformMode.NoScaleOrReflection
		};

		public float Scale { get; set; }

		public SkeletonBinary(params Atlas[] atlasArray)
			: this(new AtlasAttachmentLoader(atlasArray))
		{
		}

		public SkeletonBinary(AttachmentLoader attachmentLoader)
		{
			if (attachmentLoader == null)
			{
				throw new ArgumentNullException("attachmentLoader");
			}
			this.attachmentLoader = attachmentLoader;
			Scale = 1f;
		}

		public SkeletonData ReadSkeletonData(string path)
		{
			using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				SkeletonData skeletonData = ReadSkeletonData(input);
				skeletonData.name = Path.GetFileNameWithoutExtension(path);
				return skeletonData;
			}
		}

		public static string GetVersionString(Stream file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			SkeletonInput input = new SkeletonInput(file);
			return input.GetVersionString();
		}

		public SkeletonData ReadSkeletonData(Stream file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			float scale = Scale;
			SkeletonData skeletonData = new SkeletonData();
			SkeletonInput input = new SkeletonInput(file);
			skeletonData.hash = input.ReadString();
			if (skeletonData.hash.Length == 0)
			{
				skeletonData.hash = null;
			}
			skeletonData.version = input.ReadString();
			if (skeletonData.version.Length == 0)
			{
				skeletonData.version = null;
			}
			if ("3.8.75" == skeletonData.version)
			{
				throw new Exception("Unsupported skeleton data, please export with a newer version of Spine.");
			}
			skeletonData.x = input.ReadFloat();
			skeletonData.y = input.ReadFloat();
			skeletonData.width = input.ReadFloat();
			skeletonData.height = input.ReadFloat();
			bool nonessential = input.ReadBoolean();
			if (nonessential)
			{
				skeletonData.fps = input.ReadFloat();
				skeletonData.imagesPath = input.ReadString();
				if (string.IsNullOrEmpty(skeletonData.imagesPath))
				{
					skeletonData.imagesPath = null;
				}
				skeletonData.audioPath = input.ReadString();
				if (string.IsNullOrEmpty(skeletonData.audioPath))
				{
					skeletonData.audioPath = null;
				}
			}
			int n2;
			input.strings = new ExposedList<string>(n2 = input.ReadInt(true));
			object[] items = input.strings.Resize(n2).Items;
			object[] o = items;
			for (int i5 = 0; i5 < n2; i5++)
			{
				o[i5] = input.ReadString();
			}
			items = skeletonData.bones.Resize(n2 = input.ReadInt(true)).Items;
			o = items;
			for (int i4 = 0; i4 < n2; i4++)
			{
				string name = input.ReadString();
				BoneData parent = ((i4 == 0) ? null : skeletonData.bones.Items[input.ReadInt(true)]);
				BoneData data = new BoneData(i4, name, parent);
				data.rotation = input.ReadFloat();
				data.x = input.ReadFloat() * scale;
				data.y = input.ReadFloat() * scale;
				data.scaleX = input.ReadFloat();
				data.scaleY = input.ReadFloat();
				data.shearX = input.ReadFloat();
				data.shearY = input.ReadFloat();
				data.length = input.ReadFloat() * scale;
				data.transformMode = TransformModeValues[input.ReadInt(true)];
				data.skinRequired = input.ReadBoolean();
				if (nonessential)
				{
					input.ReadInt();
				}
				o[i4] = data;
			}
			items = skeletonData.slots.Resize(n2 = input.ReadInt(true)).Items;
			o = items;
			for (int i3 = 0; i3 < n2; i3++)
			{
				string slotName = input.ReadString();
				BoneData boneData = skeletonData.bones.Items[input.ReadInt(true)];
				SlotData slotData = new SlotData(i3, slotName, boneData);
				int color = input.ReadInt();
				slotData.r = (float)((color & 0xFF000000u) >> 24) / 255f;
				slotData.g = (float)((color & 0xFF0000) >> 16) / 255f;
				slotData.b = (float)((color & 0xFF00) >> 8) / 255f;
				slotData.a = (float)(color & 0xFF) / 255f;
				int darkColor = input.ReadInt();
				if (darkColor != -1)
				{
					slotData.hasSecondColor = true;
					slotData.r2 = (float)((darkColor & 0xFF0000) >> 16) / 255f;
					slotData.g2 = (float)((darkColor & 0xFF00) >> 8) / 255f;
					slotData.b2 = (float)(darkColor & 0xFF) / 255f;
				}
				slotData.attachmentName = input.ReadStringRef();
				slotData.blendMode = (BlendMode)input.ReadInt(true);
				o[i3] = slotData;
			}
			items = skeletonData.ikConstraints.Resize(n2 = input.ReadInt(true)).Items;
			o = items;
			for (int i2 = 0; i2 < n2; i2++)
			{
				IkConstraintData data2 = new IkConstraintData(input.ReadString());
				data2.order = input.ReadInt(true);
				data2.skinRequired = input.ReadBoolean();
				int nn;
				items = data2.bones.Resize(nn = input.ReadInt(true)).Items;
				object[] bones = items;
				for (int ii = 0; ii < nn; ii++)
				{
					bones[ii] = skeletonData.bones.Items[input.ReadInt(true)];
				}
				data2.target = skeletonData.bones.Items[input.ReadInt(true)];
				data2.mix = input.ReadFloat();
				data2.softness = input.ReadFloat() * scale;
				data2.bendDirection = input.ReadSByte();
				data2.compress = input.ReadBoolean();
				data2.stretch = input.ReadBoolean();
				data2.uniform = input.ReadBoolean();
				o[i2] = data2;
			}
			items = skeletonData.transformConstraints.Resize(n2 = input.ReadInt(true)).Items;
			o = items;
			for (int n = 0; n < n2; n++)
			{
				TransformConstraintData data3 = new TransformConstraintData(input.ReadString());
				data3.order = input.ReadInt(true);
				data3.skinRequired = input.ReadBoolean();
				int nn2;
				items = data3.bones.Resize(nn2 = input.ReadInt(true)).Items;
				object[] bones2 = items;
				for (int ii2 = 0; ii2 < nn2; ii2++)
				{
					bones2[ii2] = skeletonData.bones.Items[input.ReadInt(true)];
				}
				data3.target = skeletonData.bones.Items[input.ReadInt(true)];
				data3.local = input.ReadBoolean();
				data3.relative = input.ReadBoolean();
				data3.offsetRotation = input.ReadFloat();
				data3.offsetX = input.ReadFloat() * scale;
				data3.offsetY = input.ReadFloat() * scale;
				data3.offsetScaleX = input.ReadFloat();
				data3.offsetScaleY = input.ReadFloat();
				data3.offsetShearY = input.ReadFloat();
				data3.rotateMix = input.ReadFloat();
				data3.translateMix = input.ReadFloat();
				data3.scaleMix = input.ReadFloat();
				data3.shearMix = input.ReadFloat();
				o[n] = data3;
			}
			items = skeletonData.pathConstraints.Resize(n2 = input.ReadInt(true)).Items;
			o = items;
			for (int m = 0; m < n2; m++)
			{
				PathConstraintData data4 = new PathConstraintData(input.ReadString());
				data4.order = input.ReadInt(true);
				data4.skinRequired = input.ReadBoolean();
				int nn3;
				items = data4.bones.Resize(nn3 = input.ReadInt(true)).Items;
				object[] bones3 = items;
				for (int ii3 = 0; ii3 < nn3; ii3++)
				{
					bones3[ii3] = skeletonData.bones.Items[input.ReadInt(true)];
				}
				data4.target = skeletonData.slots.Items[input.ReadInt(true)];
				data4.positionMode = (PositionMode)Enum.GetValues(typeof(PositionMode)).GetValue(input.ReadInt(true));
				data4.spacingMode = (SpacingMode)Enum.GetValues(typeof(SpacingMode)).GetValue(input.ReadInt(true));
				data4.rotateMode = (RotateMode)Enum.GetValues(typeof(RotateMode)).GetValue(input.ReadInt(true));
				data4.offsetRotation = input.ReadFloat();
				data4.position = input.ReadFloat();
				if (data4.positionMode == PositionMode.Fixed)
				{
					data4.position *= scale;
				}
				data4.spacing = input.ReadFloat();
				if (data4.spacingMode == SpacingMode.Length || data4.spacingMode == SpacingMode.Fixed)
				{
					data4.spacing *= scale;
				}
				data4.rotateMix = input.ReadFloat();
				data4.translateMix = input.ReadFloat();
				o[m] = data4;
			}
			Skin defaultSkin = ReadSkin(input, skeletonData, true, nonessential);
			if (defaultSkin != null)
			{
				skeletonData.defaultSkin = defaultSkin;
				skeletonData.skins.Add(defaultSkin);
			}
			int l = skeletonData.skins.Count;
			items = skeletonData.skins.Resize(n2 = l + input.ReadInt(true)).Items;
			o = items;
			for (; l < n2; l++)
			{
				o[l] = ReadSkin(input, skeletonData, false, nonessential);
			}
			n2 = linkedMeshes.Count;
			for (int k = 0; k < n2; k++)
			{
				SkeletonJson.LinkedMesh linkedMesh = linkedMeshes[k];
				Skin skin = ((linkedMesh.skin == null) ? skeletonData.DefaultSkin : skeletonData.FindSkin(linkedMesh.skin));
				if (skin == null)
				{
					throw new Exception("Skin not found: " + linkedMesh.skin);
				}
				Attachment parent2 = skin.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
				if (parent2 == null)
				{
					throw new Exception("Parent mesh not found: " + linkedMesh.parent);
				}
				linkedMesh.mesh.DeformAttachment = (linkedMesh.inheritDeform ? ((VertexAttachment)parent2) : linkedMesh.mesh);
				linkedMesh.mesh.ParentMesh = (MeshAttachment)parent2;
				linkedMesh.mesh.UpdateUVs();
			}
			linkedMeshes.Clear();
			items = skeletonData.events.Resize(n2 = input.ReadInt(true)).Items;
			o = items;
			for (int j = 0; j < n2; j++)
			{
				EventData data5 = new EventData(input.ReadStringRef());
				data5.Int = input.ReadInt(false);
				data5.Float = input.ReadFloat();
				data5.String = input.ReadString();
				data5.AudioPath = input.ReadString();
				if (data5.AudioPath != null)
				{
					data5.Volume = input.ReadFloat();
					data5.Balance = input.ReadFloat();
				}
				o[j] = data5;
			}
			items = skeletonData.animations.Resize(n2 = input.ReadInt(true)).Items;
			o = items;
			for (int i = 0; i < n2; i++)
			{
				o[i] = ReadAnimation(input.ReadString(), input, skeletonData);
			}
			return skeletonData;
		}

		private Skin ReadSkin(SkeletonInput input, SkeletonData skeletonData, bool defaultSkin, bool nonessential)
		{
			int slotCount;
			Skin skin;
			if (defaultSkin)
			{
				slotCount = input.ReadInt(true);
				if (slotCount == 0)
				{
					return null;
				}
				skin = new Skin("default");
			}
			else
			{
				skin = new Skin(input.ReadStringRef());
				object[] items = skin.bones.Resize(input.ReadInt(true)).Items;
				object[] bones = items;
				int m = 0;
				for (int n4 = skin.bones.Count; m < n4; m++)
				{
					bones[m] = skeletonData.bones.Items[input.ReadInt(true)];
				}
				int l = 0;
				for (int n3 = input.ReadInt(true); l < n3; l++)
				{
					skin.constraints.Add(skeletonData.ikConstraints.Items[input.ReadInt(true)]);
				}
				int k = 0;
				for (int n2 = input.ReadInt(true); k < n2; k++)
				{
					skin.constraints.Add(skeletonData.transformConstraints.Items[input.ReadInt(true)]);
				}
				int j = 0;
				for (int n = input.ReadInt(true); j < n; j++)
				{
					skin.constraints.Add(skeletonData.pathConstraints.Items[input.ReadInt(true)]);
				}
				skin.constraints.TrimExcess();
				slotCount = input.ReadInt(true);
			}
			for (int i = 0; i < slotCount; i++)
			{
				int slotIndex = input.ReadInt(true);
				int ii = 0;
				for (int nn = input.ReadInt(true); ii < nn; ii++)
				{
					string name = input.ReadStringRef();
					Attachment attachment = ReadAttachment(input, skeletonData, skin, slotIndex, name, nonessential);
					if (attachment != null)
					{
						skin.SetAttachment(slotIndex, name, attachment);
					}
				}
			}
			return skin;
		}

		private Attachment ReadAttachment(SkeletonInput input, SkeletonData skeletonData, Skin skin, int slotIndex, string attachmentName, bool nonessential)
		{
			float scale = Scale;
			string name = input.ReadStringRef();
			if (name == null)
			{
				name = attachmentName;
			}
			switch (input.ReadByte())
			{
			case 0:
			{
				string path = input.ReadStringRef();
				float rotation = input.ReadFloat();
				float x = input.ReadFloat();
				float y = input.ReadFloat();
				float scaleX = input.ReadFloat();
				float scaleY = input.ReadFloat();
				float width = input.ReadFloat();
				float height = input.ReadFloat();
				int color = input.ReadInt();
				if (path == null)
				{
					path = name;
				}
				RegionAttachment region = attachmentLoader.NewRegionAttachment(skin, name, path);
				if (region == null)
				{
					return null;
				}
				region.Path = path;
				region.x = x * scale;
				region.y = y * scale;
				region.scaleX = scaleX;
				region.scaleY = scaleY;
				region.rotation = rotation;
				region.width = width * scale;
				region.height = height * scale;
				region.r = (float)((color & 0xFF000000u) >> 24) / 255f;
				region.g = (float)((color & 0xFF0000) >> 16) / 255f;
				region.b = (float)((color & 0xFF00) >> 8) / 255f;
				region.a = (float)(color & 0xFF) / 255f;
				region.UpdateOffset();
				return region;
			}
			case 1:
			{
				int vertexCount2 = input.ReadInt(true);
				Vertices vertices = ReadVertices(input, vertexCount2);
				if (nonessential)
				{
					input.ReadInt();
				}
				BoundingBoxAttachment box = attachmentLoader.NewBoundingBoxAttachment(skin, name);
				if (box == null)
				{
					return null;
				}
				box.worldVerticesLength = vertexCount2 << 1;
				box.vertices = vertices.vertices;
				box.bones = vertices.bones;
				return box;
			}
			case 2:
			{
				string path2 = input.ReadStringRef();
				int color2 = input.ReadInt();
				int vertexCount3 = input.ReadInt(true);
				float[] uvs = ReadFloatArray(input, vertexCount3 << 1, 1f);
				int[] triangles = ReadShortArray(input);
				Vertices vertices3 = ReadVertices(input, vertexCount3);
				int hullLength = input.ReadInt(true);
				int[] edges = null;
				float width2 = 0f;
				float height2 = 0f;
				if (nonessential)
				{
					edges = ReadShortArray(input);
					width2 = input.ReadFloat();
					height2 = input.ReadFloat();
				}
				if (path2 == null)
				{
					path2 = name;
				}
				MeshAttachment mesh = attachmentLoader.NewMeshAttachment(skin, name, path2);
				if (mesh == null)
				{
					return null;
				}
				mesh.Path = path2;
				mesh.r = (float)((color2 & 0xFF000000u) >> 24) / 255f;
				mesh.g = (float)((color2 & 0xFF0000) >> 16) / 255f;
				mesh.b = (float)((color2 & 0xFF00) >> 8) / 255f;
				mesh.a = (float)(color2 & 0xFF) / 255f;
				mesh.bones = vertices3.bones;
				mesh.vertices = vertices3.vertices;
				mesh.WorldVerticesLength = vertexCount3 << 1;
				mesh.triangles = triangles;
				mesh.regionUVs = uvs;
				mesh.UpdateUVs();
				mesh.HullLength = hullLength << 1;
				if (nonessential)
				{
					mesh.Edges = edges;
					mesh.Width = width2 * scale;
					mesh.Height = height2 * scale;
				}
				return mesh;
			}
			case 3:
			{
				string path3 = input.ReadStringRef();
				int color3 = input.ReadInt();
				string skinName = input.ReadStringRef();
				string parent = input.ReadStringRef();
				bool inheritDeform = input.ReadBoolean();
				float width3 = 0f;
				float height3 = 0f;
				if (nonessential)
				{
					width3 = input.ReadFloat();
					height3 = input.ReadFloat();
				}
				if (path3 == null)
				{
					path3 = name;
				}
				MeshAttachment mesh2 = attachmentLoader.NewMeshAttachment(skin, name, path3);
				if (mesh2 == null)
				{
					return null;
				}
				mesh2.Path = path3;
				mesh2.r = (float)((color3 & 0xFF000000u) >> 24) / 255f;
				mesh2.g = (float)((color3 & 0xFF0000) >> 16) / 255f;
				mesh2.b = (float)((color3 & 0xFF00) >> 8) / 255f;
				mesh2.a = (float)(color3 & 0xFF) / 255f;
				if (nonessential)
				{
					mesh2.Width = width3 * scale;
					mesh2.Height = height3 * scale;
				}
				linkedMeshes.Add(new SkeletonJson.LinkedMesh(mesh2, skinName, slotIndex, parent, inheritDeform));
				return mesh2;
			}
			case 4:
			{
				bool closed = input.ReadBoolean();
				bool constantSpeed = input.ReadBoolean();
				int vertexCount4 = input.ReadInt(true);
				Vertices vertices4 = ReadVertices(input, vertexCount4);
				float[] lengths = new float[vertexCount4 / 3];
				int i = 0;
				for (int j = lengths.Length; i < j; i++)
				{
					lengths[i] = input.ReadFloat() * scale;
				}
				if (nonessential)
				{
					input.ReadInt();
				}
				PathAttachment path4 = attachmentLoader.NewPathAttachment(skin, name);
				if (path4 == null)
				{
					return null;
				}
				path4.closed = closed;
				path4.constantSpeed = constantSpeed;
				path4.worldVerticesLength = vertexCount4 << 1;
				path4.vertices = vertices4.vertices;
				path4.bones = vertices4.bones;
				path4.lengths = lengths;
				return path4;
			}
			case 5:
			{
				float rotation2 = input.ReadFloat();
				float x2 = input.ReadFloat();
				float y2 = input.ReadFloat();
				if (nonessential)
				{
					input.ReadInt();
				}
				PointAttachment point = attachmentLoader.NewPointAttachment(skin, name);
				if (point == null)
				{
					return null;
				}
				point.x = x2 * scale;
				point.y = y2 * scale;
				point.rotation = rotation2;
				return point;
			}
			case 6:
			{
				int endSlotIndex = input.ReadInt(true);
				int vertexCount = input.ReadInt(true);
				Vertices vertices2 = ReadVertices(input, vertexCount);
				if (nonessential)
				{
					input.ReadInt();
				}
				ClippingAttachment clip = attachmentLoader.NewClippingAttachment(skin, name);
				if (clip == null)
				{
					return null;
				}
				clip.EndSlot = skeletonData.slots.Items[endSlotIndex];
				clip.worldVerticesLength = vertexCount << 1;
				clip.vertices = vertices2.vertices;
				clip.bones = vertices2.bones;
				return clip;
			}
			default:
				return null;
			}
		}

		private Vertices ReadVertices(SkeletonInput input, int vertexCount)
		{
			float scale = Scale;
			int verticesLength = vertexCount << 1;
			Vertices vertices = new Vertices();
			if (!input.ReadBoolean())
			{
				vertices.vertices = ReadFloatArray(input, verticesLength, scale);
				return vertices;
			}
			ExposedList<float> weights = new ExposedList<float>(verticesLength * 3 * 3);
			ExposedList<int> bonesArray = new ExposedList<int>(verticesLength * 3);
			for (int i = 0; i < vertexCount; i++)
			{
				int boneCount = input.ReadInt(true);
				bonesArray.Add(boneCount);
				for (int ii = 0; ii < boneCount; ii++)
				{
					bonesArray.Add(input.ReadInt(true));
					weights.Add(input.ReadFloat() * scale);
					weights.Add(input.ReadFloat() * scale);
					weights.Add(input.ReadFloat());
				}
			}
			vertices.vertices = weights.ToArray();
			vertices.bones = bonesArray.ToArray();
			return vertices;
		}

		private float[] ReadFloatArray(SkeletonInput input, int n, float scale)
		{
			float[] array = new float[n];
			if (scale == 1f)
			{
				for (int j = 0; j < n; j++)
				{
					array[j] = input.ReadFloat();
				}
			}
			else
			{
				for (int i = 0; i < n; i++)
				{
					array[i] = input.ReadFloat() * scale;
				}
			}
			return array;
		}

		private int[] ReadShortArray(SkeletonInput input)
		{
			int j = input.ReadInt(true);
			int[] array = new int[j];
			for (int i = 0; i < j; i++)
			{
				array[i] = (input.ReadByte() << 8) | input.ReadByte();
			}
			return array;
		}

		private Animation ReadAnimation(string name, SkeletonInput input, SkeletonData skeletonData)
		{
			ExposedList<Timeline> timelines = new ExposedList<Timeline>(32);
			float scale = Scale;
			float duration = 0f;
			int i = 0;
			for (int n2 = input.ReadInt(true); i < n2; i++)
			{
				int slotIndex = input.ReadInt(true);
				int ii = 0;
				for (int nn = input.ReadInt(true); ii < nn; ii++)
				{
					int timelineType = input.ReadByte();
					int frameCount = input.ReadInt(true);
					switch (timelineType)
					{
					case 0:
					{
						AttachmentTimeline timeline = new AttachmentTimeline(frameCount);
						timeline.slotIndex = slotIndex;
						for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
						{
							timeline.SetFrame(frameIndex, input.ReadFloat(), input.ReadStringRef());
						}
						timelines.Add(timeline);
						duration = Math.Max(duration, timeline.frames[frameCount - 1]);
						break;
					}
					case 1:
					{
						ColorTimeline timeline2 = new ColorTimeline(frameCount);
						timeline2.slotIndex = slotIndex;
						for (int frameIndex2 = 0; frameIndex2 < frameCount; frameIndex2++)
						{
							float time = input.ReadFloat();
							int color = input.ReadInt();
							float r = (float)((color & 0xFF000000u) >> 24) / 255f;
							float g = (float)((color & 0xFF0000) >> 16) / 255f;
							float b = (float)((color & 0xFF00) >> 8) / 255f;
							float a = (float)(color & 0xFF) / 255f;
							timeline2.SetFrame(frameIndex2, time, r, g, b, a);
							if (frameIndex2 < frameCount - 1)
							{
								ReadCurve(input, frameIndex2, timeline2);
							}
						}
						timelines.Add(timeline2);
						duration = Math.Max(duration, timeline2.frames[(frameCount - 1) * 5]);
						break;
					}
					case 2:
					{
						TwoColorTimeline timeline3 = new TwoColorTimeline(frameCount);
						timeline3.slotIndex = slotIndex;
						for (int frameIndex3 = 0; frameIndex3 < frameCount; frameIndex3++)
						{
							float time2 = input.ReadFloat();
							int color2 = input.ReadInt();
							float r2 = (float)((color2 & 0xFF000000u) >> 24) / 255f;
							float g2 = (float)((color2 & 0xFF0000) >> 16) / 255f;
							float b2 = (float)((color2 & 0xFF00) >> 8) / 255f;
							float a2 = (float)(color2 & 0xFF) / 255f;
							int color3 = input.ReadInt();
							float r3 = (float)((color3 & 0xFF0000) >> 16) / 255f;
							float g3 = (float)((color3 & 0xFF00) >> 8) / 255f;
							float b3 = (float)(color3 & 0xFF) / 255f;
							timeline3.SetFrame(frameIndex3, time2, r2, g2, b2, a2, r3, g3, b3);
							if (frameIndex3 < frameCount - 1)
							{
								ReadCurve(input, frameIndex3, timeline3);
							}
						}
						timelines.Add(timeline3);
						duration = Math.Max(duration, timeline3.frames[(frameCount - 1) * 8]);
						break;
					}
					}
				}
			}
			int j = 0;
			for (int n3 = input.ReadInt(true); j < n3; j++)
			{
				int boneIndex = input.ReadInt(true);
				int ii2 = 0;
				for (int nn2 = input.ReadInt(true); ii2 < nn2; ii2++)
				{
					int timelineType2 = input.ReadByte();
					int frameCount2 = input.ReadInt(true);
					switch (timelineType2)
					{
					case 0:
					{
						RotateTimeline timeline4 = new RotateTimeline(frameCount2);
						timeline4.boneIndex = boneIndex;
						for (int frameIndex4 = 0; frameIndex4 < frameCount2; frameIndex4++)
						{
							timeline4.SetFrame(frameIndex4, input.ReadFloat(), input.ReadFloat());
							if (frameIndex4 < frameCount2 - 1)
							{
								ReadCurve(input, frameIndex4, timeline4);
							}
						}
						timelines.Add(timeline4);
						duration = Math.Max(duration, timeline4.frames[(frameCount2 - 1) * 2]);
						break;
					}
					case 1:
					case 2:
					case 3:
					{
						float timelineScale = 1f;
						TranslateTimeline timeline5;
						switch (timelineType2)
						{
						case 2:
							timeline5 = new ScaleTimeline(frameCount2);
							break;
						case 3:
							timeline5 = new ShearTimeline(frameCount2);
							break;
						default:
							timeline5 = new TranslateTimeline(frameCount2);
							timelineScale = scale;
							break;
						}
						timeline5.boneIndex = boneIndex;
						for (int frameIndex5 = 0; frameIndex5 < frameCount2; frameIndex5++)
						{
							timeline5.SetFrame(frameIndex5, input.ReadFloat(), input.ReadFloat() * timelineScale, input.ReadFloat() * timelineScale);
							if (frameIndex5 < frameCount2 - 1)
							{
								ReadCurve(input, frameIndex5, timeline5);
							}
						}
						timelines.Add(timeline5);
						duration = Math.Max(duration, timeline5.frames[(frameCount2 - 1) * 3]);
						break;
					}
					}
				}
			}
			int k = 0;
			for (int n4 = input.ReadInt(true); k < n4; k++)
			{
				int index = input.ReadInt(true);
				int frameCount3 = input.ReadInt(true);
				IkConstraintTimeline timeline6 = new IkConstraintTimeline(frameCount3)
				{
					ikConstraintIndex = index
				};
				for (int frameIndex6 = 0; frameIndex6 < frameCount3; frameIndex6++)
				{
					timeline6.SetFrame(frameIndex6, input.ReadFloat(), input.ReadFloat(), input.ReadFloat() * scale, input.ReadSByte(), input.ReadBoolean(), input.ReadBoolean());
					if (frameIndex6 < frameCount3 - 1)
					{
						ReadCurve(input, frameIndex6, timeline6);
					}
				}
				timelines.Add(timeline6);
				duration = Math.Max(duration, timeline6.frames[(frameCount3 - 1) * 6]);
			}
			int l = 0;
			for (int n5 = input.ReadInt(true); l < n5; l++)
			{
				int index2 = input.ReadInt(true);
				int frameCount4 = input.ReadInt(true);
				TransformConstraintTimeline timeline8 = new TransformConstraintTimeline(frameCount4);
				timeline8.transformConstraintIndex = index2;
				for (int frameIndex7 = 0; frameIndex7 < frameCount4; frameIndex7++)
				{
					timeline8.SetFrame(frameIndex7, input.ReadFloat(), input.ReadFloat(), input.ReadFloat(), input.ReadFloat(), input.ReadFloat());
					if (frameIndex7 < frameCount4 - 1)
					{
						ReadCurve(input, frameIndex7, timeline8);
					}
				}
				timelines.Add(timeline8);
				duration = Math.Max(duration, timeline8.frames[(frameCount4 - 1) * 5]);
			}
			int m = 0;
			for (int n6 = input.ReadInt(true); m < n6; m++)
			{
				int index3 = input.ReadInt(true);
				PathConstraintData data = skeletonData.pathConstraints.Items[index3];
				int ii4 = 0;
				for (int nn3 = input.ReadInt(true); ii4 < nn3; ii4++)
				{
					int timelineType3 = input.ReadSByte();
					int frameCount5 = input.ReadInt(true);
					switch (timelineType3)
					{
					case 0:
					case 1:
					{
						float timelineScale2 = 1f;
						PathConstraintPositionTimeline timeline9;
						if (timelineType3 == 1)
						{
							timeline9 = new PathConstraintSpacingTimeline(frameCount5);
							if (data.spacingMode == SpacingMode.Length || data.spacingMode == SpacingMode.Fixed)
							{
								timelineScale2 = scale;
							}
						}
						else
						{
							timeline9 = new PathConstraintPositionTimeline(frameCount5);
							if (data.positionMode == PositionMode.Fixed)
							{
								timelineScale2 = scale;
							}
						}
						timeline9.pathConstraintIndex = index3;
						for (int frameIndex8 = 0; frameIndex8 < frameCount5; frameIndex8++)
						{
							timeline9.SetFrame(frameIndex8, input.ReadFloat(), input.ReadFloat() * timelineScale2);
							if (frameIndex8 < frameCount5 - 1)
							{
								ReadCurve(input, frameIndex8, timeline9);
							}
						}
						timelines.Add(timeline9);
						duration = Math.Max(duration, timeline9.frames[(frameCount5 - 1) * 2]);
						break;
					}
					case 2:
					{
						PathConstraintMixTimeline timeline10 = new PathConstraintMixTimeline(frameCount5);
						timeline10.pathConstraintIndex = index3;
						for (int frameIndex9 = 0; frameIndex9 < frameCount5; frameIndex9++)
						{
							timeline10.SetFrame(frameIndex9, input.ReadFloat(), input.ReadFloat(), input.ReadFloat());
							if (frameIndex9 < frameCount5 - 1)
							{
								ReadCurve(input, frameIndex9, timeline10);
							}
						}
						timelines.Add(timeline10);
						duration = Math.Max(duration, timeline10.frames[(frameCount5 - 1) * 3]);
						break;
					}
					}
				}
			}
			int n = 0;
			for (int n7 = input.ReadInt(true); n < n7; n++)
			{
				Skin skin = skeletonData.skins.Items[input.ReadInt(true)];
				int ii5 = 0;
				for (int nn4 = input.ReadInt(true); ii5 < nn4; ii5++)
				{
					int slotIndex2 = input.ReadInt(true);
					int iii = 0;
					for (int nnn = input.ReadInt(true); iii < nnn; iii++)
					{
						VertexAttachment attachment = (VertexAttachment)skin.GetAttachment(slotIndex2, input.ReadStringRef());
						bool weighted = attachment.bones != null;
						float[] vertices = attachment.vertices;
						int deformLength = (weighted ? (vertices.Length / 3 * 2) : vertices.Length);
						int frameCount6 = input.ReadInt(true);
						DeformTimeline timeline11 = new DeformTimeline(frameCount6);
						timeline11.slotIndex = slotIndex2;
						timeline11.attachment = attachment;
						for (int frameIndex10 = 0; frameIndex10 < frameCount6; frameIndex10++)
						{
							float time4 = input.ReadFloat();
							int end = input.ReadInt(true);
							float[] deform;
							if (end == 0)
							{
								deform = (weighted ? new float[deformLength] : vertices);
							}
							else
							{
								deform = new float[deformLength];
								int start = input.ReadInt(true);
								end += start;
								if (scale == 1f)
								{
									for (int v3 = start; v3 < end; v3++)
									{
										deform[v3] = input.ReadFloat();
									}
								}
								else
								{
									for (int v2 = start; v2 < end; v2++)
									{
										deform[v2] = input.ReadFloat() * scale;
									}
								}
								if (!weighted)
								{
									int v = 0;
									for (int vn = deform.Length; v < vn; v++)
									{
										deform[v] += vertices[v];
									}
								}
							}
							timeline11.SetFrame(frameIndex10, time4, deform);
							if (frameIndex10 < frameCount6 - 1)
							{
								ReadCurve(input, frameIndex10, timeline11);
							}
						}
						timelines.Add(timeline11);
						duration = Math.Max(duration, timeline11.frames[frameCount6 - 1]);
					}
				}
			}
			int drawOrderCount = input.ReadInt(true);
			if (drawOrderCount > 0)
			{
				DrawOrderTimeline timeline7 = new DrawOrderTimeline(drawOrderCount);
				int slotCount = skeletonData.slots.Count;
				for (int i2 = 0; i2 < drawOrderCount; i2++)
				{
					float time3 = input.ReadFloat();
					int offsetCount = input.ReadInt(true);
					int[] drawOrder = new int[slotCount];
					for (int ii3 = slotCount - 1; ii3 >= 0; ii3--)
					{
						drawOrder[ii3] = -1;
					}
					int[] unchanged = new int[slotCount - offsetCount];
					int originalIndex = 0;
					int unchangedIndex = 0;
					for (int ii7 = 0; ii7 < offsetCount; ii7++)
					{
						int slotIndex3 = input.ReadInt(true);
						while (originalIndex != slotIndex3)
						{
							unchanged[unchangedIndex++] = originalIndex++;
						}
						drawOrder[originalIndex + input.ReadInt(true)] = originalIndex++;
					}
					while (originalIndex < slotCount)
					{
						unchanged[unchangedIndex++] = originalIndex++;
					}
					for (int ii6 = slotCount - 1; ii6 >= 0; ii6--)
					{
						if (drawOrder[ii6] == -1)
						{
							drawOrder[ii6] = unchanged[--unchangedIndex];
						}
					}
					timeline7.SetFrame(i2, time3, drawOrder);
				}
				timelines.Add(timeline7);
				duration = Math.Max(duration, timeline7.frames[drawOrderCount - 1]);
			}
			int eventCount = input.ReadInt(true);
			if (eventCount > 0)
			{
				EventTimeline timeline12 = new EventTimeline(eventCount);
				for (int i3 = 0; i3 < eventCount; i3++)
				{
					float time5 = input.ReadFloat();
					EventData eventData = skeletonData.events.Items[input.ReadInt(true)];
					Event e = new Event(time5, eventData)
					{
						Int = input.ReadInt(false),
						Float = input.ReadFloat(),
						String = (input.ReadBoolean() ? input.ReadString() : eventData.String)
					};
					if (e.data.AudioPath != null)
					{
						e.volume = input.ReadFloat();
						e.balance = input.ReadFloat();
					}
					timeline12.SetFrame(i3, e);
				}
				timelines.Add(timeline12);
				duration = Math.Max(duration, timeline12.frames[eventCount - 1]);
			}
			timelines.TrimExcess();
			return new Animation(name, timelines, duration);
		}

		private void ReadCurve(SkeletonInput input, int frameIndex, CurveTimeline timeline)
		{
			switch (input.ReadByte())
			{
			case 1:
				timeline.SetStepped(frameIndex);
				break;
			case 2:
				timeline.SetCurve(frameIndex, input.ReadFloat(), input.ReadFloat(), input.ReadFloat(), input.ReadFloat());
				break;
			}
		}
	}
}
