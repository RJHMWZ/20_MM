namespace Spine
{
	public class BoundingBoxAttachment : VertexAttachment
	{
		public BoundingBoxAttachment(string name)
			: base(name)
		{
		}

		public override Attachment Copy()
		{
			BoundingBoxAttachment copy = new BoundingBoxAttachment(base.Name);
			CopyTo(copy);
			return copy;
		}
	}
}
