// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.LightReplacerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Light.EntitySystems;
using Content.Shared.Storage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedLightReplacerSystem)})]
public sealed class LightReplacerComponent : 
  Component,
  ISerializationGenerated<LightReplacerComponent>,
  ISerializationGenerated
{
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier Sound;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container InsertedBulbs;
  [DataField("contents", false, 1, false, false, null)]
  public List<EntitySpawnEntry> Contents;

  public LightReplacerComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Weapons/click.ogg");
    soundPathSpecifier.Params = new AudioParams()
    {
      Volume = -4f
    };
    this.Sound = (SoundSpecifier) soundPathSpecifier;
    this.Contents = new List<EntitySpawnEntry>();
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LightReplacerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LightReplacerComponent) target1;
    if (serialization.TryCustomCopy<LightReplacerComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target2;
    List<EntitySpawnEntry> target3 = (List<EntitySpawnEntry>) null;
    if (this.Contents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.Contents, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.Contents, hookCtx, context);
    target.Contents = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LightReplacerComponent target,
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
    LightReplacerComponent target1 = (LightReplacerComponent) target;
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
    LightReplacerComponent target1 = (LightReplacerComponent) target;
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
    LightReplacerComponent target1 = (LightReplacerComponent) target;
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
  virtual LightReplacerComponent Component.Instantiate() => new LightReplacerComponent();
}
