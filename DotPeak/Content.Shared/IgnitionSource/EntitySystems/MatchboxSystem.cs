// Decompiled with JetBrains decompiler
// Type: Content.Shared.IgnitionSource.EntitySystems.MatchboxSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.IgnitionSource.Components;
using Content.Shared.Interaction;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.IgnitionSource.EntitySystems;

public sealed class MatchboxSystem : EntitySystem
{
  [Dependency]
  private MatchstickSystem _match;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MatchboxComponent, InteractUsingEvent>(new EntityEventRefHandler<MatchboxComponent, InteractUsingEvent>(this.OnInteractUsing), new Type[1]
    {
      typeof (SharedStorageSystem)
    });
  }

  private void OnInteractUsing(Entity<MatchboxComponent> ent, ref InteractUsingEvent args)
  {
    MatchstickComponent comp;
    if (args.Handled || !this.TryComp<MatchstickComponent>(args.Used, out comp))
      return;
    args.Handled = this._match.TryIgnite((Entity<MatchstickComponent>) (args.Used, comp), new EntityUid?(args.User));
  }
}
