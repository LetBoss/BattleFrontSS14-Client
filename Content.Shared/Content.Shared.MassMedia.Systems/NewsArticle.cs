using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.MassMedia.Systems;

[Serializable]
[NetSerializable]
public struct NewsArticle
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string Title;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string Content;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? Author;

	[ViewVariables]
	public ICollection<(NetEntity, uint)>? AuthorStationRecordKeyIds;

	[ViewVariables]
	public TimeSpan ShareTime;
}
