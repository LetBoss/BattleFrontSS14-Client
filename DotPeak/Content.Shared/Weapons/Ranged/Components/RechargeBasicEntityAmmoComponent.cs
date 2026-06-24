// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.RechargeBasicEntityAmmoComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class RechargeBasicEntityAmmoComponent : 
  Component,
  ISerializationGenerated<RechargeBasicEntityAmmoComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("rechargeCooldown", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RechargeCooldown;
  [DataField("rechargeSound", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? RechargeSound;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("nextCharge", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NextCharge;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowExamineText;

  public RechargeBasicEntityAmmoComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Magic/forcewall.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVolume(-5f);
    this.RechargeSound = (SoundSpecifier) soundPathSpecifier;
    this.ShowExamineText = true;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RechargeBasicEntityAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RechargeBasicEntityAmmoComponent) target1;
    if (serialization.TryCustomCopy<RechargeBasicEntityAmmoComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RechargeCooldown, ref target2, hookCtx, false, context))
      target2 = this.RechargeCooldown;
    target.RechargeCooldown = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RechargeSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.RechargeSound, hookCtx, context);
    target.RechargeSound = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextCharge, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.NextCharge, hookCtx, context);
    target.NextCharge = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowExamineText, ref target5, hookCtx, false, context))
      target5 = this.ShowExamineText;
    target.ShowExamineText = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RechargeBasicEntityAmmoComponent target,
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
    RechargeBasicEntityAmmoComponent target1 = (RechargeBasicEntityAmmoComponent) target;
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
    RechargeBasicEntityAmmoComponent target1 = (RechargeBasicEntityAmmoComponent) target;
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
    RechargeBasicEntityAmmoComponent target1 = (RechargeBasicEntityAmmoComponent) target;
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
  virtual RechargeBasicEntityAmmoComponent Component.Instantiate()
  {
    return new RechargeBasicEntityAmmoComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RechargeBasicEntityAmmoComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RechargeBasicEntityAmmoComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RechargeBasicEntityAmmoComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.NextCharge.HasValue)
        component.NextCharge = new TimeSpan?(component.NextCharge.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RechargeBasicEntityAmmoComponent_AutoState : IComponentState
  {
    public float RechargeCooldown;
    public 
    #nullable enable
    SoundSpecifier? RechargeSound;
    public TimeSpan? NextCharge;
    public bool ShowExamineText;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RechargeBasicEntityAmmoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, ComponentGetState>(new ComponentEventRefHandler<RechargeBasicEntityAmmoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, ComponentHandleState>(new ComponentEventRefHandler<RechargeBasicEntityAmmoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RechargeBasicEntityAmmoComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RechargeBasicEntityAmmoComponent.RechargeBasicEntityAmmoComponent_AutoState()
      {
        RechargeCooldown = component.RechargeCooldown,
        RechargeSound = component.RechargeSound,
        NextCharge = component.NextCharge,
        ShowExamineText = component.ShowExamineText
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RechargeBasicEntityAmmoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RechargeBasicEntityAmmoComponent.RechargeBasicEntityAmmoComponent_AutoState current))
        return;
      component.RechargeCooldown = current.RechargeCooldown;
      component.RechargeSound = current.RechargeSound;
      component.NextCharge = current.NextCharge;
      component.ShowExamineText = current.ShowExamineText;
    }
  }
}
