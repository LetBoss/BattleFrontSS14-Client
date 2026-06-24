// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.ListGridsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class ListGridsCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly SharedTransformSystem _transformSystem;

  public override string Command => "lsgrid";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    EntityQuery<TransformComponent> entityQuery = this.EntityManager.GetEntityQuery<TransformComponent>();
    List<(EntityUid Uid, MapGridComponent Component)> tupleList = this.EntityManager.AllComponentsList<MapGridComponent>();
    tupleList.Sort((Comparison<(EntityUid, MapGridComponent)>) ((x, y) => x.Uid.CompareTo(y.Uid)));
    foreach ((EntityUid entityUid, MapGridComponent _) in tupleList)
    {
      TransformComponent component = entityQuery.GetComponent(entityUid);
      Vector2 worldPosition = this._transformSystem.GetWorldPosition(component);
      StringBuilder stringBuilder2 = stringBuilder1;
      \u003C\u003Ey__InlineArray5<object> buffer = new \u003C\u003Ey__InlineArray5<object>();
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 0) = (object) entityUid;
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 1) = (object) component.MapID;
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 2) = (object) entityUid;
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 3) = (object) worldPosition.X;
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray5<object>, object>(ref buffer, 4) = (object) worldPosition.Y;
      // ISSUE: reference to a compiler-generated method
      ReadOnlySpan<object> args1 = \u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray5<object>, object>(in buffer, 5);
      stringBuilder2.AppendFormat("{0}: map: {1}, ent: {2}, pos: {3:0.0},{4:0.0} \n", args1);
    }
    IConsoleShell consoleShell = shell;
    string str = stringBuilder1.ToString();
    string text = str.Substring(0, str.Length - 1);
    consoleShell.WriteLine(text);
  }
}
