// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.GameBuildInformation
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;

#nullable enable
namespace Robust.Shared.Utility;

internal sealed record GameBuildInformation(
  string EngineVersion,
  string? ZipHash,
  string? ZipDownload,
  string ForkId,
  string Version,
  string? ManifestHash,
  string? ManifestUrl,
  string? ManifestDownloadUrl)
{
  public static GameBuildInformation GetBuildInfoFromConfig(IConfigurationManager cfg)
  {
    string zipHash = cfg.GetCVar<string>(CVars.BuildHash);
    string manifestHash = cfg.GetCVar<string>(CVars.BuildManifestHash);
    string forkId = cfg.GetCVar<string>(CVars.BuildForkId);
    string forkVersion = cfg.GetCVar<string>(CVars.BuildVersion);
    string ManifestDownloadUrl = Interpolate(cfg.GetCVar<string>(CVars.BuildManifestDownloadUrl));
    string ManifestUrl = Interpolate(cfg.GetCVar<string>(CVars.BuildManifestUrl));
    string ZipDownload = Interpolate(cfg.GetCVar<string>(CVars.BuildDownloadUrl));
    if (ZipDownload == "")
      ZipDownload = (string) null;
    if (zipHash == "")
      zipHash = (string) null;
    if (manifestHash == "")
      manifestHash = (string) null;
    if (ManifestDownloadUrl == "")
      ManifestDownloadUrl = (string) null;
    if (ManifestUrl == "")
      ManifestUrl = (string) null;
    return new GameBuildInformation(cfg.GetCVar<string>(CVars.BuildEngineVersion), zipHash, ZipDownload, forkId, forkVersion, manifestHash, ManifestUrl, ManifestDownloadUrl);

    string? Interpolate(string? value)
    {
      return value?.Replace("{FORK_VERSION}", forkVersion).Replace("{FORK_ID}", forkId).Replace("{MANIFEST_HASH}", manifestHash).Replace("{ZIP_HASH}", zipHash);
    }
  }
}
