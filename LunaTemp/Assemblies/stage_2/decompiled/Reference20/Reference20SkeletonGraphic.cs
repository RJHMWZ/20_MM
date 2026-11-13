using Spine.Unity;

namespace Reference20
{
	public sealed class Reference20SkeletonGraphic : SkeletonGraphic
	{
		protected override void UpdateGeometry()
		{
			if (base.IsValid)
			{
				UpdateMesh();
			}
		}
	}
}
