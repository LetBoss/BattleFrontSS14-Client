// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Barricade.Components.BarbedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Barricade.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedBarbedSystem)})]
public sealed class BarbedComponent : 
  Component,
  ISerializationGenerated<BarbedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public DamageSpecifier ThornsDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsBarbed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxHealthIncrease;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Spawn;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> RemoveQuality;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? CutSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? BarbSound;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan WireTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CutTime;
  [DataField(null, false, 1, false, false, null)]
  public string FixtureId;

  public BarbedComponent()
  {
    SoundPathSpecifier soundPathSpecifier1 = new SoundPathSpecifier("/Audio/Items/wirecutter.ogg");
    soundPathSpecifier1.Params = AudioParams.Default.WithVariation(new float?(0.35f));
    this.CutSound = (SoundSpecifier) soundPathSpecifier1;
    SoundPathSpecifier soundPathSpecifier2 = new SoundPathSpecifier("/Audio/_RMC14/Items/barbed_wire_movement.ogg");
    soundPathSpecifier2.Params = AudioParams.Default.WithVariation(new float?(0.35f));
    this.BarbSound = (SoundSpecifier) soundPathSpecifier2;
    this.WireTime = TimeSpan.FromSeconds(2L);
    this.CutTime = TimeSpan.FromSeconds(1L);
    this.FixtureId = "fix1";
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BarbedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BarbedComponent) target1;
    if (serialization.TryCustomCopy<BarbedComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.ThornsDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ThornsDamage, ref target2, hookCtx, false, context))
    {
      if (this.ThornsDamage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ThornsDamage, ref target2, hookCtx, context, true);
    }
    target.ThornsDamage = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsBarbed, ref target3, hookCtx, false, context))
      target3 = this.IsBarbed;
    target.IsBarbed = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxHealthIncrease, ref target4, hookCtx, false, context))
      target4 = this.MaxHealthIncrease;
    target.MaxHealthIncrease = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target5;
    ProtoId<ToolQualityPrototype> target6 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.RemoveQuality, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.RemoveQuality, hookCtx, context);
    target.RemoveQuality = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CutSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.CutSound, hookCtx, context);
    target.CutSound = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BarbSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.BarbSound, hookCtx, context);
    target.BarbSound = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WireTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.WireTime, hookCtx, context);
    target.WireTime = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CutTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.CutTime, hookCtx, context);
    target.CutTime = target10;
    string target11 = (string) null;
    if (this.FixtureId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FixtureId, ref target11, hookCtx, false, context))
      target11 = this.FixtureId;
    target.FixtureId = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BarbedComponent target,
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
    BarbedComponent target1 = (BarbedComponent) target;
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
    BarbedComponent target1 = (BarbedComponent) target;
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
    BarbedComponent target1 = (BarbedComponent) target;
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
  virtual BarbedComponent Component.Instantiate() => new BarbedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BarbedComponent_AutoState : IComponentState
  {
    public bool IsBarbed;
    public int MaxHealthIncrease;
    public SoundSpecifier? CutSound;
    public SoundSpecifier? BarbSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BarbedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BarbedComponent, ComponentGetState>(new ComponentEventRefHandler<BarbedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BarbedComponent, ComponentHandleState>(new ComponentEventRefHandler<BarbedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, BarbedComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new BarbedComponent.BarbedComponent_AutoState()
      {
        IsBarbed = component.IsBarbed,
        MaxHealthIncrease = component.MaxHealthIncrease,
        CutSound = component.CutSound,
        BarbSound = component.BarbSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BarbedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BarbedComponent.BarbedComponent_AutoState current))
        return;
      component.IsBarbed = current.IsBarbed;
      component.MaxHealthIncrease = current.MaxHealthIncrease;
      component.CutSound = current.CutSound;
      component.BarbSound = current.BarbSound;
    }
  }
}
