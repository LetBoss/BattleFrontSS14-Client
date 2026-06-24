// Decompiled with JetBrains decompiler
// Type: Content.Shared.GameTicking.SharedGameTicker
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.GameTicking;

public abstract class SharedGameTicker : EntitySystem
{
  [Dependency]
  private IReplayRecordingManager _replay;
  [Dependency]
  private IGameTiming _gameTiming;
  public static readonly ProtoId<JobPrototype> FallbackOverflowJob = (ProtoId<JobPrototype>) "CMRifleman";
  public const string FallbackOverflowJobName = "cm-job-name-rifleman";

  [Robust.Shared.ViewVariables.ViewVariables]
  public int RoundId { get; protected set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan RoundStartTimeSpan { get; protected set; }

  public override void Initialize()
  {
    base.Initialize();
    this._replay.RecordingStarted += new Action<MappingDataNode, List<object>>(this.OnRecordingStart);
  }

  public override void Shutdown()
  {
    this._replay.RecordingStarted -= new Action<MappingDataNode, List<object>>(this.OnRecordingStart);
  }

  private void OnRecordingStart(MappingDataNode metadata, List<object> events)
  {
    if (this.RoundId == 0)
      return;
    metadata["roundId"] = (DataNode) new ValueDataNode(this.RoundId.ToString());
  }

  public TimeSpan RoundDuration() => this._gameTiming.CurTime.Subtract(this.RoundStartTimeSpan);
}
