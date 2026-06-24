using System;
using Robust.Shared.Maths;

namespace Robust.Shared.Toolshed.TypeParsers.Math;

public sealed class Box2iTypeParser : SpanLikeTypeParser<Box2i, int>
{
	public override int Elements => 4;

	public override Box2i Create(Span<int> elements)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return new Box2i(elements[0], elements[1], elements[2], elements[3]);
	}
}
