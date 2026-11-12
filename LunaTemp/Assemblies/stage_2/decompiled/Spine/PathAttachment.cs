using System;

namespace Spine
{
	public class PathAttachment : VertexAttachment
	{
		internal float[] lengths;

		internal bool closed;

		internal bool constantSpeed;

		public float[] Lengths
		{
			get
			{
				return lengths;
			}
			set
			{
				lengths = value;
			}
		}

		public bool Closed
		{
			get
			{
				return closed;
			}
			set
			{
				closed = value;
			}
		}

		public bool ConstantSpeed
		{
			get
			{
				return constantSpeed;
			}
			set
			{
				constantSpeed = value;
			}
		}

		public PathAttachment(string name)
			: base(name)
		{
		}

		public override Attachment Copy()
		{
			PathAttachment copy = new PathAttachment(base.Name);
			CopyTo(copy);
			copy.lengths = new float[lengths.Length];
			Array.Copy(lengths, 0, copy.lengths, 0, lengths.Length);
			copy.closed = closed;
			copy.constantSpeed = constantSpeed;
			return copy;
		}
	}
}
