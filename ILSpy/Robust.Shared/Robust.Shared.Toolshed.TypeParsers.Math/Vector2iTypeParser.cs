using System;
using Robust.Shared.Maths;

namespace Robust.Shared.Toolshed.TypeParsers.Math;

internal sealed class Vector2iTypeParser : SpanLikeTypeParser<Vector2i, int>
{
	public override int Elements => 2;

	public override Vector2i Create(Span<int> elements)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i(elements[0], elements[1]);
	}
}
