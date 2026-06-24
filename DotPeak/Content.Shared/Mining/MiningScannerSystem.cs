// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mining.MiningScannerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Mining.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Mining;

public sealed class MiningScannerSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private InventorySystem _inventory;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MiningScannerComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<MiningScannerComponent, EntGotInsertedIntoContainerMessage>(this.OnInserted));
    this.SubscribeLocalEvent<MiningScannerComponent, EntGotRemovedFromContainerMessage>(new EntityEventRefHandler<MiningScannerComponent, EntGotRemovedFromContainerMessage>(this.OnRemoved));
    this.SubscribeLocalEvent<MiningScannerComponent, ItemToggledEvent>(new EntityEventRefHandler<MiningScannerComponent, ItemToggledEvent>(this.OnToggled));
  }

  private void OnInserted(
    Entity<MiningScannerComponent> ent,
    ref EntGotInsertedIntoContainerMessage args)
  {
    this.UpdateViewerComponent(args.Container.Owner);
  }

  private void OnRemoved(
    Entity<MiningScannerComponent> ent,
    ref EntGotRemovedFromContainerMessage args)
  {
    this.UpdateViewerComponent(args.Container.Owner);
  }

  private void OnToggled(Entity<MiningScannerComponent> ent, ref ItemToggledEvent args)
  {
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (ent.Owner, (TransformComponent) null, (MetaDataComponent) null), out container))
      return;
    this.UpdateViewerComponent(container.Owner);
  }

  public void UpdateViewerComponent(EntityUid uid)
  {
    Entity<MiningScannerComponent>? nullable = new Entity<MiningScannerComponent>?();
    foreach (EntityUid orInventoryEntity in this._inventory.GetHandOrInventoryEntities((Entity<HandsComponent, InventoryComponent>) uid))
    {
      MiningScannerComponent comp1;
      ItemToggleComponent comp2;
      if (this.TryComp<MiningScannerComponent>(orInventoryEntity, out comp1) && this.TryComp<ItemToggleComponent>(orInventoryEntity, out comp2) && comp2.Activated && (!nullable.HasValue || (double) comp1.Range > (double) nullable.Value.Comp.Range))
        nullable = new Entity<MiningScannerComponent>?((Entity<MiningScannerComponent>) (orInventoryEntity, comp1));
    }
    if (!this._net.IsServer)
      return;
    if (!nullable.HasValue)
    {
      MiningScannerViewerComponent comp;
      if (!this.TryComp<MiningScannerViewerComponent>(uid, out comp))
        return;
      comp.QueueRemoval = true;
    }
    else
    {
      MiningScannerViewerComponent scannerViewerComponent = this.EnsureComp<MiningScannerViewerComponent>(uid);
      scannerViewerComponent.ViewRange = nullable.Value.Comp.Range;
      scannerViewerComponent.QueueRemoval = false;
      scannerViewerComponent.NextPingTime = this._timing.CurTime + scannerViewerComponent.PingDelay;
      this.Dirty(uid, (IComponent) scannerViewerComponent);
    }
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<MiningScannerViewerComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MiningScannerViewerComponent, TransformComponent>();
    EntityUid uid;
    MiningScannerViewerComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.QueueRemoval)
        this.RemCompDeferred(uid, (IComponent) comp1);
      else if (!(this._timing.CurTime < comp1.NextPingTime))
      {
        comp1.NextPingTime = this._timing.CurTime + comp1.PingDelay;
        comp1.LastPingLocation = new EntityCoordinates?(comp2.Coordinates);
        if (this._net.IsClient && this._timing.IsFirstTimePredicted)
          this._audio.PlayEntity(comp1.PingSound, uid, uid);
      }
    }
  }
}
