using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spine.Unity
{
	[ExecuteAlways]
	[RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
	[DisallowMultipleComponent]
	[AddComponentMenu("Spine/SkeletonGraphic (Unity UI Canvas)")]
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonGraphic-Component")]
	public class SkeletonGraphic : MaskableGraphic, ISkeletonComponent, IAnimationStateComponent, ISkeletonAnimation, IHasSkeletonDataAsset
	{
		public delegate void SkeletonRendererDelegate(SkeletonGraphic skeletonGraphic);

		public SkeletonDataAsset skeletonDataAsset;

		[SpineSkin("", "skeletonDataAsset", true, false, true)]
		public string initialSkinName;

		public bool initialFlipX;

		public bool initialFlipY;

		[SpineAnimation("", "skeletonDataAsset", true, false)]
		public string startingAnimation;

		public bool startingLoop;

		public float timeScale = 1f;

		public bool freeze;

		protected UpdateMode updateMode = UpdateMode.FullUpdate;

		public UpdateMode updateWhenInvisible = UpdateMode.FullUpdate;

		public bool unscaledTime;

		public bool allowMultipleCanvasRenderers = false;

		public List<CanvasRenderer> canvasRenderers = new List<CanvasRenderer>();

		protected List<RawImage> rawImages = new List<RawImage>();

		protected int usedRenderersCount = 0;

		public const string SeparatorPartGameObjectName = "Part";

		[SerializeField]
		[SpineSlot("", "", false, true, false)]
		protected string[] separatorSlotNames = new string[0];

		[NonSerialized]
		public readonly List<Slot> separatorSlots = new List<Slot>();

		public bool enableSeparatorSlots = false;

		[SerializeField]
		protected List<Transform> separatorParts = new List<Transform>();

		public bool updateSeparatorPartLocation = true;

		private bool wasUpdatedAfterInit = true;

		private Texture baseTexture = null;

		[NonSerialized]
		private readonly Dictionary<Texture, Texture> customTextureOverride = new Dictionary<Texture, Texture>();

		[NonSerialized]
		private readonly Dictionary<Texture, Material> customMaterialOverride = new Dictionary<Texture, Material>();

		private Texture overrideTexture;

		protected Skeleton skeleton;

		protected AnimationState state;

		[SerializeField]
		protected MeshGenerator meshGenerator = new MeshGenerator();

		private DoubleBuffered<MeshRendererBuffers.SmartMesh> meshBuffers;

		private SkeletonRendererInstruction currentInstructions = new SkeletonRendererInstruction();

		private readonly ExposedList<Mesh> meshes = new ExposedList<Mesh>();

		public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

		public UpdateMode UpdateMode
		{
			get
			{
				return updateMode;
			}
			set
			{
				updateMode = value;
			}
		}

		public List<Transform> SeparatorParts => separatorParts;

		public Dictionary<Texture, Texture> CustomTextureOverride => customTextureOverride;

		public Dictionary<Texture, Material> CustomMaterialOverride => customMaterialOverride;

		public Texture OverrideTexture
		{
			get
			{
				return overrideTexture;
			}
			set
			{
				overrideTexture = value;
				base.canvasRenderer.SetTexture(mainTexture);
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (overrideTexture != null)
				{
					return overrideTexture;
				}
				return baseTexture;
			}
		}

		public Skeleton Skeleton
		{
			get
			{
				Initialize(false);
				return skeleton;
			}
			set
			{
				skeleton = value;
			}
		}

		public SkeletonData SkeletonData => (skeleton == null) ? null : skeleton.data;

		public bool IsValid => skeleton != null;

		public AnimationState AnimationState
		{
			get
			{
				Initialize(false);
				return state;
			}
		}

		public MeshGenerator MeshGenerator => meshGenerator;

		public event SkeletonRendererDelegate OnRebuild;

		public event SkeletonRendererDelegate OnMeshAndMaterialsUpdated;

		public event UpdateBonesDelegate BeforeApply;

		public event UpdateBonesDelegate UpdateLocal;

		public event UpdateBonesDelegate UpdateWorld;

		public event UpdateBonesDelegate UpdateComplete;

		public event MeshGeneratorDelegate OnPostProcessVertices;

		public static SkeletonGraphic NewSkeletonGraphicGameObject(SkeletonDataAsset skeletonDataAsset, Transform parent, Material material)
		{
			SkeletonGraphic sg = AddSkeletonGraphicComponent(new GameObject("New Spine GameObject"), skeletonDataAsset, material);
			if (parent != null)
			{
				sg.transform.SetParent(parent, false);
			}
			return sg;
		}

		public static SkeletonGraphic AddSkeletonGraphicComponent(GameObject gameObject, SkeletonDataAsset skeletonDataAsset, Material material)
		{
			SkeletonGraphic c = gameObject.AddComponent<SkeletonGraphic>();
			if (skeletonDataAsset != null)
			{
				c.material = material;
				c.skeletonDataAsset = skeletonDataAsset;
				c.Initialize(false);
			}
			return c;
		}

		protected override void Awake()
		{
			base.Awake();
			base.onCullStateChanged.AddListener(OnCullStateChanged);
			SyncRawImagesWithCanvasRenderers();
			if (!IsValid)
			{
				Initialize(false);
				Rebuild(CanvasUpdate.PreRender);
			}
		}

		protected override void OnDestroy()
		{
			Clear();
			base.OnDestroy();
		}

		public override void Rebuild(CanvasUpdate update)
		{
			base.Rebuild(update);
			if (!base.canvasRenderer.cull)
			{
				if (update == CanvasUpdate.PreRender)
				{
					UpdateMesh(true);
				}
				if (allowMultipleCanvasRenderers)
				{
					base.canvasRenderer.Clear();
				}
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			foreach (CanvasRenderer canvasRenderer in canvasRenderers)
			{
				canvasRenderer.Clear();
			}
		}

		public virtual void Update()
		{
			if (!freeze)
			{
				Update(unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
			}
		}

		public virtual void Update(float deltaTime)
		{
			if (!IsValid)
			{
				return;
			}
			wasUpdatedAfterInit = true;
			if (updateMode >= UpdateMode.OnlyAnimationStatus)
			{
				UpdateAnimationStatus(deltaTime);
				if (updateMode != UpdateMode.OnlyAnimationStatus)
				{
					ApplyAnimation();
				}
			}
		}

		protected void SyncRawImagesWithCanvasRenderers()
		{
			rawImages.Clear();
			foreach (CanvasRenderer canvasRenderer in canvasRenderers)
			{
				RawImage rawImage = canvasRenderer.GetComponent<RawImage>();
				if (rawImage == null)
				{
					rawImage = canvasRenderer.gameObject.AddComponent<RawImage>();
					rawImage.maskable = base.maskable;
					rawImage.raycastTarget = false;
				}
				rawImages.Add(rawImage);
			}
		}

		protected void UpdateAnimationStatus(float deltaTime)
		{
			deltaTime *= timeScale;
			skeleton.Update(deltaTime);
			state.Update(deltaTime);
		}

		protected void ApplyAnimation()
		{
			if (this.BeforeApply != null)
			{
				this.BeforeApply(this);
			}
			if (updateMode != UpdateMode.OnlyEventTimelines)
			{
				state.Apply(skeleton);
			}
			else
			{
				state.ApplyEventTimelinesOnly(skeleton);
			}
			if (this.UpdateLocal != null)
			{
				this.UpdateLocal(this);
			}
			skeleton.UpdateWorldTransform();
			if (this.UpdateWorld != null)
			{
				this.UpdateWorld(this);
				skeleton.UpdateWorldTransform();
			}
			if (this.UpdateComplete != null)
			{
				this.UpdateComplete(this);
			}
		}

		public void LateUpdate()
		{
			if (!wasUpdatedAfterInit)
			{
				Update(0f);
			}
			if (!freeze && updateMode == UpdateMode.FullUpdate)
			{
				UpdateMesh();
			}
		}

		protected void OnCullStateChanged(bool culled)
		{
			if (culled)
			{
				OnBecameInvisible();
			}
			else
			{
				OnBecameVisible();
			}
		}

		public void OnBecameVisible()
		{
			updateMode = UpdateMode.FullUpdate;
		}

		public void OnBecameInvisible()
		{
			updateMode = updateWhenInvisible;
		}

		public void ReapplySeparatorSlotNames()
		{
			if (!IsValid)
			{
				return;
			}
			separatorSlots.Clear();
			int i = 0;
			for (int j = separatorSlotNames.Length; i < j; i++)
			{
				string slotName = separatorSlotNames[i];
				if (!(slotName == ""))
				{
					Slot slot = skeleton.FindSlot(slotName);
					if (slot != null)
					{
						separatorSlots.Add(slot);
					}
				}
			}
			UpdateSeparatorPartParents();
		}

		public Mesh GetLastMesh()
		{
			return meshBuffers.GetCurrent().mesh;
		}

		public bool MatchRectTransformWithBounds()
		{
			UpdateMesh();
			if (!allowMultipleCanvasRenderers)
			{
				return MatchRectTransformSingleRenderer();
			}
			return MatchRectTransformMultipleRenderers();
		}

		protected bool MatchRectTransformSingleRenderer()
		{
			Mesh mesh = GetLastMesh();
			if (mesh == null)
			{
				return false;
			}
			if (mesh.vertexCount == 0)
			{
				base.rectTransform.sizeDelta = new Vector2(50f, 50f);
				base.rectTransform.pivot = new Vector2(0.5f, 0.5f);
				return false;
			}
			mesh.RecalculateBounds();
			SetRectTransformBounds(mesh.bounds);
			return true;
		}

		protected bool MatchRectTransformMultipleRenderers()
		{
			bool anyBoundsAdded = false;
			Bounds combinedBounds = default(Bounds);
			for (int i = 0; i < canvasRenderers.Count; i++)
			{
				CanvasRenderer canvasRenderer = canvasRenderers[i];
				if (!canvasRenderer.gameObject.activeSelf)
				{
					continue;
				}
				Mesh mesh = meshes.Items[i];
				if (!(mesh == null) && mesh.vertexCount != 0)
				{
					mesh.RecalculateBounds();
					Bounds bounds = mesh.bounds;
					if (anyBoundsAdded)
					{
						combinedBounds.Encapsulate(bounds);
						continue;
					}
					anyBoundsAdded = true;
					combinedBounds = bounds;
				}
			}
			if (!anyBoundsAdded)
			{
				base.rectTransform.sizeDelta = new Vector2(50f, 50f);
				base.rectTransform.pivot = new Vector2(0.5f, 0.5f);
				return false;
			}
			SetRectTransformBounds(combinedBounds);
			return true;
		}

		private void SetRectTransformBounds(Bounds combinedBounds)
		{
			Vector3 size = combinedBounds.size;
			Vector3 center = combinedBounds.center;
			Vector2 p = new Vector2(0.5f - center.x / size.x, 0.5f - center.y / size.y);
			base.rectTransform.sizeDelta = size;
			base.rectTransform.pivot = p;
		}

		public void Clear()
		{
			skeleton = null;
			base.canvasRenderer.Clear();
			for (int i = 0; i < canvasRenderers.Count; i++)
			{
				canvasRenderers[i].Clear();
			}
			DestroyMeshes();
			DisposeMeshBuffers();
		}

		public void TrimRenderers()
		{
			List<CanvasRenderer> newList = new List<CanvasRenderer>();
			foreach (CanvasRenderer canvasRenderer in canvasRenderers)
			{
				if (canvasRenderer.gameObject.activeSelf)
				{
					newList.Add(canvasRenderer);
				}
				else if (Application.isEditor && !Application.isPlaying)
				{
					UnityEngine.Object.DestroyImmediate(canvasRenderer.gameObject);
				}
				else
				{
					UnityEngine.Object.Destroy(canvasRenderer.gameObject);
				}
			}
			canvasRenderers = newList;
			SyncRawImagesWithCanvasRenderers();
		}

		public void Initialize(bool overwrite)
		{
			if ((IsValid && !overwrite) || skeletonDataAsset == null)
			{
				return;
			}
			SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(false);
			if (skeletonData == null || skeletonDataAsset.atlasAssets.Length == 0 || skeletonDataAsset.atlasAssets[0].MaterialCount <= 0)
			{
				return;
			}
			state = new AnimationState(skeletonDataAsset.GetAnimationStateData());
			if (state == null)
			{
				Clear();
				return;
			}
			skeleton = new Skeleton(skeletonData)
			{
				ScaleX = ((!initialFlipX) ? 1 : (-1)),
				ScaleY = ((!initialFlipY) ? 1 : (-1))
			};
			InitMeshBuffers();
			baseTexture = skeletonDataAsset.atlasAssets[0].PrimaryMaterial.mainTexture;
			base.canvasRenderer.SetTexture(mainTexture);
			if (!string.IsNullOrEmpty(initialSkinName))
			{
				skeleton.SetSkin(initialSkinName);
			}
			separatorSlots.Clear();
			for (int i = 0; i < separatorSlotNames.Length; i++)
			{
				separatorSlots.Add(skeleton.FindSlot(separatorSlotNames[i]));
			}
			wasUpdatedAfterInit = false;
			if (!string.IsNullOrEmpty(startingAnimation))
			{
				Animation animationObject = skeletonDataAsset.GetSkeletonData(false).FindAnimation(startingAnimation);
				if (animationObject != null)
				{
					state.SetAnimation(0, animationObject, startingLoop);
				}
			}
			if (this.OnRebuild != null)
			{
				this.OnRebuild(this);
			}
		}

		public void UpdateMesh(bool keepRendererCount = false)
		{
			if (IsValid)
			{
				skeleton.SetColor(color);
				SkeletonRendererInstruction currentInstructions = this.currentInstructions;
				if (!allowMultipleCanvasRenderers)
				{
					UpdateMeshSingleCanvasRenderer();
				}
				else
				{
					UpdateMeshMultipleCanvasRenderers(currentInstructions, keepRendererCount);
				}
				if (this.OnMeshAndMaterialsUpdated != null)
				{
					this.OnMeshAndMaterialsUpdated(this);
				}
			}
		}

		public bool HasMultipleSubmeshInstructions()
		{
			if (!IsValid)
			{
				return false;
			}
			return MeshGenerator.RequiresMultipleSubmeshesByDrawOrder(skeleton);
		}

		protected void InitMeshBuffers()
		{
			if (meshBuffers != null)
			{
				meshBuffers.GetNext().Clear();
				meshBuffers.GetNext().Clear();
			}
			else
			{
				meshBuffers = new DoubleBuffered<MeshRendererBuffers.SmartMesh>();
			}
		}

		protected void DisposeMeshBuffers()
		{
			if (meshBuffers != null)
			{
				meshBuffers.GetNext().Dispose();
				meshBuffers.GetNext().Dispose();
				meshBuffers = null;
			}
		}

		protected void UpdateMeshSingleCanvasRenderer()
		{
			if (canvasRenderers.Count > 0)
			{
				DisableUnusedCanvasRenderers(0);
			}
			MeshRendererBuffers.SmartMesh smartMesh = meshBuffers.GetNext();
			MeshGenerator.GenerateSingleSubmeshInstruction(currentInstructions, skeleton, null);
			bool updateTriangles = SkeletonRendererInstruction.GeometryNotEqual(currentInstructions, smartMesh.instructionUsed);
			meshGenerator.Begin();
			if (currentInstructions.hasActiveClipping && currentInstructions.submeshInstructions.Count > 0)
			{
				meshGenerator.AddSubmesh(currentInstructions.submeshInstructions.Items[0], updateTriangles);
			}
			else
			{
				meshGenerator.BuildMeshWithArrays(currentInstructions, updateTriangles);
			}
			if (base.canvas != null)
			{
				meshGenerator.ScaleVertexData(base.canvas.referencePixelsPerUnit);
			}
			if (this.OnPostProcessVertices != null)
			{
				this.OnPostProcessVertices(meshGenerator.Buffers);
			}
			Mesh mesh = smartMesh.mesh;
			meshGenerator.FillVertexData(mesh);
			if (updateTriangles)
			{
				meshGenerator.FillTriangles(mesh);
			}
			meshGenerator.FillLateVertexData(mesh);
			base.canvasRenderer.SetMesh(mesh);
			smartMesh.instructionUsed.Set(currentInstructions);
			if (currentInstructions.submeshInstructions.Count > 0)
			{
				Material material = currentInstructions.submeshInstructions.Items[0].material;
				if (material != null && baseTexture != material.mainTexture)
				{
					baseTexture = material.mainTexture;
					if (overrideTexture == null)
					{
						base.canvasRenderer.SetTexture(mainTexture);
					}
				}
			}
			usedRenderersCount = 0;
		}

		protected void UpdateMeshMultipleCanvasRenderers(SkeletonRendererInstruction currentInstructions, bool keepRendererCount)
		{
			MeshGenerator.GenerateSkeletonRendererInstruction(currentInstructions, skeleton, null, enableSeparatorSlots ? separatorSlots : null, enableSeparatorSlots && separatorSlots.Count > 0);
			int submeshCount = currentInstructions.submeshInstructions.Count;
			if (keepRendererCount && submeshCount != usedRenderersCount)
			{
				return;
			}
			EnsureCanvasRendererCount(submeshCount);
			EnsureMeshesCount(submeshCount);
			EnsureSeparatorPartCount();
			Canvas c = base.canvas;
			float scale = ((c == null) ? 100f : c.referencePixelsPerUnit);
			Mesh[] meshesItems = meshes.Items;
			bool useOriginalTextureAndMaterial = customMaterialOverride.Count == 0 && customTextureOverride.Count == 0;
			int separatorSlotGroupIndex = 0;
			Transform parent = ((separatorSlots.Count == 0) ? base.transform : separatorParts[0]);
			if (updateSeparatorPartLocation)
			{
				for (int p = 0; p < separatorParts.Count; p++)
				{
					separatorParts[p].position = base.transform.position;
					separatorParts[p].rotation = base.transform.rotation;
				}
			}
			int targetSiblingIndex = 0;
			for (int i = 0; i < submeshCount; i++)
			{
				SubmeshInstruction submeshInstructionItem = currentInstructions.submeshInstructions.Items[i];
				meshGenerator.Begin();
				meshGenerator.AddSubmesh(submeshInstructionItem);
				Mesh targetMesh = meshesItems[i];
				meshGenerator.ScaleVertexData(scale);
				if (this.OnPostProcessVertices != null)
				{
					this.OnPostProcessVertices(meshGenerator.Buffers);
				}
				meshGenerator.FillVertexData(targetMesh);
				meshGenerator.FillTriangles(targetMesh);
				meshGenerator.FillLateVertexData(targetMesh);
				Material submeshMaterial = submeshInstructionItem.material;
				CanvasRenderer canvasRenderer = canvasRenderers[i];
				if (i >= usedRenderersCount)
				{
					canvasRenderer.gameObject.SetActive(true);
				}
				canvasRenderer.SetMesh(targetMesh);
				canvasRenderer.materialCount = 1;
				if (canvasRenderer.transform.parent != parent.transform)
				{
					canvasRenderer.transform.SetParent(parent.transform, false);
					canvasRenderer.transform.localPosition = Vector3.zero;
				}
				canvasRenderer.transform.SetSiblingIndex(targetSiblingIndex++);
				if (submeshInstructionItem.forceSeparate)
				{
					targetSiblingIndex = 0;
					parent = separatorParts[++separatorSlotGroupIndex];
				}
				if (useOriginalTextureAndMaterial)
				{
					canvasRenderer.SetMaterial(materialForRendering, submeshMaterial.mainTexture);
					continue;
				}
				Texture originalTexture = submeshMaterial.mainTexture;
				if (!customMaterialOverride.TryGetValue(originalTexture, out var usedMaterial))
				{
					usedMaterial = material;
				}
				if (!customTextureOverride.TryGetValue(originalTexture, out var usedTexture))
				{
					usedTexture = originalTexture;
				}
				canvasRenderer.SetMaterial(usedMaterial, usedTexture);
			}
			DisableUnusedCanvasRenderers(submeshCount);
			usedRenderersCount = submeshCount;
		}

		protected void EnsureCanvasRendererCount(int targetCount)
		{
			int currentCount = canvasRenderers.Count;
			for (int i = currentCount; i < targetCount; i++)
			{
				GameObject go = new GameObject($"Renderer{i}", typeof(RectTransform));
				go.transform.SetParent(base.transform, false);
				go.transform.localPosition = Vector3.zero;
				CanvasRenderer canvasRenderer = go.AddComponent<CanvasRenderer>();
				canvasRenderers.Add(canvasRenderer);
				RawImage rawImage = go.AddComponent<RawImage>();
				rawImage.maskable = base.maskable;
				rawImage.raycastTarget = false;
				rawImages.Add(rawImage);
			}
		}

		protected void DisableUnusedCanvasRenderers(int usedCount)
		{
			for (int i = usedCount; i < canvasRenderers.Count; i++)
			{
				canvasRenderers[i].Clear();
				canvasRenderers[i].gameObject.SetActive(false);
			}
		}

		protected void EnsureMeshesCount(int targetCount)
		{
			int oldCount = meshes.Count;
			meshes.EnsureCapacity(targetCount);
			for (int i = oldCount; i < targetCount; i++)
			{
				meshes.Add(SpineMesh.NewSkeletonMesh());
			}
		}

		protected void DestroyMeshes()
		{
			foreach (Mesh mesh in meshes)
			{
				UnityEngine.Object.Destroy(mesh);
			}
			meshes.Clear();
		}

		protected void EnsureSeparatorPartCount()
		{
			int targetCount = separatorSlots.Count + 1;
			if (targetCount != 1)
			{
				int currentCount = separatorParts.Count;
				for (int i = currentCount; i < targetCount; i++)
				{
					GameObject go = new GameObject(string.Format("{0}[{1}]", "Part", i), typeof(RectTransform));
					go.transform.SetParent(base.transform, false);
					go.transform.localPosition = Vector3.zero;
					separatorParts.Add(go.transform);
				}
			}
		}

		protected void UpdateSeparatorPartParents()
		{
			int usedCount = separatorSlots.Count + 1;
			if (usedCount == 1)
			{
				usedCount = 0;
				for (int j = 0; j < canvasRenderers.Count; j++)
				{
					CanvasRenderer canvasRenderer = canvasRenderers[j];
					if (canvasRenderer.transform.parent.name.Contains("Part"))
					{
						canvasRenderer.transform.SetParent(base.transform, false);
						canvasRenderer.transform.localPosition = Vector3.zero;
					}
				}
			}
			for (int i = 0; i < separatorParts.Count; i++)
			{
				bool isUsed = i < usedCount;
				separatorParts[i].gameObject.SetActive(isUsed);
			}
		}
	}
}
