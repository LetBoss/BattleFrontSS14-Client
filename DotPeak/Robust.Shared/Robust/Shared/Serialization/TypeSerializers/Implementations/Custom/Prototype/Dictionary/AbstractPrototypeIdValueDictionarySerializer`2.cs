// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary.AbstractPrototypeIdValueDictionarySerializer`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

public sealed class AbstractPrototypeIdValueDictionarySerializer<TValue, TPrototype> : 
  PrototypeIdValueDictionarySerializer<TValue, TPrototype>
  where TValue : notnull
  where TPrototype : class, IPrototype, IInheritingPrototype
{
  protected override PrototypeIdSerializer<TPrototype> PrototypeSerializer
  {
    get => (PrototypeIdSerializer<TPrototype>) new AbstractPrototypeIdSerializer<TPrototype>();
  }
}
