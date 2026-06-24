// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Prototypes.BodyPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Body.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class BodyPrototype : IPrototype, IInheritingPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; private set; } = "";

  [DataField("root", false, 1, false, false, null)]
  public string Root { get; private set; } = string.Empty;

  [DataField("slots", false, 1, false, false, null)]
  public Dictionary<string, BodyPrototypeSlot> Slots { get; private set; } = new Dictionary<string, BodyPrototypeSlot>();

  private BodyPrototype()
  {
  }

  public BodyPrototype(
    string id,
    string name,
    string root,
    Dictionary<string, BodyPrototypeSlot> slots)
  {
    this.ID = id;
    this.Name = name;
    this.Root = root;
    this.Slots = slots;
  }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<BodyPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [AbstractDataField(1)]
  public bool Abstract { get; private set; }
}
