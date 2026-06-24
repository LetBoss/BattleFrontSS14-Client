// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.Roles.GhostRole
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Ghost.Roles;

[NetSerializable]
[Serializable]
public sealed class GhostRole
{
  public NetEntity Id;

  public string Name { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;
}
