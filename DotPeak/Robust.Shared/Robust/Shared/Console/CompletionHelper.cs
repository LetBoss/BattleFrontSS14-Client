// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.CompletionHelper
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Console;

public static class CompletionHelper
{
  public static IEnumerable<CompletionOption> Booleans
  {
    get
    {
      return (IEnumerable<CompletionOption>) new CompletionOption[2]
      {
        new CompletionOption(bool.FalseString),
        new CompletionOption(bool.TrueString)
      };
    }
  }

  public static IEnumerable<CompletionOption> AudioFilePath(
    string arg,
    IPrototypeManager protoManager,
    IResourceManager res)
  {
    ResPath updatedPath = CompletionHelper.GetUpdatedPath(arg);
    HashSet<string> inputs = new HashSet<string>();
    foreach (string directoryEntry in res.ContentGetDirectoryEntries(updatedPath))
      inputs.Add(directoryEntry);
    foreach (AudioMetadataPrototype enumeratePrototype in protoManager.EnumeratePrototypes<AudioMetadataPrototype>())
    {
      ResPath path = new ResPath(enumeratePrototype.ID);
      if (path.TryRelativeTo(updatedPath, out ResPath? _))
        inputs.Add(path.GetNextSegment(updatedPath).ToString());
    }
    return CompletionHelper.GetPaths(updatedPath, (IEnumerable<string>) inputs, res);
  }

  private static ResPath GetUpdatedPath(string arg)
  {
    string canonPath = arg;
    if (!canonPath.StartsWith("/"))
      canonPath = "/";
    ResPath updatedPath = new ResPath(canonPath);
    if (!canonPath.EndsWith("/"))
      updatedPath = (updatedPath / "..").Clean();
    return updatedPath;
  }

  private static IEnumerable<CompletionOption> GetPaths(
    ResPath resPath,
    IEnumerable<string> inputs,
    IResourceManager res)
  {
    return inputs.OrderBy<string, string>((Func<string, string>) (c => c)).Select<string, CompletionOption>((Func<string, CompletionOption>) (c =>
    {
      string str = (resPath / c).ToString();
      return c.EndsWith("/") ? new CompletionOption(str, Flags: CompletionOptionFlags.PartialCompletion) : new CompletionOption(str);
    }));
  }

  public static IEnumerable<CompletionOption> ContentFilePath(string arg, IResourceManager res)
  {
    ResPath updatedPath = CompletionHelper.GetUpdatedPath(arg);
    return CompletionHelper.GetPaths(updatedPath, res.ContentGetDirectoryEntries(updatedPath), res);
  }

  public static IEnumerable<CompletionOption> ContentDirPath(string arg, IResourceManager res)
  {
    string canonPath = arg;
    if (!canonPath.StartsWith("/"))
      return (IEnumerable<CompletionOption>) new CompletionOption[1]
      {
        new CompletionOption("/")
      };
    ResPath resPath = new ResPath(canonPath);
    if (!canonPath.EndsWith("/"))
    {
      resPath /= "..";
      resPath = resPath.Clean();
    }
    return res.ContentGetDirectoryEntries(resPath).Where<string>((Func<string, bool>) (c => c.EndsWith("/"))).OrderBy<string, string>((Func<string, string>) (c => c)).Select<string, CompletionOption>((Func<string, CompletionOption>) (c => new CompletionOption((resPath / c).ToString(), Flags: CompletionOptionFlags.PartialCompletion)));
  }

  public static IEnumerable<CompletionOption> UserFilePath(
    string arg,
    IWritableDirProvider provider)
  {
    string canonPath = arg;
    if (canonPath == "")
      canonPath = "/";
    ResPath resPath = new ResPath(canonPath);
    if (!resPath.IsRooted)
      return Enumerable.Empty<CompletionOption>();
    if (!canonPath.EndsWith("/"))
    {
      resPath /= "..";
      resPath = resPath.Clean();
    }
    return (IEnumerable<CompletionOption>) provider.DirectoryEntries(resPath).Select<string, CompletionOption>((Func<string, CompletionOption>) (c =>
    {
      ResPath path = resPath / c;
      if (!provider.IsDir(path))
        return new CompletionOption(path.ToString());
      return new CompletionOption($"{path}", Flags: CompletionOptionFlags.PartialCompletion);
    })).OrderBy<CompletionOption, string>((Func<CompletionOption, string>) (c => c.Value));
  }

  public static IEnumerable<CompletionOption> PrototypeIDs<T>(bool sorted = true, IPrototypeManager? proto = null) where T : class, IPrototype
  {
    IoCManager.Resolve<IPrototypeManager>(ref proto);
    IEnumerable<CompletionOption> source = proto.EnumeratePrototypes<T>().Select<T, CompletionOption>((Func<T, CompletionOption>) (p => new CompletionOption(p.ID)));
    return !sorted ? source : (IEnumerable<CompletionOption>) source.OrderBy<CompletionOption, string>((Func<CompletionOption, string>) (o => o.Value));
  }

