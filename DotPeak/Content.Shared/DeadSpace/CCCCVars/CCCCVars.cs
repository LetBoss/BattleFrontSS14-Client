// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeadSpace.CCCCVars.CCCCVars
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Configuration;

#nullable enable
namespace Content.Shared.DeadSpace.CCCCVars;

[CVarDefs]
public sealed class CCCCVars
{
  public static readonly CVarDef<bool> GCFEnabled = CVarDef.Create<bool>("gcf_auto.enabled", false, (CVar) 0, (string) null);
  public static readonly CVarDef<bool> GCFNotify = CVarDef.Create<bool>("gcf_auto.notify", false, (CVar) 0, (string) null);
  public static readonly CVarDef<float> GCFFrequency = CVarDef.Create<float>("gcf_auto.frequency", 300f, (CVar) 0, (string) null);
  public static readonly CVarDef<string> InfoLinksIPs = CVarDef.Create<string>("infolinks.ips", string.Empty, (CVar) 10, (string) null);
  public static readonly CVarDef<float> PlayTimeMultiplier = CVarDef.Create<float>("playtime.multiplier", 1f, (CVar) 10, (string) null);
  public static readonly CVarDef<float> TTSVolumeRadio = CVarDef.Create<float>("tts.volume_radio", 1f, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<bool> RadioTTSSoundsEnabled = CVarDef.Create<bool>("audio.radio_tts_sounds_enabled", true, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<float> JukeboxMusicVolume = CVarDef.Create<float>("jukebox.volume", 1f, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<bool> TaipanEnabled = CVarDef.Create<bool>("taipan.enabled", false, (CVar) 64 /*0x40*/, (string) null);
  public static readonly CVarDef<string> Background = CVarDef.Create<string>("ui.background", "Image", (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<bool> GameModesUseTotalPlayers = CVarDef.Create<bool>("game.modes_use_total_players", true, (CVar) 80 /*0x50*/, (string) null);
  public static readonly CVarDef<string> SysNotifyCvar = CVarDef.Create<string>("sysnotify.Dict", "", (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<int> SysNotifyCoolDown = CVarDef.Create<int>("sysnotify.cooldown", 1, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<bool> SysNotifyPerm = CVarDef.Create<bool>("sysnotify.permission", true, (CVar) 144 /*0x90*/, (string) null);
  public static readonly CVarDef<string> SysNotifySoundPath = CVarDef.Create<string>("sysnotifys.soundpath", "/Audio/Effects/balloon-pop.ogg", (CVar) 144 /*0x90*/, (string) null);
}
