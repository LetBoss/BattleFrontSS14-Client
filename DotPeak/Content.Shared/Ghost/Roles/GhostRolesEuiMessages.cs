// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.Roles.GhostRoleInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Ghost.Roles;

[NetSerializable]
[Serializable]
public struct GhostRoleInfo
{
  public uint Identifier { get; set; }

  public string Name { get; set; }

  public string Description { get; set; }

  public string Rules { get; set; }

  public HashSet<JobRequirement>? Requirements { get; set; }

  public GhostRoleKind Kind { get; set; }

  public uint RafflePlayerCount { get; set; }

  public TimeSpan RaffleEndTime { get; set; }
}
