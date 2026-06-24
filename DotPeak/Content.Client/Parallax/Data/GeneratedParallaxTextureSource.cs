// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Data.GeneratedParallaxTextureSource
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Parallax.Managers;
using Robust.Client.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Parallax.Data;

[DataDefinition]
public sealed class GeneratedParallaxTextureSource : 
  IParallaxTextureSource,
  ISerializationGenerated<IParallaxTextureSource>,
  ISerializationGenerated,
  ISerializationGenerated<GeneratedParallaxTextureSource>
{
  [DataField("configPath", false, 1, false, false, null)]
  public ResPath ParallaxConfigPath { get; private set; } = new ResPath("/parallax_config.toml");

  [DataField("id", false, 1, false, false, null)]
  public string Identifier { get; private set; } = "other";

  async Task<Texture> IParallaxTextureSource.GenerateTexture(CancellationToken cancel)
  {
    return await IoCManager.Resolve<GeneratedParallaxCache>().Load(this.Identifier, this.ParallaxConfigPath, cancel);
  }

  void IParallaxTextureSource.Unload(IDependencyCollection dependencies)
  {
    dependencies.Resolve<GeneratedParallaxCache>().Unload(this.Identifier);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GeneratedParallaxTextureSource target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<GeneratedParallaxTextureSource>(this, ref target, hookCtx, false, context))
      return;
    ResPath resPath = new ResPath();
    if (!serialization.TryCustomCopy<ResPath>(this.ParallaxConfigPath, ref resPath, hookCtx, false, context))
      resPath = serialization.CreateCopy<ResPath>(this.ParallaxConfigPath, hookCtx, context, false);
    target.ParallaxConfigPath = resPath;
    string str = (string) null;
    if (this.Identifier == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Identifier, ref str, hookCtx, false, context))
      str = this.Identifier;
    target.Identifier = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GeneratedParallaxTextureSource target,
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
    GeneratedParallaxTextureSource target1 = (GeneratedParallaxTextureSource) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IParallaxTextureSource target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GeneratedParallaxTextureSource target1 = (GeneratedParallaxTextureSource) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IParallaxTextureSource) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IParallaxTextureSource target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public GeneratedParallaxTextureSource Instantiate() => new GeneratedParallaxTextureSource();

  IParallaxTextureSource IParallaxTextureSource.Instantiate()
  {
    return (IParallaxTextureSource) this.Instantiate();
  }

  IParallaxTextureSource ISerializationGenerated<IParallaxTextureSource>.Instantiate()
  {
    return (IParallaxTextureSource) this.Instantiate();
  }
}
