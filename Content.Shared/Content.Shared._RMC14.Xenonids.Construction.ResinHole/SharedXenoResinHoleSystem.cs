using System;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Construction.ResinHole;

public abstract class SharedXenoResinHoleSystem : EntitySystem
{
	[Dependency]
	protected SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	protected MobStateSystem _mobState;

	[Dependency]
	protected RMCHandsSystem _rmcHands;

	[Dependency]
	protected SharedXenoHiveSystem _hive;

	[Dependency]
	protected INetManager _net;

	[Dependency]
	protected SharedPopupSystem _popup;

	[Dependency]
	protected SharedDoAfterSystem _doAfter;

	[Dependency]
	private AreaSystem _areas;

	[Dependency]
	private SharedXenoAnnounceSystem _announce;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoResinHoleComponent, InteractUsingEvent>((EntityEventRefHandler<XenoResinHoleComponent, InteractUsingEvent>)OnPlaceParasiteInXenoResinHole, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoResinHoleComponent, ActivateInWorldEvent>((EntityEventRefHandler<XenoResinHoleComponent, ActivateInWorldEvent>)OnActivateInWorldResinHole, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoResinHoleComponent, XenoResinHoleActivationEvent>((EntityEventRefHandler<XenoResinHoleComponent, XenoResinHoleActivationEvent>)OnResinHoleActivation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoResinHoleComponent, GettingAttackedAttemptEvent>((EntityEventRefHandler<XenoResinHoleComponent, GettingAttackedAttemptEvent>)OnXenoResinHoleAttacked, (Type[])null, (Type[])null);
	}

	private void OnXenoResinHoleAttacked(Entity<XenoResinHoleComponent> resinHole, ref GettingAttackedAttemptEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(args.Attacker), Entity<HiveMemberComponent>.op_Implicit(resinHole.Owner)) && resinHole.Comp.TrapPrototype.HasValue)
		{
			args.Cancelled = true;
		}
	}

	protected bool CanPlaceInHole(EntityUid uid, Entity<XenoResinHoleComponent> resinHole, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoParasiteComponent>(uid) || _mobState.IsDead(uid))
		{
			return false;
		}
		EntProtoId? trapPrototype = resinHole.Comp.TrapPrototype;
		if (trapPrototype.HasValue)
		{
			_popup.PopupEntity(base.Loc.GetString("cm-xeno-construction-resin-hole-full"), Entity<XenoResinHoleComponent>.op_Implicit(resinHole), user);
			return false;
		}
		if (!_rmcHands.IsPickupByAllowed(Entity<WhitelistPickupByComponent>.op_Implicit(uid), Entity<WhitelistPickupComponent>.op_Implicit(user)))
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<ParasiteAIComponent>(uid))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-awake-child", (ValueTuple<string, object>)("parasite", uid)), user, user, PopupType.SmallCaution);
			return false;
		}
		return true;
	}

	private void OnPlaceParasiteInXenoResinHole(Entity<XenoResinHoleComponent> resinHole, ref InteractUsingEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (!_net.IsClient && CanPlaceInHole(args.Used, resinHole, args.User))
			{
				XenoPlaceParasiteInHoleDoAfterEvent ev = new XenoPlaceParasiteInHoleDoAfterEvent();
				DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, resinHole.Comp.AddParasiteDelay, ev, Entity<XenoResinHoleComponent>.op_Implicit(resinHole), Entity<XenoResinHoleComponent>.op_Implicit(resinHole), args.Used)
				{
					BreakOnDropItem = true,
					BreakOnMove = true,
					BreakOnHandChange = true
				};
				_doAfter.TryStartDoAfter(doAfterArgs);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-resin-hole-filling-parasite"), Entity<XenoResinHoleComponent>.op_Implicit(resinHole), args.User);
			}
		}
	}

	private void OnActivateInWorldResinHole(Entity<XenoResinHoleComponent> resinHole, ref ActivateInWorldEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoParasiteComponent>(args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (!_mobState.IsDead(args.User) && !_net.IsClient && !resinHole.Comp.TrapPrototype.HasValue)
			{
				SetTrapType(resinHole, "CMXenoParasite");
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-resin-hole-enter-parasite", (ValueTuple<string, object>)("parasite", args.User)), Entity<XenoResinHoleComponent>.op_Implicit(resinHole));
				_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(args.User), Entity<HiveMemberComponent>.op_Implicit(resinHole.Owner));
				((EntitySystem)this).QueueDel((EntityUid?)args.User);
				_appearanceSystem.SetData(resinHole.Owner, (Enum)XenoResinHoleVisuals.Contained, (object)ContainedTrap.Parasite, (AppearanceComponent)null);
			}
		}
	}

	private void OnResinHoleActivation(Entity<XenoResinHoleComponent> ent, ref XenoResinHoleActivationEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner));
		if (hive.HasValue)
		{
			Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
			string locationName = "Unknown";
			if (_areas.TryGetArea(Entity<XenoResinHoleComponent>.op_Implicit(ent), out Entity<AreaComponent>? _, out EntityPrototype areaProto))
			{
				locationName = areaProto.Name;
			}
			string msg = base.Loc.GetString(LocId.op_Implicit(args.message), (ValueTuple<string, object>)("location", locationName), (ValueTuple<string, object>)("type", GetTrapTypeName(ent)));
			SharedXenoAnnounceSystem announce = _announce;
			EntityUid owner = ent.Owner;
			EntityUid hive3 = Entity<HiveComponent>.op_Implicit(hive2);
			Color? color = ent.Comp.MessageColor;
			announce.AnnounceToHive(owner, hive3, msg, null, null, color);
		}
	}

	protected void SetTrapType(Entity<XenoResinHoleComponent> resinHole, string? newTrapPrototype)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		resinHole.Comp.TrapPrototype = EntProtoId.op_Implicit(newTrapPrototype);
		XenoAnnounceStructureDestructionComponent structureDestructionComp = default(XenoAnnounceStructureDestructionComponent);
		if (((EntitySystem)this).TryComp<XenoAnnounceStructureDestructionComponent>(resinHole.Owner, ref structureDestructionComp))
		{
			structureDestructionComp.StructureName = GetTrapTypeName(resinHole);
		}
		((EntitySystem)this).Dirty<XenoResinHoleComponent>(resinHole, (MetaDataComponent)null);
	}

	public string GetTrapTypeName(Entity<XenoResinHoleComponent> resinHole)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId? trapPrototype = resinHole.Comp.TrapPrototype;
		switch (trapPrototype.HasValue ? EntProtoId.op_Implicit(trapPrototype.GetValueOrDefault()) : null)
		{
		case "CMXenoParasite":
			return base.Loc.GetString("rmc-xeno-construction-resin-hole-parasite-name");
		case "RMCSmokeAcid":
		case "RMCSmokeNeurotoxin":
			return base.Loc.GetString("rmc-xeno-construction-resin-hole-gas-name");
		case "XenoAcidSprayTrapWeak":
		case "XenoAcidSprayTrap":
		case "XenoAcidSprayTrapStrong":
			return base.Loc.GetString("rmc-xeno-construction-resin-hole-acid-name");
		default:
			return base.Loc.GetString("rmc-xeno-construction-resin-hole-empty-name");
		}
	}
}
