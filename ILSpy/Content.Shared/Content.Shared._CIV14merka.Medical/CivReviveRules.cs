using Content.Shared.FixedPoint;

namespace Content.Shared._CIV14merka.Medical;

public static class CivReviveRules
{
	public const float ReviveWindowMinutes = 4f;

	public static readonly FixedPoint2 MaxReviveDamage = FixedPoint2.New(300);

	public const float CorpseHealMultiplier = 2f;
}
