// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.RoleRemovedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Content.Shared.Roles;

public sealed record RoleRemovedEvent(EntityUid MindId, MindComponent Mind, bool RoleTypeUpdate) : 
  RoleEvent(MindId, Mind, RoleTypeUpdate)
{
  [CompilerGenerated]
  protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);

  [CompilerGenerated]
  public override int GetHashCode() => base.GetHashCode();

  [CompilerGenerated]
  public sealed override bool Equals(RoleEvent? other) => this.Equals((object) other);

  [CompilerGenerated]
  public bool Equals(RoleRemovedEvent? other)
  {
    return (object) this == (object) other || base.Equals((RoleEvent) other);
  }

  [CompilerGenerated]
  public new void Deconstruct(
    out EntityUid MindId,
    out MindComponent Mind,
    out bool RoleTypeUpdate)
  {
    MindId = this.MindId;
    Mind = this.Mind;
    RoleTypeUpdate = this.RoleTypeUpdate;
  }
}
