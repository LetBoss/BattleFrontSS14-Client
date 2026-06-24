// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.OreSilo.OreSiloBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Materials.OreSilo;

[NetSerializable]
[Serializable]
public sealed class OreSiloBuiState : BoundUserInterfaceState
{
  public readonly HashSet<(NetEntity, string, string)> Clients;

  public OreSiloBuiState(HashSet<(NetEntity, string, string)> clients) => this.Clients = clients;
}
