// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.Components.MeleeSpeechComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Speech.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MeleeSpeechComponent : 
  Component,
  ISerializationGenerated<MeleeSpeechComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("Battlecry", false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Battlecry;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("MaxBattlecryLength", false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxBattlecryLength = 12;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ConfigureAction = (EntProtoId) "ActionConfigureMeleeSpeech";
  [DataField("configureActionEntity", false, 1, false, false, null)]
  public EntityUid? ConfigureActionEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MeleeSpeechComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MeleeSpeechComponent) target1;
    if (serialization.TryCustomCopy<MeleeSpeechComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Battlecry, ref target2, hookCtx, false, context))
      target2 = this.Battlecry;
    target.Battlecry = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxBattlecryLength, ref target3, hookCtx, false, context))
      target3 = this.MaxBattlecryLength;
    target.MaxBattlecryLength = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ConfigureAction, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ConfigureAction, hookCtx, context);
    target.ConfigureAction = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ConfigureActionEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ConfigureActionEntity, hookCtx, context);
    target.ConfigureActionEntity = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MeleeSpeechComponent target,
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
    MeleeSpeechComponent target1 = (MeleeSpeechComponent) target;
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
    MeleeSpeechComponent target1 = (MeleeSpeechComponent) target;
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
    MeleeSpeechComponent target1 = (MeleeSpeechComponent) target;
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
  virtual MeleeSpeechComponent Component.Instantiate() => new MeleeSpeechComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MeleeSpeechComponent_AutoState : IComponentState
  {
    public string? Battlecry;
    public int MaxBattlecryLength;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MeleeSpeechComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MeleeSpeechComponent, ComponentGetState>(new ComponentEventRefHandler<MeleeSpeechComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MeleeSpeechComponent, ComponentHandleState>(new ComponentEventRefHandler<MeleeSpeechComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MeleeSpeechComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MeleeSpeechComponent.MeleeSpeechComponent_AutoState()
      {
        Battlecry = component.Battlecry,
        MaxBattlecryLength = component.MaxBattlecryLength
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MeleeSpeechComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MeleeSpeechComponent.MeleeSpeechComponent_AutoState current))
        return;
      component.Battlecry = current.Battlecry;
      component.MaxBattlecryLength = current.MaxBattlecryLength;
    }
  }
}
