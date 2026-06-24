using Robust.Shared.Utility;

namespace Robust.Shared.Replays;

public static class ReplayConstants
{
	public const string Ext = "dat";

	public const string DataFilePrefix = "data_";

	public static readonly ResPath FileMeta = new ResPath("replay.yml");

	public static readonly ResPath FileMetaFinal = new ResPath("replay_final.yml");

	public static readonly ResPath FileCvars = new ResPath("cvars.toml");

	public static readonly ResPath FileStrings = new ResPath("strings.dat");

	public static readonly ResPath FileInit = new ResPath("init.dat");

	public static readonly ResPath ReplayZipFolder = new ResPath("_replay");

	public const string MetaKeyTypeHash = "typeHash";

	public const string MetaKeyComponentHash = "componentHash";

	public const string MetaKeyStringHash = "stringHash";

	public const string MetaKeyTime = "time";

	public const string MetaKeyName = "name";

	public const string MetaKeyStartTick = "startTick";

	public const string MetaKeyStartTime = "startTime";

	public const string MetaKeyBaseTick = "timeBaseTick";

	public const string MetaKeyBaseTime = "timeBaseTime";

	public const string MetaKeyEngineVersion = "engineVersion";

	public const string MetaKeyForkId = "buildForkId";

	public const string MetaKeyForkVersion = "buildForkVersion";

	public const string MetaKeyIsClientRecording = "isClientRecording";

	public const string MetaKeyRecordedBy = "recordedBy";

	public const string MetaFinalKeyFileCount = "fileCount";

	public const string MetaFinalKeyDuration = "duration";

	public const string MetaFinalKeyCompressedSize = "size";

	public const string MetaFinalKeyUncompressedSize = "uncompressedSize";

	public const string MetaFinalKeyEndTick = "endTick";

	public const string MetaFinalKeyEndTime = "endTime";
}
