// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.NearbyCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class NearbyCommand : ToolshedCommand
{
  [Dependency]
  private readonly IConfigurationManager _cfg;
  private EntityLookupSystem? _lookup;

  [CommandImplementation(null)]
  public IEnumerable<EntityUid> Nearby([PipedArgument] IEnumerable<EntityUid> input, float range)
  {
    int cvar = this._cfg.GetCVar<int>(CVars.ToolshedNearbyLimit);
    if ((double) range > (double) cvar)
      throw new ArgumentException($"Tried to query too big of a range with nearby ({range})! Limit: {cvar}. Change the {CVars.ToolshedNearbyLimit.Name} cvar to increase this at your own risk.");
    if (this._lookup == null)
      this._lookup = this.GetSys<EntityLookupSystem>();
    int i = 0;
    int entitiesLimit = this._cfg.GetCVar<int>(CVars.ToolshedNearbyEntitiesLimit);
    return input.SelectMany<EntityUid, EntityUid>((Func<EntityUid, IEnumerable<EntityUid>>) (x =>
    {
      if (i++ > entitiesLimit)
        throw new ArgumentException($"Too many entities were passed to nearby ({i})! Limit: {entitiesLimit}. Change the {CVars.ToolshedNearbyEntitiesLimit.Name} cvar to increase this at your own risk.");
      return (IEnumerable<EntityUid>) this._lookup.GetEntitiesInRange(x, range);
    })).Distinct<EntityUid>();
  }
}
