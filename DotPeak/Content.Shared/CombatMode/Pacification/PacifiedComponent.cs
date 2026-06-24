// Decompiled with JetBrains decompiler
// Type: Content.Shared.CombatMode.Pacification.PacifiedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.CombatMode.Pacification;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (PacificationSystem)})]
public sealed class PacifiedComponent : 
  Component,
  ISerializationGenerated<PacifiedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool DisallowDisarm;
  [DataField(null, false, 1, false, false, null)]
  public bool DisallowAllCombat;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan PopupCooldown = TimeSpan.FromSeconds(3.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan? NextPopupTime;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? LastAttackedEntity;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> PacifiedAlert = ProtoId<AlertPrototype>.op_Implicit("Pacified");

  public virtual bool SendOnlyToOwner => true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PacifiedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PacifiedComponent) component;
    if (serialization.TryCustomCopy<PacifiedComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisallowDisarm, ref flag1, hookCtx, false, context))
      flag1 = this.DisallowDisarm;
    target.DisallowDisarm = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisallowAllCombat, ref flag2, hookCtx, false, context))
      flag2 = this.DisallowAllCombat;
    target.DisallowAllCombat = flag2;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PopupCooldown, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.PopupCooldown, hookCtx, context, false);
    target.PopupCooldown = timeSpan;
    TimeSpan? nullable1 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextPopupTime, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<TimeSpan?>(this.NextPopupTime, hookCtx, context, false);
    target.NextPopupTime = nullable1;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.LastAttackedEntity, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.LastAttackedEntity, hookCtx, context, false);
    target.LastAttackedEntity = nullable2;
    ProtoId<AlertPrototype> protoId = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.PacifiedAlert, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.PacifiedAlert, hookCtx, context, false);
    target.PacifiedAlert = protoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PacifiedComponent target,
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
    PacifiedComponent target1 = (PacifiedComponent) target;
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
    PacifiedComponent target1 = (PacifiedComponent) target;
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
    PacifiedComponent target1 = (PacifiedComponent) target;
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
  virtual PacifiedComponent Component.Instantiate() => new PacifiedComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PacifiedComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<PacifiedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<PacifiedComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      PacifiedComponent component,
      ref EntityUnpausedEvent args)
    {
      if (!component.NextPopupTime.HasValue)
        return;
      component.NextPopupTime = new TimeSpan?(component.NextPopupTime.Value + args.PausedTime);
    }
  }
}
