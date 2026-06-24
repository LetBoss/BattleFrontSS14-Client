// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ame.EntitySystems.AmeFuelContainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ame.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Ame.EntitySystems;

public sealed class AmeFuelContainerSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AmeFuelContainerComponent, ExaminedEvent>(new ComponentEventHandler<AmeFuelContainerComponent, ExaminedEvent>((object) this, __methodptr(OnFuelExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnFuelExamined(EntityUid uid, AmeFuelContainerComponent comp, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    bool flag = comp.FuelAmount * 4 < comp.FuelCapacity;
    args.PushMarkup(this.Loc.GetString("ame-fuel-container-component-on-examine-detailed-message", new (string, object)[3]
    {
      ("colorName", flag ? (object) "darkorange" : (object) "orange"),
      ("amount", (object) comp.FuelAmount),
      ("capacity", (object) comp.FuelCapacity)
    }));
  }
}
