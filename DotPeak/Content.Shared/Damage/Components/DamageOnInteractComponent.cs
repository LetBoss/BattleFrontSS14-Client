// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.DamageOnInteractComponent
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
public sealed class DamageOnInteractComponent : 
  Component,
  ISerializationGenerated<DamageOnInteractComponent>,
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
  [DataField(null, false, 1, false, false, null)]
  public bool Throw;
  [DataField(null, false, 1, false, false, null)]
  public int ThrowSpeed = 10;
  [DataField(null, false, 1, false, false, null)]
  public uint InteractTimer;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LastInteraction = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextInteraction = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public float StunChance;
  [DataField(null, false, 1, false, false, null)]
  public float StunSeconds;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageOnInteractComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DamageOnInteractComponent) component;
    if (serialization.TryCustomCopy<DamageOnInteractComponent>(this, ref target, hookCtx, false, context))
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
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Throw, ref flag3, hookCtx, false, context))
      flag3 = this.Throw;
    target.Throw = flag3;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.ThrowSpeed, ref num1, hookCtx, false, context))
      num1 = this.ThrowSpeed;
    target.ThrowSpeed = num1;
    uint num2 = 0;
    if (!serialization.TryCustomCopy<uint>(this.InteractTimer, ref num2, hookCtx, false, context))
      num2 = this.InteractTimer;
    target.InteractTimer = num2;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastInteraction, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.LastInteraction, hookCtx, context, false);
    target.LastInteraction = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextInteraction, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.NextInteraction, hookCtx, context, false);
    target.NextInteraction = timeSpan2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunChance, ref num3, hookCtx, false, context))
      num3 = this.StunChance;
    target.StunChance = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunSeconds, ref num4, hookCtx, false, context))
      num4 = this.StunSeconds;
    target.StunSeconds = num4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageOnInteractComponent target,
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
    DamageOnInteractComponent target1 = (DamageOnInteractComponent) target;
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
    DamageOnInteractComponent target1 = (DamageOnInteractComponent) target;
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
    DamageOnInteractComponent target1 = (DamageOnInteractComponent) target;
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
  virtual DamageOnInteractComponent Component.Instantiate() => new DamageOnInteractComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageOnInteractComponent_AutoState : IComponentState
  {
    public DamageSpecifier Damage;
    public bool IsDamageActive;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageOnInteractComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DamageOnInteractComponent, ComponentGetState>(new ComponentEventRefHandler<DamageOnInteractComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DamageOnInteractComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageOnInteractComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      DamageOnInteractComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DamageOnInteractComponent.DamageOnInteractComponent_AutoState()
      {
        Damage = component.Damage,
        IsDamageActive = component.IsDamageActive
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageOnInteractComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is DamageOnInteractComponent.DamageOnInteractComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.IsDamageActive = current.IsDamageActive;
    }
  }
}
