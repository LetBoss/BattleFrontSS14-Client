using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction;

[Serializable]
[NetSerializable]
public sealed class ResponseConstructionGuide : EntityEventArgs
{
	public readonly string ConstructionId;

	public readonly ConstructionGuide Guide;

	public ResponseConstructionGuide(string constructionId, ConstructionGuide guide)
	{
		ConstructionId = constructionId;
		Guide = guide;
	}
}
