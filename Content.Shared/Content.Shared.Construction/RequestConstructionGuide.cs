using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction;

[Serializable]
[NetSerializable]
public sealed class RequestConstructionGuide : EntityEventArgs
{
	public readonly string ConstructionId;

	public RequestConstructionGuide(string constructionId)
	{
		ConstructionId = constructionId;
	}
}
