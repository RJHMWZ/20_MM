using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Spine.Unity
{
	[ExecuteAlways]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[DisallowMultipleComponent]
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonRenderer-Component")]
	public class SkeletonRenderer : MonoBehaviour, ISkeletonComponent, IHasSkeletonDataAsset
	{
		[Serializable]
		public class SpriteMaskInteractionMaterials
		{
			public Material[] materialsMaskDisabled = new Material[0];

			public Material[] materialsInsideMask = new Material[0];

			public Material[] materialsOutsideMask = new Material[0];

			public bool AnyMaterialCreated => materialsMaskDisabled.Length != 0 || materialsInsideMask.Length != 0 || materialsOutsideMask.Length != 0;
		}

		public delegate void InstructionDelegate(SkeletonRendererInstruction instruction);

		public delegate void SkeletonRendererDelegate(SkeletonRenderer skeletonRenderer);

		public SkeletonDataAsset skeletonDataAsset;

		[SpineSkin("", "", true, false, true)]
		public string initialSkinName;

		public bool initialFlipX;

		public bool initialFlipY;

		protected UpdateMode updateMode = UpdateMode.FullUpdate;

		public UpdateMode updateWhenInvisible = UpdateMode.FullUpdate;

		[FormerlySerializedAs("submeshSeparators")]
		[SerializeField]
		[SpineSlot("", "", false, true, false)]
		protected string[] separatorSlotNames = new string[0];

		[NonSerialized]
		public readonly List<Slot> separatorSlots = new List<Slot>();

		[Range(-0.1f, 0f)]
		public float zSpacing;

		public bool useClipping = true;

		public bool immutableTriangles = false;

		public bool pmaVertexColors = true;

		public bool clearStateOnDisable = false;

		public bool tintBlack = false;

		public bool singleSubmesh = false;

		public bool fixDrawOrder = false;

		[FormerlySerializedAs("calculateNormals")]
		public bool addNormals = false;

		public bool calculateTangents = false;

		public SpriteMaskInteraction maskInteraction = SpriteMaskInteraction.None;

		public SpriteMaskInteractionMaterials maskMaterials = new SpriteMaskInteractionMaterials();

		public static readonly int STENCIL_COMP_PARAM_ID = Shader.PropertyToID("_StencilComp");

		public const CompareFunction STENCIL_COMP_MASKINTERACTION_NONE = CompareFunction.Always;

		public const CompareFunction STENCIL_COMP_MASKINTERACTION_VISIBLE_INSIDE = CompareFunction.LessEqual;

		public const CompareFunction STENCIL_COMP_MASKINTERACTION_VISIBLE_OUTSIDE = CompareFunction.Greater;

		public bool disableRenderingOnOverride = true;

		[NonSerialized]
		private readonly Dictionary<Material, Material> customMaterialOverride = new Dictionary<Material, Material>();

		[NonSerialized]
		private readonly Dictionary<Slot, Material> customSlotMaterials = new Dictionary<Slot, Material>();

		[NonSerialized]
		private readonly SkeletonRendererInstruction currentInstructions = new SkeletonRendererInstruction();

		private readonly MeshGenerator meshGenerator = new MeshGenerator();

		[NonSerialized]
		private readonly MeshRendererBuffers rendererBuffers = new MeshRendererBuffers();

		private MeshRenderer meshRenderer;

		private MeshFilter meshFilter;

		[NonSerialized]
		public bool valid;

		[NonSerialized]
		public Skeleton skeleton;

		private MaterialPropertyBlock reusedPropertyBlock;

		public static readonly int SUBMESH_DUMMY_PARAM_ID = Shader.PropertyToID("_Submesh");

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

		public Dictionary<Material, Material> CustomMaterialOverride => customMaterialOverride;

		public Dictionary<Slot, Material> CustomSlotMaterials => customSlotMaterials;

		public Skeleton Skeleton
		{
			get
			{
				Initialize(false);
				return skeleton;
			}
		}

		public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

		private event InstructionDelegate generateMeshOverride;

		public event InstructionDelegate GenerateMeshOverride
		{
			add
			{
				generateMeshOverride += value;
				if (disableRenderingOnOverride && this.generateMeshOverride != null)
				{
					Initialize(false);
					if ((bool)meshRenderer)
					{
						meshRenderer.enabled = false;
					}
				}
			}
			remove
			{
				generateMeshOverride -= value;
				if (disableRenderingOnOverride && this.generateMeshOverride == null)
				{
					Initialize(false);
					if ((bool)meshRenderer)
					{
						meshRenderer.enabled = true;
					}
				}
			}
		}

		public event MeshGeneratorDelegate OnPostProcessVertices;

		public event SkeletonRendererDelegate OnRebuild;

		public event SkeletonRendererDelegate OnMeshAndMaterialsUpdated;

		public static T NewSpineGameObject<T>(SkeletonDataAsset skeletonDataAsset, bool quiet = false) where T : SkeletonRenderer
		{
			return AddSpineComponent<T>(new GameObject("New Spine GameObject"), skeletonDataAsset, quiet);
		}

		public static T AddSpineComponent<T>(GameObject gameObject, SkeletonDataAsset skeletonDataAsset, bool quiet = false) where T : SkeletonRenderer
		{
			T c = gameObject.AddComponent<T>();
			if (skeletonDataAsset != null)
			{
				c.skeletonDataAsset = skeletonDataAsset;
				c.Initialize(false, quiet);
			}
			return c;
		}

		public void SetMeshSettings(MeshGenerator.Settings settings)
		{
			calculateTangents = settings.calculateTangents;
			immutableTriangles = settings.immutableTriangles;
			pmaVertexColors = settings.pmaVertexColors;
			tintBlack = settings.tintBlack;
			useClipping = settings.useClipping;
			zSpacing = settings.zSpacing;
			meshGenerator.settings = settings;
		}

		public virtual void Awake()
		{
			Initialize(false);
			updateMode = updateWhenInvisible;
		}

		private void OnDisable()
		{
			if (clearStateOnDisable && valid)
			{
				ClearState();
			}
		}

		private void OnDestroy()
		{
			rendererBuffers.Dispose();
			valid = false;
		}

		public virtual void ClearState()
		{
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				meshFilter.sharedMesh = null;
			}
			currentInstructions.Clear();
			if (skeleton != null)
			{
				skeleton.SetToSetupPose();
			}
		}

		public void EnsureMeshGeneratorCapacity(int minimumVertexCount)
		{
			meshGenerator.EnsureVertexCapacity(minimumVertexCount);
		}

		public virtual void Initialize(bool overwrite, bool quiet = false)
		{
			if (valid && !overwrite)
			{
				return;
			}
			currentInstructions.Clear();
			rendererBuffers.Clear();
			meshGenerator.Begin();
			skeleton = null;
			valid = false;
			if (skeletonDataAsset == null)
			{
				return;
			}
			SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(false);
			if (skeletonData != null)
			{
				valid = true;
				meshFilter = GetComponent<MeshFilter>();
				meshRenderer = GetComponent<MeshRenderer>();
				rendererBuffers.Initialize();
				skeleton = new Skeleton(skeletonData)
				{
					ScaleX = ((!initialFlipX) ? 1 : (-1)),
					ScaleY = ((!initialFlipY) ? 1 : (-1))
				};
				if (!string.IsNullOrEmpty(initialSkinName) && !string.Equals(initialSkinName, "default", StringComparison.Ordinal))
				{
					skeleton.SetSkin(initialSkinName);
				}
				separatorSlots.Clear();
				for (int i = 0; i < separatorSlotNames.Length; i++)
				{
					separatorSlots.Add(skeleton.FindSlot(separatorSlotNames[i]));
				}
				LateUpdate();
				if (this.OnRebuild != null)
				{
					this.OnRebuild(this);
				}
			}
		}

		public virtual void LateUpdate()
		{
			if (!valid || updateMode != UpdateMode.FullUpdate)
			{
				return;
			}
			bool doMeshOverride = this.generateMeshOverride != null;
			if (!meshRenderer.enabled && !doMeshOverride)
			{
				return;
			}
			SkeletonRendererInstruction currentInstructions = this.currentInstructions;
			ExposedList<SubmeshInstruction> workingSubmeshInstructions = currentInstructions.submeshInstructions;
			MeshRendererBuffers.SmartMesh currentSmartMesh = rendererBuffers.GetNextMesh();
			bool updateTriangles;
			if (singleSubmesh)
			{
				MeshGenerator.GenerateSingleSubmeshInstruction(currentInstructions, skeleton, skeletonDataAsset.atlasAssets[0].PrimaryMaterial);
				if (customMaterialOverride.Count > 0)
				{
					MeshGenerator.TryReplaceMaterials(workingSubmeshInstructions, customMaterialOverride);
				}
				meshGenerator.settings = new MeshGenerator.Settings
				{
					pmaVertexColors = pmaVertexColors,
					zSpacing = zSpacing,
					useClipping = useClipping,
					tintBlack = tintBlack,
					calculateTangents = calculateTangents,
					addNormals = addNormals
				};
				meshGenerator.Begin();
				updateTriangles = SkeletonRendererInstruction.GeometryNotEqual(currentInstructions, currentSmartMesh.instructionUsed);
				if (currentInstructions.hasActiveClipping)
				{
					meshGenerator.AddSubmesh(workingSubmeshInstructions.Items[0], updateTriangles);
				}
				else
				{
					meshGenerator.BuildMeshWithArrays(currentInstructions, updateTriangles);
				}
			}
			else
			{
				MeshGenerator.GenerateSkeletonRendererInstruction(currentInstructions, skeleton, customSlotMaterials, separatorSlots, doMeshOverride, immutableTriangles);
				if (customMaterialOverride.Count > 0)
				{
					MeshGenerator.TryReplaceMaterials(workingSubmeshInstructions, customMaterialOverride);
				}
				if (doMeshOverride)
				{
					this.generateMeshOverride(currentInstructions);
					if (disableRenderingOnOverride)
					{
						return;
					}
				}
				updateTriangles = SkeletonRendererInstruction.GeometryNotEqual(currentInstructions, currentSmartMesh.instructionUsed);
				meshGenerator.settings = new MeshGenerator.Settings
				{
					pmaVertexColors = pmaVertexColors,
					zSpacing = zSpacing,
					useClipping = useClipping,
					tintBlack = tintBlack,
					calculateTangents = calculateTangents,
					addNormals = addNormals
				};
				meshGenerator.Begin();
				if (currentInstructions.hasActiveClipping)
				{
					meshGenerator.BuildMesh(currentInstructions, updateTriangles);
				}
				else
				{
					meshGenerator.BuildMeshWithArrays(currentInstructions, updateTriangles);
				}
			}
			if (this.OnPostProcessVertices != null)
			{
				this.OnPostProcessVertices(meshGenerator.Buffers);
			}
			Mesh currentMesh = currentSmartMesh.mesh;
			meshGenerator.FillVertexData(currentMesh);
			rendererBuffers.UpdateSharedMaterials(workingSubmeshInstructions);
			bool materialsChanged = rendererBuffers.MaterialsChangedInLastUpdate();
			if (updateTriangles)
			{
				meshGenerator.FillTriangles(currentMesh);
				meshRenderer.sharedMaterials = rendererBuffers.GetUpdatedSharedMaterialsArray();
			}
			else if (materialsChanged)
			{
				meshRenderer.sharedMaterials = rendererBuffers.GetUpdatedSharedMaterialsArray();
			}
			if (materialsChanged && maskMaterials.AnyMaterialCreated)
			{
				maskMaterials = new SpriteMaskInteractionMaterials();
			}
			meshGenerator.FillLateVertexData(currentMesh);
			meshFilter.sharedMesh = currentMesh;
			currentSmartMesh.instructionUsed.Set(currentInstructions);
			if (meshRenderer != null)
			{
				AssignSpriteMaskMaterials();
			}
			if (fixDrawOrder && meshRenderer.sharedMaterials.Length > 2)
			{
				SetMaterialSettingsToFixDrawOrder();
			}
			if (this.OnMeshAndMaterialsUpdated != null)
			{
				this.OnMeshAndMaterialsUpdated(this);
			}
		}

		public void OnBecameVisible()
		{
			UpdateMode previousUpdateMode = updateMode;
			updateMode = UpdateMode.FullUpdate;
			if (previousUpdateMode != UpdateMode.FullUpdate)
			{
				LateUpdate();
			}
		}

		public void OnBecameInvisible()
		{
			updateMode = updateWhenInvisible;
		}

		public void FindAndApplySeparatorSlots(string startsWith, bool clearExistingSeparators = true, bool updateStringArray = false)
		{
			if (!string.IsNullOrEmpty(startsWith))
			{
				FindAndApplySeparatorSlots((string slotName) => slotName.StartsWith(startsWith), clearExistingSeparators, updateStringArray);
			}
		}

		public void FindAndApplySeparatorSlots(Func<string, bool> slotNamePredicate, bool clearExistingSeparators = true, bool updateStringArray = false)
		{
			if (slotNamePredicate == null || !valid)
			{
				return;
			}
			if (clearExistingSeparators)
			{
				separatorSlots.Clear();
			}
			ExposedList<Slot> slots = skeleton.slots;
			foreach (Slot slot2 in slots)
			{
				if (slotNamePredicate(slot2.data.name))
				{
					separatorSlots.Add(slot2);
				}
			}
			if (!updateStringArray)
			{
				return;
			}
			List<string> detectedSeparatorNames = new List<string>();
			foreach (Slot slot in skeleton.slots)
			{
				string slotName = slot.data.name;
				if (slotNamePredicate(slotName))
				{
					detectedSeparatorNames.Add(slotName);
				}
			}
			if (!clearExistingSeparators)
			{
				string[] originalNames = separatorSlotNames;
				string[] array = originalNames;
				foreach (string originalName in array)
				{
					detectedSeparatorNames.Add(originalName);
				}
			}
			separatorSlotNames = detectedSeparatorNames.ToArray();
		}

		public void ReapplySeparatorSlotNames()
		{
			if (!valid)
			{
				return;
			}
			separatorSlots.Clear();
			int i = 0;
			for (int j = separatorSlotNames.Length; i < j; i++)
			{
				Slot slot = skeleton.FindSlot(separatorSlotNames[i]);
				if (slot != null)
				{
					separatorSlots.Add(slot);
				}
			}
		}

		private void AssignSpriteMaskMaterials()
		{
			if (Application.isPlaying && maskInteraction != 0 && maskMaterials.materialsMaskDisabled.Length == 0)
			{
				maskMaterials.materialsMaskDisabled = meshRenderer.sharedMaterials;
			}
			if (maskMaterials.materialsMaskDisabled.Length != 0 && maskMaterials.materialsMaskDisabled[0] != null && maskInteraction == SpriteMaskInteraction.None)
			{
				meshRenderer.materials = maskMaterials.materialsMaskDisabled;
			}
			else if (maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
			{
				if ((maskMaterials.materialsInsideMask.Length != 0 && !(maskMaterials.materialsInsideMask[0] == null)) || InitSpriteMaskMaterialsInsideMask())
				{
					meshRenderer.materials = maskMaterials.materialsInsideMask;
				}
			}
			else if (maskInteraction == SpriteMaskInteraction.VisibleOutsideMask && ((maskMaterials.materialsOutsideMask.Length != 0 && !(maskMaterials.materialsOutsideMask[0] == null)) || InitSpriteMaskMaterialsOutsideMask()))
			{
				meshRenderer.materials = maskMaterials.materialsOutsideMask;
			}
		}

		private bool InitSpriteMaskMaterialsInsideMask()
		{
			return InitSpriteMaskMaterialsForMaskType(CompareFunction.LessEqual, ref maskMaterials.materialsInsideMask);
		}

		private bool InitSpriteMaskMaterialsOutsideMask()
		{
			return InitSpriteMaskMaterialsForMaskType(CompareFunction.Greater, ref maskMaterials.materialsOutsideMask);
		}

		private bool InitSpriteMaskMaterialsForMaskType(CompareFunction maskFunction, ref Material[] materialsToFill)
		{
			Material[] originalMaterials = maskMaterials.materialsMaskDisabled;
			materialsToFill = new Material[originalMaterials.Length];
			for (int i = 0; i < originalMaterials.Length; i++)
			{
				Material newMaterial = new Material(originalMaterials[i]);
				newMaterial.SetFloat(STENCIL_COMP_PARAM_ID, (float)maskFunction);
				materialsToFill[i] = newMaterial;
			}
			return true;
		}

		private void SetMaterialSettingsToFixDrawOrder()
		{
			if (reusedPropertyBlock == null)
			{
				reusedPropertyBlock = new MaterialPropertyBlock();
			}
			bool hasPerRendererBlock = meshRenderer.HasPropertyBlock();
			if (hasPerRendererBlock)
			{
				meshRenderer.GetPropertyBlock(reusedPropertyBlock);
			}
			for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
			{
				if ((bool)meshRenderer.sharedMaterials[i])
				{
					if (!hasPerRendererBlock)
					{
						meshRenderer.GetPropertyBlock(reusedPropertyBlock, i);
					}
					reusedPropertyBlock.SetFloat(SUBMESH_DUMMY_PARAM_ID, i);
					meshRenderer.SetPropertyBlock(reusedPropertyBlock, i);
					meshRenderer.sharedMaterials[i].enableInstancing = false;
				}
			}
		}
	}
}
