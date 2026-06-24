// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.IReplayRecordingManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.ContentPack;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Markdown.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Replays;

[NotContentImplementable]
public interface IReplayRecordingManager
{
  bool CanStartRecording();

  void RecordServerMessage(object obj);

  void RecordClientMessage(object obj);

  void RecordReplayMessage(object obj);

  bool IsRecording { get; }

  object? ActiveRecordingState { get; }

  void Update(GameState? state);

  event Action<MappingDataNode, List<object>> RecordingStarted;

  event Action<MappingDataNode> RecordingStopped;

  event Action<ReplayRecordingStopped> RecordingStopped2;

  event Action<ReplayRecordingFinished> RecordingFinished;

  bool TryStartRecording(
    IWritableDirProvider directory,
    string? name = null,
    bool overwrite = false,
    TimeSpan? duration = null,
    object? state = null);

  void StopRecording();

  ReplayRecordingStats GetReplayStats();

  Task WaitWriteTasks();

  bool IsWriting();
}