  public static IEnumerable<CompletionOption> PrototypeIdsLimited<T>(
    string currentArgument,
    IPrototypeManager proto,
    bool sorted = true,
    int maxCount = 30)
    where T : class, IPrototype
  {
    IEnumerable<CompletionOption> source = proto.EnumeratePrototypes<T>().Where<T>((Func<T, bool>) (p => p.ID.StartsWith(currentArgument, StringComparison.OrdinalIgnoreCase))).Take<T>(maxCount).Select<T, CompletionOption>((Func<T, CompletionOption>) (p => new CompletionOption(p.ID)));
    if (sorted)
      source = (IEnumerable<CompletionOption>) source.OrderBy<CompletionOption, string>((Func<CompletionOption, string>) (o => o.Value));
    return source;
  }

  public static IEnumerable<CompletionOption> SessionNames(
    bool sorted = true,
    ISharedPlayerManager? players = null)
  {
    IoCManager.Resolve<ISharedPlayerManager>(ref players);
    IEnumerable<CompletionOption> source = ((IEnumerable<ICommonSession>) players.Sessions).Select<ICommonSession, CompletionOption>((Func<ICommonSession, CompletionOption>) (p => new CompletionOption(p.Name)));
    return !sorted ? source : (IEnumerable<CompletionOption>) source.OrderBy<CompletionOption, string>((Func<CompletionOption, string>) (o => o.Value));
  }

  public static IEnumerable<CompletionOption> MapIds(string text, IEntityManager entManager)
  {
    return CompletionHelper.GetComponents<MapComponent>(text, entManager).Select<(MapComponent, string, string), CompletionOption>((Func<(MapComponent, string, string), CompletionOption>) (o => new CompletionOption(o.Component.MapId.ToString(), o.EntityName)));
  }

  public static IEnumerable<CompletionOption> MapIds(IEntityManager? entManager = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entManager);
    return entManager.EntityQuery<MapComponent, MetaDataComponent>(true).Select<(MapComponent, MetaDataComponent), CompletionOption>((Func<(MapComponent, MetaDataComponent), CompletionOption>) (o => new CompletionOption(o.Item1.MapId.ToString(), o.Item2.EntityName)));
  }

  public static IEnumerable<CompletionOption> MapUids(IEntityManager? entManager = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entManager);
    return CompletionHelper.Components<MapComponent>(string.Empty, entManager, 128 /*0x80*/);
  }

  public static IEnumerable<CompletionOption> NetEntities(
    string text,
    IEntityManager? entManager = null,
    int limit = 20)
  {
    if (!(text != string.Empty) || NetEntity.TryParse(text.AsSpan(), out NetEntity _))
    {
      IoCManager.Resolve<IEntityManager>(ref entManager);
      AllEntityQueryEnumerator<MetaDataComponent> query = entManager.AllEntityQueryEnumerator<MetaDataComponent>();
      int i = 0;
      MetaDataComponent comp1;
      while (i < limit && query.MoveNext(out comp1))
      {
        string str = comp1.NetEntity.ToString();
        if (str.StartsWith(text))
        {
          ++i;
          yield return new CompletionOption(str, comp1.EntityName);
        }
      }
    }
  }

  private static IEnumerable<(T Component, string NetString, string EntityName)> GetComponents<T>(
    string text,
    IEntityManager entManager,
    int limit = 20)
    where T : IComponent
  {
    if (!(text != string.Empty) || NetEntity.TryParse(text.AsSpan(), out NetEntity _))
    {
      AllEntityQueryEnumerator<T, MetaDataComponent> query = entManager.AllEntityQueryEnumerator<T, MetaDataComponent>();
      int i = 0;
      T comp1;
      MetaDataComponent comp2;
      while (i < limit && query.MoveNext(out comp1, out comp2))
      {
        string str = comp2.NetEntity.ToString();
        if (str.StartsWith(text))
        {
          ++i;
          yield return (comp1, str, comp2.EntityName);
        }
      }
    }
  }

  public static IEnumerable<CompletionOption> Components<T>(
    string text,
    IEntityManager? entManager = null,
    int limit = 20)
    where T : IComponent
  {
    IoCManager.Resolve<IEntityManager>(ref entManager);
    return CompletionHelper.GetComponents<T>(text, entManager, limit).Select<(T, string, string), CompletionOption>((Func<(T, string, string), CompletionOption>) (o => new CompletionOption(o.NetString, o.EntityName)));
  }
}
