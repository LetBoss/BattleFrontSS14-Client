// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Reagent.PrototypeManagerExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Reagent;

public static class PrototypeManagerExtensions
{
  [Obsolete("Use ReagentSystem")]
  public static Content.Shared._RMC14.Chemistry.Reagent.Reagent IndexReagent(
    this IPrototypeManager prototypes,
    ProtoId<ReagentPrototype> id)
  {
    return IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().Index(id);
  }

  [Obsolete("Use ReagentSystem")]
  public static Content.Shared._RMC14.Chemistry.Reagent.Reagent IndexReagent<T>(
    this IPrototypeManager prototypes,
    ProtoId<ReagentPrototype> id)
  {
    return IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().Index(id);
  }

  [Obsolete("Use ReagentSystem")]
  public static bool TryIndexReagent(
    this IPrototypeManager prototypes,
    ProtoId<ReagentPrototype> id,
    [NotNullWhen(true)] out ReagentPrototype? reagentProto)
  {
    Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
    if (!IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().TryIndex(id, out reagent))
    {
      reagentProto = (ReagentPrototype) null;
      return false;
    }
    reagentProto = (ReagentPrototype) reagent;
    return true;
  }

  [Obsolete("Use ReagentSystem")]
  public static bool TryIndexReagent<T>(
    this IPrototypeManager prototypes,
    ProtoId<ReagentPrototype> id,
    [NotNullWhen(true)] out ReagentPrototype? reagentProto)
  {
    Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
    if (!IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().TryIndex(id, out reagent))
    {
      reagentProto = (ReagentPrototype) null;
      return false;
    }
    reagentProto = (ReagentPrototype) reagent;
    return true;
  }
}
