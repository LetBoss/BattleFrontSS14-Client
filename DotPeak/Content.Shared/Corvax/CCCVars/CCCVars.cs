// Decompiled with JetBrains decompiler
// Type: Content.Shared.Corvax.CCCVars.CCCVars
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Configuration;

#nullable enable
namespace Content.Shared.Corvax.CCCVars;

[CVarDefs]
public sealed class CCCVars
{
  public static readonly CVarDef<bool> QueueEnabled = CVarDef.Create<bool>("queue.enabled", false, (CVar) 64 /*0x40*/, (string) null);
  public static readonly CVarDef<bool> TTSEnabled = CVarDef.Create<bool>("tts.enabled", false, (CVar) 26, (string) null);
  public static readonly CVarDef<string> TTSApiUrl = CVarDef.Create<string>("tts.api_url", "https://ntts.fdev.team/api/v1/tts", (CVar) 80 /*0x50*/, (string) null);
  public static readonly CVarDef<string> TTSRadioEffect = CVarDef.Create<string>("tts.radio_effect", "radio_v2", (CVar) 80 /*0x50*/, (string) null);
  public static readonly CVarDef<string> TTSCommanderEffect = CVarDef.Create<string>("tts.commander_effect", "megaphone", (CVar) 80 /*0x50*/, (string) null);
  public static readonly CVarDef<string> TTSApiToken = CVarDef.Create<string>("tts.api_token", "", (CVar) 320, (string) null);
  public static readonly CVarDef<int> TTSApiTimeout = CVarDef.Create<int>("tts.api_timeout", 5, (CVar) 80 /*0x50*/, (string) null);
  public static readonly CVarDef<float> TTSVolume = CVarDef.Create<float>("tts.volume", 3f, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<float> TTSVoiceRange = CVarDef.Create<float>("tts.voice_range", 10f, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<float> TTSWhisperRange = CVarDef.Create<float>("tts.whisper_range", 5f, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<int> TTSMaxConcurrent = CVarDef.Create<int>("tts.max_concurrent", 5, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<int> TTSMaxCache = CVarDef.Create<int>("tts.max_cache", 500, (CVar) 80 /*0x50*/, (string) null);
  public static readonly CVarDef<float> TTSRateLimitPeriod = CVarDef.Create<float>("tts.rate_limit_period", 1f, (CVar) 64 /*0x40*/, (string) null);
  public static readonly CVarDef<int> TTSRateLimitCount = CVarDef.Create<int>("tts.rate_limit_count", 8, (CVar) 64 /*0x40*/, (string) null);
  public static readonly CVarDef<int> TTSLongMinLength = CVarDef.Create<int>("tts.long_min_length", 20, (CVar) 80 /*0x50*/, (string) null);
  public static readonly CVarDef<float> TTSLongRatePeriod = CVarDef.Create<float>("tts.long_rate_period", 5f, (CVar) 64 /*0x40*/, (string) null);
  public static readonly CVarDef<int> TTSLongRateCount = CVarDef.Create<int>("tts.long_rate_count", 2, (CVar) 64 /*0x40*/, (string) null);
  public static readonly CVarDef<bool> DiscordAuthEnabled = CVarDef.Create<bool>("discord_auth.enabled", false, (CVar) 10, (string) null);
  public static readonly CVarDef<string> DiscordAuthApiUrl = CVarDef.Create<string>("discord_auth.api_url", "", (CVar) 64 /*0x40*/, (string) null);
  public static readonly CVarDef<string> DiscordAuthApiKey = CVarDef.Create<string>("discord_auth.api_key", "", (CVar) 320, (string) null);
  public static readonly CVarDef<bool> DiscordAuthIsOptional = CVarDef.Create<bool>("discord_auth.is_opt", false, (CVar) 10, (string) null);
}
