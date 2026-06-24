// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Data.IParallaxTextureSource
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Parallax.Data;

[ImplicitDataDefinitionForInheritors]
public interface IParallaxTextureSource : 
  ISerializationGenerated<IParallaxTextureSource>,
  ISerializationGenerated
{
  Task<Texture> GenerateTexture(CancellationToken cancel = default (CancellationToken));

  void Unload(IDependencyCollection dependencies)
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  void InternalCopy(
    ref IParallaxTextureSource target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<IParallaxTextureSource>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  void Copy(
    ref IParallaxTextureSource target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IParallaxTextureSource target1 = (IParallaxTextureSource) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  IParallaxTextureSource Instantiate() => throw new NotImplementedException();
}
