// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.IdentityManagement.IdentityEntity
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Shared._RMC14.IdentityManagement;

public readonly record struct IdentityEntity(EntityUid Entity, string Name) : ILocValue
{
  public static implicit operator EntityUid(IdentityEntity ent) => ent.Entity;

  public static implicit operator string(IdentityEntity ent) => ent.Name;

  public string Format(LocContext ctx) => this.Name;

  public object Value => (object) this.Entity;

  public override string ToString() => this.Name;
}
