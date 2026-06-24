using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

[Virtual]
internal class ResourceManager : IResourceManagerInternal, IResourceManager
{
	private sealed class DirLoader : IContentRoot
	{
		private readonly DirectoryInfo _directory;

		private readonly ISawmill _sawmill;

		private readonly bool _checkCasing;

		public DirLoader(DirectoryInfo directory, ISawmill sawmill, bool checkCasing)
		{
			_directory = directory;
			_sawmill = sawmill;
			_checkCasing = checkCasing;
		}

		public void Mount()
		{
		}

		public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
		{
			FileStream stream2;
			bool result = FileHelper.TryOpenFileRead(GetPath(relPath), out stream2);
			stream = stream2;
			return result;
		}

		public bool FileExists(ResPath relPath)
		{
			return File.Exists(GetPath(relPath));
		}

		internal string GetPath(ResPath relPath)
		{
			return PathHelpers.SafeGetResourcePath(_directory.FullName, relPath);
		}

		public IEnumerable<ResPath> FindFiles(ResPath path)
		{
			string path2 = GetPath(path);
			if (!Directory.Exists(path2))
			{
				yield break;
			}
			IEnumerable<string> files = PathHelpers.GetFiles(path2);
			foreach (string item in files)
			{
				string path3 = item.Substring(_directory.FullName.Length);
				yield return ResPath.FromRelativeSystemPath(path3);
			}
		}

		public IEnumerable<string> GetEntries(ResPath path)
		{
			string fullPath = GetPath(path);
			if (!Directory.Exists(fullPath))
			{
				return Enumerable.Empty<string>();
			}
			return Directory.EnumerateFileSystemEntries(fullPath).Select(delegate(string c)
			{
				string relativePath = Path.GetRelativePath(fullPath, c);
				return Directory.Exists(c) ? (relativePath + "/") : relativePath;
			});
		}

		[Conditional("DEBUG")]
		private void CheckPathCasing(ResPath path)
		{
			if (!_checkCasing)
			{
				return;
			}
			Task.Run(delegate
			{
				string text = GetPath(ResPath.Root);
				ResPath root = ResPath.Root;
				bool flag = false;
				string[] array = path.CanonPath.Split('/');
				foreach (string b in array)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(text);
					bool flag2 = false;
					foreach (FileSystemInfo item in directoryInfo.EnumerateFileSystemInfos())
					{
						if (string.Equals(item.Name, b, StringComparison.InvariantCultureIgnoreCase))
						{
							if (!string.Equals(item.Name, b, StringComparison.InvariantCulture))
							{
								flag = true;
							}
							root /= item.Name;
							text = Path.Combine(text, item.Name);
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						return;
					}
				}
				if (flag)
				{
					_sawmill.Warning("Path '{0}' has mismatching case from file on disk ('{1}'). This can cause loading failures on certain file system configurations and should be corrected.", path, root);
				}
			});
		}

		public IEnumerable<string> GetRelativeFilePaths()
		{
			return GetRelativeFilePaths(_directory);
		}

