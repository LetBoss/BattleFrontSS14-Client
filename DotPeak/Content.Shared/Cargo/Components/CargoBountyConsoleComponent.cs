// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.CargoBountyConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cargo.Components;

[RegisterComponent]
[AutoGenerateComponentPause]
public sealed class CargoBountyConsoleComponent : 
  Component,
  ISerializationGenerated<CargoBountyConsoleComponent>,
  ISerializationGenerated
{
  [DataField("bountyLabelId", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string BountyLabelId = "PaperCargoBountyManifest";
  [DataField("nextPrintTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextPrintTime = TimeSpan.Zero;
  [DataField("printDelay", false, 1, false, false, null)]
  public TimeSpan PrintDelay = TimeSpan.FromSeconds(5L);
  [DataField("printSound", false, 1, false, false, null)]
  public SoundSpecifier PrintSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/printer.ogg", new AudioParams?());
  [DataField("skipSound", false, 1, false, false, null)]
  public SoundSpecifier SkipSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Cargo/ping.ogg", new AudioParams?());
  [DataField("denySound", false, 1, false, false, null)]
  public SoundSpecifier DenySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Cargo/buzz_two.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextDenySoundTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DenySoundDelay = TimeSpan.FromSeconds(2L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CargoBountyConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CargoBountyConsoleComponent) component;
    if (serialization.TryCustomCopy<CargoBountyConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.BountyLabelId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BountyLabelId, ref str, hookCtx, false, context))
      str = this.BountyLabelId;
    target.BountyLabelId = str;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPrintTime, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextPrintTime, hookCtx, context, false);
    target.NextPrintTime = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PrintDelay, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.PrintDelay, hookCtx, context, false);
    target.PrintDelay = timeSpan2;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.PrintSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PrintSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.PrintSound, hookCtx, context, false);
    target.PrintSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (this.SkipSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SkipSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.SkipSound, hookCtx, context, false);
    target.SkipSound = soundSpecifier2;
    SoundSpecifier soundSpecifier3 = (SoundSpecifier) null;
    if (this.DenySound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DenySound, ref soundSpecifier3, hookCtx, true, context))
      soundSpecifier3 = serialization.CreateCopy<SoundSpecifier>(this.DenySound, hookCtx, context, false);
    target.DenySound = soundSpecifier3;
    TimeSpan timeSpan3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDenySoundTime, ref timeSpan3, hookCtx, false, context))
      timeSpan3 = serialization.CreateCopy<TimeSpan>(this.NextDenySoundTime, hookCtx, context, false);
    target.NextDenySoundTime = timeSpan3;
    TimeSpan timeSpan4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DenySoundDelay, ref timeSpan4, hookCtx, false, context))
      timeSpan4 = serialization.CreateCopy<TimeSpan>(this.DenySoundDelay, hookCtx, context, false);
    target.DenySoundDelay = timeSpan4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CargoBountyConsoleComponent target,
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
    CargoBountyConsoleComponent target1 = (CargoBountyConsoleComponent) target;
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
    CargoBountyConsoleComponent target1 = (CargoBountyConsoleComponent) target;
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
    CargoBountyConsoleComponent target1 = (CargoBountyConsoleComponent) target;
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
  virtual CargoBountyConsoleComponent Component.Instantiate() => new CargoBountyConsoleComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CargoBountyConsoleComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CargoBountyConsoleComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CargoBountyConsoleComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CargoBountyConsoleComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextDenySoundTime += args.PausedTime;
    }
  }
}
