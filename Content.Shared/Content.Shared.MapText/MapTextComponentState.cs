using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.MapText;

[Serializable]
[NetSerializable]
public sealed class MapTextComponentState : ComponentState
{
	public string? Text { get; init; }

	public LocId LocText { get; init; }

	public Color Color { get; init; }

	public string FontId { get; init; }

	public int FontSize { get; init; }

	public Vector2 Offset { get; init; }
}
