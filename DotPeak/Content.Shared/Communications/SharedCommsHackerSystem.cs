// Decompiled with JetBrains decompiler
// Type: Content.Shared.Communications.SharedCommsHackerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.Communications;

public abstract class SharedCommsHackerSystem : EntitySystem
{
  public void SetThreats(EntityUid uid, string threats, CommsHackerComponent? comp = null)
  {
    if (!this.Resolve<CommsHackerComponent>(uid, ref comp, true))
      return;
    comp.Threats = ProtoId<WeightedRandomPrototype>.op_Implicit(threats);
  }
}
