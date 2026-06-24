// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sprite.RandomSpriteComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Sprite;

[RegisterComponent]
[NetworkedComponent]
public sealed class RandomSpriteComponent : 
  Component,
  ISerializationGenerated<RandomSpriteComponent>,
  ISerializationGenerated
{
  [DataField("getAllGroups", false, 1, false, false, null)]
  public bool GetAllGroups;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("available", false, 1, false, false, null)]
  public List<Dictionary<string, Dictionary<string, string?>>> Available = new List<Dictionary<string, Dictionary<string, string>>>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("selected", false, 1, false, false, null)]
  public Dictionary<string, (string State, Color? Color)> Selected = new Dictionary<string, (string, Color?)>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RandomSpriteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RandomSpriteComponent) target1;
    if (serialization.TryCustomCopy<RandomSpriteComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.GetAllGroups, ref target2, hookCtx, false, context))
      target2 = this.GetAllGroups;
    target.GetAllGroups = target2;
    List<Dictionary<string, Dictionary<string, string>>> target3 = (List<Dictionary<string, Dictionary<string, string>>>) null;
    if (this.Available == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Dictionary<string, Dictionary<string, string>>>>(this.Available, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<Dictionary<string, Dictionary<string, string>>>>(this.Available, hookCtx, context);
    target.Available = target3;
    Dictionary<string, (string, Color?)> target4 = (Dictionary<string, (string, Color?)>) null;
    if (this.Selected == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, (string, Color?)>>(this.Selected, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<string, (string, Color?)>>(this.Selected, hookCtx, context);
    target.Selected = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RandomSpriteComponent target,
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
    RandomSpriteComponent target1 = (RandomSpriteComponent) target;
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
    RandomSpriteComponent target1 = (RandomSpriteComponent) target;
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
    RandomSpriteComponent target1 = (RandomSpriteComponent) target;
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
  virtual RandomSpriteComponent Component.Instantiate() => new RandomSpriteComponent();
}
