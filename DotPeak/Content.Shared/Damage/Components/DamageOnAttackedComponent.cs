// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.DamageOnAttackedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class DamageOnAttackedComponent : 
  Component,
  ISerializationGenerated<DamageOnAttackedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreResistances;
  [DataField(null, false, 1, false, false, null)]
  public LocId? PopupText;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier InteractSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/lightburn.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsDamageActive = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageOnAttackedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DamageOnAttackedComponent) component;
    if (serialization.TryCustomCopy<DamageOnAttackedComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier damageSpecifier = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref damageSpecifier, hookCtx, false, context))
    {
      if (this.Damage == null)
        damageSpecifier = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref damageSpecifier, hookCtx, context, true);
    }
    target.Damage = damageSpecifier;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreResistances, ref flag1, hookCtx, false, context))
      flag1 = this.IgnoreResistances;
    target.IgnoreResistances = flag1;
    LocId? nullable = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.PopupText, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<LocId?>(this.PopupText, hookCtx, context, false);
    target.PopupText = nullable;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.InteractSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InteractSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.InteractSound, hookCtx, context, false);
    target.InteractSound = soundSpecifier;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsDamageActive, ref flag2, hookCtx, false, context))
      flag2 = this.IsDamageActive;
    target.IsDamageActive = flag2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageOnAttackedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageOnAttackedComponent target1 = (DamageOnAttackedComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageOnAttackedComponent target1 = (DamageOnAttackedComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageOnAttackedComponent target1 = (DamageOnAttackedComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DamageOnAttackedComponent Component.Instantiate() => new DamageOnAttackedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageOnAttackedComponent_AutoState : IComponentState
  {
    public DamageSpecifier Damage;
    public bool IsDamageActive;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageOnAttackedComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DamageOnAttackedComponent, ComponentGetState>(new ComponentEventRefHandler<DamageOnAttackedComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DamageOnAttackedComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageOnAttackedComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      DamageOnAttackedComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DamageOnAttackedComponent.DamageOnAttackedComponent_AutoState()
      {
        Damage = component.Damage,
        IsDamageActive = component.IsDamageActive
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageOnAttackedComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is DamageOnAttackedComponent.DamageOnAttackedComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.IsDamageActive = current.IsDamageActive;
    }
  }
}
