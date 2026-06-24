// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.ResourceManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.ContentPack;

[Virtual]
internal class ResourceManager : IResourceManagerInternal, IResourceManager
{
  [Dependency]
  private readonly IConfigurationManager _config;
  [Dependency]
  private readonly ILogManager _logManager;
  private (ResPath prefix, IContentRoot root)[] _contentRoots = new (ResPath, IContentRoot)[0];
  private StreamSeekMode _streamSeekMode;
  private readonly object _rootMutateLock = new object();
  private static readonly Regex BadPathSegmentRegex = new Regex("^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])$", RegexOptions.IgnoreCase);
  private static readonly Regex BadPathCharacterRegex = new Regex("[<>:\"|?*\0\\x01-\\x1f]", RegexOptions.IgnoreCase);
  protected ISawmill Sawmill;

  public IWritableDirProvider UserData { get; private set; }

  public virtual void Initialize(string? userData, bool hideRootDir)
  {
    this.Sawmill = this._logManager.GetSawmill("res");
    this.UserData = userData == null ? (IWritableDirProvider) new VirtualWritableDirProvider() : (IWritableDirProvider) new WritableDirProvider(Directory.CreateDirectory(userData), hideRootDir);
    this._config.OnValueChanged<int>(CVars.ResStreamSeekMode, (Action<int>) (i => this._streamSeekMode = (StreamSeekMode) i), true);
  }

  public void MountDefaultContentPack()
  {
    string cvar = this._config.GetCVar<string>("resource.pack");
    if (string.IsNullOrWhiteSpace(cvar))
      this.Sawmill.Warning("No default ContentPack to load in configuration.");
    else
      this.MountContentPack(cvar, new ResPath?());
  }

  public void MountContentPack(string pack, ResPath? prefix = null)
  {
    prefix = new ResPath?(ResourceManager.SanitizePrefix(prefix));
    if (!Path.IsPathRooted(pack))
      pack = PathHelpers.ExecutableRelativeFile(pack);
    FileInfo pack1 = new FileInfo(pack);
    ResourceManager.PackLoader loader = pack1.Exists ? new ResourceManager.PackLoader(pack1, this.Sawmill) : throw new FileNotFoundException("Specified ContentPack does not exist: " + pack1.FullName);
    this.AddRoot(prefix.Value, (IContentRoot) loader);
  }

  public void MountContentPack(Stream zipStream, ResPath? prefix = null)
  {
    prefix = new ResPath?(ResourceManager.SanitizePrefix(prefix));
    ResourceManager.PackLoader loader = new ResourceManager.PackLoader(zipStream, this.Sawmill);
    this.AddRoot(prefix.Value, (IContentRoot) loader);
  }

  public void AddRoot(ResPath prefix, IContentRoot loader)
  {
    lock (this._rootMutateLock)
    {
      loader.Mount();
      (ResPath, IContentRoot)[] contentRoots = this._contentRoots;
      Array.Resize<(ResPath, IContentRoot)>(ref contentRoots, contentRoots.Length + 1);
      (ResPath, IContentRoot)[] valueTupleArray = contentRoots;
      valueTupleArray[valueTupleArray.Length - 1] = (prefix, loader);
      this._contentRoots = contentRoots;
    }
  }

  private static ResPath SanitizePrefix(ResPath? prefix)
  {
    if (!prefix.HasValue)
      prefix = new ResPath?(ResPath.Root);
    else if (!prefix.Value.IsRooted)
      throw new ArgumentException("Prefix must be rooted.", nameof (prefix));
    return prefix.Value;
  }

  public void MountContentDirectory(string path, ResPath? prefix = null)
  {
    prefix = new ResPath?(ResourceManager.SanitizePrefix(prefix));
    if (!Path.IsPathRooted(path))
      path = PathHelpers.ExecutableRelativeFile(path);
    DirectoryInfo directory = new DirectoryInfo(path);
    if (!directory.Exists)
      throw new DirectoryNotFoundException("Specified directory does not exist: " + directory.FullName);
    ResourceManager.DirLoader loader = new ResourceManager.DirLoader(directory, this._logManager.GetSawmill("res"), this._config.GetCVar<bool>(CVars.ResCheckPathCasing));
    this.AddRoot(prefix.Value, (IContentRoot) loader);
  }

  public Stream ContentFileRead(string path) => this.ContentFileRead(new ResPath(path));

  public Stream ContentFileRead(ResPath path)
  {
    Stream fileStream;
    if (this.TryContentFileRead(new ResPath?(path), out fileStream))
      return fileStream;
    throw new FileNotFoundException($"Path does not exist in the VFS: '{path}'");
  }

