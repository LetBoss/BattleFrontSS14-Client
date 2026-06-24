// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Interfaces.ITypeValidator`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Interfaces;

public interface ITypeValidator<TType, TNode> : 
  BaseSerializerInterfaces.ITypeNodeInterface<TType, TNode>
  where TNode : DataNode
{
  ValidationNode Validate(
    ISerializationManager serializationManager,
    TNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null);
}
