using System.Collections.Generic;
using System.IO;
using Robust.Shared.Analyzers;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

[NotContentImplementable]
public interface IWritableDirProvider
{
	string? RootDir { get; }

	void CreateDir(ResPath path);

	void Delete(ResPath path);

	bool Exists(ResPath path);

	(IEnumerable<ResPath> files, IEnumerable<ResPath> directories) Find(string pattern, bool recursive = true);

	IEnumerable<string> DirectoryEntries(ResPath path);

	bool IsDir(ResPath path);

	Stream Open(ResPath path, FileMode fileMode, FileAccess access, FileShare share);

	Stream Open(ResPath path, FileMode fileMode)
	{
		return Open(path, fileMode, (fileMode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.None);
	}

	IWritableDirProvider OpenSubdirectory(ResPath path);

	void Rename(ResPath oldPath, ResPath newPath);

	void OpenOsWindow(ResPath path);
}
