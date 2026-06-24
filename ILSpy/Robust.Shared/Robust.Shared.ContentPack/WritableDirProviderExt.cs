using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

public static class WritableDirProviderExt
{
	public static Stream OpenRead(this IWritableDirProvider provider, ResPath path)
	{
		return provider.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
	}

	public static StreamReader OpenText(this IWritableDirProvider provider, ResPath path)
	{
		return new StreamReader(provider.OpenRead(path), EncodingHelpers.UTF8);
	}

	public static Stream OpenWrite(this IWritableDirProvider provider, ResPath path)
	{
		return provider.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
	}

	public static StreamWriter OpenWriteText(this IWritableDirProvider provider, ResPath path)
	{
		return new StreamWriter(provider.OpenWrite(path), EncodingHelpers.UTF8);
	}

	public static void AppendAllText(this IWritableDirProvider provider, ResPath path, ReadOnlySpan<char> content)
	{
		using Stream stream = provider.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read);
		using StreamWriter streamWriter = new StreamWriter(stream, EncodingHelpers.UTF8);
		streamWriter.Write(content);
	}

	public static string ReadAllText(this IWritableDirProvider provider, ResPath path)
	{
		using StreamReader streamReader = provider.OpenText(path);
		return streamReader.ReadToEnd();
	}

	public static bool TryReadAllText(this IWritableDirProvider provider, ResPath path, [NotNullWhen(true)] out string? text)
	{
		try
		{
			text = provider.ReadAllText(path);
			return true;
		}
		catch (FileNotFoundException)
		{
			text = null;
			return false;
		}
	}

	public static byte[] ReadAllBytes(this IWritableDirProvider provider, ResPath path)
	{
		using Stream stream = provider.OpenRead(path);
		using MemoryStream memoryStream = new MemoryStream((int)stream.Length);
		stream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}

	public static void WriteAllText(this IWritableDirProvider provider, ResPath path, ReadOnlySpan<char> content)
	{
		using StreamWriter streamWriter = provider.OpenWriteText(path);
		streamWriter.Write(content);
	}

	public static void WriteAllBytes(this IWritableDirProvider provider, ResPath path, ReadOnlySpan<byte> content)
	{
		using Stream stream = provider.OpenWrite(path);
		stream.Write(content);
	}
}
