// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IComponentDebug
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

public interface IComponentDebug : 
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated,
  ISerializationGenerated<IComponentDebug>
{
  string GetDebugString();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void InternalCopy(
    ref IComponentDebug target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<IComponentDebug>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref IComponentDebug target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IComponentDebug target1 = (IComponentDebug) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IComponentDebug target1 = (IComponentDebug) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  new IComponentDebug Instantiate() => throw new NotImplementedException();

  IComponent IComponent.Instantiate() => (IComponent) this.Instantiate();

  IComponent ISerializationGenerated<IComponent>.Instantiate() => (IComponent) this.Instantiate();
}
