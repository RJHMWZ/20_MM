using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteAlways]
	[HelpURL("http://esotericsoftware.com/spine-unity#BoundingBoxFollower")]
	public class BoundingBoxFollower : MonoBehaviour
	{
		internal static bool DebugMessages = true;

		public SkeletonRenderer skeletonRenderer;

		[SpineSlot("", "skeletonRenderer", true, true, false)]
		public string slotName;

		public bool isTrigger;

		public bool clearStateOnDisable = true;

		private Slot slot;

		private BoundingBoxAttachment currentAttachment;

		private string currentAttachmentName;

		private PolygonCollider2D currentCollider;

		public readonly Dictionary<BoundingBoxAttachment, PolygonCollider2D> colliderTable = new Dictionary<BoundingBoxAttachment, PolygonCollider2D>();

		public readonly Dictionary<BoundingBoxAttachment, string> nameTable = new Dictionary<BoundingBoxAttachment, string>();

		public Slot Slot => slot;

		public BoundingBoxAttachment CurrentAttachment => currentAttachment;

		public string CurrentAttachmentName => currentAttachmentName;

		public PolygonCollider2D CurrentCollider => currentCollider;

		public bool IsTrigger => isTrigger;

		private void Start()
		{
			Initialize();
		}

		private void OnEnable()
		{
			if (skeletonRenderer != null)
			{
				skeletonRenderer.OnRebuild -= HandleRebuild;
				skeletonRenderer.OnRebuild += HandleRebuild;
			}
			Initialize();
		}

		private void HandleRebuild(SkeletonRenderer sr)
		{
			Initialize();
		}

		public void Initialize(bool overwrite = false)
		{
			if (skeletonRenderer == null)
			{
				return;
			}
			skeletonRenderer.Initialize(false);
			if (string.IsNullOrEmpty(slotName) || (!overwrite && colliderTable.Count > 0 && slot != null && skeletonRenderer.skeleton == slot.Skeleton && slotName == slot.data.name))
			{
				return;
			}
			slot = null;
			currentAttachment = null;
			currentAttachmentName = null;
			currentCollider = null;
			colliderTable.Clear();
			nameTable.Clear();
			Skeleton skeleton = skeletonRenderer.skeleton;
			if (skeleton == null)
			{
				return;
			}
			slot = skeleton.FindSlot(slotName);
			int slotIndex = skeleton.FindSlotIndex(slotName);
			if (slot == null)
			{
				if (DebugMessages)
				{
					Debug.LogWarning($"Slot '{slotName}' not found for BoundingBoxFollower on '{base.gameObject.name}'. (Previous colliders were disposed.)");
				}
				return;
			}
			int requiredCollidersCount = 0;
			PolygonCollider2D[] colliders = GetComponents<PolygonCollider2D>();
			if (base.gameObject.activeInHierarchy)
			{
				foreach (Skin skin in skeleton.Data.Skins)
				{
					AddCollidersForSkin(skin, slotIndex, colliders, ref requiredCollidersCount);
				}
				if (skeleton.skin != null)
				{
					AddCollidersForSkin(skeleton.skin, slotIndex, colliders, ref requiredCollidersCount);
				}
			}
			DisposeExcessCollidersAfter(requiredCollidersCount);
			if (DebugMessages && colliderTable.Count == 0)
			{
				if (base.gameObject.activeInHierarchy)
				{
					Debug.LogWarning("Bounding Box Follower not valid! Slot [" + slotName + "] does not contain any Bounding Box Attachments!");
				}
				else
				{
					Debug.LogWarning("Bounding Box Follower tried to rebuild as a prefab.");
				}
			}
		}

		private void AddCollidersForSkin(Skin skin, int slotIndex, PolygonCollider2D[] previousColliders, ref int collidersCount)
		{
			if (skin == null)
			{
				return;
			}
			List<Skin.SkinEntry> skinEntries = new List<Skin.SkinEntry>();
			skin.GetAttachments(slotIndex, skinEntries);
			foreach (Skin.SkinEntry entry in skinEntries)
			{
				Attachment attachment = skin.GetAttachment(slotIndex, entry.Name);
				BoundingBoxAttachment boundingBoxAttachment = attachment as BoundingBoxAttachment;
				if (DebugMessages && attachment != null && boundingBoxAttachment == null)
				{
					Debug.Log("BoundingBoxFollower tried to follow a slot that contains non-boundingbox attachments: " + slotName);
				}
				if (boundingBoxAttachment != null && !colliderTable.ContainsKey(boundingBoxAttachment))
				{
					PolygonCollider2D bbCollider = ((collidersCount < previousColliders.Length) ? previousColliders[collidersCount] : base.gameObject.AddComponent<PolygonCollider2D>());
					collidersCount++;
					SkeletonUtility.SetColliderPointsLocal(bbCollider, slot, boundingBoxAttachment);
					bbCollider.isTrigger = isTrigger;
					bbCollider.enabled = false;
					bbCollider.hideFlags = HideFlags.NotEditable;
					bbCollider.isTrigger = IsTrigger;
					colliderTable.Add(boundingBoxAttachment, bbCollider);
					nameTable.Add(boundingBoxAttachment, entry.Name);
				}
			}
		}

		private void OnDisable()
		{
			if (clearStateOnDisable)
			{
				ClearState();
			}
			if (skeletonRenderer != null)
			{
				skeletonRenderer.OnRebuild -= HandleRebuild;
			}
		}

		public void ClearState()
		{
			if (colliderTable != null)
			{
				foreach (PolygonCollider2D col in colliderTable.Values)
				{
					col.enabled = false;
				}
			}
			currentAttachment = null;
			currentAttachmentName = null;
			currentCollider = null;
		}

		private void DisposeExcessCollidersAfter(int requiredCount)
		{
			PolygonCollider2D[] colliders = GetComponents<PolygonCollider2D>();
			if (colliders.Length == 0)
			{
				return;
			}
			for (int i = requiredCount; i < colliders.Length; i++)
			{
				PolygonCollider2D collider = colliders[i];
				if (collider != null)
				{
					Object.Destroy(collider);
				}
			}
		}

		private void LateUpdate()
		{
			if (slot != null && slot.Attachment != currentAttachment)
			{
				MatchAttachment(slot.Attachment);
			}
		}

		private void MatchAttachment(Attachment attachment)
		{
			BoundingBoxAttachment bbAttachment = attachment as BoundingBoxAttachment;
			if (DebugMessages && attachment != null && bbAttachment == null)
			{
				Debug.LogWarning("BoundingBoxFollower tried to match a non-boundingbox attachment. It will treat it as null.");
			}
			if (currentCollider != null)
			{
				currentCollider.enabled = false;
			}
			if (bbAttachment == null)
			{
				currentCollider = null;
				currentAttachment = null;
				currentAttachmentName = null;
				return;
			}
			colliderTable.TryGetValue(bbAttachment, out var foundCollider);
			if (foundCollider != null)
			{
				currentCollider = foundCollider;
				currentCollider.enabled = true;
				currentAttachment = bbAttachment;
				currentAttachmentName = nameTable[bbAttachment];
				return;
			}
			currentCollider = null;
			currentAttachment = bbAttachment;
			currentAttachmentName = null;
			if (DebugMessages)
			{
				Debug.LogFormat("Collider for BoundingBoxAttachment named '{0}' was not initialized. It is possibly from a new skin. currentAttachmentName will be null. You may need to call BoundingBoxFollower.Initialize(overwrite: true);", bbAttachment.Name);
			}
		}
	}
}
