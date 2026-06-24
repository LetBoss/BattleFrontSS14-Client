using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MassMedia.Components;

[Serializable]
[NetSerializable]
public sealed class NewsWriterSaveDraftMessage : BoundUserInterfaceMessage
{
	public readonly string DraftTitle;

	public readonly string DraftContent;

	public NewsWriterSaveDraftMessage(string draftTitle, string draftContent)
	{
		DraftTitle = draftTitle;
		DraftContent = draftContent;
	}
}
