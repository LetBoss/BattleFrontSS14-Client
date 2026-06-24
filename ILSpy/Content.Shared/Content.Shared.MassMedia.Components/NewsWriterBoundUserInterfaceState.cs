using System;
using Content.Shared.MassMedia.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MassMedia.Components;

[Serializable]
[NetSerializable]
public sealed class NewsWriterBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly NewsArticle[] Articles;

	public readonly bool PublishEnabled;

	public readonly TimeSpan NextPublish;

	public readonly string DraftTitle;

	public readonly string DraftContent;

	public NewsWriterBoundUserInterfaceState(NewsArticle[] articles, bool publishEnabled, TimeSpan nextPublish, string draftTitle, string draftContent)
	{
		Articles = articles;
		PublishEnabled = publishEnabled;
		NextPublish = nextPublish;
		DraftTitle = draftTitle;
		DraftContent = draftContent;
	}
}
