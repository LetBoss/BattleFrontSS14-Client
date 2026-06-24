// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.IContentRoot
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

#nullable enable
namespace Robust.Shared.ContentPack;

public interface IContentRoot
{
  void Mount();

  bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream);

  bool FileExists(ResPath relPath);

  IEnumerable<ResPath> FindFiles(ResPath path);

  IEnumerable<string> GetRelativeFilePaths();

  IEnumerable<string> GetEntries(ResPath path)
  {
    int countDirs = path == ResPath.Self ? 0 : ((IEnumerable<string>) path.CanonPath.Split('/')).Count<string>();
    return this.FindFiles(path).Select<ResPath, string>((Func<ResPath, string>) (c =>
    {
      string[] source = c.CanonPath.Split('/');
      int num = ((IEnumerable<string>) source).Count<string>();
      string entries = ((IEnumerable<string>) source).Skip<string>(countDirs).First<string>();
      if (num > countDirs + 1)
        entries += "/";
      return entries;
    })).Distinct<string>();
  }
}
