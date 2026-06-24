using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.Whitelist;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAEDamageInAreaSystem : BaseXAESystem<XAEDamageInAreaComponent>
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private IGameTiming _timing;

	private readonly HashSet<EntityUid> _entitiesInRange = new HashSet<EntityUid>();

	protected override void OnActivated(Entity<XAEDamageInAreaComponent> ent, ref XenoArtifactNodeActivatedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		XAEDamageInAreaComponent damageInAreaComponent = ent.Comp;
		_entitiesInRange.Clear();
		_lookup.GetEntitiesInRange(ent.Owner, damageInAreaComponent.Radius, _entitiesInRange, (LookupFlags)110);
		foreach (EntityUid entityInRange in _entitiesInRange)
		{
			if (RandomExtensions.Prob(_random, damageInAreaComponent.DamageChance) && !_whitelistSystem.IsWhitelistFail(damageInAreaComponent.Whitelist, entityInRange))
			{
				_damageable.TryChangeDamage(entityInRange, damageInAreaComponent.Damage, damageInAreaComponent.IgnoreResistances);
			}
		}
	}
}
