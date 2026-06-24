using System;
using System.Collections.Generic;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;

namespace Robust.Shared.Replays;

public sealed class ReplayData
{
	public readonly List<GameState> States;

	public readonly List<ReplayMessage> Messages;

	public readonly TimeSpan[] ReplayTime;

	public readonly GameTick TickOffset;

	public readonly TimeSpan StartTime;

	public readonly TimeSpan? Duration;

	public readonly CheckpointState[] Checkpoints;

	public readonly bool ClientSideRecording;

	public readonly MappingDataNode YamlData;

	public readonly NetUserId? Recorder;

	public ReplayMessage? InitialMessages;

	public int CurrentIndex { get; internal set; } = -1;

	public GameTick LastApplied { get; internal set; }

	public GameTick CurTick => new GameTick((uint)CurrentIndex + TickOffset.Value);

	public GameState CurState => States[CurrentIndex];

	public GameState? NextState
	{
		get
		{
			if (CurrentIndex + 1 >= States.Count)
			{
				return null;
			}
			return States[CurrentIndex + 1];
		}
	}

	public ReplayMessage CurMessages => Messages[CurrentIndex];

	public TimeSpan CurrentReplayTime => ReplayTime[CurrentIndex];

	public ReplayData(List<GameState> states, List<ReplayMessage> messages, TimeSpan[] replayTime, GameTick tickOffset, TimeSpan startTime, TimeSpan? duration, CheckpointState[] checkpointStates, ReplayMessage? initData, bool clientSideRecording, MappingDataNode yamlData)
	{
		States = states;
		Messages = messages;
		ReplayTime = replayTime;
		TickOffset = tickOffset;
		StartTime = startTime;
		Duration = duration;
		Checkpoints = checkpointStates;
		InitialMessages = initData;
		ClientSideRecording = clientSideRecording;
		YamlData = yamlData;
		if (YamlData.TryGet("recordedBy", out ValueDataNode node) && Guid.TryParse(node.Value, out var result))
		{
			Recorder = new NetUserId(result);
		}
	}
}
