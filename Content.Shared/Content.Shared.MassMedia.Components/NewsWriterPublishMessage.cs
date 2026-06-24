using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MassMedia.Components;

[Serializable]
[NetSerializable]
public sealed class NewsWriterPublishMessage : BoundUserInterfaceMessage
{
	public readonly string Title;

	public readonly string Content;

	public NewsWriterPublishMessage(string title, string content)
	{
		Title = title;
		Content = content;
	}
}
