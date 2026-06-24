using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MassMedia.Components;

[Serializable]
[NetSerializable]
public sealed class NewsWriterDeleteMessage : BoundUserInterfaceMessage
{
	public readonly int ArticleNum;

	public NewsWriterDeleteMessage(int num)
	{
		ArticleNum = num;
	}
}