  public bool TryContentFileRead(string path, [NotNullWhen(true)] out Stream? fileStream)
  {
    return this.TryContentFileRead(new ResPath?(new ResPath(path)), out fileStream);
  }

  public bool TryContentFileRead(ResPath? path, [NotNullWhen(true)] out Stream? fileStream)
  {
    if (!path.HasValue)
      throw new ArgumentNullException(nameof (path));
    if (!path.Value.IsRooted)
      throw new ArgumentException($"Path '{path}' must be rooted", nameof (path));
    if (path.Value.CanonPath.EndsWith('/'))
    {
      fileStream = (Stream) null;
      return false;
    }
    foreach ((ResPath resPath, IContentRoot root) in this._contentRoots)
    {
      ResPath? relative;
      Stream stream;
      if (path.Value.TryRelativeTo(resPath, out relative) && root.TryGetFile(relative.Value, out stream))
      {
        fileStream = this.WrapStream(stream);
        return true;
      }
    }
    fileStream = (Stream) null;
    return false;
  }

  private Stream WrapStream(Stream stream)
  {
    switch (this._streamSeekMode)
    {
      case StreamSeekMode.None:
        return stream;
      case StreamSeekMode.ForceSeekable:
        if (stream.CanSeek)
          return stream;
        MemoryStream memoryStream = new MemoryStream(stream.CopyToArray(), false);
        stream.Dispose();
        return (Stream) memoryStream;
      case StreamSeekMode.ForceNonSeekable:
        return !stream.CanSeek ? stream : (Stream) new NonSeekableStream(stream);
      default:
        throw new InvalidOperationException();
    }
  }

  public bool ContentFileExists(string path) => this.ContentFileExists(new ResPath(path));

  public bool ContentFileExists(ResPath path)
  {
    Stream fileStream;
    if (!this.TryContentFileRead(new ResPath?(path), out fileStream))
      return false;
    fileStream.Dispose();
    return true;
  }

  public IEnumerable<ResPath> ContentFindFiles(string path)
  {
    return this.ContentFindFiles(new ResPath?(new ResPath(path)));
  }

  public IEnumerable<string> ContentGetDirectoryEntries(ResPath path)
  {
    if (!path.IsRooted)
      throw new ArgumentException("Path is not rooted", nameof (path));
    if (!path.CanonPath.EndsWith("/"))
      path = new ResPath(path.CanonPath + "/");
    HashSet<string> directoryEntries = new HashSet<string>();
    foreach ((ResPath resPath, IContentRoot root) in this._contentRoots)
    {
      ResPath? relative;
      if (path.TryRelativeTo(resPath, out relative))
        directoryEntries.UnionWith(root.GetEntries(relative.Value));
    }
    foreach ((ResPath prefix, IContentRoot root) contentRoot in this._contentRoots)
    {
      ResPath? relative;
      if (contentRoot.prefix.TryRelativeTo(path, out relative))
      {
        string[] strArray = relative.Value.EnumerateSegments();
        if (strArray == null || strArray.Length != 1 || !(strArray[0] == "."))
          directoryEntries.Add(strArray[0] + "/");
      }
    }
    return (IEnumerable<string>) directoryEntries;
  }

  public IEnumerable<ResPath> ContentFindFiles(ResPath? path)
  {
    if (!path.HasValue)
      throw new ArgumentNullException(nameof (path));
    if (!path.Value.IsRooted)
      throw new ArgumentException("Path is not rooted", nameof (path));
    HashSet<ResPath> alreadyReturnedFiles = new HashSet<ResPath>();
    (ResPath, IContentRoot)[] valueTupleArray = this._contentRoots;
    for (int index = 0; index < valueTupleArray.Length; ++index)
    {
      (ResPath basePath, IContentRoot contentRoot) = valueTupleArray[index];
      ResPath? relative;
      if (path.Value.TryRelativeTo(basePath, out relative))
      {
        foreach (ResPath file1 in contentRoot.FindFiles(relative.Value))
        {
          ResPath file2 = basePath / file1;
          if (!alreadyReturnedFiles.Contains(file2))
          {
            alreadyReturnedFiles.Add(file2);
            yield return file2;
          }
        }
        basePath = new ResPath();
      }
    }
    valueTupleArray = ((ResPath, IContentRoot)[]) null;
  }

