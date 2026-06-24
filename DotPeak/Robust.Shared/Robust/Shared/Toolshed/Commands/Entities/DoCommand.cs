// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.DoCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Toolshed.Invocation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class DoCommand : ToolshedCommand
{
  private SharedTransformSystem? _xformSys;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Do<T>(IInvocationContext ctx, [PipedArgument] IEnumerable<T> input, string command)
  {
    DoCommand doCommand = this;
    OldShellInvocationContext reqCtx = ctx as OldShellInvocationContext;
    if (reqCtx == null || reqCtx.Shell == null)
      throw new NotImplementedException("do can only be executed in a shell invocation context. Some commands like emplace provide their own context.");
    if (doCommand._xformSys == null)
      doCommand._xformSys = doCommand.GetSys<SharedTransformSystem>();
    EntityQuery<TransformComponent> xformQ = doCommand.GetEntityQuery<TransformComponent>();
    IConsoleShell shell = reqCtx.Shell;
    foreach (T obj in input)
    {
      string str1 = command;
      if (obj is EntityUid uid)
      {
        Vector2 worldPosition = doCommand._xformSys.GetWorldPosition(uid, xformQ);
        EntityCoordinates coordinates = xformQ.GetComponent(uid).Coordinates;
        string str2 = str1.Replace("$ID", uid.ToString()).Replace("$PID", ((EntityUid?) reqCtx.Session?.AttachedEntity ?? EntityUid.Invalid).ToString()).Replace("$WX", worldPosition.X.ToString((IFormatProvider) CultureInfo.InvariantCulture)).Replace("$WY", worldPosition.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        float num = coordinates.X;
        string newValue1 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        string str3 = str2.Replace("$LX", newValue1);
        num = coordinates.Y;
        string newValue2 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        str1 = str3.Replace("$LY", newValue2);
      }
      shell.ExecuteCommand(str1.Replace("$SELF", obj.ToString() ?? ""));
      yield return obj;
    }
  }
}
