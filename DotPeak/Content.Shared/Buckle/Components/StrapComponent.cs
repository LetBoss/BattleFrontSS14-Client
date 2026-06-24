// Decompiled with JetBrains decompiler
// Type: Content.Shared.Buckle.Components.StrapComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Buckle.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedBuckleSystem)})]
public sealed class StrapComponent : 
  Component,
  ISerializationGenerated<StrapComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> BuckledEntities = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public StrapPosition Position;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 BuckleOffset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public Angle Rotation;
  [DataField(null, false, 1, false, false, null)]
  public int Size = 100;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BuckleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/buckle.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier UnbuckleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/unbuckle.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype>? BuckledAlertType = ProtoId<AlertPrototype>.op_Implicit("Buckled");
  [DataField(null, false, 1, false, false, null)]
  public float BuckleDoafterTime = 2f;
  [DataField(null, false, 1, false, false, null)]
  public bool BuckleOnInteractHand = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StrapComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (StrapComponent) component;
    if (serialization.TryCustomCopy<StrapComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntityUid> entityUidSet = (HashSet<EntityUid>) null;
    if (this.BuckledEntities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.BuckledEntities, ref entityUidSet, hookCtx, true, context))
      entityUidSet = serialization.CreateCopy<HashSet<EntityUid>>(this.BuckledEntities, hookCtx, context, false);
    target.BuckledEntities = entityUidSet;
    EntityWhitelist entityWhitelist1 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        entityWhitelist1 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, context, false);
    }
    target.Whitelist = entityWhitelist1;
    EntityWhitelist entityWhitelist2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref entityWhitelist2, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        entityWhitelist2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref entityWhitelist2, hookCtx, context, false);
    }
    target.Blacklist = entityWhitelist2;
    StrapPosition strapPosition = StrapPosition.None;
    if (!serialization.TryCustomCopy<StrapPosition>(this.Position, ref strapPosition, hookCtx, false, context))
      strapPosition = this.Position;
    target.Position = strapPosition;
    Vector2 vector2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.BuckleOffset, ref vector2, hookCtx, false, context))
      vector2 = serialization.CreateCopy<Vector2>(this.BuckleOffset, hookCtx, context, false);
    target.BuckleOffset = vector2;
    Angle angle = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Rotation, ref angle, hookCtx, false, context))
      angle = serialization.CreateCopy<Angle>(this.Rotation, hookCtx, context, false);
    target.Rotation = angle;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.Size, ref num1, hookCtx, false, context))
      num1 = this.Size;
    target.Size = num1;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag1, hookCtx, false, context))
      flag1 = this.Enabled;
    target.Enabled = flag1;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.BuckleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BuckleSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.BuckleSound, hookCtx, context, false);
    target.BuckleSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (this.UnbuckleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnbuckleSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.UnbuckleSound, hookCtx, context, false);
    target.UnbuckleSound = soundSpecifier2;
    ProtoId<AlertPrototype>? nullable = new ProtoId<AlertPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>?>(this.BuckledAlertType, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<ProtoId<AlertPrototype>?>(this.BuckledAlertType, hookCtx, context, false);
    target.BuckledAlertType = nullable;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BuckleDoafterTime, ref num2, hookCtx, false, context))
      num2 = this.BuckleDoafterTime;
    target.BuckleDoafterTime = num2;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.BuckleOnInteractHand, ref flag2, hookCtx, false, context))
      flag2 = this.BuckleOnInteractHand;
    target.BuckleOnInteractHand = flag2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StrapComponent target,
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
    StrapComponent target1 = (StrapComponent) target;
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
    StrapComponent target1 = (StrapComponent) target;
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
    StrapComponent target1 = (StrapComponent) target;
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
  virtual StrapComponent Component.Instantiate() => new StrapComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StrapComponent_AutoState : IComponentState
  {
    public HashSet<NetEntity> BuckledEntities;
    public StrapPosition Position;
    public Vector2 BuckleOffset;
    public bool Enabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StrapComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<StrapComponent, ComponentGetState>(new ComponentEventRefHandler<StrapComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<StrapComponent, ComponentHandleState>(new ComponentEventRefHandler<StrapComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, StrapComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new StrapComponent.StrapComponent_AutoState()
      {
        BuckledEntities = this.GetNetEntitySet(component.BuckledEntities),
        Position = component.Position,
        BuckleOffset = component.BuckleOffset,
        Enabled = component.Enabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StrapComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is StrapComponent.StrapComponent_AutoState current))
        return;
      this.EnsureEntitySet<StrapComponent>(current.BuckledEntities, uid, component.BuckledEntities);
      component.Position = current.Position;
      component.BuckleOffset = current.BuckleOffset;
      component.Enabled = current.Enabled;
    }
  }
}
