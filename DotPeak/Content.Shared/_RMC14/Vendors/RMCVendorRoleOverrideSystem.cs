// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.VendorRoleOverrideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Squads;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vendors;

public sealed class VendorRoleOverrideSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCVendorRoleOverrideComponent, GetMarineIconEvent>(new EntityEventRefHandler<RMCVendorRoleOverrideComponent, GetMarineIconEvent>(this.OnGetMarineIcon), after: new Type[2]
    {
      typeof (SharedMarineSystem),
      typeof (SquadSystem)
    });
    this.SubscribeLocalEvent<RMCVendorRoleOverrideComponent, GetMarineSquadNameEvent>(new EntityEventRefHandler<RMCVendorRoleOverrideComponent, GetMarineSquadNameEvent>(this.OnGetSquadTitle), after: new Type[1]
    {
      typeof (SquadSystem)
    });
  }

  private void OnGetMarineIcon(
    Entity<RMCVendorRoleOverrideComponent> ent,
    ref GetMarineIconEvent args)
  {
    if (this.HasComp<SquadLeaderComponent>((EntityUid) ent) || ent.Comp.GiveIcon == null)
      return;
    args.Icon = (SpriteSpecifier) ent.Comp.GiveIcon;
  }

  private void OnGetSquadTitle(
    Entity<RMCVendorRoleOverrideComponent> ent,
    ref GetMarineSquadNameEvent args)
  {
    if (!ent.Comp.GiveSquadRoleName.HasValue)
      return;
    if (ent.Comp.IsAppendSquadRoleName)
    {
      ref GetMarineSquadNameEvent local = ref args;
      string roleName = args.RoleName;
      ILocalizationManager loc = this.Loc;
      LocId? giveSquadRoleName = ent.Comp.GiveSquadRoleName;
      string valueOrDefault = giveSquadRoleName.HasValue ? (string) giveSquadRoleName.GetValueOrDefault() : (string) null;
      string str1 = loc.GetString(valueOrDefault);
      string str2 = $"{roleName} {str1}";
      local.RoleName = str2;
    }
    else
    {
      ref GetMarineSquadNameEvent local = ref args;
      ILocalizationManager loc = this.Loc;
      LocId? giveSquadRoleName = ent.Comp.GiveSquadRoleName;
      string valueOrDefault = giveSquadRoleName.HasValue ? (string) giveSquadRoleName.GetValueOrDefault() : (string) null;
      string str = loc.GetString(valueOrDefault);
      local.RoleName = str;
    }
  }
}
