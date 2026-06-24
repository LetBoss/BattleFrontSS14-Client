// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Visor.ToggleVisorComponent
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
namespace Content.Shared._RMC14.Visor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VisorSystem)})]
public sealed class ToggleVisorComponent : 
  Component,
  ISerializationGenerated<ToggleVisorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IgnoreRedundancy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundOn = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Handling/hud_on.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundOff = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Handling/hud_off.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ToggleVisorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ToggleVisorComponent) target1;
    if (serialization.TryCustomCopy<ToggleVisorComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreRedundancy, ref target2, hookCtx, false, context))
      target2 = this.IgnoreRedundancy;
    target.IgnoreRedundancy = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundOn, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.SoundOn, hookCtx, context);
    target.SoundOn = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundOff, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.SoundOff, hookCtx, context);
    target.SoundOff = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ToggleVisorComponent target,
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
    ToggleVisorComponent target1 = (ToggleVisorComponent) target;
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
    ToggleVisorComponent target1 = (ToggleVisorComponent) target;
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
    ToggleVisorComponent target1 = (ToggleVisorComponent) target;
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
  virtual ToggleVisorComponent Component.Instantiate() => new ToggleVisorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ToggleVisorComponent_AutoState : IComponentState
  {
    public bool IgnoreRedundancy;
    public SoundSpecifier? SoundOn;
    public SoundSpecifier? SoundOff;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ToggleVisorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ToggleVisorComponent, ComponentGetState>(new ComponentEventRefHandler<ToggleVisorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ToggleVisorComponent, ComponentHandleState>(new ComponentEventRefHandler<ToggleVisorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ToggleVisorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ToggleVisorComponent.ToggleVisorComponent_AutoState()
      {
        IgnoreRedundancy = component.IgnoreRedundancy,
        SoundOn = component.SoundOn,
        SoundOff = component.SoundOff
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ToggleVisorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ToggleVisorComponent.ToggleVisorComponent_AutoState current))
        return;
      component.IgnoreRedundancy = current.IgnoreRedundancy;
      component.SoundOn = current.SoundOn;
      component.SoundOff = current.SoundOff;
    }
  }
}
