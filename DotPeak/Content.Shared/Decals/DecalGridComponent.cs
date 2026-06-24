// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.DecalGridComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Content.Shared.Decals;

[RegisterComponent]
[Access(new Type[] {typeof (SharedDecalSystem)})]
[NetworkedComponent]
public sealed class DecalGridComponent : 
  Component,
  ISerializationGenerated<DecalGridComponent>,
  ISerializationGenerated
{
  [Access(new Type[] {})]
  [DataField(null, false, 1, false, true, null)]
  public DecalGridComponent.DecalGridChunkCollection ChunkCollection = new DecalGridComponent.DecalGridChunkCollection(new Dictionary<Vector2i, DecalGridComponent.DecalChunk>());
  public readonly Dictionary<uint, Vector2i> DecalIndex = new Dictionary<uint, Vector2i>();

  public GameTick ForceTick { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DecalGridComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DecalGridComponent) component;
    if (serialization.TryCustomCopy<DecalGridComponent>(this, ref target, hookCtx, false, context))
      return;
    DecalGridComponent.DecalGridChunkCollection gridChunkCollection = (DecalGridComponent.DecalGridChunkCollection) null;
    if (this.ChunkCollection == (DecalGridComponent.DecalGridChunkCollection) null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DecalGridComponent.DecalGridChunkCollection>(this.ChunkCollection, ref gridChunkCollection, hookCtx, true, context))
      gridChunkCollection = serialization.CreateCopy<DecalGridComponent.DecalGridChunkCollection>(this.ChunkCollection, hookCtx, context, false);
    target.ChunkCollection = gridChunkCollection;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DecalGridComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DecalGridComponent target1 = (DecalGridComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DecalGridComponent target1 = (DecalGridComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DecalGridComponent target1 = (DecalGridComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DecalGridComponent Component.Instantiate() => new DecalGridComponent();

  [DataDefinition]
  [NetSerializable]
  [Serializable]
  public sealed class DecalChunk : 
    ISerializationGenerated<DecalGridComponent.DecalChunk>,
    ISerializationGenerated
  {
    [IncludeDataField(false, 1, false, typeof (DictionarySerializer<uint, Decal>))]
    public Dictionary<uint, Decal> Decals;
    [NonSerialized]
    public GameTick LastModified;

    public DecalChunk() => this.Decals = new Dictionary<uint, Decal>();

    public DecalChunk(Dictionary<uint, Decal> decals) => this.Decals = decals;

    public DecalChunk(DecalGridComponent.DecalChunk chunk)
    {
      this.Decals = Extensions.ShallowClone<uint, Decal>(chunk.Decals);
      this.LastModified = chunk.LastModified;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref DecalGridComponent.DecalChunk target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<DecalGridComponent.DecalChunk>(this, ref target, hookCtx, false, context))
        return;
      if (this.Decals == null)
        throw new NullNotAllowedException();
      Dictionary<uint, Decal> dictionary1 = (Dictionary<uint, Decal>) null;
      serialization.CopyTo<Dictionary<uint, Decal>, DictionarySerializer<uint, Decal>>(this.Decals, ref dictionary1, hookCtx, context, true);
      Dictionary<uint, Decal> dictionary2 = dictionary1;
      target.Decals = dictionary2;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref DecalGridComponent.DecalChunk target,
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
      DecalGridComponent.DecalChunk target1 = (DecalGridComponent.DecalChunk) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public DecalGridComponent.DecalChunk Instantiate() => new DecalGridComponent.DecalChunk();
  }

  [DataRecord]
  [NetSerializable]
  [Serializable]
  public record DecalGridChunkCollection(
    Dictionary<Vector2i, DecalGridComponent.DecalChunk> ChunkCollection)
  {
    public uint NextDecalId;

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("ChunkCollection = ");
      builder.Append((object) this.ChunkCollection);
      builder.Append(", NextDecalId = ");
      builder.Append(this.NextDecalId.ToString());
      return true;
    }
  }
}
