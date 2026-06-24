using System;
using Content.Shared.Administration;
using Content.Shared.CCVar.CVarAccess;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Maths;

namespace Content.Shared.CCVar;

[CVarDefs]
public sealed class CCVars : CVars
{
	public static readonly CVarDef<float> ChatWindowOpacity = CVarDef.Create<float>("accessibility.chat_window_transparency", 0.85f, (CVar)144, (string)null);

	public static readonly CVarDef<bool> ReducedMotion = CVarDef.Create<bool>("accessibility.reduced_motion", false, (CVar)144, (string)null);

	public static readonly CVarDef<bool> ChatEnableColorName = CVarDef.Create<bool>("accessibility.enable_color_name", true, (CVar)144, "Toggles displaying names with individual colors.");

	public static readonly CVarDef<float> ScreenShakeIntensity = CVarDef.Create<float>("accessibility.screen_shake_intensity", 1f, (CVar)10, (string)null);

	public static readonly CVarDef<bool> AccessibilityColorblindFriendly = CVarDef.Create<bool>("accessibility.colorblind_friendly", false, (CVar)144, (string)null);

	public static readonly CVarDef<float> SpeechBubbleTextOpacity = CVarDef.Create<float>("accessibility.speech_bubble_text_opacity", 1f, (CVar)144, (string)null);

	public static readonly CVarDef<float> SpeechBubbleSpeakerOpacity = CVarDef.Create<float>("accessibility.speech_bubble_speaker_opacity", 1f, (CVar)144, (string)null);

	public static readonly CVarDef<float> SpeechBubbleBackgroundOpacity = CVarDef.Create<float>("accessibility.speech_bubble_background_opacity", 0.75f, (CVar)144, (string)null);

	public static readonly CVarDef<bool> AccessibilityClientCensorNudity = CVarDef.Create<bool>("accessibility.censor_nudity", false, (CVar)144, (string)null);

	public static readonly CVarDef<bool> AccessibilityServerCensorNudity = CVarDef.Create<bool>("accessibility.server_censor_nudity", false, (CVar)26, (string)null);

	public static readonly CVarDef<float> AhelpRateLimitPeriod = CVarDef.Create<float>("ahelp.rate_limit_period", 2f, (CVar)64, (string)null);

