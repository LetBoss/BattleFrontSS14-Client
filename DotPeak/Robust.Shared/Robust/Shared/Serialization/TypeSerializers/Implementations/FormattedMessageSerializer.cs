// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.FormattedMessageSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class FormattedMessageSerializer : 
  ITypeSerializer<FormattedMessage, ValueDataNode>,
  ITypeReader<FormattedMessage, ValueDataNode>,
  ITypeValidator<FormattedMessage, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<FormattedMessage, ValueDataNode>,
  ITypeWriter<FormattedMessage>,
  BaseSerializerInterfaces.ITypeInterface<FormattedMessage>,
  ITypeCopyCreator<FormattedMessage>
{
  public FormattedMessage Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<FormattedMessage>? instanceProvider = null)
  {
    return FormattedMessage.FromMarkupOrThrow(node.Value);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !FormattedMessage.ValidMarkup(node.Value) ? (ValidationNode) new ErrorNode((DataNode) node, "Invalid markup in FormattedMessage.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    FormattedMessage value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToMarkup());
  }

  public FormattedMessage CreateCopy(
    ISerializationManager serializationManager,
    FormattedMessage source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new FormattedMessage(source);
  }
}
