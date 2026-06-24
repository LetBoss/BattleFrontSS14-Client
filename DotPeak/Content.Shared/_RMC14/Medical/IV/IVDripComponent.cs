// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.IV.IVDripComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.IV;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
public sealed class IVDripComponent : 
  Component,
  ISerializationGenerated<IVDripComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? AttachedTo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Slot = "pack";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 TransferAmount = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TransferDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan TransferAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AttachedState = "hooked";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string UnattachedState = "unhooked";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<(int Percentage, string State)> ReagentStates = new List<(int, string)>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color FillColor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FillPercentage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 2;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier? RipDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Injecting = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> RipEmote = (ProtoId<EmotePrototype>) "Scream";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> SkillRequired = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>()
  {
    [(EntProtoId<SkillDefinitionComponent>) "RMCSkillSurgery"] = 1
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IVDripComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IVDripComponent) target1;
    if (serialization.TryCustomCopy<IVDripComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AttachedTo, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.AttachedTo, hookCtx, context);
    target.AttachedTo = target2;
    string target3 = (string) null;
    if (this.Slot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Slot, ref target3, hookCtx, false, context))
      target3 = this.Slot;
    target.Slot = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferAmount, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.TransferAmount, hookCtx, context);
    target.TransferAmount = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TransferDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.TransferDelay, hookCtx, context);
    target.TransferDelay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TransferAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.TransferAt, hookCtx, context);
    target.TransferAt = target6;
    string target7 = (string) null;
    if (this.AttachedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AttachedState, ref target7, hookCtx, false, context))
      target7 = this.AttachedState;
    target.AttachedState = target7;
    string target8 = (string) null;
    if (this.UnattachedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UnattachedState, ref target8, hookCtx, false, context))
      target8 = this.UnattachedState;
    target.UnattachedState = target8;
    List<(int, string)> target9 = (List<(int, string)>) null;
    if (this.ReagentStates == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<(int, string)>>(this.ReagentStates, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<List<(int, string)>>(this.ReagentStates, hookCtx, context);
    target.ReagentStates = target9;
    Color target10 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.FillColor, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Color>(this.FillColor, hookCtx, context);
    target.FillColor = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.FillPercentage, ref target11, hookCtx, false, context))
      target11 = this.FillPercentage;
    target.FillPercentage = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target12, hookCtx, false, context))
      target12 = this.Range;
    target.Range = target12;
    DamageSpecifier target13 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.RipDamage, ref target13, hookCtx, false, context))
    {
      if (this.RipDamage == null)
        target13 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.RipDamage, ref target13, hookCtx, context);
    }
    target.RipDamage = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.Injecting, ref target14, hookCtx, false, context))
      target14 = this.Injecting;
    target.Injecting = target14;
    ProtoId<EmotePrototype> target15 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.RipEmote, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.RipEmote, hookCtx, context);
    target.RipEmote = target15;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target16 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.SkillRequired == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.SkillRequired, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.SkillRequired, hookCtx, context);
    target.SkillRequired = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IVDripComponent target,
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
    IVDripComponent target1 = (IVDripComponent) target;
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
    IVDripComponent target1 = (IVDripComponent) target;
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
    IVDripComponent target1 = (IVDripComponent) target;
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
  virtual IVDripComponent Component.Instantiate() => new IVDripComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IVDripComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IVDripComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<IVDripComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      IVDripComponent component,
      ref EntityUnpausedEvent args)
    {
      component.TransferAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IVDripComponent_AutoState : IComponentState
  {
    public NetEntity? AttachedTo;
    public 
    #nullable enable
    string Slot;
    public FixedPoint2 TransferAmount;
    public TimeSpan TransferDelay;
    public TimeSpan TransferAt;
    public string AttachedState;
    public string UnattachedState;
    public List<(int Percentage, string State)> ReagentStates;
    public Color FillColor;
    public int FillPercentage;
    public int Range;
    public bool Injecting;
    public ProtoId<EmotePrototype> RipEmote;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> SkillRequired;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IVDripComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IVDripComponent, ComponentGetState>(new ComponentEventRefHandler<IVDripComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IVDripComponent, ComponentHandleState>(new ComponentEventRefHandler<IVDripComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, IVDripComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new IVDripComponent.IVDripComponent_AutoState()
      {
        AttachedTo = this.GetNetEntity(component.AttachedTo),
        Slot = component.Slot,
        TransferAmount = component.TransferAmount,
        TransferDelay = component.TransferDelay,
        TransferAt = component.TransferAt,
        AttachedState = component.AttachedState,
        UnattachedState = component.UnattachedState,
        ReagentStates = component.ReagentStates,
        FillColor = component.FillColor,
        FillPercentage = component.FillPercentage,
        Range = component.Range,
        Injecting = component.Injecting,
        RipEmote = component.RipEmote,
        SkillRequired = component.SkillRequired
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IVDripComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IVDripComponent.IVDripComponent_AutoState current))
        return;
      component.AttachedTo = this.EnsureEntity<IVDripComponent>(current.AttachedTo, uid);
      component.Slot = current.Slot;
      component.TransferAmount = current.TransferAmount;
      component.TransferDelay = current.TransferDelay;
      component.TransferAt = current.TransferAt;
      component.AttachedState = current.AttachedState;
      component.UnattachedState = current.UnattachedState;
      component.ReagentStates = current.ReagentStates == null ? (List<(int, string)>) null : new List<(int, string)>((IEnumerable<(int, string)>) current.ReagentStates);
      component.FillColor = current.FillColor;
      component.FillPercentage = current.FillPercentage;
      component.Range = current.Range;
      component.Injecting = current.Injecting;
      component.RipEmote = current.RipEmote;
      component.SkillRequired = current.SkillRequired == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.SkillRequired);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, IVDripComponent>(uid, component, ref args1);
    }
  }
}
