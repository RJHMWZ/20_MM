using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Spine.Unity
{
	[ExecuteAlways]
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonRenderSeparator")]
	public class SkeletonRenderSeparator : MonoBehaviour
	{
		public const int DefaultSortingOrderIncrement = 5;

		[SerializeField]
		protected SkeletonRenderer skeletonRenderer;

		private MeshRenderer mainMeshRenderer;

		public bool copyPropertyBlock = true;

		[Tooltip("Copies MeshRenderer flags into each parts renderer")]
		public bool copyMeshRendererFlags = true;

		public List<SkeletonPartsRenderer> partsRenderers = new List<SkeletonPartsRenderer>();

		private MaterialPropertyBlock copiedBlock;

		public SkeletonRenderer SkeletonRenderer
		{
			get
			{
				return skeletonRenderer;
			}
			set
			{
				if (skeletonRenderer != null)
				{
					skeletonRenderer.GenerateMeshOverride -= HandleRender;
				}
				skeletonRenderer = value;
				if (value == null)
				{
					base.enabled = false;
				}
			}
		}

		public event SkeletonRenderer.SkeletonRendererDelegate OnMeshAndMaterialsUpdated;

		public static SkeletonRenderSeparator AddToSkeletonRenderer(SkeletonRenderer skeletonRenderer, int sortingLayerID = 0, int extraPartsRenderers = 0, int sortingOrderIncrement = 5, int baseSortingOrder = 0, bool addMinimumPartsRenderers = true)
		{
			if (skeletonRenderer == null)
			{
				Debug.Log("Tried to add SkeletonRenderSeparator to a null SkeletonRenderer reference.");
				return null;
			}
			SkeletonRenderSeparator srs = skeletonRenderer.gameObject.AddComponent<SkeletonRenderSeparator>();
			srs.skeletonRenderer = skeletonRenderer;
			skeletonRenderer.Initialize(false);
			int count = extraPartsRenderers;
			if (addMinimumPartsRenderers)
			{
				count = extraPartsRenderers + skeletonRenderer.separatorSlots.Count + 1;
			}
			Transform skeletonRendererTransform = skeletonRenderer.transform;
			List<SkeletonPartsRenderer> componentRenderers = srs.partsRenderers;
			for (int i = 0; i < count; i++)
			{
				SkeletonPartsRenderer spr = SkeletonPartsRenderer.NewPartsRendererGameObject(skeletonRendererTransform, i.ToString());
				MeshRenderer mr = spr.MeshRenderer;
				mr.sortingLayerID = sortingLayerID;
				mr.sortingOrder = baseSortingOrder + i * sortingOrderIncrement;
				componentRenderers.Add(spr);
			}
			srs.OnEnable();
			return srs;
		}

		public SkeletonPartsRenderer AddPartsRenderer(int sortingOrderIncrement = 5, string name = null)
		{
			int sortingLayerID = 0;
			int sortingOrder = 0;
			if (partsRenderers.Count > 0)
			{
				SkeletonPartsRenderer previous = partsRenderers[partsRenderers.Count - 1];
				MeshRenderer previousMeshRenderer = previous.MeshRenderer;
				sortingLayerID = previousMeshRenderer.sortingLayerID;
				sortingOrder = previousMeshRenderer.sortingOrder + sortingOrderIncrement;
			}
			if (string.IsNullOrEmpty(name))
			{
				name = partsRenderers.Count.ToString();
			}
			SkeletonPartsRenderer spr = SkeletonPartsRenderer.NewPartsRendererGameObject(skeletonRenderer.transform, name);
			partsRenderers.Add(spr);
			MeshRenderer mr = spr.MeshRenderer;
			mr.sortingLayerID = sortingLayerID;
			mr.sortingOrder = sortingOrder;
			return spr;
		}

		public void OnEnable()
		{
			if (skeletonRenderer == null)
			{
				return;
			}
			if (copiedBlock == null)
			{
				copiedBlock = new MaterialPropertyBlock();
			}
			mainMeshRenderer = skeletonRenderer.GetComponent<MeshRenderer>();
			skeletonRenderer.GenerateMeshOverride -= HandleRender;
			skeletonRenderer.GenerateMeshOverride += HandleRender;
			if (!copyMeshRendererFlags)
			{
				return;
			}
			LightProbeUsage lightProbeUsage = mainMeshRenderer.lightProbeUsage;
			bool receiveShadows = mainMeshRenderer.receiveShadows;
			ReflectionProbeUsage reflectionProbeUsage = mainMeshRenderer.reflectionProbeUsage;
			ShadowCastingMode shadowCastingMode = mainMeshRenderer.shadowCastingMode;
			MotionVectorGenerationMode motionVectorGenerationMode = mainMeshRenderer.motionVectorGenerationMode;
			Transform probeAnchor = mainMeshRenderer.probeAnchor;
			for (int i = 0; i < partsRenderers.Count; i++)
			{
				SkeletonPartsRenderer currentRenderer = partsRenderers[i];
				if (!(currentRenderer == null))
				{
					MeshRenderer mr = currentRenderer.MeshRenderer;
					mr.lightProbeUsage = lightProbeUsage;
					mr.receiveShadows = receiveShadows;
					mr.reflectionProbeUsage = reflectionProbeUsage;
					mr.shadowCastingMode = shadowCastingMode;
					mr.motionVectorGenerationMode = motionVectorGenerationMode;
					mr.probeAnchor = probeAnchor;
				}
			}
		}

		public void OnDisable()
		{
			if (skeletonRenderer == null)
			{
				return;
			}
			skeletonRenderer.GenerateMeshOverride -= HandleRender;
			skeletonRenderer.LateUpdate();
			foreach (SkeletonPartsRenderer partsRenderer in partsRenderers)
			{
				if (partsRenderer != null)
				{
					partsRenderer.ClearMesh();
				}
			}
		}

		private void HandleRender(SkeletonRendererInstruction instruction)
		{
			int rendererCount = partsRenderers.Count;
			if (rendererCount <= 0)
			{
				return;
			}
			if (copyPropertyBlock)
			{
				mainMeshRenderer.GetPropertyBlock(copiedBlock);
			}
			MeshGenerator.Settings settings2 = default(MeshGenerator.Settings);
			settings2.addNormals = skeletonRenderer.addNormals;
			settings2.calculateTangents = skeletonRenderer.calculateTangents;
			settings2.immutableTriangles = false;
			settings2.pmaVertexColors = skeletonRenderer.pmaVertexColors;
			settings2.tintBlack = skeletonRenderer.tintBlack;
			settings2.useClipping = true;
			settings2.zSpacing = skeletonRenderer.zSpacing;
			MeshGenerator.Settings settings = settings2;
			ExposedList<SubmeshInstruction> submeshInstructions = instruction.submeshInstructions;
			SubmeshInstruction[] submeshInstructionsItems = submeshInstructions.Items;
			int lastSubmeshInstruction = submeshInstructions.Count - 1;
			int rendererIndex = 0;
			SkeletonPartsRenderer currentRenderer = partsRenderers[rendererIndex];
			int si = 0;
			int start = 0;
			for (; si <= lastSubmeshInstruction; si++)
			{
				if (!(currentRenderer == null) && (submeshInstructionsItems[si].forceSeparate || si == lastSubmeshInstruction))
				{
					MeshGenerator meshGenerator = currentRenderer.MeshGenerator;
					meshGenerator.settings = settings;
					if (copyPropertyBlock)
					{
						currentRenderer.SetPropertyBlock(copiedBlock);
					}
					currentRenderer.RenderParts(instruction.submeshInstructions, start, si + 1);
					start = si + 1;
					rendererIndex++;
					if (rendererIndex >= rendererCount)
					{
						break;
					}
					currentRenderer = partsRenderers[rendererIndex];
				}
			}
			if (this.OnMeshAndMaterialsUpdated != null)
			{
				this.OnMeshAndMaterialsUpdated(skeletonRenderer);
			}
			for (; rendererIndex < rendererCount; rendererIndex++)
			{
				currentRenderer = partsRenderers[rendererIndex];
				if (currentRenderer != null)
				{
					partsRenderers[rendererIndex].ClearMesh();
				}
			}
		}
	}
}
