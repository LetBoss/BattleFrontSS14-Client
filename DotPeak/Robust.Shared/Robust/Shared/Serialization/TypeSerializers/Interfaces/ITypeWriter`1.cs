// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Interfaces.ITypeWriter`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Interfaces;

public interface ITypeWriter<TType> : BaseSerializerInterfaces.ITypeInterface<TType>
{
  DataNode Write(
    ISerializationManager serializationManager,
    TType value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null);
}
