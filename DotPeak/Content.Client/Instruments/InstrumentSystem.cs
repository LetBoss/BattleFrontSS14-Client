// Decompiled with JetBrains decompiler
// Type: Content.Client.Instruments.InstrumentSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Instruments;
using Robust.Client.Audio.Midi;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Midi;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable enable
namespace Content.Client.Instruments;

public sealed class InstrumentSystem : SharedInstrumentSystem
{
  [Dependency]
  private IClientNetManager _netManager;
  [Dependency]
  private IMidiManager _midiManager;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IConfigurationManager _cfg;
  public readonly TimeSpan OneSecAgo = TimeSpan.FromSeconds(-1L);
  private bool _isUpdateQueued;

  public int MaxMidiEventsPerBatch { get; private set; }

  public int MaxMidiEventsPerSecond { get; private set; }

  public event Action? OnChannelsUpdated;

  public virtual void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    EntitySystemSubscriptionExt.CVar<int>(this.Subs, this._cfg, CCVars.MaxMidiEventsPerBatch, new Action<int>(this.OnMaxMidiEventsPerBatchChanged), true);
    EntitySystemSubscriptionExt.CVar<int>(this.Subs, this._cfg, CCVars.MaxMidiEventsPerSecond, new Action<int>(this.OnMaxMidiEventsPerSecondChanged), true);
    this.SubscribeNetworkEvent<InstrumentMidiEventEvent>(new EntityEventHandler<InstrumentMidiEventEvent>(this.OnMidiEventRx), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<InstrumentStartMidiEvent>(new EntityEventHandler<InstrumentStartMidiEvent>(this.OnMidiStart), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<InstrumentStopMidiEvent>(new EntityEventHandler<InstrumentStopMidiEvent>(this.OnMidiStop), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InstrumentComponent, ComponentShutdown>(new ComponentEventHandler<InstrumentComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InstrumentComponent, ComponentHandleState>(new ComponentEventRefHandler<InstrumentComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActiveInstrumentComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<ActiveInstrumentComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnActiveInstrumentAfterHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnActiveInstrumentAfterHandleState(
    Entity<ActiveInstrumentComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this._isUpdateQueued = true;
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    if (!this._isUpdateQueued)
      return;
    this._isUpdateQueued = false;
    Action onChannelsUpdated = this.OnChannelsUpdated;
    if (onChannelsUpdated == null)
      return;
    onChannelsUpdated();
  }

  private void OnHandleState(
    EntityUid uid,
    SharedInstrumentComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is InstrumentComponentState current))
      return;
    component.Playing = current.Playing;
    component.InstrumentProgram = current.InstrumentProgram;
    component.InstrumentBank = current.InstrumentBank;
    component.AllowPercussion = current.AllowPercussion;
    component.AllowProgramChange = current.AllowProgramChange;
    component.RespectMidiLimits = current.RespectMidiLimits;
    component.Master = this.EnsureEntity<InstrumentComponent>(current.Master, uid);
    component.FilteredChannels = current.FilteredChannels;
    if (component.Playing)
      this.SetupRenderer(uid, true, component);
    else
      this.EndRenderer(uid, true, component);
  }

  private void OnShutdown(EntityUid uid, InstrumentComponent component, ComponentShutdown args)
  {
    this.EndRenderer(uid, false, (SharedInstrumentComponent) component);
  }

  public void SetMaster(EntityUid uid, EntityUid? masterUid)
  {
    if (!this.HasComp<InstrumentComponent>(uid))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new InstrumentSetMasterEvent(this.GetNetEntity(uid, (MetaDataComponent) null), this.GetNetEntity(masterUid, (MetaDataComponent) null)));
  }

  public void SetFilteredChannel(EntityUid uid, int channel, bool value)
  {
    InstrumentComponent instrumentComponent;
    if (!this.TryComp<InstrumentComponent>(uid, ref instrumentComponent))
      return;
    if (value)
      instrumentComponent.Renderer?.SendMidiEvent(RobustMidiEvent.AllNotesOff((byte) channel, 0U), false);
    this.RaiseNetworkEvent((EntityEventArgs) new InstrumentSetFilteredChannelEvent(this.GetNetEntity(uid, (MetaDataComponent) null), channel, value));
  }

  public override bool ResolveInstrument(EntityUid uid, ref SharedInstrumentComponent? component)
  {
    if (component != null)
      return true;
    InstrumentComponent instrumentComponent;
    this.TryComp<InstrumentComponent>(uid, ref instrumentComponent);
    component = (SharedInstrumentComponent) instrumentComponent;
    return component != null;
  }

  public override void SetupRenderer(
    EntityUid uid,
    bool fromStateChange,
    SharedInstrumentComponent? component = null)
  {
    if (!this.ResolveInstrument(uid, ref component))
      return;
    InstrumentComponent instrument = component as InstrumentComponent;
    if (instrument == null)
      return;
    if (instrument.IsRendererAlive)
    {
      if (!fromStateChange)
        return;
      this.UpdateRenderer(uid, instrument);
    }
    else
    {
      instrument.SequenceDelay = 0U;
      instrument.SequenceStartTick = 0U;
      instrument.Renderer = this._midiManager.GetNewRenderer(true);
      if (instrument.Renderer != null)
      {
        instrument.Renderer.SendMidiEvent(RobustMidiEvent.SystemReset(instrument.Renderer.SequencerTick), true);
        this.UpdateRenderer(uid, instrument);
        instrument.Renderer.OnMidiPlayerFinished += (Action) (() =>
        {
          instrument.PlaybackEndedInvoke();
          this.EndRenderer(uid, fromStateChange, (SharedInstrumentComponent) instrument);
        });
      }
      if (fromStateChange)
        return;
      this.RaiseNetworkEvent((EntityEventArgs) new InstrumentStartMidiEvent(this.GetNetEntity(uid, (MetaDataComponent) null)));
    }
  }

  public void UpdateRenderer(EntityUid uid, InstrumentComponent? instrument = null)
  {
    if (!this.Resolve<InstrumentComponent>(uid, ref instrument, true) || instrument.Renderer == null)
      return;
    instrument.Renderer.TrackingEntity = new EntityUid?(uid);
    instrument.Renderer.FilteredChannels.SetAll(false);
    instrument.Renderer.FilteredChannels.Or(instrument.FilteredChannels);
    instrument.Renderer.DisablePercussionChannel = !instrument.AllowPercussion;
    instrument.Renderer.DisableProgramChangeEvent = !instrument.AllowProgramChange;
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      if (instrument.FilteredChannels[index])
        instrument.Renderer.SendMidiEvent(RobustMidiEvent.AllNotesOff((byte) index, 0U), true);
    }
    if (!instrument.AllowProgramChange)
    {
      instrument.Renderer.MidiBank = instrument.InstrumentBank;
      instrument.Renderer.MidiProgram = instrument.InstrumentProgram;
    }
    this.UpdateRendererMaster(instrument);
    instrument.Renderer.LoopMidi = instrument.LoopMidi;
  }

  private void UpdateRendererMaster(InstrumentComponent instrument)
  {
    if (instrument.Renderer == null)
      return;
    InstrumentComponent instrumentComponent;
    if (!instrument.Master.HasValue || !this.TryComp<InstrumentComponent>(instrument.Master, ref instrumentComponent) || instrumentComponent.Renderer == null)
      instrument.Renderer.Master = (IMidiRenderer) null;
    else
      instrument.Renderer.Master = instrumentComponent.Renderer;
  }

  public override void EndRenderer(
    EntityUid uid,
    bool fromStateChange,
    SharedInstrumentComponent? component = null)
  {
    if (!this.ResolveInstrument(uid, ref component) || !(component is InstrumentComponent instrument))
      return;
    if (instrument.IsInputOpen)
      this.CloseInput(uid, fromStateChange, instrument);
    else if (instrument.IsMidiOpen)
    {
      this.CloseMidi(uid, fromStateChange, instrument);
    }
    else
    {
      IMidiRenderer renderer = instrument.Renderer;
      if (renderer != null)
      {
        renderer.Master = (IMidiRenderer) null;
        renderer.SystemReset();
        renderer.ClearAllEvents();
        Timer.Spawn(2000, (Action) (() => ((IDisposable) renderer).Dispose()), new CancellationToken());
      }
      instrument.Renderer = (IMidiRenderer) null;
      instrument.MidiEventBuffer.Clear();
      if (fromStateChange || !((INetManager) this._netManager).IsConnected)
        return;
      this.RaiseNetworkEvent((EntityEventArgs) new InstrumentStopMidiEvent(this.GetNetEntity(uid, (MetaDataComponent) null)));
    }
  }

  public void SetPlayerTick(EntityUid uid, int playerTick, InstrumentComponent? instrument = null)
  {
    if (!this.Resolve<InstrumentComponent>(uid, ref instrument, true))
      return;
    IMidiRenderer renderer = instrument.Renderer;
    if (renderer == null || renderer.Status != 2)
      return;
    instrument.MidiEventBuffer.Clear();
    uint num = instrument.Renderer.SequencerTick - 1U;
    instrument.MidiEventBuffer.Add(RobustMidiEvent.SystemReset(num));
    instrument.Renderer.PlayerTick = playerTick;
  }

  public bool OpenInput(EntityUid uid, InstrumentComponent? instrument = null)
  {
    if (!this.Resolve<InstrumentComponent>(uid, ref instrument, false))
      return false;
    this.SetupRenderer(uid, false, (SharedInstrumentComponent) instrument);
    if (instrument.Renderer == null || !instrument.Renderer.OpenInput())
      return false;
    this.SetMaster(uid, new EntityUid?());
    instrument.MidiEventBuffer.Clear();
    instrument.Renderer.OnMidiEvent += new Action<RobustMidiEvent>(instrument.MidiEventBuffer.Add);
    return true;
  }

  [Obsolete("Use overload that takes in byte[] instead.")]
  public bool OpenMidi(EntityUid uid, ReadOnlySpan<byte> data, InstrumentComponent? instrument = null)
  {
    return this.OpenMidi(uid, data.ToArray(), instrument);
  }

  public bool OpenMidi(EntityUid uid, byte[] data, InstrumentComponent? instrument = null)
  {
    if (!this.Resolve<InstrumentComponent>(uid, ref instrument, true))
      return false;
    this.SetupRenderer(uid, false, (SharedInstrumentComponent) instrument);
    if (instrument.Renderer == null || !instrument.Renderer.OpenMidi((ReadOnlySpan<byte>) data))
      return false;
    this.SetMaster(uid, new EntityUid?());
    this.TrySetChannels(uid, data);
    instrument.MidiEventBuffer.Clear();
    instrument.Renderer.OnMidiEvent += new Action<RobustMidiEvent>(instrument.MidiEventBuffer.Add);
    return true;
  }

  public bool CloseInput(EntityUid uid, bool fromStateChange, InstrumentComponent? instrument = null)
  {
    if (!this.Resolve<InstrumentComponent>(uid, ref instrument, true) || instrument.Renderer == null || !instrument.Renderer.CloseInput())
      return false;
    this.EndRenderer(uid, fromStateChange, (SharedInstrumentComponent) instrument);
    return true;
  }

  public bool CloseMidi(EntityUid uid, bool fromStateChange, InstrumentComponent? instrument = null)
  {
    if (!this.Resolve<InstrumentComponent>(uid, ref instrument, true) || instrument.Renderer == null || !instrument.Renderer.CloseMidi())
      return false;
    this.EndRenderer(uid, fromStateChange, (SharedInstrumentComponent) instrument);
    return true;
  }

  private void OnMaxMidiEventsPerSecondChanged(int obj) => this.MaxMidiEventsPerSecond = obj;

  private void OnMaxMidiEventsPerBatchChanged(int obj) => this.MaxMidiEventsPerBatch = obj;

  private void OnMidiEventRx(InstrumentMidiEventEvent midiEv)
  {
    EntityUid entity = this.GetEntity(midiEv.Uid);
    InstrumentComponent instrument;
    if (!this.TryComp<InstrumentComponent>(entity, ref instrument))
      return;
    IMidiRenderer renderer = instrument.Renderer;
    if (renderer != null)
    {
      if (instrument.IsInputOpen || instrument.IsMidiOpen)
        return;
      if (instrument.SequenceStartTick <= 0U)
        instrument.SequenceStartTick = ((IEnumerable<RobustMidiEvent>) midiEv.MidiEvent).Min<RobustMidiEvent, uint>((Func<RobustMidiEvent, uint>) (x => x.Tick)) - 1U;
      INetChannel serverChannel = this._netManager.ServerChannel;
      float num = MathF.Sqrt((serverChannel != null ? (float) serverChannel.Ping : 0.0f) / 1000f);
      uint val2 = (uint) (renderer.SequencerTimeScale * (0.2 + (double) num)) - instrument.SequenceStartTick;
      instrument.SequenceDelay = Math.Max(instrument.SequenceDelay, val2);
      this.SendMidiEvents((IReadOnlyList<RobustMidiEvent>) midiEv.MidiEvent, instrument);
    }
    else
    {
      if (instrument.SequenceStartTick != 0U)
        return;
      this.SetupRenderer(entity, true, (SharedInstrumentComponent) instrument);
    }
  }

  private void SendMidiEvents(
    IReadOnlyList<RobustMidiEvent> midiEvents,
    InstrumentComponent instrument)
  {
    if (instrument.Renderer == null)
    {
      this.Log.Warning("Tried to send Midi events to an instrument without a renderer.");
    }
    else
    {
      uint sequencerTick = instrument.Renderer.SequencerTick;
      for (uint index = 0; (long) index < (long) midiEvents.Count; ++index)
      {
        RobustMidiEvent midiEvent = midiEvents[(int) index];
        uint num = midiEvent.Tick + instrument.SequenceDelay;
        if (num < sequencerTick)
        {
          instrument.SequenceDelay += sequencerTick - midiEvent.Tick;
          num = midiEvent.Tick + instrument.SequenceDelay;
        }
        instrument.Renderer?.ScheduleMidiEvent(midiEvent, num + index, true);
      }
    }
  }

  private void OnMidiStart(InstrumentStartMidiEvent ev)
  {
    this.SetupRenderer(this.GetEntity(ev.Uid), true, (SharedInstrumentComponent) null);
  }

  private void OnMidiStop(InstrumentStopMidiEvent ev)
  {
    this.EndRenderer(this.GetEntity(ev.Uid), true, (SharedInstrumentComponent) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._gameTiming.IsFirstTimePredicted)
      return;
    EntityQueryEnumerator<InstrumentComponent> entityQueryEnumerator = this.EntityQueryEnumerator<InstrumentComponent>();
    EntityUid entityUid;
    InstrumentComponent instrument;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref instrument))
    {
      if (instrument != null)
      {
        IMidiRenderer renderer = instrument.Renderer;
        if (renderer != null && renderer.Master == null && instrument.Master.HasValue)
          this.UpdateRendererMaster(instrument);
      }
      if (instrument == null || instrument.IsMidiOpen || instrument.IsInputOpen)
      {
        TimeSpan realTime = this._gameTiming.RealTime;
        TimeSpan timeSpan = realTime.Add(this.OneSecAgo);
        if (instrument.LastMeasured <= timeSpan)
        {
          instrument.LastMeasured = realTime;
          instrument.SentWithinASec = 0;
        }
        if (instrument.MidiEventBuffer.Count != 0)
        {
          int count = instrument.RespectMidiLimits ? Math.Min(this.MaxMidiEventsPerBatch, this.MaxMidiEventsPerSecond - instrument.SentWithinASec) : instrument.MidiEventBuffer.Count;
          if (count > 0)
          {
            double num = !instrument.IsRendererAlive || instrument.Renderer.Status == null ? 0.0 : instrument.Renderer.SequencerTimeScale * 0.20000000298023224;
            double bufferedTick = instrument.IsRendererAlive ? (double) instrument.Renderer.SequencerTick - num : (double) int.MaxValue;
            RobustMidiEvent[] array = instrument.MidiEventBuffer.TakeWhile<RobustMidiEvent>((Func<RobustMidiEvent, bool>) (x => (double) x.Tick < bufferedTick)).Take<RobustMidiEvent>(count).ToArray<RobustMidiEvent>();
            int length = array.Length;
            if (length != 0)
            {
              this.RaiseNetworkEvent((EntityEventArgs) new InstrumentMidiEventEvent(this.GetNetEntity(entityUid, (MetaDataComponent) null), array));
              instrument.SentWithinASec += length;
              instrument.MidiEventBuffer.RemoveRange(0, length);
            }
          }
        }
      }
    }
  }

  private bool TrySetChannels(EntityUid uid, byte[] data)
  {
    MidiTrack[] tracks;
    string error;
    if (!Content.Client.Instruments.MidiParser.MidiParser.TryGetMidiTracks(data, out tracks, out error))
    {
      this.Log.Error(error);
      return false;
    }
    List<MidiTrack> source = new List<MidiTrack>();
    for (int index = 0; index < tracks.Length; ++index)
    {
      MidiTrack midiTrack = tracks[index];
      if (midiTrack == null || midiTrack.TrackName != null || midiTrack.ProgramName != null || midiTrack.InstrumentName != null)
      {
        if (midiTrack != null)
        {
          if (midiTrack.TrackName == null)
          {
            if (midiTrack.ProgramName == null)
              goto label_10;
          }
          else if (midiTrack.ProgramName == null)
          {
            string instrumentName = midiTrack.InstrumentName;
          }
          source.Add(midiTrack);
          goto label_11;
        }
label_10:
        source.Add((MidiTrack) null);
label_11:
        this.Log.Debug($"Channel name: {source.Last<MidiTrack>()}");
      }
    }
    this.RaiseNetworkEvent((EntityEventArgs) new InstrumentSetChannelsEvent(this.GetNetEntity(uid, (MetaDataComponent) null), source.Take<MidiTrack>(16 /*0x10*/).ToArray<MidiTrack>()));
    this.Log.Debug($"Resolved {source.Count} channels.");
    return true;
  }
}
