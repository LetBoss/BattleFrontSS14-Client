// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.Components.ItemToggleComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Item.ItemToggle.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ItemToggleComponent : 
  Component,
  ISerializationGenerated<ItemToggleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Activated;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnActivate = true;
  [DataField(null, false, 1, false, false, null)]
  public bool OnUse = true;
  [DataField(null, false, 1, false, false, null)]
  public string VerbToggleOn = "item-toggle-activate";
  [DataField(null, false, 1, false, false, null)]
  public string VerbToggleOff = "item-toggle-deactivate";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Predictable = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundActivate;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundDeactivate;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundFailToActivate;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemToggleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemToggleComponent) target1;
    if (serialization.TryCustomCopy<ItemToggleComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Activated, ref target2, hookCtx, false, context))
      target2 = this.Activated;
    target.Activated = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnActivate, ref target3, hookCtx, false, context))
      target3 = this.OnActivate;
    target.OnActivate = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnUse, ref target4, hookCtx, false, context))
      target4 = this.OnUse;
    target.OnUse = target4;
    string target5 = (string) null;
    if (this.VerbToggleOn == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.VerbToggleOn, ref target5, hookCtx, false, context))
      target5 = this.VerbToggleOn;
    target.VerbToggleOn = target5;
    string target6 = (string) null;
    if (this.VerbToggleOff == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.VerbToggleOff, ref target6, hookCtx, false, context))
      target6 = this.VerbToggleOff;
    target.VerbToggleOff = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Predictable, ref target7, hookCtx, false, context))
      target7 = this.Predictable;
    target.Predictable = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundActivate, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.SoundActivate, hookCtx, context);
    target.SoundActivate = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundDeactivate, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.SoundDeactivate, hookCtx, context);
    target.SoundDeactivate = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundFailToActivate, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.SoundFailToActivate, hookCtx, context);
    target.SoundFailToActivate = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemToggleComponent target,
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
    ItemToggleComponent target1 = (ItemToggleComponent) target;
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
    ItemToggleComponent target1 = (ItemToggleComponent) target;
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
    ItemToggleComponent target1 = (ItemToggleComponent) target;
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
  virtual ItemToggleComponent Component.Instantiate() => new ItemToggleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemToggleComponent_AutoState : IComponentState
  {
    public bool Activated;
    public bool OnActivate;
    public bool Predictable;
    public SoundSpecifier? SoundActivate;
    public SoundSpecifier? SoundDeactivate;
    public SoundSpecifier? SoundFailToActivate;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemToggleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemToggleComponent, ComponentGetState>(new ComponentEventRefHandler<ItemToggleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemToggleComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemToggleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ItemToggleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemToggleComponent.ItemToggleComponent_AutoState()
      {
        Activated = component.Activated,
        OnActivate = component.OnActivate,
        Predictable = component.Predictable,
        SoundActivate = component.SoundActivate,
        SoundDeactivate = component.SoundDeactivate,
        SoundFailToActivate = component.SoundFailToActivate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemToggleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemToggleComponent.ItemToggleComponent_AutoState current))
        return;
      component.Activated = current.Activated;
      component.OnActivate = current.OnActivate;
      component.Predictable = current.Predictable;
      component.SoundActivate = current.SoundActivate;
      component.SoundDeactivate = current.SoundDeactivate;
      component.SoundFailToActivate = current.SoundFailToActivate;
    }
  }
}
