// Decompiled with JetBrains decompiler
// Type: Content.Shared.SprayPainter.Prototypes.AirlockDepartmentsPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.SprayPainter.Prototypes;

[Prototype(null, 1)]
public sealed class AirlockDepartmentsPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<string, ProtoId<DepartmentPrototype>> Departments = new Dictionary<string, ProtoId<DepartmentPrototype>>();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
