// Decompiled with JetBrains decompiler
// Type: Content.Shared.DragDrop.SharedDragDropSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.DragDrop;

public abstract class SharedDragDropSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlockerSystem;
  [Dependency]
  private SharedInteractionSystem _interaction;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<DragDropRequestEvent>(new EntitySessionEventHandler<DragDropRequestEvent>(this.OnDragDropRequestEvent));
  }

  private void OnDragDropRequestEvent(DragDropRequestEvent msg, EntitySessionEventArgs args)
  {
    EntityUid entity1 = this.GetEntity(msg.Dragged);
    EntityUid entity2 = this.GetEntity(msg.Target);
    if (this.Deleted(entity1) || this.Deleted(entity2))
      return;
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue || !this._actionBlockerSystem.CanInteract(attachedEntity.Value, new EntityUid?(entity2)) || !this._interaction.InRangeUnobstructed((Entity<TransformComponent>) attachedEntity.Value, (Entity<TransformComponent>) entity1, popup: true) || !this._interaction.InRangeUnobstructed((Entity<TransformComponent>) attachedEntity.Value, (Entity<TransformComponent>) entity2, popup: true))
      return;
    DragDropDraggedEvent args1 = new DragDropDraggedEvent(attachedEntity.Value, entity2);
    this.RaiseLocalEvent<DragDropDraggedEvent>(entity1, ref args1);
    if (args1.Handled)
      return;
    DragDropTargetEvent args2 = new DragDropTargetEvent(attachedEntity.Value, entity1);
    this.RaiseLocalEvent<DragDropTargetEvent>(this.GetEntity(msg.Target), ref args2);
  }
}
