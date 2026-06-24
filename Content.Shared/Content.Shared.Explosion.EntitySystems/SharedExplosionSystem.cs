using System;
using Content.Shared.Armor;
using Content.Shared.Explosion.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared.Explosion.EntitySystems;

public abstract class SharedExplosionSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ExplosionResistanceComponent, ArmorExamineEvent>((EntityEventRefHandler<ExplosionResistanceComponent, ArmorExamineEvent>)OnArmorExamine, (Type[])null, (Type[])null);
	}

	private void OnArmorExamine(Entity<ExplosionResistanceComponent> ent, ref ArmorExamineEvent args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float value = MathF.Round((1f - ent.Comp.DamageCoefficient) * 100f, 1);
		if (value != 0f)
		{
			args.Msg.PushNewline();
			args.Msg.AddMarkupOrThrow(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Examine), (ValueTuple<string, object>)("value", value)));
		}
	}

	public virtual void TriggerExplosive(EntityUid uid, ExplosiveComponent? explosive = null, bool delete = true, float? totalIntensity = null, float? radius = null, EntityUid? user = null)
	{
	}

	public void SetExplosionResistance(EntityUid uid, float damageCoefficient, bool worn, ExplosionResistanceComponent? resistance = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (resistance == null)
		{
			resistance = ((EntitySystem)this).EnsureComp<ExplosionResistanceComponent>(uid);
		}
		resistance.DamageCoefficient = damageCoefficient;
		resistance.Worn = worn;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)resistance, (MetaDataComponent)null);
	}
}
