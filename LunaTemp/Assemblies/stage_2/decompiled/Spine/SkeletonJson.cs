using System;
using System.Collections.Generic;
using System.IO;

namespace Spine
{
	public class SkeletonJson
	{
		internal class LinkedMesh
		{
			internal string parent;

			internal string skin;

			internal int slotIndex;

			internal MeshAttachment mesh;

			internal bool inheritDeform;

			public LinkedMesh(MeshAttachment mesh, string skin, int slotIndex, string parent, bool inheritDeform)
			{
				this.mesh = mesh;
				this.skin = skin;
				this.slotIndex = slotIndex;
				this.parent = parent;
				this.inheritDeform = inheritDeform;
			}
		}

		private AttachmentLoader attachmentLoader;

		private List<LinkedMesh> linkedMeshes = new List<LinkedMesh>();

		public float Scale { get; set; }

		public SkeletonJson(params Atlas[] atlasArray)
			: this(new AtlasAttachmentLoader(atlasArray))
		{
		}

		public SkeletonJson(AttachmentLoader attachmentLoader)
		{
			if (attachmentLoader == null)
			{
				throw new ArgumentNullException("attachmentLoader", "attachmentLoader cannot be null.");
			}
			this.attachmentLoader = attachmentLoader;
			Scale = 1f;
		}

		public SkeletonData ReadSkeletonData(string path)
		{
			using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				SkeletonData skeletonData = ReadSkeletonData(reader);
				skeletonData.name = Path.GetFileNameWithoutExtension(path);
				return skeletonData;
			}
		}

