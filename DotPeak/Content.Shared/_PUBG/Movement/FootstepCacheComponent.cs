// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Movement.FootstepCacheComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Movement;

[RegisterComponent]
[NetworkedComponent]
public sealed class FootstepCacheComponent : 
  Component,
  ISerializationGenerated<FootstepCacheComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Vector2i? LastTile;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? CachedSound;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CacheTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CacheDuration = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? CachedGridUid;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FootstepCacheComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FootstepCacheComponent) target1;
    if (serialization.TryCustomCopy<FootstepCacheComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2i? target2 = new Vector2i?();
    if (!serialization.TryCustomCopy<Vector2i?>(this.LastTile, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i?>(this.LastTile, hookCtx, context);
    target.LastTile = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CachedSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.CachedSound, hookCtx, context);
    target.CachedSound = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CacheTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.CacheTime, hookCtx, context);
    target.CacheTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CacheDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.CacheDuration, hookCtx, context);
    target.CacheDuration = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CachedGridUid, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.CachedGridUid, hookCtx, context);
    target.CachedGridUid = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FootstepCacheComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FootstepCacheComponent target1 = (FootstepCacheComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FootstepCacheComponent target1 = (FootstepCacheComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FootstepCacheComponent target1 = (FootstepCacheComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FootstepCacheComponent Component.Instantiate() => new FootstepCacheComponent();
}
