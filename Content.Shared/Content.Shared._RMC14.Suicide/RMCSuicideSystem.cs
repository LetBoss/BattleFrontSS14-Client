using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Suicide;

public sealed class RMCSuicideSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _admin;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private RMCUnrevivableSystem _unrevivable;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCSuicideComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<RMCSuicideComponent, GetVerbsEvent<Verb>>)OnSuicideGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSuicideComponent, RMCSuicideDoAfterEvent>((EntityEventRefHandler<RMCSuicideComponent, RMCSuicideDoAfterEvent>)OnSuicideDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHasSuicidedComponent, UpdateMobStateEvent>((EntityEventRefHandler<RMCHasSuicidedComponent, UpdateMobStateEvent>)OnHasSuicidedUpdateMobState, (Type[])null, (Type[])null);
	}

	private void OnSuicideGetVerbs(Entity<RMCSuicideComponent> ent, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanInteract)
		{
			return;
		}
		EntityUid user = args.User;
		if (user != args.Target || args.Hands == null || !_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(args.Target), out var active) || !((EntitySystem)this).HasComp<GunComponent>(active))
		{
			return;
		}
		args.Verbs.Add(new Verb
		{
			Text = base.Loc.GetString("rmc-suicide"),
			Act = delegate
			{
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
				TimeSpan curTime = _timing.CurTime;
				if (curTime < ent.Comp.LastAttempt + ent.Comp.Cooldown)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-suicide-fumble-self"), user, user, PopupType.SmallCaution);
				}
				else
				{
					ent.Comp.LastAttempt = curTime;
					RMCSuicideDoAfterEvent rMCSuicideDoAfterEvent = new RMCSuicideDoAfterEvent();
					DoAfterArgs args2 = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.Delay, rMCSuicideDoAfterEvent, user)
					{
						BreakOnMove = true,
						NeedHand = true,
						BreakOnHandChange = true,
						ForceVisible = true
					};
					if (_doAfter.TryStartDoAfter(args2))
					{
						ISharedAdminLogManager admin = _admin;
						LogStringHandler handler = new LogStringHandler(20, 1);
						handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
						handler.AppendLiteral(" started to suicide.");
						admin.Add(LogType.RMCSuicide, LogImpact.High, ref handler);
						string recipientMessage = base.Loc.GetString("rmc-suicide-start-self");
						string othersMessage = base.Loc.GetString("rmc-suicide-start-others", (ValueTuple<string, object>)("user", user));
						_popup.PopupPredicted(recipientMessage, othersMessage, user, user, PopupType.LargeCaution);
					}
				}
			}
		});
	}

	private void OnSuicideDoAfter(Entity<RMCSuicideComponent> ent, ref RMCSuicideDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (args.Cancelled)
		{
			ISharedAdminLogManager admin = _admin;
			LogStringHandler handler = new LogStringHandler(25, 1);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler.AppendLiteral("'s suicide was cancelled.");
			admin.Add(LogType.RMCSuicide, LogImpact.High, ref handler);
			string selfMsg = base.Loc.GetString("rmc-suicide-cancel-self");
			string othersMsg = base.Loc.GetString("rmc-suicide-cancel-others", (ValueTuple<string, object>)("user", user));
			_popup.PopupPredicted(selfMsg, othersMsg, user, user, PopupType.MediumCaution);
		}
		else
		{
			if (((HandledEntityEventArgs)args).Handled)
			{
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(user));
			if (activeItem.HasValue)
			{
				EntityUid held = activeItem.GetValueOrDefault();
				GunComponent gun = default(GunComponent);
				if (((EntitySystem)this).TryComp<GunComponent>(held, ref gun))
				{
					List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>();
					TakeAmmoEvent ev = new TakeAmmoEvent(1, ammo, ((EntitySystem)this).Transform(user).Coordinates, user);
					((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(held, ev, false);
					if (ev.Ammo.Count == 0)
					{
						ISharedAdminLogManager admin2 = _admin;
						LogStringHandler handler2 = new LogStringHandler(28, 1);
						handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
						handler2.AppendLiteral(" failed to suicide: no ammo.");
						admin2.Add(LogType.RMCSuicide, LogImpact.High, ref handler2);
						_audio.PlayPredicted(gun.SoundEmpty, held, (EntityUid?)Entity<RMCSuicideComponent>.op_Implicit(ent), (AudioParams?)null);
						return;
					}
					foreach (var item in ev.Ammo)
					{
						EntityUid? bullet = item.Entity;
						((EntitySystem)this).QueueDel(bullet);
					}
					ISharedAdminLogManager admin3 = _admin;
					LogStringHandler handler3 = new LogStringHandler(10, 1);
					handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
					handler3.AppendLiteral(" suicided.");
					admin3.Add(LogType.RMCSuicide, LogImpact.High, ref handler3);
					_damageable.TryChangeDamage(user, ent.Comp.Damage, ignoreResistances: true);
					_mobState.ChangeMobState(user, MobState.Dead);
					_unrevivable.MakeUnrevivable(Entity<RMCRevivableComponent>.op_Implicit(user));
					_audio.PlayPredicted(gun.SoundGunshot, held, (EntityUid?)Entity<RMCSuicideComponent>.op_Implicit(ent), (AudioParams?)null);
					_unrevivable.MakeUnrevivable(Entity<RMCRevivableComponent>.op_Implicit(user), killLarva: false);
					((EntitySystem)this).EnsureComp<RMCHasSuicidedComponent>(user);
					return;
				}
			}
			ISharedAdminLogManager admin4 = _admin;
			LogStringHandler handler4 = new LogStringHandler(27, 1);
			handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler4.AppendLiteral(" failed to suicide: no gun.");
			admin4.Add(LogType.RMCSuicide, LogImpact.High, ref handler4);
		}
	}

	private void OnHasSuicidedUpdateMobState(Entity<RMCHasSuicidedComponent> ent, ref UpdateMobStateEvent args)
	{
		args.State = MobState.Dead;
	}
}
