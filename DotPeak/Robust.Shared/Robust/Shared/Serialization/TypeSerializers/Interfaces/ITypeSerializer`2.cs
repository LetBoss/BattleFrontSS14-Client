// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Interfaces.ITypeSerializer`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Interfaces;

public interface ITypeSerializer<TType, TNode> : 
  ITypeReader<TType, TNode>,
  ITypeValidator<TType, TNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<TType, TNode>,
  ITypeWriter<TType>,
  BaseSerializerInterfaces.ITypeInterface<TType>
  where TNode : DataNode
{
}
