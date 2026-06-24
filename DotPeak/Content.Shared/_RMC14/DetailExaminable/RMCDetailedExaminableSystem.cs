// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.DetailExaminable.RMCDetailedExaminableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DetailExaminable;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.DetailExaminable;

public sealed class RMCDetailedExaminableSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  private readonly List<Entity<DetailExaminableComponent>> _queue = new List<Entity<DetailExaminableComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart));
    this.SubscribeLocalEvent<DetailExaminableComponent, MapInitEvent>(new EntityEventRefHandler<DetailExaminableComponent, MapInitEvent>(this.OnDetailExaminableMapInit));
  }

  private void OnRoundRestart(RoundRestartCleanupEvent ev) => this._queue.Clear();

  private void OnDetailExaminableMapInit(
    Entity<DetailExaminableComponent> ent,
    ref MapInitEvent args)
  {
    this._queue.Add(ent);
  }

  public override void Update(float frameTime)
  {
    try
    {
      foreach (Entity<DetailExaminableComponent> entity in this._queue)
      {
        ISharedAdminLogManager adminLog = this._adminLog;
        LogStringHandler logStringHandler = new LogStringHandler(36, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) entity)), "player", "ToPrettyString(ent)");
        logStringHandler.AppendLiteral(" had a character description added:\n");
        logStringHandler.AppendFormatted(entity.Comp.Content, format: "description");
        ref LogStringHandler local = ref logStringHandler;
        adminLog.Add(LogType.RMCCharacterDescription, ref local);
      }
    }
    finally
    {
      this._queue.Clear();
    }
  }
}
