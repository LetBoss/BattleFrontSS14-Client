using System;
using System.Collections.Generic;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.GameTicking;

public abstract class SharedGameTicker : EntitySystem
{
	[Dependency]
	private IReplayRecordingManager _replay;

	[Dependency]
	private IGameTiming _gameTiming;

	public static readonly ProtoId<JobPrototype> FallbackOverflowJob = ProtoId<JobPrototype>.op_Implicit("CMRifleman");

	public const string FallbackOverflowJobName = "cm-job-name-rifleman";

	[ViewVariables]
	public int RoundId { get; protected set; }

	[ViewVariables]
	public TimeSpan RoundStartTimeSpan { get; protected set; }

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_replay.RecordingStarted += OnRecordingStart;
	}

	public override void Shutdown()
	{
		_replay.RecordingStarted -= OnRecordingStart;
	}

	private void OnRecordingStart(MappingDataNode metadata, List<object> events)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		if (RoundId != 0)
		{
			metadata["roundId"] = (DataNode)new ValueDataNode(RoundId.ToString());
		}
	}

	public TimeSpan RoundDuration()
	{
		return _gameTiming.CurTime.Subtract(RoundStartTimeSpan);
	}
}
