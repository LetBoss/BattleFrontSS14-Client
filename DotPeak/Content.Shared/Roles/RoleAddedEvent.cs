// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.RoleAddedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Roles;

public sealed record RoleAddedEvent(
  EntityUid MindId,
  MindComponent Mind,
  bool RoleTypeUpdate,
  bool Silent = false) : RoleEvent(MindId, Mind, RoleTypeUpdate)
{
  public bool Silent { get; init; } = Silent;

  [CompilerGenerated]
  public sealed override bool Equals(RoleEvent? other) => this.Equals((object) other);

  [CompilerGenerated]
  public void Deconstruct(
    out EntityUid MindId,
    out MindComponent Mind,
    out bool RoleTypeUpdate,
    out bool Silent)
  {
    MindId = this.MindId;
    Mind = this.Mind;
    RoleTypeUpdate = this.RoleTypeUpdate;
    Silent = this.Silent;
  }
}
