// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.SpriteSpecifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Utility;

[NetSerializable]
[Serializable]
public abstract class SpriteSpecifier
{
  public static readonly SpriteSpecifier Invalid = (SpriteSpecifier) new SpriteSpecifier.Texture(ResPath.Self);

  public static SpriteSpecifier FromYaml(YamlNode node)
  {
    switch (node)
    {
      case YamlScalarNode _:
        return (SpriteSpecifier) new SpriteSpecifier.Texture(node.AsResourcePath());
      case YamlMappingNode yamlMappingNode:
        return (SpriteSpecifier) new SpriteSpecifier.Rsi(((YamlNode) yamlMappingNode)[YamlNode.op_Implicit("sprite")].AsResourcePath(), ((YamlNode) yamlMappingNode)[YamlNode.op_Implicit("state")].AsString());
      default:
        throw new InvalidOperationException();
    }
  }

  [NetSerializable]
  [DataDefinition]
  [Serializable]
  public sealed class Rsi : 
    SpriteSpecifier,
    ISerializationGenerated<SpriteSpecifier.Rsi>,
    ISerializationGenerated
  {
    [DataField("sprite", false, 1, false, false, null)]
    public ResPath RsiPath { get; internal set; }

    [DataField("state", false, 1, false, false, null)]
    public string RsiState { get; internal set; }

    public Rsi(ResPath rsiPath, string rsiState)
    {
      this.RsiPath = rsiPath;
      this.RsiState = rsiState;
    }

    public override bool Equals(object? obj)
    {
      return obj is SpriteSpecifier.Rsi rsi && rsi.RsiPath == this.RsiPath && rsi.RsiState == this.RsiState;
    }

    public override int GetHashCode() => this.RsiPath.GetHashCode() ^ this.RsiState.GetHashCode();

    public Rsi()
    {
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SpriteSpecifier.Rsi target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this, ref target, hookCtx, false, context))
        return;
      ResPath target1 = new ResPath();
      if (!serialization.TryCustomCopy<ResPath>(this.RsiPath, ref target1, hookCtx, false, context))
        target1 = serialization.CreateCopy<ResPath>(this.RsiPath, hookCtx, context);
      target.RsiPath = target1;
      string target2 = (string) null;
      if (this.RsiState == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string>(this.RsiState, ref target2, hookCtx, false, context))
        target2 = this.RsiState;
      target.RsiState = target2;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SpriteSpecifier.Rsi target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SpriteSpecifier.Rsi target1 = (SpriteSpecifier.Rsi) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public SpriteSpecifier.Rsi Instantiate() => new SpriteSpecifier.Rsi();
  }

  [NetSerializable]
  [Serializable]
  public sealed class Texture : SpriteSpecifier
  {
    public ResPath TexturePath { get; internal set; }

    private Texture() => this.TexturePath = new ResPath();

    public Texture(ResPath texturePath) => this.TexturePath = texturePath;

    public override bool Equals(object? obj)
    {
      return obj is SpriteSpecifier.Texture texture && texture.TexturePath == this.TexturePath;
    }

    public override int GetHashCode() => this.TexturePath.GetHashCode();
  }

  [NetSerializable]
  [Serializable]
  public sealed class EntityPrototype : SpriteSpecifier
  {
    public readonly string EntityPrototypeId;

    public EntityPrototype(string entityPrototypeId) => this.EntityPrototypeId = entityPrototypeId;

    public override bool Equals(object? obj)
    {
      return obj is SpriteSpecifier.EntityPrototype entityPrototype && this.EntityPrototypeId == entityPrototype.EntityPrototypeId;
    }

    public override int GetHashCode() => this.EntityPrototypeId.GetHashCode();
  }
}
