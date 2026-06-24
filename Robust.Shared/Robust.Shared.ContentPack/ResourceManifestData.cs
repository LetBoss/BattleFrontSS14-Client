using System;
using System.IO;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.ContentPack;

internal sealed record ResourceManifestData(string[] Modules, string? AssemblyPrefix, string? DefaultWindowTitle, string? WindowIconSet, string? SplashLogo, bool? ShowLoadingBar, bool AutoConnect, string[]? ClientAssemblies)
{
	public static readonly ResourceManifestData Default = new ResourceManifestData(Array.Empty<string>(), null, null, null, null, null, AutoConnect: true, null);

	public static ResourceManifestData LoadResourceManifest(IResourceManager res)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		if (!res.TryContentFileRead("/manifest.yml", out Stream fileStream))
		{
			return Default;
		}
		YamlStream val = new YamlStream();
		using (fileStream)
		{
			using StreamReader streamReader = new StreamReader(fileStream, EncodingHelpers.UTF8);
			val.Load((TextReader)streamReader);
		}
		if (val.Documents.Count == 0)
		{
			return Default;
		}
		if (val.Documents.Count == 1)
		{
			YamlNode rootNode = val.Documents[0].RootNode;
			YamlMappingNode val2 = (YamlMappingNode)(object)((rootNode is YamlMappingNode) ? rootNode : null);
			if (val2 != null)
			{
				string[] modules = ReadStringArray(val2, "modules") ?? Array.Empty<string>();
				string assemblyPrefix = null;
				if (val2.TryGetNode("assemblyPrefix", out YamlNode returnNode))
				{
					assemblyPrefix = returnNode.AsString();
				}
				string defaultWindowTitle = null;
				if (val2.TryGetNode("defaultWindowTitle", out YamlNode returnNode2))
				{
					defaultWindowTitle = returnNode2.AsString();
				}
				string windowIconSet = null;
				if (val2.TryGetNode("windowIconSet", out YamlNode returnNode3))
				{
					windowIconSet = returnNode3.AsString();
				}
				string splashLogo = null;
				if (val2.TryGetNode("splashLogo", out YamlNode returnNode4))
				{
					splashLogo = returnNode4.AsString();
				}
				bool? showLoadingBar = null;
				if (val2.TryGetNode("show_loading_bar", out YamlNode returnNode5))
				{
					showLoadingBar = returnNode5.AsBool();
				}
				bool autoConnect = true;
				if (val2.TryGetNode("autoConnect", out YamlNode returnNode6))
				{
					autoConnect = returnNode6.AsBool();
				}
				string[] clientAssemblies = ReadStringArray(val2, "clientAssemblies");
				return new ResourceManifestData(modules, assemblyPrefix, defaultWindowTitle, windowIconSet, splashLogo, showLoadingBar, autoConnect, clientAssemblies);
			}
		}
		throw new InvalidOperationException("Expected a single YAML document with root mapping for /manifest.yml");
		static string[]? ReadStringArray(YamlMappingNode mapping, string key)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			if (!mapping.TryGetNode(key, out YamlNode returnNode7))
			{
				return null;
			}
			YamlSequenceNode val3 = (YamlSequenceNode)returnNode7;
			string[] array = new string[val3.Children.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((YamlNode)val3)[i].AsString();
			}
			return array;
		}
	}
}
