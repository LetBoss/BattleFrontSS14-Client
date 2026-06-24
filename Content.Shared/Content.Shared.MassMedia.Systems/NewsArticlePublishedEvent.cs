using Robust.Shared.GameObjects;

namespace Content.Shared.MassMedia.Systems;

[ByRefEvent]
public record struct NewsArticlePublishedEvent(NewsArticle Article);
