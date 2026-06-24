// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Systems.SharedMetabolizerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Events;
using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Body.Systems;

public abstract class SharedMetabolizerSystem : EntitySystem
{
  public void UpdateMetabolicMultiplier(EntityUid uid)
  {
    GetMetabolicMultiplierEvent metabolicMultiplierEvent1 = new GetMetabolicMultiplierEvent();
    this.RaiseLocalEvent<GetMetabolicMultiplierEvent>(uid, ref metabolicMultiplierEvent1, false);
    ApplyMetabolicMultiplierEvent metabolicMultiplierEvent2 = new ApplyMetabolicMultiplierEvent(metabolicMultiplierEvent1.Multiplier);
    this.RaiseLocalEvent<ApplyMetabolicMultiplierEvent>(uid, ref metabolicMultiplierEvent2, false);
  }
}
