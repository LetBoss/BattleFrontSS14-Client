using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared.EntityEffects;

public record EntityEffectReagentArgs : EntityEffectBaseArgs
{
	public EntityUid? OrganEntity;

	public Solution? Source;

	public FixedPoint2 Quantity;

	public ReagentPrototype? Reagent;

	public ReactionMethod? Method;

	public FixedPoint2 Scale;

	public EntityEffectReagentArgs(EntityUid targetEntity, IEntityManager entityManager, EntityUid? organEntity, Solution? source, FixedPoint2 quantity, ReagentPrototype? reagent, ReactionMethod? method, FixedPoint2 scale)
		: base(targetEntity, entityManager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OrganEntity = organEntity;
		Source = source;
		Quantity = quantity;
		Reagent = reagent;
		Method = method;
		Scale = scale;
	}
}
