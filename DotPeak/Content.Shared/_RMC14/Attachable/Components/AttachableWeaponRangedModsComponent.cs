// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Components.AttachableWeaponRangedModsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AttachableModifiersSystem)})]
public sealed class AttachableWeaponRangedModsComponent : 
  Component,
  ISerializationGenerated<AttachableWeaponRangedModsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<AttachableWeaponRangedModifierSet> Modifiers = new List<AttachableWeaponRangedModifierSet>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<AttachableWeaponFireModesModifierSet>? FireModeMods;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableWeaponRangedModsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AttachableWeaponRangedModsComponent) target1;
    if (serialization.TryCustomCopy<AttachableWeaponRangedModsComponent>(this, ref target, hookCtx, false, context))
      return;
    List<AttachableWeaponRangedModifierSet> target2 = (List<AttachableWeaponRangedModifierSet>) null;
    if (this.Modifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<AttachableWeaponRangedModifierSet>>(this.Modifiers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<AttachableWeaponRangedModifierSet>>(this.Modifiers, hookCtx, context);
    target.Modifiers = target2;
    List<AttachableWeaponFireModesModifierSet> target3 = (List<AttachableWeaponFireModesModifierSet>) null;
    if (!serialization.TryCustomCopy<List<AttachableWeaponFireModesModifierSet>>(this.FireModeMods, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<AttachableWeaponFireModesModifierSet>>(this.FireModeMods, hookCtx, context);
    target.FireModeMods = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableWeaponRangedModsComponent target,
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
    AttachableWeaponRangedModsComponent target1 = (AttachableWeaponRangedModsComponent) target;
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
    AttachableWeaponRangedModsComponent target1 = (AttachableWeaponRangedModsComponent) target;
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
    AttachableWeaponRangedModsComponent target1 = (AttachableWeaponRangedModsComponent) target;
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
  virtual AttachableWeaponRangedModsComponent Component.Instantiate()
  {
    return new AttachableWeaponRangedModsComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AttachableWeaponRangedModsComponent_AutoState : IComponentState
  {
    public List<AttachableWeaponRangedModifierSet> Modifiers;
    public List<AttachableWeaponFireModesModifierSet>? FireModeMods;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AttachableWeaponRangedModsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, ComponentGetState>(new ComponentEventRefHandler<AttachableWeaponRangedModsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, ComponentHandleState>(new ComponentEventRefHandler<AttachableWeaponRangedModsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AttachableWeaponRangedModsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AttachableWeaponRangedModsComponent.AttachableWeaponRangedModsComponent_AutoState()
      {
        Modifiers = component.Modifiers,
        FireModeMods = component.FireModeMods
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AttachableWeaponRangedModsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AttachableWeaponRangedModsComponent.AttachableWeaponRangedModsComponent_AutoState current))
        return;
      component.Modifiers = current.Modifiers == null ? (List<AttachableWeaponRangedModifierSet>) null : new List<AttachableWeaponRangedModifierSet>((IEnumerable<AttachableWeaponRangedModifierSet>) current.Modifiers);
      component.FireModeMods = current.FireModeMods == null ? (List<AttachableWeaponFireModesModifierSet>) null : new List<AttachableWeaponFireModesModifierSet>((IEnumerable<AttachableWeaponFireModesModifierSet>) current.FireModeMods);
    }
  }
}