  public bool TryGetDiskFilePath(ResPath path, [NotNullWhen(true)] out string? diskPath)
  {
    foreach ((ResPath prefix, IContentRoot root) contentRoot in this._contentRoots)
    {
      ResPath prefix = contentRoot.prefix;
      ResPath? relative;
      if (contentRoot.root is ResourceManager.DirLoader root && path.TryRelativeTo(prefix, out relative))
      {
        diskPath = root.GetPath(relative.Value);
        if (File.Exists(diskPath))
          return true;
      }
    }
    diskPath = (string) null;
    return false;
  }

  public void MountStreamAt(MemoryStream stream, ResPath path)
  {
    ResourceManager.SingleStreamLoader loader = new ResourceManager.SingleStreamLoader(stream, path.ToRelativePath());
    this.AddRoot(ResPath.Root, (IContentRoot) loader);
  }

  IEnumerable<ResPath> IResourceManager.GetContentRoots()
  {
    return (IEnumerable<ResPath>) Array.Empty<ResPath>();
  }

  public IEnumerable<string> GetContentRoots()
  {
    (ResPath, IContentRoot)[] valueTupleArray = this._contentRoots;
    for (int index = 0; index < valueTupleArray.Length; ++index)
    {
      if (valueTupleArray[index].Item2 is ResourceManager.DirLoader dirLoader)
        yield return dirLoader.GetPath(ResPath.Root);
    }
    valueTupleArray = ((ResPath, IContentRoot)[]) null;
  }

  internal static bool IsPathValid(ResPath path)
  {
    string input1 = path.ToString();
    if (ResourceManager.BadPathCharacterRegex.IsMatch(input1))
      return false;
    foreach (string input2 in path.CanonPath.Split('/'))
    {
      if (ResourceManager.BadPathSegmentRegex.IsMatch(input2))
        return false;
    }
    return true;
  }

  private sealed class DirLoader : IContentRoot
  {
    private readonly DirectoryInfo _directory;
    private readonly ISawmill _sawmill;
    private readonly bool _checkCasing;

    public DirLoader(DirectoryInfo directory, ISawmill sawmill, bool checkCasing)
    {
      this._directory = directory;
      this._sawmill = sawmill;
      this._checkCasing = checkCasing;
    }

    public void Mount()
    {
    }

    public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
    {
      FileStream stream1;
      int num = FileHelper.TryOpenFileRead(this.GetPath(relPath), out stream1) ? 1 : 0;
      stream = (Stream) stream1;
      return num != 0;
    }

    public bool FileExists(ResPath relPath) => File.Exists(this.GetPath(relPath));

    internal string GetPath(ResPath relPath)
    {
      return PathHelpers.SafeGetResourcePath(this._directory.FullName, relPath);
    }

    public IEnumerable<ResPath> FindFiles(ResPath path)
    {
      string path1 = this.GetPath(path);
      if (Directory.Exists(path1))
      {
        foreach (string file in PathHelpers.GetFiles(path1))
          yield return ResPath.FromRelativeSystemPath(file.Substring(this._directory.FullName.Length));
      }
    }

    public IEnumerable<string> GetEntries(ResPath path)
    {
      string fullPath = this.GetPath(path);
      return !Directory.Exists(fullPath) ? Enumerable.Empty<string>() : Directory.EnumerateFileSystemEntries(fullPath).Select<string, string>((Func<string, string>) (c =>
      {
        string relativePath = Path.GetRelativePath(fullPath, c);
        return Directory.Exists(c) ? relativePath + "/" : relativePath;
      }));
    }

    [Conditional("DEBUG")]
    private void CheckPathCasing(ResPath path)
    {
      if (!this._checkCasing)
        return;
      Task.Run((Action) (() =>
      {
        string str = this.GetPath(ResPath.Root);
        ResPath root = ResPath.Root;
        bool flag1 = false;
        foreach (string b in path.CanonPath.Split('/'))
        {
          DirectoryInfo directoryInfo = new DirectoryInfo(str);
          bool flag2 = false;
          foreach (FileSystemInfo enumerateFileSystemInfo in directoryInfo.EnumerateFileSystemInfos())
          {
            if (string.Equals(enumerateFileSystemInfo.Name, b, StringComparison.InvariantCultureIgnoreCase))
            {
              if (!string.Equals(enumerateFileSystemInfo.Name, b, StringComparison.InvariantCulture))
                flag1 = true;
              root /= enumerateFileSystemInfo.Name;
              str = Path.Combine(str, enumerateFileSystemInfo.Name);
              flag2 = true;
              break;
            }
          }
          if (!flag2)
            return;
        }
        if (!flag1)
          return;
        this._sawmill.Warning("Path '{0}' has mismatching case from file on disk ('{1}'). This can cause loading failures on certain file system configurations and should be corrected.", (object) path, (object) root);
      }));
    }

