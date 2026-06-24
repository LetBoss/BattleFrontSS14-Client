// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Pointing.RMCPointingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Sprite;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Pointing;

public sealed class RMCPointingSystem : EntitySystem
{
  [Dependency]
  private SharedRMCSpriteSystem _rmcSprite;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCPointingComponent, RMCSpawnPointingArrowEvent>(new EntityEventRefHandler<RMCPointingComponent, RMCSpawnPointingArrowEvent>(this.OnGetPointingArrow));
  }

  private void OnGetPointingArrow(
    Entity<RMCPointingComponent> ent,
    ref RMCSpawnPointingArrowEvent ev)
  {
    SquadMemberComponent comp;
    if (!this.TryComp<SquadMemberComponent>((EntityUid) ent, out comp))
    {
      ev.Spawned = new EntityUid?(this.Spawn((string) ent.Comp.Arrow, ev.Coordinates));
    }
    else
    {
      ev.Spawned = new EntityUid?(this.Spawn((string) ent.Comp.SquadArrow, ev.Coordinates));
      this._rmcSprite.SetColor((Entity<SpriteColorComponent>) ev.Spawned.Value, comp.BackgroundColor);
    }
  }
}
