using UnityEngine;

namespace Spine.Unity
{
	public static class SpineMesh
	{
		internal const HideFlags MeshHideflags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

		public static Mesh NewSkeletonMesh()
		{
			Mesh i = new Mesh();
			i.MarkDynamic();
			i.name = "Skeleton Mesh";
			i.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
			return i;
		}
	}
}
