// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunSkilledRecoilComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SkillsSystem)})]
public sealed class GunSkilledRecoilComponent : 
  Component,
  ISerializationGenerated<GunSkilledRecoilComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SetRecoil;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MustBeWielded = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunSkilledRecoilComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunSkilledRecoilComponent) target1;
    if (serialization.TryCustomCopy<GunSkilledRecoilComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SetRecoil, ref target2, hookCtx, false, context))
      target2 = this.SetRecoil;
    target.SetRecoil = target2;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target3 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.Skills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, hookCtx, context);
    target.Skills = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.MustBeWielded, ref target4, hookCtx, false, context))
      target4 = this.MustBeWielded;
    target.MustBeWielded = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunSkilledRecoilComponent target,
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
    GunSkilledRecoilComponent target1 = (GunSkilledRecoilComponent) target;
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
    GunSkilledRecoilComponent target1 = (GunSkilledRecoilComponent) target;
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
    GunSkilledRecoilComponent target1 = (GunSkilledRecoilComponent) target;
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
  virtual GunSkilledRecoilComponent Component.Instantiate() => new GunSkilledRecoilComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunSkilledRecoilComponent_AutoState : IComponentState
  {
    public float SetRecoil;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills;
    public bool MustBeWielded;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunSkilledRecoilComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunSkilledRecoilComponent, ComponentGetState>(new ComponentEventRefHandler<GunSkilledRecoilComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunSkilledRecoilComponent, ComponentHandleState>(new ComponentEventRefHandler<GunSkilledRecoilComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunSkilledRecoilComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunSkilledRecoilComponent.GunSkilledRecoilComponent_AutoState()
      {
        SetRecoil = component.SetRecoil,
        Skills = component.Skills,
        MustBeWielded = component.MustBeWielded
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunSkilledRecoilComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunSkilledRecoilComponent.GunSkilledRecoilComponent_AutoState current))
        return;
      component.SetRecoil = current.SetRecoil;
      component.Skills = current.Skills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.Skills);
      component.MustBeWielded = current.MustBeWielded;
    }
  }
}
