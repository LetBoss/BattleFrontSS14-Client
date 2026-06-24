using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Tabletop.Components;
using Content.Shared.Tabletop.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop;

public abstract class SharedTabletopSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class TabletopDraggableComponentState : ComponentState
	{
		public NetUserId? DraggingPlayer;

		public TabletopDraggableComponentState(NetUserId? draggingPlayer)
		{
			DraggingPlayer = draggingPlayer;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class TabletopRequestTakeOut : EntityEventArgs
	{
		public NetEntity Entity;

		public NetEntity TableUid;
	}

	[Dependency]
	protected ActionBlockerSystem ActionBlockerSystem;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	protected SharedTransformSystem Transforms;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeAllEvent<TabletopDraggingPlayerChangedEvent>((EntitySessionEventHandler<TabletopDraggingPlayerChangedEvent>)OnDraggingPlayerChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<TabletopMoveEvent>((EntitySessionEventHandler<TabletopMoveEvent>)OnTabletopMove, (Type[])null, (Type[])null);
	}

	protected virtual void OnTabletopMove(TabletopMoveEvent msg, EntitySessionEventArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession playerSession = ((EntitySessionEventArgs)(ref args)).SenderSession;
		if (playerSession == null)
		{
			return;
		}
		EntityUid? attachedEntity = playerSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid playerEntity = attachedEntity.GetValueOrDefault();
			EntityUid table = ((EntitySystem)this).GetEntity(msg.TableUid);
			EntityUid moved = ((EntitySystem)this).GetEntity(msg.MovedEntityUid);
			if (CanSeeTable(playerEntity, table) && CanDrag(playerEntity, moved, out TabletopDraggableComponent _))
			{
				TransformComponent transform = ((EntitySystem)this).Comp<TransformComponent>(moved);
				Transforms.SetParent(moved, transform, _mapSystem.GetMapOrInvalid((MapId?)transform.MapID), (TransformComponent)null);
				Transforms.SetLocalPositionNoLerp(moved, msg.Coordinates.Position, transform);
			}
		}
	}

	private void OnDraggingPlayerChanged(TabletopDraggingPlayerChangedEvent msg, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		EntityUid dragged = ((EntitySystem)this).GetEntity(msg.DraggedEntityUid);
		TabletopDraggableComponent draggableComponent = default(TabletopDraggableComponent);
		if (!((EntitySystem)this).TryComp<TabletopDraggableComponent>(dragged, ref draggableComponent))
		{
			return;
		}
		draggableComponent.DraggingPlayer = (msg.IsDragging ? new NetUserId?(((EntitySessionEventArgs)(ref args)).SenderSession.UserId) : ((NetUserId?)null));
		((EntitySystem)this).Dirty(dragged, (IComponent)(object)draggableComponent, (MetaDataComponent)null);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(dragged, ref appearance))
		{
			if (draggableComponent.DraggingPlayer.HasValue)
			{
				_appearance.SetData(dragged, (Enum)TabletopItemVisuals.Scale, (object)new Vector2(1.25f, 1.25f), appearance);
				_appearance.SetData(dragged, (Enum)TabletopItemVisuals.DrawDepth, (object)5, appearance);
			}
			else
			{
				_appearance.SetData(dragged, (Enum)TabletopItemVisuals.Scale, (object)Vector2.One, appearance);
				_appearance.SetData(dragged, (Enum)TabletopItemVisuals.DrawDepth, (object)4, appearance);
			}
		}
	}

	protected bool CanSeeTable(EntityUid playerEntity, EntityUid? table)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent meta = default(MetaDataComponent);
		if (!((EntitySystem)this).TryComp(table, ref meta) || (int)meta.EntityLifeStage >= 4 || (meta.Flags & 2) == 2)
		{
			return false;
		}
		if (_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(playerEntity), Entity<TransformComponent>.op_Implicit(table.Value)))
		{
			return ActionBlockerSystem.CanInteract(playerEntity, table);
		}
		return false;
	}

	protected bool CanDrag(EntityUid playerEntity, EntityUid target, [NotNullWhen(true)] out TabletopDraggableComponent? draggable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TryComp<TabletopDraggableComponent>(target, ref draggable))
		{
			return false;
		}
		HandsComponent hands = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>(playerEntity, ref hands))
		{
			return hands.Hands.Count > 0;
		}
		return false;
	}
}
