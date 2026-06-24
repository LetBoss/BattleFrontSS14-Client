// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reaction.ReactionPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.Reaction;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class ReactionPrototype : IPrototype, IComparable<ReactionPrototype>
{
  [DataField("reactants", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<ReactantPrototype, ReagentPrototype>))]
  public System.Collections.Generic.Dictionary<string, ReactantPrototype> Reactants = new System.Collections.Generic.Dictionary<string, ReactantPrototype>();
  [DataField("minTemp", false, 1, false, false, null)]
  public float MinimumTemperature;
  [DataField("conserveEnergy", false, 1, false, false, null)]
  public bool ConserveEnergy = true;
  [DataField("maxTemp", false, 1, false, false, null)]
  public float MaximumTemperature = float.PositiveInfinity;
  [DataField("requiredMixerCategories", false, 1, false, false, null)]
  public List<ProtoId<MixingCategoryPrototype>>? MixingCategories;
  [DataField("products", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<FixedPoint2, ReagentPrototype>))]
  public System.Collections.Generic.Dictionary<string, FixedPoint2> Products = new System.Collections.Generic.Dictionary<string, FixedPoint2>();
  [DataField("effects", false, 1, false, false, null)]
  public List<EntityEffect> Effects = new List<EntityEffect>();
  [DataField("impact", false, 1, false, true, null)]
  public LogImpact Impact = LogImpact.Low;
  [DataField("quantized", false, 1, false, false, null)]
  public bool Quantized;
  [DataField("priority", false, 1, false, false, null)]
  public int Priority;
  [DataField(null, false, 1, false, false, null)]
  public bool Source;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; private set; } = string.Empty;

  [DataField("sound", false, 1, false, true, null)]
  public SoundSpecifier Sound { get; private set; } = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Chemistry/bubbles.ogg", new AudioParams?());

  public int CompareTo(ReactionPrototype? other)
  {
    if (other == null)
      return -1;
    if (this.Priority != other.Priority)
      return other.Priority - this.Priority;
    return this.Products.Count != other.Products.Count ? this.Products.Count - other.Products.Count : string.Compare(this.ID, other.ID, StringComparison.Ordinal);
  }
}
