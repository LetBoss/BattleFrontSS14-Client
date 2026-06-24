using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Content.Client.Instruments.MidiParser;
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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _cfg, CCVars.MaxMidiEventsPerBatch, (Action<int>)OnMaxMidiEventsPerBatchChanged, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _cfg, CCVars.MaxMidiEventsPerSecond, (Action<int>)OnMaxMidiEventsPerSecondChanged, true);
		((EntitySystem)this).SubscribeNetworkEvent<InstrumentMidiEventEvent>((EntityEventHandler<InstrumentMidiEventEvent>)OnMidiEventRx, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<InstrumentStartMidiEvent>((EntityEventHandler<InstrumentStartMidiEvent>)OnMidiStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<InstrumentStopMidiEvent>((EntityEventHandler<InstrumentStopMidiEvent>)OnMidiStop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InstrumentComponent, ComponentShutdown>((ComponentEventHandler<InstrumentComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InstrumentComponent, ComponentHandleState>((ComponentEventRefHandler<InstrumentComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveInstrumentComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ActiveInstrumentComponent, AfterAutoHandleStateEvent>)OnActiveInstrumentAfterHandleState, (Type[])null, (Type[])null);
	}

	private void OnActiveInstrumentAfterHandleState(Entity<ActiveInstrumentComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		_isUpdateQueued = true;
	}

	public override void FrameUpdate(float frameTime)
	{
		((EntitySystem)this).FrameUpdate(frameTime);
		if (_isUpdateQueued)
		{
			_isUpdateQueued = false;
			this.OnChannelsUpdated?.Invoke();
		}
	}

	private void OnHandleState(EntityUid uid, SharedInstrumentComponent component, ref ComponentHandleState args)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is InstrumentComponentState instrumentComponentState)
		{
			component.Playing = instrumentComponentState.Playing;
			component.InstrumentProgram = instrumentComponentState.InstrumentProgram;
			component.InstrumentBank = instrumentComponentState.InstrumentBank;
			component.AllowPercussion = instrumentComponentState.AllowPercussion;
			component.AllowProgramChange = instrumentComponentState.AllowProgramChange;
			component.RespectMidiLimits = instrumentComponentState.RespectMidiLimits;
			component.Master = ((EntitySystem)this).EnsureEntity<InstrumentComponent>(instrumentComponentState.Master, uid);
			component.FilteredChannels = instrumentComponentState.FilteredChannels;
			if (component.Playing)
			{
				SetupRenderer(uid, fromStateChange: true, component);
			}
			else
			{
				EndRenderer(uid, fromStateChange: true, component);
			}
		}
	}

	private void OnShutdown(EntityUid uid, InstrumentComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		EndRenderer(uid, fromStateChange: false, component);
	}

	public void SetMaster(EntityUid uid, EntityUid? masterUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<InstrumentComponent>(uid))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new InstrumentSetMasterEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(masterUid, (MetaDataComponent)null)));
		}
	}

	public void SetFilteredChannel(EntityUid uid, int channel, bool value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		InstrumentComponent instrumentComponent = default(InstrumentComponent);
		if (!((EntitySystem)this).TryComp<InstrumentComponent>(uid, ref instrumentComponent))
		{
			return;
		}
		if (value)
		{
			IMidiRenderer? renderer = instrumentComponent.Renderer;
			if (renderer != null)
			{
				renderer.SendMidiEvent(RobustMidiEvent.AllNotesOff((byte)channel, 0u), false);
			}
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new InstrumentSetFilteredChannelEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), channel, value));
	}

	public override bool ResolveInstrument(EntityUid uid, ref SharedInstrumentComponent? component)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (component != null)
		{
			return true;
		}
		InstrumentComponent instrumentComponent = default(InstrumentComponent);
		((EntitySystem)this).TryComp<InstrumentComponent>(uid, ref instrumentComponent);
		component = instrumentComponent;
		return component != null;
	}

	public override void SetupRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent? component = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveInstrument(uid, ref component))
		{
			return;
		}
		InstrumentComponent instrument = component as InstrumentComponent;
		if (instrument == null)
		{
			return;
		}
		if (instrument.IsRendererAlive)
		{
			if (fromStateChange)
			{
				UpdateRenderer(uid, instrument);
			}
			return;
		}
		instrument.SequenceDelay = 0u;
		instrument.SequenceStartTick = 0u;
		instrument.Renderer = _midiManager.GetNewRenderer(true);
		if (instrument.Renderer != null)
		{
			instrument.Renderer.SendMidiEvent(RobustMidiEvent.SystemReset(instrument.Renderer.SequencerTick), true);
			UpdateRenderer(uid, instrument);
			instrument.Renderer.OnMidiPlayerFinished += delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				instrument.PlaybackEndedInvoke();
				EndRenderer(uid, fromStateChange, instrument);
			};
		}
		if (!fromStateChange)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new InstrumentStartMidiEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null)));
		}
	}

	public void UpdateRenderer(EntityUid uid, InstrumentComponent? instrument = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InstrumentComponent>(uid, ref instrument, true) || instrument.Renderer == null)
		{
			return;
		}
		instrument.Renderer.TrackingEntity = uid;
		instrument.Renderer.FilteredChannels.SetAll(value: false);
		instrument.Renderer.FilteredChannels.Or(instrument.FilteredChannels);
		instrument.Renderer.DisablePercussionChannel = !instrument.AllowPercussion;
		instrument.Renderer.DisableProgramChangeEvent = !instrument.AllowProgramChange;
		for (int i = 0; i < 16; i++)
		{
			if (instrument.FilteredChannels[i])
			{
				instrument.Renderer.SendMidiEvent(RobustMidiEvent.AllNotesOff((byte)i, 0u), true);
			}
		}
		if (!instrument.AllowProgramChange)
		{
			instrument.Renderer.MidiBank = instrument.InstrumentBank;
			instrument.Renderer.MidiProgram = instrument.InstrumentProgram;
		}
		UpdateRendererMaster(instrument);
		instrument.Renderer.LoopMidi = instrument.LoopMidi;
	}

	private void UpdateRendererMaster(InstrumentComponent instrument)
	{
		if (instrument.Renderer != null)
		{
			InstrumentComponent instrumentComponent = default(InstrumentComponent);
			if (!instrument.Master.HasValue || !((EntitySystem)this).TryComp<InstrumentComponent>(instrument.Master, ref instrumentComponent) || instrumentComponent.Renderer == null)
			{
				instrument.Renderer.Master = null;
			}
			else
			{
				instrument.Renderer.Master = instrumentComponent.Renderer;
			}
		}
	}

	public override void EndRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent? component = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveInstrument(uid, ref component) || !(component is InstrumentComponent instrumentComponent))
		{
			return;
		}
		if (instrumentComponent.IsInputOpen)
		{
			CloseInput(uid, fromStateChange, instrumentComponent);
			return;
		}
		if (instrumentComponent.IsMidiOpen)
		{
			CloseMidi(uid, fromStateChange, instrumentComponent);
			return;
		}
		IMidiRenderer renderer = instrumentComponent.Renderer;
		if (renderer != null)
		{
			renderer.Master = null;
			renderer.SystemReset();
			renderer.ClearAllEvents();
			Timer.Spawn(2000, (Action)delegate
			{
				((IDisposable)renderer).Dispose();
			}, default(CancellationToken));
		}
		instrumentComponent.Renderer = null;
		instrumentComponent.MidiEventBuffer.Clear();
		if (!fromStateChange && ((INetManager)_netManager).IsConnected)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new InstrumentStopMidiEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null)));
		}
	}

	public void SetPlayerTick(EntityUid uid, int playerTick, InstrumentComponent? instrument = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<InstrumentComponent>(uid, ref instrument, true))
		{
			IMidiRenderer renderer = instrument.Renderer;
			if (renderer != null && (int)renderer.Status == 2)
			{
				instrument.MidiEventBuffer.Clear();
				uint num = instrument.Renderer.SequencerTick - 1;
				instrument.MidiEventBuffer.Add(RobustMidiEvent.SystemReset(num));
				instrument.Renderer.PlayerTick = playerTick;
			}
		}
	}

	public bool OpenInput(EntityUid uid, InstrumentComponent? instrument = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InstrumentComponent>(uid, ref instrument, false))
		{
			return false;
		}
		SetupRenderer(uid, fromStateChange: false, instrument);
		if (instrument.Renderer == null || !instrument.Renderer.OpenInput())
		{
			return false;
		}
		SetMaster(uid, null);
		instrument.MidiEventBuffer.Clear();
		instrument.Renderer.OnMidiEvent += instrument.MidiEventBuffer.Add;
		return true;
	}

	[Obsolete("Use overload that takes in byte[] instead.")]
	public bool OpenMidi(EntityUid uid, ReadOnlySpan<byte> data, InstrumentComponent? instrument = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return OpenMidi(uid, data.ToArray(), instrument);
	}

	public bool OpenMidi(EntityUid uid, byte[] data, InstrumentComponent? instrument = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InstrumentComponent>(uid, ref instrument, true))
		{
			return false;
		}
		SetupRenderer(uid, fromStateChange: false, instrument);
		if (instrument.Renderer == null || !instrument.Renderer.OpenMidi((ReadOnlySpan<byte>)data))
		{
			return false;
		}
		SetMaster(uid, null);
		TrySetChannels(uid, data);
		instrument.MidiEventBuffer.Clear();
		instrument.Renderer.OnMidiEvent += instrument.MidiEventBuffer.Add;
		return true;
	}

	public bool CloseInput(EntityUid uid, bool fromStateChange, InstrumentComponent? instrument = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InstrumentComponent>(uid, ref instrument, true))
		{
			return false;
		}
		if (instrument.Renderer == null || !instrument.Renderer.CloseInput())
		{
			return false;
		}
		EndRenderer(uid, fromStateChange, instrument);
		return true;
	}

	public bool CloseMidi(EntityUid uid, bool fromStateChange, InstrumentComponent? instrument = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InstrumentComponent>(uid, ref instrument, true))
		{
			return false;
		}
		if (instrument.Renderer == null || !instrument.Renderer.CloseMidi())
		{
			return false;
		}
		EndRenderer(uid, fromStateChange, instrument);
		return true;
	}

	private void OnMaxMidiEventsPerSecondChanged(int obj)
	{
		MaxMidiEventsPerSecond = obj;
	}

	private void OnMaxMidiEventsPerBatchChanged(int obj)
	{
		MaxMidiEventsPerBatch = obj;
	}

	private void OnMidiEventRx(InstrumentMidiEventEvent midiEv)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(midiEv.Uid);
		InstrumentComponent instrumentComponent = default(InstrumentComponent);
		if (!((EntitySystem)this).TryComp<InstrumentComponent>(entity, ref instrumentComponent))
		{
			return;
		}
		IMidiRenderer renderer = instrumentComponent.Renderer;
		if (renderer != null)
		{
			if (instrumentComponent.IsInputOpen || instrumentComponent.IsMidiOpen)
			{
				return;
			}
			if (instrumentComponent.SequenceStartTick == 0)
			{
				instrumentComponent.SequenceStartTick = midiEv.MidiEvent.Min((RobustMidiEvent x) => x.Tick) - 1;
			}
			INetChannel serverChannel = _netManager.ServerChannel;
			float num = MathF.Sqrt((float)((serverChannel != null) ? serverChannel.Ping : 0) / 1000f);
			uint val = (uint)(renderer.SequencerTimeScale * (0.2 + (double)num)) - instrumentComponent.SequenceStartTick;
			instrumentComponent.SequenceDelay = Math.Max(instrumentComponent.SequenceDelay, val);
			SendMidiEvents(midiEv.MidiEvent, instrumentComponent);
		}
		else if (instrumentComponent.SequenceStartTick == 0)
		{
			SetupRenderer(entity, fromStateChange: true, instrumentComponent);
		}
	}

	private void SendMidiEvents(IReadOnlyList<RobustMidiEvent> midiEvents, InstrumentComponent instrument)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (instrument.Renderer == null)
		{
			((EntitySystem)this).Log.Warning("Tried to send Midi events to an instrument without a renderer.");
			return;
		}
		uint sequencerTick = instrument.Renderer.SequencerTick;
		for (uint num = 0u; num < midiEvents.Count; num++)
		{
			RobustMidiEvent val = midiEvents[(int)num];
			uint num2 = val.Tick + instrument.SequenceDelay;
			if (num2 < sequencerTick)
			{
				instrument.SequenceDelay += sequencerTick - val.Tick;
				num2 = val.Tick + instrument.SequenceDelay;
			}
			IMidiRenderer? renderer = instrument.Renderer;
			if (renderer != null)
			{
				renderer.ScheduleMidiEvent(val, num2 + num, true);
			}
		}
	}

	private void OnMidiStart(InstrumentStartMidiEvent ev)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SetupRenderer(((EntitySystem)this).GetEntity(ev.Uid), fromStateChange: true);
	}

	private void OnMidiStop(InstrumentStopMidiEvent ev)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		EndRenderer(((EntitySystem)this).GetEntity(ev.Uid), fromStateChange: true);
	}

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_gameTiming.IsFirstTimePredicted)
		{
			return;
		}
		EntityQueryEnumerator<InstrumentComponent> val = ((EntitySystem)this).EntityQueryEnumerator<InstrumentComponent>();
		EntityUid val2 = default(EntityUid);
		InstrumentComponent instrumentComponent = default(InstrumentComponent);
		while (val.MoveNext(ref val2, ref instrumentComponent))
		{
			if (instrumentComponent != null)
			{
				IMidiRenderer renderer = instrumentComponent.Renderer;
				if (renderer != null && renderer.Master == null && instrumentComponent.Master.HasValue)
				{
					UpdateRendererMaster(instrumentComponent);
				}
			}
			if (instrumentComponent != null && !instrumentComponent.IsMidiOpen && !instrumentComponent.IsInputOpen)
			{
				continue;
			}
			TimeSpan realTime = _gameTiming.RealTime;
			TimeSpan timeSpan = realTime.Add(OneSecAgo);
			if (instrumentComponent.LastMeasured <= timeSpan)
			{
				instrumentComponent.LastMeasured = realTime;
				instrumentComponent.SentWithinASec = 0;
			}
			if (instrumentComponent.MidiEventBuffer.Count == 0)
			{
				continue;
			}
			int num = (instrumentComponent.RespectMidiLimits ? Math.Min(MaxMidiEventsPerBatch, MaxMidiEventsPerSecond - instrumentComponent.SentWithinASec) : instrumentComponent.MidiEventBuffer.Count);
			if (num > 0)
			{
				double num2 = ((instrumentComponent.IsRendererAlive && (int)instrumentComponent.Renderer.Status != 0) ? (instrumentComponent.Renderer.SequencerTimeScale * 0.20000000298023224) : 0.0);
				double bufferedTick = (instrumentComponent.IsRendererAlive ? ((double)instrumentComponent.Renderer.SequencerTick - num2) : 2147483647.0);
				RobustMidiEvent[] array = instrumentComponent.MidiEventBuffer.TakeWhile((RobustMidiEvent x) => (double)x.Tick < bufferedTick).Take(num).ToArray();
				int num3 = array.Length;
				if (num3 != 0)
				{
					((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new InstrumentMidiEventEvent(((EntitySystem)this).GetNetEntity(val2, (MetaDataComponent)null), array));
					instrumentComponent.SentWithinASec += num3;
					instrumentComponent.MidiEventBuffer.RemoveRange(0, num3);
				}
			}
		}
	}

	private bool TrySetChannels(EntityUid uid, byte[] data)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (!Content.Client.Instruments.MidiParser.MidiParser.TryGetMidiTracks(data, out MidiTrack[] tracks, out string error))
		{
			((EntitySystem)this).Log.Error(error);
			return false;
		}
		List<MidiTrack> list = new List<MidiTrack>();
		foreach (MidiTrack midiTrack in tracks)
		{
			if (midiTrack != null && midiTrack.TrackName == null && midiTrack.ProgramName == null && midiTrack.InstrumentName == null)
			{
				continue;
			}
			if (midiTrack == null)
			{
				goto IL_007e;
			}
			if (midiTrack.TrackName == null)
			{
				if (midiTrack.ProgramName == null)
				{
					goto IL_007e;
				}
			}
			else if (midiTrack.ProgramName == null)
			{
				_ = midiTrack.InstrumentName;
			}
			list.Add(midiTrack);
			goto IL_0085;
			IL_0085:
			((EntitySystem)this).Log.Debug($"Channel name: {list.Last()}");
			continue;
			IL_007e:
			list.Add(null);
			goto IL_0085;
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new InstrumentSetChannelsEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), list.Take(16).ToArray()));
		((EntitySystem)this).Log.Debug($"Resolved {list.Count} channels.");
		return true;
	}
}
