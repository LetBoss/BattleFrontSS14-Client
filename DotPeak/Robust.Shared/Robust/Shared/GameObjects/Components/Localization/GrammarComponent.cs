// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.Components.Localization.GrammarComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects.Components.Localization;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class GrammarComponent : 
  Robust.Shared.GameObjects.Component,
  ISerializationGenerated<GrammarComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, string> Attributes = new Dictionary<string, string>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public Robust.Shared.Enums.Gender? Gender
  {
    get
    {
      string str;
      return !this.Attributes.TryGetValue("gender", out str) ? new Robust.Shared.Enums.Gender?() : new Robust.Shared.Enums.Gender?(Enum.Parse<Robust.Shared.Enums.Gender>(str, true));
    }
    [Obsolete("Use GrammarSystem.SetGender instead")] set
    {
      IoCManager.Resolve<IEntityManager>().System<GrammarSystem>().SetGender((Entity<GrammarComponent>) (this.Owner, this), value);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool? ProperNoun
  {
    get
    {
      string str;
      return !this.Attributes.TryGetValue("proper", out str) ? new bool?() : new bool?(bool.Parse(str));
    }
    [Obsolete("Use GrammarSystem.SetProperNoun instead")] set
    {
      IoCManager.Resolve<IEntityManager>().System<GrammarSystem>().SetProperNoun((Entity<GrammarComponent>) (this.Owner, this), value);
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GrammarComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Robust.Shared.GameObjects.Component target1 = (Robust.Shared.GameObjects.Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GrammarComponent) target1;
    if (serialization.TryCustomCopy<GrammarComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, string> target2 = (Dictionary<string, string>) null;
    if (this.Attributes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, string>>(this.Attributes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, string>>(this.Attributes, hookCtx, context);
    target.Attributes = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GrammarComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Robust.Shared.GameObjects.Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GrammarComponent target1 = (GrammarComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Robust.Shared.GameObjects.Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GrammarComponent target1 = (GrammarComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref Robust.Shared.GameObjects.IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GrammarComponent target1 = (GrammarComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Robust.Shared.GameObjects.IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Robust.Shared.GameObjects.IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GrammarComponent Robust.Shared.GameObjects.Component.Instantiate()
  {
    return new GrammarComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GrammarComponent_AutoState : IComponentState
  {
    public Dictionary<string, string> Attributes;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GrammarComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GrammarComponent, ComponentGetState>(new ComponentEventRefHandler<GrammarComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GrammarComponent, ComponentHandleState>(new ComponentEventRefHandler<GrammarComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, GrammarComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new GrammarComponent.GrammarComponent_AutoState()
      {
        Attributes = component.Attributes
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GrammarComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GrammarComponent.GrammarComponent_AutoState current))
        return;
      component.Attributes = current.Attributes == null ? (Dictionary<string, string>) null : new Dictionary<string, string>((IDictionary<string, string>) current.Attributes);
    }
  }
}
