using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._RMC14.CCVar;

[CVarDefs]
public sealed class RMCCVars : CVars
{
	public static readonly CVarDef<float> CMXenoDamageDealtMultiplier = CVarDef.Create<float>("rmc.xeno_damage_dealt_multiplier", 1f, (CVar)2, (string)null);

	public static readonly CVarDef<float> CMXenoDamageReceivedMultiplier = CVarDef.Create<float>("rmc.xeno_damage_received_multiplier", 1f, (CVar)2, (string)null);

	public static readonly CVarDef<float> CMXenoSpeedMultiplier = CVarDef.Create<float>("rmc.xeno_speed_multiplier", 1f, (CVar)2, (string)null);

	public static readonly CVarDef<bool> RMCAutoPunctuate = CVarDef.Create<bool>("rmc.auto_punctuate", false, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCAutoEjectMagazines = CVarDef.Create<bool>("rmc.auto_eject_magazines", true, (CVar)536, (string)null);

	public static readonly CVarDef<string> CMOocWebhook = CVarDef.Create<string>("rmc.ooc_webhook", "", (CVar)320, (string)null);

	public static readonly CVarDef<int> CMMaxHeavyAttackTargets = CVarDef.Create<int>("rmc.max_heavy_attack_targets", 1, (CVar)10, (string)null);

	public static readonly CVarDef<float> CMBloodlossMultiplier = CVarDef.Create<float>("rmc.bloodloss_multiplier", 1.5f, (CVar)10, (string)null);

	public static readonly CVarDef<float> CMBleedTimeMultiplier = CVarDef.Create<float>("rmc.bleed_time_multiplier", 1f, (CVar)10, (string)null);

	public static readonly CVarDef<float> CMMarinesPerXeno = CVarDef.Create<float>("rmc.marines_per_xeno", 3f, (CVar)66, (string)null);

	public static readonly CVarDef<bool> RMCAutoBalance = CVarDef.Create<bool>("rmc.auto_balance", true, (CVar)66, (string)null);

	public static readonly CVarDef<float> RMCAutoBalanceStep = CVarDef.Create<float>("rmc.auto_balance_step", 1f, (CVar)66, (string)null);

	public static readonly CVarDef<float> RMCAutoBalanceMin = CVarDef.Create<float>("rmc.auto_balance_min", 3f, (CVar)66, (string)null);

	public static readonly CVarDef<float> RMCAutoBalanceMax = CVarDef.Create<float>("rmc.auto_balance_max", 6.5f, (CVar)66, (string)null);

	public static readonly CVarDef<int> RMCPatronLobbyMessageTimeSeconds = CVarDef.Create<int>("rmc.patron_lobby_message_time_seconds", 30, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPatronLobbyMessageInitialDelaySeconds = CVarDef.Create<int>("rmc.patron_lobby_message_initial_delay_seconds", 5, (CVar)10, (string)null);

	public static readonly CVarDef<string> RMCDiscordAccountLinkingMessageLink = CVarDef.Create<string>("rmc.discord_account_linking_message_link", "", (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCRequisitionsStartingBalance = CVarDef.Create<int>("rmc.requisitions_starting_balance", 0, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCRequisitionsBalanceGain = CVarDef.Create<int>("rmc.requisitions_balance_gain", 150, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCRequisitionsStartingDollarsPerMarine = CVarDef.Create<int>("rmc.requisitions_starting_dollars_per_marine", 0, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCRequisitionsPointsScale = CVarDef.Create<int>("rmc.requisitions_points_scale", 12000, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCRequisitionsFreeCratesXenoDivider = CVarDef.Create<int>("rmc.requisitions_free_crates_xeno_divider", 4, (CVar)2, (string)null);

	public static readonly CVarDef<string> RMCDiscordToken = CVarDef.Create<string>("rmc.discord_token", "", (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordAdminChatChannel = CVarDef.Create<long>("rmc.discord_admin_chat_channel", 0L, (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordMentorChatChannel = CVarDef.Create<long>("rmc.discord_mentor_chat_channel", 0L, (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordAHelpChannel = CVarDef.Create<long>("rmc.discord_ahelp_channel", 0L, (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordOOCChannel = CVarDef.Create<long>("rmc.discord_ooc_channel", 0L, (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordPartyChatChannel = CVarDef.Create<long>("rmc.discord_party_chat_channel", 0L, (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordLobbyChatChannel = CVarDef.Create<long>("rmc.discord_lobby_chat_channel", 0L, (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordPubgMapChannel = CVarDef.Create<long>("rmc.discord_pubg_map_channel", 1469338644610416802L, (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordAnticheatAlertChannel = CVarDef.Create<long>("rmc.discord_anticheat_alert_channel", 1476142054018121730L, (CVar)322, (string)null);

	public static readonly CVarDef<long> RMCDiscordAnticheatImageChannel = CVarDef.Create<long>("rmc.discord_anticheat_image_channel", 1476142054018121730L, (CVar)322, (string)null);

	public static readonly CVarDef<bool> RMCDiscordSendServerAnnouncements = CVarDef.Create<bool>("rmc.discord_send_server_announcements", true, (CVar)66, (string)null);

	public static readonly CVarDef<int> RMCPlanetCoordinateVariance = CVarDef.Create<int>("rmc.planet_coordinate_variance", 500, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCDrawStorageIconLabels = CVarDef.Create<bool>("rmc.draw_storage_icon_labels", true, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCFTLCrashLand = CVarDef.Create<bool>("rmc.ftl_crash_land", true, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCDropshipInitialDelayMinutes = CVarDef.Create<float>("rmc.dropship_initial_delay_minutes", 15f, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCDropshipHijackInitialDelayMinutes = CVarDef.Create<int>("rmc.dropship_hijack_initial_delay_minutes", 40, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCLandingZonePrimaryAutoMinutes = CVarDef.Create<float>("rmc.landing_zone_primary_auto_minutes", 25f, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCLandingZoneMiasmaEnabled = CVarDef.Create<bool>("rmc.landing_zone_miasma_enabled", false, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCCorrosiveAcidTickDelaySeconds = CVarDef.Create<int>("rmc.corrosive_acid_tick_delay_seconds", 10, (CVar)10, (string)null);

	public static readonly CVarDef<string> RMCCorrosiveAcidDamageType = CVarDef.Create<string>("rmc.corrosive_acid_damage_type", "Heat", (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCCorrosiveAcidDamageTimeSeconds = CVarDef.Create<int>("rmc.corrosive_acid_damage_time_seconds", 40, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCEvolutionPointsRequireOvipositorMinutes = CVarDef.Create<int>("rmc.evolution_points_require_ovipositor_minutes", 5, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCEvolutionPointsAccumulateBeforeMinutes = CVarDef.Create<int>("rmc.evolution_points_accumulate_before_minutes", 15, (CVar)2, (string)null);

	public static readonly CVarDef<bool> RMCAtmosTileEqualize = CVarDef.Create<bool>("rmc.atmos_tile_equalize", false, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCGasTileOverlayUpdate = CVarDef.Create<bool>("rmc.gas_tile_overlay_update", false, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCActiveInputMoverEnabled = CVarDef.Create<bool>("rmc.active_input_mover_enabled", true, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCBioscanInitialDelaySeconds = CVarDef.Create<int>("rmc.bioscan_initial_delay_seconds", 300, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCBioscanCheckDelaySeconds = CVarDef.Create<int>("rmc.bioscan_check_delay_seconds", 60, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCBioscanMinimumCooldownSeconds = CVarDef.Create<int>("rmc.bioscan_minimum_cooldown_seconds", 300, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCBioscanBaseCooldownSeconds = CVarDef.Create<int>("rmc.bioscan_base_cooldown_seconds", 1800, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCBioscanVariance = CVarDef.Create<int>("rmc.bioscan_variance", 2, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCDropshipFabricatorStartingPoints = CVarDef.Create<int>("rmc.dropship_fabricator_starting_points", 10000, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCDropshipFabricatorGainEverySeconds = CVarDef.Create<float>("rmc.dropship_fabricator_gain_every_seconds", 3.33333f, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCDropshipCASDebug = CVarDef.Create<bool>("rmc.dropship_cas_debug", false, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCDropshipFlyByTimeSeconds = CVarDef.Create<int>("rmc.dropship_fly_by_time_seconds", 100, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCDropshipHijackTravelTimeSeconds = CVarDef.Create<int>("rmc.dropship_hijack_travel_time_seconds", 180, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCEntitiesLogDelete = CVarDef.Create<bool>("rmc.entities_log_delete", false, (CVar)66, (string)null);

	public static readonly CVarDef<bool> RMCPlanetMapVote = CVarDef.Create<bool>("rmc.planet_map_vote", true, (CVar)66, (string)null);

	public static readonly CVarDef<int> RMCPlanetMapVoteExcludeLast = CVarDef.Create<int>("rmc.planet_map_vote_exclude_last", 2, (CVar)66, (string)null);

	public static readonly CVarDef<bool> RMCUseCarryoverVoting = CVarDef.Create<bool>("rmc.planet_map_vote_carryover", true, (CVar)66, (string)null);

	public static readonly CVarDef<int> RMCTacticalMapAnnounceCooldownSeconds = CVarDef.Create<int>("rmc.tactical_map_announce_cooldown_seconds", 240, (CVar)66, (string)null);

	public static readonly CVarDef<int> RMCTacticalMapLineLimit = CVarDef.Create<int>("rmc.tactical_map_line_limit", 1000, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCTacticalMapAdminHistorySize = CVarDef.Create<int>("rmc.tactical_map_admin_history_size", 100, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCTacticalMapUpdateEverySeconds = CVarDef.Create<float>("rmc.tactical_map_update_every_seconds", 0.5f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCTacticalMapForceUpdateEverySeconds = CVarDef.Create<float>("rmc.tactical_map_force_update_every_seconds", 30f, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCTacticalMapShowAreaLabels = CVarDef.Create<bool>("rmc.tactical_map_show_area_labels", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCGunPrediction = CVarDef.Create<bool>("rmc.gun_prediction", true, (CVar)10, (string)null);

	public static readonly CVarDef<bool> VehicleDebugOverlay = CVarDef.Create<bool>("rmc.vehicle_debug_overlay", false, (CVar)528, (string)null);

	public static readonly CVarDef<bool> VehicleCollisionOverlay = CVarDef.Create<bool>("rmc.vehicle_collision_overlay", false, (CVar)528, (string)null);

	public static readonly CVarDef<bool> VehicleMovementOverlay = CVarDef.Create<bool>("rmc.vehicle_movement_overlay", false, (CVar)528, (string)null);

	public static readonly CVarDef<bool> VehicleHardpointOverlay = CVarDef.Create<bool>("rmc.vehicle_hardpoint_overlay", false, (CVar)528, (string)null);

	public static readonly CVarDef<bool> RMCGunPredictionPreventCollision = CVarDef.Create<bool>("rmc.gun_prediction_prevent_collision", false, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCGunPredictionLogHits = CVarDef.Create<bool>("rmc.gun_prediction_log_hits", false, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCGunPredictionCoordinateDeviation = CVarDef.Create<float>("rmc.gun_prediction_coordinate_deviation", 3f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCGunPredictionLowestCoordinateDeviation = CVarDef.Create<float>("rmc.gun_prediction_lowest_coordinate_deviation", 3f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCGunPredictionAabbEnlargement = CVarDef.Create<float>("rmc.gun_prediction_aabb_enlargement", 1.5f, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCJobSlotScaling = CVarDef.Create<bool>("rmc.job_slot_scaling", true, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCEmoteCooldownSeconds = CVarDef.Create<float>("rmc.emote_cooldown_seconds", 20f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCPowerUpdateEverySeconds = CVarDef.Create<float>("rmc.power_update_every_seconds", 1f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCPowerLoadMultiplier = CVarDef.Create<float>("rmc.power_load_multiplier", 0.01f, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCMarinesPerSurvivor = CVarDef.Create<int>("rmc.marines_per_survivor", 18, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCSurvivorsMinimum = CVarDef.Create<int>("rmc.survivors_minimum", 2, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCSurvivorsMaximum = CVarDef.Create<int>("rmc.survivors_maximum", 7, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCSpawnerMaxCorpses = CVarDef.Create<int>("rmc.spawner_max_corpses", 100, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCHiveSpreadEarlyMinutes = CVarDef.Create<int>("rmc.hive_spread_early_minutes", 0, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCNewPlayerTimeTotalHours = CVarDef.Create<int>("rmc.new_player_time_total_hours", 25, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCNewPlayerTimeJobHours = CVarDef.Create<int>("rmc.new_player_time_job_hours", 10, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCBrandNewPlayerTimeJobHours = CVarDef.Create<int>("rmc.brand_new_player_time_job_hours", 1, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCLateJoinsPerBurrowedLarvaEarlyThresholdMinutes = CVarDef.Create<float>("rmc.late_joins_per_burrowed_larva_early_threshold_minutes", 15f, (CVar)2, (string)null);

	public static readonly CVarDef<float> RMCLateJoinsPerBurrowedLarvaEarly = CVarDef.Create<float>("rmc.late_joins_per_burrowed_larva_early", 7.5f, (CVar)66, (string)null);

	public static readonly CVarDef<float> RMCLateJoinsPerBurrowedLarva = CVarDef.Create<float>("rmc.late_joins_per_burrowed_larva", 7f, (CVar)66, (string)null);

	public static readonly CVarDef<float> RMCLateJoinsBurrowedLarvaDeathTime = CVarDef.Create<float>("rmc.late_joins_burrowed_larva_death_time", 2.5f, (CVar)2, (string)null);

	public static readonly CVarDef<float> RMCLateJoinsBurrowedLarvaDeathTimeIgnoreBeforeMinutes = CVarDef.Create<float>("rmc.late_joins_burrowed_larva_death_time_ignore_before_minutes", 2.5f, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCBurrowedLarvaSacrificeTimeMinutes = CVarDef.Create<int>("rmc.burrowed_larva_sacrifice_time_minutes", 15, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCBurrowedLarvaEvolutionPointsPer = CVarDef.Create<int>("rmc.burrowed_larva_evolution_points_per", 250, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeBronzeMedalTimeHours = CVarDef.Create<int>("rmc.playtime_bronze_medal_time_hours", 10, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeSilverMedalTimeHours = CVarDef.Create<int>("rmc.playtime_silver_medal_time_hours", 25, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeGoldMedalTimeHours = CVarDef.Create<int>("rmc.playtime_gold_medal_time_hours", 70, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPlaytimePlatinumMedalTimeHours = CVarDef.Create<int>("rmc.playtime_platinum_medal_time_hours", 175, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeRubyMedalTimeHours = CVarDef.Create<int>("rmc.playtime_ruby_medal_time_hours", 350, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeAmethystMedalTimeHours = CVarDef.Create<int>("rmc.playtime_amethyst_medal_time_hours", 600, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeEmeraldMedalTimeHours = CVarDef.Create<int>("rmc.playtime_emerald_medal_time_hours", 1000, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPlaytimePrismaticMedalTimeHours = CVarDef.Create<int>("rmc.playtime_prismatic_medal_time_hours", 1500, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeXenoPrefixThreeTimeHours = CVarDef.Create<int>("rmc.playtime_xeno_prefix_three_time_hours", 124, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeXenoPostfixTimeHours = CVarDef.Create<int>("rmc.playtime_xeno_postfix_time_hours", 24, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCPlaytimeXenoPostfixTwoTimeHours = CVarDef.Create<int>("rmc.playtime_xeno_postfix_two_time_hours", 300, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCDisconnectedXenoGhostRoleTimeSeconds = CVarDef.Create<int>("rmc.disconnected_xeno_ghost_role_time_seconds", 300, (CVar)2, (string)null);

	public static readonly CVarDef<float> RMCMarineScalingNormal = CVarDef.Create<float>("rmc.marine_scaling_normal", 50f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCMarineScalingBonus = CVarDef.Create<float>("rmc.marine_scaling_bonus", 0f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCMentorHelpRateLimitPeriod = CVarDef.Create<float>("rmc.mentor_help_rate_limit_period", 2f, (CVar)64, (string)null);

	public static readonly CVarDef<int> RMCMentorHelpRateLimitCount = CVarDef.Create<int>("rmc.mentor_help_rate_limit_count", 10, (CVar)64, (string)null);

	public static readonly CVarDef<string> RMCMentorHelpSound = CVarDef.Create<string>("rmc.mentor_help_sound", "/Audio/_RMC14/Effects/Admin/mhelp.ogg", (CVar)536, (string)null);

	public static readonly CVarDef<string> RMCMentorChatSound = CVarDef.Create<string>("rmc.mentor_chat_sound", "/Audio/Items/pop.ogg", (CVar)536, (string)null);

	public static readonly CVarDef<float> RMCMentorChatVolume = CVarDef.Create<float>("rmc.mentor_help_volume", -5f, (CVar)536, (string)null);

	public static readonly CVarDef<int> RMCJelliesPerQueen = CVarDef.Create<int>("rmc.jellies_per_queen", 5, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCCommendationMaxLength = CVarDef.Create<int>("rmc.commendation_max_length", 1000, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCRoundEndNoEorgPopup = CVarDef.Create<bool>("game.round_end_eorg_popup_enabled", true, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCSkipRoundEndNoEorgPopup = CVarDef.Create<bool>("game.skip_round_end_eorg_popup", false, (CVar)144, (string)null);

	public static readonly CVarDef<float> RMCRoundEndNoEorgPopupTime = CVarDef.Create<float>("game.round_end_eorg_popup_time", 5f, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCXenoEvolveSameCasteCooldownSeconds = CVarDef.Create<int>("rmc.xeno_evolve_same_caste_cooldown_seconds", 300, (CVar)2, (string)null);

	public static readonly CVarDef<bool> GuidebookShowEditorSpeciesButton = CVarDef.Create<bool>("guidebook.show_editor_species_button", false, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCEnableSuicide = CVarDef.Create<bool>("rmc.enable_suicide", false, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCWeedKillerDropshipDelaySeconds = CVarDef.Create<int>("rmc.weed_killer_dropship_delay_seconds", 20, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCWeedKillerDisableDurationMinutes = CVarDef.Create<int>("rmc.weed_killer_disable_duration_minutes", 8, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCIntelPaperScraps = CVarDef.Create<int>("rmc.intel_paper_scraps", 45, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelProgressReports = CVarDef.Create<int>("rmc.intel_progress_reports", 15, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelFolders = CVarDef.Create<int>("rmc.intel_folders", 30, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelTechnicalManuals = CVarDef.Create<int>("rmc.intel_technical_manuals", 10, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelDisks = CVarDef.Create<int>("rmc.intel_disks", 30, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelExperimentalDevices = CVarDef.Create<int>("rmc.intel_experimental_devices", 15, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelResearchPapers = CVarDef.Create<int>("rmc.intel_research_papers", 15, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelVialBoxes = CVarDef.Create<int>("rmc.intel_vial_boxes", 20, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCIntelMaxProcessTimeMilliseconds = CVarDef.Create<float>("rmc.intel_max_process_time_milliseconds", 2f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCIntelAnnounceEveryMinutes = CVarDef.Create<float>("rmc.intel_announce_every_minutes", 15f, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelPowerObjectiveWattsRequired = CVarDef.Create<int>("rmc.intel_power_objective_watts_required", 300000, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCIntelHumanoidCorpsesMax = CVarDef.Create<int>("rmc.intel_humanoid_corpses_max", 48, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCMaxTacmapAlertProcessTimeMilliseconds = CVarDef.Create<float>("rmc.tacmap_alert_max_process_time_milliseconds", 1f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCParasiteSpawnInitialDelayMinutes = CVarDef.Create<float>("rmc.parasite_spawn_initial_delay_minutes", 15f, (CVar)2, (string)null);

	public static readonly CVarDef<float> RMCXenoSpawnInitialMuteDurationSeconds = CVarDef.Create<float>("rmc.xeno_spawn_initial_mute_duration_seconds", 180f, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCXenoEarlyEvoPointBoostBeforeMinutes = CVarDef.Create<int>("rmc.evolution_early_evo_point_boost_minutes", 15, (CVar)2, (string)null);

	public static readonly CVarDef<bool> RMCDamageYourself = CVarDef.Create<bool>("rmc.damage_yourself", false, (CVar)536, (string)null);

	public static readonly CVarDef<float> RMCOverwatchMaxProcessTimeMilliseconds = CVarDef.Create<float>("rmc.overwatch_max_process_time_milliseconds", 1f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCOverwatchConsoleUpdateEverySeconds = CVarDef.Create<float>("rmc.overwatch_console_update_every_seconds", 0.5f, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCResinConstructionDensityCostIncreaseThreshold = CVarDef.Create<float>("rmc.resin_construction_density_cost_increase_threshold", 0.4f, (CVar)2, (string)null);

	public static readonly CVarDef<bool> RMCUseAlternateSprites = CVarDef.Create<bool>("rmc.use_alternate_sprites", false, (CVar)536, (string)null);

	public static readonly CVarDef<int> RMCSunsetDuration = CVarDef.Create<int>("rmc.lighting_sunset_duration", 280, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCSunriseDuration = CVarDef.Create<int>("rmc.lighting_sunrise_duration", 280, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCForceEndHijackTimeMinutes = CVarDef.Create<int>("rmc.force_hijack_end_time_minutes", 25, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCMovementPenCapSubtract = CVarDef.Create<float>("rmc.movement_pen_cap_subtract", 0.8f, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCMovementBigXenosCancelMovement = CVarDef.Create<bool>("rmc.movement_big_xenos_cancel_movement", true, (CVar)2, (string)null);

	public static readonly CVarDef<float> RMCHijackShipWeight = CVarDef.Create<float>("rmc.hijack_ship_weight", 0.5f, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCMinimumHijackBurrowed = CVarDef.Create<int>("rmc.hijack_minimum_burrowed", 5, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCDistressXenosMinimum = CVarDef.Create<int>("rmc.distress_xenos_minimum", 4, (CVar)2, (string)null);

	public static readonly CVarDef<float> VolumeGainCassettes = CVarDef.Create<float>("rmc.volume_gain_cassettes", 0.33f, (CVar)536, (string)null);

	public static readonly CVarDef<float> VolumeGainHijackSong = CVarDef.Create<float>("rmc.volume_gain_hijack_song", 0.5f, (CVar)536, (string)null);

	public static readonly CVarDef<bool> HidePlayerIdentities = CVarDef.Create<bool>("rmc.hide_player_identities", true, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCQueenBuildingBoost = CVarDef.Create<bool>("rmc.queen_building_boost", true, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCQueenBuildingBoostDurationMinutes = CVarDef.Create<int>("rmc.queen_building_boost_duration_minutes", 30, (CVar)2, (string)null);

	public static readonly CVarDef<float> RMCQueenBuildingBoostSpeedMultiplier = CVarDef.Create<float>("rmc.queen_building_boost_speed_multiplier", 5f / 6f, (CVar)2, (string)null);

	public static readonly CVarDef<float> RMCQueenBuildingBoostRemoteRange = CVarDef.Create<float>("rmc.queen_building_boost_remote_range", 50f, (CVar)2, (string)null);

	public static readonly CVarDef<bool> RMCAutomaticCommanderPromotion = CVarDef.Create<bool>("rmc.automatic_commander_promotion", true, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCDeadChatEnabled = CVarDef.Create<bool>("rmc.dead_chat_enabled", true, (CVar)42, (string)null);

	public static readonly CVarDef<bool> RMCDelayRoundEnd = CVarDef.Create<bool>("rmc.delay_round_end", false, (CVar)66, (string)null);

	public static readonly CVarDef<bool> RMCLobbyStartPaused = CVarDef.Create<bool>("rmc.lobby_start_paused", false, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCChatRepeatHistory = CVarDef.Create<int>("rmc.chat_repeat_history", 4, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCChatSquadColorMode = CVarDef.Create<bool>("rmc.chat_squad_color_mode", true, (CVar)144, (string)null);

	public static readonly CVarDef<int> RMCLagCompensationMilliseconds = CVarDef.Create<int>("rmc.lag_compensation_milliseconds", 750, (CVar)10, (string)null);

	public static readonly CVarDef<float> RMCLagCompensationMarginTiles = CVarDef.Create<float>("rmc.lag_compensation_margin_tiles", 0.25f, (CVar)10, (string)null);

	public static readonly CVarDef<bool> RMCGhostCanBoo = CVarDef.Create<bool>("rmc.ghosts_can_boo", true, (CVar)66, (string)null);

	public static readonly CVarDef<int> RMCRoyalResinEveryMinutes = CVarDef.Create<int>("rmc.royal_resin_every_minutes", 5, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCCommunicationTowerXenoTakeoverMinutes = CVarDef.Create<int>("rmc.communication_tower_xeno_takeover_minutes", 55, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCBoonsLiveMarineRequirement = CVarDef.Create<int>("rmc.boons_live_marine_requirement", 12, (CVar)10, (string)null);

	public static readonly CVarDef<int> RMCKingVoteCandidateTimeRequirementHours = CVarDef.Create<int>("rmc.king_vote_candidate_time_requirement", 50, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCKingHatchingFirstWarningMinutes = CVarDef.Create<int>("rmc.king_hatching_first_warning_minutes", 5, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCKingVoteStartTimeSeconds = CVarDef.Create<int>("rmc.king_vote_start_time_seconds", 60, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCKingVoteAskCandidatesTimeSeconds = CVarDef.Create<int>("rmc.king_vote_ask_candidates_time_seconds", 40, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCKingVoteStartHatchingTimeSeconds = CVarDef.Create<int>("rmc.king_vote_start_hatching_time_seconds", 20, (CVar)2, (string)null);

	public static readonly CVarDef<int> RMCNewResinPreventCollideTimeSeconds = CVarDef.Create<int>("rmc.new_resin_prevent_collide_time_seconds", 5, (CVar)2, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesYourself = CVarDef.Create<bool>("rmc.play_emotes_yourself", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesArachnid = CVarDef.Create<bool>("rmc.play_emotes_arachnid", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesDiona = CVarDef.Create<bool>("rmc.play_emotes_diona", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesDwarf = CVarDef.Create<bool>("rmc.play_emotes_dwarf", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesFelinid = CVarDef.Create<bool>("rmc.play_emotes_felinid", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesHuman = CVarDef.Create<bool>("rmc.play_emotes_human", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesMoth = CVarDef.Create<bool>("rmc.play_emotes_moth", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesReptilian = CVarDef.Create<bool>("rmc.play_emotes_reptilian", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesSlime = CVarDef.Create<bool>("rmc.play_emotes_slime", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesAvali = CVarDef.Create<bool>("rmc.play_emotes_avali", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesVulpkanin = CVarDef.Create<bool>("rmc.play_emotes_vulpkanin", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesRodentia = CVarDef.Create<bool>("rmc.play_emotes_rodentia", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesFeroxi = CVarDef.Create<bool>("rmc.play_emotes_feroxi", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayEmotesSkrell = CVarDef.Create<bool>("rmc.play_emotes_skrell", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesYourself = CVarDef.Create<bool>("rmc.play_voicelines_yourself", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesArachnid = CVarDef.Create<bool>("rmc.play_voicelines_arachnid", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesDiona = CVarDef.Create<bool>("rmc.play_voicelines_diona", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesDwarf = CVarDef.Create<bool>("rmc.play_voicelines_dwarf", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesFelinid = CVarDef.Create<bool>("rmc.play_voicelines_felinid", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesHuman = CVarDef.Create<bool>("rmc.play_voicelines_human", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesMoth = CVarDef.Create<bool>("rmc.play_voicelines_moth", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesReptilian = CVarDef.Create<bool>("rmc.play_voicelines_reptilian", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesSlime = CVarDef.Create<bool>("rmc.play_voicelines_slime", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesAvali = CVarDef.Create<bool>("rmc.play_voicelines_avali", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesVulpkanin = CVarDef.Create<bool>("rmc.play_voicelines_vulpkanin", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesRodentia = CVarDef.Create<bool>("rmc.play_voicelines_rodentia", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesFeroxi = CVarDef.Create<bool>("rmc.play_voicelines_feroxi", true, (CVar)536, (string)null);

	public static readonly CVarDef<bool> RMCPlayVoicelinesSkrell = CVarDef.Create<bool>("rmc.play_voicelines_skrell", true, (CVar)536, (string)null);
}
