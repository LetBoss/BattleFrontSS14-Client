using Robust.Shared.Configuration;

namespace Robust.Shared.Utility;

internal sealed record GameBuildInformation(string EngineVersion, string? ZipHash, string? ZipDownload, string ForkId, string Version, string? ManifestHash, string? ManifestUrl, string? ManifestDownloadUrl)
{
	public static GameBuildInformation GetBuildInfoFromConfig(IConfigurationManager cfg)
	{
		string zipHash = cfg.GetCVar(CVars.BuildHash);
		string manifestHash = cfg.GetCVar(CVars.BuildManifestHash);
		string forkId = cfg.GetCVar(CVars.BuildForkId);
		string forkVersion = cfg.GetCVar(CVars.BuildVersion);
		string text = Interpolate(cfg.GetCVar(CVars.BuildManifestDownloadUrl));
		string text2 = Interpolate(cfg.GetCVar(CVars.BuildManifestUrl));
		string text3 = Interpolate(cfg.GetCVar(CVars.BuildDownloadUrl));
		if (text3 == "")
		{
			text3 = null;
		}
		if (zipHash == "")
		{
			zipHash = null;
		}
		if (manifestHash == "")
		{
			manifestHash = null;
		}
		if (text == "")
		{
			text = null;
		}
		if (text2 == "")
		{
			text2 = null;
		}
		return new GameBuildInformation(cfg.GetCVar(CVars.BuildEngineVersion), zipHash, text3, forkId, forkVersion, manifestHash, text2, text);
		string? Interpolate(string? value)
		{
			return value?.Replace("{FORK_VERSION}", forkVersion).Replace("{FORK_ID}", forkId).Replace("{MANIFEST_HASH}", manifestHash)
				.Replace("{ZIP_HASH}", zipHash);
		}
	}
}
