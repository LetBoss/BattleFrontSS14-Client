using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

internal sealed class WritableDirProvider : IWritableDirProvider
{
	private readonly bool _hideRootDir;

	public string RootDir { get; }

	string? IWritableDirProvider.RootDir
	{
		get
		{
			if (!_hideRootDir)
			{
				return RootDir;
			}
			return null;
		}
	}

	public WritableDirProvider(DirectoryInfo rootDir, bool hideRootDir)
	{
		RootDir = rootDir.FullName + Path.DirectorySeparatorChar;
		_hideRootDir = hideRootDir;
	}

	public void CreateDir(ResPath path)
	{
		Directory.CreateDirectory(GetFullPath(path));
	}

	public void Delete(ResPath path)
	{
		string fullPath = GetFullPath(path);
		if (Directory.Exists(fullPath))
		{
			Directory.Delete(fullPath, recursive: true);
		}
		else if (File.Exists(fullPath))
		{
			File.Delete(fullPath);
		}
	}

	public bool Exists(ResPath path)
	{
		string fullPath = GetFullPath(path);
		if (!Directory.Exists(fullPath))
		{
			return File.Exists(fullPath);
		}
		return true;
	}

	public (IEnumerable<ResPath> files, IEnumerable<ResPath> directories) Find(string pattern, bool recursive = true)
	{
		if (pattern.Contains(".."))
		{
			throw new InvalidOperationException("Pattern may not contain '..'. Pattern: " + pattern + ".");
		}
		int startIndex = RootDir.Length - 1;
		SearchOption searchOption = (recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		string[] files = Directory.GetFiles(RootDir, pattern, searchOption);
		string[] directories = Directory.GetDirectories(RootDir, pattern, searchOption);
		List<ResPath> list = new List<ResPath>(files.Length);
		List<ResPath> list2 = new List<ResPath>(directories.Length);
		string[] array = files;
		foreach (string text in array)
		{
			if (!text.Contains("\\..") && !text.Contains("/.."))
			{
				list.Add(ResPath.FromRelativeSystemPath(text.Substring(startIndex)).ToRootedPath());
			}
		}
		array = directories;
		foreach (string text2 in array)
		{
			if (!text2.Contains("\\..") && !text2.Contains("/.."))
			{
				list2.Add(ResPath.FromRelativeSystemPath(text2.Substring(startIndex)).ToRootedPath());
			}
		}
		return (files: list, directories: list2);
	}

	public IEnumerable<string> DirectoryEntries(ResPath path)
	{
		string fullPath = GetFullPath(path);
		if (!Directory.Exists(fullPath))
		{
			yield break;
		}
		foreach (string item in Directory.EnumerateFileSystemEntries(fullPath))
		{
			yield return Path.GetRelativePath(fullPath, item);
		}
	}

	public bool IsDir(ResPath path)
	{
		return Directory.Exists(GetFullPath(path));
	}

	public Stream Open(ResPath path, FileMode fileMode, FileAccess access, FileShare share)
	{
		return File.Open(GetFullPath(path), fileMode, access, share);
	}

	public IWritableDirProvider OpenSubdirectory(ResPath path)
	{
		if (!IsDir(path))
		{
			throw new FileNotFoundException();
		}
		return new WritableDirProvider(new DirectoryInfo(GetFullPath(path)), _hideRootDir);
	}

	public void Rename(ResPath oldPath, ResPath newPath)
	{
		string fullPath = GetFullPath(oldPath);
		string fullPath2 = GetFullPath(newPath);
		File.Move(fullPath, fullPath2);
	}

	public void OpenOsWindow(ResPath path)
	{
		if (!IsDir(path))
		{
			path = path.Directory;
		}
		string fullPath = GetFullPath(path);
		if (OperatingSystem.IsWindows())
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = Environment.GetEnvironmentVariable("SystemRoot") + "\\explorer.exe",
				Arguments = ".",
				WorkingDirectory = fullPath
			});
			return;
		}
		if (OperatingSystem.IsMacOS())
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "open",
				Arguments = ".",
				WorkingDirectory = fullPath
			});
			return;
		}
		if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "xdg-open",
				Arguments = ".",
				WorkingDirectory = fullPath
			});
			return;
		}
		throw new NotSupportedException("Opening OS windows not supported on this OS");
	}

	public string GetFullPath(ResPath path)
	{
		if (!path.IsRooted)
		{
			throw new ArgumentException($"Path must be rooted. Path: {path}");
		}
		path = path.Clean();
		return PathHelpers.SafeGetResourcePath(RootDir, path);
	}
}
