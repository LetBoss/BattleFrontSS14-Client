// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bql.ToolshedVisualizeEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Bql;

[NetSerializable]
[Serializable]
public sealed class ToolshedVisualizeEuiState : EuiStateBase
{
  public readonly (string name, NetEntity entity)[] Entities;

  public ToolshedVisualizeEuiState((string name, NetEntity entity)[] entities)
  {
    this.Entities = entities;
  }
}
