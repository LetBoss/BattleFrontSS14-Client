// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wieldable.Components.WieldableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Wieldable.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedWieldableSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class WieldableComponent : 
  Component,
  ISerializationGenerated<WieldableComponent>,
  ISerializationGenerated
{
  [DataField("wieldSound", false, 1, false, false, null)]
  public SoundSpecifier? WieldSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg");
  [DataField("unwieldSound", false, 1, false, false, null)]
  public SoundSpecifier? UnwieldSound;
  [DataField("freeHandsRequired", false, 1, false, false, null)]
  public int FreeHandsRequired = 1;
  [AutoNetworkedField]
  [DataField("wielded", false, 1, false, false, null)]
  public bool Wielded;
  [DataField(null, false, 1, false, false, null)]
  public bool UnwieldOnUse = true;
  [DataField(null, false, 1, false, false, null)]
  public bool UseDelayOnWield = true;
  [DataField("wieldedInhandPrefix", false, 1, false, false, null)]
  public string? WieldedInhandPrefix = "wielded";
  public string? OldInhandPrefix;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WieldableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WieldableComponent) target1;
    if (serialization.TryCustomCopy<WieldableComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WieldSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.WieldSound, hookCtx, context);
    target.WieldSound = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnwieldSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.UnwieldSound, hookCtx, context);
    target.UnwieldSound = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.FreeHandsRequired, ref target4, hookCtx, false, context))
      target4 = this.FreeHandsRequired;
    target.FreeHandsRequired = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Wielded, ref target5, hookCtx, false, context))
      target5 = this.Wielded;
    target.Wielded = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.UnwieldOnUse, ref target6, hookCtx, false, context))
      target6 = this.UnwieldOnUse;
    target.UnwieldOnUse = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseDelayOnWield, ref target7, hookCtx, false, context))
      target7 = this.UseDelayOnWield;
    target.UseDelayOnWield = target7;
    string target8 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.WieldedInhandPrefix, ref target8, hookCtx, false, context))
      target8 = this.WieldedInhandPrefix;
    target.WieldedInhandPrefix = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WieldableComponent target,
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
    WieldableComponent target1 = (WieldableComponent) target;
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
    WieldableComponent target1 = (WieldableComponent) target;
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
    WieldableComponent target1 = (WieldableComponent) target;
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
  virtual WieldableComponent Component.Instantiate() => new WieldableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WieldableComponent_AutoState : IComponentState
  {
    public bool Wielded;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WieldableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WieldableComponent, ComponentGetState>(new ComponentEventRefHandler<WieldableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WieldableComponent, ComponentHandleState>(new ComponentEventRefHandler<WieldableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WieldableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new WieldableComponent.WieldableComponent_AutoState()
      {
        Wielded = component.Wielded
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WieldableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WieldableComponent.WieldableComponent_AutoState current))
        return;
      component.Wielded = current.Wielded;
    }
  }
}
