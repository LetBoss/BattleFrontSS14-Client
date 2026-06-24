// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.IResourceManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.ContentPack;

[NotContentImplementable]
public interface IResourceManager
{
  IWritableDirProvider UserData { get; }

  void AddRoot(ResPath prefix, IContentRoot loader);

  Stream ContentFileRead(ResPath path);

  Stream ContentFileRead(string path);

  bool ContentFileExists(ResPath path);

  bool ContentFileExists(string path);

  bool TryContentFileRead(ResPath? path, [NotNullWhen(true)] out Stream? fileStream);

  bool TryContentFileRead(string path, [NotNullWhen(true)] out Stream? fileStream);

  IEnumerable<ResPath> ContentFindFiles(ResPath? path);

  IEnumerable<ResPath> ContentFindRelativeFiles(ResPath path)
  {
    foreach (ResPath file in this.ContentFindFiles(new ResPath?(path)))
    {
      ResPath? relative;
      if (!file.TryRelativeTo(path, out relative))
        throw new InvalidOperationException("This is unreachable");
      yield return relative.Value;
    }
  }

  IEnumerable<ResPath> ContentFindFiles(string path);

  IEnumerable<string> ContentGetDirectoryEntries(ResPath path);

  [Obsolete("This API is no longer content-accessible")]
  IEnumerable<ResPath> GetContentRoots();

  string ContentFileReadAllText(string path) => this.ContentFileReadAllText(new ResPath(path));

  string ContentFileReadAllText(ResPath path)
  {
    return this.ContentFileReadAllText(path, EncodingHelpers.UTF8);
  }

  string ContentFileReadAllText(ResPath path, Encoding encoding)
  {
    using (Stream stream = this.ContentFileRead(path))
    {
      using (StreamReader streamReader = new StreamReader(stream, encoding))
        return streamReader.ReadToEnd();
    }
  }

  YamlStream ContentFileReadYaml(ResPath path)
  {
    using (StreamReader streamReader = this.ContentFileReadText(path))
    {
      YamlStream yamlStream = new YamlStream();
      yamlStream.Load((TextReader) streamReader);
      return yamlStream;
    }
  }

  StreamReader ContentFileReadText(ResPath path)
  {
    return this.ContentFileReadText(path, EncodingHelpers.UTF8);
  }

  StreamReader ContentFileReadText(ResPath path, Encoding encoding)
  {
    return new StreamReader(this.ContentFileRead(path), encoding);
  }
}
