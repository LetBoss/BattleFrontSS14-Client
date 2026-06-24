// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Prototypes.RandomHumanoidSettingsPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class RandomHumanoidSettingsPrototype : IPrototype, IInheritingPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [ParentDataField(typeof (PrototypeIdArraySerializer<RandomHumanoidSettingsPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [AbstractDataField(1)]
  [NeverPushInheritance]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool RandomizeName { get; private set; } = true;

  [DataField("speciesBlacklist", false, 1, false, false, null)]
  public HashSet<string> SpeciesBlacklist { get; private set; } = new HashSet<string>();

  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public ComponentRegistry? Components { get; private set; }
}
