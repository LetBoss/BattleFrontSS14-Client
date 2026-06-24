// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Prototypes.ConstructionPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared._RMC14.Prototypes;
using Content.Shared.Construction.Conditions;
using Content.Shared.Whitelist;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Construction.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class ConstructionPrototype : IPrototype, IInheritingPrototype, ICMSpecific
{
  [DataField("conditions", false, 1, false, false, null)]
  private List<IConstructionCondition> _conditions = new List<IConstructionCondition>();
  [DataField(null, false, 1, false, false, null)]
  public bool Hide;
  [DataField("name", false, 1, false, false, null)]
  public string? SetName;
  public string? Name;
  [DataField("description", false, 1, false, false, null)]
  public string? SetDescription;
  public string? Description;
  [DataField(null, false, 1, false, false, null)]
  public string PlacementMode = "PlaceFree";
  [DataField(null, false, 1, false, false, null)]
  public bool CanRotate = true;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ConstructionPrototype>[] AlternativePrototypes = System.Array.Empty<ProtoId<ConstructionPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public Color IconColor = Color.FromHex((ReadOnlySpan<char>) "#ffffff", new Color?());

  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ConstructionGraphPrototype> Graph { get; private set; } = ProtoId<ConstructionGraphPrototype>.op_Implicit(string.Empty);

  [DataField(null, false, 1, true, false, null)]
  public string TargetNode { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  public string StartNode { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool CanBuildInImpassable { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? EntityWhitelist { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string Category { get; private set; } = string.Empty;

  [DataField("objectType", false, 1, false, false, null)]
  public ConstructionType Type { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ConstructionPrototype>? Mirror { get; private set; }

  public IReadOnlyList<IConstructionCondition> Conditions
  {
    get => (IReadOnlyList<IConstructionCondition>) this._conditions;
  }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<ConstructionPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool IsCM { get; private set; }

  [DataField("rmcPrototype", false, 1, false, false, null)]
  public ProtoId<RMCConstructionPrototype>? RMCPrototype { get; private set; }
}
