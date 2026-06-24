using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Audio;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Robust.Shared.Console;

public static class CompletionHelper
{
	public static IEnumerable<CompletionOption> Booleans => new CompletionOption[2]
	{
		new CompletionOption(bool.FalseString),
		new CompletionOption(bool.TrueString)
	};

	public static IEnumerable<CompletionOption> AudioFilePath(string arg, IPrototypeManager protoManager, IResourceManager res)
	{
		ResPath updatedPath = GetUpdatedPath(arg);
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string item in res.ContentGetDirectoryEntries(updatedPath))
		{
			hashSet.Add(item);
		}
		foreach (AudioMetadataPrototype item2 in protoManager.EnumeratePrototypes<AudioMetadataPrototype>())
		{
			ResPath path = new ResPath(item2.ID);
			if (path.TryRelativeTo(updatedPath, out var _))
			{
				hashSet.Add(path.GetNextSegment(updatedPath).ToString());
			}
		}
		return GetPaths(updatedPath, hashSet, res);
	}

	private static ResPath GetUpdatedPath(string arg)
	{
		string text = arg;
		if (!text.StartsWith("/"))
		{
			text = "/";
		}
		ResPath resPath = new ResPath(text);
		if (!text.EndsWith("/"))
		{
			resPath /= "..";
			return resPath.Clean();
		}
		return resPath;
	}

	private static IEnumerable<CompletionOption> GetPaths(ResPath resPath, IEnumerable<string> inputs, IResourceManager res)
	{
		return inputs.OrderBy((string c) => c).Select(delegate(string c)
		{
			string value = (resPath / c).ToString();
			return c.EndsWith("/") ? new CompletionOption(value, null, CompletionOptionFlags.PartialCompletion) : new CompletionOption(value);
		});
	}

	public static IEnumerable<CompletionOption> ContentFilePath(string arg, IResourceManager res)
	{
		ResPath updatedPath = GetUpdatedPath(arg);
		return GetPaths(updatedPath, res.ContentGetDirectoryEntries(updatedPath), res);
	}

	public static IEnumerable<CompletionOption> ContentDirPath(string arg, IResourceManager res)
	{
		if (!arg.StartsWith("/"))
		{
			return new CompletionOption[1]
			{
				new CompletionOption("/")
			};
		}
		ResPath resPath = new ResPath(arg);
		if (!arg.EndsWith("/"))
		{
			resPath /= "..";
			resPath = resPath.Clean();
		}
		return from c in res.ContentGetDirectoryEntries(resPath)
			where c.EndsWith("/")
			orderby c
			select new CompletionOption((resPath / c).ToString(), null, CompletionOptionFlags.PartialCompletion);
	}

	public static IEnumerable<CompletionOption> UserFilePath(string arg, IWritableDirProvider provider)
	{
		string text = arg;
		if (text == "")
		{
			text = "/";
		}
		ResPath resPath = new ResPath(text);
		if (!resPath.IsRooted)
		{
			return Enumerable.Empty<CompletionOption>();
		}
		if (!text.EndsWith("/"))
		{
			resPath /= "..";
			resPath = resPath.Clean();
		}
		return from c in provider.DirectoryEntries(resPath).Select(delegate(string c)
			{
				ResPath resPath2 = resPath / c;
				return provider.IsDir(resPath2) ? new CompletionOption($"{resPath2}", null, CompletionOptionFlags.PartialCompletion) : new CompletionOption(resPath2.ToString());
			})
			orderby c.Value
			select c;
	}

	public static IEnumerable<CompletionOption> PrototypeIDs<T>(bool sorted = true, IPrototypeManager? proto = null) where T : class, IPrototype
	{
		IoCManager.Resolve(ref proto);
		IEnumerable<CompletionOption> enumerable = from p in proto.EnumeratePrototypes<T>()
			select new CompletionOption(p.ID);
		if (!sorted)
		{
			return enumerable;
		}
		return enumerable.OrderBy((CompletionOption o) => o.Value);
	}

	public static IEnumerable<CompletionOption> PrototypeIdsLimited<T>(string currentArgument, IPrototypeManager proto, bool sorted = true, int maxCount = 30) where T : class, IPrototype
	{
		IEnumerable<CompletionOption> enumerable = from p in (from p in proto.EnumeratePrototypes<T>()
				where p.ID.StartsWith(currentArgument, StringComparison.OrdinalIgnoreCase)
				select p).Take(maxCount)
			select new CompletionOption(p.ID);
		if (sorted)
		{
			enumerable = enumerable.OrderBy((CompletionOption o) => o.Value);
		}
		return enumerable;
	}

	public static IEnumerable<CompletionOption> SessionNames(bool sorted = true, ISharedPlayerManager? players = null)
	{
		IoCManager.Resolve(ref players);
		IEnumerable<CompletionOption> enumerable = players.Sessions.Select((ICommonSession p) => new CompletionOption(p.Name));
		if (!sorted)
		{
			return enumerable;
		}
		return enumerable.OrderBy((CompletionOption o) => o.Value);
	}

	public static IEnumerable<CompletionOption> MapIds(string text, IEntityManager entManager)
	{
		return from o in GetComponents<MapComponent>(text, entManager)
			select new CompletionOption(o.Component.MapId.ToString(), o.EntityName);
	}

	public static IEnumerable<CompletionOption> MapIds(IEntityManager? entManager = null)
	{
		IoCManager.Resolve(ref entManager);
		return from o in entManager.EntityQuery<MapComponent, MetaDataComponent>(includePaused: true)
			select new CompletionOption(o.Item1.MapId.ToString(), o.Item2.EntityName);
	}

	public static IEnumerable<CompletionOption> MapUids(IEntityManager? entManager = null)
	{
		IoCManager.Resolve(ref entManager);
		return Components<MapComponent>(string.Empty, entManager, 128);
	}

	public static IEnumerable<CompletionOption> NetEntities(string text, IEntityManager? entManager = null, int limit = 20)
	{
		if (text != string.Empty && !NetEntity.TryParse(text.AsSpan(), out var _))
		{
			yield break;
		}
		IoCManager.Resolve(ref entManager);
		AllEntityQueryEnumerator<MetaDataComponent> query = entManager.AllEntityQueryEnumerator<MetaDataComponent>();
		int i = 0;
		MetaDataComponent comp;
		while (i < limit && query.MoveNext(out comp))
		{
			string text2 = comp.NetEntity.ToString();
			if (text2.StartsWith(text))
			{
				i++;
				yield return new CompletionOption(text2, comp.EntityName);
			}
		}
	}

	private static IEnumerable<(T Component, string NetString, string EntityName)> GetComponents<T>(string text, IEntityManager entManager, int limit = 20) where T : IComponent
	{
		if (text != string.Empty && !NetEntity.TryParse(text.AsSpan(), out var _))
		{
			yield break;
		}
		AllEntityQueryEnumerator<T, MetaDataComponent> query = entManager.AllEntityQueryEnumerator<T, MetaDataComponent>();
		int i = 0;
		T comp;
		MetaDataComponent comp2;
		while (i < limit && query.MoveNext(out comp, out comp2))
		{
			string text2 = comp2.NetEntity.ToString();
			if (text2.StartsWith(text))
			{
				i++;
				yield return (Component: comp, NetString: text2, EntityName: comp2.EntityName);
			}
		}
	}

	public static IEnumerable<CompletionOption> Components<T>(string text, IEntityManager? entManager = null, int limit = 20) where T : IComponent
	{
		IoCManager.Resolve(ref entManager);
		return from o in GetComponents<T>(text, entManager, limit)
			select new CompletionOption(o.NetString, o.EntityName);
	}
}
