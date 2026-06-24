// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Components.BlindableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eye.Blinding.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Eye.Blinding.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (BlindableSystem)})]
public sealed class BlindableComponent : 
  Component,
  ISerializationGenerated<BlindableComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("isBlind", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsBlind;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("EyeDamage", false, 1, false, false, null)]
  [AutoNetworkedField]
  public int EyeDamage;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField(null, false, 1, false, false, null)]
  public int MaxDamage = 9;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField(null, false, 1, false, false, null)]
  public int MinDamage;
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public bool LightSetup;
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public bool GraceFrame;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BlindableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BlindableComponent) target1;
    if (serialization.TryCustomCopy<BlindableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsBlind, ref target2, hookCtx, false, context))
      target2 = this.IsBlind;
    target.IsBlind = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.EyeDamage, ref target3, hookCtx, false, context))
      target3 = this.EyeDamage;
    target.EyeDamage = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxDamage, ref target4, hookCtx, false, context))
      target4 = this.MaxDamage;
    target.MaxDamage = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinDamage, ref target5, hookCtx, false, context))
      target5 = this.MinDamage;
    target.MinDamage = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BlindableComponent target,
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
    BlindableComponent target1 = (BlindableComponent) target;
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
    BlindableComponent target1 = (BlindableComponent) target;
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
    BlindableComponent target1 = (BlindableComponent) target;
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
  virtual BlindableComponent Component.Instantiate() => new BlindableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BlindableComponent_AutoState : IComponentState
  {
    public bool IsBlind;
    public int EyeDamage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BlindableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BlindableComponent, ComponentGetState>(new ComponentEventRefHandler<BlindableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BlindableComponent, ComponentHandleState>(new ComponentEventRefHandler<BlindableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BlindableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BlindableComponent.BlindableComponent_AutoState()
      {
        IsBlind = component.IsBlind,
        EyeDamage = component.EyeDamage
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BlindableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BlindableComponent.BlindableComponent_AutoState current))
        return;
      component.IsBlind = current.IsBlind;
      component.EyeDamage = current.EyeDamage;
    }
  }
}
