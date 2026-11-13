using Spine.Unity;

namespace Reference20 {
// Spine owns this mesh. A UI geometry rebuild must regenerate it rather than
// running Graphic's empty OnPopulateMesh implementation over the renderer.
public sealed class Reference20SkeletonGraphic : SkeletonGraphic {
 protected override void UpdateGeometry(){if(IsValid)UpdateMesh();}
}
}
