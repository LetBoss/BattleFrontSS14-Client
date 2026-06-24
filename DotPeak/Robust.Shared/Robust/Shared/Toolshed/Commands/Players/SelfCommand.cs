// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Players.SelfCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Toolshed.Errors;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Players;

[ToolshedCommand]
internal sealed class SelfCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public EntityUid Self(IInvocationContext ctx)
  {
    if (ctx.Session == null)
    {
      ctx.ReportError((IConError) new NotForServerConsoleError());
      return new EntityUid();
    }
    EntityUid? attachedEntity = ctx.Session.AttachedEntity;
    if (attachedEntity.HasValue)
      return attachedEntity.GetValueOrDefault();
    ctx.ReportError((IConError) new SessionHasNoEntityError(ctx.Session));
    return new EntityUid();
  }
}
