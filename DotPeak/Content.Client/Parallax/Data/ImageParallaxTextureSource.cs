// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Data.ImageParallaxTextureSource
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.IoC;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Parallax.Data;

[DataDefinition]
public sealed class ImageParallaxTextureSource : 
  IParallaxTextureSource,
  ISerializationGenerated<IParallaxTextureSource>,
  ISerializationGenerated,
  ISerializationGenerated<ImageParallaxTextureSource>
{
  [DataField("path", false, 1, true, false, null)]
  public ResPath Path { get; private set; }

  Task<Texture> IParallaxTextureSource.GenerateTexture(CancellationToken cancel)
  {
    return Task.FromResult<Texture>(StaticIoC.ResC.GetTexture(this.Path));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ImageParallaxTextureSource target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ImageParallaxTextureSource>(this, ref target, hookCtx, false, context))
      return;
    ResPath resPath = new ResPath();
    if (!serialization.TryCustomCopy<ResPath>(this.Path, ref resPath, hookCtx, false, context))
      resPath = serialization.CreateCopy<ResPath>(this.Path, hookCtx, context, false);
    target.Path = resPath;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ImageParallaxTextureSource target,
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
    ImageParallaxTextureSource target1 = (ImageParallaxTextureSource) target;
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
    ImageParallaxTextureSource target1 = (ImageParallaxTextureSource) target;
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
  public ImageParallaxTextureSource Instantiate() => new ImageParallaxTextureSource();

  IParallaxTextureSource IParallaxTextureSource.Instantiate()
  {
    return (IParallaxTextureSource) this.Instantiate();
  }

  IParallaxTextureSource ISerializationGenerated<IParallaxTextureSource>.Instantiate()
  {
    return (IParallaxTextureSource) this.Instantiate();
  }
}
