// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.EntitySystems.ItemSlotRequiresPowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Power.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Power.EntitySystems;

public sealed class ItemSlotRequiresPowerSystem : EntitySystem
{
  [Dependency]
  private SharedPowerReceiverSystem _receiver;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ItemSlotRequiresPowerComponent, ItemSlotInsertAttemptEvent>(new EntityEventRefHandler<ItemSlotRequiresPowerComponent, ItemSlotInsertAttemptEvent>(this.OnInsertAttempt));
  }

  private void OnInsertAttempt(
    Entity<ItemSlotRequiresPowerComponent> ent,
    ref ItemSlotInsertAttemptEvent args)
  {
    if (this._receiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) ent.Owner))
      return;
    args.Cancelled = true;
  }
}
