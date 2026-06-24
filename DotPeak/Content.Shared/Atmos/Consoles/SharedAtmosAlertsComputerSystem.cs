// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Consoles.SharedAtmosAlertsComputerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Atmos.Consoles;

public abstract class SharedAtmosAlertsComputerSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosAlertsComputerComponent, AtmosAlertsComputerDeviceSilencedMessage>(new ComponentEventHandler<AtmosAlertsComputerComponent, AtmosAlertsComputerDeviceSilencedMessage>((object) this, __methodptr(OnDeviceSilencedMessage)), (Type[]) null, (Type[]) null);
  }

  private void OnDeviceSilencedMessage(
    EntityUid uid,
    AtmosAlertsComputerComponent component,
    AtmosAlertsComputerDeviceSilencedMessage args)
  {
    if (args.SilenceDevice)
      component.SilencedDevices.Add(args.AtmosDevice);
    else
      component.SilencedDevices.Remove(args.AtmosDevice);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }
}
