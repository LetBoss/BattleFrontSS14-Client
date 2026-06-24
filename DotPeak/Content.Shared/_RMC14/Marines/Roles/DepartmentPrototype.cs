// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.DepartmentPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Prototypes;
using Content.Shared.Radio;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Roles;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class DepartmentPrototype : IPrototype, IInheritingPrototype, ICMSpecific
{
  [DataField(null, false, 1, true, false, null)]
  public LocId Name = (LocId) string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public LocId Description = (LocId) string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public Color Color;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public List<ProtoId<JobPrototype>> Roles = new List<ProtoId<JobPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Primary = true;
  [DataField(null, false, 1, false, false, null)]
  public bool EditorHidden;
  [DataField(null, false, 1, false, false, null)]
  public string? CustomName;
  [DataField(null, false, 1, false, false, null)]
  public bool Hidden;

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public int Weight { get; private set; }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<DepartmentPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool IsCM { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RadioChannelPrototype>? DepartmentRadio { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype>? HeadOfDepartment { get; private set; }
}
