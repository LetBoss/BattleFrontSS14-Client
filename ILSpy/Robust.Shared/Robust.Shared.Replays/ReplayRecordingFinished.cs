using Robust.Shared.ContentPack;
using Robust.Shared.Utility;

namespace Robust.Shared.Replays;

public record ReplayRecordingFinished(IWritableDirProvider Directory, ResPath Path, object? State);
