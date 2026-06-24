using System.IO;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

public static class ResourceManagerExt
{
	public static Stream? ContentFileReadOrNull(this IResourceManager res, ResPath path)
	{
		if (res.TryContentFileRead(path, out Stream fileStream))
		{
			return fileStream;
		}
		return null;
	}
}
