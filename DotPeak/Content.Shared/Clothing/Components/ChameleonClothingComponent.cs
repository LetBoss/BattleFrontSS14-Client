// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.ChameleonClothingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedChameleonClothingSystem)})]
public sealed class ChameleonClothingComponent : 
  Component,
  ISerializationGenerated<ChameleonClothingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public SlotFlags Slot;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId? Default;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? User;
  [DataField(null, false, 1, false, false, null)]
  public string? RequireTag;
  [DataField(null, false, 1, false, false, null)]
  public bool AffectedByEmp = true;
  [DataField(null, false, 1, false, false, null)]
  public int EmpChangeIntensity = 7;
  [DataField(null, false, 1, false, false, null)]
  public bool EmpContinuous = true;
  [AutoPausedField]
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextEmpChange = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChameleonClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ChameleonClothingComponent) component;
    if (serialization.TryCustomCopy<ChameleonClothingComponent>(this, ref target, hookCtx, false, context))
      return;
    SlotFlags slotFlags = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slot, ref slotFlags, hookCtx, false, context))
      slotFlags = this.Slot;
    target.Slot = slotFlags;
    EntProtoId? nullable = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Default, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntProtoId?>(this.Default, hookCtx, context, false);
    target.Default = nullable;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.RequireTag, ref str, hookCtx, false, context))
      str = this.RequireTag;
    target.RequireTag = str;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.AffectedByEmp, ref flag1, hookCtx, false, context))
      flag1 = this.AffectedByEmp;
    target.AffectedByEmp = flag1;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.EmpChangeIntensity, ref num, hookCtx, false, context))
      num = this.EmpChangeIntensity;
    target.EmpChangeIntensity = num;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.EmpContinuous, ref flag2, hookCtx, false, context))
      flag2 = this.EmpContinuous;
    target.EmpContinuous = flag2;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextEmpChange, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.NextEmpChange, hookCtx, context, false);
    target.NextEmpChange = timeSpan;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChameleonClothingComponent target,
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
    ChameleonClothingComponent target1 = (ChameleonClothingComponent) target;
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
    ChameleonClothingComponent target1 = (ChameleonClothingComponent) target;
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
    ChameleonClothingComponent target1 = (ChameleonClothingComponent) target;
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
  virtual ChameleonClothingComponent Component.Instantiate() => new ChameleonClothingComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ChameleonClothingComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ChameleonClothingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ChameleonClothingComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ChameleonClothingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextEmpChange += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ChameleonClothingComponent_AutoState : IComponentState
  {
    public EntProtoId? Default;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ChameleonClothingComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ChameleonClothingComponent, ComponentGetState>(new ComponentEventRefHandler<ChameleonClothingComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ChameleonClothingComponent, ComponentHandleState>(new ComponentEventRefHandler<ChameleonClothingComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      ChameleonClothingComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ChameleonClothingComponent.ChameleonClothingComponent_AutoState()
      {
        Default = component.Default
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ChameleonClothingComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ChameleonClothingComponent.ChameleonClothingComponent_AutoState current))
        return;
      component.Default = current.Default;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, ChameleonClothingComponent>(uid, component, ref handleStateEvent);
    }
  }
}
