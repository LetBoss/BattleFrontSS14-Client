// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mortar.MortarCameraShellComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Mortar;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedMortarSystem)})]
public sealed class MortarCameraShellComponent : 
  Component,
  ISerializationGenerated<MortarCameraShellComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromMinutes(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/flaregun.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Flare = (EntProtoId) "RMCMortarFlare";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Camera = (EntProtoId) "RMCMortarCamera";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MortarCameraShellComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MortarCameraShellComponent) target1;
    if (serialization.TryCustomCopy<MortarCameraShellComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Flare, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Flare, hookCtx, context);
    target.Flare = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Camera, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.Camera, hookCtx, context);
    target.Camera = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MortarCameraShellComponent target,
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
    MortarCameraShellComponent target1 = (MortarCameraShellComponent) target;
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
    MortarCameraShellComponent target1 = (MortarCameraShellComponent) target;
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
    MortarCameraShellComponent target1 = (MortarCameraShellComponent) target;
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
  virtual MortarCameraShellComponent Component.Instantiate() => new MortarCameraShellComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MortarCameraShellComponent_AutoState : IComponentState
  {
    public TimeSpan Duration;
    public SoundSpecifier? Sound;
    public EntProtoId Flare;
    public EntProtoId Camera;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MortarCameraShellComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MortarCameraShellComponent, ComponentGetState>(new ComponentEventRefHandler<MortarCameraShellComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MortarCameraShellComponent, ComponentHandleState>(new ComponentEventRefHandler<MortarCameraShellComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MortarCameraShellComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MortarCameraShellComponent.MortarCameraShellComponent_AutoState()
      {
        Duration = component.Duration,
        Sound = component.Sound,
        Flare = component.Flare,
        Camera = component.Camera
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MortarCameraShellComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MortarCameraShellComponent.MortarCameraShellComponent_AutoState current))
        return;
      component.Duration = current.Duration;
      component.Sound = current.Sound;
      component.Flare = current.Flare;
      component.Camera = current.Camera;
    }
  }
}
