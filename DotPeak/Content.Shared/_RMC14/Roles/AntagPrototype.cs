// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.AntagPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Prototypes;
using Content.Shared.Guidebook;
using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Roles;

[Prototype(null, 1)]
public sealed class AntagPrototype : IPrototype, ICMSpecific
{
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedRoleSystem)}, Other = AccessPermissions.None)]
  public HashSet<JobRequirement>? Requirements;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<GuideEntryPrototype>>? Guides;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; private set; } = "";

  [DataField("objective", false, 1, true, false, null)]
  public string Objective { get; private set; } = "";

  [DataField("antagonist", false, 1, false, false, null)]
  public bool Antagonist { get; private set; }

  [DataField("setPreference", false, 1, false, false, null)]
  public bool SetPreference { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool IsCM { get; private set; }
}
