// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Components.RMCCanBeFultonedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship.Utility.Systems;
using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCFultonSystem)})]
public sealed class RMCCanBeFultonedComponent : 
  Component,
  ISerializationGenerated<RMCCanBeFultonedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillIntel";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ReturnDelay = TimeSpan.FromSeconds(150L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? FultonSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Items/fulton.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCCanBeFultonedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCCanBeFultonedComponent) target1;
    if (serialization.TryCustomCopy<RMCCanBeFultonedComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target2;
    EntProtoId<SkillDefinitionComponent> target3 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ReturnDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.ReturnDelay, hookCtx, context);
    target.ReturnDelay = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FultonSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.FultonSound, hookCtx, context);
    target.FultonSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCCanBeFultonedComponent target,
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
    RMCCanBeFultonedComponent target1 = (RMCCanBeFultonedComponent) target;
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
    RMCCanBeFultonedComponent target1 = (RMCCanBeFultonedComponent) target;
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
    RMCCanBeFultonedComponent target1 = (RMCCanBeFultonedComponent) target;
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
  virtual RMCCanBeFultonedComponent Component.Instantiate() => new RMCCanBeFultonedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCCanBeFultonedComponent_AutoState : IComponentState
  {
    public TimeSpan Delay;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public TimeSpan ReturnDelay;
    public SoundSpecifier? FultonSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCCanBeFultonedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCCanBeFultonedComponent, ComponentGetState>(new ComponentEventRefHandler<RMCCanBeFultonedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCCanBeFultonedComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCCanBeFultonedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCCanBeFultonedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCCanBeFultonedComponent.RMCCanBeFultonedComponent_AutoState()
      {
        Delay = component.Delay,
        Skill = component.Skill,
        ReturnDelay = component.ReturnDelay,
        FultonSound = component.FultonSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCCanBeFultonedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCCanBeFultonedComponent.RMCCanBeFultonedComponent_AutoState current))
        return;
      component.Delay = current.Delay;
      component.Skill = current.Skill;
      component.ReturnDelay = current.ReturnDelay;
      component.FultonSound = current.FultonSound;
    }
  }
}
