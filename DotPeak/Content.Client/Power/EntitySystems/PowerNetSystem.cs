// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.EntitySystems.PowerNetSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power.Components;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;

#nullable enable
namespace Content.Client.Power.EntitySystems;

public sealed class PowerNetSystem : SharedPowerNetSystem
{
  public override bool IsPoweredCalculate(SharedApcPowerReceiverComponent comp)
  {
    return this.IsPoweredCalculate((ApcPowerReceiverComponent) comp);
  }

  private bool IsPoweredCalculate(ApcPowerReceiverComponent comp)
  {
    return !comp.PowerDisabled && !comp.NeedsPower;
  }
}
