// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Destroy.XenoDestroyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Maths;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Explosion;
using Content.Shared.Whitelist;
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
namespace Content.Shared._RMC14.Xenonids.Destroy;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoDestroyComponent : 
  Component,
  ISerializationGenerated<XenoDestroyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier StructureDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier MobDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Gibs = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Telegraph = (EntProtoId) "RMCEffectXenoTelegraphKing";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 7f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan JumpTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CrashTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> Emote = (ProtoId<EmotePrototype>) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/meteorimpact.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist Structures = new EntityWhitelist();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Knockback = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ShakeCameraRange = RMCMathExtensions.CircleAreaFromSquareAbilityRange(7f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(60L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ExplosionPrototype> ExplosionType = (ProtoId<ExplosionPrototype>) "RMCOB";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId SmokeEffect = (EntProtoId) "CMExplosionEffectGrenade";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoDestroyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoDestroyComponent) target1;
    if (serialization.TryCustomCopy<XenoDestroyComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.StructureDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.StructureDamage, ref target2, hookCtx, false, context))
    {
      if (this.StructureDamage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.StructureDamage, ref target2, hookCtx, context, true);
    }
    target.StructureDamage = target2;
    DamageSpecifier target3 = (DamageSpecifier) null;
    if (this.MobDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.MobDamage, ref target3, hookCtx, false, context))
    {
      if (this.MobDamage == null)
        target3 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.MobDamage, ref target3, hookCtx, context, true);
    }
    target.MobDamage = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Gibs, ref target4, hookCtx, false, context))
      target4 = this.Gibs;
    target.Gibs = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Telegraph, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.Telegraph, hookCtx, context);
    target.Telegraph = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target6, hookCtx, false, context))
      target6 = this.Range;
    target.Range = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.JumpTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.JumpTime, hookCtx, context);
    target.JumpTime = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CrashTime, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.CrashTime, hookCtx, context);
    target.CrashTime = target8;
    ProtoId<EmotePrototype> target9 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.Emote, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.Emote, hookCtx, context);
    target.Emote = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target10;
    EntityWhitelist target11 = (EntityWhitelist) null;
    if (this.Structures == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Structures, ref target11, hookCtx, false, context))
    {
      if (this.Structures == null)
        target11 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Structures, ref target11, hookCtx, context, true);
    }
    target.Structures = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Knockback, ref target12, hookCtx, false, context))
      target12 = this.Knockback;
    target.Knockback = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShakeCameraRange, ref target13, hookCtx, false, context))
      target13 = this.ShakeCameraRange;
    target.ShakeCameraRange = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target14;
    ProtoId<ExplosionPrototype> target15 = new ProtoId<ExplosionPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ExplosionPrototype>>(this.ExplosionType, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<ProtoId<ExplosionPrototype>>(this.ExplosionType, hookCtx, context);
    target.ExplosionType = target15;
    EntProtoId target16 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.SmokeEffect, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<EntProtoId>(this.SmokeEffect, hookCtx, context);
    target.SmokeEffect = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoDestroyComponent target,
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
    XenoDestroyComponent target1 = (XenoDestroyComponent) target;
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
    XenoDestroyComponent target1 = (XenoDestroyComponent) target;
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
    XenoDestroyComponent target1 = (XenoDestroyComponent) target;
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
  virtual XenoDestroyComponent Component.Instantiate() => new XenoDestroyComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoDestroyComponent_AutoState : IComponentState
  {
    public DamageSpecifier StructureDamage;
    public DamageSpecifier MobDamage;
    public bool Gibs;
    public EntProtoId Telegraph;
    public float Range;
    public TimeSpan JumpTime;
    public TimeSpan CrashTime;
    public ProtoId<EmotePrototype> Emote;
    public SoundSpecifier Sound;
    public EntityWhitelist Structures;
    public float Knockback;
    public float ShakeCameraRange;
    public TimeSpan Cooldown;
    public ProtoId<ExplosionPrototype> ExplosionType;
    public EntProtoId SmokeEffect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoDestroyComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoDestroyComponent, ComponentGetState>(new ComponentEventRefHandler<XenoDestroyComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoDestroyComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoDestroyComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoDestroyComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoDestroyComponent.XenoDestroyComponent_AutoState()
      {
        StructureDamage = component.StructureDamage,
        MobDamage = component.MobDamage,
        Gibs = component.Gibs,
        Telegraph = component.Telegraph,
        Range = component.Range,
        JumpTime = component.JumpTime,
        CrashTime = component.CrashTime,
        Emote = component.Emote,
        Sound = component.Sound,
        Structures = component.Structures,
        Knockback = component.Knockback,
        ShakeCameraRange = component.ShakeCameraRange,
        Cooldown = component.Cooldown,
        ExplosionType = component.ExplosionType,
        SmokeEffect = component.SmokeEffect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoDestroyComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoDestroyComponent.XenoDestroyComponent_AutoState current))
        return;
      component.StructureDamage = current.StructureDamage;
      component.MobDamage = current.MobDamage;
      component.Gibs = current.Gibs;
      component.Telegraph = current.Telegraph;
      component.Range = current.Range;
      component.JumpTime = current.JumpTime;
      component.CrashTime = current.CrashTime;
      component.Emote = current.Emote;
      component.Sound = current.Sound;
      component.Structures = current.Structures;
      component.Knockback = current.Knockback;
      component.ShakeCameraRange = current.ShakeCameraRange;
      component.Cooldown = current.Cooldown;
      component.ExplosionType = current.ExplosionType;
      component.SmokeEffect = current.SmokeEffect;
    }
  }
}
