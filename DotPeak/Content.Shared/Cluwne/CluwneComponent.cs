// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cluwne.CluwneComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cluwne;

[RegisterComponent]
[NetworkedComponent]
public sealed class CluwneComponent : 
  Component,
  ISerializationGenerated<CluwneComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan DamageGiggleCooldown = TimeSpan.FromSeconds(2L);
  [Robust.Shared.ViewVariables.ViewVariables]
  public float KnockChance = 0.05f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float GiggleRandomChance = 0.1f;
  [DataField("emoteId", false, 1, false, false, typeof (PrototypeIdSerializer<EmoteSoundsPrototype>))]
  public string? EmoteSoundsId = "Cluwne";
  [Robust.Shared.ViewVariables.ViewVariables]
  public float ParalyzeTime = 2f;
  [DataField("spawnsound", false, 1, false, false, null)]
  public SoundSpecifier SpawnSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/bikehorn.ogg", new AudioParams?());
  [DataField("knocksound", false, 1, false, false, null)]
  public SoundSpecifier KnockSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/airhorn.ogg", new AudioParams?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CluwneComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CluwneComponent) component;
    if (serialization.TryCustomCopy<CluwneComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.EmoteSoundsId, ref str, hookCtx, false, context))
      str = this.EmoteSoundsId;
    target.EmoteSoundsId = str;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.SpawnSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SpawnSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.SpawnSound, hookCtx, context, false);
    target.SpawnSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (this.KnockSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.KnockSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.KnockSound, hookCtx, context, false);
    target.KnockSound = soundSpecifier2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CluwneComponent target,
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
    CluwneComponent target1 = (CluwneComponent) target;
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
    CluwneComponent target1 = (CluwneComponent) target;
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
    CluwneComponent target1 = (CluwneComponent) target;
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
  virtual CluwneComponent Component.Instantiate() => new CluwneComponent();
}
