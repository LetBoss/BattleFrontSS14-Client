// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.WritableDirProvider
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#nullable enable
namespace Robust.Shared.ContentPack;

internal sealed class WritableDirProvider : IWritableDirProvider
{
  private readonly bool _hideRootDir;

  public string RootDir { get; }

  string? IWritableDirProvider.RootDir => !this._hideRootDir ? this.RootDir : (string) null;

  public WritableDirProvider(DirectoryInfo rootDir, bool hideRootDir)
  {
    ReadOnlySpan<char> fullName = (ReadOnlySpan<char>) rootDir.FullName;
    char directorySeparatorChar = Path.DirectorySeparatorChar;
    ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(ref directorySeparatorChar);
    this.RootDir = fullName.ToString() + readOnlySpan;
    this._hideRootDir = hideRootDir;
  }

  public void CreateDir(ResPath path) => Directory.CreateDirectory(this.GetFullPath(path));

  public void Delete(ResPath path)
  {
    string fullPath = this.GetFullPath(path);
    if (Directory.Exists(fullPath))
    {
      Directory.Delete(fullPath, true);
    }
    else
    {
      if (!File.Exists(fullPath))
        return;
      File.Delete(fullPath);
    }
  }

  public bool Exists(ResPath path)
  {
    string fullPath = this.GetFullPath(path);
    return Directory.Exists(fullPath) || File.Exists(fullPath);
  }

  public (IEnumerable<ResPath> files, IEnumerable<ResPath> directories) Find(
    string pattern,
    bool recursive = true)
  {
    if (pattern.Contains(".."))
      throw new InvalidOperationException($"Pattern may not contain '..'. Pattern: {pattern}.");
    int startIndex = this.RootDir.Length - 1;
    SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    string[] files = Directory.GetFiles(this.RootDir, pattern, searchOption);
    string[] directories = Directory.GetDirectories(this.RootDir, pattern, searchOption);
    List<ResPath> resPathList1 = new List<ResPath>(files.Length);
    List<ResPath> resPathList2 = new List<ResPath>(directories.Length);
    foreach (string str in files)
    {
      if (!str.Contains("\\..") && !str.Contains("/.."))
        resPathList1.Add(ResPath.FromRelativeSystemPath(str.Substring(startIndex)).ToRootedPath());
    }
    foreach (string str in directories)
    {
      if (!str.Contains("\\..") && !str.Contains("/.."))
        resPathList2.Add(ResPath.FromRelativeSystemPath(str.Substring(startIndex)).ToRootedPath());
    }
    return ((IEnumerable<ResPath>) resPathList1, (IEnumerable<ResPath>) resPathList2);
  }

  public IEnumerable<string> DirectoryEntries(ResPath path)
  {
    string fullPath = this.GetFullPath(path);
    if (Directory.Exists(fullPath))
    {
      foreach (string enumerateFileSystemEntry in Directory.EnumerateFileSystemEntries(fullPath))
        yield return Path.GetRelativePath(fullPath, enumerateFileSystemEntry);
    }
  }

  public bool IsDir(ResPath path) => Directory.Exists(this.GetFullPath(path));

  public Stream Open(ResPath path, FileMode fileMode, FileAccess access, FileShare share)
  {
    return (Stream) File.Open(this.GetFullPath(path), fileMode, access, share);
  }

  public IWritableDirProvider OpenSubdirectory(ResPath path)
  {
    return this.IsDir(path) ? (IWritableDirProvider) new WritableDirProvider(new DirectoryInfo(this.GetFullPath(path)), this._hideRootDir) : throw new FileNotFoundException();
  }

  public void Rename(ResPath oldPath, ResPath newPath)
  {
    File.Move(this.GetFullPath(oldPath), this.GetFullPath(newPath));
  }

  public void OpenOsWindow(ResPath path)
  {
    if (!this.IsDir(path))
      path = path.Directory;
    string fullPath = this.GetFullPath(path);
    if (OperatingSystem.IsWindows())
      Process.Start(new ProcessStartInfo()
      {
        FileName = Environment.GetEnvironmentVariable("SystemRoot") + "\\explorer.exe",
        Arguments = ".",
        WorkingDirectory = fullPath
      });
    else if (OperatingSystem.IsMacOS())
    {
      Process.Start(new ProcessStartInfo()
      {
        FileName = "open",
        Arguments = ".",
        WorkingDirectory = fullPath
      });
    }
    else
    {
      if (!OperatingSystem.IsLinux() && !OperatingSystem.IsFreeBSD())
        throw new NotSupportedException("Opening OS windows not supported on this OS");
      Process.Start(new ProcessStartInfo()
      {
        FileName = "xdg-open",
        Arguments = ".",
        WorkingDirectory = fullPath
      });
    }
  }

  public string GetFullPath(ResPath path)
  {
    path = path.IsRooted ? path.Clean() : throw new ArgumentException($"Path must be rooted. Path: {path}");
    return PathHelpers.SafeGetResourcePath(this.RootDir, path);
  }
}
