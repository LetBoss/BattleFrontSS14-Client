// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.FireProtectionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Armor;
using Content.Shared.Atmos;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class FireProtectionSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FireProtectionComponent, InventoryRelayedEvent<GetFireProtectionEvent>>(new EntityEventRefHandler<FireProtectionComponent, InventoryRelayedEvent<GetFireProtectionEvent>>((object) this, __methodptr(OnGetProtection)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FireProtectionComponent, ArmorExamineEvent>(new EntityEventRefHandler<FireProtectionComponent, ArmorExamineEvent>((object) this, __methodptr(OnArmorExamine)), (Type[]) null, (Type[]) null);
  }

  private void OnGetProtection(
    Entity<FireProtectionComponent> ent,
    ref InventoryRelayedEvent<GetFireProtectionEvent> args)
  {
    args.Args.Reduce(ent.Comp.Reduction);
  }

  private void OnArmorExamine(Entity<FireProtectionComponent> ent, ref ArmorExamineEvent args)
  {
    float num = MathF.Round(ent.Comp.Reduction * 100f, 1);
    if ((double) num == 0.0)
      return;
    args.Msg.PushNewline();
    args.Msg.AddMarkupOrThrow(this.Loc.GetString(LocId.op_Implicit(ent.Comp.ExamineMessage), ("value", (object) num)));
  }
}