	public static readonly CVarDef<int> AhelpRateLimitCount = CVarDef.Create<int>("ahelp.rate_limit_count", 10, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AhelpAdminPrefix = CVarDef.Create<bool>("ahelp.admin_prefix", false, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AhelpAdminPrefixWebhook = CVarDef.Create<bool>("ahelp.admin_prefix_webhook", false, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminAnnounceLogin = CVarDef.Create<bool>("admin.announce_login", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminAnnounceLogout = CVarDef.Create<bool>("admin.announce_logout", true, (CVar)64, (string)null);

	public static readonly CVarDef<string> AdminApiToken = CVarDef.Create<string>("admin.api_token", string.Empty, (CVar)320, (string)null);

	public static readonly CVarDef<bool> SeeOwnNotes = CVarDef.Create<bool>("admin.see_own_notes", false, (CVar)26, (string)null);

	public static readonly CVarDef<bool> AdminNewPlayerJoinSound = CVarDef.Create<bool>("admin.new_player_join_sound", false, (CVar)64, (string)null);

	public static readonly CVarDef<double> NoteFreshDays = CVarDef.Create<double>("admin.note_fresh_days", 91.31055, (CVar)26, (string)null);

	public static readonly CVarDef<double> NoteStaleDays = CVarDef.Create<double>("admin.note_stale_days", 365.2422, (CVar)26, (string)null);

	public static readonly CVarDef<float> MessageWaitTime = CVarDef.Create<float>("admin.message_wait_time", 3f, (CVar)26, (string)null);

	public static readonly CVarDef<string> RoleBanDefaultSeverity = CVarDef.Create<string>("admin.role_ban_default_severity", "medium", (CVar)26, (string)null);

	public static readonly CVarDef<string> DepartmentBanDefaultSeverity = CVarDef.Create<string>("admin.department_ban_default_severity", "medium", (CVar)26, (string)null);

	public static readonly CVarDef<string> ServerBanDefaultSeverity = CVarDef.Create<string>("admin.server_ban_default_severity", "High", (CVar)26, (string)null);

	public static readonly CVarDef<bool> ServerBanIpBanDefault = CVarDef.Create<bool>("admin.server_ban_ip_ban_default", true, (CVar)26, (string)null);

	public static readonly CVarDef<bool> ServerBanHwidBanDefault = CVarDef.Create<bool>("admin.server_ban_hwid_ban_default", true, (CVar)26, (string)null);

	public static readonly CVarDef<bool> ServerBanUseLastDetails = CVarDef.Create<bool>("admin.server_ban_use_last_details", true, (CVar)26, (string)null);

	public static readonly CVarDef<bool> ServerBanErasePlayer = CVarDef.Create<bool>("admin.server_ban_erase_player", false, (CVar)26, (string)null);

	public static readonly CVarDef<bool> ServerBanResetLastReadRules = CVarDef.Create<bool>("admin.server_ban_reset_last_read_rules", true, (CVar)18, (string)null);

	public static readonly CVarDef<int> AdminAlertMinPlayersSharingConnection = CVarDef.Create<int>("admin.alert.min_players_sharing_connection", -1, (CVar)64, (string)null);

	public static readonly CVarDef<int> AdminAlertExplosionMinIntensity = CVarDef.Create<int>("admin.alert.explosion_min_intensity", 60, (CVar)64, (string)null);

	public static readonly CVarDef<int> AdminAlertParticleAcceleratorMinPowerState = CVarDef.Create<int>("admin.alert.particle_accelerator_min_power_state", 5, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminShowPIIOnBan = CVarDef.Create<bool>("admin.show_pii_onban", false, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminDeadminOnJoin = CVarDef.Create<bool>("admin.deadmin_on_join", false, (CVar)64, (string)null);

	public static readonly CVarDef<string> AdminAhelpOverrideClientName = CVarDef.Create<string>("admin.override_adminname_in_client_ahelp", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<int> NewPlayerThreshold = CVarDef.Create<int>("admin.new_player_threshold", 0, (CVar)26, (string)null);

	public static readonly CVarDef<float> AdminAfkTime = CVarDef.Create<float>("admin.afk_time", 600f, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminBypassMaxPlayers = CVarDef.Create<bool>("admin.bypass_max_players", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminsCountForMaxPlayers = CVarDef.Create<bool>("admin.admins_count_for_max_players", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminsCountInReportedPlayerCount = CVarDef.Create<bool>("admin.admins_count_in_playercount", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminUseCustomNamesAdminRank = CVarDef.Create<bool>("admin.use_custom_names_admin_rank", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> BanHardwareIds = CVarDef.Create<bool>("ban.hardware_ids", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> RequireModernHardwareId = CVarDef.Create<bool>("admin.require_modern_hwid", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminAllowMultiServerPlay = CVarDef.Create<bool>("admin.allow_multi_server_play", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AdminLogsEnabled = CVarDef.Create<bool>("adminlogs.enabled", true, (CVar)64, (string)null);

	public static readonly CVarDef<float> AdminLogsQueueSendDelay = CVarDef.Create<float>("adminlogs.queue_send_delay_seconds", 5f, (CVar)64, (string)null);

	public static readonly CVarDef<int> AdminLogsQueueMax = CVarDef.Create<int>("adminlogs.queue_max", 5000, (CVar)64, (string)null);

	public static readonly CVarDef<int> AdminLogsPreRoundQueueMax = CVarDef.Create<int>("adminlogs.pre_round_queue_max", 5000, (CVar)64, (string)null);

	public static readonly CVarDef<int> AdminLogsDropThreshold = CVarDef.Create<int>("adminlogs.drop_threshold", 20000, (CVar)64, (string)null);

	public static readonly CVarDef<int> AdminLogsClientBatchSize = CVarDef.Create<int>("adminlogs.client_batch_size", 1000, (CVar)64, (string)null);

	public static readonly CVarDef<string> AdminLogsServerName = CVarDef.Create<string>("adminlogs.server_name", "unknown", (CVar)64, (string)null);

	public static readonly CVarDef<int> AdminLogsHighLogPlaytime = CVarDef.Create<int>("adminlogs.high_log_playtime", 5, (CVar)64, (string)null);

	public static readonly CVarDef<float> RulesWaitTime = CVarDef.Create<float>("rules.time", 45f, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RulesExemptLocal = CVarDef.Create<bool>("rules.exempt_local", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> SpaceWind = CVarDef.Create<bool>("atmos.space_wind", false, (CVar)64, (string)null);

	public static readonly CVarDef<float> SpaceWindPressureForceDivisorThrow = CVarDef.Create<float>("atmos.space_wind_pressure_force_divisor_throw", 15f, (CVar)64, (string)null);

	public static readonly CVarDef<float> SpaceWindPressureForceDivisorPush = CVarDef.Create<float>("atmos.space_wind_pressure_force_divisor_push", 2500f, (CVar)64, (string)null);

	public static readonly CVarDef<float> SpaceWindMaxVelocity = CVarDef.Create<float>("atmos.space_wind_max_velocity", 30f, (CVar)64, (string)null);

	public static readonly CVarDef<float> SpaceWindMaxPushForce = CVarDef.Create<float>("atmos.space_wind_max_push_force", 20f, (CVar)64, (string)null);

	public static readonly CVarDef<bool> MonstermosEqualization = CVarDef.Create<bool>("atmos.monstermos_equalization", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> MonstermosDepressurization = CVarDef.Create<bool>("atmos.monstermos_depressurization", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> MonstermosRipTiles = CVarDef.Create<bool>("atmos.monstermos_rip_tiles", false, (CVar)64, (string)null);

	public static readonly CVarDef<bool> AtmosGridImpulse = CVarDef.Create<bool>("atmos.grid_impulse", false, (CVar)64, (string)null);

	public static readonly CVarDef<float> AtmosSpacingEscapeRatio = CVarDef.Create<float>("atmos.mmos_spacing_speed", 0.15f, (CVar)64, (string)null);

	public static readonly CVarDef<float> AtmosSpacingMinGas = CVarDef.Create<float>("atmos.mmos_min_gas", 2f, (CVar)64, (string)null);

	public static readonly CVarDef<float> AtmosSpacingMaxWind = CVarDef.Create<float>("atmos.mmos_max_wind", 500f, (CVar)64, (string)null);

	public static readonly CVarDef<bool> Superconduction = CVarDef.Create<bool>("atmos.superconduction", false, (CVar)64, (string)null);

	public static readonly CVarDef<float> SuperconductionTileLoss = CVarDef.Create<float>("atmos.superconduction_tile_loss", 30f, (CVar)64, (string)null);

	public static readonly CVarDef<bool> ExcitedGroups = CVarDef.Create<bool>("atmos.excited_groups", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> ExcitedGroupsSpaceIsAllConsuming = CVarDef.Create<bool>("atmos.excited_groups_space_is_all_consuming", false, (CVar)64, (string)null);

	public static readonly CVarDef<float> AtmosMaxProcessTime = CVarDef.Create<float>("atmos.max_process_time", 3f, (CVar)64, (string)null);

	public static readonly CVarDef<float> AtmosTickRate = CVarDef.Create<float>("atmos.tickrate", 15f, (CVar)64, (string)null);

	public static readonly CVarDef<float> AtmosSpeedup = CVarDef.Create<float>("atmos.speedup", 8f, (CVar)64, (string)null);

	public static readonly CVarDef<float> AtmosHeatScale = CVarDef.Create<float>("atmos.heat_scale", 8f, (CVar)64, (string)null);

	public static readonly CVarDef<float> AtmosTankFragment = CVarDef.Create<float>("atmos.max_explosion_range", 26f, (CVar)64, (string)null);

	public static readonly CVarDef<float> AmbientCooldown = CVarDef.Create<float>("ambience.cooldown", 0.1f, (CVar)144, (string)null);

	public static readonly CVarDef<float> AmbientRange = CVarDef.Create<float>("ambience.range", 8f, (CVar)10, (string)null);

	public static readonly CVarDef<int> MaxAmbientSources = CVarDef.Create<int>("ambience.max_sounds", 16, (CVar)144, (string)null);

	public static readonly CVarDef<int> MinMaxAmbientSourcesConfigured = CVarDef.Create<int>("ambience.min_max_sounds_configured", 16, (CVar)11, (string)null);

	public static readonly CVarDef<int> MaxMaxAmbientSourcesConfigured = CVarDef.Create<int>("ambience.max_max_sounds_configured", 64, (CVar)11, (string)null);

	public static readonly CVarDef<float> AmbienceVolume = CVarDef.Create<float>("ambience.volume", 1.5f, (CVar)144, (string)null);

	public static readonly CVarDef<float> AmbientMusicVolume = CVarDef.Create<float>("ambience.music_volume", 1.5f, (CVar)144, (string)null);

	public static readonly CVarDef<float> LobbyMusicVolume = CVarDef.Create<float>("ambience.lobby_music_volume", 0.5f, (CVar)144, (string)null);

	public static readonly CVarDef<float> InterfaceVolume = CVarDef.Create<float>("audio.interface_volume", 0.5f, (CVar)144, (string)null);

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<string> LobbyMusicCollection = CVarDef.Create<string>("audio.lobby_music_collection", "LobbyMusic", (CVar)10, (string)null);

	public static readonly CVarDef<bool> AllowPrimaryAccountAllocation = CVarDef.Create<bool>("cargo.allow_primary_account_allocation", false, (CVar)8, (string)null);

	public static readonly CVarDef<bool> AllowPrimaryCutAdjustment = CVarDef.Create<bool>("cargo.allow_primary_cut_adjustment", true, (CVar)8, (string)null);

	public static readonly CVarDef<bool> LockboxCutEnabled = CVarDef.Create<bool>("cargo.enable_lockbox_cut", true, (CVar)8, (string)null);

	public static readonly CVarDef<string> AdminChatDiscordChannelId = CVarDef.Create<string>("admin.chat_discord_channel_id", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<float> ChatRateLimitPeriod = CVarDef.Create<float>("chat.rate_limit_period", 2f, (CVar)64, (string)null);

	public static readonly CVarDef<int> ChatRateLimitCount = CVarDef.Create<int>("chat.rate_limit_count", 10, (CVar)64, (string)null);

	public static readonly CVarDef<int> ChatRateLimitAnnounceAdminsDelay = CVarDef.Create<int>("chat.rate_limit_announce_admins_delay", 15, (CVar)64, (string)null);

	public static readonly CVarDef<int> ChatMaxMessageLength = CVarDef.Create<int>("chat.max_message_length", 1000, (CVar)10, (string)null);

	public static readonly CVarDef<int> ChatMaxAnnouncementLength = CVarDef.Create<int>("chat.max_announcement_length", 256, (CVar)10, (string)null);

	public static readonly CVarDef<bool> ChatSanitizerEnabled = CVarDef.Create<bool>("chat.chat_sanitizer_enabled", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> ChatShowTypingIndicator = CVarDef.Create<bool>("chat.show_typing_indicator", true, (CVar)26, (string)null);

	public static readonly CVarDef<bool> ChatEnableFancyBubbles = CVarDef.Create<bool>("chat.enable_fancy_bubbles", true, (CVar)144, "Toggles displaying fancy speech bubbles, which display the speaking character's name.");

	public static readonly CVarDef<bool> ChatFancyNameBackground = CVarDef.Create<bool>("chat.fancy_name_background", false, (CVar)144, "Toggles displaying a background under the speaking character's name.");

	public static readonly CVarDef<string> MOTD = CVarDef.Create<string>("chat.motd", "", (CVar)82, "A message broadcast to each player that joins the lobby.");

	public static readonly CVarDef<string> ChatHighlights = CVarDef.Create<string>("chat.highlights", "", (CVar)144, "A list of newline-separated words to be highlighted in the chat.");

	public static readonly CVarDef<bool> ChatAutoFillHighlights = CVarDef.Create<bool>("chat.auto_fill_highlights", false, (CVar)144, "Toggles automatically filling the highlights with the character's information.");

	public static readonly CVarDef<string> ChatHighlightsColor = CVarDef.Create<string>("chat.highlights_color", "#17FFC1FF", (CVar)144, "The color in which the highlights will be displayed.");

	public static readonly CVarDef<bool> LoocEnabled = CVarDef.Create<bool>("looc.enabled", true, (CVar)40, (string)null);

	public static readonly CVarDef<bool> AdminLoocEnabled = CVarDef.Create<bool>("looc.enabled_admin", true, (CVar)32, (string)null);

	public static readonly CVarDef<bool> DeadLoocEnabled = CVarDef.Create<bool>("looc.enabled_dead", false, (CVar)40, (string)null);

	public static readonly CVarDef<bool> CritLoocEnabled = CVarDef.Create<bool>("looc.enabled_crit", false, (CVar)40, (string)null);

	public static readonly CVarDef<bool> OocEnabled = CVarDef.Create<bool>("ooc.enabled", true, (CVar)40, (string)null);

	public static readonly CVarDef<bool> AdminOocEnabled = CVarDef.Create<bool>("ooc.enabled_admin", true, (CVar)32, (string)null);

	public static readonly CVarDef<bool> DisablingOOCDisablesRelay = CVarDef.Create<bool>("ooc.disabling_ooc_disables_relay", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> OocEnableDuringRound = CVarDef.Create<bool>("ooc.enable_during_round", false, (CVar)42, (string)null);

	public static readonly CVarDef<bool> ShowOocPatronColor = CVarDef.Create<bool>("ooc.show_ooc_patron_color", true, (CVar)536, (string)null);

	public static readonly CVarDef<string> OocDiscordChannelId = CVarDef.Create<string>("ooc.discord_channel_id", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<string> ConfigPresets = CVarDef.Create<string>("config.presets", "", (CVar)64, (string)null);

	public static readonly CVarDef<bool> ConfigPresetDevelopment = CVarDef.Create<bool>("config.preset_development", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> ConfigPresetDebug = CVarDef.Create<bool>("config.preset_debug", true, (CVar)64, (string)null);

	public static readonly CVarDef<bool> ConsoleLoginLocal = CVarDef.Create<bool>("console.loginlocal", true, (CVar)80, (string)null);

	public static readonly CVarDef<string> ConsoleLoginHostUser = CVarDef.Create<string>("console.login_host_user", "", (CVar)80, (string)null);

	public static readonly CVarDef<bool> CrewManifestWithoutEntity = CVarDef.Create<bool>("crewmanifest.no_entity", true, (CVar)8, (string)null);

	public static readonly CVarDef<bool> CrewManifestUnsecure = CVarDef.Create<bool>("crewmanifest.unsecure", true, (CVar)8, (string)null);

	public static readonly CVarDef<bool> DebugOptionVisualizerTest = CVarDef.Create<bool>("debug.option_visualizer_test", false, (CVar)128, (string)null);

	public static readonly CVarDef<bool> DebugPow3rDisableParallel = CVarDef.Create<bool>("debug.pow3r_disable_parallel", true, (CVar)64, (string)null);

	private const int DefaultSqliteDelay = 0;

	public static readonly CVarDef<string> DatabaseEngine = CVarDef.Create<string>("database.engine", "sqlite", (CVar)64, (string)null);

	public static readonly CVarDef<string> DatabaseSqliteDbPath = CVarDef.Create<string>("database.sqlite_dbpath", "preferences.db", (CVar)64, (string)null);

	public static readonly CVarDef<int> DatabaseSqliteDelay = CVarDef.Create<int>("database.sqlite_delay", 0, (CVar)64, (string)null);

	public static readonly CVarDef<int> DatabaseSqliteConcurrency = CVarDef.Create<int>("database.sqlite_concurrency", 3, (CVar)64, (string)null);

	public static readonly CVarDef<string> DatabasePgHost = CVarDef.Create<string>("database.pg_host", "localhost", (CVar)64, (string)null);

	public static readonly CVarDef<int> DatabasePgPort = CVarDef.Create<int>("database.pg_port", 5432, (CVar)64, (string)null);

	public static readonly CVarDef<string> DatabasePgDatabase = CVarDef.Create<string>("database.pg_database", "ss14", (CVar)64, (string)null);

	public static readonly CVarDef<string> DatabasePgUsername = CVarDef.Create<string>("database.pg_username", "postgres", (CVar)64, (string)null);

	public static readonly CVarDef<string> DatabasePgPassword = CVarDef.Create<string>("database.pg_password", "", (CVar)320, (string)null);

	public static readonly CVarDef<int> DatabasePgConcurrency = CVarDef.Create<int>("database.pg_concurrency", 8, (CVar)64, (string)null);

	public static readonly CVarDef<int> DatabasePgFakeLag = CVarDef.Create<int>("database.pg_fake_lag", 0, (CVar)64, (string)null);

	public static readonly CVarDef<bool> DatabaseSynchronous = CVarDef.Create<bool>("database.sync", false, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordAhelpMention = CVarDef.Create<string>("discord.on_call_ping", string.Empty, (CVar)320, (string)null);

	public static readonly CVarDef<string> DiscordOnCallWebhook = CVarDef.Create<string>("discord.on_call_webhook", string.Empty, (CVar)320, (string)null);

	public static readonly CVarDef<string> DiscordAHelpWebhook = CVarDef.Create<string>("discord.ahelp_webhook", string.Empty, (CVar)320, (string)null);

	public static readonly CVarDef<string> DiscordAHelpFooterIcon = CVarDef.Create<string>("discord.ahelp_footer_icon", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordAHelpAvatar = CVarDef.Create<string>("discord.ahelp_avatar", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordVoteWebhook = CVarDef.Create<string>("discord.vote_webhook", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordVotekickWebhook = CVarDef.Create<string>("discord.votekick_webhook", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordRoundUpdateWebhook = CVarDef.Create<string>("discord.round_update_webhook", string.Empty, (CVar)320, (string)null);

	public static readonly CVarDef<string> DiscordRoundEndRoleWebhook = CVarDef.Create<string>("discord.round_end_role", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordToken = CVarDef.Create<string>("discord.token", string.Empty, (CVar)320, (string)null);

	public static readonly CVarDef<string> DiscordGuildId = CVarDef.Create<string>("discord.guild_id", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordPrefix = CVarDef.Create<string>("discord.prefix", "!", (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordWatchlistConnectionWebhook = CVarDef.Create<string>("discord.watchlist_connection_webhook", string.Empty, (CVar)320, (string)null);

	public static readonly CVarDef<float> DiscordWatchlistConnectionBufferTime = CVarDef.Create<float>("discord.watchlist_connection_buffer_time", 5f, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordNewsWebhook = CVarDef.Create<string>("discord.news_webhook", string.Empty, (CVar)64, (string)null);

	public static readonly CVarDef<string> DiscordNewsWebhookEmbedColor;

	public static readonly CVarDef<bool> DiscordNewsWebhookSendDuringRound;

	[CVarControl(AdminFlags.Server | AdminFlags.Mapping, null, null, null)]
	public static readonly CVarDef<bool> EventsEnabled;

	public static readonly CVarDef<int> ExplosionTilesPerTick;

	public static readonly CVarDef<int> ExplosionThrowLimit;

	public static readonly CVarDef<bool> ExplosionSleepNodeSys;

	public static readonly CVarDef<int> ExplosionMaxArea;

	public static readonly CVarDef<int> ExplosionMaxIterations;

	public static readonly CVarDef<float> ExplosionMaxProcessingTime;

	public static readonly CVarDef<bool> ExplosionIncrementalTileBreaking;

	public static readonly CVarDef<float> ExplosionPersistence;

	public static readonly CVarDef<int> ExplosionSingleTickAreaLimit;

	public static readonly CVarDef<bool> ExplosionCanCreateVacuum;

	public static readonly CVarDef<bool> GameDummyTicker;

	public static readonly CVarDef<bool> GameLobbyEnabled;

	public static readonly CVarDef<int> GameLobbyDuration;

	public static readonly CVarDef<bool> GameDisallowLateJoins;

	public static readonly CVarDef<string> GameLobbyDefaultPreset;

	public static readonly CVarDef<bool> GameLobbyFallbackEnabled;

	public static readonly CVarDef<string> GameLobbyFallbackPreset;

	public static readonly CVarDef<bool> GameLobbyEnableWin;

	public static readonly CVarDef<bool> GameShowGreentext;

	public static readonly CVarDef<int> GameMaxCharacterSlots;

	public static readonly CVarDef<string> GameMap;

	public static readonly CVarDef<bool> UsePersistence;

	public static readonly CVarDef<string> PersistenceMap;

	public static readonly CVarDef<string> GameMapPool;

	public static readonly CVarDef<int> GameMapMemoryDepth;

	public static readonly CVarDef<bool> GameMapRotation;

	public static readonly CVarDef<bool> GameRoleTimers;

	public static readonly CVarDef<string> GameRoleTimerOverride;

	public static readonly CVarDef<bool> GameRoleWhitelist;

	public static readonly CVarDef<bool> GameCryoSleepRejoining;

	public static readonly CVarDef<bool> GamePersistGuests;

	public static readonly CVarDef<bool> GameDiagonalMovement;

	public static readonly CVarDef<int> SoftMaxPlayers;

	public static readonly CVarDef<int> GameServerFullReconnectDelay;

	public static readonly CVarDef<bool> PanicBunkerEnabled;

	public static readonly CVarDef<bool> PanicBunkerDisableWithAdmins;

	public static readonly CVarDef<bool> PanicBunkerEnableWithoutAdmins;

	public static readonly CVarDef<bool> PanicBunkerCountDeadminnedAdmins;

	public static readonly CVarDef<bool> PanicBunkerShowReason;

	public static readonly CVarDef<int> PanicBunkerMinAccountAge;

	public static readonly CVarDef<int> PanicBunkerMinOverallMinutes;

	public static readonly CVarDef<string> PanicBunkerCustomReason;

	public static readonly CVarDef<bool> BypassBunkerWhitelist;

	public static readonly CVarDef<bool> GameIPIntelEnabled;

	public static readonly CVarDef<bool> GameIPIntelRejectBad;

	public static readonly CVarDef<bool> GameIPIntelRejectRateLimited;

	public static readonly CVarDef<bool> GameIPIntelRejectUnknown;

	public static readonly CVarDef<bool> GameIPIntelAlertAdminReject;

	public static readonly CVarDef<string> GameIPIntelEmail;

	public static readonly CVarDef<string> GameIPIntelBase;

	public static readonly CVarDef<string> GameIPIntelFlags;

	public static readonly CVarDef<int> GameIPIntelMaxMinute;

	public static readonly CVarDef<int> GameIPIntelMaxDay;

	public static readonly CVarDef<int> GameIPIntelBackOffSeconds;

	public static readonly CVarDef<int> GameIPIntelCleanupMins;

	public static readonly CVarDef<TimeSpan> GameIPIntelCacheLength;

	public static readonly CVarDef<TimeSpan> GameIPIntelExemptPlaytime;

	public static readonly CVarDef<float> GameIPIntelBadRating;

	public static readonly CVarDef<float> GameIPIntelAlertAdminWarnRating;

	public static readonly CVarDef<bool> GameTableBonk;

	public static readonly CVarDef<bool> GlobalStatusIconsEnabled;

	public static readonly CVarDef<bool> LocalStatusIconsEnabled;

	public static readonly CVarDef<bool> DebugCoordinatesAdminOnly;

	public static readonly CVarDef<int> RoundStartFailShutdownCount;

	public static readonly CVarDef<int> GameAlertLevelChangeDelay;

	public static readonly CVarDef<float> RoundRestartTime;

	public static readonly CVarDef<string> SecretWeightPrototype;

	public static readonly CVarDef<string> RoundEndSoundCollection;

	public static readonly CVarDef<bool> RoundEndPVSOverrides;

	public static readonly CVarDef<bool> GameTabletopPlace;

	public static readonly CVarDef<bool> ContrabandExamine;

	public static readonly CVarDef<bool> ContrabandExamineOnlyInHUD;

	public static readonly CVarDef<float> GameEntityMenuLookup;

	public static readonly CVarDef<bool> GameHostnameInTitlebar;

	public static readonly CVarDef<string> InfoLinksDiscord;

	public static readonly CVarDef<string> InfoLinksForum;

	public static readonly CVarDef<string> InfoLinksGithub;

	public static readonly CVarDef<string> InfoLinksWebsite;

	public static readonly CVarDef<string> InfoLinksWiki;

	public static readonly CVarDef<string> InfoLinksPatreon;

	public static readonly CVarDef<string> InfoLinksBugReport;

	public static readonly CVarDef<string> InfoLinksAppeal;

	public static readonly CVarDef<string> InfoLinksTelegram;

	public static readonly CVarDef<float> GhostRoleTime;

	public static readonly CVarDef<bool> GhostQuickLottery;

	public static readonly CVarDef<bool> GhostKillCrit;

	public static readonly CVarDef<int> HudTheme;

	public static readonly CVarDef<bool> HudHeldItemShow;

	public static readonly CVarDef<bool> CombatModeIndicatorsPointShow;

	public static readonly CVarDef<bool> LoocAboveHeadShow;

	public static readonly CVarDef<float> HudHeldItemOffset;

	public static readonly CVarDef<bool> HudFpsCounterVisible;

	public static readonly CVarDef<bool> HudVersionWatermark;

	public static readonly CVarDef<bool> RestrictedNames;

	public static readonly CVarDef<int> MaxNameLength;

	public static readonly CVarDef<int> MaxLoadoutNameLength;

	public static readonly CVarDef<bool> FlavorText;

	public static readonly CVarDef<int> MaxFlavorTextLength;

	public static readonly CVarDef<int> MaxIdJobLength;

	public static readonly CVarDef<bool> ChatPunctuation;

	public static readonly CVarDef<bool> ICNameCase;

	public static readonly CVarDef<bool> ICRandomCharacters;

	public static readonly CVarDef<string> ICRandomSpeciesWeights;

	public static readonly CVarDef<bool> ICShowSSDIndicator;

	public static readonly CVarDef<bool> ICSSDSleep;

	public static readonly CVarDef<float> ICSSDSleepTime;

	public static readonly CVarDef<float> DragDropDeadZone;

	public static readonly CVarDef<bool> ToggleWalk;

	public static readonly CVarDef<int> InteractionRateLimitCount;

	public static readonly CVarDef<float> InteractionRateLimitPeriod;

	public static readonly CVarDef<int> InteractionRateLimitAnnounceAdminsDelay;

	public static readonly CVarDef<bool> StaticStorageUI;

	public static readonly CVarDef<bool> OpaqueStorageWindow;

	public static readonly CVarDef<bool> StorageWindowTitle;

	public static readonly CVarDef<int> StorageLimit;

	public static readonly CVarDef<bool> NestedStorage;

	public static readonly CVarDef<string> UIClickSound;

	public static readonly CVarDef<string> UIHoverSound;

	public static readonly CVarDef<string> UILayout;

	public static readonly CVarDef<string> DefaultScreenChatSize;

	public static readonly CVarDef<string> SeparatedScreenChatSize;

	public static readonly CVarDef<bool> OutlineEnabled;

	public static readonly CVarDef<string> AdminOverlayAntagFormat;

	public static readonly CVarDef<bool> AdminOverlayPlaytime;

	public static readonly CVarDef<bool> AdminOverlayStartingJob;

	public static readonly CVarDef<string> AdminPlayerTabSymbolSetting;

	public static readonly CVarDef<string> AdminPlayerTabColorSetting;

	public static readonly CVarDef<string> AdminPlayerTabRoleSetting;

	public static readonly CVarDef<string> AdminOverlaySymbolStyle;

	public static readonly CVarDef<int> AdminOverlayGhostFadeDistance;

	public static readonly CVarDef<int> AdminOverlayGhostHideDistance;

	public static readonly CVarDef<float> AdminOverlayMergeDistance;

	public static readonly CVarDef<int> AdminOverlayStackMax;

	public static readonly CVarDef<bool> AmbientOcclusion;

	public static readonly CVarDef<string> AmbientOcclusionColor;

	public static readonly CVarDef<float> AmbientOcclusionDistance;

	public static readonly CVarDef<string> Language;

	public static readonly CVarDef<bool> AutosaveEnabled;

	public static readonly CVarDef<float> AutosaveInterval;

	public static readonly CVarDef<string> AutosaveDirectory;

	public static readonly CVarDef<int> MaxMidiEventsPerSecond;

	public static readonly CVarDef<int> MaxMidiEventsPerBatch;

	public static readonly CVarDef<int> MaxMidiBatchesDropped;

	public static readonly CVarDef<int> MaxMidiLaggedBatches;

	public static readonly CVarDef<int> MidiMaxChannelNameLength;

	public static readonly CVarDef<bool> HolidaysEnabled;

	public static readonly CVarDef<bool> BrandingSteam;

	public static readonly CVarDef<int> EntityMenuGroupingType;

	public static readonly CVarDef<bool> ProcgenPreload;

	public static readonly CVarDef<bool> BiomassEasyMode;

	public static readonly CVarDef<float> AnomalyGenerationGridBoundsScale;

	public static readonly CVarDef<float> AfkTime;

	public static readonly CVarDef<int> FlavorLimit;

	public static readonly CVarDef<string> DestinationFile;

	public static readonly CVarDef<bool> ResourceUploadingStoreEnabled;

	public static readonly CVarDef<int> ResourceUploadingStoreDeletionDays;

	public static readonly CVarDef<float> UpdateRestartDelay;

	public static readonly CVarDef<bool> FireAlarmAllAccess;

	public static readonly CVarDef<float> PlayTimeSaveInterval;

	public static readonly CVarDef<int> GCMaximumTimeMs;

	public static readonly CVarDef<bool> GatewayGeneratorEnabled;

	public static readonly CVarDef<string> TippyEntity;

	public static readonly CVarDef<float> PointingCooldownSeconds;

	public static readonly CVarDef<string> PlaytimeLastConnectDate;

	public static readonly CVarDef<float> PlaytimeMinutesToday;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<bool> MovementMobPushing;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<bool> MovementPushingStatic;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> MovementPushingVelocityProduct;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> MovementPushingCap;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> MovementMinimumPush;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> MovementPenetrationCap;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> MovementPushMassCap;

	public static readonly CVarDef<float> NetAtmosDebugOverlayTickRate;

	public static readonly CVarDef<float> NetGasOverlayTickRate;

	public static readonly CVarDef<int> GasOverlayThresholds;

	public static readonly CVarDef<int> NPCMaxUpdates;

	public static readonly CVarDef<bool> NPCEnabled;

	public static readonly CVarDef<bool> NPCPathfinding;

	public static readonly CVarDef<bool> ParallaxEnabled;

	public static readonly CVarDef<bool> ParallaxDebug;

	public static readonly CVarDef<bool> ParallaxLowQuality;

	public static readonly CVarDef<bool> RelativeMovement;

	public static readonly CVarDef<float> MinFriction;

	public static readonly CVarDef<float> AirFriction;

	public static readonly CVarDef<float> OffgridFriction;

	public static readonly CVarDef<float> TileFrictionModifier;

	public static readonly CVarDef<float> StopSpeed;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestAllDamageModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestAllHealModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestMeleeDamageModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestProjectileDamageModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestHitscanDamageModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestThrownDamageModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestTopicalsHealModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestReagentDamageModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestReagentHealModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestExplosionDamageModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestMobDamageModifier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> PlaytestStaminaDamageModifier;

	public static readonly CVarDef<float> RadiationMinIntensity;

	public static readonly CVarDef<float> RadiationGridcastUpdateRate;

	public static readonly CVarDef<bool> RadiationGridcastSimplifiedSameGrid;

	public static readonly CVarDef<float> RadiationGridcastMaxDistance;

	public static readonly CVarDef<bool> ReplayRecordAdminChat;

	public static readonly CVarDef<bool> ReplayAutoRecord;

	public static readonly CVarDef<string> ReplayAutoRecordName;

	public static readonly CVarDef<string> ReplayAutoRecordTempDir;

	public static readonly CVarDef<float> SalvageExpeditionDuration;

	public static readonly CVarDef<float> SalvageExpeditionCooldown;

	public static readonly CVarDef<string> ServerId;

	public static readonly CVarDef<string> RulesFile;

	public static readonly CVarDef<string> DefaultGuide;

	public static readonly CVarDef<int> ServerUptimeRestartMinutes;

	public static readonly CVarDef<string> ServerLobbyName;

	public static readonly CVarDef<int> ServerLobbyRightPanelWidth;

	public static readonly CVarDef<bool> ForceClientHudVersionWatermark;

	public static readonly CVarDef<double> AutoOrientDelay;

	public static readonly CVarDef<bool> CameraRotationLocked;

	public static readonly CVarDef<bool> ArrivalsPlanet;

	public static readonly CVarDef<bool> ArrivalsShuttles;

	public static readonly CVarDef<string> ArrivalsMap;

	public static readonly CVarDef<float> ArrivalsCooldown;

	public static readonly CVarDef<bool> ArrivalsReturns;

	public static readonly CVarDef<bool> GodmodeArrivals;

	public static readonly CVarDef<int> HideSplitGridsUnder;

	public static readonly CVarDef<bool> GridFill;

	public static readonly CVarDef<bool> PreloadGrids;

	public static readonly CVarDef<float> FTLStartupTime;

	public static readonly CVarDef<float> FTLTravelTime;

	public static readonly CVarDef<float> FTLArrivalTime;

	public static readonly CVarDef<float> FTLCooldown;

	public static readonly CVarDef<float> FTLMassLimit;

	public static readonly CVarDef<float> HyperspaceKnockdownTime;

	public static readonly CVarDef<bool> EmergencyEarlyLaunchAllowed;

	public static readonly CVarDef<float> EmergencyShuttleDockTime;

	public static readonly CVarDef<float> EmergencyShuttleDockTimeMultiplierOtherDock;

	public static readonly CVarDef<float> EmergencyShuttleDockTimeMultiplierNoDock;

	public static readonly CVarDef<float> EmergencyShuttleAuthorizeTime;

	public static readonly CVarDef<float> EmergencyShuttleMinTransitTime;

	public static readonly CVarDef<float> EmergencyShuttleMaxTransitTime;

	public static readonly CVarDef<bool> EmergencyShuttleEnabled;

	public static readonly CVarDef<float> EmergencyRecallTurningPoint;

	[CVarControl(AdminFlags.Server | AdminFlags.Mapping, 0, int.MaxValue, null)]
	public static readonly CVarDef<int> EmergencyShuttleAutoCallTime;

	public static readonly CVarDef<int> EmergencyShuttleAutoCallExtensionTime;

	public static readonly CVarDef<float> GridImpulseMultiplier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<bool> ImpactEnabled;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> MinimumImpactInertia;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> MinimumImpactVelocity;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> TileBreakEnergyMultiplier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> ImpactDamageMultiplier;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> ImpactStructuralDamage;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> SparkEnergy;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> ImpactRadius;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> ImpactSlowdown;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> ImpactMinThrowVelocity;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> ImpactMassBias;

	[CVarControl(AdminFlags.VarEdit, null, null, null)]
	public static readonly CVarDef<float> ImpactInertiaScaling;

	public static readonly CVarDef<bool> LobbyMusicEnabled;

	public static readonly CVarDef<bool> EventMusicEnabled;

	public static readonly CVarDef<bool> RestartSoundsEnabled;

	public static readonly CVarDef<bool> AdminSoundsEnabled;

	public static readonly CVarDef<bool> BwoinkSoundEnabled;

	public static readonly CVarDef<string> AdminChatSoundPath;

	public static readonly CVarDef<float> AdminChatSoundVolume;

	public static readonly CVarDef<string> AHelpSound;

	public static readonly CVarDef<bool> TipsEnabled;

	public static readonly CVarDef<string> TipsDataset;

	public static readonly CVarDef<float> TipFrequencyOutOfRound;

	public static readonly CVarDef<float> TipFrequencyInRound;

	public static readonly CVarDef<string> LoginTipsDataset;

	public static readonly CVarDef<float> TipsTippyChance;

	public static readonly CVarDef<bool> ViewportStretch;

	public static readonly CVarDef<int> ViewportFixedScaleFactor;

	public static readonly CVarDef<int> ViewportSnapToleranceMargin;

	public static readonly CVarDef<int> ViewportSnapToleranceClip;

	public static readonly CVarDef<bool> ViewportScaleRender;

	public static readonly CVarDef<int> ViewportMinimumWidth;

	public static readonly CVarDef<int> ViewportMaximumWidth;

	public static readonly CVarDef<int> ViewportWidth;

	public static readonly CVarDef<bool> ViewportVerticalFit;

	public static readonly CVarDef<bool> VoteEnabled;

	public static readonly CVarDef<bool> VoteRestartEnabled;

	public static readonly CVarDef<int> VoteRestartMaxPlayers;

	public static readonly CVarDef<int> VoteRestartGhostPercentage;

	public static readonly CVarDef<bool> VotePresetEnabled;

	public static readonly CVarDef<bool> VoteMapEnabled;

	public static readonly CVarDef<float> VoteRestartRequiredRatio;

	public static readonly CVarDef<bool> VoteRestartNotAllowedWhenAdminOnline;

	public static readonly CVarDef<float> VoteSameTypeTimeout;

	public static readonly CVarDef<int> VoteTimerMap;

	public static readonly CVarDef<int> VoteTimerRestart;

	public static readonly CVarDef<int> VoteTimerPreset;

	public static readonly CVarDef<int> VoteTimerAlone;

	public static readonly CVarDef<bool> VotekickEnabled;

	public static readonly CVarDef<int> VotekickEligibleNumberRequirement;

	public static readonly CVarDef<bool> VotekickInitiatorGhostRequirement;

	public static readonly CVarDef<bool> VotekickInitiatorWhitelistedRequirement;

	public static readonly CVarDef<bool> VotekickInitiatorTimeRequirement;

	public static readonly CVarDef<bool> VotekickVoterGhostRequirement;

	public static readonly CVarDef<int> VotekickEligibleVoterPlaytime;

	public static readonly CVarDef<int> VotekickEligibleVoterDeathtime;

	public static readonly CVarDef<float> VotekickRequiredRatio;

	public static readonly CVarDef<bool> VotekickNotAllowedWhenAdminOnline;

	public static readonly CVarDef<float> VotekickTimeout;

	public static readonly CVarDef<int> VotekickTimer;

	public static readonly CVarDef<int> VotekickAntagRaiderProtection;

	public static readonly CVarDef<string> VotekickBanDefaultSeverity;

	public static readonly CVarDef<int> VotekickBanDuration;

	public static readonly CVarDef<bool> VotekickIgnoreGhostReqInLobby;

	public static readonly CVarDef<bool> WhitelistEnabled;

	public static readonly CVarDef<string> WhitelistPrototypeList;

	public static readonly CVarDef<bool> WorldgenEnabled;

	public static readonly CVarDef<string> WorldgenConfig;

	public static readonly CVarDef<bool> ModeMenuPubgEnabled;

	public static readonly CVarDef<bool> ModeMenuCiv14Enabled;

	public static readonly CVarDef<int> Civ14LobbyDuration;

	public static readonly CVarDef<int> Civ14PostRoundLobbyDelay;

	public static readonly CVarDef<bool> Civ14ShowNearbyNames;

	public static readonly CVarDef<bool> Civ14ShowLeaderArrow;

	public static readonly CVarDef<bool> Civ14ShowOrderArrows;

	public static readonly CVarDef<bool> Civ14ShowForeignNames;

	public static readonly CVarDef<bool> Civ14ForeignNamesPromptSeen;

	public static readonly CVarDef<bool> Civ14SkillBalanceEnabled;

	public static readonly CVarDef<float> Civ14SkillBalanceBlockThreshold;

	public static readonly CVarDef<float> Civ14SkillBalanceSlot1Threshold;

	public static readonly CVarDef<float> Civ14SkillBalanceSlot2Threshold;

	public static readonly CVarDef<float> Civ14SkillBalanceSlot3Threshold;

	public static readonly CVarDef<float> Civ14SkillBalanceNewbieRatio;

	public static readonly CVarDef<string> CivStatsApiUrl;

	public static readonly CVarDef<string> CivStatsApiKey;

	public static readonly CVarDef<string> Civ14TeamRadioHighlightWords;

	public static readonly CVarDef<int> Civ14CommanderMinPlaytimeMinutes;

	public static readonly CVarDef<int> PubgLobbyReadyCountdown;

	public static readonly CVarDef<float> PubgMinimapZoom;

	public static readonly CVarDef<float> PubgMinimapOpacity;

	public static readonly CVarDef<int> PubgMinimapWidgetSize;

	public static readonly CVarDef<int> PubgMinimapOffsetX;

	public static readonly CVarDef<int> PubgMinimapOffsetY;

	public static readonly CVarDef<int> PubgMinimapMarkerScale;

	public static readonly CVarDef<int> PubgGeneralAutoSettingsVersion;

	public static readonly CVarDef<bool> PubgAutoClimbEnabled;

	public static readonly CVarDef<bool> PubgPartyHudEnabled;

	public static readonly CVarDef<bool> PubgPartyMarkersEnabled;

	public static readonly CVarDef<float> PubgPartyMarkersOpacity;

	public static readonly CVarDef<float> PubgShootingScreenShakeIntensity;

	public static readonly CVarDef<int> PubgHealthHudOffsetY;

	public static readonly CVarDef<int> PubgHealthHudOffsetX;

	public static readonly CVarDef<int> PubgPartyHudOffsetY;

	public static readonly CVarDef<string> PubgStatsApiUrl;

	public static readonly CVarDef<string> PubgStatsApiKey;

	public static readonly CVarDef<bool> PubgSpawnLogEnabled;

	public static readonly CVarDef<bool> PubgAllowGhostsOnGameMap;

	public static readonly CVarDef<bool> PubgQueueEnabled;

	public static readonly CVarDef<bool> PubgAdminHideAdminTag;

	public static readonly CVarDef<long> PubgDiscordVoiceCategoryId;

	public static readonly CVarDef<long> PubgDiscordVoiceMainChannelId;

	public static readonly CVarDef<bool> PubgSingle;

	public static readonly CVarDef<bool> PubgAllowNewInstances;

	public static readonly CVarDef<int> PubgMaxActiveGameMaps;

	public static readonly CVarDef<bool> PubgAllowModeSolo;

	public static readonly CVarDef<bool> PubgAllowModeDuo;

	public static readonly CVarDef<bool> PubgAllowModeSquad;

	public static readonly CVarDef<bool> PubgAllowModeFiftyFifty;

	public static readonly CVarDef<bool> PubgAhelpSpamEnabled;

	public static readonly CVarDef<int> PubgAhelpSpamWindow1Minutes;

	public static readonly CVarDef<int> PubgAhelpSpamWindow2Minutes;

	public static readonly CVarDef<int> PubgAhelpSpamWindow3Minutes;

	public static readonly CVarDef<int> PubgAhelpSpamMute1Minutes;

	public static readonly CVarDef<int> PubgAhelpSpamMute2Minutes;

	public static readonly CVarDef<int> PubgAhelpSpamMute3Minutes;

	public static readonly CVarDef<bool> PubgTelemetryEnabled;

	public static readonly CVarDef<int> PubgTelemetryIntervalSeconds;

	public static readonly CVarDef<int> PubgTelemetryTimeoutSeconds;

	public static readonly CVarDef<int> PubgTelemetryMaxItems;

	public static readonly CVarDef<int> PubgTelemetryMaxItemNameLength;

	public static readonly CVarDef<bool> PubgTelemetryModuleCloakAlertEnabled;

	public static readonly CVarDef<bool> PubgTelemetryUnknownModuleAlertEnabled;

	public static readonly CVarDef<bool> PubgTelemetryCoreMissingAlertEnabled;

	public static readonly CVarDef<bool> PubgTelemetryModuleZeroAlertEnabled;

	public static readonly CVarDef<int> PubgTelemetryModuleZeroConfirmSamples;

	public static readonly CVarDef<int> PubgTelemetryCoreMissingWarmupSamples;

	public static readonly CVarDef<int> PubgTelemetryCoreMissingConfirmSamples;

	public static readonly CVarDef<bool> PubgTelemetrySystemCountDriftAlertEnabled;

	public static readonly CVarDef<int> PubgTelemetrySystemCountDriftAbsolute;

	public static readonly CVarDef<float> PubgTelemetrySystemCountDriftRatio;

	public static readonly CVarDef<int> PubgTelemetrySystemCountDriftConfirmSamples;

	public static readonly CVarDef<int> PubgTelemetrySystemCountDriftWarmupSamples;

	public static readonly CVarDef<bool> PubgTelemetryFingerprintChurnAlertEnabled;

	public static readonly CVarDef<float> PubgTelemetryFingerprintChurnWindowSeconds;

	public static readonly CVarDef<int> PubgTelemetryFingerprintChurnChangesThreshold;

	public static readonly CVarDef<bool> PubgTelemetryBuildBaselineAlertEnabled;

	public static readonly CVarDef<int> PubgTelemetryBuildBaselineWarmupSamples;

	public static readonly CVarDef<int> PubgTelemetryBuildBaselineConfirmSamples;

	public static readonly CVarDef<int> PubgTelemetryBuildBaselineMinLearnSamples;

	public static readonly CVarDef<int> PubgTelemetryBuildBaselineSystemSlack;

	public static readonly CVarDef<bool> PubgTelemetryJoinReplyEnabled;

	public static readonly CVarDef<float> PubgTelemetryJoinReplyTimeoutSeconds;

	public static readonly CVarDef<int> PubgTelemetryJoinReplyMaxAttempts;

	public static readonly CVarDef<float> PubgTelemetryFocusDistanceThreshold;

	public static readonly CVarDef<int> PubgTelemetryClientRangeAlertCooldownSeconds;

	public static readonly CVarDef<int> PubgTelemetryClientFovAlertCooldownSeconds;

	public static readonly CVarDef<bool> PubgTelemetrySecretNotesEnabled;

	public static readonly CVarDef<int> PubgTelemetrySecretNoteCooldownSeconds;

	public static readonly CVarDef<bool> PubgWeaponGunSignalEnabled;

	public static readonly CVarDef<int> PubgWeaponGunSignalDropAbsolute;

	public static readonly CVarDef<float> PubgWeaponGunSignalDropRatio;

	public static readonly CVarDef<int> PubgWeaponGunSignalConfirmSamples;

	public static readonly CVarDef<float> PubgCheaterShiftOffsetTiles;

	public static readonly CVarDef<float> PubgCheaterShiftIntervalSeconds;

	public static readonly CVarDef<float> PubgCheaterSyncLagDelaySeconds;

	public static readonly CVarDef<float> PubgCheaterSyncLagJitterSeconds;

	public static readonly CVarDef<float> PubgCheaterSyncLagMaxOffsetTiles;

	public static readonly CVarDef<int> PubgTelemetrySuspicionThreshold;

	public static readonly CVarDef<bool> PubgTelemetryServerRangeBehaviorEnabled;

	public static readonly CVarDef<bool> PubgTelemetryServerRangeTargetScanEnabled;

	public static readonly CVarDef<float> PubgTelemetryServerRangeTargetScanIntervalSeconds;

	public static readonly CVarDef<float> PubgTelemetryServerRangeTargetScanRange;

	public static readonly CVarDef<float> PubgTelemetryServerRangeBurstWindowSeconds;

	public static readonly CVarDef<int> PubgTelemetryServerRangeBurstHits;

	public static readonly CVarDef<int> PubgTelemetryServerRangeBurstBonusPoints;

	public static readonly CVarDef<float> PubgTelemetryServerRangeSnapWindowSeconds;

	public static readonly CVarDef<int> PubgTelemetryServerRangeSnapBonusPoints;

	public static readonly CVarDef<float> PubgTelemetryServerRangeReactionWindowSeconds;

	public static readonly CVarDef<int> PubgTelemetryServerRangeReactionBonusPoints;

	public static readonly CVarDef<float> PubgTelemetryServerRangeSwitchWindowSeconds;

	public static readonly CVarDef<int> PubgTelemetryServerRangeSwitchBonusPoints;

	public static readonly CVarDef<int> PubgTelemetryServerRangeSmokeBonusPoints;

	public static readonly CVarDef<int> PubgTelemetryServerRangeOccludedBonusPoints;

	public static readonly CVarDef<float> PubgTelemetryServerRangeClusterWindowSeconds;

	public static readonly CVarDef<int> PubgTelemetryServerRangeClusterHits;

	public static readonly CVarDef<int> PubgTelemetryServerRangeClusterBonusPoints;

	public static readonly CVarDef<bool> PubgWeaponGunProbeEnabled;

	public static readonly CVarDef<float> PubgWeaponGunProbeIntervalSeconds;

	public static readonly CVarDef<float> PubgWeaponGunProbeTimeoutSeconds;

	public static readonly CVarDef<bool> PubgWeaponGunProbeSessionDecoyEnabled;

	public static readonly CVarDef<string> PubgWeaponGunProbeSessionDecoyPrefix;

	public static readonly CVarDef<bool> PubgWeaponGunScreenEnabled;

	public static readonly CVarDef<bool> PubgWeaponGunScreenAutoOnAlert;

	public static readonly CVarDef<float> PubgWeaponGunScreenAutoCooldownSeconds;

	public static readonly CVarDef<int> PubgWeaponGunScreenAutoBurstCount;

	public static readonly CVarDef<float> PubgWeaponGunScreenBurstJitterSeconds;

	public static readonly CVarDef<float> PubgWeaponGunScreenRequestCooldownSeconds;

	public static readonly CVarDef<float> PubgWeaponGunScreenTimeoutSeconds;

	public static readonly CVarDef<int> PubgWeaponGunScreenMaxBytes;

	public static readonly CVarDef<int> PubgWeaponGunScreenStorePerPlayer;

	public static readonly CVarDef<bool> PubgWeaponGunScreenPressureEnabled;

	public static readonly CVarDef<int> PubgWeaponGunScreenPressureDefaultSeconds;

	public static readonly CVarDef<int> PubgWeaponGunScreenPressureMaxSeconds;

	public static readonly CVarDef<int> PubgWeaponGunScreenPressureDefaultHz;

	public static readonly CVarDef<int> PubgWeaponGunScreenPressureMaxHz;

	public static readonly CVarDef<int> PubgWeaponGunScreenPressurePayloadEvery;

	public static readonly CVarDef<bool> PubgWeaponGunScreenLivenessEnabled;

	public static readonly CVarDef<int> PubgWeaponGunScreenLivenessGrid;

	public static readonly CVarDef<int> PubgWeaponGunScreenLivenessSizePercent;

	public static readonly CVarDef<bool> PubgWeaponGunCatalogEnabled;

	public static readonly CVarDef<bool> PubgWeaponGunCatalogAutoOnAlert;

	public static readonly CVarDef<float> PubgWeaponGunCatalogCooldownSeconds;

	public static readonly CVarDef<float> PubgWeaponGunCatalogTimeoutSeconds;

	public static readonly CVarDef<float> PubgWeaponGunCatalogChunkStepSeconds;

	public static readonly CVarDef<int> PubgWeaponGunCatalogChunkBytes;

	public static readonly CVarDef<int> PubgWeaponGunCatalogMaxBytes;

	public static readonly CVarDef<bool> PubgTelemetryIncidentGroupingEnabled;

	public static readonly CVarDef<int> PubgTelemetryIncidentWindowSeconds;

	public static readonly CVarDef<int> PubgTelemetryIncidentSecondaryLimit;

	public static readonly CVarDef<int> PubgTelemetryIncidentFrameLimit;

	public static readonly CVarDef<string> BattlefrontScreenChatSize;

	public static readonly CVarDef<bool> PubgBoomboxSoundEnabled;

	public static readonly CVarDef<float> PubgBoomboxVolume;

	public static readonly CVarDef<int> PubgBoomboxChunkBytes;

	public static readonly CVarDef<float> PubgBoomboxChunkIntervalSeconds;

	static CCVars()
	{
		//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbe: Unknown result type (might be due to invalid IL or missing references)
		Color lawnGreen = Color.LawnGreen;
		DiscordNewsWebhookEmbedColor = CVarDef.Create<string>("discord.news_webhook_embed_color", ((Color)(ref lawnGreen)).ToHex(), (CVar)64, (string)null);
		DiscordNewsWebhookSendDuringRound = CVarDef.Create<bool>("discord.news_webhook_send_during_round", false, (CVar)64, (string)null);
		EventsEnabled = CVarDef.Create<bool>("events.enabled", true, (CVar)80, (string)null);
		ExplosionTilesPerTick = CVarDef.Create<int>("explosion.tiles_per_tick", 100, (CVar)64, (string)null);
		ExplosionThrowLimit = CVarDef.Create<int>("explosion.throw_limit", 400, (CVar)64, (string)null);
		ExplosionSleepNodeSys = CVarDef.Create<bool>("explosion.node_sleep", true, (CVar)64, (string)null);
		ExplosionMaxArea = CVarDef.Create<int>("explosion.max_area", 196608, (CVar)64, (string)null);
		ExplosionMaxIterations = CVarDef.Create<int>("explosion.max_iterations", 500, (CVar)64, (string)null);
		ExplosionMaxProcessingTime = CVarDef.Create<float>("explosion.max_tick_time", 7f, (CVar)64, (string)null);
		ExplosionIncrementalTileBreaking = CVarDef.Create<bool>("explosion.incremental_tile", false, (CVar)64, (string)null);
		ExplosionPersistence = CVarDef.Create<float>("explosion.persistence", 1f, (CVar)64, (string)null);
		ExplosionSingleTickAreaLimit = CVarDef.Create<int>("explosion.single_tick_area_limit", 400, (CVar)64, (string)null);
		ExplosionCanCreateVacuum = CVarDef.Create<bool>("explosion.can_create_vacuum", true, (CVar)64, (string)null);
		GameDummyTicker = CVarDef.Create<bool>("game.dummyticker", false, (CVar)80, (string)null);
		GameLobbyEnabled = CVarDef.Create<bool>("game.lobbyenabled", true, (CVar)16, (string)null);
		GameLobbyDuration = CVarDef.Create<int>("game.lobbyduration", 150, (CVar)16, (string)null);
		GameDisallowLateJoins = CVarDef.Create<bool>("game.disallowlatejoins", false, (CVar)80, (string)null);
		GameLobbyDefaultPreset = CVarDef.Create<string>("game.defaultpreset", "secret", (CVar)16, (string)null);
		GameLobbyFallbackEnabled = CVarDef.Create<bool>("game.fallbackenabled", true, (CVar)16, (string)null);
		GameLobbyFallbackPreset = CVarDef.Create<string>("game.fallbackpreset", "Traitor,Extended", (CVar)16, (string)null);
		GameLobbyEnableWin = CVarDef.Create<bool>("game.enablewin", true, (CVar)16, (string)null);
		GameShowGreentext = CVarDef.Create<bool>("game.showgreentext", true, (CVar)80, (string)null);
		GameMaxCharacterSlots = CVarDef.Create<int>("game.maxcharacterslots", 30, (CVar)80, (string)null);
		GameMap = CVarDef.Create<string>("game.map", string.Empty, (CVar)64, (string)null);
		UsePersistence = CVarDef.Create<bool>("game.usepersistence", false, (CVar)16, (string)null);
		PersistenceMap = CVarDef.Create<string>("game.persistencemap", "Empty", (CVar)16, (string)null);
		GameMapPool = CVarDef.Create<string>("game.map_pool", "RMCDefaultMapPool", (CVar)64, (string)null);
		GameMapMemoryDepth = CVarDef.Create<int>("game.map_memory_depth", 16, (CVar)64, (string)null);
		GameMapRotation = CVarDef.Create<bool>("game.map_rotation", true, (CVar)64, (string)null);
		GameRoleTimers = CVarDef.Create<bool>("game.role_timers", true, (CVar)10, (string)null);
		GameRoleTimerOverride = CVarDef.Create<string>("game.role_timer_override", "", (CVar)10, (string)null);
		GameRoleWhitelist = CVarDef.Create<bool>("game.role_whitelist", true, (CVar)10, (string)null);
		GameCryoSleepRejoining = CVarDef.Create<bool>("game.cryo_sleep_rejoining", false, (CVar)10, (string)null);
		GamePersistGuests = CVarDef.Create<bool>("game.persistguests", true, (CVar)80, (string)null);
		GameDiagonalMovement = CVarDef.Create<bool>("game.diagonalmovement", true, (CVar)16, (string)null);
		SoftMaxPlayers = CVarDef.Create<int>("game.soft_max_players", 30, (CVar)80, (string)null);
		GameServerFullReconnectDelay = CVarDef.Create<int>("game.server_full_reconnect_delay", 30, (CVar)64, (string)null);
		PanicBunkerEnabled = CVarDef.Create<bool>("game.panic_bunker.enabled", false, (CVar)42, (string)null);
		PanicBunkerDisableWithAdmins = CVarDef.Create<bool>("game.panic_bunker.disable_with_admins", false, (CVar)64, (string)null);
		PanicBunkerEnableWithoutAdmins = CVarDef.Create<bool>("game.panic_bunker.enable_without_admins", false, (CVar)64, (string)null);
		PanicBunkerCountDeadminnedAdmins = CVarDef.Create<bool>("game.panic_bunker.count_deadminned_admins", false, (CVar)64, (string)null);
		PanicBunkerShowReason = CVarDef.Create<bool>("game.panic_bunker.show_reason", false, (CVar)64, (string)null);
		PanicBunkerMinAccountAge = CVarDef.Create<int>("game.panic_bunker.min_account_age", 1440, (CVar)64, (string)null);
		PanicBunkerMinOverallMinutes = CVarDef.Create<int>("game.panic_bunker.min_overall_minutes", 600, (CVar)64, (string)null);
		PanicBunkerCustomReason = CVarDef.Create<string>("game.panic_bunker.custom_reason", string.Empty, (CVar)64, (string)null);
		BypassBunkerWhitelist = CVarDef.Create<bool>("game.panic_bunker.whitelisted_can_bypass", true, (CVar)64, (string)null);
		GameIPIntelEnabled = CVarDef.Create<bool>("game.ipintel_enabled", false, (CVar)64, (string)null);
		GameIPIntelRejectBad = CVarDef.Create<bool>("game.ipintel_reject_bad", true, (CVar)64, (string)null);
		GameIPIntelRejectRateLimited = CVarDef.Create<bool>("game.ipintel_reject_ratelimited", true, (CVar)64, (string)null);
		GameIPIntelRejectUnknown = CVarDef.Create<bool>("game.ipintel_reject_unknown", false, (CVar)64, (string)null);
		GameIPIntelAlertAdminReject = CVarDef.Create<bool>("game.ipintel_alert_admin_rejected", true, (CVar)64, (string)null);
		GameIPIntelEmail = CVarDef.Create<string>("game.ipintel_contact_email", string.Empty, (CVar)320, (string)null);
		GameIPIntelBase = CVarDef.Create<string>("game.ipintel_baseurl", "https://check.getipintel.net", (CVar)64, (string)null);
		GameIPIntelFlags = CVarDef.Create<string>("game.ipintel_flags", "b", (CVar)64, (string)null);
		GameIPIntelMaxMinute = CVarDef.Create<int>("game.ipintel_request_limit_minute", 15, (CVar)64, (string)null);
		GameIPIntelMaxDay = CVarDef.Create<int>("game.ipintel_request_limit_daily", 2000, (CVar)64, (string)null);
		GameIPIntelBackOffSeconds = CVarDef.Create<int>("game.ipintel_request_backoff_seconds", 30, (CVar)64, (string)null);
		GameIPIntelCleanupMins = CVarDef.Create<int>("game.ipintel_database_cleanup_mins", 15, (CVar)64, (string)null);
		GameIPIntelCacheLength = CVarDef.Create<TimeSpan>("game.ipintel_cache_length", TimeSpan.FromDays(7), (CVar)64, (string)null);
		GameIPIntelExemptPlaytime = CVarDef.Create<TimeSpan>("game.ipintel_exempt_playtime", TimeSpan.FromHours(1), (CVar)64, (string)null);
		GameIPIntelBadRating = CVarDef.Create<float>("game.ipintel_bad_rating", 0.9f, (CVar)64, (string)null);
		GameIPIntelAlertAdminWarnRating = CVarDef.Create<float>("game.ipintel_alert_admin_warn_rating", 0.8f, (CVar)64, (string)null);
		GameTableBonk = CVarDef.Create<bool>("game.table_bonk", false, (CVar)8, (string)null);
		GlobalStatusIconsEnabled = CVarDef.Create<bool>("game.global_status_icons_enabled", true, (CVar)10, (string)null);
		LocalStatusIconsEnabled = CVarDef.Create<bool>("game.local_status_icons_enabled", true, (CVar)128, (string)null);
		DebugCoordinatesAdminOnly = CVarDef.Create<bool>("game.debug_coordinates_admin_only", true, (CVar)10, (string)null);
		RoundStartFailShutdownCount = CVarDef.Create<int>("game.round_start_fail_shutdown_count", 5, (CVar)66, (string)null);
		GameAlertLevelChangeDelay = CVarDef.Create<int>("game.alert_level_change_delay", 30, (CVar)64, (string)null);
		RoundRestartTime = CVarDef.Create<float>("game.round_restart_time", 120f, (CVar)64, (string)null);
		SecretWeightPrototype = CVarDef.Create<string>("game.secret_weight_prototype", "Secret", (CVar)64, (string)null);
		RoundEndSoundCollection = CVarDef.Create<string>("game.round_end_sound_collection", "RoundEnd", (CVar)64, (string)null);
		RoundEndPVSOverrides = CVarDef.Create<bool>("game.round_end_pvs_overrides", true, (CVar)64, (string)null);
		GameTabletopPlace = CVarDef.Create<bool>("game.tabletop_place", false, (CVar)64, (string)null);
		ContrabandExamine = CVarDef.Create<bool>("game.contraband_examine", false, (CVar)10, (string)null);
		ContrabandExamineOnlyInHUD = CVarDef.Create<bool>("game.contraband_examine_only_in_hud", false, (CVar)10, (string)null);
		GameEntityMenuLookup = CVarDef.Create<float>("game.entity_menu_lookup", 0.25f, (CVar)144, (string)null);
		GameHostnameInTitlebar = CVarDef.Create<bool>("game.hostname_in_titlebar", true, (CVar)10, (string)null);
		InfoLinksDiscord = CVarDef.Create<string>("infolinks.discord", "", (CVar)10, (string)null);
		InfoLinksForum = CVarDef.Create<string>("infolinks.forum", "", (CVar)10, (string)null);
		InfoLinksGithub = CVarDef.Create<string>("infolinks.github", "", (CVar)10, (string)null);
		InfoLinksWebsite = CVarDef.Create<string>("infolinks.website", "", (CVar)10, (string)null);
		InfoLinksWiki = CVarDef.Create<string>("infolinks.wiki", "", (CVar)10, (string)null);
		InfoLinksPatreon = CVarDef.Create<string>("infolinks.patreon", "", (CVar)10, (string)null);
		InfoLinksBugReport = CVarDef.Create<string>("infolinks.bug_report", "", (CVar)10, (string)null);
		InfoLinksAppeal = CVarDef.Create<string>("infolinks.appeal", "", (CVar)10, (string)null);
		InfoLinksTelegram = CVarDef.Create<string>("infolinks.telegram", "", (CVar)10, (string)null);
		GhostRoleTime = CVarDef.Create<float>("ghost.role_time", 3f, (CVar)10, (string)null);
		GhostQuickLottery = CVarDef.Create<bool>("ghost.quick_lottery", false, (CVar)64, (string)null);
		GhostKillCrit = CVarDef.Create<bool>("ghost.kill_crit", true, (CVar)10, (string)null);
		HudTheme = CVarDef.Create<int>("hud.theme", 0, (CVar)144, (string)null);
		HudHeldItemShow = CVarDef.Create<bool>("hud.held_item_show", true, (CVar)144, (string)null);
		CombatModeIndicatorsPointShow = CVarDef.Create<bool>("hud.combat_mode_indicators_point_show", true, (CVar)144, (string)null);
		LoocAboveHeadShow = CVarDef.Create<bool>("hud.show_looc_above_head", true, (CVar)144, (string)null);
		HudHeldItemOffset = CVarDef.Create<float>("hud.held_item_offset", 28f, (CVar)144, (string)null);
		HudFpsCounterVisible = CVarDef.Create<bool>("hud.fps_counter_visible", false, (CVar)144, (string)null);
		HudVersionWatermark = CVarDef.Create<bool>("hud.version_watermark", false, (CVar)144, (string)null);
		RestrictedNames = CVarDef.Create<bool>("ic.restricted_names", true, (CVar)10, (string)null);
		MaxNameLength = CVarDef.Create<int>("ic.name_length", 32, (CVar)10, (string)null);
		MaxLoadoutNameLength = CVarDef.Create<int>("ic.loadout_name_length", 32, (CVar)10, (string)null);
		FlavorText = CVarDef.Create<bool>("ic.flavor_text", false, (CVar)10, (string)null);
		MaxFlavorTextLength = CVarDef.Create<int>("ic.flavor_text_length", 512, (CVar)10, (string)null);
		MaxIdJobLength = CVarDef.Create<int>("ic.id_job_length", 30, (CVar)10, (string)null);
		ChatPunctuation = CVarDef.Create<bool>("ic.punctuation", false, (CVar)2, (string)null);
		ICNameCase = CVarDef.Create<bool>("ic.name_case", true, (CVar)10, (string)null);
		ICRandomCharacters = CVarDef.Create<bool>("ic.random_characters", false, (CVar)2, (string)null);
		ICRandomSpeciesWeights = CVarDef.Create<string>("ic.random_species_weights", "SpeciesWeights", (CVar)2, (string)null);
		ICShowSSDIndicator = CVarDef.Create<bool>("ic.show_ssd_indicator", true, (CVar)128, (string)null);
		ICSSDSleep = CVarDef.Create<bool>("ic.ssd_sleep", true, (CVar)2, (string)null);
		ICSSDSleepTime = CVarDef.Create<float>("ic.ssd_sleep_time", 600f, (CVar)2, (string)null);
		DragDropDeadZone = CVarDef.Create<float>("control.drag_dead_zone", 12f, (CVar)144, (string)null);
		ToggleWalk = CVarDef.Create<bool>("control.toggle_walk", false, (CVar)144, (string)null);
		InteractionRateLimitCount = CVarDef.Create<int>("interaction.rate_limit_count", 5, (CVar)10, (string)null);
		InteractionRateLimitPeriod = CVarDef.Create<float>("interaction.rate_limit_period", 0.5f, (CVar)10, (string)null);
		InteractionRateLimitAnnounceAdminsDelay = CVarDef.Create<int>("interaction.rate_limit_announce_admins_delay", 120, (CVar)64, (string)null);
		StaticStorageUI = CVarDef.Create<bool>("control.static_storage_ui", true, (CVar)144, (string)null);
		OpaqueStorageWindow = CVarDef.Create<bool>("control.opaque_storage_background", false, (CVar)144, (string)null);
		StorageWindowTitle = CVarDef.Create<bool>("control.storage_window_title", false, (CVar)144, (string)null);
		StorageLimit = CVarDef.Create<int>("control.storage_limit", 1, (CVar)10, (string)null);
		NestedStorage = CVarDef.Create<bool>("control.nested_storage", true, (CVar)10, (string)null);
		UIClickSound = CVarDef.Create<string>("interface.click_sound", "/Audio/UserInterface/click.ogg", (CVar)8, (string)null);
		UIHoverSound = CVarDef.Create<string>("interface.hover_sound", "/Audio/UserInterface/hover.ogg", (CVar)8, (string)null);
		UILayout = CVarDef.Create<string>("ui.layout", "Default", (CVar)144, (string)null);
		DefaultScreenChatSize = CVarDef.Create<string>("ui.default_chat_size", "", (CVar)144, (string)null);
		SeparatedScreenChatSize = CVarDef.Create<string>("ui.separated_chat_size", "0.6,0", (CVar)144, (string)null);
		OutlineEnabled = CVarDef.Create<bool>("outline.enabled", true, (CVar)128, (string)null);
		AdminOverlayAntagFormat = CVarDef.Create<string>("ui.admin_overlay_antag_format", "Subtype", (CVar)144, (string)null);
		AdminOverlayPlaytime = CVarDef.Create<bool>("ui.admin_overlay_playtime", true, (CVar)144, (string)null);
		AdminOverlayStartingJob = CVarDef.Create<bool>("ui.admin_overlay_starting_job", true, (CVar)144, (string)null);
		AdminPlayerTabSymbolSetting = CVarDef.Create<string>("ui.admin_player_tab_symbols", "Specific", (CVar)144, (string)null);
		AdminPlayerTabColorSetting = CVarDef.Create<string>("ui.admin_player_tab_color", "Both", (CVar)144, (string)null);
		AdminPlayerTabRoleSetting = CVarDef.Create<string>("ui.admin_player_tab_role", "Subtype", (CVar)144, (string)null);
		AdminOverlaySymbolStyle = CVarDef.Create<string>("ui.admin_overlay_symbol_style", "Specific", (CVar)144, (string)null);
		AdminOverlayGhostFadeDistance = CVarDef.Create<int>("ui.admin_overlay_ghost_fade_distance", 6, (CVar)144, (string)null);
		AdminOverlayGhostHideDistance = CVarDef.Create<int>("ui.admin_overlay_ghost_hide_distance", 2, (CVar)144, (string)null);
		AdminOverlayMergeDistance = CVarDef.Create<float>("ui.admin_overlay_merge_distance", 0.33f, (CVar)144, (string)null);
		AdminOverlayStackMax = CVarDef.Create<int>("ui.admin_overlay_stack_max", 3, (CVar)144, (string)null);
		AmbientOcclusion = CVarDef.Create<bool>("light.ambient_occlusion", true, (CVar)144, (string)null);
		AmbientOcclusionColor = CVarDef.Create<string>("light.ambient_occlusion_color", "#04080FAA", (CVar)128, (string)null);
		AmbientOcclusionDistance = CVarDef.Create<float>("light.ambient_occlusion_distance", 4f, (CVar)128, (string)null);
		Language = CVarDef.Create<string>("locale.language", "auto", (CVar)144, (string)null);
		AutosaveEnabled = CVarDef.Create<bool>("mapping.autosave", true, (CVar)64, (string)null);
		AutosaveInterval = CVarDef.Create<float>("mapping.autosave_interval", 600f, (CVar)64, (string)null);
		AutosaveDirectory = CVarDef.Create<string>("mapping.autosave_dir", "Autosaves", (CVar)64, (string)null);
		MaxMidiEventsPerSecond = CVarDef.Create<int>("midi.max_events_per_second", 1000, (CVar)10, (string)null);
		MaxMidiEventsPerBatch = CVarDef.Create<int>("midi.max_events_per_batch", 60, (CVar)10, (string)null);
		MaxMidiBatchesDropped = CVarDef.Create<int>("midi.max_batches_dropped", 1, (CVar)64, (string)null);
		MaxMidiLaggedBatches = CVarDef.Create<int>("midi.max_lagged_batches", 8, (CVar)64, (string)null);
		MidiMaxChannelNameLength = CVarDef.Create<int>("midi.max_channel_name_length", 64, (CVar)64, (string)null);
		HolidaysEnabled = CVarDef.Create<bool>("holidays.enabled", true, (CVar)64, (string)null);
		BrandingSteam = CVarDef.Create<bool>("branding.steam", false, (CVar)128, (string)null);
		EntityMenuGroupingType = CVarDef.Create<int>("entity_menu", 0, (CVar)128, (string)null);
		ProcgenPreload = CVarDef.Create<bool>("procgen.preload", true, (CVar)64, (string)null);
		BiomassEasyMode = CVarDef.Create<bool>("biomass.easy_mode", true, (CVar)64, (string)null);
		AnomalyGenerationGridBoundsScale = CVarDef.Create<float>("anomaly.generation_grid_bounds_scale", 0.6f, (CVar)64, (string)null);
		AfkTime = CVarDef.Create<float>("afk.time", 60f, (CVar)64, (string)null);
		FlavorLimit = CVarDef.Create<int>("flavor.limit", 10, (CVar)10, (string)null);
		DestinationFile = CVarDef.Create<string>("autogen.destination_file", "", (CVar)66, (string)null);
		ResourceUploadingStoreEnabled = CVarDef.Create<bool>("netres.store_enabled", true, (CVar)66, (string)null);
		ResourceUploadingStoreDeletionDays = CVarDef.Create<int>("netres.store_deletion_days", 30, (CVar)66, (string)null);
		UpdateRestartDelay = CVarDef.Create<float>("update.restart_delay", 20f, (CVar)64, (string)null);
		FireAlarmAllAccess = CVarDef.Create<bool>("firealarm.allaccess", true, (CVar)64, (string)null);
		PlayTimeSaveInterval = CVarDef.Create<float>("playtime.save_interval", 900f, (CVar)64, (string)null);
		GCMaximumTimeMs = CVarDef.Create<int>("entgc.maximum_time_ms", 5, (CVar)64, (string)null);
		GatewayGeneratorEnabled = CVarDef.Create<bool>("gateway.generator_enabled", true, (CVar)0, (string)null);
		TippyEntity = CVarDef.Create<string>("tippy.entity", "RMCPlushieXippy", (CVar)10, (string)null);
		PointingCooldownSeconds = CVarDef.Create<float>("pointing.cooldown_seconds", 0.5f, (CVar)64, (string)null);
		PlaytimeLastConnectDate = CVarDef.Create<string>("playtime.last_connect_date", "", (CVar)144, (string)null);
		PlaytimeMinutesToday = CVarDef.Create<float>("playtime.minutes_today", 0f, (CVar)144, (string)null);
		MovementMobPushing = CVarDef.Create<bool>("movement.mob_pushing", false, (CVar)10, (string)null);
		MovementPushingStatic = CVarDef.Create<bool>("movement.pushing_static", true, (CVar)10, (string)null);
		MovementPushingVelocityProduct = CVarDef.Create<float>("movement.pushing_velocity_product", -9999f, (CVar)10, (string)null);
		MovementPushingCap = CVarDef.Create<float>("movement.pushing_cap", 25f, (CVar)10, (string)null);
		MovementMinimumPush = CVarDef.Create<float>("movement.minimum_push", 0f, (CVar)10, (string)null);
		MovementPenetrationCap = CVarDef.Create<float>("movement.penetration_cap", 0.5f, (CVar)10, (string)null);
		MovementPushMassCap = CVarDef.Create<float>("movement.push_mass_cap", 1.75f, (CVar)10, (string)null);
		NetAtmosDebugOverlayTickRate = CVarDef.Create<float>("net.atmosdbgoverlaytickrate", 3f, (CVar)0, (string)null);
		NetGasOverlayTickRate = CVarDef.Create<float>("net.gasoverlaytickrate", 3f, (CVar)0, (string)null);
		GasOverlayThresholds = CVarDef.Create<int>("net.gasoverlaythresholds", 20, (CVar)0, (string)null);
		NPCMaxUpdates = CVarDef.Create<int>("npc.max_updates", 128, (CVar)0, (string)null);
		NPCEnabled = CVarDef.Create<bool>("npc.enabled", true, (CVar)0, (string)null);
		NPCPathfinding = CVarDef.Create<bool>("npc.pathfinding", true, (CVar)0, (string)null);
		ParallaxEnabled = CVarDef.Create<bool>("parallax.enabled", true, (CVar)128, (string)null);
		ParallaxDebug = CVarDef.Create<bool>("parallax.debug", false, (CVar)128, (string)null);
		ParallaxLowQuality = CVarDef.Create<bool>("parallax.low_quality", false, (CVar)144, (string)null);
		RelativeMovement = CVarDef.Create<bool>("physics.relative_movement", true, (CVar)26, (string)null);
		MinFriction = CVarDef.Create<float>("physics.min_friction", 0f, (CVar)26, (string)null);
		AirFriction = CVarDef.Create<float>("physics.air_friction", 0.2f, (CVar)26, (string)null);
		OffgridFriction = CVarDef.Create<float>("physics.offgrid_friction", 0.05f, (CVar)26, (string)null);
		TileFrictionModifier = CVarDef.Create<float>("physics.tile_friction", 8f, (CVar)26, (string)null);
		StopSpeed = CVarDef.Create<float>("physics.stop_speed", 0.1f, (CVar)26, (string)null);
		PlaytestAllDamageModifier = CVarDef.Create<float>("playtest.all_damage_modifier", 1f, (CVar)10, (string)null);
		PlaytestAllHealModifier = CVarDef.Create<float>("playtest.all_heal_modifier", 1f, (CVar)10, (string)null);
		PlaytestMeleeDamageModifier = CVarDef.Create<float>("playtest.melee_damage_modifier", 1f, (CVar)10, (string)null);
		PlaytestProjectileDamageModifier = CVarDef.Create<float>("playtest.projectile_damage_modifier", 1f, (CVar)10, (string)null);
		PlaytestHitscanDamageModifier = CVarDef.Create<float>("playtest.hitscan_damage_modifier", 1f, (CVar)10, (string)null);
		PlaytestThrownDamageModifier = CVarDef.Create<float>("playtest.thrown_damage_modifier", 1f, (CVar)10, (string)null);
		PlaytestTopicalsHealModifier = CVarDef.Create<float>("playtest.topicals_heal_modifier", 1f, (CVar)10, (string)null);
		PlaytestReagentDamageModifier = CVarDef.Create<float>("playtest.reagent_damage_modifier", 1f, (CVar)10, (string)null);
		PlaytestReagentHealModifier = CVarDef.Create<float>("playtest.reagent_heal_modifier", 1f, (CVar)10, (string)null);
		PlaytestExplosionDamageModifier = CVarDef.Create<float>("playtest.explosion_damage_modifier", 1f, (CVar)10, (string)null);
		PlaytestMobDamageModifier = CVarDef.Create<float>("playtest.mob_damage_modifier", 1f, (CVar)10, (string)null);
		PlaytestStaminaDamageModifier = CVarDef.Create<float>("playtest.stamina_damage_modifier", 1f, (CVar)10, (string)null);
		RadiationMinIntensity = CVarDef.Create<float>("radiation.min_intensity", 0.1f, (CVar)64, (string)null);
		RadiationGridcastUpdateRate = CVarDef.Create<float>("radiation.gridcast.update_rate", 1f, (CVar)64, (string)null);
		RadiationGridcastSimplifiedSameGrid = CVarDef.Create<bool>("radiation.gridcast.simplified_same_grid", true, (CVar)64, (string)null);
		RadiationGridcastMaxDistance = CVarDef.Create<float>("radiation.gridcast.max_distance", 50f, (CVar)64, (string)null);
		ReplayRecordAdminChat = CVarDef.Create<bool>("replay.record_admin_chat", false, (CVar)16, (string)null);
		ReplayAutoRecord = CVarDef.Create<bool>("replay.auto_record", false, (CVar)64, (string)null);
		ReplayAutoRecordName = CVarDef.Create<string>("replay.auto_record_name", "{year}_{month}_{day}-{hour}_{minute}-round_{round}.zip", (CVar)64, (string)null);
		ReplayAutoRecordTempDir = CVarDef.Create<string>("replay.auto_record_temp_dir", "", (CVar)64, (string)null);
		SalvageExpeditionDuration = CVarDef.Create<float>("salvage.expedition_duration", 660f, (CVar)8, (string)null);
		SalvageExpeditionCooldown = CVarDef.Create<float>("salvage.expedition_cooldown", 780f, (CVar)8, (string)null);
		ServerId = CVarDef.Create<string>("server.id", "unknown_server_id", (CVar)10, (string)null);
		RulesFile = CVarDef.Create<string>("server.rules_file", "DefaultRuleset", (CVar)10, (string)null);
		DefaultGuide = CVarDef.Create<string>("server.default_guide", "NewPlayer", (CVar)10, (string)null);
		ServerUptimeRestartMinutes = CVarDef.Create<int>("server.uptime_restart_minutes", 0, (CVar)64, (string)null);
		ServerLobbyName = CVarDef.Create<string>("server.lobby_name", "", (CVar)10, (string)null);
		ServerLobbyRightPanelWidth = CVarDef.Create<int>("server.lobby_right_panel_width", 650, (CVar)10, (string)null);
		ForceClientHudVersionWatermark = CVarDef.Create<bool>("server.force_client_hud_version_watermark", false, (CVar)10, (string)null);
		AutoOrientDelay = CVarDef.Create<double>("shuttle.auto_orient_delay", 2.0, (CVar)10, (string)null);
		CameraRotationLocked = CVarDef.Create<bool>("shuttle.camera_rotation_locked", true, (CVar)8, (string)null);
		ArrivalsPlanet = CVarDef.Create<bool>("shuttle.arrivals_planet", true, (CVar)64, (string)null);
		ArrivalsShuttles = CVarDef.Create<bool>("shuttle.arrivals", true, (CVar)64, (string)null);
		ArrivalsMap = CVarDef.Create<string>("shuttle.arrivals_map", "/Maps/Misc/terminal.yml", (CVar)64, (string)null);
		ArrivalsCooldown = CVarDef.Create<float>("shuttle.arrivals_cooldown", 50f, (CVar)64, (string)null);
		ArrivalsReturns = CVarDef.Create<bool>("shuttle.arrivals_returns", false, (CVar)64, (string)null);
		GodmodeArrivals = CVarDef.Create<bool>("shuttle.godmode_arrivals", false, (CVar)64, (string)null);
		HideSplitGridsUnder = CVarDef.Create<int>("shuttle.hide_split_grids_under", 30, (CVar)64, (string)null);
		GridFill = CVarDef.Create<bool>("shuttle.grid_fill", true, (CVar)64, (string)null);
		PreloadGrids = CVarDef.Create<bool>("shuttle.preload_grids", true, (CVar)64, (string)null);
		FTLStartupTime = CVarDef.Create<float>("shuttle.startup_time", 5.5f, (CVar)64, (string)null);
		FTLTravelTime = CVarDef.Create<float>("shuttle.travel_time", 20f, (CVar)64, (string)null);
		FTLArrivalTime = CVarDef.Create<float>("shuttle.arrival_time", 5f, (CVar)64, (string)null);
		FTLCooldown = CVarDef.Create<float>("shuttle.cooldown", 10f, (CVar)64, (string)null);
		FTLMassLimit = CVarDef.Create<float>("shuttle.mass_limit", 300f, (CVar)64, (string)null);
		HyperspaceKnockdownTime = CVarDef.Create<float>("shuttle.hyperspace_knockdown_time", 5f, (CVar)64, (string)null);
		EmergencyEarlyLaunchAllowed = CVarDef.Create<bool>("shuttle.emergency_early_launch_allowed", false, (CVar)64, (string)null);
		EmergencyShuttleDockTime = CVarDef.Create<float>("shuttle.emergency_dock_time", 180f, (CVar)64, (string)null);
		EmergencyShuttleDockTimeMultiplierOtherDock = CVarDef.Create<float>("shuttle.emergency_dock_time_multiplier_other_dock", 1.6667f, (CVar)64, (string)null);
		EmergencyShuttleDockTimeMultiplierNoDock = CVarDef.Create<float>("shuttle.emergency_dock_time_multiplier_no_dock", 2f, (CVar)64, (string)null);
		EmergencyShuttleAuthorizeTime = CVarDef.Create<float>("shuttle.emergency_authorize_time", 10f, (CVar)64, (string)null);
		EmergencyShuttleMinTransitTime = CVarDef.Create<float>("shuttle.emergency_transit_time_min", 60f, (CVar)64, (string)null);
		EmergencyShuttleMaxTransitTime = CVarDef.Create<float>("shuttle.emergency_transit_time_max", 180f, (CVar)64, (string)null);
		EmergencyShuttleEnabled = CVarDef.Create<bool>("shuttle.emergency", true, (CVar)64, (string)null);
		EmergencyRecallTurningPoint = CVarDef.Create<float>("shuttle.recall_turning_point", 0.5f, (CVar)64, (string)null);
		EmergencyShuttleAutoCallTime = CVarDef.Create<int>("shuttle.auto_call_time", 0, (CVar)64, (string)null);
		EmergencyShuttleAutoCallExtensionTime = CVarDef.Create<int>("shuttle.auto_call_extension_time", 45, (CVar)64, (string)null);
		GridImpulseMultiplier = CVarDef.Create<float>("shuttle.grid_impulse_multiplier", 0.01f, (CVar)64, (string)null);
		ImpactEnabled = CVarDef.Create<bool>("shuttle.impact.enabled", true, (CVar)64, (string)null);
		MinimumImpactInertia = CVarDef.Create<float>("shuttle.impact.minimum_inertia", 250f, (CVar)64, (string)null);
		MinimumImpactVelocity = CVarDef.Create<float>("shuttle.impact.minimum_velocity", 15f, (CVar)64, (string)null);
		TileBreakEnergyMultiplier = CVarDef.Create<float>("shuttle.impact.tile_break_energy", 3000f, (CVar)64, (string)null);
		ImpactDamageMultiplier = CVarDef.Create<float>("shuttle.impact.damage_multiplier", 5E-05f, (CVar)64, (string)null);
		ImpactStructuralDamage = CVarDef.Create<float>("shuttle.impact.structural_damage", 5f, (CVar)64, (string)null);
		SparkEnergy = CVarDef.Create<float>("shuttle.impact.spark_energy", 2000000f, (CVar)64, (string)null);
		ImpactRadius = CVarDef.Create<float>("shuttle.impact.radius", 4f, (CVar)64, (string)null);
		ImpactSlowdown = CVarDef.Create<float>("shuttle.impact.slowdown", 8f, (CVar)64, (string)null);
		ImpactMinThrowVelocity = CVarDef.Create<float>("shuttle.impact.min_throw_velocity", 1f, (CVar)64, (string)null);
		ImpactMassBias = CVarDef.Create<float>("shuttle.impact.mass_bias", 0.65f, (CVar)64, (string)null);
		ImpactInertiaScaling = CVarDef.Create<float>("shuttle.impact.inertia_scaling", 0.5f, (CVar)64, (string)null);
		LobbyMusicEnabled = CVarDef.Create<bool>("ambience.lobby_music_enabled", true, (CVar)144, (string)null);
		EventMusicEnabled = CVarDef.Create<bool>("ambience.event_music_enabled", true, (CVar)144, (string)null);
		RestartSoundsEnabled = CVarDef.Create<bool>("ambience.restart_sounds_enabled", true, (CVar)144, (string)null);
		AdminSoundsEnabled = CVarDef.Create<bool>("audio.admin_sounds_enabled", true, (CVar)144, (string)null);
		BwoinkSoundEnabled = CVarDef.Create<bool>("audio.bwoink_sound_enabled", true, (CVar)144, (string)null);
		AdminChatSoundPath = CVarDef.Create<string>("audio.admin_chat_sound_path", "/Audio/Items/pop.ogg", (CVar)536, (string)null);
		AdminChatSoundVolume = CVarDef.Create<float>("audio.admin_chat_sound_volume", -5f, (CVar)536, (string)null);
		AHelpSound = CVarDef.Create<string>("audio.ahelp_sound", "/Audio/_RMC14/Effects/Admin/adminhelp.ogg", (CVar)144, (string)null);
		TipsEnabled = CVarDef.Create<bool>("tips.enabled", false, (CVar)0, (string)null);
		TipsDataset = CVarDef.Create<string>("tips.dataset", "RMC_Tips", (CVar)0, (string)null);
		TipFrequencyOutOfRound = CVarDef.Create<float>("tips.out_of_game_frequency", 30f, (CVar)0, (string)null);
		TipFrequencyInRound = CVarDef.Create<float>("tips.in_game_frequency", 1800f, (CVar)0, (string)null);
		LoginTipsDataset = CVarDef.Create<string>("tips.login_dataset", "RMC_Tips", (CVar)0, (string)null);
		TipsTippyChance = CVarDef.Create<float>("tips.tippy_chance", 0f, (CVar)0, (string)null);
		ViewportStretch = CVarDef.Create<bool>("viewport.stretch", true, (CVar)144, (string)null);
		ViewportFixedScaleFactor = CVarDef.Create<int>("viewport.fixed_scale_factor", 2, (CVar)144, (string)null);
		ViewportSnapToleranceMargin = CVarDef.Create<int>("viewport.snap_tolerance_margin", 64, (CVar)144, (string)null);
		ViewportSnapToleranceClip = CVarDef.Create<int>("viewport.snap_tolerance_clip", 32, (CVar)144, (string)null);
		ViewportScaleRender = CVarDef.Create<bool>("viewport.scale_render", true, (CVar)144, (string)null);
		ViewportMinimumWidth = CVarDef.Create<int>("viewport.minimum_width", 15, (CVar)10, (string)null);
		ViewportMaximumWidth = CVarDef.Create<int>("viewport.maximum_width", 21, (CVar)10, (string)null);
		ViewportWidth = CVarDef.Create<int>("viewport.width", 21, (CVar)144, (string)null);
		ViewportVerticalFit = CVarDef.Create<bool>("viewport.vertical_fit", true, (CVar)144, (string)null);
		VoteEnabled = CVarDef.Create<bool>("vote.enabled", true, (CVar)64, (string)null);
		VoteRestartEnabled = CVarDef.Create<bool>("vote.restart_enabled", true, (CVar)64, (string)null);
		VoteRestartMaxPlayers = CVarDef.Create<int>("vote.restart_max_players", 20, (CVar)64, (string)null);
		VoteRestartGhostPercentage = CVarDef.Create<int>("vote.restart_ghost_percentage", 55, (CVar)64, (string)null);
		VotePresetEnabled = CVarDef.Create<bool>("vote.preset_enabled", true, (CVar)64, (string)null);
		VoteMapEnabled = CVarDef.Create<bool>("vote.map_enabled", false, (CVar)64, (string)null);
		VoteRestartRequiredRatio = CVarDef.Create<float>("vote.restart_required_ratio", 0.85f, (CVar)64, (string)null);
		VoteRestartNotAllowedWhenAdminOnline = CVarDef.Create<bool>("vote.restart_not_allowed_when_admin_online", true, (CVar)64, (string)null);
		VoteSameTypeTimeout = CVarDef.Create<float>("vote.same_type_timeout", 240f, (CVar)64, (string)null);
		VoteTimerMap = CVarDef.Create<int>("vote.timermap", 40, (CVar)64, (string)null);
		VoteTimerRestart = CVarDef.Create<int>("vote.timerrestart", 60, (CVar)64, (string)null);
		VoteTimerPreset = CVarDef.Create<int>("vote.timerpreset", 40, (CVar)64, (string)null);
		VoteTimerAlone = CVarDef.Create<int>("vote.timeralone", 10, (CVar)64, (string)null);
		VotekickEnabled = CVarDef.Create<bool>("votekick.enabled", false, (CVar)64, (string)null);
		VotekickEligibleNumberRequirement = CVarDef.Create<int>("votekick.eligible_number", 5, (CVar)64, (string)null);
		VotekickInitiatorGhostRequirement = CVarDef.Create<bool>("votekick.initiator_ghost_requirement", true, (CVar)64, (string)null);
		VotekickInitiatorWhitelistedRequirement = CVarDef.Create<bool>("votekick.initiator_whitelist_requirement", true, (CVar)64, (string)null);
		VotekickInitiatorTimeRequirement = CVarDef.Create<bool>("votekick.initiator_time_requirement", false, (CVar)64, (string)null);
		VotekickVoterGhostRequirement = CVarDef.Create<bool>("votekick.voter_ghost_requirement", true, (CVar)64, (string)null);
		VotekickEligibleVoterPlaytime = CVarDef.Create<int>("votekick.voter_playtime", 100, (CVar)64, (string)null);
		VotekickEligibleVoterDeathtime = CVarDef.Create<int>("votekick.voter_deathtime", 30, (CVar)10, (string)null);
		VotekickRequiredRatio = CVarDef.Create<float>("votekick.required_ratio", 0.6f, (CVar)64, (string)null);
		VotekickNotAllowedWhenAdminOnline = CVarDef.Create<bool>("votekick.not_allowed_when_admin_online", true, (CVar)64, (string)null);
		VotekickTimeout = CVarDef.Create<float>("votekick.timeout", 60f, (CVar)64, (string)null);
		VotekickTimer = CVarDef.Create<int>("votekick.timer", 45, (CVar)64, (string)null);
		VotekickAntagRaiderProtection = CVarDef.Create<int>("votekick.antag_raider_protection", 10, (CVar)64, (string)null);
		VotekickBanDefaultSeverity = CVarDef.Create<string>("votekick.ban_default_severity", "High", (CVar)26, (string)null);
		VotekickBanDuration = CVarDef.Create<int>("votekick.ban_duration", 180, (CVar)64, (string)null);
		VotekickIgnoreGhostReqInLobby = CVarDef.Create<bool>("votekick.ignore_ghost_req_in_lobby", true, (CVar)64, (string)null);
		WhitelistEnabled = CVarDef.Create<bool>("whitelist.enabled", false, (CVar)64, (string)null);
		WhitelistPrototypeList = CVarDef.Create<string>("whitelist.prototype_list", "basicWhitelist", (CVar)64, (string)null);
		WorldgenEnabled = CVarDef.Create<bool>("worldgen.enabled", false, (CVar)64, (string)null);
		WorldgenConfig = CVarDef.Create<string>("worldgen.worldgen_config", "Default", (CVar)64, (string)null);
		ModeMenuPubgEnabled = CVarDef.Create<bool>("game.mode_menu_pubg_enabled", false, (CVar)80, (string)null);
		ModeMenuCiv14Enabled = CVarDef.Create<bool>("game.mode_menu_civ14_enabled", true, (CVar)80, (string)null);
		Civ14LobbyDuration = CVarDef.Create<int>("game.civ14_lobby_duration", 222, (CVar)80, (string)null);
		Civ14PostRoundLobbyDelay = CVarDef.Create<int>("game.civ14_post_round_lobby_delay", 60, (CVar)80, (string)null);
		Civ14ShowNearbyNames = CVarDef.Create<bool>("ui.civ14_show_nearby_names", false, (CVar)144, (string)null);
		Civ14ShowLeaderArrow = CVarDef.Create<bool>("ui.civ14_show_leader_arrow", true, (CVar)144, (string)null);
		Civ14ShowOrderArrows = CVarDef.Create<bool>("ui.civ14_show_order_arrows", true, (CVar)144, (string)null);
		Civ14ShowForeignNames = CVarDef.Create<bool>("ui.civ14_show_foreign_names", true, (CVar)144, (string)null);
		Civ14ForeignNamesPromptSeen = CVarDef.Create<bool>("ui.civ14_foreign_names_prompt_seen", false, (CVar)144, (string)null);
		Civ14SkillBalanceEnabled = CVarDef.Create<bool>("game.civ14_skill_balance", true, (CVar)80, (string)null);
		Civ14SkillBalanceBlockThreshold = CVarDef.Create<float>("game.civ14_skill_balance_block", 1.3f, (CVar)80, (string)null);
		Civ14SkillBalanceSlot1Threshold = CVarDef.Create<float>("game.civ14_skill_balance_slot1", 1.2f, (CVar)80, (string)null);
		Civ14SkillBalanceSlot2Threshold = CVarDef.Create<float>("game.civ14_skill_balance_slot2", 1.4f, (CVar)80, (string)null);
		Civ14SkillBalanceSlot3Threshold = CVarDef.Create<float>("game.civ14_skill_balance_slot3", 1.6f, (CVar)80, (string)null);
		Civ14SkillBalanceNewbieRatio = CVarDef.Create<float>("game.civ14_skill_balance_newbie_ratio", 0.5f, (CVar)80, (string)null);
		CivStatsApiUrl = CVarDef.Create<string>("game.civ_stats_api_url", "http://localhost:3331", (CVar)336, (string)null);
		CivStatsApiKey = CVarDef.Create<string>("game.civ_stats_api_key", "", (CVar)336, (string)null);
		Civ14TeamRadioHighlightWords = CVarDef.Create<string>("game.civ14_team_radio_highlight", "Кома", (CVar)80, (string)null);
		Civ14CommanderMinPlaytimeMinutes = CVarDef.Create<int>("game.civ14_commander_min_playtime", 300, (CVar)80, (string)null);
		PubgLobbyReadyCountdown = CVarDef.Create<int>("game.pubg_lobby_countdown", 400, (CVar)80, (string)null);
		PubgMinimapZoom = CVarDef.Create<float>("pubg.minimap_zoom", 5f, (CVar)144, (string)null);
		PubgMinimapOpacity = CVarDef.Create<float>("game.pubg_minimap_opacity", 0.25f, (CVar)144, (string)null);
		PubgMinimapWidgetSize = CVarDef.Create<int>("pubg.minimap_widget_size", 200, (CVar)144, (string)null);
		PubgMinimapOffsetX = CVarDef.Create<int>("pubg.minimap_offset_x", 0, (CVar)144, (string)null);
		PubgMinimapOffsetY = CVarDef.Create<int>("pubg.minimap_offset_y", 0, (CVar)144, (string)null);
		PubgMinimapMarkerScale = CVarDef.Create<int>("pubg.minimap_marker_scale", 100, (CVar)144, (string)null);
		PubgGeneralAutoSettingsVersion = CVarDef.Create<int>("pubg.general_auto_settings_version", 0, (CVar)144, (string)null);
		PubgAutoClimbEnabled = CVarDef.Create<bool>("pubg.auto_climb_enabled", true, (CVar)536, (string)null);
		PubgPartyHudEnabled = CVarDef.Create<bool>("pubg.party_hud_enabled", true, (CVar)144, (string)null);
		PubgPartyMarkersEnabled = CVarDef.Create<bool>("pubg.party_markers_enabled", true, (CVar)144, (string)null);
		PubgPartyMarkersOpacity = CVarDef.Create<float>("pubg.party_markers_opacity", 1f, (CVar)144, (string)null);
		PubgShootingScreenShakeIntensity = CVarDef.Create<float>("pubg.shooting_screen_shake_intensity", 0.5f, (CVar)144, (string)null);
		PubgHealthHudOffsetY = CVarDef.Create<int>("pubg.health_hud_offset_y", 0, (CVar)144, (string)null);
		PubgHealthHudOffsetX = CVarDef.Create<int>("pubg.health_hud_offset_x", 0, (CVar)144, (string)null);
		PubgPartyHudOffsetY = CVarDef.Create<int>("pubg.party_hud_offset_y", 0, (CVar)144, (string)null);
		PubgStatsApiUrl = CVarDef.Create<string>("game.pubg_stats_api_url", "http://localhost:3331", (CVar)336, (string)null);
		PubgStatsApiKey = CVarDef.Create<string>("game.pubg_stats_api_key", "", (CVar)336, (string)null);
		PubgSpawnLogEnabled = CVarDef.Create<bool>("game.pubg_spawn_log_enabled", true, (CVar)80, (string)null);
		PubgAllowGhostsOnGameMap = CVarDef.Create<bool>("game.pubg_allow_ghosts_on_game_map", true, (CVar)80, (string)null);
		PubgQueueEnabled = CVarDef.Create<bool>("game.pubg_queue_enabled", false, (CVar)80, (string)null);
		PubgAdminHideAdminTag = CVarDef.Create<bool>("pubg.admin_hide_admin_tag", false, (CVar)536, (string)null);
		PubgDiscordVoiceCategoryId = CVarDef.Create<long>("pubg.discord_voice_category_id", 0L, (CVar)80, (string)null);
		PubgDiscordVoiceMainChannelId = CVarDef.Create<long>("pubg.discord_voice_main_channel_id", 0L, (CVar)80, (string)null);
		PubgSingle = CVarDef.Create<bool>("pubg_single", true, (CVar)80, (string)null);
		PubgAllowNewInstances = CVarDef.Create<bool>("pubg_allow_new_instances", true, (CVar)80, (string)null);
		PubgMaxActiveGameMaps = CVarDef.Create<int>("pubg_max_active_game_maps", 5, (CVar)80, (string)null);
		PubgAllowModeSolo = CVarDef.Create<bool>("pubg_allow_mode_solo", true, (CVar)80, (string)null);
		PubgAllowModeDuo = CVarDef.Create<bool>("pubg_allow_mode_duo", false, (CVar)80, (string)null);
		PubgAllowModeSquad = CVarDef.Create<bool>("pubg_allow_mode_squad", false, (CVar)80, (string)null);
		PubgAllowModeFiftyFifty = CVarDef.Create<bool>("pubg_allow_mode_fiftyfifty", false, (CVar)80, (string)null);
		PubgAhelpSpamEnabled = CVarDef.Create<bool>("game.pubg_ahelp_spam_enabled", true, (CVar)80, (string)null);
		PubgAhelpSpamWindow1Minutes = CVarDef.Create<int>("game.pubg_ahelp_spam_window_1_minutes", 60, (CVar)80, (string)null);
		PubgAhelpSpamWindow2Minutes = CVarDef.Create<int>("game.pubg_ahelp_spam_window_2_minutes", 120, (CVar)80, (string)null);
		PubgAhelpSpamWindow3Minutes = CVarDef.Create<int>("game.pubg_ahelp_spam_window_3_minutes", 180, (CVar)80, (string)null);
		PubgAhelpSpamMute1Minutes = CVarDef.Create<int>("game.pubg_ahelp_spam_mute_1_minutes", 5, (CVar)80, (string)null);
		PubgAhelpSpamMute2Minutes = CVarDef.Create<int>("game.pubg_ahelp_spam_mute_2_minutes", 30, (CVar)80, (string)null);
		PubgAhelpSpamMute3Minutes = CVarDef.Create<int>("game.pubg_ahelp_spam_mute_3_minutes", 60, (CVar)80, (string)null);
		PubgTelemetryEnabled = CVarDef.Create<bool>("anticheat", true, (CVar)80, (string)null);
		PubgTelemetryIntervalSeconds = CVarDef.Create<int>("game.pubg_telemetry_interval_seconds", 60, (CVar)80, (string)null);
		PubgTelemetryTimeoutSeconds = CVarDef.Create<int>("game.pubg_telemetry_timeout_seconds", 120, (CVar)80, (string)null);
		PubgTelemetryMaxItems = CVarDef.Create<int>("game.pubg_telemetry_max_items", 256, (CVar)80, (string)null);
		PubgTelemetryMaxItemNameLength = CVarDef.Create<int>("game.pubg_telemetry_max_item_name_length", 96, (CVar)80, (string)null);
		PubgTelemetryModuleCloakAlertEnabled = CVarDef.Create<bool>("game.pubg_telemetry_module_cloak_alert_enabled", true, (CVar)80, (string)null);
		PubgTelemetryUnknownModuleAlertEnabled = CVarDef.Create<bool>("game.pubg_telemetry_unknown_module_alert_enabled", true, (CVar)80, (string)null);
		PubgTelemetryCoreMissingAlertEnabled = CVarDef.Create<bool>("game.pubg_telemetry_core_missing_alert_enabled", true, (CVar)80, (string)null);
		PubgTelemetryModuleZeroAlertEnabled = CVarDef.Create<bool>("game.pubg_telemetry_module_zero_alert_enabled", true, (CVar)80, (string)null);
		PubgTelemetryModuleZeroConfirmSamples = CVarDef.Create<int>("game.pubg_telemetry_module_zero_confirm_samples", 3, (CVar)80, (string)null);
		PubgTelemetryCoreMissingWarmupSamples = CVarDef.Create<int>("game.pubg_telemetry_core_missing_warmup_samples", 4, (CVar)80, (string)null);
		PubgTelemetryCoreMissingConfirmSamples = CVarDef.Create<int>("game.pubg_telemetry_core_missing_confirm_samples", 8, (CVar)80, (string)null);
		PubgTelemetrySystemCountDriftAlertEnabled = CVarDef.Create<bool>("game.pubg_telemetry_system_count_drift_alert_enabled", true, (CVar)80, (string)null);
		PubgTelemetrySystemCountDriftAbsolute = CVarDef.Create<int>("game.pubg_telemetry_system_count_drift_absolute", 12, (CVar)80, (string)null);
		PubgTelemetrySystemCountDriftRatio = CVarDef.Create<float>("game.pubg_telemetry_system_count_drift_ratio", 0.12f, (CVar)80, (string)null);
		PubgTelemetrySystemCountDriftConfirmSamples = CVarDef.Create<int>("game.pubg_telemetry_system_count_drift_confirm_samples", 2, (CVar)80, (string)null);
		PubgTelemetrySystemCountDriftWarmupSamples = CVarDef.Create<int>("game.pubg_telemetry_system_count_drift_warmup_samples", 4, (CVar)80, (string)null);
		PubgTelemetryFingerprintChurnAlertEnabled = CVarDef.Create<bool>("game.pubg_telemetry_fingerprint_churn_alert_enabled", true, (CVar)80, (string)null);
		PubgTelemetryFingerprintChurnWindowSeconds = CVarDef.Create<float>("game.pubg_telemetry_fingerprint_churn_window_seconds", 90f, (CVar)80, (string)null);
		PubgTelemetryFingerprintChurnChangesThreshold = CVarDef.Create<int>("game.pubg_telemetry_fingerprint_churn_changes_threshold", 4, (CVar)80, (string)null);
		PubgTelemetryBuildBaselineAlertEnabled = CVarDef.Create<bool>("game.pubg_telemetry_build_baseline_alert_enabled", true, (CVar)80, (string)null);
		PubgTelemetryBuildBaselineWarmupSamples = CVarDef.Create<int>("game.pubg_telemetry_build_baseline_warmup_samples", 4, (CVar)80, (string)null);
		PubgTelemetryBuildBaselineConfirmSamples = CVarDef.Create<int>("game.pubg_telemetry_build_baseline_confirm_samples", 2, (CVar)80, (string)null);
		PubgTelemetryBuildBaselineMinLearnSamples = CVarDef.Create<int>("game.pubg_telemetry_build_baseline_min_learn_samples", 2, (CVar)80, (string)null);
		PubgTelemetryBuildBaselineSystemSlack = CVarDef.Create<int>("game.pubg_telemetry_build_baseline_system_slack", 4, (CVar)80, (string)null);
		PubgTelemetryJoinReplyEnabled = CVarDef.Create<bool>("join_reply", true, (CVar)80, (string)null);
		PubgTelemetryJoinReplyTimeoutSeconds = CVarDef.Create<float>("game.pubg_telemetry_join_reply_timeout_seconds", 4f, (CVar)80, (string)null);
		PubgTelemetryJoinReplyMaxAttempts = CVarDef.Create<int>("game.pubg_telemetry_join_reply_max_attempts", 2, (CVar)80, (string)null);
		PubgTelemetryFocusDistanceThreshold = CVarDef.Create<float>("game.pubg_telemetry_focus_distance_threshold", 17f, (CVar)80, (string)null);
		PubgTelemetryClientRangeAlertCooldownSeconds = CVarDef.Create<int>("game.pubg_telemetry_client_range_alert_cooldown_seconds", 180, (CVar)80, (string)null);
		PubgTelemetryClientFovAlertCooldownSeconds = CVarDef.Create<int>("game.pubg_telemetry_client_fov_alert_cooldown_seconds", 180, (CVar)80, (string)null);
		PubgTelemetrySecretNotesEnabled = CVarDef.Create<bool>("game.pubg_telemetry_secret_notes_enabled", true, (CVar)80, (string)null);
		PubgTelemetrySecretNoteCooldownSeconds = CVarDef.Create<int>("game.pubg_telemetry_secret_note_cooldown_seconds", 300, (CVar)80, (string)null);
		PubgWeaponGunSignalEnabled = CVarDef.Create<bool>("game.pubg_weapon_gun_signal_enabled", true, (CVar)80, (string)null);
		PubgWeaponGunSignalDropAbsolute = CVarDef.Create<int>("game.pubg_weapon_gun_signal_drop_absolute", 1, (CVar)80, (string)null);
		PubgWeaponGunSignalDropRatio = CVarDef.Create<float>("game.pubg_weapon_gun_signal_drop_ratio", 0.005f, (CVar)80, (string)null);
		PubgWeaponGunSignalConfirmSamples = CVarDef.Create<int>("game.pubg_weapon_gun_signal_confirm_samples", 2, (CVar)80, (string)null);
		PubgCheaterShiftOffsetTiles = CVarDef.Create<float>("game.pubg_cheater_shift_offset_tiles", 1f, (CVar)80, (string)null);
		PubgCheaterShiftIntervalSeconds = CVarDef.Create<float>("game.pubg_cheater_shift_interval_seconds", 0.35f, (CVar)80, (string)null);
		PubgCheaterSyncLagDelaySeconds = CVarDef.Create<float>("game.pubg_cheater_sync_lag_delay_seconds", 1.35f, (CVar)80, (string)null);
		PubgCheaterSyncLagJitterSeconds = CVarDef.Create<float>("game.pubg_cheater_sync_lag_jitter_seconds", 0.55f, (CVar)80, (string)null);
		PubgCheaterSyncLagMaxOffsetTiles = CVarDef.Create<float>("game.pubg_cheater_sync_lag_max_offset_tiles", 4f, (CVar)80, (string)null);
		PubgTelemetrySuspicionThreshold = CVarDef.Create<int>("game.pubg_telemetry_suspicion_threshold", 3, (CVar)80, (string)null);
		PubgTelemetryServerRangeBehaviorEnabled = CVarDef.Create<bool>("game.pubg_telemetry_server_range_behavior_enabled", true, (CVar)80, (string)null);
		PubgTelemetryServerRangeTargetScanEnabled = CVarDef.Create<bool>("game.pubg_telemetry_server_range_target_scan_enabled", true, (CVar)80, (string)null);
		PubgTelemetryServerRangeTargetScanIntervalSeconds = CVarDef.Create<float>("game.pubg_telemetry_server_range_target_scan_interval_seconds", 0.2f, (CVar)80, (string)null);
		PubgTelemetryServerRangeTargetScanRange = CVarDef.Create<float>("game.pubg_telemetry_server_range_target_scan_range", 180f, (CVar)80, (string)null);
		PubgTelemetryServerRangeBurstWindowSeconds = CVarDef.Create<float>("game.pubg_telemetry_server_range_burst_window_seconds", 6f, (CVar)80, (string)null);
		PubgTelemetryServerRangeBurstHits = CVarDef.Create<int>("game.pubg_telemetry_server_range_burst_hits", 4, (CVar)80, (string)null);
		PubgTelemetryServerRangeBurstBonusPoints = CVarDef.Create<int>("game.pubg_telemetry_server_range_burst_bonus_points", 1, (CVar)80, (string)null);
		PubgTelemetryServerRangeSnapWindowSeconds = CVarDef.Create<float>("game.pubg_telemetry_server_range_snap_window_seconds", 0.35f, (CVar)80, (string)null);
		PubgTelemetryServerRangeSnapBonusPoints = CVarDef.Create<int>("game.pubg_telemetry_server_range_snap_bonus_points", 1, (CVar)80, (string)null);
		PubgTelemetryServerRangeReactionWindowSeconds = CVarDef.Create<float>("game.pubg_telemetry_server_range_reaction_window_seconds", 0.75f, (CVar)80, (string)null);
		PubgTelemetryServerRangeReactionBonusPoints = CVarDef.Create<int>("game.pubg_telemetry_server_range_reaction_bonus_points", 1, (CVar)80, (string)null);
		PubgTelemetryServerRangeSwitchWindowSeconds = CVarDef.Create<float>("game.pubg_telemetry_server_range_switch_window_seconds", 1.1f, (CVar)80, (string)null);
		PubgTelemetryServerRangeSwitchBonusPoints = CVarDef.Create<int>("game.pubg_telemetry_server_range_switch_bonus_points", 1, (CVar)80, (string)null);
		PubgTelemetryServerRangeSmokeBonusPoints = CVarDef.Create<int>("game.pubg_telemetry_server_range_smoke_bonus_points", 1, (CVar)80, (string)null);
		PubgTelemetryServerRangeOccludedBonusPoints = CVarDef.Create<int>("game.pubg_telemetry_server_range_occluded_bonus_points", 1, (CVar)80, (string)null);
		PubgTelemetryServerRangeClusterWindowSeconds = CVarDef.Create<float>("game.pubg_telemetry_server_range_cluster_window_seconds", 3.5f, (CVar)80, (string)null);
		PubgTelemetryServerRangeClusterHits = CVarDef.Create<int>("game.pubg_telemetry_server_range_cluster_hits", 4, (CVar)80, (string)null);
		PubgTelemetryServerRangeClusterBonusPoints = CVarDef.Create<int>("game.pubg_telemetry_server_range_cluster_bonus_points", 1, (CVar)80, (string)null);
		PubgWeaponGunProbeEnabled = CVarDef.Create<bool>("game.pubg_weapon_gun_probe_enabled", true, (CVar)80, (string)null);
		PubgWeaponGunProbeIntervalSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_probe_interval_seconds", 18f, (CVar)80, (string)null);
		PubgWeaponGunProbeTimeoutSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_probe_timeout_seconds", 7f, (CVar)80, (string)null);
		PubgWeaponGunProbeSessionDecoyEnabled = CVarDef.Create<bool>("game.pubg_weapon_gun_probe_session_decoy_enabled", true, (CVar)80, (string)null);
		PubgWeaponGunProbeSessionDecoyPrefix = CVarDef.Create<string>("game.pubg_weapon_gun_probe_session_decoy_prefix", "wmcache,sysreg,xcache,pcfg,tmpres,nodecache,layoutcfg,framemap,uicfg,glref", (CVar)80, (string)null);
		PubgWeaponGunScreenEnabled = CVarDef.Create<bool>("game.pubg_weapon_gun_screen_enabled", true, (CVar)80, (string)null);
		PubgWeaponGunScreenAutoOnAlert = CVarDef.Create<bool>("game.pubg_weapon_gun_screen_auto_on_alert", true, (CVar)80, (string)null);
		PubgWeaponGunScreenAutoCooldownSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_screen_auto_cooldown_seconds", 180f, (CVar)80, (string)null);
		PubgWeaponGunScreenAutoBurstCount = CVarDef.Create<int>("game.pubg_weapon_gun_screen_auto_burst_count", 3, (CVar)80, (string)null);
		PubgWeaponGunScreenBurstJitterSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_screen_burst_jitter_seconds", 0.35f, (CVar)80, (string)null);
		PubgWeaponGunScreenRequestCooldownSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_screen_request_cooldown_seconds", 12f, (CVar)80, (string)null);
		PubgWeaponGunScreenTimeoutSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_screen_timeout_seconds", 20f, (CVar)80, (string)null);
		PubgWeaponGunScreenMaxBytes = CVarDef.Create<int>("game.pubg_weapon_gun_screen_max_bytes", 900000, (CVar)80, (string)null);
		PubgWeaponGunScreenStorePerPlayer = CVarDef.Create<int>("game.pubg_weapon_gun_screen_store_per_player", 4, (CVar)80, (string)null);
		PubgWeaponGunScreenPressureEnabled = CVarDef.Create<bool>("game.pubg_weapon_gun_screen_pressure_enabled", true, (CVar)80, (string)null);
		PubgWeaponGunScreenPressureDefaultSeconds = CVarDef.Create<int>("game.pubg_weapon_gun_screen_pressure_default_seconds", 20, (CVar)80, (string)null);
		PubgWeaponGunScreenPressureMaxSeconds = CVarDef.Create<int>("game.pubg_weapon_gun_screen_pressure_max_seconds", 45, (CVar)80, (string)null);
		PubgWeaponGunScreenPressureDefaultHz = CVarDef.Create<int>("game.pubg_weapon_gun_screen_pressure_default_hz", 3, (CVar)80, (string)null);
		PubgWeaponGunScreenPressureMaxHz = CVarDef.Create<int>("game.pubg_weapon_gun_screen_pressure_max_hz", 6, (CVar)80, (string)null);
		PubgWeaponGunScreenPressurePayloadEvery = CVarDef.Create<int>("game.pubg_weapon_gun_screen_pressure_payload_every", 3, (CVar)80, (string)null);
		PubgWeaponGunScreenLivenessEnabled = CVarDef.Create<bool>("game.pubg_weapon_gun_screen_liveness_enabled", true, (CVar)80, (string)null);
		PubgWeaponGunScreenLivenessGrid = CVarDef.Create<int>("game.pubg_weapon_gun_screen_liveness_grid", 6, (CVar)80, (string)null);
		PubgWeaponGunScreenLivenessSizePercent = CVarDef.Create<int>("game.pubg_weapon_gun_screen_liveness_size_percent", 8, (CVar)80, (string)null);
		PubgWeaponGunCatalogEnabled = CVarDef.Create<bool>("profile_catalog", true, (CVar)80, (string)null);
		PubgWeaponGunCatalogAutoOnAlert = CVarDef.Create<bool>("game.pubg_weapon_gun_catalog_auto_on_alert", true, (CVar)80, (string)null);
		PubgWeaponGunCatalogCooldownSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_catalog_cooldown_seconds", 240f, (CVar)80, (string)null);
		PubgWeaponGunCatalogTimeoutSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_catalog_timeout_seconds", 60f, (CVar)80, (string)null);
		PubgWeaponGunCatalogChunkStepSeconds = CVarDef.Create<float>("game.pubg_weapon_gun_catalog_chunk_step_seconds", 0.25f, (CVar)80, (string)null);
		PubgWeaponGunCatalogChunkBytes = CVarDef.Create<int>("game.pubg_weapon_gun_catalog_chunk_bytes", 24000, (CVar)80, (string)null);
		PubgWeaponGunCatalogMaxBytes = CVarDef.Create<int>("game.pubg_weapon_gun_catalog_max_bytes", 4000000, (CVar)80, (string)null);
		PubgTelemetryIncidentGroupingEnabled = CVarDef.Create<bool>("game.pubg_telemetry_incident_grouping_enabled", true, (CVar)80, (string)null);
		PubgTelemetryIncidentWindowSeconds = CVarDef.Create<int>("game.pubg_telemetry_incident_window_seconds", 180, (CVar)80, (string)null);
		PubgTelemetryIncidentSecondaryLimit = CVarDef.Create<int>("game.pubg_telemetry_incident_secondary_limit", 4, (CVar)80, (string)null);
		PubgTelemetryIncidentFrameLimit = CVarDef.Create<int>("game.pubg_telemetry_incident_frame_limit", 4, (CVar)80, (string)null);
		BattlefrontScreenChatSize = CVarDef.Create<string>("ui.battlefront_chat_size", "", (CVar)144, (string)null);
		PubgBoomboxSoundEnabled = CVarDef.Create<bool>("pubg.boombox_sound_enabled", true, (CVar)144, (string)null);
		PubgBoomboxVolume = CVarDef.Create<float>("pubg.boombox_volume", 1f, (CVar)144, (string)null);
		PubgBoomboxChunkBytes = CVarDef.Create<int>("game.pubg_boombox_chunk_bytes", 48000, (CVar)80, (string)null);
		PubgBoomboxChunkIntervalSeconds = CVarDef.Create<float>("game.pubg_boombox_chunk_interval_seconds", 0.2f, (CVar)80, (string)null);
	}
}
