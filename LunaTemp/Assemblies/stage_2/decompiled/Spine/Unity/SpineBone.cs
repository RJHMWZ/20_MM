namespace Spine.Unity
{
	public class SpineBone : SpineAttributeBase
	{
		public SpineBone(string startsWith = "", string dataField = "", bool includeNone = true, bool fallbackToTextField = false)
		{
			base.startsWith = startsWith;
			base.dataField = dataField;
			base.includeNone = includeNone;
			base.fallbackToTextField = fallbackToTextField;
		}

		public static Bone GetBone(string boneName, SkeletonRenderer renderer)
		{
			return (renderer.skeleton == null) ? null : renderer.skeleton.FindBone(boneName);
		}

		public static BoneData GetBoneData(string boneName, SkeletonDataAsset skeletonDataAsset)
		{
			SkeletonData data = skeletonDataAsset.GetSkeletonData(true);
			return data.FindBone(boneName);
		}
	}
}
