// Decompiled with JetBrains decompiler
// Type: Content.Shared.CombatMode.CombatModeComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.CombatMode;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedCombatModeSystem)})]
public sealed class CombatModeComponent : 
  Component,
  ISerializationGenerated<CombatModeComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("canDisarm", false, 1, false, false, null)]
  public bool? CanDisarm;
  [DataField("disarmSuccessSound", false, 1, false, false, null)]
  public SoundSpecifier DisarmSuccessSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg", new AudioParams?());
  [DataField("disarmFailChance", false, 1, false, false, null)]
  public float BaseDisarmFailChance = 0.75f;
  [DataField("combatToggleAction", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string CombatToggleAction = "ActionCombatModeToggle";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CombatToggleActionEntity;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("isInCombatMode", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsInCombatMode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ToggleMouseRotator = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CombatModeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CombatModeComponent) component;
    if (serialization.TryCustomCopy<CombatModeComponent>(this, ref target, hookCtx, false, context))
      return;
    bool? nullable1 = new bool?();
    if (!serialization.TryCustomCopy<bool?>(this.CanDisarm, ref nullable1, hookCtx, false, context))
      nullable1 = this.CanDisarm;
    target.CanDisarm = nullable1;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.DisarmSuccessSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DisarmSuccessSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.DisarmSuccessSound, hookCtx, context, false);
    target.DisarmSuccessSound = soundSpecifier;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseDisarmFailChance, ref num, hookCtx, false, context))
      num = this.BaseDisarmFailChance;
    target.BaseDisarmFailChance = num;
    string str = (string) null;
    if (this.CombatToggleAction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CombatToggleAction, ref str, hookCtx, false, context))
      str = this.CombatToggleAction;
    target.CombatToggleAction = str;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CombatToggleActionEntity, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.CombatToggleActionEntity, hookCtx, context, false);
    target.CombatToggleActionEntity = nullable2;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsInCombatMode, ref flag1, hookCtx, false, context))
      flag1 = this.IsInCombatMode;
    target.IsInCombatMode = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ToggleMouseRotator, ref flag2, hookCtx, false, context))
      flag2 = this.ToggleMouseRotator;
    target.ToggleMouseRotator = flag2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CombatModeComponent target,
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
    CombatModeComponent target1 = (CombatModeComponent) target;
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
    CombatModeComponent target1 = (CombatModeComponent) target;
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
    CombatModeComponent target1 = (CombatModeComponent) target;
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
  virtual CombatModeComponent Component.Instantiate() => new CombatModeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CombatModeComponent_AutoState : IComponentState
  {
    public NetEntity? CombatToggleActionEntity;
    public bool IsInCombatMode;
    public bool ToggleMouseRotator;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CombatModeComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CombatModeComponent, ComponentGetState>(new ComponentEventRefHandler<CombatModeComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CombatModeComponent, ComponentHandleState>(new ComponentEventRefHandler<CombatModeComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      CombatModeComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new CombatModeComponent.CombatModeComponent_AutoState()
      {
        CombatToggleActionEntity = this.GetNetEntity(component.CombatToggleActionEntity, (MetaDataComponent) null),
        IsInCombatMode = component.IsInCombatMode,
        ToggleMouseRotator = component.ToggleMouseRotator
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CombatModeComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is CombatModeComponent.CombatModeComponent_AutoState current))
        return;
      component.CombatToggleActionEntity = this.EnsureEntity<CombatModeComponent>(current.CombatToggleActionEntity, uid);
      component.IsInCombatMode = current.IsInCombatMode;
      component.ToggleMouseRotator = current.ToggleMouseRotator;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, CombatModeComponent>(uid, component, ref handleStateEvent);
    }
  }
}
