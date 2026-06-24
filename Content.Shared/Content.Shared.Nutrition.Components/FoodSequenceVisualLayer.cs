using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Nutrition.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Nutrition.Components;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct FoodSequenceVisualLayer
{
	public SpriteSpecifier? Sprite { get; set; }

	public Vector2 Scale { get; set; }

	public Vector2 LocalOffset { get; set; }

	public ProtoId<FoodSequenceElementPrototype> Proto;

	public FoodSequenceVisualLayer(ProtoId<FoodSequenceElementPrototype> proto, SpriteSpecifier? sprite, Vector2 scale, Vector2 offset)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Sprite = SpriteSpecifier.Invalid;
		Scale = Vector2.One;
		LocalOffset = Vector2.Zero;
		Proto = proto;
		Sprite = sprite;
		Scale = scale;
		LocalOffset = offset;
	}

	[CompilerGenerated]
	private readonly bool PrintMembers(StringBuilder builder)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		builder.Append("Proto = ");
		builder.Append(((object)Proto/*cast due to constrained. prefix*/).ToString());
		builder.Append(", Sprite = ");
		builder.Append(Sprite);
		builder.Append(", Scale = ");
		builder.Append(Scale.ToString());
		builder.Append(", LocalOffset = ");
		builder.Append(LocalOffset.ToString());
		return true;
	}

	[CompilerGenerated]
	public override readonly int GetHashCode()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((EqualityComparer<ProtoId<FoodSequenceElementPrototype>>.Default.GetHashCode(Proto) * -1521134295 + EqualityComparer<SpriteSpecifier>.Default.GetHashCode(Sprite)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(Scale)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(LocalOffset);
	}

	[CompilerGenerated]
	public readonly bool Equals(FoodSequenceVisualLayer other)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (EqualityComparer<ProtoId<FoodSequenceElementPrototype>>.Default.Equals(Proto, other.Proto) && EqualityComparer<SpriteSpecifier>.Default.Equals(Sprite, other.Sprite) && EqualityComparer<Vector2>.Default.Equals(Scale, other.Scale))
		{
			return EqualityComparer<Vector2>.Default.Equals(LocalOffset, other.LocalOffset);
		}
		return false;
	}
}
