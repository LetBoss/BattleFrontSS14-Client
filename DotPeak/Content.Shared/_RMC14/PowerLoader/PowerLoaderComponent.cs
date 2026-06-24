// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.PowerLoader.PowerLoaderComponent
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.PowerLoader;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (PowerLoaderSystem)})]
public sealed class PowerLoaderComponent : 
  Component,
  ISerializationGenerated<PowerLoaderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SkillWhitelist Skills;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> SpeedSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillPowerLoader";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedPerSkill = 1.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string VirtualContainerId = "rmc_power_loader_cargo_virtual";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId VirtualRight = (EntProtoId) "RMCVirtualPowerLoaderRight";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId VirtualLeft = (EntProtoId) "RMCVirtualPowerLoaderLeft";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Content.Shared.DoAfter.DoAfter? DoAfter;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PowerLoaderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PowerLoaderComponent) target1;
    if (serialization.TryCustomCopy<PowerLoaderComponent>(this, ref target, hookCtx, false, context))
      return;
    SkillWhitelist target2 = (SkillWhitelist) null;
    if (this.Skills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SkillWhitelist>(this.Skills, ref target2, hookCtx, false, context))
    {
      if (this.Skills == null)
        target2 = (SkillWhitelist) null;
      else
        serialization.CopyTo<SkillWhitelist>(this.Skills, ref target2, hookCtx, context, true);
    }
    target.Skills = target2;
    EntProtoId<SkillDefinitionComponent> target3 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.SpeedSkill, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.SpeedSkill, hookCtx, context);
    target.SpeedSkill = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedPerSkill, ref target4, hookCtx, false, context))
      target4 = this.SpeedPerSkill;
    target.SpeedPerSkill = target4;
    string target5 = (string) null;
    if (this.VirtualContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.VirtualContainerId, ref target5, hookCtx, false, context))
      target5 = this.VirtualContainerId;
    target.VirtualContainerId = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.VirtualRight, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.VirtualRight, hookCtx, context);
    target.VirtualRight = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.VirtualLeft, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.VirtualLeft, hookCtx, context);
    target.VirtualLeft = target7;
    Content.Shared.DoAfter.DoAfter target8 = (Content.Shared.DoAfter.DoAfter) null;
    if (!serialization.TryCustomCopy<Content.Shared.DoAfter.DoAfter>(this.DoAfter, ref target8, hookCtx, false, context))
    {
      if (this.DoAfter == null)
        target8 = (Content.Shared.DoAfter.DoAfter) null;
      else
        serialization.CopyTo<Content.Shared.DoAfter.DoAfter>(this.DoAfter, ref target8, hookCtx, context);
    }
    target.DoAfter = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PowerLoaderComponent target,
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
    PowerLoaderComponent target1 = (PowerLoaderComponent) target;
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
    PowerLoaderComponent target1 = (PowerLoaderComponent) target;
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
    PowerLoaderComponent target1 = (PowerLoaderComponent) target;
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
  virtual PowerLoaderComponent Component.Instantiate() => new PowerLoaderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PowerLoaderComponent_AutoState : IComponentState
  {
    public SkillWhitelist Skills;
    public EntProtoId<SkillDefinitionComponent> SpeedSkill;
    public float SpeedPerSkill;
    public string VirtualContainerId;
    public EntProtoId VirtualRight;
    public EntProtoId VirtualLeft;
    public Content.Shared.DoAfter.DoAfter? DoAfter;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PowerLoaderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PowerLoaderComponent, ComponentGetState>(new ComponentEventRefHandler<PowerLoaderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PowerLoaderComponent, ComponentHandleState>(new ComponentEventRefHandler<PowerLoaderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PowerLoaderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PowerLoaderComponent.PowerLoaderComponent_AutoState()
      {
        Skills = component.Skills,
        SpeedSkill = component.SpeedSkill,
        SpeedPerSkill = component.SpeedPerSkill,
        VirtualContainerId = component.VirtualContainerId,
        VirtualRight = component.VirtualRight,
        VirtualLeft = component.VirtualLeft,
        DoAfter = component.DoAfter
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PowerLoaderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PowerLoaderComponent.PowerLoaderComponent_AutoState current))
        return;
      component.Skills = current.Skills;
      component.SpeedSkill = current.SpeedSkill;
      component.SpeedPerSkill = current.SpeedPerSkill;
      component.VirtualContainerId = current.VirtualContainerId;
      component.VirtualRight = current.VirtualRight;
      component.VirtualLeft = current.VirtualLeft;
      component.DoAfter = current.DoAfter;
    }
  }
}
