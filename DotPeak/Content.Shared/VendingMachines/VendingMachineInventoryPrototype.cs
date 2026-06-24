// Decompiled with JetBrains decompiler
// Type: Content.Shared.VendingMachines.VendingMachineInventoryPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

#nullable enable
namespace Content.Shared.VendingMachines;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class VendingMachineInventoryPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("startingInventory", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
  public System.Collections.Generic.Dictionary<string, uint> StartingInventory { get; private set; } = new System.Collections.Generic.Dictionary<string, uint>();

  [DataField("emaggedInventory", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
  public System.Collections.Generic.Dictionary<string, uint>? EmaggedInventory { get; private set; }

  [DataField("contrabandInventory", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
  public System.Collections.Generic.Dictionary<string, uint>? ContrabandInventory { get; private set; }
}
