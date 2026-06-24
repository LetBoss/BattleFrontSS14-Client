// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.SpriteSpecifierSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

public abstract class SpriteSpecifierSerializer : 
  ITypeSerializer<SpriteSpecifier.Texture, ValueDataNode>,
  ITypeReader<SpriteSpecifier.Texture, ValueDataNode>,
  ITypeValidator<SpriteSpecifier.Texture, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier.Texture, ValueDataNode>,
  ITypeWriter<SpriteSpecifier.Texture>,
  BaseSerializerInterfaces.ITypeInterface<SpriteSpecifier.Texture>,
  ITypeSerializer<SpriteSpecifier.EntityPrototype, ValueDataNode>,
  ITypeReader<SpriteSpecifier.EntityPrototype, ValueDataNode>,
  ITypeValidator<SpriteSpecifier.EntityPrototype, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier.EntityPrototype, ValueDataNode>,
  ITypeWriter<SpriteSpecifier.EntityPrototype>,
  BaseSerializerInterfaces.ITypeInterface<SpriteSpecifier.EntityPrototype>,
  ITypeSerializer<SpriteSpecifier.Rsi, MappingDataNode>,
  ITypeReader<SpriteSpecifier.Rsi, MappingDataNode>,
  ITypeValidator<SpriteSpecifier.Rsi, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier.Rsi, MappingDataNode>,
  ITypeWriter<SpriteSpecifier.Rsi>,
  BaseSerializerInterfaces.ITypeInterface<SpriteSpecifier.Rsi>,
  ITypeSerializer<SpriteSpecifier, MappingDataNode>,
  ITypeReader<SpriteSpecifier, MappingDataNode>,
  ITypeValidator<SpriteSpecifier, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier, MappingDataNode>,
  ITypeWriter<SpriteSpecifier>,
  BaseSerializerInterfaces.ITypeInterface<SpriteSpecifier>,
  ITypeSerializer<SpriteSpecifier, ValueDataNode>,
  ITypeReader<SpriteSpecifier, ValueDataNode>,
  ITypeValidator<SpriteSpecifier, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier, ValueDataNode>,
  ITypeCopyCreator<SpriteSpecifier>,
  ITypeCopyCreator<SpriteSpecifier.Rsi>,
  ITypeCopyCreator<SpriteSpecifier.Texture>,
  ITypeCopyCreator<SpriteSpecifier.EntityPrototype>,
  ITypeCopier<SpriteSpecifier.Rsi>,
  ITypeCopier<SpriteSpecifier.Texture>
{
  public static readonly ResPath TextureRoot = new ResPath("/Textures");

  SpriteSpecifier.Texture ITypeReader<SpriteSpecifier.Texture, ValueDataNode>.Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<SpriteSpecifier.Texture>? instanceProvider)
  {
    return new SpriteSpecifier.Texture(serializationManager.Read<ResPath>((DataNode) node, hookCtx, context));
  }

  SpriteSpecifier ITypeReader<SpriteSpecifier, ValueDataNode>.Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<SpriteSpecifier>? instanceProvider)
  {
    return (SpriteSpecifier) ((ITypeReader<SpriteSpecifier.Texture, ValueDataNode>) this).Read(serializationManager, node, dependencies, hookCtx, context, (ISerializationManager.InstantiationDelegate<SpriteSpecifier.Texture>) instanceProvider);
  }

  SpriteSpecifier.EntityPrototype ITypeReader<SpriteSpecifier.EntityPrototype, ValueDataNode>.Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<SpriteSpecifier.EntityPrototype>? instanceProvider)
  {
    return new SpriteSpecifier.EntityPrototype(node.Value);
  }

  SpriteSpecifier.Rsi ITypeReader<SpriteSpecifier.Rsi, MappingDataNode>.Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<SpriteSpecifier.Rsi>? instanceProvider)
  {
    DataNode node1;
    if (!node.TryGet("sprite", out node1))
      throw new InvalidMappingException("Expected sprite-node");
    DataNode node2;
    if (!node.TryGet("state", out node2) || !(node2 is ValueDataNode valueDataNode))
      throw new InvalidMappingException("Expected state-node as a valuenode");
    return new SpriteSpecifier.Rsi(serializationManager.Read<ResPath>(node1, hookCtx, context), valueDataNode.Value);
  }

  SpriteSpecifier ITypeReader<SpriteSpecifier, MappingDataNode>.Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<SpriteSpecifier>? instanceProvider)
  {
    DataNode node1;
    return node.TryGet("entity", out node1) && node1 is ValueDataNode node2 ? (SpriteSpecifier) ((ITypeReader<SpriteSpecifier.EntityPrototype, ValueDataNode>) this).Read(serializationManager, node2, dependencies, hookCtx, context, (ISerializationManager.InstantiationDelegate<SpriteSpecifier.EntityPrototype>) instanceProvider) : (SpriteSpecifier) ((ITypeReader<SpriteSpecifier.Rsi, MappingDataNode>) this).Read(serializationManager, node, dependencies, hookCtx, context, (ISerializationManager.InstantiationDelegate<SpriteSpecifier.Rsi>) instanceProvider);
  }

  ValidationNode ITypeValidator<SpriteSpecifier, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return ((ITypeValidator<SpriteSpecifier.Texture, ValueDataNode>) this).Validate(serializationManager, node, dependencies, context);
  }

  ValidationNode ITypeValidator<SpriteSpecifier.EntityPrototype, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return dependencies.Resolve<IPrototypeManager>().HasIndex<Robust.Shared.Prototypes.EntityPrototype>(node.Value) ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, "Invalid EntityPrototype id");
  }

  ValidationNode ITypeValidator<SpriteSpecifier.Texture, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    ResPath resPath = SpriteSpecifierSerializer.TextureRoot / node.Value;
    return resPath.ToString().Contains(".rsi/") ? (ValidationNode) new ErrorNode((DataNode) node, "Texture paths may not be inside RSI files.") : serializationManager.ValidateNode<ResPath>((DataNode) new ValueDataNode(resPath.ToString()), context);
  }

  ValidationNode ITypeValidator<SpriteSpecifier, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    DataNode node1;
    if (!node.TryGet("entity", out node1))
      return ((ITypeValidator<SpriteSpecifier.Rsi, MappingDataNode>) this).Validate(serializationManager, node, dependencies, context);
    return node1 is ValueDataNode node2 ? ((ITypeValidator<SpriteSpecifier.EntityPrototype, ValueDataNode>) this).Validate(serializationManager, node2, dependencies, context) : (ValidationNode) new ErrorNode((DataNode) node, "Sprite specifier entity node must be a ValueDataNode");
  }

  ValidationNode ITypeValidator<SpriteSpecifier.Rsi, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.ValidateRsi(serializationManager, node, dependencies, context);
  }

  public abstract ValidationNode ValidateRsi(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context);

  public DataNode Write(
    ISerializationManager serializationManager,
    SpriteSpecifier.Texture value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return serializationManager.WriteValue<ResPath>(value.TexturePath, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    SpriteSpecifier.EntityPrototype value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new MappingDataNode()
    {
      {
        "entity",
        (DataNode) new ValueDataNode(value.EntityPrototypeId)
      }
    };
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    SpriteSpecifier.Rsi value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new MappingDataNode()
    {
      {
        "sprite",
        serializationManager.WriteValue<ResPath>(value.RsiPath)
      },
      {
        "state",
        (DataNode) new ValueDataNode(value.RsiState)
      }
    };
  }

  public SpriteSpecifier.Texture CreateCopy(
    ISerializationManager serializationManager,
    SpriteSpecifier.Texture source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new SpriteSpecifier.Texture(source.TexturePath);
  }

  public SpriteSpecifier.EntityPrototype CreateCopy(
    ISerializationManager serializationManager,
    SpriteSpecifier.EntityPrototype source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new SpriteSpecifier.EntityPrototype(source.EntityPrototypeId);
  }

  public SpriteSpecifier.Rsi CreateCopy(
    ISerializationManager serializationManager,
    SpriteSpecifier.Rsi source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new SpriteSpecifier.Rsi(source.RsiPath, source.RsiState);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    SpriteSpecifier value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    switch (value)
    {
      case SpriteSpecifier.Rsi rsi:
        return this.Write(serializationManager, rsi, dependencies, alwaysWrite, context);
      case SpriteSpecifier.Texture texture:
        return this.Write(serializationManager, texture, dependencies, alwaysWrite, context);
      case SpriteSpecifier.EntityPrototype entityPrototype:
        return this.Write(serializationManager, entityPrototype, dependencies, alwaysWrite, context);
      default:
        throw new InvalidOperationException("Invalid SpriteSpecifier specified!");
    }
  }

  public SpriteSpecifier CreateCopy(
    ISerializationManager serializationManager,
    SpriteSpecifier source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    switch (source)
    {
      case SpriteSpecifier.Rsi source1:
        return (SpriteSpecifier) this.CreateCopy(serializationManager, source1, dependencies, hookCtx, context);
      case SpriteSpecifier.Texture source2:
        return (SpriteSpecifier) this.CreateCopy(serializationManager, source2, dependencies, hookCtx, context);
      case SpriteSpecifier.EntityPrototype source3:
        return (SpriteSpecifier) this.CreateCopy(serializationManager, source3, dependencies, hookCtx, context);
      default:
        throw new InvalidOperationException("Invalid SpriteSpecifier specified!");
    }
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    SpriteSpecifier.Rsi source,
    ref SpriteSpecifier.Rsi target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.RsiPath = source.RsiPath;
    target.RsiState = source.RsiState;
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    SpriteSpecifier.Texture source,
    ref SpriteSpecifier.Texture target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.TexturePath = source.TexturePath;
  }
}
