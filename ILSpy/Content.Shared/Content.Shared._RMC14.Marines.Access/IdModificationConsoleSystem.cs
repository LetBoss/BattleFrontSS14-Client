using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Marines.Access;

public sealed class IdModificationConsoleSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private GunIFFSystem _iff;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SharedRankSystem _rank;

	[Dependency]
	private ISerializationManager _serialization;

	[Dependency]
	private SquadSystem _squad;

	private FrozenDictionary<string, AccessGroupPrototype> _accessGroup = FrozenDictionary<string, AccessGroupPrototype>.Empty;

	private FrozenDictionary<string, AccessLevelPrototype> _accessLevel = FrozenDictionary<string, AccessLevelPrototype>.Empty;

	public override void Initialize()
	{
		BoundUserInterfaceRegisterExt.BuiEvents<IdModificationConsoleComponent>(((EntitySystem)this).Subs, (object)IdModificationConsoleUIKey.Key, (BuiEventSubscriber<IdModificationConsoleComponent>)delegate(Subscriber<IdModificationConsoleComponent> subs)
		{
			subs.Event<IdModificationConsoleAccessChangeBuiMsg>((EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleAccessChangeBuiMsg>)OnAccessChangeMsg);
			subs.Event<IdModificationConsoleMultipleAccessChangeBuiMsg>((EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleMultipleAccessChangeBuiMsg>)OnMultipleAccessChangeMsg);
			subs.Event<IdModificationConsoleSignInBuiMsg>((EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleSignInBuiMsg>)OnSignInMsg);
			subs.Event<IdModificationConsoleSignInTargetBuiMsg>((EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleSignInTargetBuiMsg>)OnSignInTargetMsg);
			subs.Event<IdModificationConsoleIFFChangeBuiMsg>((EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleIFFChangeBuiMsg>)OnIFFChangeMsg);
			subs.Event<IdModificationConsoleJobChangeBuiMsg>((EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleJobChangeBuiMsg>)OnJobChangeMsg);
			subs.Event<IdModificationConsoleTerminateConfirmBuiMsg>((EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleTerminateConfirmBuiMsg>)OnTerminateConfirmMsg);
		});
		((EntitySystem)this).SubscribeLocalEvent<IdModificationConsoleComponent, MapInitEvent>((EntityEventRefHandler<IdModificationConsoleComponent, MapInitEvent>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IdModificationConsoleComponent, InteractUsingEvent>((EntityEventRefHandler<IdModificationConsoleComponent, InteractUsingEvent>)OnInteractHand, (Type[])null, (Type[])null);
		ReloadJobPrototypes();
		ReloadAccessPrototypes();
	}

	private void OnInteractHand(Entity<IdModificationConsoleComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = ContainerInHandler(ent, args.User);
	}

	private void OnJobChangeMsg(Entity<IdModificationConsoleComponent> ent, ref IdModificationConsoleJobChangeBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		AccessComponent access = default(AccessComponent);
		if (!ent.Comp.Authenticated || !TryContainerEntity(ent, ent.Comp.TargetIdSlot, out var uid) || !((EntitySystem)this).TryComp<AccessComponent>(uid, ref access))
		{
			return;
		}
		access.Tags.Clear();
		AccessGroupPrototype accessGroupPrototype = default(AccessGroupPrototype);
		if (!_prototype.TryIndex<AccessGroupPrototype>(args.AccessGroup, ref accessGroupPrototype))
		{
			return;
		}
		foreach (ProtoId<AccessLevelPrototype> tag in accessGroupPrototype.Tags)
		{
			access.Tags.Add(tag);
		}
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(33, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" has changed the accesses of ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(accessGroupPrototype.Name);
		adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref handler);
	}

	private void OnTerminateConfirmMsg(Entity<IdModificationConsoleComponent> ent, ref IdModificationConsoleTerminateConfirmBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		ItemIFFComponent iff = default(ItemIFFComponent);
		IdCardComponent idCard = default(IdCardComponent);
		AccessComponent access = default(AccessComponent);
		if (!ent.Comp.Authenticated || !TryContainerEntity(ent, ent.Comp.TargetIdSlot, out var uid) || !((EntitySystem)this).TryComp<ItemIFFComponent>(uid, ref iff) || !((EntitySystem)this).TryComp<IdCardComponent>(uid, ref idCard) || !((EntitySystem)this).TryComp<AccessComponent>(uid, ref access))
		{
			return;
		}
		_iff.SetIdFaction(Entity<ItemIFFComponent>.op_Implicit((uid.Value, iff)), EntProtoId<IFFFactionComponent>.op_Implicit("FactionSurvivor"));
		ent.Comp.HasIFF = false;
		foreach (ProtoId<AccessLevelPrototype> accessToRemove in ent.Comp.AccessList)
		{
			access.Tags.Remove(accessToRemove);
		}
		foreach (ProtoId<AccessLevelPrototype> accessToRemove2 in ent.Comp.HiddenAccessList)
		{
			access.Tags.Remove(accessToRemove2);
		}
		idCard._jobTitle = "Civilian";
		((EntitySystem)this).Dirty(uid.Value, (IComponent)(object)idCard, (MetaDataComponent)null);
		if (idCard.OriginalOwner.HasValue)
		{
			_rank.SetRank(idCard.OriginalOwner.Value, ProtoId<RankPrototype>.op_Implicit("RMCRankCivilian"));
			_squad.RemoveSquad(idCard.OriginalOwner.Value, null);
			_metaData.SetEntityName(uid.Value, ((EntitySystem)this).MetaData(idCard.OriginalOwner.Value).EntityName + " (" + idCard._jobTitle + ")", (MetaDataComponent)null, true);
		}
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(19, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" has terminated ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
		handler.AppendLiteral(" & ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString(idCard.OriginalOwner, (MetaDataComponent)null), "player", "ToPrettyString(idCard.OriginalOwner)");
		adminLogger.Add(LogType.RMCIdModify, LogImpact.High, ref handler);
	}

	private void OnIFFChangeMsg(Entity<IdModificationConsoleComponent> ent, ref IdModificationConsoleIFFChangeBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Authenticated && TryContainerEntity(ent, ent.Comp.TargetIdSlot, out var uid) && uid.HasValue)
		{
			ItemIFFComponent iff = default(ItemIFFComponent);
			((EntitySystem)this).EnsureComp<ItemIFFComponent>(uid.Value, ref iff);
			if (!iff.Factions.Contains(ent.Comp.Faction) && !args.Revoke)
			{
				_iff.SetIdFaction(Entity<ItemIFFComponent>.op_Implicit((uid.Value, iff)), ent.Comp.Faction);
				ent.Comp.HasIFF = true;
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(26, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
				handler.AppendLiteral(" has revoked the ");
				handler.AppendFormatted<EntProtoId<IFFFactionComponent>>(ent.Comp.Faction, "ent.Comp.Faction");
				handler.AppendLiteral(" IFF for ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
				adminLogger.Add(LogType.RMCIdModify, LogImpact.Medium, ref handler);
			}
			else if (args.Revoke)
			{
				_iff.SetIdFaction(Entity<ItemIFFComponent>.op_Implicit((uid.Value, iff)), EntProtoId<IFFFactionComponent>.op_Implicit("FactionSurvivor"));
				ent.Comp.HasIFF = false;
				ISharedAdminLogManager adminLogger2 = _adminLogger;
				LogStringHandler handler2 = new LogStringHandler(26, 3);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
				handler2.AppendLiteral(" has granted the ");
				handler2.AppendFormatted<EntProtoId<IFFFactionComponent>>(ent.Comp.Faction, "ent.Comp.Faction");
				handler2.AppendLiteral(" IFF for ");
				handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
				adminLogger2.Add(LogType.RMCIdModify, LogImpact.Low, ref handler2);
			}
			((EntitySystem)this).Dirty<IdModificationConsoleComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnSignInTargetMsg(Entity<IdModificationConsoleComponent> ent, ref IdModificationConsoleSignInTargetBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (TryContainerEntity(ent, ent.Comp.TargetIdSlot, out var _))
		{
			ContainerOutHandler(ent, ((BaseBoundUserInterfaceEvent)args).Actor, ent.Comp.TargetIdSlot);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(25, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
			handler.AppendLiteral(" has ejected from ");
			handler.AppendFormatted(ent.Comp.TargetIdSlot);
			handler.AppendLiteral(" from: ");
			handler.AppendFormatted<EntityUid>(ent.Owner, "entity", "ent.Owner");
			adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref handler);
		}
		else
		{
			ContainerInHandler(ent, ((BaseBoundUserInterfaceEvent)args).Actor, ent.Comp.TargetIdSlot);
		}
	}

	private void OnSignInMsg(Entity<IdModificationConsoleComponent> ent, ref IdModificationConsoleSignInBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if (TryContainerEntity(ent, ent.Comp.PrivilegedIdSlot, out var _))
		{
			ContainerOutHandler(ent, ((BaseBoundUserInterfaceEvent)args).Actor, ent.Comp.PrivilegedIdSlot);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(25, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
			handler.AppendLiteral(" has ejected from ");
			handler.AppendFormatted(ent.Comp.PrivilegedIdSlot);
			handler.AppendLiteral(" from: ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner)), "entity", "ToPrettyString(ent.Owner)");
			adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref handler);
		}
		else
		{
			ContainerInHandler(ent, ((BaseBoundUserInterfaceEvent)args).Actor, ent.Comp.PrivilegedIdSlot);
			AccessLevelPrototype accessPrototype = default(AccessLevelPrototype);
			if (!ent.Comp.Authenticated && _prototype.TryIndex<AccessLevelPrototype>(ent.Comp.Access, ref accessPrototype) && accessPrototype.Name != null)
			{
				_popup.PopupClient("This id is missing the " + base.Loc.GetString(accessPrototype.Name), ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.MediumCaution);
			}
		}
	}

	private void OnMultipleAccessChangeMsg(Entity<IdModificationConsoleComponent> ent, ref IdModificationConsoleMultipleAccessChangeBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		AccessComponent access = default(AccessComponent);
		if (!ent.Comp.Authenticated || !TryContainerEntity(ent, ent.Comp.TargetIdSlot, out var uid) || !((EntitySystem)this).TryComp<AccessComponent>(uid, ref access))
		{
			return;
		}
		switch (args.Type)
		{
		case "GrantAll":
		{
			AccessLevelPrototype accessPrototype2 = default(AccessLevelPrototype);
			foreach (ProtoId<AccessLevelPrototype> accessToAdd2 in ent.Comp.AccessList)
			{
				if (_prototype.TryIndex<AccessLevelPrototype>(accessToAdd2, ref accessPrototype2) && !(accessPrototype2.AccessGroup != args.AccessList))
				{
					access.Tags.Add(ProtoId<AccessLevelPrototype>.op_Implicit(accessPrototype2));
				}
			}
			ISharedAdminLogManager adminLogger4 = _adminLogger;
			LogStringHandler handler4 = new LogStringHandler(34, 3);
			handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
			handler4.AppendLiteral(" has granted all accesses for ");
			handler4.AppendFormatted(args.AccessList);
			handler4.AppendLiteral(" on ");
			handler4.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
			adminLogger4.Add(LogType.RMCIdModify, LogImpact.Medium, ref handler4);
			break;
		}
		case "RevokeAll":
		{
			AccessLevelPrototype accessPrototype = default(AccessLevelPrototype);
			foreach (ProtoId<AccessLevelPrototype> accessToRemove2 in ent.Comp.AccessList)
			{
				if (_prototype.TryIndex<AccessLevelPrototype>(accessToRemove2, ref accessPrototype) && !(accessPrototype.AccessGroup != args.AccessList))
				{
					access.Tags.Remove(accessToRemove2);
				}
			}
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(34, 3);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
			handler2.AppendLiteral(" has revoked all accesses for ");
			handler2.AppendFormatted(args.AccessList);
			handler2.AppendLiteral(" on ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
			adminLogger2.Add(LogType.RMCIdModify, LogImpact.Medium, ref handler2);
			break;
		}
		case "GrantAllGroup":
		{
			foreach (ProtoId<AccessLevelPrototype> accessToAdd in ent.Comp.AccessList)
			{
				access.Tags.Add(accessToAdd);
			}
			ISharedAdminLogManager adminLogger3 = _adminLogger;
			LogStringHandler handler3 = new LogStringHandler(29, 2);
			handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
			handler3.AppendLiteral(" has granted all accesses on ");
			handler3.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
			adminLogger3.Add(LogType.RMCIdModify, LogImpact.Medium, ref handler3);
			break;
		}
		case "RevokeAllGroup":
		{
			foreach (ProtoId<AccessLevelPrototype> accessToRemove in ent.Comp.AccessList)
			{
				access.Tags.Remove(accessToRemove);
			}
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(29, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
			handler.AppendLiteral(" has revoked all accesses on ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
			adminLogger.Add(LogType.RMCIdModify, LogImpact.Medium, ref handler);
			break;
		}
		}
		((EntitySystem)this).Dirty<IdModificationConsoleComponent>(ent, (MetaDataComponent)null);
	}

	private void OnAccessChangeMsg(Entity<IdModificationConsoleComponent> ent, ref IdModificationConsoleAccessChangeBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		AccessComponent access = default(AccessComponent);
		if (ent.Comp.Authenticated && TryContainerEntity(ent, ent.Comp.TargetIdSlot, out var uid) && ((EntitySystem)this).TryComp<AccessComponent>(uid, ref access))
		{
			if (args.Add)
			{
				access.Tags.Add(args.Access);
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(17, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
				handler.AppendLiteral(" has granted ");
				handler.AppendFormatted<ProtoId<AccessLevelPrototype>>(args.Access, "args.Access");
				handler.AppendLiteral(" to ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
				adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref handler);
			}
			else
			{
				access.Tags.Remove(args.Access);
				ISharedAdminLogManager adminLogger2 = _adminLogger;
				LogStringHandler handler2 = new LogStringHandler(17, 3);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
				handler2.AppendLiteral(" has revoked ");
				handler2.AppendFormatted<ProtoId<AccessLevelPrototype>>(args.Access, "args.Access");
				handler2.AppendLiteral(" to ");
				handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(uid, (MetaDataComponent)null), "entity", "ToPrettyString(uid)");
				adminLogger2.Add(LogType.RMCIdModify, LogImpact.Low, ref handler2);
			}
		}
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
	{
		if (ev.WasModified<AccessLevelPrototype>())
		{
			ReloadAccessPrototypes();
		}
		if (ev.WasModified<AccessGroupPrototype>())
		{
			ReloadJobPrototypes();
		}
	}

	private void ReloadAccessPrototypes()
	{
		Dictionary<string, AccessLevelPrototype> dict = new Dictionary<string, AccessLevelPrototype>();
		foreach (AccessLevelPrototype accessLevelProto in _prototype.EnumeratePrototypes<AccessLevelPrototype>())
		{
			object accessLevelObj = new AccessLevelPrototype();
			_serialization.CopyTo((object)accessLevelProto, ref accessLevelObj, (ISerializationContext)null, false, false);
			if (accessLevelObj is AccessLevelPrototype accessLevel)
			{
				dict[accessLevelProto.ID] = accessLevel;
			}
		}
		_accessLevel = dict.ToFrozenDictionary();
	}

	private void ReloadJobPrototypes()
	{
		Dictionary<string, AccessGroupPrototype> dict = new Dictionary<string, AccessGroupPrototype>();
		foreach (AccessGroupPrototype accessLevelProto in _prototype.EnumeratePrototypes<AccessGroupPrototype>())
		{
			object accessGroupObj = new AccessGroupPrototype();
			_serialization.CopyTo((object)accessLevelProto, ref accessGroupObj, (ISerializationContext)null, false, false);
			if (accessGroupObj is AccessGroupPrototype accessLevel)
			{
				dict[accessLevelProto.ID] = accessLevel;
			}
		}
		_accessGroup = dict.ToFrozenDictionary();
	}

	private bool ContainerInHandler(Entity<IdModificationConsoleComponent> ent, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		IdCardComponent idCardComponent = default(IdCardComponent);
		AccessComponent accessComponent = default(AccessComponent);
		if (!_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var handItem) || !((EntitySystem)this).TryComp<IdCardComponent>(handItem, ref idCardComponent) || !((EntitySystem)this).TryComp<AccessComponent>(handItem, ref accessComponent))
		{
			return false;
		}
		if (accessComponent.Tags.Contains(ent.Comp.Access))
		{
			return ContainerInHandler(ent, user, ent.Comp.PrivilegedIdSlot);
		}
		return ContainerInHandler(ent, user, ent.Comp.TargetIdSlot);
	}

	private bool ContainerInHandler(Entity<IdModificationConsoleComponent> ent, EntityUid user, string containerType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		IdCardComponent idCardComponent = default(IdCardComponent);
		AccessComponent accessComponent = default(AccessComponent);
		if (!_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var handItem) || !((EntitySystem)this).TryComp<IdCardComponent>(handItem, ref idCardComponent) || !((EntitySystem)this).TryComp<AccessComponent>(handItem, ref accessComponent))
		{
			return false;
		}
		if (accessComponent.Tags.Contains(ent.Comp.Access) && containerType == ent.Comp.PrivilegedIdSlot)
		{
			ent.Comp.Authenticated = true;
		}
		ItemIFFComponent iff = default(ItemIFFComponent);
		if (((EntitySystem)this).TryComp<ItemIFFComponent>(handItem, ref iff) && containerType == ent.Comp.TargetIdSlot)
		{
			ent.Comp.HasIFF = iff.Factions.Contains(ent.Comp.Faction);
		}
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<IdModificationConsoleComponent>.op_Implicit(ent), containerType, (ContainerManagerComponent)null);
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(handItem.Value), (BaseContainer)(object)container, (TransformComponent)null, false);
		((EntitySystem)this).Dirty<IdModificationConsoleComponent>(ent, (MetaDataComponent)null);
		return true;
	}

	private bool ContainerOutHandler(Entity<IdModificationConsoleComponent> ent, EntityUid user, string containerType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<IdModificationConsoleComponent>.op_Implicit(ent), containerType, (ContainerManagerComponent)null);
		EntityUid? contained = container.ContainedEntity;
		if (!contained.HasValue)
		{
			return false;
		}
		_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(contained.Value), (BaseContainer)(object)container, true, false, (EntityCoordinates?)null, (Angle?)null);
		if (containerType == ent.Comp.PrivilegedIdSlot)
		{
			ent.Comp.Authenticated = false;
		}
		if (containerType == ent.Comp.TargetIdSlot)
		{
			ent.Comp.HasIFF = false;
		}
		_hands.PickupOrDrop(user, contained.Value);
		((EntitySystem)this).Dirty<IdModificationConsoleComponent>(ent, (MetaDataComponent)null);
		return true;
	}

	private bool TryContainerEntity(Entity<IdModificationConsoleComponent> ent, string containerType, out EntityUid? contained)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<IdModificationConsoleComponent>.op_Implicit(ent), containerType, (ContainerManagerComponent)null);
		contained = container.ContainedEntity;
		((EntitySystem)this).Dirty<IdModificationConsoleComponent>(ent, (MetaDataComponent)null);
		return contained.HasValue;
	}

	private void OnComponentInit(Entity<IdModificationConsoleComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAccessList(ent);
	}

	private void UpdateAccessList(Entity<IdModificationConsoleComponent> ent)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ProtoId<AccessLevelPrototype>> accessList = new HashSet<ProtoId<AccessLevelPrototype>>();
		HashSet<ProtoId<AccessLevelPrototype>> accessListHidden = new HashSet<ProtoId<AccessLevelPrototype>>();
		HashSet<ProtoId<AccessLevelPrototype>> accessGroups = new HashSet<ProtoId<AccessLevelPrototype>>();
		ImmutableArray<AccessLevelPrototype>.Enumerator enumerator = _accessLevel.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			AccessLevelPrototype accessLevel = enumerator.Current;
			EntProtoId<IFFFactionComponent>? faction = accessLevel.Faction;
			EntProtoId<IFFFactionComponent> faction2 = ent.Comp.Faction;
			if (faction.HasValue && faction.GetValueOrDefault() == faction2 && !accessLevel.Hidden)
			{
				if (accessLevel.Name != null && accessLevel.Name.Contains("protobaseaccess"))
				{
					accessGroups.Add(ProtoId<AccessLevelPrototype>.op_Implicit(accessLevel));
				}
				else
				{
					accessList.Add(ProtoId<AccessLevelPrototype>.op_Implicit(accessLevel));
				}
				continue;
			}
			faction = accessLevel.Faction;
			faction2 = ent.Comp.Faction;
			if (faction.HasValue && faction.GetValueOrDefault() == faction2 && accessLevel.Hidden && accessLevel.Name != null && !accessLevel.Name.Contains("protobaseaccess"))
			{
				accessListHidden.Add(ProtoId<AccessLevelPrototype>.op_Implicit(accessLevel));
			}
		}
		ent.Comp.AccessGroups = accessGroups;
		ent.Comp.AccessList = accessList;
		ent.Comp.HiddenAccessList = accessListHidden;
		HashSet<ProtoId<AccessGroupPrototype>> groupList = new HashSet<ProtoId<AccessGroupPrototype>>();
		HashSet<ProtoId<AccessGroupPrototype>> groupGroups = new HashSet<ProtoId<AccessGroupPrototype>>();
		ImmutableArray<AccessGroupPrototype>.Enumerator enumerator2 = _accessGroup.Values.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			AccessGroupPrototype accessGroup = enumerator2.Current;
			EntProtoId<IFFFactionComponent>? faction = accessGroup.Faction;
			EntProtoId<IFFFactionComponent> faction2 = ent.Comp.Faction;
			if (faction.HasValue && faction.GetValueOrDefault() == faction2 && !accessGroup.Hidden)
			{
				if (accessGroup.Name != null && accessGroup.Name.Contains("protobaseaccess"))
				{
					groupGroups.Add(ProtoId<AccessGroupPrototype>.op_Implicit(accessGroup));
				}
				else
				{
					groupList.Add(ProtoId<AccessGroupPrototype>.op_Implicit(accessGroup));
				}
			}
		}
		ent.Comp.JobGroups = groupGroups;
		ent.Comp.JobList = groupList;
	}
}
