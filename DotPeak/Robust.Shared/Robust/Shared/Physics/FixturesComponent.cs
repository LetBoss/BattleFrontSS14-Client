// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.FixturesComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics;

[RegisterComponent]
[NetworkedComponent]
public sealed class FixturesComponent : 
  Component,
  ISerializationGenerated<FixturesComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("fixtures", false, 1, false, false, typeof (FixtureSerializer))]
  [NeverPushInheritance]
  [Access(new Type[] {typeof (FixtureSystem)}, Other = AccessPermissions.ReadExecute)]
  public Dictionary<string, Fixture> Fixtures = new Dictionary<string, Fixture>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public int FixtureCount => this.Fixtures.Count;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FixturesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FixturesComponent) target1;
    if (serialization.TryCustomCopy<FixturesComponent>(this, ref target, hookCtx, false, context))
      return;
    if (this.Fixtures == null)
      throw new NullNotAllowedException();
    Dictionary<string, Fixture> target2 = (Dictionary<string, Fixture>) null;
    serialization.CopyTo<Dictionary<string, Fixture>, FixtureSerializer>(this.Fixtures, ref target2, hookCtx, context, true);
    Dictionary<string, Fixture> dictionary = target2;
    target.Fixtures = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FixturesComponent target,
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
    FixturesComponent target1 = (FixturesComponent) target;
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
    FixturesComponent target1 = (FixturesComponent) target;
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
    FixturesComponent target1 = (FixturesComponent) target;
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
  virtual FixturesComponent Component.Instantiate() => new FixturesComponent();
}
