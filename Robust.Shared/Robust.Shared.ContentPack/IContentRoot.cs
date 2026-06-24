using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

public interface IContentRoot
{
	void Mount();

	bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream);

	bool FileExists(ResPath relPath);

	IEnumerable<ResPath> FindFiles(ResPath path);

	IEnumerable<string> GetRelativeFilePaths();

	IEnumerable<string> GetEntries(ResPath path)
	{
		int countDirs = ((!(path == ResPath.Self)) ? path.CanonPath.Split('/').Count() : 0);
		return FindFiles(path).Select(delegate(ResPath c)
		{
			string[] source = c.CanonPath.Split('/');
			int num = source.Count();
			string text = source.Skip(countDirs).First();
			if (num > countDirs + 1)
			{
				text += "/";
			}
			return text;
		}).Distinct();
	}
}
