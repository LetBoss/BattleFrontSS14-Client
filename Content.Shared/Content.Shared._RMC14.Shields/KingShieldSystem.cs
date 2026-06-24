using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Shields;

public sealed class KingShieldSystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private XenoShieldSystem _shield;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private LineSystem _line;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private MobThresholdSystem _threshold;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<KingShieldComponent, DamageModifyAfterResistEvent>((EntityEventRefHandler<KingShieldComponent, DamageModifyAfterResistEvent>)OnShieldDamage, new Type[1] { typeof(XenoShieldSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KingShieldComponent, RemovedShieldEvent>((EntityEventRefHandler<KingShieldComponent, RemovedShieldEvent>)OnShieldRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KingLightningComponent, MoveEvent>((EntityEventRefHandler<KingLightningComponent, MoveEvent>)OnLightningMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KingLightningComponent, ComponentShutdown>((EntityEventRefHandler<KingLightningComponent, ComponentShutdown>)OnLightningRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBulwarkOfTheHiveComponent, MoveEvent>((EntityEventRefHandler<XenoBulwarkOfTheHiveComponent, MoveEvent>)OnBulwarkMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBulwarkOfTheHiveComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoBulwarkOfTheHiveComponent, EntityTerminatingEvent>)OnBulwarkDelete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBulwarkOfTheHiveComponent, XenoBulwarkOfTheHiveActionEvent>((EntityEventRefHandler<XenoBulwarkOfTheHiveComponent, XenoBulwarkOfTheHiveActionEvent>)OnXenoBulwarkOfTheHiveAction, (Type[])null, (Type[])null);
	}

	private void OnXenoBulwarkOfTheHiveAction(Entity<XenoBulwarkOfTheHiveComponent> xeno, ref XenoBulwarkOfTheHiveActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoBulwarkOfTheHiveComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoBulwarkOfTheHiveComponent>.op_Implicit(xeno), (AudioParams?)null);
		((EntitySystem)this).EnsureComp<KingShieldComponent>(Entity<XenoBulwarkOfTheHiveComponent>.op_Implicit(xeno));
		_shield.ApplyShield(Entity<XenoBulwarkOfTheHiveComponent>.op_Implicit(xeno), XenoShieldSystem.ShieldType.King, xeno.Comp.ShieldAmount, xeno.Comp.DecayTime, xeno.Comp.DecayAmount, addShield: false, 200.0, xeno.Comp.VisualState);
		_ = _timing.CurTime;
		foreach (Entity<XenoComponent> ent in _entityLookup.GetEntitiesInRange<XenoComponent>(_transform.GetMapCoordinates(Entity<XenoBulwarkOfTheHiveComponent>.op_Implicit(xeno), (TransformComponent)null), xeno.Comp.Range, (LookupFlags)110))
		{
			if (!_mob.IsDead(Entity<XenoComponent>.op_Implicit(ent)) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(ent.Owner)))
			{
				((EntitySystem)this).EnsureComp<KingShieldComponent>(Entity<XenoComponent>.op_Implicit(ent));
				_shield.ApplyShield(Entity<XenoComponent>.op_Implicit(ent), XenoShieldSystem.ShieldType.King, xeno.Comp.ShieldAmount, xeno.Comp.DecayTime, xeno.Comp.DecayAmount, addShield: false, 200.0, xeno.Comp.VisualState);
			}
		}
	}

	private void OnShieldDamage(Entity<KingShieldComponent> xeno, ref DamageModifyAfterResistEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		XenoShieldComponent shield = default(XenoShieldComponent);
		if (((EntitySystem)this).TryComp<XenoShieldComponent>(Entity<KingShieldComponent>.op_Implicit(xeno), ref shield) && shield.Active && shield.Shield == XenoShieldSystem.ShieldType.King)
		{
			if (!_threshold.TryGetIncapThreshold(Entity<KingShieldComponent>.op_Implicit(xeno), out var threshold))
			{
				_threshold.TryGetDeadThreshold(Entity<KingShieldComponent>.op_Implicit(xeno), out threshold);
			}
			FixedPoint2? maxDamage = threshold * xeno.Comp.MaxDamagePercent;
			if (maxDamage.HasValue)
			{
				args.Damage.ClampMax(maxDamage.Value);
			}
		}
	}

	private void OnShieldRemove(Entity<KingShieldComponent> xeno, ref RemovedShieldEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Type == XenoShieldSystem.ShieldType.King)
		{
			((EntitySystem)this).RemCompDeferred<KingShieldComponent>(Entity<KingShieldComponent>.op_Implicit(xeno));
		}
	}

	public void UpdateTrail(Entity<KingLightningComponent> ent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		KingLightningComponent lightning = ent.Comp;
		if (!lightning.StopUpdating)
		{
			if (lightning.Trail.Count != 0)
			{
				_line.DeleteBeam(lightning.Trail);
			}
			if (_line.TryCreateLine(lightning.Source, Entity<KingLightningComponent>.op_Implicit(ent), EntProtoId.op_Implicit(lightning.Lightning), out List<EntityUid> lines))
			{
				lightning.Trail = lines;
			}
		}
	}

	private void OnBulwarkMove(Entity<XenoBulwarkOfTheHiveComponent> xeno, ref MoveEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Supporting.Count == 0)
		{
			return;
		}
		List<EntityUid> toRemove = new List<EntityUid>();
		KingLightningComponent hookComp = default(KingLightningComponent);
		foreach (EntityUid supported in xeno.Comp.Supporting)
		{
			if (!((EntitySystem)this).TryComp<KingLightningComponent>(supported, ref hookComp))
			{
				toRemove.Add(supported);
			}
			else
			{
				UpdateTrail(Entity<KingLightningComponent>.op_Implicit((supported, hookComp)));
			}
		}
		foreach (EntityUid ent in toRemove)
		{
			xeno.Comp.Supporting.Remove(ent);
		}
	}

	private void OnBulwarkDelete(Entity<XenoBulwarkOfTheHiveComponent> xeno, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Supporting.Count == 0)
		{
			return;
		}
		foreach (EntityUid support in xeno.Comp.Supporting)
		{
			((EntitySystem)this).RemCompDeferred<KingLightningComponent>(support);
		}
		xeno.Comp.Supporting.Clear();
	}

	private void OnLightningMove(Entity<KingLightningComponent> xeno, ref MoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateTrail(xeno);
	}

	private void OnLightningRemoved(Entity<KingLightningComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		XenoBulwarkOfTheHiveComponent hookSource = default(XenoBulwarkOfTheHiveComponent);
		if (((EntitySystem)this).TryComp<XenoBulwarkOfTheHiveComponent>(ent.Comp.Source, ref hookSource))
		{
			hookSource.Supporting.Remove(Entity<KingLightningComponent>.op_Implicit(ent));
		}
		ent.Comp.StopUpdating = true;
		((EntitySystem)this).Dirty<KingLightningComponent>(ent, (MetaDataComponent)null);
		_line.DeleteBeam(ent.Comp.Trail);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<KingLightningComponent> query = ((EntitySystem)this).EntityQueryEnumerator<KingLightningComponent>();
		EntityUid uid = default(EntityUid);
		KingLightningComponent lightning = default(KingLightningComponent);
		while (query.MoveNext(ref uid, ref lightning))
		{
			if (!(time < lightning.DisappearAt))
			{
				((EntitySystem)this).RemCompDeferred<KingLightningComponent>(uid);
			}
		}
	}
}
