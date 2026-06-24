// Decompiled with JetBrains decompiler
// Type: Content.Client.Revolutionary.RevolutionarySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Revolutionary;
using Content.Shared.Revolutionary.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Revolutionary;

public sealed class RevolutionarySystem : SharedRevolutionarySystem
{
  [Dependency]
  private IPrototypeManager _prototype;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RevolutionaryComponent, GetStatusIconsEvent>(new EntityEventRefHandler<RevolutionaryComponent, GetStatusIconsEvent>((object) this, __methodptr(GetRevIcon)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HeadRevolutionaryComponent, GetStatusIconsEvent>(new EntityEventRefHandler<HeadRevolutionaryComponent, GetStatusIconsEvent>((object) this, __methodptr(GetHeadRevIcon)), (Type[]) null, (Type[]) null);
  }

  private void GetRevIcon(Entity<RevolutionaryComponent> ent, ref GetStatusIconsEvent args)
  {
    FactionIconPrototype factionIconPrototype;
    if (this.HasComp<HeadRevolutionaryComponent>(Entity<RevolutionaryComponent>.op_Implicit(ent)) || !this._prototype.TryIndex<FactionIconPrototype>(ent.Comp.StatusIcon, ref factionIconPrototype))
      return;
    args.StatusIcons.Add((StatusIconData) factionIconPrototype);
  }

  private void GetHeadRevIcon(Entity<HeadRevolutionaryComponent> ent, ref GetStatusIconsEvent args)
  {
    FactionIconPrototype factionIconPrototype;
    if (!this._prototype.TryIndex<FactionIconPrototype>(ent.Comp.StatusIcon, ref factionIconPrototype))
      return;
    args.StatusIcons.Add((StatusIconData) factionIconPrototype);
  }
}
