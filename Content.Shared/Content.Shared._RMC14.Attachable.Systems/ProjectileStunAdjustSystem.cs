using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Stun;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class ProjectileStunAdjustSystem : EntitySystem
{
	private const string ModifierExamineColour = "yellow";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ProjectileStunAdjustComponent, AmmoShotEvent>((EntityEventRefHandler<ProjectileStunAdjustComponent, AmmoShotEvent>)OnAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantProjectileStunAdjustComponent, AttachableAlteredEvent>((EntityEventRefHandler<GrantProjectileStunAdjustComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantProjectileStunAdjustComponent, AttachableGetExamineDataEvent>((EntityEventRefHandler<GrantProjectileStunAdjustComponent, AttachableGetExamineDataEvent>)OnGrantProjectileStunAdjustmentGetExamineData, (Type[])null, (Type[])null);
	}

	private void OnAmmoShot(Entity<ProjectileStunAdjustComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		RMCStunOnHitComponent stunComp = default(RMCStunOnHitComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (((EntitySystem)this).TryComp<RMCStunOnHitComponent>(projectile, ref stunComp))
			{
				Span<RMCStunOnHit> stuns = CollectionsMarshal.AsSpan(stunComp.Stuns);
				for (int i = 0; i < stuns.Length; i++)
				{
					ref RMCStunOnHit reference = ref stuns[i];
					reference.StunTime *= (double)ent.Comp.StunDurationAdjustment;
					reference.DazeTime *= (double)ent.Comp.DazeDurationAdjustment;
					reference.MaxRange *= ent.Comp.MaxRangeAdjustment;
					reference.ForceKnockBack = ent.Comp.ForceKnockBackAdjustment;
					reference.KnockBackPowerMin *= ent.Comp.KnockBackPowerMinAdjustment;
					reference.KnockBackPowerMax *= ent.Comp.KnockBackPowerMaxAdjustment;
					reference.LosesEffectWithRange = ent.Comp.LosesEffectWithRangeAdjustment;
					reference.SlowsEffectBigXenos = ent.Comp.SlowsEffectBigXenosAdjustment;
					reference.SuperSlowTime *= (double)ent.Comp.SuperSlowTimeAdjustment;
					reference.SlowTime *= (double)ent.Comp.SlowTimeAdjustment;
					reference.StunArea += ent.Comp.StunAreaAdjustment;
				}
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)stunComp, (MetaDataComponent)null);
			}
		}
	}

	private void OnAttachableAltered(Entity<GrantProjectileStunAdjustComponent> ent, ref AttachableAlteredEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		switch (args.Alteration)
		{
		case AttachableAlteredType.Attached:
		{
			ProjectileStunAdjustComponent stunAdjust = ((EntitySystem)this).EnsureComp<ProjectileStunAdjustComponent>(args.Holder);
			stunAdjust.StunDurationAdjustment = ent.Comp.StunDurationAdjustment;
			stunAdjust.DazeDurationAdjustment = ent.Comp.DazeDurationAdjustment;
			stunAdjust.MaxRangeAdjustment = ent.Comp.MaxRangeAdjustment;
			stunAdjust.ForceKnockBackAdjustment = ent.Comp.ForceKnockBackAdjustment;
			stunAdjust.KnockBackPowerMinAdjustment = ent.Comp.KnockBackPowerMinAdjustment;
			stunAdjust.KnockBackPowerMaxAdjustment = ent.Comp.KnockBackPowerMaxAdjustment;
			stunAdjust.LosesEffectWithRangeAdjustment = ent.Comp.LosesEffectWithRangeAdjustment;
			stunAdjust.SlowsEffectBigXenosAdjustment = ent.Comp.SlowsEffectBigXenosAdjustment;
			stunAdjust.SuperSlowTimeAdjustment = ent.Comp.SuperSlowTimeAdjustment;
			stunAdjust.SlowTimeAdjustment = ent.Comp.SlowTimeAdjustment;
			stunAdjust.StunAreaAdjustment = ent.Comp.StunAreaAdjustment;
			((EntitySystem)this).Dirty(args.Holder, (IComponent)(object)stunAdjust, (MetaDataComponent)null);
			break;
		}
		case AttachableAlteredType.Detached:
			((EntitySystem)this).RemComp<ProjectileStunAdjustComponent>(args.Holder);
			break;
		}
	}

	private void OnGrantProjectileStunAdjustmentGetExamineData(Entity<GrantProjectileStunAdjustComponent> attachable, ref AttachableGetExamineDataEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		List<string> effects = new List<string>();
		float stunDurationAdjustment = attachable.Comp.StunDurationAdjustment;
		if ((stunDurationAdjustment >= 1.01f || stunDurationAdjustment <= 0.99f) ? true : false)
		{
			effects.Add(base.Loc.GetString("rmc-attachable-examine-ranged-projectile-stun-duration", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (attachable.Comp.StunDurationAdjustment > 1f) ? ((object)'+') : ""),
				("stunDurationMult", attachable.Comp.StunDurationAdjustment - 1f)
			}));
		}
		if (!args.Data.ContainsKey(0))
		{
			args.Data[0] = (null, effects);
		}
		else
		{
			args.Data[0].effectStrings.AddRange(effects);
		}
	}
}
