// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.Roles.GhostRolePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Ghost.Roles;

[Prototype(null, 1)]
public sealed class GhostRolePrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId EntityPrototype;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? IconPrototype;
  [DataField(null, false, 1, true, false, null)]
  public string Rules;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  public string Name { get; set; }

  [DataField(null, false, 1, true, false, null)]
  public string Description { get; set; }
}
