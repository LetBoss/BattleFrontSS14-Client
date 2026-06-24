// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Prototypes.CargoProductPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared.Cargo.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class CargoProductPrototype : IPrototype, IInheritingPrototype
{
  [DataField("name", false, 1, false, false, null)]
  private string _name = string.Empty;
  [DataField("description", false, 1, false, false, null)]
  private string _description = string.Empty;

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<CargoProductPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Name
  {
    get
    {
      EntityPrototype entityPrototype;
      if (this._name.Trim().Length != 0 || !IoCManager.Resolve<IPrototypeManager>().TryIndex(this.Product, ref entityPrototype))
        return this._name;
      this._name = entityPrototype.Name;
      return this._name;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Description
  {
    get
    {
      EntityPrototype entityPrototype;
      if (this._description.Trim().Length != 0 || !IoCManager.Resolve<IPrototypeManager>().TryIndex(this.Product, ref entityPrototype))
        return this._description;
      this._description = entityPrototype.Description;
      return this._description;
    }
  }

  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier Icon { get; private set; } = SpriteSpecifier.Invalid;

  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Product { get; private set; } = EntProtoId.op_Implicit(string.Empty);

  [DataField(null, false, 1, false, false, null)]
  public int Cost { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string Category { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<CargoMarketPrototype> Group { get; private set; } = ProtoId<CargoMarketPrototype>.op_Implicit("market");
}
