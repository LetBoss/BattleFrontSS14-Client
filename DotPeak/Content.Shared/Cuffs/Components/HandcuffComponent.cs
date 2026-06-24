// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cuffs.Components.HandcuffComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.Cuffs.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedCuffableSystem)})]
public sealed class HandcuffComponent : 
  Component,
  ISerializationGenerated<HandcuffComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float CuffTime = 3.5f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float UncuffTime = 3.5f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float BreakoutTime = 15f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float StunBonus = 2f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool BreakOnRemove;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntProtoId? BrokenPrototype;
  [DataField(null, false, 1, false, false, null)]
  public bool Removing;
  [DataField(null, false, 1, false, false, null)]
  public bool Used;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? CuffedRSI = "Objects/Misc/handcuffs.rsi";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public string? BodyIconState = "body-overlay";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Color Color = Color.White;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier StartCuffSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_start.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier EndCuffSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_end.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier StartBreakoutSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_breakout_start.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier StartUncuffSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_takeoff_start.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier EndUncuffSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_takeoff_end.ogg", new AudioParams?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HandcuffComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (HandcuffComponent) component;
    if (serialization.TryCustomCopy<HandcuffComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CuffTime, ref num1, hookCtx, false, context))
      num1 = this.CuffTime;
    target.CuffTime = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UncuffTime, ref num2, hookCtx, false, context))
      num2 = this.UncuffTime;
    target.UncuffTime = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreakoutTime, ref num3, hookCtx, false, context))
      num3 = this.BreakoutTime;
    target.BreakoutTime = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunBonus, ref num4, hookCtx, false, context))
      num4 = this.StunBonus;
    target.StunBonus = num4;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnRemove, ref flag1, hookCtx, false, context))
      flag1 = this.BreakOnRemove;
    target.BreakOnRemove = flag1;
    EntProtoId? nullable = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.BrokenPrototype, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntProtoId?>(this.BrokenPrototype, hookCtx, context, false);
    target.BrokenPrototype = nullable;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Removing, ref flag2, hookCtx, false, context))
      flag2 = this.Removing;
    target.Removing = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Used, ref flag3, hookCtx, false, context))
      flag3 = this.Used;
    target.Used = flag3;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.CuffedRSI, ref str1, hookCtx, false, context))
      str1 = this.CuffedRSI;
    target.CuffedRSI = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BodyIconState, ref str2, hookCtx, false, context))
      str2 = this.BodyIconState;
    target.BodyIconState = str2;
    Color color = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref color, hookCtx, false, context))
      color = serialization.CreateCopy<Color>(this.Color, hookCtx, context, false);
    target.Color = color;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.StartCuffSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StartCuffSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.StartCuffSound, hookCtx, context, false);
    target.StartCuffSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (this.EndCuffSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EndCuffSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.EndCuffSound, hookCtx, context, false);
    target.EndCuffSound = soundSpecifier2;
    SoundSpecifier soundSpecifier3 = (SoundSpecifier) null;
    if (this.StartBreakoutSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StartBreakoutSound, ref soundSpecifier3, hookCtx, true, context))
      soundSpecifier3 = serialization.CreateCopy<SoundSpecifier>(this.StartBreakoutSound, hookCtx, context, false);
    target.StartBreakoutSound = soundSpecifier3;
    SoundSpecifier soundSpecifier4 = (SoundSpecifier) null;
    if (this.StartUncuffSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StartUncuffSound, ref soundSpecifier4, hookCtx, true, context))
      soundSpecifier4 = serialization.CreateCopy<SoundSpecifier>(this.StartUncuffSound, hookCtx, context, false);
    target.StartUncuffSound = soundSpecifier4;
    SoundSpecifier soundSpecifier5 = (SoundSpecifier) null;
    if (this.EndUncuffSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EndUncuffSound, ref soundSpecifier5, hookCtx, true, context))
      soundSpecifier5 = serialization.CreateCopy<SoundSpecifier>(this.EndUncuffSound, hookCtx, context, false);
    target.EndUncuffSound = soundSpecifier5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HandcuffComponent target,
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
    HandcuffComponent target1 = (HandcuffComponent) target;
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
    HandcuffComponent target1 = (HandcuffComponent) target;
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
    HandcuffComponent target1 = (HandcuffComponent) target;
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
  virtual HandcuffComponent Component.Instantiate() => new HandcuffComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HandcuffComponent_AutoState : IComponentState
  {
    public string? BodyIconState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HandcuffComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<HandcuffComponent, ComponentGetState>(new ComponentEventRefHandler<HandcuffComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<HandcuffComponent, ComponentHandleState>(new ComponentEventRefHandler<HandcuffComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, HandcuffComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new HandcuffComponent.HandcuffComponent_AutoState()
      {
        BodyIconState = component.BodyIconState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HandcuffComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is HandcuffComponent.HandcuffComponent_AutoState current))
        return;
      component.BodyIconState = current.BodyIconState;
    }
  }
}
