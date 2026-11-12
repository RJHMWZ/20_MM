namespace Spine.Unity
{
	public class SkeletonRendererInstruction
	{
		public readonly ExposedList<SubmeshInstruction> submeshInstructions = new ExposedList<SubmeshInstruction>();

		public bool immutableTriangles;

		public bool hasActiveClipping;

		public int rawVertexCount = -1;

		public readonly ExposedList<Attachment> attachments = new ExposedList<Attachment>();

		public void Clear()
		{
			attachments.Clear(false);
			rawVertexCount = -1;
			hasActiveClipping = false;
			submeshInstructions.Clear(false);
		}

		public void Dispose()
		{
			attachments.Clear();
		}

		public void SetWithSubset(ExposedList<SubmeshInstruction> instructions, int startSubmesh, int endSubmesh)
		{
			int runningVertexCount = 0;
			ExposedList<SubmeshInstruction> submeshes = submeshInstructions;
			submeshes.Clear(false);
			int submeshCount = endSubmesh - startSubmesh;
			submeshes.Resize(submeshCount);
			SubmeshInstruction[] submeshesItems = submeshes.Items;
			SubmeshInstruction[] instructionsItems = instructions.Items;
			for (int j = 0; j < submeshCount; j++)
			{
				SubmeshInstruction instruction = (submeshesItems[j] = instructionsItems[startSubmesh + j]);
				hasActiveClipping |= instruction.hasClipping;
				submeshesItems[j].rawFirstVertexIndex = runningVertexCount;
				runningVertexCount += instruction.rawVertexCount;
			}
			rawVertexCount = runningVertexCount;
			int startSlot = instructionsItems[startSubmesh].startSlot;
			int endSlot = instructionsItems[endSubmesh - 1].endSlot;
			attachments.Clear(false);
			int attachmentCount = endSlot - startSlot;
			attachments.Resize(attachmentCount);
			Attachment[] attachmentsItems = attachments.Items;
			Slot[] drawOrderItems = instructionsItems[0].skeleton.drawOrder.Items;
			for (int i = 0; i < attachmentCount; i++)
			{
				Slot slot = drawOrderItems[startSlot + i];
				if (slot.bone.active)
				{
					attachmentsItems[i] = slot.attachment;
				}
			}
		}

		public void Set(SkeletonRendererInstruction other)
		{
			immutableTriangles = other.immutableTriangles;
			hasActiveClipping = other.hasActiveClipping;
			rawVertexCount = other.rawVertexCount;
			attachments.Clear(false);
			attachments.EnsureCapacity(other.attachments.Capacity);
			attachments.Count = other.attachments.Count;
			other.attachments.CopyTo(attachments.Items);
			submeshInstructions.Clear(false);
			submeshInstructions.EnsureCapacity(other.submeshInstructions.Capacity);
			submeshInstructions.Count = other.submeshInstructions.Count;
			other.submeshInstructions.CopyTo(submeshInstructions.Items);
		}

		public static bool GeometryNotEqual(SkeletonRendererInstruction a, SkeletonRendererInstruction b)
		{
			if (a.hasActiveClipping || b.hasActiveClipping)
			{
				return true;
			}
			if (a.rawVertexCount != b.rawVertexCount)
			{
				return true;
			}
			if (a.immutableTriangles != b.immutableTriangles)
			{
				return true;
			}
			int attachmentCountB = b.attachments.Count;
			if (a.attachments.Count != attachmentCountB)
			{
				return true;
			}
			int submeshCountA = a.submeshInstructions.Count;
			int submeshCountB = b.submeshInstructions.Count;
			if (submeshCountA != submeshCountB)
			{
				return true;
			}
			SubmeshInstruction[] submeshInstructionsItemsA = a.submeshInstructions.Items;
			SubmeshInstruction[] submeshInstructionsItemsB = b.submeshInstructions.Items;
			Attachment[] attachmentsA = a.attachments.Items;
			Attachment[] attachmentsB = b.attachments.Items;
			for (int j = 0; j < attachmentCountB; j++)
			{
				if (attachmentsA[j] != attachmentsB[j])
				{
					return true;
				}
			}
			for (int i = 0; i < submeshCountB; i++)
			{
				SubmeshInstruction submeshA = submeshInstructionsItemsA[i];
				SubmeshInstruction submeshB = submeshInstructionsItemsB[i];
				if (submeshA.rawVertexCount != submeshB.rawVertexCount || submeshA.startSlot != submeshB.startSlot || submeshA.endSlot != submeshB.endSlot || submeshA.rawTriangleCount != submeshB.rawTriangleCount || submeshA.rawFirstVertexIndex != submeshB.rawFirstVertexIndex)
				{
					return true;
				}
			}
			return false;
		}
	}
}
