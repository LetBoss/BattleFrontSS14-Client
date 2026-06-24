// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.ResourceManifestData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.IO;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.ContentPack;

internal sealed record ResourceManifestData(
  string[] Modules,
  string? AssemblyPrefix,
  string? DefaultWindowTitle,
  string? WindowIconSet,
  string? SplashLogo,
  bool? ShowLoadingBar,
  bool AutoConnect,
  string[]? ClientAssemblies)
{
  public static readonly ResourceManifestData Default = new ResourceManifestData(Array.Empty<string>(), (string) null, (string) null, (string) null, (string) null, new bool?(), true, (string[]) null);

  public static ResourceManifestData LoadResourceManifest(IResourceManager res)
  {
    Stream fileStream;
    if (!res.TryContentFileRead("/manifest.yml", out fileStream))
      return ResourceManifestData.Default;
    YamlStream yamlStream = new YamlStream();
    using (fileStream)
    {
      using (StreamReader streamReader = new StreamReader(fileStream, EncodingHelpers.UTF8))
        yamlStream.Load((TextReader) streamReader);
    }
    if (yamlStream.Documents.Count == 0)
      return ResourceManifestData.Default;
    if (yamlStream.Documents.Count != 1 || !(yamlStream.Documents[0].RootNode is YamlMappingNode rootNode))
      throw new InvalidOperationException("Expected a single YAML document with root mapping for /manifest.yml");
    string[] Modules = ReadStringArray(rootNode, "modules") ?? Array.Empty<string>();
    string AssemblyPrefix = (string) null;
    YamlNode returnNode1;
    if (rootNode.TryGetNode("assemblyPrefix", out returnNode1))
      AssemblyPrefix = returnNode1.AsString();
    string DefaultWindowTitle = (string) null;
    YamlNode returnNode2;
    if (rootNode.TryGetNode("defaultWindowTitle", out returnNode2))
      DefaultWindowTitle = returnNode2.AsString();
    string WindowIconSet = (string) null;
    YamlNode returnNode3;
    if (rootNode.TryGetNode("windowIconSet", out returnNode3))
      WindowIconSet = returnNode3.AsString();
    string SplashLogo = (string) null;
    YamlNode returnNode4;
    if (rootNode.TryGetNode("splashLogo", out returnNode4))
      SplashLogo = returnNode4.AsString();
    bool? ShowLoadingBar = new bool?();
    YamlNode returnNode5;
    if (rootNode.TryGetNode("show_loading_bar", out returnNode5))
      ShowLoadingBar = new bool?(returnNode5.AsBool());
    bool AutoConnect = true;
    YamlNode returnNode6;
    if (rootNode.TryGetNode("autoConnect", out returnNode6))
      AutoConnect = returnNode6.AsBool();
    string[] ClientAssemblies = ReadStringArray(rootNode, "clientAssemblies");
    return new ResourceManifestData(Modules, AssemblyPrefix, DefaultWindowTitle, WindowIconSet, SplashLogo, ShowLoadingBar, AutoConnect, ClientAssemblies);

    static string[]? ReadStringArray(YamlMappingNode mapping, string key)
    {
      YamlNode returnNode;
      if (!mapping.TryGetNode(key, out returnNode))
        return (string[]) null;
      YamlSequenceNode yamlSequenceNode = (YamlSequenceNode) returnNode;
      string[] strArray = new string[yamlSequenceNode.Children.Count];
      for (int index = 0; index < strArray.Length; ++index)
        strArray[index] = ((YamlNode) yamlSequenceNode)[index].AsString();
      return strArray;
    }
  }
}
