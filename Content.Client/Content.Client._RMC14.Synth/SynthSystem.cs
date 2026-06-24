using System;
using Content.Client.Damage;
using Content.Shared._RMC14.Synth;
using Content.Shared.Damage.Prototypes;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Synth;

public sealed class SynthSystem : SharedSynthSystem
{
	[Dependency]
	private DamageVisualsSystem _damageVisuals;

	private static readonly ProtoId<DamageGroupPrototype> GroupToChange = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SynthComponent, ComponentStartup>((EntityEventRefHandler<SynthComponent, ComponentStartup>)OnCompStartup, (Type[])null, (Type[])null);
	}

	protected override void MakeSynth(Entity<SynthComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.MakeSynth(ent);
	}

	private void OnCompStartup(Entity<SynthComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		DamageVisualsComponent damageVisuals = default(DamageVisualsComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(ent.Owner, ref item) && ((EntitySystem)this).TryComp<DamageVisualsComponent>(ent.Owner, ref damageVisuals))
		{
			_damageVisuals.ChangeDamageGroupColor(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), damageVisuals, ProtoId<DamageGroupPrototype>.op_Implicit(GroupToChange), ent.Comp.DamageVisualsColor);
		}
	}
}
