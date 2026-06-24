// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.MindRoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Roles;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MindRoleComponent : 
  BaseMindRoleComponent,
  ISerializationGenerated<MindRoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Antag;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RoleTypePrototype>? RoleType;
  [DataField(null, false, 1, false, false, null)]
  public LocId? Subtype;
  [DataField(null, false, 1, false, false, null)]
  public bool ExclusiveAntag;
  [DataField(null, false, 1, false, false, null)]
  public int SortWeight;

  public Entity<MindComponent> Mind { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<Content.Shared.Roles.AntagPrototype>? AntagPrototype { get; set; }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<Content.Shared.Roles.JobPrototype>? JobPrototype { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MindRoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseMindRoleComponent target1 = (BaseMindRoleComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MindRoleComponent) target1;
    if (serialization.TryCustomCopy<MindRoleComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Antag, ref target2, hookCtx, false, context))
      target2 = this.Antag;
    target.Antag = target2;
    ProtoId<RoleTypePrototype>? target3 = new ProtoId<RoleTypePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<RoleTypePrototype>?>(this.RoleType, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<RoleTypePrototype>?>(this.RoleType, hookCtx, context);
    target.RoleType = target3;
    LocId? target4 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Subtype, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId?>(this.Subtype, hookCtx, context);
    target.Subtype = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ExclusiveAntag, ref target5, hookCtx, false, context))
      target5 = this.ExclusiveAntag;
    target.ExclusiveAntag = target5;
    ProtoId<Content.Shared.Roles.AntagPrototype>? target6 = new ProtoId<Content.Shared.Roles.AntagPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<Content.Shared.Roles.AntagPrototype>?>(this.AntagPrototype, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<Content.Shared.Roles.AntagPrototype>?>(this.AntagPrototype, hookCtx, context);
    target.AntagPrototype = target6;
    ProtoId<Content.Shared.Roles.JobPrototype>? target7 = new ProtoId<Content.Shared.Roles.JobPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<Content.Shared.Roles.JobPrototype>?>(this.JobPrototype, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<Content.Shared.Roles.JobPrototype>?>(this.JobPrototype, hookCtx, context);
    target.JobPrototype = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.SortWeight, ref target8, hookCtx, false, context))
      target8 = this.SortWeight;
    target.SortWeight = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MindRoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BaseMindRoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MindRoleComponent target1 = (MindRoleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BaseMindRoleComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MindRoleComponent target1 = (MindRoleComponent) target;
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
    MindRoleComponent target1 = (MindRoleComponent) target;
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
  virtual MindRoleComponent BaseMindRoleComponent.Instantiate() => new MindRoleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MindRoleComponent_AutoState : IComponentState
  {
    public ProtoId<Content.Shared.Roles.JobPrototype>? JobPrototype;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MindRoleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MindRoleComponent, ComponentGetState>(new ComponentEventRefHandler<MindRoleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MindRoleComponent, ComponentHandleState>(new ComponentEventRefHandler<MindRoleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, MindRoleComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new MindRoleComponent.MindRoleComponent_AutoState()
      {
        JobPrototype = component.JobPrototype
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MindRoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MindRoleComponent.MindRoleComponent_AutoState current))
        return;
      component.JobPrototype = current.JobPrototype;
    }
  }
}
