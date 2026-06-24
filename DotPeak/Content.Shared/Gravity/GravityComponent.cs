// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gravity.GravityComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Gravity;

[RegisterComponent]
[NetworkedComponent]
public sealed class GravityComponent : 
  Component,
  ISerializationGenerated<GravityComponent>,
  ISerializationGenerated
{
  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("inherent", false, 1, false, false, null)]
  public bool Inherent;

  [DataField("gravityShakeSound", false, 1, false, false, null)]
  public SoundSpecifier GravityShakeSound { get; set; } = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/alert.ogg");

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool EnabledVV
  {
    get => this.Enabled;
    set
    {
      if (this.Enabled == value)
        return;
      this.Enabled = value;
      IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
      GravityChangedEvent args = new GravityChangedEvent(this.Owner, value);
      entityManager.EventBus.RaiseLocalEvent<GravityChangedEvent>(this.Owner, ref args);
      entityManager.Dirty(this.Owner, (IComponent) this);
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GravityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GravityComponent) target1;
    if (serialization.TryCustomCopy<GravityComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.GravityShakeSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GravityShakeSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.GravityShakeSound, hookCtx, context);
    target.GravityShakeSound = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target3, hookCtx, false, context))
      target3 = this.Enabled;
    target.Enabled = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Inherent, ref target4, hookCtx, false, context))
      target4 = this.Inherent;
    target.Inherent = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GravityComponent target,
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
    GravityComponent target1 = (GravityComponent) target;
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
    GravityComponent target1 = (GravityComponent) target;
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
    GravityComponent target1 = (GravityComponent) target;
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
  virtual GravityComponent Component.Instantiate() => new GravityComponent();
}
