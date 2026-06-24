using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Administration.Logs;
using Content.Shared.Camera;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class RMCBattleExecuteSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _admin;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedCMChatSystem _rmcChat;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedCameraRecoilSystem _cameraRecoil;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCUnrevivableSystem _unrevivable;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MarineComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<MarineComponent, GetVerbsEvent<AlternativeVerb>>)AlternativeInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineComponent, RMCBattleExecuteEvent>((EntityEventRefHandler<MarineComponent, RMCBattleExecuteEvent>)ExecuteDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCBattleExecutedComponent, ExaminedEvent>((EntityEventRefHandler<RMCBattleExecutedComponent, ExaminedEvent>)ExamineBody, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCBattleExecuteComponent, ExaminedEvent>((EntityEventRefHandler<RMCBattleExecuteComponent, ExaminedEvent>)OnGunExecuteExamined, (Type[])null, (Type[])null);
	}

	private void OnGunExecuteExamined(Entity<RMCBattleExecuteComponent> ent, ref ExaminedEvent args)
	{
		using (args.PushGroup("RMCBattleExecuteComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-examine-text-execute"));
		}
	}

	private void AlternativeInteract(Entity<MarineComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		CombatModeComponent combatModeComponent = default(CombatModeComponent);
		RMCBattleExecuteComponent executionComponent = default(RMCBattleExecuteComponent);
		if (args.User != args.Target && _hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(args.User), out var handHeldItem) && ((EntitySystem)this).TryComp<CombatModeComponent>(args.User, ref combatModeComponent) && combatModeComponent.IsInCombatMode && ((EntitySystem)this).TryComp<RMCBattleExecuteComponent>(handHeldItem, ref executionComponent) && _skills.HasSkill(Entity<SkillsComponent>.op_Implicit(args.User), executionComponent.Skill, 1))
		{
			EntityUid target = args.Target;
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("rmc-execution"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					Execute(user, target, executionComponent, handHeldItem.Value);
				},
				Priority = 100
			});
		}
	}

	private void Execute(EntityUid user, EntityUid target, RMCBattleExecuteComponent executionComponent, EntityUid handHeldItem)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (_mobState.IsDead(target) && _unrevivable.IsUnrevivable(target))
		{
			string cancelledMessage = "You decide to not Execute " + ((EntitySystem)this).Name(target, (MetaDataComponent)null) + ", as they are already far beyond revival.";
			_popup.PopupClient(cancelledMessage, user, PopupType.MediumCaution);
			return;
		}
		RMCBattleExecuteEvent ev = new RMCBattleExecuteEvent(((EntitySystem)this).GetNetEntity(user, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null), executionComponent.Damage);
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, executionComponent.BattleExecuteTimeSeconds, ev, target, target, handHeldItem);
		_doAfter.TryStartDoAfter(doAfterArgs);
		string selfMsg = base.Loc.GetString("rmc-execute-start-self", (ValueTuple<string, object>)("target", ((EntitySystem)this).Name(target, (MetaDataComponent)null)), (ValueTuple<string, object>)("gun", ((EntitySystem)this).Name(handHeldItem, (MetaDataComponent)null)));
		string othersMsg = base.Loc.GetString("rmc-execute-start-others", new(string, object)[3]
		{
			("user", ((EntitySystem)this).Name(user, (MetaDataComponent)null)),
			("target", ((EntitySystem)this).Name(target, (MetaDataComponent)null)),
			("gun", ((EntitySystem)this).Name(handHeldItem, (MetaDataComponent)null))
		});
		_popup.PopupPredicted(selfMsg, othersMsg, user, user, PopupType.LargeCaution);
	}

	private void ExecuteDoAfter(Entity<MarineComponent> ent, ref RMCBattleExecuteEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((EntitySystem)this).GetEntity(args.User);
		EntityUid target = ((EntitySystem)this).GetEntity(args.Target);
		if (args.Cancelled)
		{
			ISharedAdminLogManager admin = _admin;
			LogStringHandler handler = new LogStringHandler(31, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler.AppendLiteral("'s Execution of ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "ToPrettyString(target)");
			handler.AppendLiteral(" was cancelled.");
			admin.Add(LogType.RMCExecution, LogImpact.High, ref handler);
			string cancelledMessage = "You decide to not Execute " + ((EntitySystem)this).Name(target, (MetaDataComponent)null) + ".";
			_popup.PopupClient(cancelledMessage, user, PopupType.MediumCaution);
		}
		else
		{
			if (((HandledEntityEventArgs)args).Handled)
			{
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			GunComponent gun = default(GunComponent);
			if (!((EntitySystem)this).Exists(args.Used) || !((EntitySystem)this).TryComp<GunComponent>(args.Used, ref gun))
			{
				return;
			}
			List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>();
			TakeAmmoEvent ev = new TakeAmmoEvent(1, ammo, ((EntitySystem)this).Transform(user).Coordinates, user);
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(args.Used.Value, ev, false);
			if (ev.Ammo.Count == 0)
			{
				ISharedAdminLogManager admin2 = _admin;
				LogStringHandler handler2 = new LogStringHandler(49, 2);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
				handler2.AppendLiteral("'s Execution of ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "ToPrettyString(target)");
				handler2.AppendLiteral(" was cancelled from lack of ammo.");
				admin2.Add(LogType.RMCExecution, LogImpact.High, ref handler2);
				_audio.PlayPredicted(gun.SoundEmpty, args.Used.Value, (EntityUid?)user, (AudioParams?)null);
				return;
			}
			foreach (var item in ev.Ammo)
			{
				EntityUid? bullet = item.Entity;
				((EntitySystem)this).Del(bullet);
			}
			ISharedAdminLogManager admin3 = _admin;
			LogStringHandler handler3 = new LogStringHandler(27, 2);
			handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler3.AppendLiteral("'s Execution of ");
			handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "ToPrettyString(target)");
			handler3.AppendLiteral(" Succeeded!");
			admin3.Add(LogType.RMCExecution, LogImpact.High, ref handler3);
			WieldableComponent wieldable = default(WieldableComponent);
			if (((EntitySystem)this).TryComp<WieldableComponent>(args.Used.Value, ref wieldable) && !wieldable.Wielded)
			{
				float recoilScalar = gun.CameraRecoilScalarModified;
				Vector2 userCoords = _transform.GetWorldPosition(user);
				Vector2 direction = _transform.GetWorldPosition(target) - userCoords;
				if (direction == Vector2.Zero)
				{
					direction = new Vector2(0f, -1f);
				}
				Vector2 kick = Vector2Helpers.Normalized(direction) * recoilScalar;
				_cameraRecoil.KickCamera(user, kick);
			}
			_damageable.TryChangeDamage(target, args.BattleExecuteDamage, ignoreResistances: true);
			_mobState.ChangeMobState(target, MobState.Dead);
			_unrevivable.MakeUnrevivable(Entity<RMCRevivableComponent>.op_Implicit(target));
			_audio.PlayPredicted(gun.SoundGunshotModified, args.Used.Value, (EntityUid?)user, (AudioParams?)null);
			string popupMessage = ((EntitySystem)this).Name(target, (MetaDataComponent)null) + " WAS EXECUTED BY " + ((EntitySystem)this).Name(user, (MetaDataComponent)null) + "!";
			_popup.PopupPredicted(popupMessage, target, user, PopupType.LargeCaution);
			string chatMsg = $"[bold][font size=24][color=red]\n{((EntitySystem)this).Name(target, (MetaDataComponent)null)} WAS EXECUTED BY {((EntitySystem)this).Name(user, (MetaDataComponent)null)}!\n[/color][/font][/bold]";
			MapCoordinates coordinates = _transform.GetMapCoordinates(target, (TransformComponent)null);
			Filter players = Filter.Empty().AddInRange(coordinates, 12f, _player, (IEntityManager)(object)base.EntityManager);
			players.RemoveWhereAttachedEntity((Predicate<EntityUid>)base.HasComp<XenoComponent>);
			_rmcChat.ChatMessageToMany(chatMsg, chatMsg, players, ChatChannel.Local);
			((EntitySystem)this).EnsureComp<RMCBattleExecutedComponent>(target);
		}
	}

	private void ExamineBody(Entity<RMCBattleExecutedComponent> ent, ref ExaminedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.ExecutedText), (ValueTuple<string, object>)("victim", ent.Owner)));
	}
}
