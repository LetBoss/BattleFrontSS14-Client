// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tabletop.SharedTabletopSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared.Tabletop;

public abstract class SharedTabletopSystem : EntitySystem
{
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
    this.SubscribeAllEvent<TabletopDraggingPlayerChangedEvent>(new EntitySessionEventHandler<TabletopDraggingPlayerChangedEvent>(this.OnDraggingPlayerChanged));
    this.SubscribeAllEvent<TabletopMoveEvent>(new EntitySessionEventHandler<TabletopMoveEvent>(this.OnTabletopMove));
  }

  protected virtual void OnTabletopMove(TabletopMoveEvent msg, EntitySessionEventArgs args)
  {
    ICommonSession senderSession = args.SenderSession;
    if (senderSession == null)
      return;
    EntityUid? attachedEntity = senderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid entity1 = this.GetEntity(msg.TableUid);
    EntityUid entity2 = this.GetEntity(msg.MovedEntityUid);
    if (!this.CanSeeTable(valueOrDefault, new EntityUid?(entity1)) || !this.CanDrag(valueOrDefault, entity2, out TabletopDraggableComponent _))
      return;
    TransformComponent xform = this.Comp<TransformComponent>(entity2);
    this.Transforms.SetParent(entity2, xform, this._mapSystem.GetMapOrInvalid(new MapId?(xform.MapID)));
    this.Transforms.SetLocalPositionNoLerp(entity2, msg.Coordinates.Position, xform);
  }

  private void OnDraggingPlayerChanged(
    TabletopDraggingPlayerChangedEvent msg,
    EntitySessionEventArgs args)
  {
    EntityUid entity = this.GetEntity(msg.DraggedEntityUid);
    TabletopDraggableComponent comp1;
    if (!this.TryComp<TabletopDraggableComponent>(entity, out comp1))
      return;
    comp1.DraggingPlayer = msg.IsDragging ? new NetUserId?(args.SenderSession.UserId) : new NetUserId?();
    this.Dirty(entity, (IComponent) comp1);
    AppearanceComponent comp2;
    if (!this.TryComp<AppearanceComponent>(entity, out comp2))
      return;
    if (comp1.DraggingPlayer.HasValue)
    {
      this._appearance.SetData(entity, (Enum) TabletopItemVisuals.Scale, (object) new Vector2(1.25f, 1.25f), comp2);
      this._appearance.SetData(entity, (Enum) TabletopItemVisuals.DrawDepth, (object) 5, comp2);
    }
    else
    {
      this._appearance.SetData(entity, (Enum) TabletopItemVisuals.Scale, (object) Vector2.One, comp2);
      this._appearance.SetData(entity, (Enum) TabletopItemVisuals.DrawDepth, (object) 4, comp2);
    }
  }

  protected bool CanSeeTable(EntityUid playerEntity, EntityUid? table)
  {
    MetaDataComponent comp;
    return this.TryComp(table, out comp) && comp.EntityLifeStage < EntityLifeStage.Terminating && (comp.Flags & MetaDataFlags.InContainer) != MetaDataFlags.InContainer && this._interactionSystem.InRangeUnobstructed((Entity<TransformComponent>) playerEntity, (Entity<TransformComponent>) table.Value) && this.ActionBlockerSystem.CanInteract(playerEntity, table);
  }

  protected bool CanDrag(
    EntityUid playerEntity,
    EntityUid target,
    [NotNullWhen(true)] out TabletopDraggableComponent? draggable)
  {
    HandsComponent comp;
    return this.TryComp<TabletopDraggableComponent>(target, out draggable) && this.TryComp<HandsComponent>(playerEntity, out comp) && comp.Hands.Count > 0;
  }

  [NetSerializable]
  [Serializable]
  public sealed class TabletopDraggableComponentState : ComponentState
  {
    public NetUserId? DraggingPlayer;

    public TabletopDraggableComponentState(NetUserId? draggingPlayer)
    {
      this.DraggingPlayer = draggingPlayer;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class TabletopRequestTakeOut : EntityEventArgs
  {
    public NetEntity Entity;
    public NetEntity TableUid;
  }
}
