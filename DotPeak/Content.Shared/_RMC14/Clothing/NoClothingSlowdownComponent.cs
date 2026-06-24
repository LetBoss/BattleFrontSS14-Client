// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Clothing.NoClothingSlowdownComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Clothing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCClothingSystem)})]
public sealed class NoClothingSlowdownComponent : 
  Component,
  ISerializationGenerated<NoClothingSlowdownComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Slot = "shoes";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WalkModifier = 0.62f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SprintModifier = 0.62f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NoClothingSlowdownComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NoClothingSlowdownComponent) target1;
    if (serialization.TryCustomCopy<NoClothingSlowdownComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Slot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Slot, ref target2, hookCtx, false, context))
      target2 = this.Slot;
    target.Slot = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkModifier, ref target3, hookCtx, false, context))
      target3 = this.WalkModifier;
    target.WalkModifier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintModifier, ref target4, hookCtx, false, context))
      target4 = this.SprintModifier;
    target.SprintModifier = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target5, hookCtx, false, context))
      target5 = this.Active;
    target.Active = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NoClothingSlowdownComponent target,
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
    NoClothingSlowdownComponent target1 = (NoClothingSlowdownComponent) target;
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
    NoClothingSlowdownComponent target1 = (NoClothingSlowdownComponent) target;
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
    NoClothingSlowdownComponent target1 = (NoClothingSlowdownComponent) target;
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
  virtual NoClothingSlowdownComponent Component.Instantiate() => new NoClothingSlowdownComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NoClothingSlowdownComponent_AutoState : IComponentState
  {
    public string Slot;
    public float WalkModifier;
    public float SprintModifier;
    public bool Active;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NoClothingSlowdownComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NoClothingSlowdownComponent, ComponentGetState>(new ComponentEventRefHandler<NoClothingSlowdownComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NoClothingSlowdownComponent, ComponentHandleState>(new ComponentEventRefHandler<NoClothingSlowdownComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NoClothingSlowdownComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NoClothingSlowdownComponent.NoClothingSlowdownComponent_AutoState()
      {
        Slot = component.Slot,
        WalkModifier = component.WalkModifier,
        SprintModifier = component.SprintModifier,
        Active = component.Active
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NoClothingSlowdownComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NoClothingSlowdownComponent.NoClothingSlowdownComponent_AutoState current))
        return;
      component.Slot = current.Slot;
      component.WalkModifier = current.WalkModifier;
      component.SprintModifier = current.SprintModifier;
      component.Active = current.Active;
    }
  }
}
