// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.VirtualWritableDirProvider
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

#nullable enable
namespace Robust.Shared.ContentPack;

public sealed class VirtualWritableDirProvider : IWritableDirProvider
{
  private readonly VirtualWritableDirProvider.DirectoryNode _rootDirectoryNode;

  public VirtualWritableDirProvider()
  {
    this._rootDirectoryNode = new VirtualWritableDirProvider.DirectoryNode();
  }

  private VirtualWritableDirProvider(VirtualWritableDirProvider.DirectoryNode node)
  {
    this._rootDirectoryNode = node;
  }

  public string? RootDir => (string) null;

  public void CreateDir(ResPath path)
  {
    path = path.IsRooted ? path.Clean() : throw new ArgumentException("Path must be rooted", nameof (path));
    VirtualWritableDirProvider.DirectoryNode directoryNode1 = this._rootDirectoryNode;
    foreach (string enumerateSegment in path.EnumerateSegments())
    {
      VirtualWritableDirProvider.INode node;
      if (directoryNode1.Children.TryGetValue(enumerateSegment, out node))
      {
        directoryNode1 = node is VirtualWritableDirProvider.DirectoryNode directoryNode2 ? directoryNode2 : throw new ArgumentException("A file already exists at that location.");
      }
      else
      {
        VirtualWritableDirProvider.DirectoryNode directoryNode3 = new VirtualWritableDirProvider.DirectoryNode();
        directoryNode1.Children.Add(enumerateSegment, (VirtualWritableDirProvider.INode) directoryNode3);
        directoryNode1 = directoryNode3;
      }
    }
  }

  public void Delete(ResPath path)
  {
    path = path.IsRooted ? path.Clean() : throw new ArgumentException("Path must be rooted", nameof (path));
    VirtualWritableDirProvider.INode node;
    if (!this.TryGetNodeAt(path.Directory, out node) || !(node is VirtualWritableDirProvider.DirectoryNode directoryNode))
      return;
    directoryNode.Children.Remove(path.Filename);
  }

  public bool Exists(ResPath path)
  {
    return this.TryGetNodeAt(path, out VirtualWritableDirProvider.INode _);
  }

  public (IEnumerable<ResPath> files, IEnumerable<ResPath> directories) Find(
    string pattern,
    bool recursive = true)
  {
    throw new NotImplementedException();
  }

  public IEnumerable<string> DirectoryEntries(ResPath path)
  {
    VirtualWritableDirProvider.INode node;
    if (!this.TryGetNodeAt(path, out node) || !(node is VirtualWritableDirProvider.DirectoryNode directoryNode))
      throw new ArgumentException("Path is not a valid directory node.");
    return (IEnumerable<string>) directoryNode.Children.Keys;
  }

  public bool IsDir(ResPath path)
  {
    VirtualWritableDirProvider.INode node;
    return this.TryGetNodeAt(path, out node) && node is VirtualWritableDirProvider.DirectoryNode;
  }

  public Stream Open(ResPath path, FileMode fileMode, FileAccess access, FileShare share)
  {
    if (!path.IsRooted)
      throw new ArgumentException("Path must be rooted", nameof (path));
    VirtualWritableDirProvider.INode node1;
    if (!this.TryGetNodeAt(path.Directory, out node1) || !(node1 is VirtualWritableDirProvider.DirectoryNode directoryNode))
      throw new ArgumentException("Parent directory does not exist.");
    string filename = path.Filename;
    VirtualWritableDirProvider.INode node2;
    if (directoryNode.Children.TryGetValue(filename, out node2) && node2 is VirtualWritableDirProvider.DirectoryNode)
      throw new ArgumentException("There is a directory at that location.");
    VirtualWritableDirProvider.FileNode fileNode1 = (VirtualWritableDirProvider.FileNode) node2;
    switch (fileMode)
    {
      case FileMode.CreateNew:
        if (fileNode1 != null)
          throw new IOException("File already exists.");
        VirtualWritableDirProvider.FileNode fileNode2 = new VirtualWritableDirProvider.FileNode();
        directoryNode.Children.Add(filename, (VirtualWritableDirProvider.INode) fileNode2);
        return (Stream) new VirtualWritableDirProvider.VirtualFileStream(fileNode2.Contents, true, true, 0L);
      case FileMode.Create:
        if (fileNode1 == null)
        {
          fileNode1 = new VirtualWritableDirProvider.FileNode();
          directoryNode.Children.Add(filename, (VirtualWritableDirProvider.INode) fileNode1);
        }
        else
          fileNode1.Contents.SetLength(0L);
        return (Stream) new VirtualWritableDirProvider.VirtualFileStream(fileNode1.Contents, true, true, 0L);
      case FileMode.Open:
        return fileNode1 != null ? (Stream) new VirtualWritableDirProvider.VirtualFileStream(fileNode1.Contents, true, true, 0L) : throw new FileNotFoundException();
      case FileMode.OpenOrCreate:
        if (fileNode1 == null)
        {
          fileNode1 = new VirtualWritableDirProvider.FileNode();
          directoryNode.Children.Add(filename, (VirtualWritableDirProvider.INode) fileNode1);
        }
        return (Stream) new VirtualWritableDirProvider.VirtualFileStream(fileNode1.Contents, true, true, 0L);
      case FileMode.Truncate:
        if (fileNode1 == null)
          throw new FileNotFoundException();
        fileNode1.Contents.SetLength(0L);
        return (Stream) new VirtualWritableDirProvider.VirtualFileStream(fileNode1.Contents, true, true, 0L);
      case FileMode.Append:
        if (fileNode1 == null)
        {
          fileNode1 = new VirtualWritableDirProvider.FileNode();
          directoryNode.Children.Add(filename, (VirtualWritableDirProvider.INode) fileNode1);
        }
        return (Stream) new VirtualWritableDirProvider.VirtualFileStream(fileNode1.Contents, false, true, fileNode1.Contents.Length);
      default:
        throw new ArgumentOutOfRangeException(nameof (fileMode), (object) fileMode, (string) null);
    }
  }

