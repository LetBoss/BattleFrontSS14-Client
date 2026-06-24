// Decompiled with JetBrains decompiler
// Type: Content.Shared.MagicMirror.MagicMirrorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.MagicMirror;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MagicMirrorComponent : 
  Component,
  ISerializationGenerated<MagicMirrorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public DoAfterId? DoAfter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Target;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan AddSlotTime = TimeSpan.FromSeconds(7L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan RemoveSlotTime = TimeSpan.FromSeconds(7L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan SelectSlotTime = TimeSpan.FromSeconds(7L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan ChangeSlotTime = TimeSpan.FromSeconds(7L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public SoundSpecifier? ChangeHairSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/scissors.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MagicMirrorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MagicMirrorComponent) target1;
    if (serialization.TryCustomCopy<MagicMirrorComponent>(this, ref target, hookCtx, false, context))
      return;
    DoAfterId? target2 = new DoAfterId?();
    if (!serialization.TryCustomCopy<DoAfterId?>(this.DoAfter, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<DoAfterId?>(this.DoAfter, hookCtx, context);
    target.DoAfter = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context);
    target.Target = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AddSlotTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.AddSlotTime, hookCtx, context);
    target.AddSlotTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RemoveSlotTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.RemoveSlotTime, hookCtx, context);
    target.RemoveSlotTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SelectSlotTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.SelectSlotTime, hookCtx, context);
    target.SelectSlotTime = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ChangeSlotTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.ChangeSlotTime, hookCtx, context);
    target.ChangeSlotTime = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ChangeHairSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.ChangeHairSound, hookCtx, context);
    target.ChangeHairSound = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MagicMirrorComponent target,
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
    MagicMirrorComponent target1 = (MagicMirrorComponent) target;
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
    MagicMirrorComponent target1 = (MagicMirrorComponent) target;
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
    MagicMirrorComponent target1 = (MagicMirrorComponent) target;
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
  virtual MagicMirrorComponent Component.Instantiate() => new MagicMirrorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MagicMirrorComponent_AutoState : IComponentState
  {
    public NetEntity? Target;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MagicMirrorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MagicMirrorComponent, ComponentGetState>(new ComponentEventRefHandler<MagicMirrorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MagicMirrorComponent, ComponentHandleState>(new ComponentEventRefHandler<MagicMirrorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MagicMirrorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MagicMirrorComponent.MagicMirrorComponent_AutoState()
      {
        Target = this.GetNetEntity(component.Target)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MagicMirrorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MagicMirrorComponent.MagicMirrorComponent_AutoState current))
        return;
      component.Target = this.EnsureEntity<MagicMirrorComponent>(current.Target, uid);
    }
  }
}
