// Decompiled with JetBrains decompiler
// Type: Content.Shared.Disposal.Unit.SharedDisposalTubeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Tube;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Disposal.Unit;

public abstract class SharedDisposalTubeSystem : EntitySystem
{
  public virtual bool TryInsert(
    EntityUid uid,
    DisposalUnitComponent from,
    IEnumerable<string>? tags = null,
    DisposalEntryComponent? entry = null)
  {
    return false;
  }
}
