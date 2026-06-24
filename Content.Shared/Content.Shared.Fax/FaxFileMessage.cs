using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax;

[Serializable]
[NetSerializable]
public sealed class FaxFileMessage : BoundUserInterfaceMessage
{
	public string? Label;

	public string Content;

	public bool OfficePaper;

	public FaxFileMessage(string? label, string content, bool officePaper)
	{
		Label = label;
		Content = content;
		OfficePaper = officePaper;
	}
}