    public IEnumerable<string> GetRelativeFilePaths() => this.GetRelativeFilePaths(this._directory);

    private IEnumerable<string> GetRelativeFilePaths(DirectoryInfo dir)
    {
      IEnumerator<FileInfo> enumerator1 = dir.EnumerateFiles().GetEnumerator();
      while (enumerator1.MoveNext())
      {
        FileInfo current = enumerator1.Current;
        if ((current.Attributes & FileAttributes.Hidden) == FileAttributes.None && current.Name[0] != '.')
        {
          ResPath resPath = ResPath.FromRelativeSystemPath(current.FullName.Substring(this._directory.FullName.Length));
          resPath = resPath.ToRootedPath();
          yield return resPath.ToString();
        }
      }
      enumerator1 = (IEnumerator<FileInfo>) null;
      foreach (DirectoryInfo enumerateDirectory in dir.EnumerateDirectories())
      {
        if ((enumerateDirectory.Attributes & FileAttributes.Hidden) == FileAttributes.None && enumerateDirectory.Name[0] != '.')
        {
          IEnumerator<string> enumerator2 = this.GetRelativeFilePaths(enumerateDirectory).GetEnumerator();
          while (enumerator2.MoveNext())
            yield return enumerator2.Current;
          enumerator2 = (IEnumerator<string>) null;
        }
      }
    }
  }

  private sealed class PackLoader : IContentRoot
  {
    private readonly FileInfo? _pack;
    private readonly ISawmill _sawmill;
    private readonly Stream? _stream;
    private ZipArchive _zip;

    public PackLoader(FileInfo pack, ISawmill sawmill)
    {
      this._pack = pack;
      this._sawmill = sawmill;
    }

    public PackLoader(Stream stream, ISawmill sawmill)
    {
      this._stream = stream;
      this._sawmill = sawmill;
    }

    public void Mount()
    {
      if (this._pack != null)
      {
        this._sawmill.Info($"Loading ContentPack: {this._pack.FullName}...");
        this._zip = ZipFile.OpenRead(this._pack.FullName);
      }
      else
        this._zip = new ZipArchive(this._stream, ZipArchiveMode.Read);
    }

    public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
    {
      ZipArchiveEntry entry = this._zip.GetEntry(relPath.ToString());
      if (entry == null)
      {
        stream = (Stream) null;
        return false;
      }
      stream = (Stream) new MemoryStream();
      lock (this._zip)
      {
        using (Stream stream1 = entry.Open())
          stream1.CopyTo(stream);
      }
      stream.Position = 0L;
      return true;
    }

    public bool FileExists(ResPath relPath) => this._zip.GetEntry(relPath.ToString()) != null;

    public IEnumerable<ResPath> FindFiles(ResPath path)
    {
      string rootPath = path.ToString() + "/";
      foreach (ZipArchiveEntry entry in this._zip.Entries)
      {
        if (!(entry.Name == "") && entry.FullName.StartsWith(rootPath))
          yield return new ResPath(entry.FullName).ToRelativePath();
      }
    }

    public IEnumerable<string> GetRelativeFilePaths()
    {
      foreach (ZipArchiveEntry entry in this._zip.Entries)
      {
        if (!(entry.Name == ""))
        {
          ResPath resPath = new ResPath(entry.FullName);
          resPath = resPath.ToRootedPath();
          yield return resPath.ToString();
        }
      }
    }
  }

  private sealed class SingleStreamLoader : IContentRoot
  {
    private readonly MemoryStream _stream;
    private readonly ResPath _resourcePath;

    public SingleStreamLoader(MemoryStream stream, ResPath resourcePath)
    {
      this._stream = stream;
      this._resourcePath = resourcePath;
    }

    public void Mount()
    {
    }

    public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
    {
      if (relPath == this._resourcePath)
      {
        stream = (Stream) new MemoryStream();
        lock (this._stream)
          this._stream.CopyTo(stream);
        stream.Position = 0L;
        return true;
      }
      stream = (Stream) null;
      return false;
    }

    public bool FileExists(ResPath relPath) => relPath == this._resourcePath;

    public IEnumerable<ResPath> FindFiles(ResPath path)
    {
      if (this._resourcePath.TryRelativeTo(path, out ResPath? _))
        yield return this._resourcePath;
    }

    public IEnumerable<string> GetRelativeFilePaths()
    {
      yield return this._resourcePath.ToString();
    }
  }
}
