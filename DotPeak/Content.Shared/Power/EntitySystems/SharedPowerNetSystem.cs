// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.EntitySystems.SharedPowerNetSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Power.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Power.EntitySystems;

public abstract class SharedPowerNetSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public abstract bool IsPoweredCalculate(SharedApcPowerReceiverComponent comp);

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AppearanceComponent, PowerChangedEvent>(new EntityEventRefHandler<AppearanceComponent, PowerChangedEvent>(this.OnPowerAppearance));
  }

  private void OnPowerAppearance(Entity<AppearanceComponent> ent, ref PowerChangedEvent args)
  {
    this._appearance.SetData((EntityUid) ent, (Enum) PowerDeviceVisuals.Powered, (object) args.Powered, ent.Comp);
  }
}
