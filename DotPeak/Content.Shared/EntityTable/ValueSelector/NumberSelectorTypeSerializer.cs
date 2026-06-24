// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.ValueSelector.NumberSelectorTypeSerializer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using System;
using System.Globalization;

#nullable enable
namespace Content.Shared.EntityTable.ValueSelector;

[TypeSerializer]
public sealed class NumberSelectorTypeSerializer : 
  ITypeReader<NumberSelector, ValueDataNode>,
  ITypeValidator<NumberSelector, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<NumberSelector, ValueDataNode>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    if (int.TryParse(node.Value, out int _))
      return (ValidationNode) new ValidatedValueNode((DataNode) node);
    return VectorSerializerUtility.TryParseArgs(node.Value, 2, out string[] _) ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, "Custom validation not supported! Please specify the type manually!");
  }

  public NumberSelector Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<NumberSelector>? instanceProvider = null)
  {
    Type type = typeof (NumberSelector);
    int result;
    if (int.TryParse(node.Value, out result))
      return (NumberSelector) new ConstantNumberSelector(result);
    string[] args;
    return VectorSerializerUtility.TryParseArgs(node.Value, 2, out args) ? (NumberSelector) new RangeNumberSelector(new Vector2i(int.Parse(args[0], (IFormatProvider) CultureInfo.InvariantCulture), int.Parse(args[1], (IFormatProvider) CultureInfo.InvariantCulture))) : (NumberSelector) serializationManager.Read(type, (DataNode) node, context);
  }
}
