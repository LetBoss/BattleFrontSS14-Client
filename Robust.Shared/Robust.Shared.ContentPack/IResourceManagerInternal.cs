using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

internal interface IResourceManagerInternal : IResourceManager
{
	void Initialize(string? userData, bool hideUserDataDir);

	void MountStreamAt(MemoryStream stream, ResPath path);

	void MountDefaultContentPack();

	void MountContentPack(string pack, ResPath? prefix = null);

	void MountContentPack(Stream zipStream, ResPath? prefix = null);

	void MountContentDirectory(string path, ResPath? prefix = null);

	bool TryGetDiskFilePath(ResPath path, [NotNullWhen(true)] out string? diskPath);

	new IEnumerable<string> GetContentRoots();
}
