// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Systems.ToolRefinablSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Storage;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Random;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Tools.Systems;

public sealed class ToolRefinablSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedToolSystem _toolSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ToolRefinableComponent, InteractUsingEvent>(new ComponentEventHandler<ToolRefinableComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<ToolRefinableComponent, WelderRefineDoAfterEvent>(new ComponentEventHandler<ToolRefinableComponent, WelderRefineDoAfterEvent>(this.OnDoAfter));
  }

  private void OnInteractUsing(
    EntityUid uid,
    ToolRefinableComponent component,
    InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this._toolSystem.UseTool(args.Used, args.User, new EntityUid?(uid), component.RefineTime, (string) component.QualityNeeded, (DoAfterEvent) new WelderRefineDoAfterEvent(), component.RefineFuel);
  }

  private void OnDoAfter(
    EntityUid uid,
    ToolRefinableComponent component,
    WelderRefineDoAfterEvent args)
  {
    if (args.Cancelled || this._net.IsClient)
      return;
    TransformComponent xform = this.Transform(uid);
    foreach (string spawn in EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) component.RefineResult, this._random))
      this.SpawnNextToOrDrop(spawn, uid, xform);
    this.Del(new EntityUid?(uid));
  }
}
