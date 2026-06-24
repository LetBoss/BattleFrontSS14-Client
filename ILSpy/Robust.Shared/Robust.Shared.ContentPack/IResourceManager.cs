using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Robust.Shared.Analyzers;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.ContentPack;

[NotContentImplementable]
public interface IResourceManager
{
	IWritableDirProvider UserData { get; }

	void AddRoot(ResPath prefix, IContentRoot loader);

	Stream ContentFileRead(ResPath path);

	Stream ContentFileRead(string path);

	bool ContentFileExists(ResPath path);

	bool ContentFileExists(string path);

	bool TryContentFileRead(ResPath? path, [NotNullWhen(true)] out Stream? fileStream);

	bool TryContentFileRead(string path, [NotNullWhen(true)] out Stream? fileStream);

	IEnumerable<ResPath> ContentFindFiles(ResPath? path);

	IEnumerable<ResPath> ContentFindRelativeFiles(ResPath path)
	{
		foreach (ResPath item in ContentFindFiles(path))
		{
			if (!item.TryRelativeTo(path, out var relative))
			{
				throw new InvalidOperationException("This is unreachable");
			}
			yield return relative.Value;
		}
	}

	IEnumerable<ResPath> ContentFindFiles(string path);

	IEnumerable<string> ContentGetDirectoryEntries(ResPath path);

	[Obsolete("This API is no longer content-accessible")]
	IEnumerable<ResPath> GetContentRoots();

	string ContentFileReadAllText(string path)
	{
		return ContentFileReadAllText(new ResPath(path));
	}

	string ContentFileReadAllText(ResPath path)
	{
		return ContentFileReadAllText(path, EncodingHelpers.UTF8);
	}

	string ContentFileReadAllText(ResPath path, Encoding encoding)
	{
		using Stream stream = ContentFileRead(path);
		using StreamReader streamReader = new StreamReader(stream, encoding);
		return streamReader.ReadToEnd();
	}

	YamlStream ContentFileReadYaml(ResPath path)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		using StreamReader streamReader = ContentFileReadText(path);
		YamlStream val = new YamlStream();
		val.Load((TextReader)streamReader);
		return val;
	}

	StreamReader ContentFileReadText(ResPath path)
	{
		return ContentFileReadText(path, EncodingHelpers.UTF8);
	}

	StreamReader ContentFileReadText(ResPath path, Encoding encoding)
	{
		return new StreamReader(ContentFileRead(path), encoding);
	}
}
