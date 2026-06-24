// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.ExpendableLightComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Stacks;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ExpendableLightComponent : 
  Component,
  ISerializationGenerated<ExpendableLightComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string? IconStateSpent;
  [DataField(null, false, 1, false, false, null)]
  public string? IconStateLit;
  [DataField(null, false, 1, false, false, null)]
  public string? SpriteShaderLit;
  [DataField(null, false, 1, false, false, null)]
  public string? SpriteShaderSpent;
  [DataField(null, false, 1, false, false, null)]
  public bool StartsActivated;
  [DataField(null, false, 1, false, false, null)]
  public Color? GlowColorLit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? PlayingStream;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StateExpiryTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan GlowDuration = TimeSpan.FromSeconds(900.0);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan FadeOutDuration = TimeSpan.FromSeconds(300.0);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StackPrototype>? RefuelMaterialID;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RefuelMaterialTime = TimeSpan.FromSeconds(15.0);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RefuelMaximumDuration = TimeSpan.FromSeconds(1800.0);
  [DataField(null, false, 1, false, false, null)]
  public bool PickupWhileOn = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Activated
  {
    get
    {
      bool activated;
      switch (this.CurrentState)
      {
        case ExpendableLightState.Lit:
        case ExpendableLightState.Fading:
          activated = true;
          break;
        default:
          activated = false;
          break;
      }
      return activated;
    }
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ExpendableLightState CurrentState { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public string TurnOnBehaviourID { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public string FadeOutBehaviourID { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public string SpentDesc { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public string SpentName { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? LitSound { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? LoopedSound { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? DieSound { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExpendableLightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExpendableLightComponent) target1;
    if (serialization.TryCustomCopy<ExpendableLightComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.IconStateSpent, ref target2, hookCtx, false, context))
      target2 = this.IconStateSpent;
    target.IconStateSpent = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.IconStateLit, ref target3, hookCtx, false, context))
      target3 = this.IconStateLit;
    target.IconStateLit = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SpriteShaderLit, ref target4, hookCtx, false, context))
      target4 = this.SpriteShaderLit;
    target.SpriteShaderLit = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SpriteShaderSpent, ref target5, hookCtx, false, context))
      target5 = this.SpriteShaderSpent;
    target.SpriteShaderSpent = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.StartsActivated, ref target6, hookCtx, false, context))
      target6 = this.StartsActivated;
    target.StartsActivated = target6;
    Color? target7 = new Color?();
    if (!serialization.TryCustomCopy<Color?>(this.GlowColorLit, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Color?>(this.GlowColorLit, hookCtx, context);
    target.GlowColorLit = target7;
    EntityUid? target8 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.PlayingStream, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntityUid?>(this.PlayingStream, hookCtx, context);
    target.PlayingStream = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StateExpiryTime, ref target9, hookCtx, false, context))
      target9 = this.StateExpiryTime;
    target.StateExpiryTime = target9;
    ExpendableLightState target10 = ExpendableLightState.BrandNew;
    if (!serialization.TryCustomCopy<ExpendableLightState>(this.CurrentState, ref target10, hookCtx, false, context))
      target10 = this.CurrentState;
    target.CurrentState = target10;
    string target11 = (string) null;
    if (this.TurnOnBehaviourID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TurnOnBehaviourID, ref target11, hookCtx, false, context))
      target11 = this.TurnOnBehaviourID;
    target.TurnOnBehaviourID = target11;
    string target12 = (string) null;
    if (this.FadeOutBehaviourID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FadeOutBehaviourID, ref target12, hookCtx, false, context))
      target12 = this.FadeOutBehaviourID;
    target.FadeOutBehaviourID = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GlowDuration, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.GlowDuration, hookCtx, context);
    target.GlowDuration = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FadeOutDuration, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.FadeOutDuration, hookCtx, context);
    target.FadeOutDuration = target14;
    ProtoId<StackPrototype>? target15 = new ProtoId<StackPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<StackPrototype>?>(this.RefuelMaterialID, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<ProtoId<StackPrototype>?>(this.RefuelMaterialID, hookCtx, context);
    target.RefuelMaterialID = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RefuelMaterialTime, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.RefuelMaterialTime, hookCtx, context);
    target.RefuelMaterialTime = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RefuelMaximumDuration, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.RefuelMaximumDuration, hookCtx, context);
    target.RefuelMaximumDuration = target17;
    string target18 = (string) null;
    if (this.SpentDesc == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SpentDesc, ref target18, hookCtx, false, context))
      target18 = this.SpentDesc;
    target.SpentDesc = target18;
    string target19 = (string) null;
    if (this.SpentName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SpentName, ref target19, hookCtx, false, context))
      target19 = this.SpentName;
    target.SpentName = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LitSound, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.LitSound, hookCtx, context);
    target.LitSound = target20;
    SoundSpecifier target21 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LoopedSound, ref target21, hookCtx, true, context))
      target21 = serialization.CreateCopy<SoundSpecifier>(this.LoopedSound, hookCtx, context);
    target.LoopedSound = target21;
    SoundSpecifier target22 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DieSound, ref target22, hookCtx, true, context))
      target22 = serialization.CreateCopy<SoundSpecifier>(this.DieSound, hookCtx, context);
    target.DieSound = target22;
    bool target23 = false;
    if (!serialization.TryCustomCopy<bool>(this.PickupWhileOn, ref target23, hookCtx, false, context))
      target23 = this.PickupWhileOn;
    target.PickupWhileOn = target23;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExpendableLightComponent target,
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
    ExpendableLightComponent target1 = (ExpendableLightComponent) target;
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
    ExpendableLightComponent target1 = (ExpendableLightComponent) target;
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
    ExpendableLightComponent target1 = (ExpendableLightComponent) target;
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
  virtual ExpendableLightComponent Component.Instantiate() => new ExpendableLightComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ExpendableLightComponent_AutoState : IComponentState
  {
    public NetEntity? PlayingStream;
    public float StateExpiryTime;
    public ExpendableLightState CurrentState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ExpendableLightComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ExpendableLightComponent, ComponentGetState>(new ComponentEventRefHandler<ExpendableLightComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ExpendableLightComponent, ComponentHandleState>(new ComponentEventRefHandler<ExpendableLightComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ExpendableLightComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ExpendableLightComponent.ExpendableLightComponent_AutoState()
      {
        PlayingStream = this.GetNetEntity(component.PlayingStream),
        StateExpiryTime = component.StateExpiryTime,
        CurrentState = component.CurrentState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ExpendableLightComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ExpendableLightComponent.ExpendableLightComponent_AutoState current))
        return;
      component.PlayingStream = this.EnsureEntity<ExpendableLightComponent>(current.PlayingStream, uid);
      component.StateExpiryTime = current.StateExpiryTime;
      component.CurrentState = current.CurrentState;
    }
  }
}