		private IEnumerable<string> GetRelativeFilePaths(DirectoryInfo dir)
		{
			foreach (FileInfo item in dir.EnumerateFiles())
			{
				if ((item.Attributes & FileAttributes.Hidden) == 0 && item.Name[0] != '.')
				{
					string path = item.FullName.Substring(_directory.FullName.Length);
					yield return ResPath.FromRelativeSystemPath(path).ToRootedPath().ToString();
				}
			}
			foreach (DirectoryInfo item2 in dir.EnumerateDirectories())
			{
				if ((item2.Attributes & FileAttributes.Hidden) != FileAttributes.None || item2.Name[0] == '.')
				{
					continue;
				}
				foreach (string relativeFilePath in GetRelativeFilePaths(item2))
				{
					yield return relativeFilePath;
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
			_pack = pack;
			_sawmill = sawmill;
		}

		public PackLoader(Stream stream, ISawmill sawmill)
		{
			_stream = stream;
			_sawmill = sawmill;
		}

		public void Mount()
		{
			if (_pack != null)
			{
				_sawmill.Info("Loading ContentPack: " + _pack.FullName + "...");
				_zip = ZipFile.OpenRead(_pack.FullName);
			}
			else
			{
				_zip = new ZipArchive(_stream, ZipArchiveMode.Read);
			}
		}

		public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
		{
			ZipArchiveEntry entry = _zip.GetEntry(relPath.ToString());
			if (entry == null)
			{
				stream = null;
				return false;
			}
			stream = new MemoryStream();
			lock (_zip)
			{
				using Stream stream2 = entry.Open();
				stream2.CopyTo(stream);
			}
			stream.Position = 0L;
			return true;
		}

		public bool FileExists(ResPath relPath)
		{
			return _zip.GetEntry(relPath.ToString()) != null;
		}

		public IEnumerable<ResPath> FindFiles(ResPath path)
		{
			string rootPath = path.ToString() + "/";
			foreach (ZipArchiveEntry entry in _zip.Entries)
			{
				if (!(entry.Name == "") && entry.FullName.StartsWith(rootPath))
				{
					yield return new ResPath(entry.FullName).ToRelativePath();
				}
			}
		}

		public IEnumerable<string> GetRelativeFilePaths()
		{
			foreach (ZipArchiveEntry entry in _zip.Entries)
			{
				if (!(entry.Name == ""))
				{
					yield return new ResPath(entry.FullName).ToRootedPath().ToString();
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
			_stream = stream;
			_resourcePath = resourcePath;
		}

		public void Mount()
		{
		}

		public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
		{
			if (relPath == _resourcePath)
			{
				stream = new MemoryStream();
				lock (_stream)
				{
					_stream.CopyTo(stream);
				}
				stream.Position = 0L;
				return true;
			}
			stream = null;
			return false;
		}

		public bool FileExists(ResPath relPath)
		{
			return relPath == _resourcePath;
		}

		public IEnumerable<ResPath> FindFiles(ResPath path)
		{
			if (_resourcePath.TryRelativeTo(path, out var _))
			{
				yield return _resourcePath;
			}
		}

		public IEnumerable<string> GetRelativeFilePaths()
		{
			yield return _resourcePath.ToString();
		}
	}

	[Dependency]
	private readonly IConfigurationManager _config;

	[Dependency]
	private readonly ILogManager _logManager;

	private (ResPath prefix, IContentRoot root)[] _contentRoots = new(ResPath, IContentRoot)[0];

	private StreamSeekMode _streamSeekMode;

	private readonly object _rootMutateLock = new object();

	private static readonly Regex BadPathSegmentRegex = new Regex("^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])$", RegexOptions.IgnoreCase);

	private static readonly Regex BadPathCharacterRegex = new Regex("[<>:\"|?*\0\\x01-\\x1f]", RegexOptions.IgnoreCase);

	protected ISawmill Sawmill;

	public IWritableDirProvider UserData { get; private set; }

	public virtual void Initialize(string? userData, bool hideRootDir)
	{
		Sawmill = _logManager.GetSawmill("res");
		if (userData != null)
		{
			UserData = new WritableDirProvider(Directory.CreateDirectory(userData), hideRootDir);
		}
		else
		{
			UserData = new VirtualWritableDirProvider();
		}
		_config.OnValueChanged(CVars.ResStreamSeekMode, delegate(int i)
		{
			_streamSeekMode = (StreamSeekMode)i;
		}, invokeImmediately: true);
	}

	public void MountDefaultContentPack()
	{
		string cVar = _config.GetCVar<string>("resource.pack");
		if (string.IsNullOrWhiteSpace(cVar))
		{
			Sawmill.Warning("No default ContentPack to load in configuration.");
		}
		else
		{
			MountContentPack(cVar);
		}
	}

	public void MountContentPack(string pack, ResPath? prefix = null)
	{
		prefix = SanitizePrefix(prefix);
		if (!Path.IsPathRooted(pack))
		{
			pack = PathHelpers.ExecutableRelativeFile(pack);
		}
		FileInfo fileInfo = new FileInfo(pack);
		if (!fileInfo.Exists)
		{
			throw new FileNotFoundException("Specified ContentPack does not exist: " + fileInfo.FullName);
		}
		PackLoader loader = new PackLoader(fileInfo, Sawmill);
		AddRoot(prefix.Value, loader);
	}

	public void MountContentPack(Stream zipStream, ResPath? prefix = null)
	{
		prefix = SanitizePrefix(prefix);
		PackLoader loader = new PackLoader(zipStream, Sawmill);
		AddRoot(prefix.Value, loader);
	}

	public void AddRoot(ResPath prefix, IContentRoot loader)
	{
		lock (_rootMutateLock)
		{
			loader.Mount();
			(ResPath, IContentRoot)[] array = _contentRoots;
			Array.Resize(ref array, array.Length + 1);
			array[^1] = (prefix, loader);
			_contentRoots = array;
		}
	}

	private static ResPath SanitizePrefix(ResPath? prefix)
	{
		if (!prefix.HasValue)
		{
			prefix = ResPath.Root;
		}
		else if (!prefix.Value.IsRooted)
		{
			throw new ArgumentException("Prefix must be rooted.", "prefix");
		}
		return prefix.Value;
	}

	public void MountContentDirectory(string path, ResPath? prefix = null)
	{
		prefix = SanitizePrefix(prefix);
		if (!Path.IsPathRooted(path))
		{
			path = PathHelpers.ExecutableRelativeFile(path);
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (!directoryInfo.Exists)
		{
			throw new DirectoryNotFoundException("Specified directory does not exist: " + directoryInfo.FullName);
		}
		DirLoader loader = new DirLoader(directoryInfo, _logManager.GetSawmill("res"), _config.GetCVar(CVars.ResCheckPathCasing));
		AddRoot(prefix.Value, loader);
	}

	public Stream ContentFileRead(string path)
	{
		return ContentFileRead(new ResPath(path));
	}

	public Stream ContentFileRead(ResPath path)
	{
		if (TryContentFileRead(path, out Stream fileStream))
		{
			return fileStream;
		}
		throw new FileNotFoundException($"Path does not exist in the VFS: '{path}'");
	}

	public bool TryContentFileRead(string path, [NotNullWhen(true)] out Stream? fileStream)
	{
		return TryContentFileRead(new ResPath(path), out fileStream);
	}

	public bool TryContentFileRead(ResPath? path, [NotNullWhen(true)] out Stream? fileStream)
	{
		if (!path.HasValue)
		{
			throw new ArgumentNullException("path");
		}
		if (!path.Value.IsRooted)
		{
			throw new ArgumentException($"Path '{path}' must be rooted", "path");
		}
		if (path.Value.CanonPath.EndsWith('/'))
		{
			fileStream = null;
			return false;
		}
		(ResPath, IContentRoot)[] contentRoots = _contentRoots;
		for (int i = 0; i < contentRoots.Length; i++)
		{
			var (basePath, contentRoot) = contentRoots[i];
			if (path.Value.TryRelativeTo(basePath, out var relative) && contentRoot.TryGetFile(relative.Value, out Stream stream))
			{
				fileStream = WrapStream(stream);
				return true;
			}
		}
		fileStream = null;
		return false;
	}

	private Stream WrapStream(Stream stream)
	{
		switch (_streamSeekMode)
		{
		case StreamSeekMode.None:
			return stream;
		case StreamSeekMode.ForceSeekable:
		{
			if (stream.CanSeek)
			{
				return stream;
			}
			MemoryStream result = new MemoryStream(stream.CopyToArray(), writable: false);
			stream.Dispose();
			return result;
		}
		case StreamSeekMode.ForceNonSeekable:
			if (!stream.CanSeek)
			{
				return stream;
			}
			return new NonSeekableStream(stream);
		default:
			throw new InvalidOperationException();
		}
	}

	public bool ContentFileExists(string path)
	{
		return ContentFileExists(new ResPath(path));
	}

	public bool ContentFileExists(ResPath path)
	{
		if (TryContentFileRead(path, out Stream fileStream))
		{
			fileStream.Dispose();
			return true;
		}
		return false;
	}

	public IEnumerable<ResPath> ContentFindFiles(string path)
	{
		return ContentFindFiles(new ResPath(path));
	}

	public IEnumerable<string> ContentGetDirectoryEntries(ResPath path)
	{
		if (!path.IsRooted)
		{
			throw new ArgumentException("Path is not rooted", "path");
		}
		if (!path.CanonPath.EndsWith("/"))
		{
			path = new ResPath(path.CanonPath + "/");
		}
		HashSet<string> hashSet = new HashSet<string>();
		(ResPath, IContentRoot)[] contentRoots = _contentRoots;
		for (int i = 0; i < contentRoots.Length; i++)
		{
			var (basePath, contentRoot) = contentRoots[i];
			if (path.TryRelativeTo(basePath, out var relative))
			{
				hashSet.UnionWith(contentRoot.GetEntries(relative.Value));
			}
		}
		contentRoots = _contentRoots;
		for (int i = 0; i < contentRoots.Length; i++)
		{
			ResPath item = contentRoots[i].Item1;
			if (item.TryRelativeTo(path, out var relative2))
			{
				string[] array = relative2.Value.EnumerateSegments();
				if (array == null || array.Length != 1 || !(array[0] == "."))
				{
					hashSet.Add(array[0] + "/");
				}
			}
		}
		return hashSet;
	}

	public IEnumerable<ResPath> ContentFindFiles(ResPath? path)
	{
		if (!path.HasValue)
		{
			throw new ArgumentNullException("path");
		}
		if (!path.Value.IsRooted)
		{
			throw new ArgumentException("Path is not rooted", "path");
		}
		HashSet<ResPath> alreadyReturnedFiles = new HashSet<ResPath>();
		(ResPath prefix, IContentRoot root)[] contentRoots = _contentRoots;
		for (int i = 0; i < contentRoots.Length; i++)
		{
			var (prefix, contentRoot) = contentRoots[i];
			if (!path.Value.TryRelativeTo(prefix, out var relative))
			{
				continue;
			}
			foreach (ResPath item in contentRoot.FindFiles(relative.Value))
			{
				ResPath resPath = prefix / item;
				if (!alreadyReturnedFiles.Contains(resPath))
				{
					alreadyReturnedFiles.Add(resPath);
					yield return resPath;
				}
			}
		}
	}

	public bool TryGetDiskFilePath(ResPath path, [NotNullWhen(true)] out string? diskPath)
	{
		(ResPath, IContentRoot)[] contentRoots = _contentRoots;
		for (int i = 0; i < contentRoots.Length; i++)
		{
			(ResPath, IContentRoot) tuple = contentRoots[i];
			var (basePath, _) = tuple;
			if (tuple.Item2 is DirLoader dirLoader && path.TryRelativeTo(basePath, out var relative))
			{
				diskPath = dirLoader.GetPath(relative.Value);
				if (File.Exists(diskPath))
				{
					return true;
				}
			}
		}
		diskPath = null;
		return false;
	}

	public void MountStreamAt(MemoryStream stream, ResPath path)
	{
		SingleStreamLoader loader = new SingleStreamLoader(stream, path.ToRelativePath());
		AddRoot(ResPath.Root, loader);
	}

	IEnumerable<ResPath> IResourceManager.GetContentRoots()
	{
		return Array.Empty<ResPath>();
	}

	public IEnumerable<string> GetContentRoots()
	{
		(ResPath prefix, IContentRoot root)[] contentRoots = _contentRoots;
		for (int i = 0; i < contentRoots.Length; i++)
		{
			if (contentRoots[i].root is DirLoader dirLoader)
			{
				yield return dirLoader.GetPath(ResPath.Root);
			}
		}
	}

	internal static bool IsPathValid(ResPath path)
	{
		string input = path.ToString();
		if (BadPathCharacterRegex.IsMatch(input))
		{
			return false;
		}
		string[] array = path.CanonPath.Split('/');
		foreach (string input2 in array)
		{
			if (BadPathSegmentRegex.IsMatch(input2))
			{
				return false;
			}
		}
		return true;
	}
}
