using System;
using System.Collections.Generic;
using System.IO;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

internal static class PathHelpers
{
	internal static string GetExecutableDirectory()
	{
		string location = typeof(PathHelpers).Assembly.Location;
		if (location == string.Empty)
		{
			throw new InvalidOperationException("Cannot find path of executable.");
		}
		return Path.GetDirectoryName(location);
	}

	public static string ExecutableRelativeFile(string file)
	{
		return Path.GetFullPath(Path.Combine(GetExecutableDirectory(), file));
	}

	public static IEnumerable<string> GetFiles(string path)
	{
		return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
	}

	public static bool IsFileInUse(IOException exception)
	{
		return (exception.HResult & 0xFFFF) switch
		{
			32 => true, 
			33 => true, 
			_ => false, 
		};
	}

	public static bool IsFileSystemCaseSensitive()
	{
		if (!OperatingSystem.IsWindows())
		{
			return !OperatingSystem.IsMacOS();
		}
		return false;
	}

	internal static string SafeGetResourcePath(string baseDir, ResPath path)
	{
		string text = path.ToRelativeSystemPath();
		if (text.Contains("\\..") || text.Contains("/..") || text.StartsWith(".."))
		{
			throw new InvalidOperationException($"This branch should never be reached. Path: {path}");
		}
		string fullPath = Path.GetFullPath(Path.Join(baseDir, text));
		if (!fullPath.StartsWith(baseDir) && fullPath != baseDir.TrimEnd(Path.DirectorySeparatorChar))
		{
			throw new InvalidOperationException($"This branch should never be reached. Path: {path}");
		}
		return fullPath;
	}
}