  public void Rename(ResPath oldPath, ResPath newPath)
  {
    if (!oldPath.IsRooted)
      throw new ArgumentException("Path must be rooted", nameof (oldPath));
    if (!newPath.IsRooted)
      throw new ArgumentException("Path must be rooted", nameof (newPath));
    VirtualWritableDirProvider.INode node1;
    if (!this.TryGetNodeAt(oldPath.Directory, out node1) || !(node1 is VirtualWritableDirProvider.DirectoryNode directoryNode1))
      throw new ArgumentException("Source directory does not exist.");
    VirtualWritableDirProvider.INode node2;
    if (!this.TryGetNodeAt(newPath.Directory, out node2) || !(node2 is VirtualWritableDirProvider.DirectoryNode directoryNode2))
      throw new ArgumentException("Target directory does not exist.");
    string filename1 = newPath.Filename;
    if (directoryNode2.Children.ContainsKey(filename1))
      throw new ArgumentException("Target node already exists");
    string filename2 = oldPath.Filename;
    VirtualWritableDirProvider.INode node3;
    if (!directoryNode1.Children.TryGetValue(filename2, out node3))
      throw new ArgumentException("Node does not exist in original directory.");
    directoryNode1.Children.Remove(filename2);
    directoryNode2.Children.Add(filename1, node3);
  }

  public void OpenOsWindow(ResPath path)
  {
  }

  private bool TryGetNodeAt(ResPath path, [NotNullWhen(true)] out VirtualWritableDirProvider.INode? node)
  {
    path = path.IsRooted ? path.Clean() : throw new ArgumentException("Path must be rooted", nameof (path));
    if (path == ResPath.Root)
    {
      node = (VirtualWritableDirProvider.INode) this._rootDirectoryNode;
      return true;
    }
    VirtualWritableDirProvider.DirectoryNode directoryNode = this._rootDirectoryNode;
    string[] strArray = path.EnumerateSegments();
    for (int index = 0; index < strArray.Length; ++index)
    {
      string key = strArray[index];
      VirtualWritableDirProvider.INode node1;
      if (!directoryNode.Children.TryGetValue(key, out node1))
      {
        node = (VirtualWritableDirProvider.INode) null;
        return false;
      }
      if (index == strArray.Length - 1)
      {
        node = node1;
        return true;
      }
      directoryNode = (VirtualWritableDirProvider.DirectoryNode) node1;
    }
    throw new InvalidOperationException("Unreachable.");
  }

  public IWritableDirProvider OpenSubdirectory(ResPath path)
  {
    VirtualWritableDirProvider.INode node1;
    if (!this.TryGetNodeAt(path, out node1) || !(node1 is VirtualWritableDirProvider.DirectoryNode node2))
      throw new FileNotFoundException();
    return (IWritableDirProvider) new VirtualWritableDirProvider(node2);
  }

  private interface INode
  {
  }

  private sealed class FileNode : VirtualWritableDirProvider.INode
  {
    public MemoryStream Contents { get; } = new MemoryStream();
  }

  private sealed class DirectoryNode : VirtualWritableDirProvider.INode
  {
    public Dictionary<string, VirtualWritableDirProvider.INode> Children { get; } = new Dictionary<string, VirtualWritableDirProvider.INode>();
  }

  private sealed class VirtualFileStream : Stream
  {
    private readonly MemoryStream _source;

    public VirtualFileStream(
      MemoryStream source,
      bool canRead,
      bool canWrite,
      long initialPosition)
    {
      this._source = source;
      this.CanRead = canRead;
      this.CanWrite = canWrite;
      this.Position = initialPosition;
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (!this.CanRead)
        throw new InvalidOperationException("Cannot read from this stream.");
      this._source.Position = this.Position;
      int num = this._source.Read(buffer, offset, count);
      this.Position = this._source.Position;
      return num;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          this.Position = offset;
          break;
        case SeekOrigin.Current:
          this.Position += offset;
          break;
        case SeekOrigin.End:
          this.Position = this.Length + offset;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (origin), (object) origin, (string) null);
      }
      return this.Position;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._source.Position = this.Position;
      this._source.Write(buffer, offset, count);
      this.Position = this._source.Position;
    }

    public override bool CanRead { get; }

    public override bool CanSeek => true;

    public override bool CanWrite { get; }

    public override long Length => this._source.Position;

    public override long Position { get; set; }
  }
}
