// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Pvo.CivPvoComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Pvo;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class CivPvoComponent : 
  Component,
  ISerializationGenerated<CivPvoComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string SideId = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TeamId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Radius = 28f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Craftable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Infinite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxCharges = 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Tier1Charges;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Tier2Charges;
  [DataField(null, false, 1, false, false, null)]
  public int StartingTier1 = 6;
  [DataField(null, false, 1, false, false, null)]
  public int StartingTier2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FlashRadius = 12f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ShotSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/gun_sentry.ogg");
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? ShotEffect;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? HitSound = (SoundSpecifier) new SoundCollectionSpecifier("ExplosionSmall", new AudioParams?(AudioParams.Default.WithVolume(-4f)));
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? HitEffect = (EntProtoId?) "CMExplosionEffectGrenade";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivPvoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivPvoComponent) target1;
    if (serialization.TryCustomCopy<CivPvoComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SideId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SideId, ref target2, hookCtx, false, context))
      target2 = this.SideId;
    target.SideId = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.TeamId, ref target3, hookCtx, false, context))
      target3 = this.TeamId;
    target.TeamId = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target4, hookCtx, false, context))
      target4 = this.Radius;
    target.Radius = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Craftable, ref target5, hookCtx, false, context))
      target5 = this.Craftable;
    target.Craftable = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Infinite, ref target6, hookCtx, false, context))
      target6 = this.Infinite;
    target.Infinite = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxCharges, ref target7, hookCtx, false, context))
      target7 = this.MaxCharges;
    target.MaxCharges = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.Tier1Charges, ref target8, hookCtx, false, context))
      target8 = this.Tier1Charges;
    target.Tier1Charges = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.Tier2Charges, ref target9, hookCtx, false, context))
      target9 = this.Tier2Charges;
    target.Tier2Charges = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.StartingTier1, ref target10, hookCtx, false, context))
      target10 = this.StartingTier1;
    target.StartingTier1 = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.StartingTier2, ref target11, hookCtx, false, context))
      target11 = this.StartingTier2;
    target.StartingTier2 = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlashRadius, ref target12, hookCtx, false, context))
      target12 = this.FlashRadius;
    target.FlashRadius = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShotSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.ShotSound, hookCtx, context);
    target.ShotSound = target13;
    EntProtoId? target14 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ShotEffect, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<EntProtoId?>(this.ShotEffect, hookCtx, context);
    target.ShotEffect = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HitSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.HitSound, hookCtx, context);
    target.HitSound = target15;
    EntProtoId? target16 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.HitEffect, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<EntProtoId?>(this.HitEffect, hookCtx, context);
    target.HitEffect = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivPvoComponent target,
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
    CivPvoComponent target1 = (CivPvoComponent) target;
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
    CivPvoComponent target1 = (CivPvoComponent) target;
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
    CivPvoComponent target1 = (CivPvoComponent) target;
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
  virtual CivPvoComponent Component.Instantiate() => new CivPvoComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivPvoComponent_AutoState : IComponentState
  {
    public string SideId;
    public int TeamId;
    public float Radius;
    public bool Craftable;
    public bool Infinite;
    public int MaxCharges;
    public int Tier1Charges;
    public int Tier2Charges;
    public float FlashRadius;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivPvoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivPvoComponent, ComponentGetState>(new ComponentEventRefHandler<CivPvoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivPvoComponent, ComponentHandleState>(new ComponentEventRefHandler<CivPvoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, CivPvoComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivPvoComponent.CivPvoComponent_AutoState()
      {
        SideId = component.SideId,
        TeamId = component.TeamId,
        Radius = component.Radius,
        Craftable = component.Craftable,
        Infinite = component.Infinite,
        MaxCharges = component.MaxCharges,
        Tier1Charges = component.Tier1Charges,
        Tier2Charges = component.Tier2Charges,
        FlashRadius = component.FlashRadius
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivPvoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivPvoComponent.CivPvoComponent_AutoState current))
        return;
      component.SideId = current.SideId;
      component.TeamId = current.TeamId;
      component.Radius = current.Radius;
      component.Craftable = current.Craftable;
      component.Infinite = current.Infinite;
      component.MaxCharges = current.MaxCharges;
      component.Tier1Charges = current.Tier1Charges;
      component.Tier2Charges = current.Tier2Charges;
      component.FlashRadius = current.FlashRadius;
    }
  }
}
