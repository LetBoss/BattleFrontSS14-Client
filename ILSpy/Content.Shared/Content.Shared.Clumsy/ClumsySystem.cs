using System;
using System.Collections.Generic;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.Hypospray.Events;
using Content.Shared.Climbing.Components;
using Content.Shared.Climbing.Events;
using Content.Shared.Damage;
using Content.Shared.IdentityManagement;
using Content.Shared.Medical;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Clumsy;

public sealed class ClumsySystem : EntitySystem
{
	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ClumsyComponent, SelfBeforeHyposprayInjectsEvent>((EntityEventRefHandler<ClumsyComponent, SelfBeforeHyposprayInjectsEvent>)BeforeHyposprayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClumsyComponent, SelfBeforeDefibrillatorZapsEvent>((EntityEventRefHandler<ClumsyComponent, SelfBeforeDefibrillatorZapsEvent>)BeforeDefibrillatorZapsEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClumsyComponent, SelfBeforeGunShotEvent>((EntityEventRefHandler<ClumsyComponent, SelfBeforeGunShotEvent>)BeforeGunShotEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClumsyComponent, CatchAttemptEvent>((EntityEventRefHandler<ClumsyComponent, CatchAttemptEvent>)OnCatchAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClumsyComponent, SelfBeforeClimbEvent>((EntityEventRefHandler<ClumsyComponent, SelfBeforeClimbEvent>)OnBeforeClimbEvent, (Type[])null, (Type[])null);
	}

	private void BeforeHyposprayEvent(Entity<ClumsyComponent> ent, ref SelfBeforeHyposprayInjectsEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ClumsyHypo && new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>
		{
			(int)_timing.CurTick.Value,
			((EntitySystem)this).GetNetEntity(Entity<ClumsyComponent>.op_Implicit(ent), (MetaDataComponent)null).Id
		})).NextDouble() < (double)ent.Comp.ClumsyDefaultCheck)
		{
			args.TargetGettingInjected = args.EntityUsingHypospray;
			args.InjectMessageOverride = base.Loc.GetString(LocId.op_Implicit(ent.Comp.HypoFailedMessage));
			_audio.PlayPredicted(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), (EntityUid?)args.EntityUsingHypospray, (AudioParams?)null);
		}
	}

	private void BeforeDefibrillatorZapsEvent(Entity<ClumsyComponent> ent, ref SelfBeforeDefibrillatorZapsEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ClumsyDefib && new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>
		{
			(int)_timing.CurTick.Value,
			((EntitySystem)this).GetNetEntity(Entity<ClumsyComponent>.op_Implicit(ent), (MetaDataComponent)null).Id
		})).NextDouble() < (double)ent.Comp.ClumsyDefaultCheck)
		{
			args.DefibTarget = args.EntityUsingDefib;
			_audio.PlayPvs(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), (AudioParams?)null);
		}
	}

	private void OnCatchAttempt(Entity<ClumsyComponent> ent, ref CatchAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ClumsyCatching && new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>
		{
			(int)_timing.CurTick.Value,
			((EntitySystem)this).GetNetEntity(args.Item, (MetaDataComponent)null).Id
		})).NextDouble() < (double)ent.Comp.ClumsyDefaultCheck)
		{
			args.Cancelled = true;
			if (ent.Comp.CatchingFailDamage != null)
			{
				_damageable.TryChangeDamage(Entity<ClumsyComponent>.op_Implicit(ent), ent.Comp.CatchingFailDamage, ignoreResistances: false, interruptsDoAfters: true, null, args.Item);
			}
			if (!_net.IsClient)
			{
				string selfMessage = base.Loc.GetString(LocId.op_Implicit(ent.Comp.CatchingFailedMessageSelf), (ValueTuple<string, object>)("item", ent.Owner), (ValueTuple<string, object>)("catcher", Identity.Entity(ent.Owner, (IEntityManager)(object)base.EntityManager)));
				string othersMessage = base.Loc.GetString(LocId.op_Implicit(ent.Comp.CatchingFailedMessageOthers), (ValueTuple<string, object>)("item", ent.Owner), (ValueTuple<string, object>)("catcher", Identity.Entity(ent.Owner, (IEntityManager)(object)base.EntityManager)));
				_popup.PopupEntity(selfMessage, ent.Owner, ent.Owner);
				_popup.PopupEntity(othersMessage, ent.Owner, Filter.PvsExcept(ent.Owner, 2f, (IEntityManager)null), recordReplay: true);
				_audio.PlayPvs(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), (AudioParams?)null);
			}
		}
	}

	private void BeforeGunShotEvent(Entity<ClumsyComponent> ent, ref SelfBeforeGunShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ClumsyGuns && !args.Gun.Comp.ClumsyProof && new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>
		{
			(int)_timing.CurTick.Value,
			((EntitySystem)this).GetNetEntity(Entity<GunComponent>.op_Implicit(args.Gun), (MetaDataComponent)null).Id
		})).NextDouble() < (double)ent.Comp.ClumsyDefaultCheck)
		{
			if (ent.Comp.GunShootFailDamage != null)
			{
				_damageable.TryChangeDamage(Entity<ClumsyComponent>.op_Implicit(ent), ent.Comp.GunShootFailDamage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<ClumsyComponent>.op_Implicit(ent));
			}
			_stun.TryParalyze(Entity<ClumsyComponent>.op_Implicit(ent), ent.Comp.GunShootFailStunTime, refresh: true);
			_audio.PlayPvs(ent.Comp.GunShootFailSound, Entity<ClumsyComponent>.op_Implicit(ent), (AudioParams?)null);
			_audio.PlayPvs(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), (AudioParams?)null);
			_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(ent.Comp.GunFailedMessage)), Entity<ClumsyComponent>.op_Implicit(ent), Entity<ClumsyComponent>.op_Implicit(ent));
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBeforeClimbEvent(Entity<ClumsyComponent> ent, ref SelfBeforeClimbEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.ClumsyVaulting)
		{
			return;
		}
		System.Random rand = new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>
		{
			(int)_timing.CurTick.Value,
			((EntitySystem)this).GetNetEntity(Entity<ClumsyComponent>.op_Implicit(ent), (MetaDataComponent)null).Id
		}));
		if (_cfg.GetCVar<bool>(CCVars.GameTableBonk) || rand.NextDouble() < (double)ent.Comp.ClumsyDefaultCheck)
		{
			HitHeadClumsy(ent, Entity<ClimbableComponent>.op_Implicit(args.BeingClimbedOn));
			_audio.PlayPredicted(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), (EntityUid?)Entity<ClumsyComponent>.op_Implicit(ent), (AudioParams?)null);
			_audio.PlayPredicted((SoundSpecifier)(object)ent.Comp.TableBonkSound, Entity<ClumsyComponent>.op_Implicit(ent), (EntityUid?)Entity<ClumsyComponent>.op_Implicit(ent), (AudioParams?)null);
			EntityUid gettingPutOnTableName = Identity.Entity(args.GettingPutOnTable, (IEntityManager)(object)base.EntityManager);
			EntityUid puttingOnTableName = Identity.Entity(args.PuttingOnTable, (IEntityManager)(object)base.EntityManager);
			if (args.PuttingOnTable == ent.Owner)
			{
				_popup.PopupPredicted(base.Loc.GetString(LocId.op_Implicit(ent.Comp.VaulingFailedMessageSelf), (ValueTuple<string, object>)("bonkable", args.BeingClimbedOn)), base.Loc.GetString(LocId.op_Implicit(ent.Comp.VaulingFailedMessageOthers), (ValueTuple<string, object>)("victim", gettingPutOnTableName), (ValueTuple<string, object>)("bonkable", args.BeingClimbedOn)), Entity<ClumsyComponent>.op_Implicit(ent), Entity<ClumsyComponent>.op_Implicit(ent));
			}
			else
			{
				_popup.PopupPredicted(base.Loc.GetString(LocId.op_Implicit(ent.Comp.VaulingFailedMessageForced), new(string, object)[3]
				{
					("bonker", puttingOnTableName),
					("victim", gettingPutOnTableName),
					("bonkable", args.BeingClimbedOn)
				}), Entity<ClumsyComponent>.op_Implicit(ent), null);
			}
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	public void HitHeadClumsy(Entity<ClumsyComponent> target, EntityUid table)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan stunTime = target.Comp.ClumsyDefaultStunTime;
		BonkableComponent bonkComp = default(BonkableComponent);
		if (((EntitySystem)this).TryComp<BonkableComponent>(table, ref bonkComp))
		{
			stunTime = bonkComp.BonkTime;
			if (bonkComp.BonkDamage != null)
			{
				_damageable.TryChangeDamage(Entity<ClumsyComponent>.op_Implicit(target), bonkComp.BonkDamage, ignoreResistances: true);
			}
		}
		_stun.TryParalyze(Entity<ClumsyComponent>.op_Implicit(target), stunTime, refresh: true);
	}
}
