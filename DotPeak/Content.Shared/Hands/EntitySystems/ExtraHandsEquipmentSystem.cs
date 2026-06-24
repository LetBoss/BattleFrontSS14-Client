// Decompiled with JetBrains decompiler
// Type: Content.Shared.Hands.EntitySystems.ExtraHandsEquipmentSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Hands.EntitySystems;

public sealed class ExtraHandsEquipmentSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _hands;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ExtraHandsEquipmentComponent, GotEquippedEvent>(new EntityEventRefHandler<ExtraHandsEquipmentComponent, GotEquippedEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<ExtraHandsEquipmentComponent, GotUnequippedEvent>(new EntityEventRefHandler<ExtraHandsEquipmentComponent, GotUnequippedEvent>(this.OnUnequipped));
  }

  private void OnEquipped(Entity<ExtraHandsEquipmentComponent> ent, ref GotEquippedEvent args)
  {
    HandsComponent comp;
    if (!this.TryComp<HandsComponent>(args.Equipee, out comp))
      return;
    foreach ((string key, Hand hand) in ent.Comp.Hands)
    {
      string handName = $"{this.GetNetEntity(ent.Owner).Id}-{key}";
      this._hands.AddHand((Entity<HandsComponent>) (args.Equipee, comp), handName, hand.Location);
    }
  }

  private void OnUnequipped(Entity<ExtraHandsEquipmentComponent> ent, ref GotUnequippedEvent args)
  {
    HandsComponent comp;
    if (!this.TryComp<HandsComponent>(args.Equipee, out comp))
      return;
    foreach (string key in ent.Comp.Hands.Keys)
    {
      string handName = $"{this.GetNetEntity(ent.Owner).Id}-{key}";
      this._hands.RemoveHand((Entity<HandsComponent>) (args.Equipee, comp), handName);
    }
  }
}
