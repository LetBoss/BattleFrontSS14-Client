// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.ReplayData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
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

  public GameTick CurTick => new GameTick((uint) this.CurrentIndex + this.TickOffset.Value);

  public GameState CurState => this.States[this.CurrentIndex];

  public GameState? NextState
  {
    get
    {
      return this.CurrentIndex + 1 >= this.States.Count ? (GameState) null : this.States[this.CurrentIndex + 1];
    }
  }

  public ReplayMessage CurMessages => this.Messages[this.CurrentIndex];

  public TimeSpan CurrentReplayTime => this.ReplayTime[this.CurrentIndex];

  public ReplayData(
    List<GameState> states,
    List<ReplayMessage> messages,
    TimeSpan[] replayTime,
    GameTick tickOffset,
    TimeSpan startTime,
    TimeSpan? duration,
    CheckpointState[] checkpointStates,
    ReplayMessage? initData,
    bool clientSideRecording,
    MappingDataNode yamlData)
  {
    this.States = states;
    this.Messages = messages;
    this.ReplayTime = replayTime;
    this.TickOffset = tickOffset;
    this.StartTime = startTime;
    this.Duration = duration;
    this.Checkpoints = checkpointStates;
    this.InitialMessages = initData;
    this.ClientSideRecording = clientSideRecording;
    this.YamlData = yamlData;
    ValueDataNode node;
    Guid result;
    if (!this.YamlData.TryGet<ValueDataNode>("recordedBy", out node) || !Guid.TryParse(node.Value, out result))
      return;
    this.Recorder = new NetUserId?(new NetUserId(result));
  }
}
