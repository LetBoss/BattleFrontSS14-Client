// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.EditSolutionsEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Administration;

[NetSerializable]
[Serializable]
public sealed class EditSolutionsEuiState : EuiStateBase
{
  public readonly NetEntity Target;
  public readonly List<(string, NetEntity)>? Solutions;
  public readonly GameTick Tick;

  public EditSolutionsEuiState(
    NetEntity target,
    List<(string, NetEntity)>? solutions,
    GameTick tick)
  {
    this.Target = target;
    this.Solutions = solutions;
    this.Tick = tick;
  }
}