		public SkeletonData ReadSkeletonData(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader", "reader cannot be null.");
			}
			float scale = Scale;
			SkeletonData skeletonData = new SkeletonData();
			if (!(Json.Deserialize(reader) is Dictionary<string, object> root))
			{
				throw new Exception("Invalid JSON.");
			}
			if (root.ContainsKey("skeleton"))
			{
				Dictionary<string, object> skeletonMap = (Dictionary<string, object>)root["skeleton"];
				skeletonData.hash = (string)skeletonMap["hash"];
				skeletonData.version = (string)skeletonMap["spine"];
				if ("3.8.75" == skeletonData.version)
				{
					throw new Exception("Unsupported skeleton data, please export with a newer version of Spine.");
				}
				skeletonData.x = GetFloat(skeletonMap, "x", 0f);
				skeletonData.y = GetFloat(skeletonMap, "y", 0f);
				skeletonData.width = GetFloat(skeletonMap, "width", 0f);
				skeletonData.height = GetFloat(skeletonMap, "height", 0f);
				skeletonData.fps = GetFloat(skeletonMap, "fps", 30f);
				skeletonData.imagesPath = GetString(skeletonMap, "images", null);
				skeletonData.audioPath = GetString(skeletonMap, "audio", null);
			}
			if (root.ContainsKey("bones"))
			{
				foreach (Dictionary<string, object> boneMap in (List<object>)root["bones"])
				{
					BoneData parent2 = null;
					if (boneMap.ContainsKey("parent"))
					{
						parent2 = skeletonData.FindBone((string)boneMap["parent"]);
						if (parent2 == null)
						{
							throw new Exception("Parent bone not found: " + boneMap["parent"]);
						}
					}
					BoneData data2 = new BoneData(skeletonData.Bones.Count, (string)boneMap["name"], parent2);
					data2.length = GetFloat(boneMap, "length", 0f) * scale;
					data2.x = GetFloat(boneMap, "x", 0f) * scale;
					data2.y = GetFloat(boneMap, "y", 0f) * scale;
					data2.rotation = GetFloat(boneMap, "rotation", 0f);
					data2.scaleX = GetFloat(boneMap, "scaleX", 1f);
					data2.scaleY = GetFloat(boneMap, "scaleY", 1f);
					data2.shearX = GetFloat(boneMap, "shearX", 0f);
					data2.shearY = GetFloat(boneMap, "shearY", 0f);
					string tm = GetString(boneMap, "transform", TransformMode.Normal.ToString());
					data2.transformMode = (TransformMode)Enum.Parse(typeof(TransformMode), tm, true);
					data2.skinRequired = GetBoolean(boneMap, "skin", false);
					skeletonData.bones.Add(data2);
				}
			}
			if (root.ContainsKey("slots"))
			{
				foreach (Dictionary<string, object> slotMap in (List<object>)root["slots"])
				{
					string slotName = (string)slotMap["name"];
					string boneName = (string)slotMap["bone"];
					BoneData boneData = skeletonData.FindBone(boneName);
					if (boneData == null)
					{
						throw new Exception("Slot bone not found: " + boneName);
					}
					SlotData data3 = new SlotData(skeletonData.Slots.Count, slotName, boneData);
					if (slotMap.ContainsKey("color"))
					{
						string color = (string)slotMap["color"];
						data3.r = ToColor(color, 0);
						data3.g = ToColor(color, 1);
						data3.b = ToColor(color, 2);
						data3.a = ToColor(color, 3);
					}
					if (slotMap.ContainsKey("dark"))
					{
						string color2 = (string)slotMap["dark"];
						data3.r2 = ToColor(color2, 0, 6);
						data3.g2 = ToColor(color2, 1, 6);
						data3.b2 = ToColor(color2, 2, 6);
						data3.hasSecondColor = true;
					}
					data3.attachmentName = GetString(slotMap, "attachment", null);
					if (slotMap.ContainsKey("blend"))
					{
						data3.blendMode = (BlendMode)Enum.Parse(typeof(BlendMode), (string)slotMap["blend"], true);
					}
					else
					{
						data3.blendMode = BlendMode.Normal;
					}
					skeletonData.slots.Add(data3);
				}
			}
			if (root.ContainsKey("ik"))
			{
				foreach (Dictionary<string, object> constraintMap in (List<object>)root["ik"])
				{
					IkConstraintData data4 = new IkConstraintData((string)constraintMap["name"]);
					data4.order = GetInt(constraintMap, "order", 0);
					data4.skinRequired = GetBoolean(constraintMap, "skin", false);
					if (constraintMap.ContainsKey("bones"))
					{
						foreach (string boneName4 in (List<object>)constraintMap["bones"])
						{
							BoneData bone4 = skeletonData.FindBone(boneName4);
							if (bone4 == null)
							{
								throw new Exception("IK bone not found: " + boneName4);
							}
							data4.bones.Add(bone4);
						}
					}
					string targetName3 = (string)constraintMap["target"];
					data4.target = skeletonData.FindBone(targetName3);
					if (data4.target == null)
					{
						throw new Exception("IK target bone not found: " + targetName3);
					}
					data4.mix = GetFloat(constraintMap, "mix", 1f);
					data4.softness = GetFloat(constraintMap, "softness", 0f) * scale;
					data4.bendDirection = (GetBoolean(constraintMap, "bendPositive", true) ? 1 : (-1));
					data4.compress = GetBoolean(constraintMap, "compress", false);
					data4.stretch = GetBoolean(constraintMap, "stretch", false);
					data4.uniform = GetBoolean(constraintMap, "uniform", false);
					skeletonData.ikConstraints.Add(data4);
				}
			}
			if (root.ContainsKey("transform"))
			{
				foreach (Dictionary<string, object> constraintMap3 in (List<object>)root["transform"])
				{
					TransformConstraintData data6 = new TransformConstraintData((string)constraintMap3["name"]);
					data6.order = GetInt(constraintMap3, "order", 0);
					data6.skinRequired = GetBoolean(constraintMap3, "skin", false);
					if (constraintMap3.ContainsKey("bones"))
					{
						foreach (string boneName3 in (List<object>)constraintMap3["bones"])
						{
							BoneData bone3 = skeletonData.FindBone(boneName3);
							if (bone3 == null)
							{
								throw new Exception("Transform constraint bone not found: " + boneName3);
							}
							data6.bones.Add(bone3);
						}
					}
					string targetName2 = (string)constraintMap3["target"];
					data6.target = skeletonData.FindBone(targetName2);
					if (data6.target == null)
					{
						throw new Exception("Transform constraint target bone not found: " + targetName2);
					}
					data6.local = GetBoolean(constraintMap3, "local", false);
					data6.relative = GetBoolean(constraintMap3, "relative", false);
					data6.offsetRotation = GetFloat(constraintMap3, "rotation", 0f);
					data6.offsetX = GetFloat(constraintMap3, "x", 0f) * scale;
					data6.offsetY = GetFloat(constraintMap3, "y", 0f) * scale;
					data6.offsetScaleX = GetFloat(constraintMap3, "scaleX", 0f);
					data6.offsetScaleY = GetFloat(constraintMap3, "scaleY", 0f);
					data6.offsetShearY = GetFloat(constraintMap3, "shearY", 0f);
					data6.rotateMix = GetFloat(constraintMap3, "rotateMix", 1f);
					data6.translateMix = GetFloat(constraintMap3, "translateMix", 1f);
					data6.scaleMix = GetFloat(constraintMap3, "scaleMix", 1f);
					data6.shearMix = GetFloat(constraintMap3, "shearMix", 1f);
					skeletonData.transformConstraints.Add(data6);
				}
			}
			if (root.ContainsKey("path"))
			{
				foreach (Dictionary<string, object> constraintMap2 in (List<object>)root["path"])
				{
					PathConstraintData data5 = new PathConstraintData((string)constraintMap2["name"]);
					data5.order = GetInt(constraintMap2, "order", 0);
					data5.skinRequired = GetBoolean(constraintMap2, "skin", false);
					if (constraintMap2.ContainsKey("bones"))
					{
						foreach (string boneName2 in (List<object>)constraintMap2["bones"])
						{
							BoneData bone2 = skeletonData.FindBone(boneName2);
							if (bone2 == null)
							{
								throw new Exception("Path bone not found: " + boneName2);
							}
							data5.bones.Add(bone2);
						}
					}
					string targetName = (string)constraintMap2["target"];
					data5.target = skeletonData.FindSlot(targetName);
					if (data5.target == null)
					{
						throw new Exception("Path target slot not found: " + targetName);
					}
					data5.positionMode = (PositionMode)Enum.Parse(typeof(PositionMode), GetString(constraintMap2, "positionMode", "percent"), true);
					data5.spacingMode = (SpacingMode)Enum.Parse(typeof(SpacingMode), GetString(constraintMap2, "spacingMode", "length"), true);
					data5.rotateMode = (RotateMode)Enum.Parse(typeof(RotateMode), GetString(constraintMap2, "rotateMode", "tangent"), true);
					data5.offsetRotation = GetFloat(constraintMap2, "rotation", 0f);
					data5.position = GetFloat(constraintMap2, "position", 0f);
					if (data5.positionMode == PositionMode.Fixed)
					{
						data5.position *= scale;
					}
					data5.spacing = GetFloat(constraintMap2, "spacing", 0f);
					if (data5.spacingMode == SpacingMode.Length || data5.spacingMode == SpacingMode.Fixed)
					{
						data5.spacing *= scale;
					}
					data5.rotateMix = GetFloat(constraintMap2, "rotateMix", 1f);
					data5.translateMix = GetFloat(constraintMap2, "translateMix", 1f);
					skeletonData.pathConstraints.Add(data5);
				}
			}
			if (root.ContainsKey("skins"))
			{
				foreach (Dictionary<string, object> skinMap in (List<object>)root["skins"])
				{
					Skin skin2 = new Skin((string)skinMap["name"]);
					if (skinMap.ContainsKey("bones"))
					{
						foreach (string entryName4 in (List<object>)skinMap["bones"])
						{
							BoneData bone = skeletonData.FindBone(entryName4);
							if (bone == null)
							{
								throw new Exception("Skin bone not found: " + entryName4);
							}
							skin2.bones.Add(bone);
						}
					}
					if (skinMap.ContainsKey("ik"))
					{
						foreach (string entryName3 in (List<object>)skinMap["ik"])
						{
							IkConstraintData constraint3 = skeletonData.FindIkConstraint(entryName3);
							if (constraint3 == null)
							{
								throw new Exception("Skin IK constraint not found: " + entryName3);
							}
							skin2.constraints.Add(constraint3);
						}
					}
					if (skinMap.ContainsKey("transform"))
					{
						foreach (string entryName2 in (List<object>)skinMap["transform"])
						{
							TransformConstraintData constraint2 = skeletonData.FindTransformConstraint(entryName2);
							if (constraint2 == null)
							{
								throw new Exception("Skin transform constraint not found: " + entryName2);
							}
							skin2.constraints.Add(constraint2);
						}
					}
					if (skinMap.ContainsKey("path"))
					{
						foreach (string entryName in (List<object>)skinMap["path"])
						{
							PathConstraintData constraint = skeletonData.FindPathConstraint(entryName);
							if (constraint == null)
							{
								throw new Exception("Skin path constraint not found: " + entryName);
							}
							skin2.constraints.Add(constraint);
						}
					}
					if (skinMap.ContainsKey("attachments"))
					{
						foreach (KeyValuePair<string, object> slotEntry in (Dictionary<string, object>)skinMap["attachments"])
						{
							int slotIndex = skeletonData.FindSlotIndex(slotEntry.Key);
							foreach (KeyValuePair<string, object> entry3 in (Dictionary<string, object>)slotEntry.Value)
							{
								try
								{
									Attachment attachment = ReadAttachment((Dictionary<string, object>)entry3.Value, skin2, slotIndex, entry3.Key, skeletonData);
									if (attachment != null)
									{
										skin2.SetAttachment(slotIndex, entry3.Key, attachment);
									}
								}
								catch (Exception e2)
								{
									throw new Exception("Error reading attachment: " + entry3.Key + ", skin: " + skin2, e2);
								}
							}
						}
					}
					skeletonData.skins.Add(skin2);
					if (skin2.name == "default")
					{
						skeletonData.defaultSkin = skin2;
					}
				}
			}
			int i = 0;
			for (int j = linkedMeshes.Count; i < j; i++)
			{
				LinkedMesh linkedMesh = linkedMeshes[i];
				Skin skin = ((linkedMesh.skin == null) ? skeletonData.defaultSkin : skeletonData.FindSkin(linkedMesh.skin));
				if (skin == null)
				{
					throw new Exception("Slot not found: " + linkedMesh.skin);
				}
				Attachment parent = skin.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
				if (parent == null)
				{
					throw new Exception("Parent mesh not found: " + linkedMesh.parent);
				}
				linkedMesh.mesh.DeformAttachment = (linkedMesh.inheritDeform ? ((VertexAttachment)parent) : linkedMesh.mesh);
				linkedMesh.mesh.ParentMesh = (MeshAttachment)parent;
				linkedMesh.mesh.UpdateUVs();
			}
			linkedMeshes.Clear();
			if (root.ContainsKey("events"))
			{
				foreach (KeyValuePair<string, object> entry2 in (Dictionary<string, object>)root["events"])
				{
					Dictionary<string, object> entryMap = (Dictionary<string, object>)entry2.Value;
					EventData data = new EventData(entry2.Key);
					data.Int = GetInt(entryMap, "int", 0);
					data.Float = GetFloat(entryMap, "float", 0f);
					data.String = GetString(entryMap, "string", string.Empty);
					data.AudioPath = GetString(entryMap, "audio", null);
					if (data.AudioPath != null)
					{
						data.Volume = GetFloat(entryMap, "volume", 1f);
						data.Balance = GetFloat(entryMap, "balance", 0f);
					}
					skeletonData.events.Add(data);
				}
			}
			if (root.ContainsKey("animations"))
			{
				foreach (KeyValuePair<string, object> entry in (Dictionary<string, object>)root["animations"])
				{
					try
					{
						ReadAnimation((Dictionary<string, object>)entry.Value, entry.Key, skeletonData);
					}
					catch (Exception e)
					{
						throw new Exception("Error reading animation: " + entry.Key, e);
					}
				}
			}
			skeletonData.bones.TrimExcess();
			skeletonData.slots.TrimExcess();
			skeletonData.skins.TrimExcess();
			skeletonData.events.TrimExcess();
			skeletonData.animations.TrimExcess();
			skeletonData.ikConstraints.TrimExcess();
			return skeletonData;
		}

		private Attachment ReadAttachment(Dictionary<string, object> map, Skin skin, int slotIndex, string name, SkeletonData skeletonData)
		{
			float scale = Scale;
			name = GetString(map, "name", name);
			string typeName = GetString(map, "type", "region");
			AttachmentType type = (AttachmentType)Enum.Parse(typeof(AttachmentType), typeName, true);
			string path = GetString(map, "path", name);
			switch (type)
			{
			case AttachmentType.Region:
			{
				RegionAttachment region = attachmentLoader.NewRegionAttachment(skin, name, path);
				if (region == null)
				{
					return null;
				}
				region.Path = path;
				region.x = GetFloat(map, "x", 0f) * scale;
				region.y = GetFloat(map, "y", 0f) * scale;
				region.scaleX = GetFloat(map, "scaleX", 1f);
				region.scaleY = GetFloat(map, "scaleY", 1f);
				region.rotation = GetFloat(map, "rotation", 0f);
				region.width = GetFloat(map, "width", 32f) * scale;
				region.height = GetFloat(map, "height", 32f) * scale;
				if (map.ContainsKey("color"))
				{
					string color = (string)map["color"];
					region.r = ToColor(color, 0);
					region.g = ToColor(color, 1);
					region.b = ToColor(color, 2);
					region.a = ToColor(color, 3);
				}
				region.UpdateOffset();
				return region;
			}
			case AttachmentType.Boundingbox:
			{
				BoundingBoxAttachment box = attachmentLoader.NewBoundingBoxAttachment(skin, name);
				if (box == null)
				{
					return null;
				}
				ReadVertices(map, box, GetInt(map, "vertexCount", 0) << 1);
				return box;
			}
			case AttachmentType.Mesh:
			case AttachmentType.Linkedmesh:
			{
				MeshAttachment mesh = attachmentLoader.NewMeshAttachment(skin, name, path);
				if (mesh == null)
				{
					return null;
				}
				mesh.Path = path;
				if (map.ContainsKey("color"))
				{
					string color2 = (string)map["color"];
					mesh.r = ToColor(color2, 0);
					mesh.g = ToColor(color2, 1);
					mesh.b = ToColor(color2, 2);
					mesh.a = ToColor(color2, 3);
				}
				mesh.Width = GetFloat(map, "width", 0f) * scale;
				mesh.Height = GetFloat(map, "height", 0f) * scale;
				string parent = GetString(map, "parent", null);
				if (parent != null)
				{
					linkedMeshes.Add(new LinkedMesh(mesh, GetString(map, "skin", null), slotIndex, parent, GetBoolean(map, "deform", true)));
					return mesh;
				}
				float[] uvs = GetFloatArray(map, "uvs", 1f);
				ReadVertices(map, mesh, uvs.Length);
				mesh.triangles = GetIntArray(map, "triangles");
				mesh.regionUVs = uvs;
				mesh.UpdateUVs();
				if (map.ContainsKey("hull"))
				{
					mesh.HullLength = GetInt(map, "hull", 0) * 2;
				}
				if (map.ContainsKey("edges"))
				{
					mesh.Edges = GetIntArray(map, "edges");
				}
				return mesh;
			}
			case AttachmentType.Path:
			{
				PathAttachment pathAttachment = attachmentLoader.NewPathAttachment(skin, name);
				if (pathAttachment == null)
				{
					return null;
				}
				pathAttachment.closed = GetBoolean(map, "closed", false);
				pathAttachment.constantSpeed = GetBoolean(map, "constantSpeed", true);
				int vertexCount = GetInt(map, "vertexCount", 0);
				ReadVertices(map, pathAttachment, vertexCount << 1);
				pathAttachment.lengths = GetFloatArray(map, "lengths", scale);
				return pathAttachment;
			}
			case AttachmentType.Point:
			{
				PointAttachment point = attachmentLoader.NewPointAttachment(skin, name);
				if (point == null)
				{
					return null;
				}
				point.x = GetFloat(map, "x", 0f) * scale;
				point.y = GetFloat(map, "y", 0f) * scale;
				point.rotation = GetFloat(map, "rotation", 0f);
				return point;
			}
			case AttachmentType.Clipping:
			{
				ClippingAttachment clip = attachmentLoader.NewClippingAttachment(skin, name);
				if (clip == null)
				{
					return null;
				}
				string end = GetString(map, "end", null);
				if (end != null)
				{
					SlotData slot = skeletonData.FindSlot(end);
					if (slot == null)
					{
						throw new Exception("Clipping end slot not found: " + end);
					}
					clip.EndSlot = slot;
				}
				ReadVertices(map, clip, GetInt(map, "vertexCount", 0) << 1);
				return clip;
			}
			default:
				return null;
			}
		}

		private void ReadVertices(Dictionary<string, object> map, VertexAttachment attachment, int verticesLength)
		{
			attachment.WorldVerticesLength = verticesLength;
			float[] vertices = GetFloatArray(map, "vertices", 1f);
			float scale = Scale;
			if (verticesLength == vertices.Length)
			{
				if (scale != 1f)
				{
					for (int j = 0; j < vertices.Length; j++)
					{
						vertices[j] *= scale;
					}
				}
				attachment.vertices = vertices;
				return;
			}
			ExposedList<float> weights = new ExposedList<float>(verticesLength * 3 * 3);
			ExposedList<int> bones = new ExposedList<int>(verticesLength * 3);
			int i = 0;
			int k = vertices.Length;
			while (i < k)
			{
				int boneCount = (int)vertices[i++];
				bones.Add(boneCount);
				for (int nn = i + boneCount * 4; i < nn; i += 4)
				{
					bones.Add((int)vertices[i]);
					weights.Add(vertices[i + 1] * Scale);
					weights.Add(vertices[i + 2] * Scale);
					weights.Add(vertices[i + 3]);
				}
			}
			attachment.bones = bones.ToArray();
			attachment.vertices = weights.ToArray();
		}

		private void ReadAnimation(Dictionary<string, object> map, string name, SkeletonData skeletonData)
		{
			float scale = Scale;
			ExposedList<Timeline> timelines = new ExposedList<Timeline>();
			float duration = 0f;
			if (map.ContainsKey("slots"))
			{
				foreach (KeyValuePair<string, object> entry in (Dictionary<string, object>)map["slots"])
				{
					string slotName = entry.Key;
					int slotIndex2 = skeletonData.FindSlotIndex(slotName);
					Dictionary<string, object> timelineMap = (Dictionary<string, object>)entry.Value;
					foreach (KeyValuePair<string, object> timelineEntry in timelineMap)
					{
						List<object> values2 = (List<object>)timelineEntry.Value;
						string timelineName = timelineEntry.Key;
						switch (timelineName)
						{
						case "attachment":
						{
							AttachmentTimeline timeline8 = new AttachmentTimeline(values2.Count);
							timeline8.slotIndex = slotIndex2;
							int frameIndex10 = 0;
							foreach (Dictionary<string, object> valueMap9 in values2)
							{
								float time4 = GetFloat(valueMap9, "time", 0f);
								timeline8.SetFrame(frameIndex10++, time4, (string)valueMap9["name"]);
							}
							timelines.Add(timeline8);
							duration = Math.Max(duration, timeline8.frames[timeline8.FrameCount - 1]);
							break;
						}
						case "color":
						{
							ColorTimeline timeline6 = new ColorTimeline(values2.Count);
							timeline6.slotIndex = slotIndex2;
							int frameIndex6 = 0;
							foreach (Dictionary<string, object> valueMap5 in values2)
							{
								float time2 = GetFloat(valueMap5, "time", 0f);
								string c = (string)valueMap5["color"];
								timeline6.SetFrame(frameIndex6, time2, ToColor(c, 0), ToColor(c, 1), ToColor(c, 2), ToColor(c, 3));
								ReadCurve(valueMap5, timeline6, frameIndex6);
								frameIndex6++;
							}
							timelines.Add(timeline6);
							duration = Math.Max(duration, timeline6.frames[(timeline6.FrameCount - 1) * 5]);
							break;
						}
						case "twoColor":
						{
							TwoColorTimeline timeline3 = new TwoColorTimeline(values2.Count);
							timeline3.slotIndex = slotIndex2;
							int frameIndex3 = 0;
							foreach (Dictionary<string, object> valueMap2 in values2)
							{
								float time = GetFloat(valueMap2, "time", 0f);
								string light = (string)valueMap2["light"];
								string dark = (string)valueMap2["dark"];
								timeline3.SetFrame(frameIndex3, time, ToColor(light, 0), ToColor(light, 1), ToColor(light, 2), ToColor(light, 3), ToColor(dark, 0, 6), ToColor(dark, 1, 6), ToColor(dark, 2, 6));
								ReadCurve(valueMap2, timeline3, frameIndex3);
								frameIndex3++;
							}
							timelines.Add(timeline3);
							duration = Math.Max(duration, timeline3.frames[(timeline3.FrameCount - 1) * 8]);
							break;
						}
						default:
							throw new Exception("Invalid timeline type for a slot: " + timelineName + " (" + slotName + ")");
						}
					}
				}
			}
			if (map.ContainsKey("bones"))
			{
				foreach (KeyValuePair<string, object> entry2 in (Dictionary<string, object>)map["bones"])
				{
					string boneName = entry2.Key;
					int boneIndex = skeletonData.FindBoneIndex(boneName);
					if (boneIndex == -1)
					{
						throw new Exception("Bone not found: " + boneName);
					}
					Dictionary<string, object> timelineMap4 = (Dictionary<string, object>)entry2.Value;
					foreach (KeyValuePair<string, object> timelineEntry3 in timelineMap4)
					{
						List<object> values7 = (List<object>)timelineEntry3.Value;
						string timelineName3 = timelineEntry3.Key;
						switch (timelineName3)
						{
						case "rotate":
						{
							RotateTimeline timeline11 = new RotateTimeline(values7.Count);
							timeline11.boneIndex = boneIndex;
							int frameIndex11 = 0;
							foreach (Dictionary<string, object> valueMap8 in values7)
							{
								timeline11.SetFrame(frameIndex11, GetFloat(valueMap8, "time", 0f), GetFloat(valueMap8, "angle", 0f));
								ReadCurve(valueMap8, timeline11, frameIndex11);
								frameIndex11++;
							}
							timelines.Add(timeline11);
							duration = Math.Max(duration, timeline11.frames[(timeline11.FrameCount - 1) * 2]);
							break;
						}
						default:
							if (!(timelineName3 == "shear"))
							{
								throw new Exception("Invalid timeline type for a bone: " + timelineName3 + " (" + boneName + ")");
							}
							goto case "translate";
						case "translate":
						case "scale":
						{
							float timelineScale2 = 1f;
							float defaultValue = 0f;
							TranslateTimeline timeline12;
							if (timelineName3 == "scale")
							{
								timeline12 = new ScaleTimeline(values7.Count);
								defaultValue = 1f;
							}
							else if (timelineName3 == "shear")
							{
								timeline12 = new ShearTimeline(values7.Count);
							}
							else
							{
								timeline12 = new TranslateTimeline(values7.Count);
								timelineScale2 = scale;
							}
							timeline12.boneIndex = boneIndex;
							int frameIndex12 = 0;
							foreach (Dictionary<string, object> valueMap10 in values7)
							{
								float time3 = GetFloat(valueMap10, "time", 0f);
								float x = GetFloat(valueMap10, "x", defaultValue);
								float y = GetFloat(valueMap10, "y", defaultValue);
								timeline12.SetFrame(frameIndex12, time3, x * timelineScale2, y * timelineScale2);
								ReadCurve(valueMap10, timeline12, frameIndex12);
								frameIndex12++;
							}
							timelines.Add(timeline12);
							duration = Math.Max(duration, timeline12.frames[(timeline12.FrameCount - 1) * 3]);
							break;
						}
						}
					}
				}
			}
			if (map.ContainsKey("ik"))
			{
				foreach (KeyValuePair<string, object> constraintMap3 in (Dictionary<string, object>)map["ik"])
				{
					IkConstraintData constraint2 = skeletonData.FindIkConstraint(constraintMap3.Key);
					List<object> values6 = (List<object>)constraintMap3.Value;
					IkConstraintTimeline timeline10 = new IkConstraintTimeline(values6.Count);
					timeline10.ikConstraintIndex = skeletonData.ikConstraints.IndexOf(constraint2);
					int frameIndex9 = 0;
					foreach (Dictionary<string, object> valueMap7 in values6)
					{
						timeline10.SetFrame(frameIndex9, GetFloat(valueMap7, "time", 0f), GetFloat(valueMap7, "mix", 1f), GetFloat(valueMap7, "softness", 0f) * scale, GetBoolean(valueMap7, "bendPositive", true) ? 1 : (-1), GetBoolean(valueMap7, "compress", false), GetBoolean(valueMap7, "stretch", false));
						ReadCurve(valueMap7, timeline10, frameIndex9);
						frameIndex9++;
					}
					timelines.Add(timeline10);
					duration = Math.Max(duration, timeline10.frames[(timeline10.FrameCount - 1) * 6]);
				}
			}
			if (map.ContainsKey("transform"))
			{
				foreach (KeyValuePair<string, object> constraintMap2 in (Dictionary<string, object>)map["transform"])
				{
					TransformConstraintData constraint = skeletonData.FindTransformConstraint(constraintMap2.Key);
					List<object> values5 = (List<object>)constraintMap2.Value;
					TransformConstraintTimeline timeline9 = new TransformConstraintTimeline(values5.Count);
					timeline9.transformConstraintIndex = skeletonData.transformConstraints.IndexOf(constraint);
					int frameIndex8 = 0;
					foreach (Dictionary<string, object> valueMap6 in values5)
					{
						timeline9.SetFrame(frameIndex8, GetFloat(valueMap6, "time", 0f), GetFloat(valueMap6, "rotateMix", 1f), GetFloat(valueMap6, "translateMix", 1f), GetFloat(valueMap6, "scaleMix", 1f), GetFloat(valueMap6, "shearMix", 1f));
						ReadCurve(valueMap6, timeline9, frameIndex8);
						frameIndex8++;
					}
					timelines.Add(timeline9);
					duration = Math.Max(duration, timeline9.frames[(timeline9.FrameCount - 1) * 5]);
				}
			}
			if (map.ContainsKey("path"))
			{
				foreach (KeyValuePair<string, object> constraintMap in (Dictionary<string, object>)map["path"])
				{
					int index2 = skeletonData.FindPathConstraintIndex(constraintMap.Key);
					if (index2 == -1)
					{
						throw new Exception("Path constraint not found: " + constraintMap.Key);
					}
					PathConstraintData data = skeletonData.pathConstraints.Items[index2];
					Dictionary<string, object> timelineMap3 = (Dictionary<string, object>)constraintMap.Value;
					foreach (KeyValuePair<string, object> timelineEntry2 in timelineMap3)
					{
						List<object> values4 = (List<object>)timelineEntry2.Value;
						string timelineName2 = timelineEntry2.Key;
						if (timelineName2 == "position" || timelineName2 == "spacing")
						{
							float timelineScale = 1f;
							PathConstraintPositionTimeline timeline5;
							if (timelineName2 == "spacing")
							{
								timeline5 = new PathConstraintSpacingTimeline(values4.Count);
								if (data.spacingMode == SpacingMode.Length || data.spacingMode == SpacingMode.Fixed)
								{
									timelineScale = scale;
								}
							}
							else
							{
								timeline5 = new PathConstraintPositionTimeline(values4.Count);
								if (data.positionMode == PositionMode.Fixed)
								{
									timelineScale = scale;
								}
							}
							timeline5.pathConstraintIndex = index2;
							int frameIndex5 = 0;
							foreach (Dictionary<string, object> valueMap3 in values4)
							{
								timeline5.SetFrame(frameIndex5, GetFloat(valueMap3, "time", 0f), GetFloat(valueMap3, timelineName2, 0f) * timelineScale);
								ReadCurve(valueMap3, timeline5, frameIndex5);
								frameIndex5++;
							}
							timelines.Add(timeline5);
							duration = Math.Max(duration, timeline5.frames[(timeline5.FrameCount - 1) * 2]);
						}
						else
						{
							if (!(timelineName2 == "mix"))
							{
								continue;
							}
							PathConstraintMixTimeline timeline7 = new PathConstraintMixTimeline(values4.Count);
							timeline7.pathConstraintIndex = index2;
							int frameIndex7 = 0;
							foreach (Dictionary<string, object> valueMap4 in values4)
							{
								timeline7.SetFrame(frameIndex7, GetFloat(valueMap4, "time", 0f), GetFloat(valueMap4, "rotateMix", 1f), GetFloat(valueMap4, "translateMix", 1f));
								ReadCurve(valueMap4, timeline7, frameIndex7);
								frameIndex7++;
							}
							timelines.Add(timeline7);
							duration = Math.Max(duration, timeline7.frames[(timeline7.FrameCount - 1) * 3]);
						}
					}
				}
			}
			if (map.ContainsKey("deform"))
			{
				foreach (KeyValuePair<string, object> deformMap in (Dictionary<string, object>)map["deform"])
				{
					Skin skin = skeletonData.FindSkin(deformMap.Key);
					foreach (KeyValuePair<string, object> slotMap in (Dictionary<string, object>)deformMap.Value)
					{
						int slotIndex3 = skeletonData.FindSlotIndex(slotMap.Key);
						if (slotIndex3 == -1)
						{
							throw new Exception("Slot not found: " + slotMap.Key);
						}
						foreach (KeyValuePair<string, object> timelineMap2 in (Dictionary<string, object>)slotMap.Value)
						{
							List<object> values3 = (List<object>)timelineMap2.Value;
							VertexAttachment attachment = (VertexAttachment)skin.GetAttachment(slotIndex3, timelineMap2.Key);
							if (attachment == null)
							{
								throw new Exception("Deform attachment not found: " + timelineMap2.Key);
							}
							bool weighted = attachment.bones != null;
							float[] vertices = attachment.vertices;
							int deformLength = (weighted ? (vertices.Length / 3 * 2) : vertices.Length);
							DeformTimeline timeline4 = new DeformTimeline(values3.Count);
							timeline4.slotIndex = slotIndex3;
							timeline4.attachment = attachment;
							int frameIndex4 = 0;
							foreach (Dictionary<string, object> valueMap in values3)
							{
								float[] deform;
								if (!valueMap.ContainsKey("vertices"))
								{
									deform = (weighted ? new float[deformLength] : vertices);
								}
								else
								{
									deform = new float[deformLength];
									int start = GetInt(valueMap, "offset", 0);
									float[] verticesValue = GetFloatArray(valueMap, "vertices", 1f);
									Array.Copy(verticesValue, 0, deform, start, verticesValue.Length);
									if (scale != 1f)
									{
										int l = start;
										for (int m = l + verticesValue.Length; l < m; l++)
										{
											deform[l] *= scale;
										}
									}
									if (!weighted)
									{
										for (int k = 0; k < deformLength; k++)
										{
											deform[k] += vertices[k];
										}
									}
								}
								timeline4.SetFrame(frameIndex4, GetFloat(valueMap, "time", 0f), deform);
								ReadCurve(valueMap, timeline4, frameIndex4);
								frameIndex4++;
							}
							timelines.Add(timeline4);
							duration = Math.Max(duration, timeline4.frames[timeline4.FrameCount - 1]);
						}
					}
				}
			}
			if (map.ContainsKey("drawOrder") || map.ContainsKey("draworder"))
			{
				List<object> values = (List<object>)map[map.ContainsKey("drawOrder") ? "drawOrder" : "draworder"];
				DrawOrderTimeline timeline2 = new DrawOrderTimeline(values.Count);
				int slotCount = skeletonData.slots.Count;
				int frameIndex2 = 0;
				foreach (Dictionary<string, object> drawOrderMap in values)
				{
					int[] drawOrder = null;
					if (drawOrderMap.ContainsKey("offsets"))
					{
						drawOrder = new int[slotCount];
						for (int j = slotCount - 1; j >= 0; j--)
						{
							drawOrder[j] = -1;
						}
						List<object> offsets = (List<object>)drawOrderMap["offsets"];
						int[] unchanged = new int[slotCount - offsets.Count];
						int originalIndex = 0;
						int unchangedIndex = 0;
						foreach (Dictionary<string, object> offsetMap in offsets)
						{
							int slotIndex = skeletonData.FindSlotIndex((string)offsetMap["slot"]);
							if (slotIndex == -1)
							{
								throw new Exception("Slot not found: " + offsetMap["slot"]);
							}
							while (originalIndex != slotIndex)
							{
								unchanged[unchangedIndex++] = originalIndex++;
							}
							int index = originalIndex + (int)(float)offsetMap["offset"];
							drawOrder[index] = originalIndex++;
						}
						while (originalIndex < slotCount)
						{
							unchanged[unchangedIndex++] = originalIndex++;
						}
						for (int i = slotCount - 1; i >= 0; i--)
						{
							if (drawOrder[i] == -1)
							{
								drawOrder[i] = unchanged[--unchangedIndex];
							}
						}
					}
					timeline2.SetFrame(frameIndex2++, GetFloat(drawOrderMap, "time", 0f), drawOrder);
				}
				timelines.Add(timeline2);
				duration = Math.Max(duration, timeline2.frames[timeline2.FrameCount - 1]);
			}
			if (map.ContainsKey("events"))
			{
				List<object> eventsMap = (List<object>)map["events"];
				EventTimeline timeline = new EventTimeline(eventsMap.Count);
				int frameIndex = 0;
				foreach (Dictionary<string, object> eventMap in eventsMap)
				{
					EventData eventData = skeletonData.FindEvent((string)eventMap["name"]);
					if (eventData == null)
					{
						throw new Exception("Event not found: " + eventMap["name"]);
					}
					Event e = new Event(GetFloat(eventMap, "time", 0f), eventData)
					{
						intValue = GetInt(eventMap, "int", eventData.Int),
						floatValue = GetFloat(eventMap, "float", eventData.Float),
						stringValue = GetString(eventMap, "string", eventData.String)
					};
					if (e.data.AudioPath != null)
					{
						e.volume = GetFloat(eventMap, "volume", eventData.Volume);
						e.balance = GetFloat(eventMap, "balance", eventData.Balance);
					}
					timeline.SetFrame(frameIndex++, e);
				}
				timelines.Add(timeline);
				duration = Math.Max(duration, timeline.frames[timeline.FrameCount - 1]);
			}
			timelines.TrimExcess();
			skeletonData.animations.Add(new Animation(name, timelines, duration));
		}

		private static void ReadCurve(Dictionary<string, object> valueMap, CurveTimeline timeline, int frameIndex)
		{
			if (valueMap.ContainsKey("curve"))
			{
				object curveObject = valueMap["curve"];
				if (curveObject is string)
				{
					timeline.SetStepped(frameIndex);
				}
				else
				{
					timeline.SetCurve(frameIndex, (float)curveObject, GetFloat(valueMap, "c2", 0f), GetFloat(valueMap, "c3", 1f), GetFloat(valueMap, "c4", 1f));
				}
			}
		}

		private static float[] GetFloatArray(Dictionary<string, object> map, string name, float scale)
		{
			List<object> list = (List<object>)map[name];
			float[] values = new float[list.Count];
			if (scale == 1f)
			{
				int j = 0;
				for (int l = list.Count; j < l; j++)
				{
					values[j] = (float)list[j];
				}
			}
			else
			{
				int i = 0;
				for (int k = list.Count; i < k; i++)
				{
					values[i] = (float)list[i] * scale;
				}
			}
			return values;
		}

		private static int[] GetIntArray(Dictionary<string, object> map, string name)
		{
			List<object> list = (List<object>)map[name];
			int[] values = new int[list.Count];
			int i = 0;
			for (int j = list.Count; i < j; i++)
			{
				values[i] = (int)(float)list[i];
			}
			return values;
		}

		private static float GetFloat(Dictionary<string, object> map, string name, float defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (float)map[name];
		}

		private static int GetInt(Dictionary<string, object> map, string name, int defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (int)(float)map[name];
		}

		private static bool GetBoolean(Dictionary<string, object> map, string name, bool defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (bool)map[name];
		}

		private static string GetString(Dictionary<string, object> map, string name, string defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (string)map[name];
		}

		private static float ToColor(string hexString, int colorIndex, int expectedLength = 8)
		{
			if (hexString.Length != expectedLength)
			{
				throw new ArgumentException("Color hexidecimal length must be " + expectedLength + ", recieved: " + hexString, "hexString");
			}
			return (float)Convert.ToInt32(hexString.Substring(colorIndex * 2, 2), 16) / 255f;
		}
	}
}
