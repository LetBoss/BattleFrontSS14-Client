using System;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Projectile.Parasite;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Construction.EggMorpher;

public sealed class EggMorpherSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _time;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedXenoParasiteSystem _parasite;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EggMorpherComponent, ExaminedEvent>((EntityEventRefHandler<EggMorpherComponent, ExaminedEvent>)OnExamineEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EggMorpherComponent, InteractHandEvent>((EntityEventRefHandler<EggMorpherComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EggMorpherComponent, InteractUsingEvent>((EntityEventRefHandler<EggMorpherComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EggMorpherComponent, XenoChangeParasiteReserveMessage>((EntityEventRefHandler<EggMorpherComponent, XenoChangeParasiteReserveMessage>)OnChangeParasiteReserve, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EggMorpherComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<EggMorpherComponent, GetVerbsEvent<ActivationVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EggMorpherComponent, StepTriggerAttemptEvent>((EntityEventRefHandler<EggMorpherComponent, StepTriggerAttemptEvent>)OnEggMorpherStepAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EggMorpherComponent, StepTriggeredOffEvent>((EntityEventRefHandler<EggMorpherComponent, StepTriggeredOffEvent>)OnEggMorpherStepTriggered, (Type[])null, (Type[])null);
	}

	private void OnExamineEvent(Entity<EggMorpherComponent> eggMorpher, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("EggMorpherComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-construction-egg-morpher-examine", (ValueTuple<string, object>)("cur_paras", eggMorpher.Comp.CurParasites), (ValueTuple<string, object>)("max_paras", eggMorpher.Comp.MaxParasites)));
		}
	}

	private void OnInteractHand(Entity<EggMorpherComponent> eggMorpher, ref InteractHandEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		EntityUid user = args.User;
		EntityUid? parasite;
		if (((EntitySystem)this).HasComp<XenoParasiteComponent>(user))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (eggMorpher.Comp.MaxParasites <= eggMorpher.Comp.CurParasites)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-egg-morpher-already-full"), Entity<EggMorpherComponent>.op_Implicit(eggMorpher), user);
			}
			else if (!_mobState.IsDead(user) && !_net.IsClient)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-morpher-return-self", (ValueTuple<string, object>)("parasite", user)), Entity<EggMorpherComponent>.op_Implicit(eggMorpher));
				((EntitySystem)this).QueueDel((EntityUid?)user);
				eggMorpher.Comp.CurParasites++;
				_appearance.SetData(Entity<EggMorpherComponent>.op_Implicit(eggMorpher), (Enum)EggmorpherOverlayVisuals.Number, (object)eggMorpher.Comp.CurParasites, (AppearanceComponent)null);
			}
		}
		else if (!TryCreateParasiteFromEggMorpher(eggMorpher, out parasite))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-egg-morpher-no-parasites"), Entity<EggMorpherComponent>.op_Implicit(eggMorpher), user);
		}
		else
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnInteractUsing(Entity<EggMorpherComponent> eggMorpher, ref InteractUsingEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		EntityUid user = args.User;
		EntityUid used = args.Used;
		if (!((EntitySystem)this).HasComp<XenoParasiteComponent>(used))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-egg-morpher-attempt-insert-non-parasite"), Entity<EggMorpherComponent>.op_Implicit(eggMorpher), user);
			return;
		}
		if (!((EntitySystem)this).HasComp<ParasiteAIComponent>(used))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-awake-child", (ValueTuple<string, object>)("parasite", used)), user, user, PopupType.SmallCaution);
			return;
		}
		if (!_mobState.IsAlive(used))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-dead-child"), Entity<EggMorpherComponent>.op_Implicit(eggMorpher), user);
			return;
		}
		if (eggMorpher.Comp.MaxParasites <= eggMorpher.Comp.CurParasites)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-egg-morpher-already-full"), Entity<EggMorpherComponent>.op_Implicit(eggMorpher), user);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		((EntitySystem)this).QueueDel((EntityUid?)used);
		eggMorpher.Comp.CurParasites++;
		_appearance.SetData(Entity<EggMorpherComponent>.op_Implicit(eggMorpher), (Enum)EggmorpherOverlayVisuals.Number, (object)eggMorpher.Comp.CurParasites, (AppearanceComponent)null);
	}

	private void OnChangeParasiteReserve(Entity<EggMorpherComponent> eggMorpher, ref XenoChangeParasiteReserveMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		eggMorpher.Comp.ReservedParasites = args.NewReserve;
	}

	private void OnGetVerbs(Entity<EggMorpherComponent> eggMorpher, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Entity<EggMorpherComponent> val = eggMorpher;
		EntityUid val2 = default(EntityUid);
		EggMorpherComponent eggMorpherComponent = default(EggMorpherComponent);
		val.Deconstruct(ref val2, ref eggMorpherComponent);
		EntityUid ent = val2;
		EggMorpherComponent comp = eggMorpherComponent;
		EntityUid user = args.User;
		if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(user), Entity<HiveMemberComponent>.op_Implicit(ent)))
		{
			ActivationVerb changeReserveVerb = new ActivationVerb
			{
				Text = base.Loc.GetString("xeno-reserve-parasites-verb"),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent), (Enum)XenoReserveParasiteChangeUI.Key, (EntityUid?)user, false);
				}
			};
			args.Verbs.Add(changeReserveVerb);
		}
		if (((EntitySystem)this).HasComp<ActorComponent>(user) && ((EntitySystem)this).HasComp<GhostComponent>(user) && comp.CurParasites > comp.ReservedParasites && comp.CurParasites > 0)
		{
			ActivationVerb parasiteVerb = new ActivationVerb
			{
				Text = base.Loc.GetString("rmc-xeno-egg-ghost-verb"),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent), (Enum)XenoParasiteGhostUI.Key, user, false);
				},
				Impact = LogImpact.High
			};
			args.Verbs.Add(parasiteVerb);
		}
	}

	private void OnEggMorpherStepAttempt(Entity<EggMorpherComponent> eggMorpher, ref StepTriggerAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (CanTrigger(args.Tripper))
		{
			args.Continue = true;
		}
	}

	private void OnEggMorpherStepTriggered(Entity<EggMorpherComponent> eggMorpher, ref StepTriggeredOffEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		TryTrigger(eggMorpher, args.Tripper);
	}

	private bool CanTrigger(EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		InfectableComponent infected = default(InfectableComponent);
		if (((EntitySystem)this).TryComp<InfectableComponent>(user, ref infected) && !infected.BeingInfected && !_mobState.IsDead(user))
		{
			return !((EntitySystem)this).HasComp<VictimInfectedComponent>(user);
		}
		return false;
	}

	private bool TryTrigger(Entity<EggMorpherComponent> eggMorpher, EntityUid tripper)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!CanTrigger(tripper))
		{
			return false;
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(eggMorpher.Owner), Entity<TransformComponent>.op_Implicit(tripper)))
		{
			return false;
		}
		if (!TryCreateParasiteFromEggMorpher(eggMorpher, out var spawnedParasite))
		{
			return false;
		}
		if (spawnedParasite.HasValue)
		{
			XenoParasiteComponent parasiteComp = ((EntitySystem)this).EnsureComp<XenoParasiteComponent>(spawnedParasite.Value);
			_parasite.Infect(Entity<XenoParasiteComponent>.op_Implicit((spawnedParasite.Value, parasiteComp)), tripper, popup: true, force: true);
		}
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		((EntitySystem)this).Update(frameTime);
		TimeSpan curTime = _time.CurTime;
		EntityQueryEnumerator<EggMorpherComponent> eggMorpherQuery = ((EntitySystem)this).EntityQueryEnumerator<EggMorpherComponent>();
		EntityUid eggMorpherEnt = default(EntityUid);
		EggMorpherComponent eggMorpherComp = default(EggMorpherComponent);
		while (eggMorpherQuery.MoveNext(ref eggMorpherEnt, ref eggMorpherComp))
		{
			if (eggMorpherComp.GrowMaxParasites <= eggMorpherComp.CurParasites)
			{
				continue;
			}
			TimeSpan newSpawnTime = GetParasiteSpawnCooldown(Entity<EggMorpherComponent>.op_Implicit((eggMorpherEnt, eggMorpherComp))) + curTime;
			if (eggMorpherComp.NextSpawnAt < curTime)
			{
				eggMorpherComp.CurParasites++;
				_appearance.SetData(eggMorpherEnt, (Enum)EggmorpherOverlayVisuals.Number, (object)eggMorpherComp.CurParasites, (AppearanceComponent)null);
				eggMorpherComp.NextSpawnAt = newSpawnTime;
				((EntitySystem)this).Dirty(eggMorpherEnt, (IComponent)(object)eggMorpherComp, (MetaDataComponent)null);
				continue;
			}
			TimeSpan value = newSpawnTime;
			TimeSpan? nextSpawnAt = eggMorpherComp.NextSpawnAt;
			if (!(value < nextSpawnAt))
			{
				nextSpawnAt = eggMorpherComp.NextSpawnAt;
				if (nextSpawnAt.HasValue)
				{
					continue;
				}
			}
			eggMorpherComp.NextSpawnAt = newSpawnTime;
		}
	}

	private TimeSpan GetParasiteSpawnCooldown(Entity<EggMorpherComponent> eggMorpher)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(eggMorpher.Owner));
		if (!hive.HasValue)
		{
			return eggMorpher.Comp.StandardSpawnCooldown;
		}
		EntityUid? currentQueen = hive.GetValueOrDefault().Comp.CurrentQueen;
		if (currentQueen.HasValue)
		{
			EntityUid curQueen = currentQueen.GetValueOrDefault();
			if (((EntitySystem)this).HasComp<XenoAttachedOvipositorComponent>(curQueen))
			{
				return eggMorpher.Comp.OviSpawnCooldown;
			}
		}
		return eggMorpher.Comp.StandardSpawnCooldown;
	}

	public bool TryCreateParasiteFromEggMorpher(Entity<EggMorpherComponent> eggMorpher, out EntityUid? parasite)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		parasite = null;
		Entity<EggMorpherComponent> val = eggMorpher;
		EntityUid val2 = default(EntityUid);
		EggMorpherComponent eggMorpherComponent = default(EggMorpherComponent);
		val.Deconstruct(ref val2, ref eggMorpherComponent);
		EntityUid ent = val2;
		EggMorpherComponent comp = eggMorpherComponent;
		if (comp.CurParasites <= 0)
		{
			return false;
		}
		comp.CurParasites--;
		_appearance.SetData(Entity<EggMorpherComponent>.op_Implicit(eggMorpher), (Enum)EggmorpherOverlayVisuals.Number, (object)eggMorpher.Comp.CurParasites, (AppearanceComponent)null);
		((EntitySystem)this).Dirty<EggMorpherComponent>(eggMorpher, (MetaDataComponent)null);
		if (_net.IsClient)
		{
			parasite = null;
			return true;
		}
		parasite = ((EntitySystem)this).SpawnAtPosition("CMXenoParasite", ent.ToCoordinates(), (ComponentRegistry)null);
		_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(eggMorpher.Owner), Entity<HiveMemberComponent>.op_Implicit(parasite.Value));
		return true;
	}
}
