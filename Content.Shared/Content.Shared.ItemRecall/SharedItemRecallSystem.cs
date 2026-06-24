using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Shared.ItemRecall;

public abstract class SharedItemRecallSystem : EntitySystem
{
	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedPvsOverrideSystem _pvs;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private SharedPopupSystem _popups;

	[Dependency]
	private SharedProjectileSystem _proj;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemRecallComponent, MapInitEvent>((EntityEventRefHandler<ItemRecallComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemRecallComponent, OnItemRecallActionEvent>((EntityEventRefHandler<ItemRecallComponent, OnItemRecallActionEvent>)OnItemRecallActionUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RecallMarkerComponent, ComponentShutdown>((EntityEventRefHandler<RecallMarkerComponent, ComponentShutdown>)OnRecallMarkerShutdown, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<ItemRecallComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.InitialName = ((EntitySystem)this).Name(Entity<ItemRecallComponent>.op_Implicit(ent), (MetaDataComponent)null);
		ent.Comp.InitialDescription = ((EntitySystem)this).Description(Entity<ItemRecallComponent>.op_Implicit(ent), (MetaDataComponent)null);
	}

	private void OnItemRecallActionUse(Entity<ItemRecallComponent> ent, ref OnItemRecallActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.MarkedEntity.HasValue)
		{
			HandsComponent hands = default(HandsComponent);
			if (((EntitySystem)this).TryComp<HandsComponent>(args.Performer, ref hands))
			{
				EntityUid? markItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit((args.Performer, hands)));
				if (!markItem.HasValue)
				{
					_popups.PopupClient(base.Loc.GetString("item-recall-item-mark-empty"), args.Performer, args.Performer);
					return;
				}
				if (((EntitySystem)this).HasComp<RecallMarkerComponent>(markItem))
				{
					_popups.PopupClient(base.Loc.GetString("item-recall-item-already-marked", (ValueTuple<string, object>)("item", markItem)), args.Performer, args.Performer);
					return;
				}
				_popups.PopupClient(base.Loc.GetString("item-recall-item-marked", (ValueTuple<string, object>)("item", markItem.Value)), args.Performer, args.Performer);
				TryMarkItem(ent, markItem.Value);
			}
		}
		else
		{
			RecallItem(Entity<RecallMarkerComponent>.op_Implicit(ent.Comp.MarkedEntity.Value));
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void RecallItem(Entity<RecallMarkerComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RecallMarkerComponent>(ent.Owner, ref ent.Comp, false))
		{
			return;
		}
		SharedActionsSystem actions = _actions;
		EntityUid? markedByAction = ent.Comp.MarkedByAction;
		Entity<ActionComponent>? action = actions.GetAction(markedByAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(markedByAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		if (!action.HasValue)
		{
			return;
		}
		markedByAction = action.GetValueOrDefault().Comp.AttachedEntity;
		if (markedByAction.HasValue)
		{
			EntityUid user = markedByAction.GetValueOrDefault();
			EmbeddableProjectileComponent projectile = default(EmbeddableProjectileComponent);
			if (((EntitySystem)this).TryComp<EmbeddableProjectileComponent>(Entity<RecallMarkerComponent>.op_Implicit(ent), ref projectile))
			{
				_proj.EmbedDetach(Entity<RecallMarkerComponent>.op_Implicit(ent), projectile, user);
			}
			_popups.PopupPredicted(base.Loc.GetString("item-recall-item-summon-self", (ValueTuple<string, object>)("item", ent)), base.Loc.GetString("item-recall-item-summon-others", (ValueTuple<string, object>)("item", ent), (ValueTuple<string, object>)("name", Identity.Entity(user, (IEntityManager)(object)base.EntityManager))), user, user);
			_popups.PopupPredictedCoordinates(base.Loc.GetString("item-recall-item-disappear", (ValueTuple<string, object>)("item", ent)), ((EntitySystem)this).Transform(Entity<RecallMarkerComponent>.op_Implicit(ent)).Coordinates, user);
			_hands.TryForcePickupAnyHand(user, Entity<RecallMarkerComponent>.op_Implicit(ent));
		}
	}

	private void OnRecallMarkerShutdown(Entity<RecallMarkerComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TryUnmarkItem(Entity<RecallMarkerComponent>.op_Implicit(ent));
	}

	private void TryMarkItem(Entity<ItemRecallComponent> ent, EntityUid item)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action = _actions.GetAction(Entity<ActionComponent>.op_Implicit(ent.Owner));
		if (action.HasValue)
		{
			Entity<ActionComponent> action2 = action.GetValueOrDefault();
			EntityUid? attachedEntity = action2.Comp.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid user = attachedEntity.GetValueOrDefault();
				AddToPvsOverride(item, user);
				ent.Comp.MarkedEntity = item;
				((EntitySystem)this).Dirty<ItemRecallComponent>(ent, (MetaDataComponent)null);
				RecallMarkerComponent marker = ((EntitySystem)this).AddComp<RecallMarkerComponent>(item);
				marker.MarkedByAction = Entity<ItemRecallComponent>.op_Implicit(ent);
				((EntitySystem)this).Dirty(item, (IComponent)(object)marker, (MetaDataComponent)null);
				UpdateActionAppearance(Entity<ActionComponent, ItemRecallComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action2), Entity<ActionComponent>.op_Implicit(action2), Entity<ItemRecallComponent>.op_Implicit(ent))));
			}
		}
	}

	private void TryUnmarkItem(EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		RecallMarkerComponent marker = default(RecallMarkerComponent);
		if (!((EntitySystem)this).TryComp<RecallMarkerComponent>(item, ref marker))
		{
			return;
		}
		SharedActionsSystem actions = _actions;
		EntityUid? markedByAction = marker.MarkedByAction;
		Entity<ActionComponent>? action = actions.GetAction(markedByAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(markedByAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		if (!action.HasValue)
		{
			return;
		}
		Entity<ActionComponent> action2 = action.GetValueOrDefault();
		ItemRecallComponent itemRecall = default(ItemRecallComponent);
		if (((EntitySystem)this).TryComp<ItemRecallComponent>(Entity<ActionComponent>.op_Implicit(action2), ref itemRecall))
		{
			markedByAction = action2.Comp.AttachedEntity;
			if (markedByAction.HasValue)
			{
				EntityUid user = markedByAction.GetValueOrDefault();
				_popups.PopupClient(base.Loc.GetString("item-recall-item-unmark", (ValueTuple<string, object>)("item", item)), user, user, PopupType.MediumCaution);
				RemoveFromPvsOverride(item, user);
			}
			itemRecall.MarkedEntity = null;
			UpdateActionAppearance(Entity<ActionComponent, ItemRecallComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action2), Entity<ActionComponent>.op_Implicit(action2), itemRecall)));
			((EntitySystem)this).Dirty(Entity<ActionComponent>.op_Implicit(action2), (IComponent)(object)itemRecall, (MetaDataComponent)null);
		}
		((EntitySystem)this).RemCompDeferred<RecallMarkerComponent>(item);
	}

	private void UpdateActionAppearance(Entity<ActionComponent, ItemRecallComponent> action)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? markedEntity = action.Comp2.MarkedEntity;
		if (markedEntity.HasValue)
		{
			EntityUid marked = markedEntity.GetValueOrDefault();
			LocId? whileMarkedName = action.Comp2.WhileMarkedName;
			if (whileMarkedName.HasValue)
			{
				LocId name = whileMarkedName.GetValueOrDefault();
				_metaData.SetEntityName(Entity<ActionComponent, ItemRecallComponent>.op_Implicit(action), base.Loc.GetString(LocId.op_Implicit(name), (ValueTuple<string, object>)("item", marked)), (MetaDataComponent)null, true);
			}
			whileMarkedName = action.Comp2.WhileMarkedDescription;
			if (whileMarkedName.HasValue)
			{
				LocId desc = whileMarkedName.GetValueOrDefault();
				_metaData.SetEntityDescription(Entity<ActionComponent, ItemRecallComponent>.op_Implicit(action), base.Loc.GetString(LocId.op_Implicit(desc), (ValueTuple<string, object>)("item", marked)), (MetaDataComponent)null);
			}
			_actions.SetEntityIcon(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent, ItemRecallComponent>.op_Implicit(action), Entity<ActionComponent, ItemRecallComponent>.op_Implicit(action))), marked);
		}
		else
		{
			string name2 = action.Comp2.InitialName;
			if (name2 != null)
			{
				_metaData.SetEntityName(Entity<ActionComponent, ItemRecallComponent>.op_Implicit(action), name2, (MetaDataComponent)null, true);
			}
			string desc2 = action.Comp2.InitialDescription;
			if (desc2 != null)
			{
				_metaData.SetEntityDescription(Entity<ActionComponent, ItemRecallComponent>.op_Implicit(action), desc2, (MetaDataComponent)null);
			}
			_actions.SetEntityIcon(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent, ItemRecallComponent>.op_Implicit(action), Entity<ActionComponent, ItemRecallComponent>.op_Implicit(action))), null);
		}
	}

	private void AddToPvsOverride(EntityUid uid, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession mindSession = default(ICommonSession);
		if (_player.TryGetSessionByEntity(user, ref mindSession))
		{
			_pvs.AddSessionOverride(uid, mindSession);
		}
	}

	private void RemoveFromPvsOverride(EntityUid uid, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession mindSession = default(ICommonSession);
		if (_player.TryGetSessionByEntity(user, ref mindSession))
		{
			_pvs.RemoveSessionOverride(uid, mindSession);
		}
	}
}
