using System;
using UnityEngine;

namespace Spine.Unity
{
	public class MeshRendererBuffers : IDisposable
	{
		public class SmartMesh : IDisposable
		{
			public Mesh mesh = SpineMesh.NewSkeletonMesh();

			public SkeletonRendererInstruction instructionUsed = new SkeletonRendererInstruction();

			public void Clear()
			{
				mesh.Clear();
				instructionUsed.Clear();
			}

			public void Dispose()
			{
				if (mesh != null)
				{
					UnityEngine.Object.Destroy(mesh);
				}
				mesh = null;
			}
		}

		private DoubleBuffered<SmartMesh> doubleBufferedMesh;

		internal readonly ExposedList<Material> submeshMaterials = new ExposedList<Material>();

		internal Material[] sharedMaterials = new Material[0];

		public void Initialize()
		{
			if (doubleBufferedMesh != null)
			{
				doubleBufferedMesh.GetNext().Clear();
				doubleBufferedMesh.GetNext().Clear();
				submeshMaterials.Clear();
			}
			else
			{
				doubleBufferedMesh = new DoubleBuffered<SmartMesh>();
			}
		}

		public Material[] GetUpdatedSharedMaterialsArray()
		{
			if (submeshMaterials.Count == sharedMaterials.Length)
			{
				submeshMaterials.CopyTo(sharedMaterials);
			}
			else
			{
				sharedMaterials = submeshMaterials.ToArray();
			}
			return sharedMaterials;
		}

		public bool MaterialsChangedInLastUpdate()
		{
			int newSubmeshMaterials = submeshMaterials.Count;
			Material[] sharedMaterials = this.sharedMaterials;
			if (newSubmeshMaterials != sharedMaterials.Length)
			{
				return true;
			}
			Material[] submeshMaterialsItems = submeshMaterials.Items;
			for (int i = 0; i < newSubmeshMaterials; i++)
			{
				if ((object)submeshMaterialsItems[i] != sharedMaterials[i])
				{
					return true;
				}
			}
			return false;
		}

		public void UpdateSharedMaterials(ExposedList<SubmeshInstruction> instructions)
		{
			int newSize = instructions.Count;
			if (newSize > submeshMaterials.Items.Length)
			{
				Array.Resize(ref submeshMaterials.Items, newSize);
			}
			submeshMaterials.Count = newSize;
			Material[] submeshMaterialsItems = submeshMaterials.Items;
			SubmeshInstruction[] instructionsItems = instructions.Items;
			for (int i = 0; i < newSize; i++)
			{
				submeshMaterialsItems[i] = instructionsItems[i].material;
			}
		}

		public SmartMesh GetNextMesh()
		{
			return doubleBufferedMesh.GetNext();
		}

		public void Clear()
		{
			sharedMaterials = new Material[0];
			submeshMaterials.Clear();
		}

		public void Dispose()
		{
			if (doubleBufferedMesh != null)
			{
				doubleBufferedMesh.GetNext().Dispose();
				doubleBufferedMesh.GetNext().Dispose();
				doubleBufferedMesh = null;
			}
		}
	}
}
