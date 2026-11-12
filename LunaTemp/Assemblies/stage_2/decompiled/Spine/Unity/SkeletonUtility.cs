using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteAlways]
	[RequireComponent(typeof(ISkeletonAnimation))]
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonUtility")]
	public sealed class SkeletonUtility : MonoBehaviour
	{
		public delegate void SkeletonUtilityDelegate();

		public Transform boneRoot;

		public bool flipBy180DegreeRotation = false;

		[HideInInspector]
		public SkeletonRenderer skeletonRenderer;

		[HideInInspector]
		public SkeletonGraphic skeletonGraphic;

		private Canvas canvas;

		[NonSerialized]
		public ISkeletonAnimation skeletonAnimation;

		private ISkeletonComponent skeletonComponent;

		[NonSerialized]
		public List<SkeletonUtilityBone> boneComponents = new List<SkeletonUtilityBone>();

		[NonSerialized]
		public List<SkeletonUtilityConstraint> constraintComponents = new List<SkeletonUtilityConstraint>();

		private float positionScale = 1f;

		private bool hasOverrideBones;

		private bool hasConstraints;

		private bool needToReprocessBones;

		public ISkeletonComponent SkeletonComponent
		{
			get
			{
				if (skeletonComponent == null)
				{
					skeletonComponent = ((skeletonRenderer != null) ? skeletonRenderer.GetComponent<ISkeletonComponent>() : ((skeletonGraphic != null) ? skeletonGraphic.GetComponent<ISkeletonComponent>() : GetComponent<ISkeletonComponent>()));
				}
				return skeletonComponent;
			}
		}

		public Skeleton Skeleton
		{
			get
			{
				if (SkeletonComponent == null)
				{
					return null;
				}
				return skeletonComponent.Skeleton;
			}
		}

		public bool IsValid => (skeletonRenderer != null && skeletonRenderer.valid) || (skeletonGraphic != null && skeletonGraphic.IsValid);

		public float PositionScale => positionScale;

		public event SkeletonUtilityDelegate OnReset;

		public static PolygonCollider2D AddBoundingBoxGameObject(Skeleton skeleton, string skinName, string slotName, string attachmentName, Transform parent, bool isTrigger = true)
		{
			Skin skin = (string.IsNullOrEmpty(skinName) ? skeleton.data.defaultSkin : skeleton.data.FindSkin(skinName));
			if (skin == null)
			{
				Debug.LogError("Skin " + skinName + " not found!");
				return null;
			}
			Attachment attachment = skin.GetAttachment(skeleton.FindSlotIndex(slotName), attachmentName);
			if (attachment == null)
			{
				Debug.LogFormat("Attachment in slot '{0}' named '{1}' not found in skin '{2}'.", slotName, attachmentName, skin.name);
				return null;
			}
			if (attachment is BoundingBoxAttachment box)
			{
				Slot slot = skeleton.FindSlot(slotName);
				return AddBoundingBoxGameObject(box.Name, box, slot, parent, isTrigger);
			}
			Debug.LogFormat("Attachment '{0}' was not a Bounding Box.", attachmentName);
			return null;
		}

		public static PolygonCollider2D AddBoundingBoxGameObject(string name, BoundingBoxAttachment box, Slot slot, Transform parent, bool isTrigger = true)
		{
			GameObject go = new GameObject("[BoundingBox]" + (string.IsNullOrEmpty(name) ? box.Name : name));
			Transform got = go.transform;
			got.parent = parent;
			got.localPosition = Vector3.zero;
			got.localRotation = Quaternion.identity;
			got.localScale = Vector3.one;
			return AddBoundingBoxAsComponent(box, slot, go, isTrigger);
		}

		public static PolygonCollider2D AddBoundingBoxAsComponent(BoundingBoxAttachment box, Slot slot, GameObject gameObject, bool isTrigger = true)
		{
			if (box == null)
			{
				return null;
			}
			PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();
			collider.isTrigger = isTrigger;
			SetColliderPointsLocal(collider, slot, box);
			return collider;
		}

		public static void SetColliderPointsLocal(PolygonCollider2D collider, Slot slot, BoundingBoxAttachment box, float scale = 1f)
		{
			if (box == null)
			{
				return;
			}
			if (box.IsWeighted())
			{
				Debug.LogWarning("UnityEngine.PolygonCollider2D does not support weighted or animated points. Collider points will not be animated and may have incorrect orientation. If you want to use it as a collider, please remove weights and animations from the bounding box in Spine editor.");
			}
			Vector2[] verts = box.GetLocalVertices(slot, null);
			if (scale != 1f)
			{
				int i = 0;
				for (int j = verts.Length; i < j; i++)
				{
					verts[i] *= scale;
				}
			}
			collider.SetPath(0, verts);
		}

		public static Bounds GetBoundingBoxBounds(BoundingBoxAttachment boundingBox, float depth = 0f)
		{
			float[] floats = boundingBox.Vertices;
			int floatCount = floats.Length;
			Bounds bounds = default(Bounds);
			bounds.center = new Vector3(floats[0], floats[1], 0f);
			for (int i = 2; i < floatCount; i += 2)
			{
				bounds.Encapsulate(new Vector3(floats[i], floats[i + 1], 0f));
			}
			Vector3 size = bounds.size;
			size.z = depth;
			bounds.size = size;
			return bounds;
		}

		public static Rigidbody2D AddBoneRigidbody2D(GameObject gameObject, bool isKinematic = true, float gravityScale = 0f)
		{
			Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
			if (rb == null)
			{
				rb = gameObject.AddComponent<Rigidbody2D>();
				rb.isKinematic = isKinematic;
				rb.gravityScale = gravityScale;
			}
			return rb;
		}

		private void Update()
		{
			Skeleton skeleton = skeletonComponent.Skeleton;
			if (skeleton != null && boneRoot != null)
			{
				if (flipBy180DegreeRotation)
				{
					boneRoot.localScale = new Vector3(Mathf.Abs(skeleton.ScaleX), Mathf.Abs(skeleton.ScaleY), 1f);
					boneRoot.eulerAngles = new Vector3((!(skeleton.ScaleY > 0f)) ? 180 : 0, (!(skeleton.ScaleX > 0f)) ? 180 : 0, 0f);
				}
				else
				{
					boneRoot.localScale = new Vector3(skeleton.ScaleX, skeleton.ScaleY, 1f);
				}
			}
			if (canvas != null)
			{
				positionScale = canvas.referencePixelsPerUnit;
			}
		}

		public void ResubscribeEvents()
		{
			OnDisable();
			OnEnable();
		}

		private void OnEnable()
		{
			if (skeletonRenderer == null)
			{
				skeletonRenderer = GetComponent<SkeletonRenderer>();
			}
			if (skeletonGraphic == null)
			{
				skeletonGraphic = GetComponent<SkeletonGraphic>();
			}
			if (skeletonAnimation == null)
			{
				skeletonAnimation = ((skeletonRenderer != null) ? skeletonRenderer.GetComponent<ISkeletonAnimation>() : ((skeletonGraphic != null) ? skeletonGraphic.GetComponent<ISkeletonAnimation>() : GetComponent<ISkeletonAnimation>()));
			}
			if (skeletonComponent == null)
			{
				skeletonComponent = ((skeletonRenderer != null) ? skeletonRenderer.GetComponent<ISkeletonComponent>() : ((skeletonGraphic != null) ? skeletonGraphic.GetComponent<ISkeletonComponent>() : GetComponent<ISkeletonComponent>()));
			}
			if (skeletonRenderer != null)
			{
				skeletonRenderer.OnRebuild -= HandleRendererReset;
				skeletonRenderer.OnRebuild += HandleRendererReset;
			}
			else if (skeletonGraphic != null)
			{
				skeletonGraphic.OnRebuild -= HandleRendererReset;
				skeletonGraphic.OnRebuild += HandleRendererReset;
				canvas = skeletonGraphic.canvas;
				if (canvas == null)
				{
					canvas = skeletonGraphic.GetComponentInParent<Canvas>();
				}
				if (canvas == null)
				{
					positionScale = 100f;
				}
			}
			if (skeletonAnimation != null)
			{
				skeletonAnimation.UpdateLocal -= UpdateLocal;
				skeletonAnimation.UpdateLocal += UpdateLocal;
			}
			CollectBones();
		}

		private void Start()
		{
			CollectBones();
		}

		private void OnDisable()
		{
			if (skeletonRenderer != null)
			{
				skeletonRenderer.OnRebuild -= HandleRendererReset;
			}
			if (skeletonGraphic != null)
			{
				skeletonGraphic.OnRebuild -= HandleRendererReset;
			}
			if (skeletonAnimation != null)
			{
				skeletonAnimation.UpdateLocal -= UpdateLocal;
				skeletonAnimation.UpdateWorld -= UpdateWorld;
				skeletonAnimation.UpdateComplete -= UpdateComplete;
			}
		}

		private void HandleRendererReset(SkeletonRenderer r)
		{
			if (this.OnReset != null)
			{
				this.OnReset();
			}
			CollectBones();
		}

		private void HandleRendererReset(SkeletonGraphic g)
		{
			if (this.OnReset != null)
			{
				this.OnReset();
			}
			CollectBones();
		}

		public void RegisterBone(SkeletonUtilityBone bone)
		{
			if (!boneComponents.Contains(bone))
			{
				boneComponents.Add(bone);
				needToReprocessBones = true;
			}
		}

		public void UnregisterBone(SkeletonUtilityBone bone)
		{
			boneComponents.Remove(bone);
		}

		public void RegisterConstraint(SkeletonUtilityConstraint constraint)
		{
			if (!constraintComponents.Contains(constraint))
			{
				constraintComponents.Add(constraint);
				needToReprocessBones = true;
			}
		}

		public void UnregisterConstraint(SkeletonUtilityConstraint constraint)
		{
			constraintComponents.Remove(constraint);
		}

		public void CollectBones()
		{
			Skeleton skeleton = skeletonComponent.Skeleton;
			if (skeleton == null)
			{
				return;
			}
			if (boneRoot != null)
			{
				List<object> constraintTargets = new List<object>();
				ExposedList<IkConstraint> ikConstraints = skeleton.IkConstraints;
				int k = 0;
				for (int n = ikConstraints.Count; k < n; k++)
				{
					constraintTargets.Add(ikConstraints.Items[k].target);
				}
				ExposedList<TransformConstraint> transformConstraints = skeleton.TransformConstraints;
				int j = 0;
				for (int m = transformConstraints.Count; j < m; j++)
				{
					constraintTargets.Add(transformConstraints.Items[j].target);
				}
				List<SkeletonUtilityBone> boneComponents = this.boneComponents;
				int i = 0;
				for (int l = boneComponents.Count; i < l; i++)
				{
					SkeletonUtilityBone b = boneComponents[i];
					if (b.bone == null)
					{
						b.DoUpdate(SkeletonUtilityBone.UpdatePhase.Local);
						if (b.bone == null)
						{
							continue;
						}
					}
					hasOverrideBones |= b.mode == SkeletonUtilityBone.Mode.Override;
					hasConstraints |= constraintTargets.Contains(b.bone);
				}
				hasConstraints |= constraintComponents.Count > 0;
				if (skeletonAnimation != null)
				{
					skeletonAnimation.UpdateWorld -= UpdateWorld;
					skeletonAnimation.UpdateComplete -= UpdateComplete;
					if (hasOverrideBones || hasConstraints)
					{
						skeletonAnimation.UpdateWorld += UpdateWorld;
					}
					if (hasConstraints)
					{
						skeletonAnimation.UpdateComplete += UpdateComplete;
					}
				}
				needToReprocessBones = false;
			}
			else
			{
				this.boneComponents.Clear();
				constraintComponents.Clear();
			}
		}

		private void UpdateLocal(ISkeletonAnimation anim)
		{
			if (needToReprocessBones)
			{
				CollectBones();
			}
			List<SkeletonUtilityBone> boneComponents = this.boneComponents;
			if (boneComponents != null)
			{
				int i = 0;
				for (int j = boneComponents.Count; i < j; i++)
				{
					boneComponents[i].transformLerpComplete = false;
				}
				UpdateAllBones(SkeletonUtilityBone.UpdatePhase.Local);
			}
		}

		private void UpdateWorld(ISkeletonAnimation anim)
		{
			UpdateAllBones(SkeletonUtilityBone.UpdatePhase.World);
			int i = 0;
			for (int j = constraintComponents.Count; i < j; i++)
			{
				constraintComponents[i].DoUpdate();
			}
		}

		private void UpdateComplete(ISkeletonAnimation anim)
		{
			UpdateAllBones(SkeletonUtilityBone.UpdatePhase.Complete);
		}

		private void UpdateAllBones(SkeletonUtilityBone.UpdatePhase phase)
		{
			if (boneRoot == null)
			{
				CollectBones();
			}
			List<SkeletonUtilityBone> boneComponents = this.boneComponents;
			if (boneComponents != null)
			{
				int i = 0;
				for (int j = boneComponents.Count; i < j; i++)
				{
					boneComponents[i].DoUpdate(phase);
				}
			}
		}

		public Transform GetBoneRoot()
		{
			if (boneRoot != null)
			{
				return boneRoot;
			}
			GameObject boneRootObject = new GameObject("SkeletonUtility-SkeletonRoot");
			if (skeletonGraphic != null)
			{
				boneRootObject.AddComponent<RectTransform>();
			}
			boneRoot = boneRootObject.transform;
			boneRoot.SetParent(base.transform);
			boneRoot.localPosition = Vector3.zero;
			boneRoot.localRotation = Quaternion.identity;
			boneRoot.localScale = Vector3.one;
			return boneRoot;
		}

		public GameObject SpawnRoot(SkeletonUtilityBone.Mode mode, bool pos, bool rot, bool sca)
		{
			GetBoneRoot();
			Skeleton skeleton = skeletonComponent.Skeleton;
			GameObject go = SpawnBone(skeleton.RootBone, boneRoot, mode, pos, rot, sca);
			CollectBones();
			return go;
		}

		public GameObject SpawnHierarchy(SkeletonUtilityBone.Mode mode, bool pos, bool rot, bool sca)
		{
			GetBoneRoot();
			Skeleton skeleton = skeletonComponent.Skeleton;
			GameObject go = SpawnBoneRecursively(skeleton.RootBone, boneRoot, mode, pos, rot, sca);
			CollectBones();
			return go;
		}

		public GameObject SpawnBoneRecursively(Bone bone, Transform parent, SkeletonUtilityBone.Mode mode, bool pos, bool rot, bool sca)
		{
			GameObject go = SpawnBone(bone, parent, mode, pos, rot, sca);
			ExposedList<Bone> childrenBones = bone.Children;
			int i = 0;
			for (int j = childrenBones.Count; i < j; i++)
			{
				Bone child = childrenBones.Items[i];
				SpawnBoneRecursively(child, go.transform, mode, pos, rot, sca);
			}
			return go;
		}

		public GameObject SpawnBone(Bone bone, Transform parent, SkeletonUtilityBone.Mode mode, bool pos, bool rot, bool sca)
		{
			GameObject go = new GameObject(bone.Data.Name);
			if (skeletonGraphic != null)
			{
				go.AddComponent<RectTransform>();
			}
			Transform goTransform = go.transform;
			goTransform.SetParent(parent);
			SkeletonUtilityBone b = go.AddComponent<SkeletonUtilityBone>();
			b.hierarchy = this;
			b.position = pos;
			b.rotation = rot;
			b.scale = sca;
			b.mode = mode;
			b.zPosition = true;
			b.Reset();
			b.bone = bone;
			b.boneName = bone.Data.Name;
			b.valid = true;
			if (mode == SkeletonUtilityBone.Mode.Override)
			{
				if (rot)
				{
					goTransform.localRotation = Quaternion.Euler(0f, 0f, b.bone.AppliedRotation);
				}
				if (pos)
				{
					goTransform.localPosition = new Vector3(b.bone.X * positionScale, b.bone.Y * positionScale, 0f);
				}
				goTransform.localScale = new Vector3(b.bone.scaleX, b.bone.scaleY, 0f);
			}
			return go;
		}
	}
}
