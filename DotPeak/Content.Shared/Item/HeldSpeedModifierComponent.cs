// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.HeldSpeedModifierComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.Item;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (HeldSpeedModifierSystem)})]
public sealed class HeldSpeedModifierComponent : 
  Component,
  ISerializationGenerated<HeldSpeedModifierComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float WalkModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float SprintModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public bool MirrorClothingModifier = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HeldSpeedModifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HeldSpeedModifierComponent) target1;
    if (serialization.TryCustomCopy<HeldSpeedModifierComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkModifier, ref target2, hookCtx, false, context))
      target2 = this.WalkModifier;
    target.WalkModifier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintModifier, ref target3, hookCtx, false, context))
      target3 = this.SprintModifier;
    target.SprintModifier = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.MirrorClothingModifier, ref target4, hookCtx, false, context))
      target4 = this.MirrorClothingModifier;
    target.MirrorClothingModifier = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HeldSpeedModifierComponent target,
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
    HeldSpeedModifierComponent target1 = (HeldSpeedModifierComponent) target;
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
    HeldSpeedModifierComponent target1 = (HeldSpeedModifierComponent) target;
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
    HeldSpeedModifierComponent target1 = (HeldSpeedModifierComponent) target;
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
  virtual HeldSpeedModifierComponent Component.Instantiate() => new HeldSpeedModifierComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HeldSpeedModifierComponent_AutoState : IComponentState
  {
    public float WalkModifier;
    public float SprintModifier;
    public bool MirrorClothingModifier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HeldSpeedModifierComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HeldSpeedModifierComponent, ComponentGetState>(new ComponentEventRefHandler<HeldSpeedModifierComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HeldSpeedModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<HeldSpeedModifierComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HeldSpeedModifierComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HeldSpeedModifierComponent.HeldSpeedModifierComponent_AutoState()
      {
        WalkModifier = component.WalkModifier,
        SprintModifier = component.SprintModifier,
        MirrorClothingModifier = component.MirrorClothingModifier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HeldSpeedModifierComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HeldSpeedModifierComponent.HeldSpeedModifierComponent_AutoState current))
        return;
      component.WalkModifier = current.WalkModifier;
      component.SprintModifier = current.SprintModifier;
      component.MirrorClothingModifier = current.MirrorClothingModifier;
    }
  }
}
