using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Interaction.Events;

public sealed class SuicideEvent : HandledEntityEventArgs
{
	public DamageSpecifier? DamageSpecifier;

	public ProtoId<DamageTypePrototype>? DamageType;

	public EntityUid Victim { get; private set; }

	public SuicideEvent(EntityUid victim)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Victim = victim;
	}
}
