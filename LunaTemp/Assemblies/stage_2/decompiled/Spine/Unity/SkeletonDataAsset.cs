using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Spine.Unity
{
	[CreateAssetMenu(fileName = "New SkeletonDataAsset", menuName = "Spine/SkeletonData Asset")]
	public class SkeletonDataAsset : ScriptableObject
	{
		public AtlasAssetBase[] atlasAssets = new AtlasAssetBase[0];

		public float scale = 0.01f;

		public TextAsset skeletonJSON;

		public bool isUpgradingBlendModeMaterials = false;

		public BlendModeMaterials blendModeMaterials = new BlendModeMaterials();

		[Tooltip("Use SkeletonDataModifierAssets to apply changes to the SkeletonData after being loaded, such as apply blend mode Materials to Attachments under slots with special blend modes.")]
		public List<SkeletonDataModifierAsset> skeletonDataModifiers = new List<SkeletonDataModifierAsset>();

		[SpineAnimation("", "", false, false)]
		public string[] fromAnimation = new string[0];

		[SpineAnimation("", "", false, false)]
		public string[] toAnimation = new string[0];

		public float[] duration = new float[0];

		public float defaultMix;

		public RuntimeAnimatorController controller;

		private SkeletonData skeletonData;

		private AnimationStateData stateData;

		public bool IsLoaded => skeletonData != null;

		private void Reset()
		{
			Clear();
		}

		public static SkeletonDataAsset CreateRuntimeInstance(TextAsset skeletonDataFile, AtlasAssetBase atlasAsset, bool initialize, float scale = 0.01f)
		{
			return CreateRuntimeInstance(skeletonDataFile, new AtlasAssetBase[1] { atlasAsset }, initialize, scale);
		}

		public static SkeletonDataAsset CreateRuntimeInstance(TextAsset skeletonDataFile, AtlasAssetBase[] atlasAssets, bool initialize, float scale = 0.01f)
		{
			SkeletonDataAsset skeletonDataAsset = ScriptableObject.CreateInstance<SkeletonDataAsset>();
			skeletonDataAsset.Clear();
			skeletonDataAsset.skeletonJSON = skeletonDataFile;
			skeletonDataAsset.atlasAssets = atlasAssets;
			skeletonDataAsset.scale = scale;
			if (initialize)
			{
				skeletonDataAsset.GetSkeletonData(true);
			}
			return skeletonDataAsset;
		}

		public void Clear()
		{
			skeletonData = null;
			stateData = null;
		}

		public AnimationStateData GetAnimationStateData()
		{
			if (stateData != null)
			{
				return stateData;
			}
			GetSkeletonData(false);
			return stateData;
		}

		public SkeletonData GetSkeletonData(bool quiet)
		{
			if (skeletonJSON == null)
			{
				if (!quiet)
				{
					Debug.LogError("Skeleton JSON file not set for SkeletonData asset: " + base.name, this);
				}
				Clear();
				return null;
			}
			if (skeletonData != null)
			{
				return skeletonData;
			}
			Atlas[] atlasArray = GetAtlasArray();
			AttachmentLoader attachmentLoader3;
			if (atlasArray.Length != 0)
			{
				AttachmentLoader attachmentLoader2 = new AtlasAttachmentLoader(atlasArray);
				attachmentLoader3 = attachmentLoader2;
			}
			else
			{
				AttachmentLoader attachmentLoader2 = new RegionlessAttachmentLoader();
				attachmentLoader3 = attachmentLoader2;
			}
			AttachmentLoader attachmentLoader = attachmentLoader3;
			float skeletonDataScale = scale;
			bool hasBinaryExtension = skeletonJSON.name.ToLower().Contains(".skel");
			SkeletonData loadedSkeletonData = null;
			try
			{
				loadedSkeletonData = ((!hasBinaryExtension) ? ReadSkeletonData(skeletonJSON.text, attachmentLoader, skeletonDataScale) : ReadSkeletonData(skeletonJSON.bytes, attachmentLoader, skeletonDataScale));
			}
			catch (Exception ex)
			{
				if (!quiet)
				{
					Debug.LogError("Error reading skeleton JSON file for SkeletonData asset: " + base.name + "\n" + ex.Message + "\n" + ex.StackTrace, skeletonJSON);
				}
			}
			if (loadedSkeletonData == null)
			{
				return null;
			}
			if (skeletonDataModifiers != null)
			{
				foreach (SkeletonDataModifierAsset modifier in skeletonDataModifiers)
				{
					if (modifier != null && (!isUpgradingBlendModeMaterials || !(modifier is BlendModeMaterialsAsset)))
					{
						modifier.Apply(loadedSkeletonData);
					}
				}
			}
			if (!isUpgradingBlendModeMaterials)
			{
				blendModeMaterials.ApplyMaterials(loadedSkeletonData);
			}
			InitializeWithData(loadedSkeletonData);
			return skeletonData;
		}

		internal void InitializeWithData(SkeletonData sd)
		{
			skeletonData = sd;
			stateData = new AnimationStateData(skeletonData);
			FillStateData();
		}

		public void FillStateData()
		{
			if (stateData == null)
			{
				return;
			}
			stateData.defaultMix = defaultMix;
			int i = 0;
			for (int j = fromAnimation.Length; i < j; i++)
			{
				if (fromAnimation[i].Length != 0 && toAnimation[i].Length != 0)
				{
					stateData.SetMix(fromAnimation[i], toAnimation[i], duration[i]);
				}
			}
		}

		internal Atlas[] GetAtlasArray()
		{
			List<Atlas> returnList = new List<Atlas>(atlasAssets.Length);
			for (int i = 0; i < atlasAssets.Length; i++)
			{
				AtlasAssetBase aa = atlasAssets[i];
				if (!(aa == null))
				{
					Atlas a = aa.GetAtlas();
					if (a != null)
					{
						returnList.Add(a);
					}
				}
			}
			return returnList.ToArray();
		}

		internal static SkeletonData ReadSkeletonData(byte[] bytes, AttachmentLoader attachmentLoader, float scale)
		{
			using (MemoryStream input = new MemoryStream(bytes))
			{
				SkeletonBinary binary = new SkeletonBinary(attachmentLoader)
				{
					Scale = scale
				};
				return binary.ReadSkeletonData(input);
			}
		}

		internal static SkeletonData ReadSkeletonData(string text, AttachmentLoader attachmentLoader, float scale)
		{
			StringReader input = new StringReader(text);
			SkeletonJson json = new SkeletonJson(attachmentLoader)
			{
				Scale = scale
			};
			return json.ReadSkeletonData(input);
		}
	}
}
