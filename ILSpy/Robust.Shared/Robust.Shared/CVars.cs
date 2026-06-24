using System;
using Robust.Shared.Configuration;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Robust.Shared;

[CVarDefs]
public abstract class CVars
{
	public static readonly CVarDef<int> NetMaxConnections = CVarDef.Create("net.max_connections", 256, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

	public static readonly CVarDef<int> NetPort = CVarDef.Create("net.port", 1212, CVar.ARCHIVE);

	public static readonly CVarDef<int> NetSendBufferSize = CVarDef.Create("net.sendbuffersize", 131071, CVar.ARCHIVE);

	public static readonly CVarDef<int> NetReceiveBufferSize = CVarDef.Create("net.receivebuffersize", 131071, CVar.ARCHIVE);

	public static readonly CVarDef<int> NetPoolSize = CVarDef.Create("net.pool_size", 512, CVar.SERVER | CVar.CLIENT);

	public static readonly CVarDef<int> NetMtu = CVarDef.Create("net.mtu", 700, CVar.ARCHIVE);

	public static readonly CVarDef<int> NetMtuIpv6 = CVarDef.Create("net.mtu_ipv6", 1232, CVar.ARCHIVE);

	public static readonly CVarDef<bool> NetMtuExpand = CVarDef.Create("net.mtu_expand", defaultValue: false, CVar.ARCHIVE);

	public static readonly CVarDef<float> NetMtuExpandFrequency = CVarDef.Create("net.mtu_expand_frequency", 2f, CVar.ARCHIVE);

	public static readonly CVarDef<int> NetMtuExpandFailAttempts = CVarDef.Create("net.mtu_expand_fail_attempts", 5, CVar.ARCHIVE);

	public static readonly CVarDef<bool> NetVerbose = CVarDef.Create("net.verbose", defaultValue: false);

	public static readonly CVarDef<string> NetBindTo = CVarDef.Create("net.bindto", "0.0.0.0,::", CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<bool> NetDualStack = CVarDef.Create("net.dualstack", defaultValue: false, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<bool> NetInterp = CVarDef.Create("net.interp", defaultValue: true, CVar.REPLICATED | CVar.ARCHIVE | CVar.CLIENT);

	public static readonly CVarDef<int> NetBufferSize = CVarDef.Create("net.buffer_size", 2, CVar.REPLICATED | CVar.ARCHIVE | CVar.CLIENT);

	public static readonly CVarDef<int> NetMaxBufferSize = CVarDef.Create("net.max_buffer_size", 512, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<bool> NetLogging = CVarDef.Create("net.logging", defaultValue: false, CVar.ARCHIVE);

	public static readonly CVarDef<bool> NetPredict = CVarDef.Create("net.predict", defaultValue: true, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<int> NetPredictTickBias = CVarDef.Create("net.predict_tick_bias", 1, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<float> NetPredictLagBias = CVarDef.Create("net.predict_lag_bias", OperatingSystem.IsWindows() ? 0.016f : 0f, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<int> NetStateBufMergeThreshold = CVarDef.Create("net.state_buf_merge_threshold", 5, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<bool> NetPVS = CVarDef.Create("net.pvs", defaultValue: true, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

	public static readonly CVarDef<int> NetPvsEntityGrowth = CVarDef.Create("net.pvs_entity_growth", 65536, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<int> NetPvsEntityInitial = CVarDef.Create("net.pvs_entity_initial", 65536, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<int> NetPvsEntityMax = CVarDef.Create("net.pvs_entity_max", 16777216, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<bool> NetPvsAsync = CVarDef.Create("net.pvs_async", defaultValue: true, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<float> NetMaxUpdateRange = CVarDef.Create("net.pvs_range", 25f, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

	public static readonly CVarDef<float> NetPvsPriorityRange = CVarDef.Create("net.pvs_priority_range", 32.5f, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

	public static readonly CVarDef<int> NetForceAckThreshold = CVarDef.Create("net.force_ack_threshold", 60, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<int> NetPVSEntityBudget = CVarDef.Create("net.pvs_budget", 50, CVar.REPLICATED | CVar.ARCHIVE | CVar.CLIENT);

	public static readonly CVarDef<int> NetPVSEntityEnterBudget = CVarDef.Create("net.pvs_enter_budget", 200, CVar.REPLICATED | CVar.ARCHIVE | CVar.CLIENT);

	public static readonly CVarDef<int> NetPVSEntityExitBudget = CVarDef.Create("net.pvs_exit_budget", 75, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<int> NetPvsCompressLevel = CVarDef.Create("net.pvs_compress_level", 3, CVar.ARCHIVE);

	public static readonly CVarDef<bool> NetLogLateMsg = CVarDef.Create("net.log_late_msg", defaultValue: true);

	public static readonly CVarDef<int> NetTickrate = CVarDef.Create("net.tickrate", 30, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

	public static readonly CVarDef<int> NetTimeStartOffset = CVarDef.Create("net.time_start_offset", 0, CVar.SERVERONLY);

	public static readonly CVarDef<float> ConnectionTimeout = CVarDef.Create("net.connection_timeout", 25f, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<float> ResendHandshakeInterval = CVarDef.Create("net.handshake_interval", 3f, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<int> MaximumHandshakeAttempts = CVarDef.Create("net.handshake_attempts", 5, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<bool> NetEncrypt = CVarDef.Create("net.encrypt", defaultValue: true, CVar.CLIENTONLY);

	public static readonly CVarDef<bool> NetUPnP = CVarDef.Create("net.upnp", defaultValue: false, CVar.SERVERONLY);

	public static readonly CVarDef<string> NetLidgrenAppIdentifier = CVarDef.Create("net.lidgren_app_identifier", "RobustToolbox");

	public static readonly CVarDef<float> NetFakeLoss = CVarDef.Create("net.fakeloss", 0f, CVar.CHEAT);

	public static readonly CVarDef<float> NetFakeLagMin = CVarDef.Create("net.fakelagmin", 0f, CVar.CHEAT);

	public static readonly CVarDef<float> NetFakeLagRand = CVarDef.Create("net.fakelagrand", 0f, CVar.CHEAT);

	public static readonly CVarDef<float> NetFakeDuplicates = CVarDef.Create("net.fakeduplicates", 0f, CVar.CHEAT);

	public static readonly CVarDef<float> NetHappyEyeballsDelay = CVarDef.Create("net.happy_eyeballs_delay", 0.025f, CVar.CLIENTONLY);

	public static readonly CVarDef<bool> NetLidgrenLogWarning = CVarDef.Create("net.lidgren_log_warning", defaultValue: true);

	public static readonly CVarDef<bool> NetLidgrenLogError = CVarDef.Create("net.lidgren_log_error", defaultValue: true);

	public static readonly CVarDef<bool> NetEncryptionThread = CVarDef.Create("net.encryption_thread", defaultValue: true);

	public static readonly CVarDef<int> NetEncryptionThreadChannelSize = CVarDef.Create("net.encryption_thread_channel_size", 16);

	public static readonly CVarDef<bool> NetHWId = CVarDef.Create("net.hwid", defaultValue: true, CVar.SERVERONLY);

	public static readonly CVarDef<bool> TransferHttp = CVarDef.Create("transfer.http", defaultValue: false, CVar.SERVERONLY);

	public static readonly CVarDef<string> TransferHttpEndpoint = CVarDef.Create("transfer.http_endpoint", "http://localhost:1212/", CVar.SERVERONLY);

	public static readonly CVarDef<int> TransferStreamLimit = CVarDef.Create("transfer.stream_limit", 10, CVar.SERVERONLY);

	internal static readonly CVarDef<bool> TransferArtificialDelay = CVarDef.Create("transfer.artificial_delay", defaultValue: false);

	public static readonly CVarDef<int> SysWinTickPeriod = CVarDef.Create("sys.win_tick_period", 3, CVar.SERVERONLY);

	public static readonly CVarDef<bool> SysProfileOpt = CVarDef.Create("sys.profile_opt", defaultValue: true);

	public static readonly CVarDef<int> SysGameThreadStackSize = CVarDef.Create("sys.game_thread_stack_size", 8388608);

	public static readonly CVarDef<int> SysGameThreadPriority = CVarDef.Create("sys.game_thread_priority", 3);

	public static readonly CVarDef<bool> SysGCCollectStart = CVarDef.Create("sys.gc_collect_start", defaultValue: true);

	public static readonly CVarDef<bool> SysPreciseSleep = CVarDef.Create("sys.precise_sleep", defaultValue: true);

	public static readonly CVarDef<bool> MetricsEnabled = CVarDef.Create("metrics.enabled", defaultValue: false, CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsHost = CVarDef.Create("metrics.host", "localhost", CVar.SERVERONLY);

	public static readonly CVarDef<int> MetricsPort = CVarDef.Create("metrics.port", 44880, CVar.SERVERONLY);

	public static readonly CVarDef<float> MetricsUpdateInterval = CVarDef.Create("metrics.update_interval", 0f, CVar.SERVERONLY);

	public static readonly CVarDef<bool> MetricsRuntime = CVarDef.Create("metrics.runtime", defaultValue: true, CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsRuntimeGc = CVarDef.Create("metrics.runtime_gc", "Counters", CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsRuntimeGcHistogram = CVarDef.Create("metrics.runtime_gc_histogram", "0.5,1.0,2.0,4.0,6.0,10.0,15.0,20.0", CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsRuntimeContention = CVarDef.Create("metrics.runtime_contention", "Counters", CVar.SERVERONLY);

	public static readonly CVarDef<int> MetricsRuntimeContentionSampleRate = CVarDef.Create("metrics.runtime_contention_sample_rate", 50, CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsRuntimeThreadPool = CVarDef.Create("metrics.runtime_thread_pool", "Counters", CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsRuntimeThreadPoolQueueHistogram = CVarDef.Create("metrics.runtime_thread_pool_queue_histogram", "0,10,30,60,120,180", CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsRuntimeJit = CVarDef.Create("metrics.runtime_jit", "Counters", CVar.SERVERONLY);

	public static readonly CVarDef<int> MetricsRuntimeJitSampleRate = CVarDef.Create("metrics.runtime_jit_sample_rate", 10, CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsRuntimeException = CVarDef.Create("metrics.runtime_exception", "Counters", CVar.SERVERONLY);

	public static readonly CVarDef<string> MetricsRuntimeSocket = CVarDef.Create("metrics.runtime_socket", "Counters", CVar.SERVERONLY);

	public static readonly CVarDef<bool> StatusEnabled = CVarDef.Create("status.enabled", defaultValue: true, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<string> StatusBind = CVarDef.Create("status.bind", "", CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<int> StatusMaxConnections = CVarDef.Create("status.max_connections", 5, CVar.SERVERONLY);

	public static readonly CVarDef<string> StatusConnectAddress = CVarDef.Create("status.connectaddress", "", CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<string> StatusPrivacyPolicyLink = CVarDef.Create("status.privacy_policy_link", "https://example.com/privacy", CVar.SERVER | CVar.REPLICATED);

	public static readonly CVarDef<string> StatusPrivacyPolicyIdentifier = CVarDef.Create("status.privacy_policy_identifier", "", CVar.SERVER | CVar.REPLICATED);

	public static readonly CVarDef<string> StatusPrivacyPolicyVersion = CVarDef.Create("status.privacy_policy_version", "", CVar.SERVER | CVar.REPLICATED);

	public static readonly CVarDef<string> BuildEngineVersion = CVarDef.Create("build.engine_version", typeof(CVars).Assembly.GetName().Version?.ToString(3) ?? string.Empty);

	public static readonly CVarDef<string> BuildForkId = CVarDef.Create("build.fork_id", "");

	public static readonly CVarDef<string> BuildVersion = CVarDef.Create("build.version", "");

	public static readonly CVarDef<string> BuildDownloadUrl = CVarDef.Create("build.download_url", string.Empty);

	public static readonly CVarDef<string> BuildManifestUrl = CVarDef.Create("build.manifest_url", string.Empty);

	public static readonly CVarDef<string> BuildManifestDownloadUrl = CVarDef.Create("build.manifest_download_url", string.Empty);

	public static readonly CVarDef<string> BuildHash = CVarDef.Create("build.hash", "");

	public static readonly CVarDef<string> BuildManifestHash = CVarDef.Create("build.manifest_hash", "");

	public static readonly CVarDef<string> EntitiesCategoryFilter = CVarDef.Create("build.entities_category_filter", "");

	public static readonly CVarDef<string> WatchdogToken = CVarDef.Create("watchdog.token", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

	public static readonly CVarDef<string> WatchdogKey = CVarDef.Create("watchdog.key", "", CVar.SERVERONLY);

	public static readonly CVarDef<string> WatchdogBaseUrl = CVarDef.Create("watchdog.baseUrl", "http://localhost:5000", CVar.SERVERONLY);

	[Obsolete("Use net.max_connections instead")]
	public static readonly CVarDef<int> GameMaxPlayers = CVarDef.Create("game.maxplayers", 0, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

	public static readonly CVarDef<string> GameHostName = CVarDef.Create("game.hostname", "MyServer", CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

	public static readonly CVarDef<string> GameDesc = CVarDef.Create("game.desc", "Just another server, don't mind me!", CVar.SERVERONLY);

	public static readonly CVarDef<bool> GameDeleteEmptyGrids = CVarDef.Create("game.delete_empty_grids", defaultValue: true, CVar.SERVER | CVar.ARCHIVE);

	public static readonly CVarDef<bool> GameAutoPauseEmpty = CVarDef.Create("game.auto_pause_empty", defaultValue: true, CVar.SERVERONLY);

	public static readonly CVarDef<float> GameTimeScale = CVarDef.Create("game.time_scale", 1f, CVar.SERVER | CVar.REPLICATED);

	public static readonly CVarDef<bool> LogEnabled = CVarDef.Create("log.enabled", defaultValue: true, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<string> LogPath = CVarDef.Create("log.path", "logs", CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<string> LogFormat = CVarDef.Create("log.format", "log_%(date)s-T%(time)s.txt", CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<LogLevel> LogLevel = CVarDef.Create("log.level", Robust.Shared.Log.LogLevel.Info, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<bool> LogRuntimeLog = CVarDef.Create("log.runtimelog", defaultValue: true, CVar.ARCHIVE | CVar.SERVERONLY);

	public static readonly CVarDef<float> MaxLightRadius = CVarDef.Create("light.max_radius", 32.1f, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<int> MaxLightCount = CVarDef.Create("light.max_light_count", 2048, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<int> MaxOccluderCount = CVarDef.Create("light.max_occluder_count", 2048, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<float> LightResolutionScale = CVarDef.Create("light.resolution_scale", 0.5f, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<int> MaxShadowcastingLights = CVarDef.Create("light.max_shadowcasting_lights", 128, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<bool> LightSoftShadows = CVarDef.Create("light.soft_shadows", defaultValue: true, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<bool> LightBlur = CVarDef.Create("light.blur", defaultValue: true, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<float> LightBlurFactor = CVarDef.Create("light.blur_factor", 0.001f, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<float> LookupEnlargementRange = CVarDef.Create("lookup.enlargement_range", 10f, CVar.CHEAT | CVar.REPLICATED | CVar.ARCHIVE);

	public static readonly CVarDef<bool> LokiEnabled = CVarDef.Create("loki.enabled", defaultValue: false, CVar.SERVERONLY);

	public static readonly CVarDef<string> LokiName = CVarDef.Create("loki.name", "", CVar.SERVERONLY);

	public static readonly CVarDef<string> LokiAddress = CVarDef.Create("loki.address", "", CVar.SERVERONLY);

	public static readonly CVarDef<string> LokiUsername = CVarDef.Create("loki.username", "", CVar.SERVERONLY);

	public static readonly CVarDef<string> LokiPassword = CVarDef.Create("loki.password", "", CVar.SERVERONLY);

	public static readonly CVarDef<int> AuthMode = CVarDef.Create("auth.mode", 1, CVar.SERVERONLY);

	public static readonly CVarDef<bool> AuthAllowLocal = CVarDef.Create("auth.allowlocal", defaultValue: true, CVar.SERVERONLY);

	public static readonly CVarDef<string> AuthServer = CVarDef.Create("auth.server", "https://auth.spacestation14.com/", CVar.SERVERONLY);

	public static readonly CVarDef<double> RenderSpriteDirectionBias = CVarDef.Create("render.sprite_direction_bias", -0.05, CVar.ARCHIVE | CVar.CLIENTONLY);

	public static readonly CVarDef<string> RenderFOVColor;

	public static readonly CVarDef<bool> RenderTileEdges;

	public static readonly CVarDef<int> DoubleClickDelay;

	public static readonly CVarDef<int> DoubleClickRange;

	public static readonly CVarDef<bool> DisplayVSync;

	public static readonly CVarDef<int> DisplayMaxFPS;

	public static readonly CVarDef<int> DisplayWindowMode;

	public static readonly CVarDef<int> DisplayWidth;

	public static readonly CVarDef<int> DisplayHeight;

	public static readonly CVarDef<float> DisplayUIScale;

	public static readonly CVarDef<int> DisplayRenderer;

	public static readonly CVarDef<bool> DisplayCompat;

	public static readonly CVarDef<int> DisplayOpenGLVersion;

	public static readonly CVarDef<bool> DisplayAngle;

	public static readonly CVarDef<bool> DisplayAngleCustomSwapChain;

	public static readonly CVarDef<bool> DisplayAngleForceEs2;

	public static readonly CVarDef<bool> DisplayAngleForce10_0;

	public static readonly CVarDef<bool> DisplayAngleDxgi1;

	public static readonly CVarDef<string> DisplayAdapter;

	public static readonly CVarDef<int> DisplayGpuPreference;

	public static readonly CVarDef<bool> DisplayEgl;

	public static readonly CVarDef<bool> DisplayAngleEs3On10_0;

	public static readonly CVarDef<int> DisplayFontDpi;

	public static readonly CVarDef<string> DisplayOGLOverrideVersion;

	public static readonly CVarDef<bool> DisplayOGLCheckErrors;

	public static readonly CVarDef<bool> DisplayForceSyncWindows;

	public static readonly CVarDef<bool> DisplayThreadWindowBlit;

	public static readonly CVarDef<bool> DisplayThreadUnlockBeforeSwap;

	public static readonly CVarDef<int> DisplayInputBufferSize;

	public static readonly CVarDef<bool> DisplayWin32Experience;

	public static readonly CVarDef<string> DisplayWindowIconSet;

	public static readonly CVarDef<string> DisplaySplashLogo;

	public static readonly CVarDef<bool> DisplayUSQWERTYHotkeys;

	public static readonly CVarDef<string> DisplayWindowingApi;

	public static readonly CVarDef<bool> DisplayWin11ImmersiveDarkMode;

	public static readonly CVarDef<bool> DisplayThreadWindowApi;

	public static readonly CVarDef<int> AudioDefaultConcurrent;

	public static readonly CVarDef<int> AudioAttenuation;

	public static readonly CVarDef<string> AudioDevice;

	public static readonly CVarDef<float> AudioMasterVolume;

	public static readonly CVarDef<float> AudioRaycastLength;

	public static readonly CVarDef<float> AudioEndBuffer;

	public static readonly CVarDef<int> AudioTickRate;

	public static readonly CVarDef<float> AudioZOffset;

	public static readonly CVarDef<string> PlayerName;

	public static readonly CVarDef<int> TargetMinimumTickrate;

	public static readonly CVarDef<bool> GenerateGridFixtures;

	public static readonly CVarDef<bool> GridSplitting;

	public static readonly CVarDef<float> GridFixtureEnlargement;

	public static readonly CVarDef<float> AngularSleepTolerance;

	public static readonly CVarDef<float> LinearSleepTolerance;

	public static readonly CVarDef<bool> SleepAllowed;

	public static readonly CVarDef<float> TimeToSleep;

	public static readonly CVarDef<int> PositionIterations;

	public static readonly CVarDef<int> VelocityIterations;

	public static readonly CVarDef<bool> WarmStarting;

	public static readonly CVarDef<bool> AutoClearForces;

	public static readonly CVarDef<float> VelocityThreshold;

	public static readonly CVarDef<float> Baumgarte;

	public static readonly CVarDef<float> MaxLinearCorrection;

	public static readonly CVarDef<float> MaxAngularCorrection;

	public static readonly CVarDef<float> MaxLinVelocity;

	public static readonly CVarDef<float> MaxAngVelocity;

	public static readonly CVarDef<string> InterfaceTheme;

	public static readonly CVarDef<bool> InterfaceAudio;

	public static readonly CVarDef<int> ResAutoScaleUpperX;

	public static readonly CVarDef<int> ResAutoScaleUpperY;

	public static readonly CVarDef<int> ResAutoScaleLowX;

	public static readonly CVarDef<int> ResAutoScaleLowY;

	public static readonly CVarDef<float> ResAutoScaleMin;

	public static readonly CVarDef<bool> ResAutoScaleEnabled;

	public static readonly CVarDef<bool> DiscordEnabled;

	public static readonly CVarDef<string> DiscordRichPresenceMainIconId;

	public static readonly CVarDef<string> DiscordRichPresenceSecondIconId;

	public static readonly CVarDef<bool> ResCheckPathCasing;

	public static readonly CVarDef<bool> ResTexturePreloadingEnabled;

	public static readonly CVarDef<int> ResRSIAtlasSize;

	public static readonly CVarDef<bool> ResTexturePreloadCache;

	public static readonly CVarDef<int> ResStreamSeekMode;

	public static readonly CVarDef<bool> ResPrototypeReloadWatch;

	public static readonly CVarDef<bool> ResCheckBadFileExtensions;

	public static readonly CVarDef<int> DebugTargetFps;

	public static readonly CVarDef<float> MidiVolume;

	public static readonly CVarDef<int> MidiParallelism;

	public static readonly CVarDef<bool> HubAdvertise;

	public static readonly CVarDef<string> HubTags;

	public static readonly CVarDef<string> HubUrls;

	public static readonly CVarDef<string> HubServerUrl;

	public static readonly CVarDef<string> HubIpifyUrl;

	public static readonly CVarDef<int> HubAdvertiseInterval;

	public static readonly CVarDef<bool> AczStreamCompress;

	public static readonly CVarDef<int> AczStreamCompressLevel;

	public static readonly CVarDef<bool> AczBlobCompress;

	public static readonly CVarDef<int> AczBlobCompressLevel;

	public static readonly CVarDef<int> AczBlobCompressSaveThreshold;

	public static readonly CVarDef<bool> AczManifestCompress;

	public static readonly CVarDef<int> AczManifestCompressLevel;

	public static readonly CVarDef<float> ConCompletionDelay;

	public static readonly CVarDef<int> ConCompletionCount;

	public static readonly CVarDef<int> ConCompletionMargin;

	public static readonly CVarDef<int> ConMaxEntries;

	public static readonly CVarDef<int> ThreadParallelCount;

	public static readonly CVarDef<bool> ProfEnabled;

	public static readonly CVarDef<int> ProfBufferSize;

	public static readonly CVarDef<int> ProfIndexSize;

	public static readonly CVarDef<string> ReplayDirectory;

	public static readonly CVarDef<long> ReplayMaxCompressedSize;

	public static readonly CVarDef<long> ReplayMaxUncompressedSize;

	public static readonly CVarDef<long> ReplayServerGCSizeThreshold;

	public static readonly CVarDef<int> ReplayTickBatchSize;

	public static readonly CVarDef<int> ReplayWriteChannelSize;

	public static readonly CVarDef<bool> ReplayServerRecordingEnabled;

	public static readonly CVarDef<bool> ReplayClientRecordingEnabled;

	public static readonly CVarDef<int> ReplayMaxScrubTime;

	public static readonly CVarDef<int> ReplaySkipThreshold;

	public static readonly CVarDef<int> CheckpointMinInterval;

	public static readonly CVarDef<int> CheckpointInterval;

	public static readonly CVarDef<int> CheckpointEntitySpawnThreshold;

	public static readonly CVarDef<int> CheckpointEntityStateThreshold;

	public static readonly CVarDef<bool> ReplayDynamicalScrubbing;

	public static readonly CVarDef<bool> ReplayMakeContentBundle;

	public static readonly CVarDef<bool> ReplayIgnoreErrors;

	public static readonly CVarDef<bool> CfgCheckUnused;

	internal static readonly CVarDef<string> CfgRollbackData;

	public static readonly CVarDef<bool> ResourceUploadingEnabled;

	public static readonly CVarDef<float> ResourceUploadingLimitMb;

	public static readonly CVarDef<bool> LaunchLauncher;

	public static readonly CVarDef<bool> LaunchContentBundle;

	public static readonly CVarDef<int> ToolshedNearbyLimit;

	public static readonly CVarDef<int> ToolshedNearbyEntitiesLimit;

	public static readonly CVarDef<int> ToolshedPrototypesAutocompleteLimit;

	public static readonly CVarDef<string> LocCultureName;

	public static readonly CVarDef<string> XamlHotReloadMarkerName;

	public static readonly CVarDef<bool> UIXamlJitPreload;

	public static readonly CVarDef<bool> FontSystem;

	public static readonly CVarDef<bool> FontWindowsDownloadable;

	public static readonly CVarDef<bool> LoadingShowBar;

	private const bool DefaultShowDebug = false;

	public static readonly CVarDef<bool> LoadingShowDebug;

	protected CVars()
	{
		throw new InvalidOperationException("This class must not be instantiated");
	}

	static CVars()
	{
		//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aed: Unknown result type (might be due to invalid IL or missing references)
		Color black = Color.Black;
		RenderFOVColor = CVarDef.Create("render.fov_color", ((Color)(ref black)).ToHex(), CVar.SERVER | CVar.REPLICATED);
		RenderTileEdges = CVarDef.Create("render.tile_edges", defaultValue: true, CVar.CLIENTONLY);
		DoubleClickDelay = CVarDef.Create("controls.double_click_delay", 250, CVar.ARCHIVE | CVar.CLIENTONLY);
		DoubleClickRange = CVarDef.Create("controls.double_click_range", 10, CVar.ARCHIVE | CVar.CLIENTONLY);
		DisplayVSync = CVarDef.Create("display.vsync", defaultValue: true, CVar.ARCHIVE | CVar.CLIENTONLY);
		DisplayMaxFPS = CVarDef.Create("display.max_fps", 0, CVar.ARCHIVE | CVar.CLIENTONLY);
		DisplayWindowMode = CVarDef.Create("display.windowmode", 0, CVar.ARCHIVE | CVar.CLIENTONLY);
		DisplayWidth = CVarDef.Create("display.width", 1280, CVar.CLIENTONLY);
		DisplayHeight = CVarDef.Create("display.height", 720, CVar.CLIENTONLY);
		DisplayUIScale = CVarDef.Create("display.uiScale", 0f, CVar.ARCHIVE | CVar.CLIENTONLY);
		DisplayRenderer = CVarDef.Create("display.renderer", 0, CVar.CLIENTONLY);
		DisplayCompat = CVarDef.Create("display.compat", defaultValue: false, CVar.CLIENTONLY);
		DisplayOpenGLVersion = CVarDef.Create("display.opengl_version", 0, CVar.CLIENTONLY);
		DisplayAngle = CVarDef.Create("display.angle", defaultValue: false, CVar.CLIENTONLY);
		DisplayAngleCustomSwapChain = CVarDef.Create("display.angle_custom_swap_chain", defaultValue: true, CVar.CLIENTONLY);
		DisplayAngleForceEs2 = CVarDef.Create("display.angle_force_es2", defaultValue: false, CVar.CLIENTONLY);
		DisplayAngleForce10_0 = CVarDef.Create("display.angle_force_10_0", defaultValue: false, CVar.CLIENTONLY);
		DisplayAngleDxgi1 = CVarDef.Create("display.angle_dxgi1", defaultValue: false, CVar.CLIENTONLY);
		DisplayAdapter = CVarDef.Create("display.adapter", "", CVar.CLIENTONLY);
		DisplayGpuPreference = CVarDef.Create("display.gpu_preference", 2, CVar.CLIENTONLY);
		DisplayEgl = CVarDef.Create("display.egl", defaultValue: false, CVar.CLIENTONLY);
		DisplayAngleEs3On10_0 = CVarDef.Create("display.angle_es3_on_10_0", defaultValue: true, CVar.CLIENTONLY);
		DisplayFontDpi = CVarDef.Create("display.fontdpi", 96, CVar.CLIENTONLY);
		DisplayOGLOverrideVersion = CVarDef.Create("display.ogl_override_version", string.Empty, CVar.CLIENTONLY);
		DisplayOGLCheckErrors = CVarDef.Create("display.ogl_check_errors", defaultValue: false, CVar.CLIENTONLY);
		DisplayForceSyncWindows = CVarDef.Create("display.force_sync_windows", defaultValue: true, CVar.CLIENTONLY);
		DisplayThreadWindowBlit = CVarDef.Create("display.thread_window_blit", defaultValue: true, CVar.CLIENTONLY);
		DisplayThreadUnlockBeforeSwap = CVarDef.Create("display.thread_unlock_before_swap", defaultValue: false, CVar.CLIENTONLY);
		DisplayInputBufferSize = CVarDef.Create("display.input_buffer_size", 32, CVar.CLIENTONLY);
		DisplayWin32Experience = CVarDef.Create("display.win32_experience", defaultValue: false, CVar.CLIENTONLY);
		DisplayWindowIconSet = CVarDef.Create("display.window_icon_set", "", CVar.CLIENTONLY);
		DisplaySplashLogo = CVarDef.Create("display.splash_logo", "", CVar.CLIENTONLY);
		DisplayUSQWERTYHotkeys = CVarDef.Create("display.use_US_QWERTY_hotkeys", defaultValue: false, CVar.ARCHIVE | CVar.CLIENTONLY);
		DisplayWindowingApi = CVarDef.Create("display.windowing_api", "sdl3", CVar.CLIENTONLY);
		DisplayWin11ImmersiveDarkMode = CVarDef.Create("display.win11_immersive_dark_mode", defaultValue: true, CVar.CLIENTONLY);
		DisplayThreadWindowApi = CVarDef.Create("display.thread_window_api", defaultValue: false, CVar.CLIENTONLY);
		AudioDefaultConcurrent = CVarDef.Create("audio.default_concurrent", 16, CVar.ARCHIVE | CVar.CLIENTONLY);
		AudioAttenuation = CVarDef.Create("audio.attenuation", 16, CVar.REPLICATED | CVar.ARCHIVE);
		AudioDevice = CVarDef.Create("audio.device", string.Empty, CVar.CLIENTONLY);
		AudioMasterVolume = CVarDef.Create("audio.mastervolume", 0.5f, CVar.ARCHIVE | CVar.CLIENTONLY);
		AudioRaycastLength = CVarDef.Create("audio.raycast_length", 15f, CVar.ARCHIVE | CVar.CLIENTONLY);
		AudioEndBuffer = CVarDef.Create("audio.end_buffer", 0.01f, CVar.REPLICATED);
		AudioTickRate = CVarDef.Create("audio.tick_rate", 30, CVar.CLIENTONLY);
		AudioZOffset = CVarDef.Create("audio.z_offset", -5f, CVar.REPLICATED);
		PlayerName = CVarDef.Create("player.name", "JoeGenero", CVar.ARCHIVE | CVar.CLIENTONLY);
		TargetMinimumTickrate = CVarDef.Create("physics.target_minimum_tickrate", 60, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);
		GenerateGridFixtures = CVarDef.Create("physics.grid_fixtures", defaultValue: true, CVar.REPLICATED);
		GridSplitting = CVarDef.Create("physics.grid_splitting", defaultValue: true, CVar.ARCHIVE);
		GridFixtureEnlargement = CVarDef.Create("physics.grid_fixture_enlargement", -0.01f, CVar.REPLICATED | CVar.ARCHIVE);
		AngularSleepTolerance = CVarDef.Create("physics.angsleeptol", 0.005235988f);
		LinearSleepTolerance = CVarDef.Create("physics.linsleeptol", 0.01f);
		SleepAllowed = CVarDef.Create("physics.sleepallowed", defaultValue: true);
		TimeToSleep = CVarDef.Create("physics.timetosleep", 0.2f);
		PositionIterations = CVarDef.Create("physics.positer", 3);
		VelocityIterations = CVarDef.Create("physics.veliter", 8);
		WarmStarting = CVarDef.Create("physics.warmstart", defaultValue: true);
		AutoClearForces = CVarDef.Create("physics.autoclearforces", defaultValue: true);
		VelocityThreshold = CVarDef.Create("physics.velocitythreshold", 0.5f);
		Baumgarte = CVarDef.Create("physics.baumgarte", 0.2f);
		MaxLinearCorrection = CVarDef.Create("physics.maxlinearcorrection", 0.2f);
		MaxAngularCorrection = CVarDef.Create("physics.maxangularcorrection", 0.13962635f);
		MaxLinVelocity = CVarDef.Create("physics.maxlinvelocity", 400f, CVar.SERVER | CVar.REPLICATED);
		MaxAngVelocity = CVarDef.Create("physics.maxangvelocity", 15f);
		InterfaceTheme = CVarDef.Create("interface.theme", "", CVar.ARCHIVE | CVar.CLIENTONLY);
		InterfaceAudio = CVarDef.Create("interface.audio", defaultValue: true, CVar.REPLICATED);
		ResAutoScaleUpperX = CVarDef.Create("interface.resolutionAutoScaleUpperCutoffX", 1080, CVar.CLIENTONLY);
		ResAutoScaleUpperY = CVarDef.Create("interface.resolutionAutoScaleUpperCutoffY", 720, CVar.CLIENTONLY);
		ResAutoScaleLowX = CVarDef.Create("interface.resolutionAutoScaleLowerCutoffX", 520, CVar.CLIENTONLY);
		ResAutoScaleLowY = CVarDef.Create("interface.resolutionAutoScaleLowerCutoffY", 520, CVar.CLIENTONLY);
		ResAutoScaleMin = CVarDef.Create("interface.resolutionAutoScaleMinimum", 0.5f, CVar.CLIENTONLY);
		ResAutoScaleEnabled = CVarDef.Create("interface.resolutionAutoScaleEnabled", defaultValue: true, CVar.ARCHIVE | CVar.CLIENTONLY);
		DiscordEnabled = CVarDef.Create("discord.enabled", defaultValue: true, CVar.ARCHIVE | CVar.CLIENTONLY);
		DiscordRichPresenceMainIconId = CVarDef.Create("discord.rich_main_icon_id", "devstation", CVar.SERVER | CVar.REPLICATED);
		DiscordRichPresenceSecondIconId = CVarDef.Create("discord.rich_second_icon_id", "logo", CVar.SERVER | CVar.REPLICATED);
		ResCheckPathCasing = CVarDef.Create("res.checkpathcasing", defaultValue: false);
		ResTexturePreloadingEnabled = CVarDef.Create("res.texturepreloadingenabled", defaultValue: true, CVar.CLIENTONLY);
		ResRSIAtlasSize = CVarDef.Create("res.rsi_atlas_size", 12288, CVar.CLIENTONLY);
		ResTexturePreloadCache = CVarDef.Create("res.texture_preload_cache", defaultValue: true, CVar.CLIENTONLY);
		ResStreamSeekMode = CVarDef.Create("res.stream_seek_mode", 0);
		ResPrototypeReloadWatch = CVarDef.Create("res.prototype_reload_watch", defaultValue: true, CVar.CLIENTONLY);
		ResCheckBadFileExtensions = CVarDef.Create("res.check_bad_file_extensions", defaultValue: true);
		DebugTargetFps = CVarDef.Create("debug.target_fps", 60, CVar.ARCHIVE | CVar.CLIENTONLY);
		MidiVolume = CVarDef.Create("midi.volume", 0.5f, CVar.ARCHIVE | CVar.CLIENTONLY);
		MidiParallelism = CVarDef.Create("midi.parallelism", 1, CVar.ARCHIVE | CVar.CLIENTONLY);
		HubAdvertise = CVarDef.Create("hub.advertise", defaultValue: false, CVar.SERVERONLY);
		HubTags = CVarDef.Create("hub.tags", "", CVar.ARCHIVE | CVar.SERVERONLY);
		HubUrls = CVarDef.Create("hub.hub_urls", "https://hub.spacestation14.com/", CVar.SERVERONLY);
		HubServerUrl = CVarDef.Create("hub.server_url", "", CVar.SERVERONLY);
		HubIpifyUrl = CVarDef.Create("hub.ipify_url", "https://api.ipify.org?format=json", CVar.SERVERONLY);
		HubAdvertiseInterval = CVarDef.Create("hub.advertise_interval", 120, CVar.SERVERONLY);
		AczStreamCompress = CVarDef.Create("acz.stream_compress", defaultValue: false, CVar.SERVERONLY);
		AczStreamCompressLevel = CVarDef.Create("acz.stream_compress_level", 3, CVar.SERVERONLY);
		AczBlobCompress = CVarDef.Create("acz.blob_compress", defaultValue: true, CVar.SERVERONLY);
		AczBlobCompressLevel = CVarDef.Create("acz.blob_compress_level", 14, CVar.SERVERONLY);
		AczBlobCompressSaveThreshold = CVarDef.Create("acz.blob_compress_save_threshold", 14, CVar.SERVERONLY);
		AczManifestCompress = CVarDef.Create("acz.manifest_compress", defaultValue: true, CVar.SERVERONLY);
		AczManifestCompressLevel = CVarDef.Create("acz.manifest_compress_level", 14, CVar.SERVERONLY);
		ConCompletionDelay = CVarDef.Create("con.completion_delay", 0f, CVar.CLIENTONLY);
		ConCompletionCount = CVarDef.Create("con.completion_count", 15, CVar.CLIENTONLY);
		ConCompletionMargin = CVarDef.Create("con.completion_margin", 3, CVar.CLIENTONLY);
		ConMaxEntries = CVarDef.Create("con.max_entries", 3000, CVar.CLIENTONLY);
		ThreadParallelCount = CVarDef.Create("thread.parallel_count", 0);
		ProfEnabled = CVarDef.Create("prof.enabled", defaultValue: false);
		ProfBufferSize = CVarDef.Create("prof.buffer_size", 8192);
		ProfIndexSize = CVarDef.Create("prof.index_size", 128);
		ReplayDirectory = CVarDef.Create("replay.directory", "replays", CVar.ARCHIVE);
		ReplayMaxCompressedSize = CVarDef.Create("replay.max_compressed_size", 524288L, CVar.ARCHIVE);
		ReplayMaxUncompressedSize = CVarDef.Create("replay.max_uncompressed_size", 1048576L, CVar.ARCHIVE);
		ReplayServerGCSizeThreshold = CVarDef.Create("replay.server_gc_size_threshold", 51200L);
		ReplayTickBatchSize = CVarDef.Create("replay.replay_tick_batchSize", 1024, CVar.ARCHIVE);
		ReplayWriteChannelSize = CVarDef.Create("replay.write_channel_size", 5);
		ReplayServerRecordingEnabled = CVarDef.Create("replay.server_recording_enabled", defaultValue: true, CVar.ARCHIVE | CVar.SERVERONLY);
		ReplayClientRecordingEnabled = CVarDef.Create("replay.client_recording_enabled", defaultValue: true, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);
		ReplayMaxScrubTime = CVarDef.Create("replay.max_scrub_time", 10);
		ReplaySkipThreshold = CVarDef.Create("replay.skip_threshold", 30);
		CheckpointMinInterval = CVarDef.Create("replay.checkpoint_min_interval", 60);
		CheckpointInterval = CVarDef.Create("replay.checkpoint_interval", 500);
		CheckpointEntitySpawnThreshold = CVarDef.Create("replay.checkpoint_entity_spawn_threshold", 1000);
		CheckpointEntityStateThreshold = CVarDef.Create("replay.checkpoint_entity_state_threshold", 30000);
		ReplayDynamicalScrubbing = CVarDef.Create("replay.dynamical_scrubbing", defaultValue: true);
		ReplayMakeContentBundle = CVarDef.Create("replay.make_content_bundle", defaultValue: true);
		ReplayIgnoreErrors = CVarDef.Create("replay.ignore_errors", defaultValue: false, CVar.CLIENTONLY);
		CfgCheckUnused = CVarDef.Create("cfg.check_unused", defaultValue: true);
		CfgRollbackData = CVarDef.Create("cfg.rollback_data", "", CVar.ARCHIVE);
		ResourceUploadingEnabled = CVarDef.Create("netres.enabled", defaultValue: true, CVar.SERVER | CVar.REPLICATED);
		ResourceUploadingLimitMb = CVarDef.Create("netres.limit", 3f, CVar.SERVER | CVar.REPLICATED);
		LaunchLauncher = CVarDef.Create("launch.launcher", defaultValue: false, CVar.CLIENTONLY);
		LaunchContentBundle = CVarDef.Create("launch.content_bundle", defaultValue: false, CVar.CLIENTONLY);
		ToolshedNearbyLimit = CVarDef.Create("toolshed.nearby_limit", 200, CVar.SERVER | CVar.REPLICATED);
		ToolshedNearbyEntitiesLimit = CVarDef.Create("toolshed.nearby_entities_limit", 5, CVar.SERVER | CVar.REPLICATED);
		ToolshedPrototypesAutocompleteLimit = CVarDef.Create("toolshed.prototype_autocomplete_limit", 256, CVar.SERVER | CVar.REPLICATED);
		LocCultureName = CVarDef.Create("loc.culture_name", "en-US", CVar.ARCHIVE);
		XamlHotReloadMarkerName = CVarDef.Create("ui.xaml_hot_reload_marker_name", "SpaceStation14.slnx", CVar.CLIENTONLY);
		UIXamlJitPreload = CVarDef.Create("ui.xaml_jit_preload", defaultValue: false, CVar.CLIENTONLY);
		FontSystem = CVarDef.Create("font.system", defaultValue: true, CVar.CLIENTONLY);
		FontWindowsDownloadable = CVarDef.Create("font.windows_downloadable", defaultValue: false, CVar.ARCHIVE | CVar.CLIENTONLY);
		LoadingShowBar = CVarDef.Create("loading.show_bar", defaultValue: true, CVar.CLIENTONLY);
		LoadingShowDebug = CVarDef.Create("loading.show_debug", defaultValue: false, CVar.CLIENTONLY);
	}
}
