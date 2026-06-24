// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.Pamphlets.SkillPamphletSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared.Interaction.Events;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Roles.Jobs;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Marines.Skills.Pamphlets;

public sealed class SkillPamphletSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedJobSystem _job;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SquadSystem _squads;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SkillPamphletComponent, UseInHandEvent>(new EntityEventRefHandler<SkillPamphletComponent, UseInHandEvent>(this.OnUse));
    this.SubscribeLocalEvent<UsedSkillPamphletComponent, GetMarineIconEvent>(new EntityEventRefHandler<UsedSkillPamphletComponent, GetMarineIconEvent>(this.OnGetMarineIcon), after: new Type[2]
    {
      typeof (SharedMarineSystem),
      typeof (SquadSystem)
    });
    this.SubscribeLocalEvent<UsedSkillPamphletComponent, GetMarineSquadNameEvent>(new EntityEventRefHandler<UsedSkillPamphletComponent, GetMarineSquadNameEvent>(this.OnGetSquadTitle), after: new Type[1]
    {
      typeof (SquadSystem)
    });
  }

  private void OnUse(Entity<SkillPamphletComponent> ent, ref UseInHandEvent args)
  {
    args.Handled = true;
    UsedSkillPamphletComponent comp;
    if (!ent.Comp.BypassLimit && this.TryComp<UsedSkillPamphletComponent>(args.User, out comp) && comp.Used)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-pamphlets-limit-reached"), (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      foreach (SkillPamphletComponent.PamphletWhitelist whitelist in ent.Comp.Whitelists)
      {
        if (this._whitelistSystem.IsWhitelistFail(whitelist.Restrictions, args.User))
        {
          this._popup.PopupClient(this.Loc.GetString(whitelist.Popup), (EntityUid) ent, new EntityUid?(args.User));
          return;
        }
      }
      bool flag = ent.Comp.JobWhitelists.Count > 0;
      LocId? nullable1 = new LocId?();
      foreach (SkillPamphletComponent.JobWhitelist jobWhitelist in ent.Comp.JobWhitelists)
      {
        EntityUid mindId;
        if (this._mind.TryGetMind(args.User, out mindId, out MindComponent _) && this._job.MindHasJobWithId(new EntityUid?(mindId), (string) jobWhitelist.JobProto))
          flag = false;
        else
          nullable1 = new LocId?(jobWhitelist.Popup);
      }
      if (flag)
      {
        if (!nullable1.HasValue)
          return;
        SharedPopupSystem popup = this._popup;
        ILocalizationManager loc = this.Loc;
        LocId? nullable2 = nullable1;
        string valueOrDefault = nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null;
        string message = loc.GetString(valueOrDefault);
        EntityUid uid = (EntityUid) ent;
        EntityUid? recipient = new EntityUid?(args.User);
        popup.PopupClient(message, uid, recipient);
      }
      else
      {
        foreach (Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry entry in ent.Comp.AddComps.Values)
        {
          if (!this.HasComp(args.User, entry.Component.GetType()))
          {
            this.EntityManager.AddComponent(args.User, entry);
            ent.Comp.GaveSkill = true;
          }
        }
        foreach (KeyValuePair<EntProtoId<SkillDefinitionComponent>, int> addSkill in ent.Comp.AddSkills)
        {
          int num1;
          int num2 = ent.Comp.SkillCap.TryGetValue(addSkill.Key, out num1) ? num1 : addSkill.Value;
          if (!this._skills.HasSkill((Entity<SkillsComponent>) args.User, addSkill.Key, num2))
          {
            int to = Math.Min(this._skills.GetSkill((Entity<SkillsComponent>) args.User, addSkill.Key) + addSkill.Value, num2);
            this._skills.SetSkill((Entity<SkillsComponent>) args.User, addSkill.Key, to);
            ent.Comp.GaveSkill = true;
          }
        }
        if (ent.Comp.GaveSkill || ent.Comp.BypassSkill)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-pamphlets-reading"), args.User, new EntityUid?(args.User));
          UsedSkillPamphletComponent pamphletComponent = this.EnsureComp<UsedSkillPamphletComponent>(args.User);
          if (ent.Comp.GiveIcon != null)
            pamphletComponent.Icon = ent.Comp.GiveIcon;
          if (ent.Comp.GiveJobTitle.HasValue)
            pamphletComponent.JobTitle = ent.Comp.GiveJobTitle;
          if (!ent.Comp.BypassLimit)
            pamphletComponent.Used = true;
          this.Dirty(args.User, (IComponent) pamphletComponent);
          MapBlipIconOverrideComponent overrideComponent = this.EnsureComp<MapBlipIconOverrideComponent>(args.User);
          if (ent.Comp.GiveMapBlip != null)
            overrideComponent.Icon = ent.Comp.GiveMapBlip;
          this.Dirty(args.User, (IComponent) overrideComponent);
          this._squads.UpdateSquadTitle(args.User);
          if (ent.Comp.GivePrefix.HasValue)
          {
            JobPrefixComponent jobPrefixComponent = this.EnsureComp<JobPrefixComponent>(args.User);
            if (ent.Comp.IsAppendPrefix)
              jobPrefixComponent.AdditionalPrefix = new LocId?(ent.Comp.GivePrefix.Value);
            else
              jobPrefixComponent.Prefix = ent.Comp.GivePrefix.Value;
            this.Dirty(args.User, (IComponent) jobPrefixComponent);
          }
          if (this._net.IsClient)
            return;
          this.QueueDel(new EntityUid?((EntityUid) ent));
        }
        else
          this._popup.PopupClient(this.Loc.GetString("rmc-pamphlets-already-know"), (EntityUid) ent, new EntityUid?(args.User));
      }
    }
  }

  private void OnGetMarineIcon(Entity<UsedSkillPamphletComponent> ent, ref GetMarineIconEvent args)
  {
    if (this.HasComp<SquadLeaderComponent>((EntityUid) ent) || ent.Comp.Icon == null)
      return;
    args.Icon = (SpriteSpecifier) ent.Comp.Icon;
  }

  private void OnGetSquadTitle(
    Entity<UsedSkillPamphletComponent> ent,
    ref GetMarineSquadNameEvent args)
  {
    if (!ent.Comp.JobTitle.HasValue)
      return;
    ref GetMarineSquadNameEvent local = ref args;
    ILocalizationManager loc = this.Loc;
    LocId? jobTitle = ent.Comp.JobTitle;
    string valueOrDefault = jobTitle.HasValue ? (string) jobTitle.GetValueOrDefault() : (string) null;
    string str = loc.GetString(valueOrDefault);
    local.RoleName = str;
  }
}
