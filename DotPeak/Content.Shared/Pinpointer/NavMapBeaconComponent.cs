// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pinpointer.NavMapBeaconComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Pinpointer;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedNavMapSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class NavMapBeaconComponent : 
  Component,
  ISerializationGenerated<NavMapBeaconComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Text;
  [DataField(null, false, 1, false, false, null)]
  public LocId? DefaultText;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color Color = Color.Orange;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NavMapBeaconComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NavMapBeaconComponent) target1;
    if (serialization.TryCustomCopy<NavMapBeaconComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Text, ref target2, hookCtx, false, context))
      target2 = this.Text;
    target.Text = target2;
    LocId? target3 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.DefaultText, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId?>(this.DefaultText, hookCtx, context);
    target.DefaultText = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target5, hookCtx, false, context))
      target5 = this.Enabled;
    target.Enabled = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NavMapBeaconComponent target,
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
    NavMapBeaconComponent target1 = (NavMapBeaconComponent) target;
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
    NavMapBeaconComponent target1 = (NavMapBeaconComponent) target;
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
    NavMapBeaconComponent target1 = (NavMapBeaconComponent) target;
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
  virtual NavMapBeaconComponent Component.Instantiate() => new NavMapBeaconComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NavMapBeaconComponent_AutoState : IComponentState
  {
    public string? Text;
    public Color Color;
    public bool Enabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NavMapBeaconComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NavMapBeaconComponent, ComponentGetState>(new ComponentEventRefHandler<NavMapBeaconComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NavMapBeaconComponent, ComponentHandleState>(new ComponentEventRefHandler<NavMapBeaconComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NavMapBeaconComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NavMapBeaconComponent.NavMapBeaconComponent_AutoState()
      {
        Text = component.Text,
        Color = component.Color,
        Enabled = component.Enabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NavMapBeaconComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NavMapBeaconComponent.NavMapBeaconComponent_AutoState current))
        return;
      component.Text = current.Text;
      component.Color = current.Color;
      component.Enabled = current.Enabled;
    }
  }
}
