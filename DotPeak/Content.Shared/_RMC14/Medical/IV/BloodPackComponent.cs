// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.IV.BloodPackComponent
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
public sealed class BloodPackComponent : 
  Component,
  ISerializationGenerated<BloodPackComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Solution = "pack";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 FillPercentage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color FillColor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxFillLevels = 7;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string FillBaseName = "bloodpack";
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
  public EntityUid? AttachedTo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AttachDelay = TimeSpan.FromSeconds(1L);
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
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string[] TransferableReagents = new string[1]
  {
    "Blood"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BloodPackComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BloodPackComponent) target1;
    if (serialization.TryCustomCopy<BloodPackComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target2, hookCtx, false, context))
      target2 = this.Solution;
    target.Solution = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.FillPercentage, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.FillPercentage, hookCtx, context);
    target.FillPercentage = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.FillColor, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.FillColor, hookCtx, context);
    target.FillColor = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxFillLevels, ref target5, hookCtx, false, context))
      target5 = this.MaxFillLevels;
    target.MaxFillLevels = target5;
    string target6 = (string) null;
    if (this.FillBaseName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FillBaseName, ref target6, hookCtx, false, context))
      target6 = this.FillBaseName;
    target.FillBaseName = target6;
    FixedPoint2 target7 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferAmount, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<FixedPoint2>(this.TransferAmount, hookCtx, context);
    target.TransferAmount = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TransferDelay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.TransferDelay, hookCtx, context);
    target.TransferDelay = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TransferAt, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.TransferAt, hookCtx, context);
    target.TransferAt = target9;
    EntityUid? target10 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AttachedTo, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntityUid?>(this.AttachedTo, hookCtx, context);
    target.AttachedTo = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AttachDelay, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.AttachDelay, hookCtx, context);
    target.AttachDelay = target11;
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
    string[] target17 = (string[]) null;
    if (this.TransferableReagents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.TransferableReagents, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<string[]>(this.TransferableReagents, hookCtx, context);
    target.TransferableReagents = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BloodPackComponent target,
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
    BloodPackComponent target1 = (BloodPackComponent) target;
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
    BloodPackComponent target1 = (BloodPackComponent) target;
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
    BloodPackComponent target1 = (BloodPackComponent) target;
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
  virtual BloodPackComponent Component.Instantiate() => new BloodPackComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BloodPackComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BloodPackComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<BloodPackComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      BloodPackComponent component,
      ref EntityUnpausedEvent args)
    {
      component.TransferAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BloodPackComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string Solution;
    public FixedPoint2 FillPercentage;
    public Color FillColor;
    public int MaxFillLevels;
    public string FillBaseName;
    public FixedPoint2 TransferAmount;
    public TimeSpan TransferDelay;
    public TimeSpan TransferAt;
    public NetEntity? AttachedTo;
    public TimeSpan AttachDelay;
    public int Range;
    public bool Injecting;
    public ProtoId<EmotePrototype> RipEmote;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> SkillRequired;
    public string[] TransferableReagents;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BloodPackComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BloodPackComponent, ComponentGetState>(new ComponentEventRefHandler<BloodPackComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BloodPackComponent, ComponentHandleState>(new ComponentEventRefHandler<BloodPackComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BloodPackComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BloodPackComponent.BloodPackComponent_AutoState()
      {
        Solution = component.Solution,
        FillPercentage = component.FillPercentage,
        FillColor = component.FillColor,
        MaxFillLevels = component.MaxFillLevels,
        FillBaseName = component.FillBaseName,
        TransferAmount = component.TransferAmount,
        TransferDelay = component.TransferDelay,
        TransferAt = component.TransferAt,
        AttachedTo = this.GetNetEntity(component.AttachedTo),
        AttachDelay = component.AttachDelay,
        Range = component.Range,
        Injecting = component.Injecting,
        RipEmote = component.RipEmote,
        SkillRequired = component.SkillRequired,
        TransferableReagents = component.TransferableReagents
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BloodPackComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BloodPackComponent.BloodPackComponent_AutoState current))
        return;
      component.Solution = current.Solution;
      component.FillPercentage = current.FillPercentage;
      component.FillColor = current.FillColor;
      component.MaxFillLevels = current.MaxFillLevels;
      component.FillBaseName = current.FillBaseName;
      component.TransferAmount = current.TransferAmount;
      component.TransferDelay = current.TransferDelay;
      component.TransferAt = current.TransferAt;
      component.AttachedTo = this.EnsureEntity<BloodPackComponent>(current.AttachedTo, uid);
      component.AttachDelay = current.AttachDelay;
      component.Range = current.Range;
      component.Injecting = current.Injecting;
      component.RipEmote = current.RipEmote;
      component.SkillRequired = current.SkillRequired == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.SkillRequired);
      component.TransferableReagents = current.TransferableReagents;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, BloodPackComponent>(uid, component, ref args1);
    }
  }
}
