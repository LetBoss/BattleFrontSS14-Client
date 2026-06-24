// Decompiled with JetBrains decompiler
// Type: Content.Shared.Audio.AmbientSoundComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Audio;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedAmbientSoundSystem)})]
public sealed class AmbientSoundComponent : 
  Component,
  IComponentTreeEntry<AmbientSoundComponent>,
  ISerializationGenerated<AmbientSoundComponent>,
  ISerializationGenerated
{
  [DataField("sound", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier Sound;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("range", false, 1, false, false, null)]
  public float Range = 2f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("volume", false, 1, false, false, null)]
  public float Volume = -10f;

  [DataField("enabled", true, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Enabled { get; set; } = true;

  public Vector2 RangeVector => new Vector2(this.Range, this.Range);

  public EntityUid? TreeUid { get; set; }

  public DynamicTree<ComponentTreeEntry<AmbientSoundComponent>>? Tree { get; set; }

  public bool AddToTree => this.Enabled;

  public bool TreeUpdateQueued { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AmbientSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AmbientSoundComponent) component;
    if (serialization.TryCustomCopy<AmbientSoundComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag, hookCtx, false, context))
      flag = this.Enabled;
    target.Enabled = flag;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context, false);
    target.Sound = soundSpecifier;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref num1, hookCtx, false, context))
      num1 = this.Range;
    target.Range = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Volume, ref num2, hookCtx, false, context))
      num2 = this.Volume;
    target.Volume = num2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AmbientSoundComponent target,
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
    AmbientSoundComponent target1 = (AmbientSoundComponent) target;
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
    AmbientSoundComponent target1 = (AmbientSoundComponent) target;
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
    AmbientSoundComponent target1 = (AmbientSoundComponent) target;
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
  virtual AmbientSoundComponent Component.Instantiate() => new AmbientSoundComponent();
}
