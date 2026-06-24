// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.ResPathSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class ResPathSerializer : 
  ITypeSerializer<ResPath, ValueDataNode>,
  ITypeReader<ResPath, ValueDataNode>,
  ITypeValidator<ResPath, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<ResPath, ValueDataNode>,
  ITypeWriter<ResPath>,
  BaseSerializerInterfaces.ITypeInterface<ResPath>,
  ITypeCopyCreator<ResPath>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    ResPath path = ResPath.FromRelativeSystemPath(node.Value);
    if (path.Extension.Equals("rsi"))
      path /= "meta.json";
    if (!((IEnumerable<string>) path.CanonPath.Split('/')).First<string>().Equals("Textures", StringComparison.InvariantCultureIgnoreCase))
      path = SpriteSpecifierSerializer.TextureRoot / path;
    path = path.ToRootedPath();
    try
    {
      IResourceManager resourceManager = dependencies.Resolve<IResourceManager>();
      if (node.Value.EndsWith('/'))
      {
        if (resourceManager.ContentGetDirectoryEntries(path).Any<string>())
          return (ValidationNode) new ValidatedValueNode((DataNode) node);
        return (ValidationNode) new ErrorNode((DataNode) node, $"Folder not found. ({path})");
      }
      if (resourceManager.ContentFileExists(path))
        return (ValidationNode) new ValidatedValueNode((DataNode) node);
      return (ValidationNode) new ErrorNode((DataNode) node, $"File not found. ({path})");
    }
    catch (Exception ex)
    {
      return (ValidationNode) new ErrorNode((DataNode) node, $"Failed parsing filepath. ({path}) ({ex.Message})");
    }
  }

  public ResPath Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<ResPath>? instanceProvider = null)
  {
    return new ResPath(node.Value);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    ResPath value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToString());
  }

  public ResPath CreateCopy(
    ISerializationManager serializationManager,
    ResPath source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new ResPath(source.ToString());
  }
}
