// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.FlammableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class FlammableComponent : 
  Component,
  ISerializationGenerated<FlammableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Resisting;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnFire;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FireStacks;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaximumFireStacks = 45f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinimumFireStacks;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string FlammableFixtureID = "flammable";
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinIgnitionTemperature = 373.15f;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public IPhysShape FlammableCollisionShape = (IPhysShape) new PhysShapeCircle(0.35f);
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AlwaysCombustible;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanExtinguish = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FirestacksOnIgnite = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float FirestackFade = -0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<AlertPrototype> FireAlert = ProtoId<AlertPrototype>.op_Implicit("Fire");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ResistStacks = -10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Intensity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Duration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ResistDuration = TimeSpan.FromSeconds(3L);

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FireSpread { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanResistFire { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FlammableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FlammableComponent) component;
    if (serialization.TryCustomCopy<FlammableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Resisting, ref flag1, hookCtx, false, context))
      flag1 = this.Resisting;
    target.Resisting = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnFire, ref flag2, hookCtx, false, context))
      flag2 = this.OnFire;
    target.OnFire = flag2;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FireStacks, ref num1, hookCtx, false, context))
      num1 = this.FireStacks;
    target.FireStacks = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaximumFireStacks, ref num2, hookCtx, false, context))
      num2 = this.MaximumFireStacks;
    target.MaximumFireStacks = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinimumFireStacks, ref num3, hookCtx, false, context))
      num3 = this.MinimumFireStacks;
    target.MinimumFireStacks = num3;
    string str = (string) null;
    if (this.FlammableFixtureID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FlammableFixtureID, ref str, hookCtx, false, context))
      str = this.FlammableFixtureID;
    target.FlammableFixtureID = str;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinIgnitionTemperature, ref num4, hookCtx, false, context))
      num4 = this.MinIgnitionTemperature;
    target.MinIgnitionTemperature = num4;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.FireSpread, ref flag3, hookCtx, false, context))
      flag3 = this.FireSpread;
    target.FireSpread = flag3;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanResistFire, ref flag4, hookCtx, false, context))
      flag4 = this.CanResistFire;
    target.CanResistFire = flag4;
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
    IPhysShape iphysShape = (IPhysShape) null;
    if (this.FlammableCollisionShape == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IPhysShape>(this.FlammableCollisionShape, ref iphysShape, hookCtx, true, context))
      iphysShape = serialization.CreateCopy<IPhysShape>(this.FlammableCollisionShape, hookCtx, context, false);
    target.FlammableCollisionShape = iphysShape;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AlwaysCombustible, ref flag5, hookCtx, false, context))
      flag5 = this.AlwaysCombustible;
    target.AlwaysCombustible = flag5;
    bool flag6 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanExtinguish, ref flag6, hookCtx, false, context))
      flag6 = this.CanExtinguish;
    target.CanExtinguish = flag6;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FirestacksOnIgnite, ref num5, hookCtx, false, context))
      num5 = this.FirestacksOnIgnite;
    target.FirestacksOnIgnite = num5;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FirestackFade, ref num6, hookCtx, false, context))
      num6 = this.FirestackFade;
    target.FirestackFade = num6;
    ProtoId<AlertPrototype> protoId = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.FireAlert, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.FireAlert, hookCtx, context, false);
    target.FireAlert = protoId;
    int num7 = 0;
    if (!serialization.TryCustomCopy<int>(this.ResistStacks, ref num7, hookCtx, false, context))
      num7 = this.ResistStacks;
    target.ResistStacks = num7;
    int num8 = 0;
    if (!serialization.TryCustomCopy<int>(this.Intensity, ref num8, hookCtx, false, context))
      num8 = this.Intensity;
    target.Intensity = num8;
    int num9 = 0;
    if (!serialization.TryCustomCopy<int>(this.Duration, ref num9, hookCtx, false, context))
      num9 = this.Duration;
    target.Duration = num9;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ResistDuration, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.ResistDuration, hookCtx, context, false);
    target.ResistDuration = timeSpan;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FlammableComponent target,
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
    FlammableComponent target1 = (FlammableComponent) target;
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
    FlammableComponent target1 = (FlammableComponent) target;
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
    FlammableComponent target1 = (FlammableComponent) target;
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
  virtual FlammableComponent Component.Instantiate() => new FlammableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FlammableComponent_AutoState : IComponentState
  {
    public bool Resisting;
    public bool OnFire;
    public float FireStacks;
    public float MaximumFireStacks;
    public float MinimumFireStacks;
    public string FlammableFixtureID;
    public float MinIgnitionTemperature;
    public bool FireSpread;
    public bool CanResistFire;
    public DamageSpecifier Damage;
    public IPhysShape FlammableCollisionShape;
    public bool AlwaysCombustible;
    public bool CanExtinguish;
    public float FirestacksOnIgnite;
    public float FirestackFade;
    public ProtoId<AlertPrototype> FireAlert;
    public int ResistStacks;
    public int Intensity;
    public int Duration;
    public TimeSpan ResistDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FlammableComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<FlammableComponent, ComponentGetState>(new ComponentEventRefHandler<FlammableComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<FlammableComponent, ComponentHandleState>(new ComponentEventRefHandler<FlammableComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      FlammableComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new FlammableComponent.FlammableComponent_AutoState()
      {
        Resisting = component.Resisting,
        OnFire = component.OnFire,
        FireStacks = component.FireStacks,
        MaximumFireStacks = component.MaximumFireStacks,
        MinimumFireStacks = component.MinimumFireStacks,
        FlammableFixtureID = component.FlammableFixtureID,
        MinIgnitionTemperature = component.MinIgnitionTemperature,
        FireSpread = component.FireSpread,
        CanResistFire = component.CanResistFire,
        Damage = component.Damage,
        FlammableCollisionShape = component.FlammableCollisionShape,
        AlwaysCombustible = component.AlwaysCombustible,
        CanExtinguish = component.CanExtinguish,
        FirestacksOnIgnite = component.FirestacksOnIgnite,
        FirestackFade = component.FirestackFade,
        FireAlert = component.FireAlert,
        ResistStacks = component.ResistStacks,
        Intensity = component.Intensity,
        Duration = component.Duration,
        ResistDuration = component.ResistDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FlammableComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is FlammableComponent.FlammableComponent_AutoState current))
        return;
      component.Resisting = current.Resisting;
      component.OnFire = current.OnFire;
      component.FireStacks = current.FireStacks;
      component.MaximumFireStacks = current.MaximumFireStacks;
      component.MinimumFireStacks = current.MinimumFireStacks;
      component.FlammableFixtureID = current.FlammableFixtureID;
      component.MinIgnitionTemperature = current.MinIgnitionTemperature;
      component.FireSpread = current.FireSpread;
      component.CanResistFire = current.CanResistFire;
      component.Damage = current.Damage;
      component.FlammableCollisionShape = current.FlammableCollisionShape;
      component.AlwaysCombustible = current.AlwaysCombustible;
      component.CanExtinguish = current.CanExtinguish;
      component.FirestacksOnIgnite = current.FirestacksOnIgnite;
      component.FirestackFade = current.FirestackFade;
      component.FireAlert = current.FireAlert;
      component.ResistStacks = current.ResistStacks;
      component.Intensity = current.Intensity;
      component.Duration = current.Duration;
      component.ResistDuration = current.ResistDuration;
    }
  }
}
