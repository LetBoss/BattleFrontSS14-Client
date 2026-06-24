using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

public sealed class VirtualWritableDirProvider : IWritableDirProvider
{
	private interface INode
	{
	}

	private sealed class FileNode : INode
	{
		public MemoryStream Contents { get; } = new MemoryStream();
	}

	private sealed class DirectoryNode : INode
	{
		public Dictionary<string, INode> Children { get; } = new Dictionary<string, INode>();
	}

	private sealed class VirtualFileStream : Stream
	{
		private readonly MemoryStream _source;

		public override bool CanRead { get; }

		public override bool CanSeek => true;

		public override bool CanWrite { get; }

		public override long Length => _source.Position;

		public override long Position { get; set; }

		public VirtualFileStream(MemoryStream source, bool canRead, bool canWrite, long initialPosition)
		{
			_source = source;
			CanRead = canRead;
			CanWrite = canWrite;
			Position = initialPosition;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!CanRead)
			{
				throw new InvalidOperationException("Cannot read from this stream.");
			}
			_source.Position = Position;
			int result = _source.Read(buffer, offset, count);
			Position = _source.Position;
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				Position = offset;
				break;
			case SeekOrigin.Current:
				Position += offset;
				break;
			case SeekOrigin.End:
				Position = Length + offset;
				break;
			default:
				throw new ArgumentOutOfRangeException("origin", origin, null);
			}
			return Position;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_source.Position = Position;
			_source.Write(buffer, offset, count);
			Position = _source.Position;
		}
	}

	private readonly DirectoryNode _rootDirectoryNode;

	public string? RootDir => null;

	public VirtualWritableDirProvider()
	{
		_rootDirectoryNode = new DirectoryNode();
	}

	private VirtualWritableDirProvider(DirectoryNode node)
	{
		_rootDirectoryNode = node;
	}

	public void CreateDir(ResPath path)
	{
		if (!path.IsRooted)
		{
			throw new ArgumentException("Path must be rooted", "path");
		}
		path = path.Clean();
		DirectoryNode directoryNode = _rootDirectoryNode;
		string[] array = path.EnumerateSegments();
		foreach (string key in array)
		{
			if (directoryNode.Children.TryGetValue(key, out INode value))
			{
				directoryNode = (value as DirectoryNode) ?? throw new ArgumentException("A file already exists at that location.");
				continue;
			}
			DirectoryNode directoryNode2 = new DirectoryNode();
			directoryNode.Children.Add(key, directoryNode2);
			directoryNode = directoryNode2;
		}
	}

	public void Delete(ResPath path)
	{
		if (!path.IsRooted)
		{
			throw new ArgumentException("Path must be rooted", "path");
		}
		path = path.Clean();
		ResPath directory = path.Directory;
		if (TryGetNodeAt(directory, out INode node) && node is DirectoryNode directoryNode)
		{
			directoryNode.Children.Remove(path.Filename);
		}
	}

	public bool Exists(ResPath path)
	{
		INode node;
		return TryGetNodeAt(path, out node);
	}

	public (IEnumerable<ResPath> files, IEnumerable<ResPath> directories) Find(string pattern, bool recursive = true)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<string> DirectoryEntries(ResPath path)
	{
		if (!TryGetNodeAt(path, out INode node) || !(node is DirectoryNode directoryNode))
		{
			throw new ArgumentException("Path is not a valid directory node.");
		}
		return directoryNode.Children.Keys;
	}

	public bool IsDir(ResPath path)
	{
		if (TryGetNodeAt(path, out INode node))
		{
			return node is DirectoryNode;
		}
		return false;
	}

	public Stream Open(ResPath path, FileMode fileMode, FileAccess access, FileShare share)
	{
		if (!path.IsRooted)
		{
			throw new ArgumentException("Path must be rooted", "path");
		}
		ResPath directory = path.Directory;
		if (!TryGetNodeAt(directory, out INode node) || !(node is DirectoryNode directoryNode))
		{
			throw new ArgumentException("Parent directory does not exist.");
		}
		string filename = path.Filename;
		if (directoryNode.Children.TryGetValue(filename, out INode value) && value is DirectoryNode)
		{
			throw new ArgumentException("There is a directory at that location.");
		}
		FileNode fileNode = (FileNode)value;
		switch (fileMode)
		{
		case FileMode.Append:
			if (fileNode == null)
			{
				fileNode = new FileNode();
				directoryNode.Children.Add(filename, fileNode);
			}
			return new VirtualFileStream(fileNode.Contents, canRead: false, canWrite: true, fileNode.Contents.Length);
		case FileMode.Create:
			if (fileNode == null)
			{
				fileNode = new FileNode();
				directoryNode.Children.Add(filename, fileNode);
			}
			else
			{
				fileNode.Contents.SetLength(0L);
			}
			return new VirtualFileStream(fileNode.Contents, canRead: true, canWrite: true, 0L);
		case FileMode.CreateNew:
			if (fileNode != null)
			{
				throw new IOException("File already exists.");
			}
			fileNode = new FileNode();
			directoryNode.Children.Add(filename, fileNode);
			return new VirtualFileStream(fileNode.Contents, canRead: true, canWrite: true, 0L);
		case FileMode.Open:
			if (fileNode == null)
			{
				throw new FileNotFoundException();
			}
			return new VirtualFileStream(fileNode.Contents, canRead: true, canWrite: true, 0L);
		case FileMode.OpenOrCreate:
			if (fileNode == null)
			{
				fileNode = new FileNode();
				directoryNode.Children.Add(filename, fileNode);
			}
			return new VirtualFileStream(fileNode.Contents, canRead: true, canWrite: true, 0L);
		case FileMode.Truncate:
			if (fileNode == null)
			{
				throw new FileNotFoundException();
			}
			fileNode.Contents.SetLength(0L);
			return new VirtualFileStream(fileNode.Contents, canRead: true, canWrite: true, 0L);
		default:
			throw new ArgumentOutOfRangeException("fileMode", fileMode, null);
		}
	}

	public void Rename(ResPath oldPath, ResPath newPath)
	{
		if (!oldPath.IsRooted)
		{
			throw new ArgumentException("Path must be rooted", "oldPath");
		}
		if (!newPath.IsRooted)
		{
			throw new ArgumentException("Path must be rooted", "newPath");
		}
		if (!TryGetNodeAt(oldPath.Directory, out INode node) || !(node is DirectoryNode directoryNode))
		{
			throw new ArgumentException("Source directory does not exist.");
		}
		if (!TryGetNodeAt(newPath.Directory, out INode node2) || !(node2 is DirectoryNode directoryNode2))
		{
			throw new ArgumentException("Target directory does not exist.");
		}
		string filename = newPath.Filename;
		if (directoryNode2.Children.ContainsKey(filename))
		{
			throw new ArgumentException("Target node already exists");
		}
		string filename2 = oldPath.Filename;
		if (!directoryNode.Children.TryGetValue(filename2, out INode value))
		{
			throw new ArgumentException("Node does not exist in original directory.");
		}
		directoryNode.Children.Remove(filename2);
		directoryNode2.Children.Add(filename, value);
	}

	public void OpenOsWindow(ResPath path)
	{
	}

	private bool TryGetNodeAt(ResPath path, [NotNullWhen(true)] out INode? node)
	{
		if (!path.IsRooted)
		{
			throw new ArgumentException("Path must be rooted", "path");
		}
		path = path.Clean();
		if (path == ResPath.Root)
		{
			node = _rootDirectoryNode;
			return true;
		}
		DirectoryNode directoryNode = _rootDirectoryNode;
		string[] array = path.EnumerateSegments();
		for (int i = 0; i < array.Length; i++)
		{
			string key = array[i];
			if (!directoryNode.Children.TryGetValue(key, out INode value))
			{
				node = null;
				return false;
			}
			if (i == array.Length - 1)
			{
				node = value;
				return true;
			}
			directoryNode = (DirectoryNode)value;
		}
		throw new InvalidOperationException("Unreachable.");
	}

	public IWritableDirProvider OpenSubdirectory(ResPath path)
	{
		if (!TryGetNodeAt(path, out INode node) || !(node is DirectoryNode node2))
		{
			throw new FileNotFoundException();
		}
		return new VirtualWritableDirProvider(node2);
	}
}
