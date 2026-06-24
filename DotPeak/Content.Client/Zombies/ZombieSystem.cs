// Decompiled with JetBrains decompiler
// Type: Content.Client.Zombies.ZombieSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Humanoid;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Zombies;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.Zombies;

public sealed class ZombieSystem : SharedZombieSystem
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ZombieComponent, ComponentStartup>(new ComponentEventHandler<ZombieComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ZombieComponent, GetStatusIconsEvent>(new EntityEventRefHandler<ZombieComponent, GetStatusIconsEvent>((object) this, __methodptr(GetZombieIcon)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InitialInfectedComponent, GetStatusIconsEvent>(new EntityEventRefHandler<InitialInfectedComponent, GetStatusIconsEvent>((object) this, __methodptr(GetInitialInfectedIcon)), (Type[]) null, (Type[]) null);
  }

  private void GetZombieIcon(Entity<ZombieComponent> ent, ref GetStatusIconsEvent args)
  {
    FactionIconPrototype factionIconPrototype = this._prototype.Index<FactionIconPrototype>(ent.Comp.StatusIcon);
    args.StatusIcons.Add((StatusIconData) factionIconPrototype);
  }

  private void GetInitialInfectedIcon(
    Entity<InitialInfectedComponent> ent,
    ref GetStatusIconsEvent args)
  {
    if (this.HasComp<ZombieComponent>(Entity<InitialInfectedComponent>.op_Implicit(ent)))
      return;
    FactionIconPrototype factionIconPrototype = this._prototype.Index<FactionIconPrototype>(ent.Comp.StatusIcon);
    args.StatusIcons.Add((StatusIconData) factionIconPrototype);
  }

  private void OnStartup(EntityUid uid, ZombieComponent component, ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (this.HasComp<HumanoidAppearanceComponent>(uid) || !this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    for (int index = 0; index < spriteComponent.AllLayers.Count<ISpriteLayer>(); ++index)
      this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), index, component.SkinColor);
  }
}
