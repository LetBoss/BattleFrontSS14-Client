// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCBattleExecuteComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
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
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMGunSystem)})]
public sealed class RMCBattleExecuteComponent : 
  Component,
  ISerializationGenerated<RMCBattleExecuteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillExecution";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier()
  {
    DamageDict = {
      ["Blunt"] = (FixedPoint2) 200
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BattleExecuteTimeSeconds = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCBattleExecuteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCBattleExecuteComponent) target1;
    if (serialization.TryCustomCopy<RMCBattleExecuteComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<SkillDefinitionComponent> target2 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target2;
    DamageSpecifier target3 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target3, hookCtx, false, context))
    {
      if (this.Damage == null)
        target3 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target3, hookCtx, context, true);
    }
    target.Damage = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BattleExecuteTimeSeconds, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.BattleExecuteTimeSeconds, hookCtx, context);
    target.BattleExecuteTimeSeconds = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCBattleExecuteComponent target,
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
    RMCBattleExecuteComponent target1 = (RMCBattleExecuteComponent) target;
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
    RMCBattleExecuteComponent target1 = (RMCBattleExecuteComponent) target;
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
    RMCBattleExecuteComponent target1 = (RMCBattleExecuteComponent) target;
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
  virtual RMCBattleExecuteComponent Component.Instantiate() => new RMCBattleExecuteComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCBattleExecuteComponent_AutoState : IComponentState
  {
    public EntProtoId<SkillDefinitionComponent> Skill;
    public DamageSpecifier Damage;
    public TimeSpan BattleExecuteTimeSeconds;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCBattleExecuteComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCBattleExecuteComponent, ComponentGetState>(new ComponentEventRefHandler<RMCBattleExecuteComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCBattleExecuteComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCBattleExecuteComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCBattleExecuteComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCBattleExecuteComponent.RMCBattleExecuteComponent_AutoState()
      {
        Skill = component.Skill,
        Damage = component.Damage,
        BattleExecuteTimeSeconds = component.BattleExecuteTimeSeconds
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCBattleExecuteComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCBattleExecuteComponent.RMCBattleExecuteComponent_AutoState current))
        return;
      component.Skill = current.Skill;
      component.Damage = current.Damage;
      component.BattleExecuteTimeSeconds = current.BattleExecuteTimeSeconds;
    }
  }
}
