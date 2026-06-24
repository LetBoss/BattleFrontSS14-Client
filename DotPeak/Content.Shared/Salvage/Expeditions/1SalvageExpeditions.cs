// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Expeditions.SalvageExpeditionConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Expeditions;

[RegisterComponent]
[NetworkedComponent]
public sealed class SalvageExpeditionConsoleComponent : 
  Component,
  ISerializationGenerated<SalvageExpeditionConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier PrintSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/terminal_insert_disc.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SalvageExpeditionConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SalvageExpeditionConsoleComponent) target1;
    if (serialization.TryCustomCopy<SalvageExpeditionConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.PrintSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PrintSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.PrintSound, hookCtx, context);
    target.PrintSound = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SalvageExpeditionConsoleComponent target,
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
    SalvageExpeditionConsoleComponent target1 = (SalvageExpeditionConsoleComponent) target;
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
    SalvageExpeditionConsoleComponent target1 = (SalvageExpeditionConsoleComponent) target;
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
    SalvageExpeditionConsoleComponent target1 = (SalvageExpeditionConsoleComponent) target;
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
  virtual SalvageExpeditionConsoleComponent Component.Instantiate()
  {
    return new SalvageExpeditionConsoleComponent();
  }
}
