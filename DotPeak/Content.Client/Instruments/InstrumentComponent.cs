// Decompiled with JetBrains decompiler
// Type: Content.Client.Instruments.InstrumentComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Instruments;
using Robust.Client.Audio.Midi;
using Robust.Shared.Audio.Midi;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Instruments;

[RegisterComponent]
public sealed class InstrumentComponent : 
  SharedInstrumentComponent,
  ISerializationGenerated<InstrumentComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public IMidiRenderer? Renderer;
  [Robust.Shared.ViewVariables.ViewVariables]
  public uint SequenceDelay;
  [Robust.Shared.ViewVariables.ViewVariables]
  public uint SequenceStartTick;
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan LastMeasured = TimeSpan.MinValue;
  [Robust.Shared.ViewVariables.ViewVariables]
  public int SentWithinASec;
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly List<RobustMidiEvent> MidiEventBuffer = new List<RobustMidiEvent>();

  public event Action? OnMidiPlaybackEnded;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool LoopMidi { get; set; }

  [DataField("handheld", false, 1, false, false, null)]
  public bool Handheld { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsMidiOpen
  {
    get
    {
      IMidiRenderer renderer = this.Renderer;
      return renderer != null && renderer.Status == 2;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsInputOpen
  {
    get
    {
      IMidiRenderer renderer = this.Renderer;
      return renderer != null && renderer.Status == 1;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsRendererAlive => this.Renderer != null;

  [Robust.Shared.ViewVariables.ViewVariables]
  public int PlayerTotalTick
  {
    get
    {
      IMidiRenderer renderer = this.Renderer;
      return renderer == null ? 0 : renderer.PlayerTotalTick;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int PlayerTick
  {
    get
    {
      IMidiRenderer renderer = this.Renderer;
      return renderer == null ? 0 : renderer.PlayerTick;
    }
  }

  public void PlaybackEndedInvoke()
  {
    Action midiPlaybackEnded = this.OnMidiPlaybackEnded;
    if (midiPlaybackEnded == null)
      return;
    midiPlaybackEnded();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InstrumentComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedInstrumentComponent target1 = (SharedInstrumentComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InstrumentComponent) target1;
    if (serialization.TryCustomCopy<InstrumentComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Handheld, ref flag, hookCtx, false, context))
      flag = this.Handheld;
    target.Handheld = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InstrumentComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SharedInstrumentComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstrumentComponent target1 = (InstrumentComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SharedInstrumentComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstrumentComponent target1 = (InstrumentComponent) target;
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
    InstrumentComponent target1 = (InstrumentComponent) target;
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
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual InstrumentComponent SharedInstrumentComponent.Instantiate() => new InstrumentComponent();
}
