// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.ListMapsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class ListMapsCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly IEntityManager _entManager;
  [Dependency]
  private readonly IMapManager _map;
  [Dependency]
  private readonly SharedMapSystem _mapSystem;

  public override string Command => "lsmap";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    foreach (MapId mapId in (IEnumerable<MapId>) this._mapSystem.GetAllMapIds().OrderBy<MapId, int>((Func<MapId, int>) (id => id.Value)))
    {
      EntityUid? uid;
      if (this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        \u003C\u003Ey__InlineArray6<object> buffer = new \u003C\u003Ey__InlineArray6<object>();
        // ISSUE: reference to a compiler-generated method
        \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray6<object>, object>(ref buffer, 0) = (object) mapId;
        // ISSUE: reference to a compiler-generated method
        \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray6<object>, object>(ref buffer, 1) = (object) this._entManager.GetComponent<MetaDataComponent>(uid.Value).EntityName;
        // ISSUE: reference to a compiler-generated method
        \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray6<object>, object>(ref buffer, 2) = (object) this._mapSystem.IsInitialized(uid);
        // ISSUE: reference to a compiler-generated method
        \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray6<object>, object>(ref buffer, 3) = (object) this._mapSystem.IsPaused(mapId);
        // ISSUE: reference to a compiler-generated method
        \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray6<object>, object>(ref buffer, 4) = (object) this._entManager.GetNetEntity(uid);
        // ISSUE: reference to a compiler-generated method
        \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray6<object>, object>(ref buffer, 5) = (object) string.Join<EntityUid>(",", this._map.GetAllGrids(mapId).Select<Entity<MapGridComponent>, EntityUid>((Func<Entity<MapGridComponent>, EntityUid>) (grid => grid.Owner)));
        // ISSUE: reference to a compiler-generated method
        ReadOnlySpan<object> args1 = \u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray6<object>, object>(in buffer, 6);
        stringBuilder2.AppendFormat("{0}: {1}, init: {2}, paused: {3}, nent: {4}, grids: {5}\n", args1);
      }
    }
    IConsoleShell consoleShell = shell;
    string str = stringBuilder1.ToString();
    string text = str.Substring(0, str.Length - 1);
    consoleShell.WriteLine(text);
  }
}
