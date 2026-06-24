// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Dogtags.DogtagsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Medical.Defibrillator;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Access.Components;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Marines.Dogtags;

public sealed class DogtagsSystem : EntitySystem
{
  [Dependency]
  private SharedRottingSystem _rotting;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private MetaDataSystem _meta;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private RMCUnrevivableSystem _unrevivableSystem;
  private readonly EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillPolice";
  private readonly int SkillRequired = 2;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TakeableTagsComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>(new EntityEventRefHandler<TakeableTagsComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>(this.GetRelayedTags));
    this.SubscribeLocalEvent<TakeableTagsComponent, GetVerbsEvent<EquipmentVerb>>(new EntityEventRefHandler<TakeableTagsComponent, GetVerbsEvent<EquipmentVerb>>(this.OnGetVerbTags));
    this.SubscribeLocalEvent<TakeableTagsComponent, ExaminedEvent>(new EntityEventRefHandler<TakeableTagsComponent, ExaminedEvent>(this.OnTagsExamine));
    this.SubscribeLocalEvent<InformationTagsComponent, ExaminedEvent>(new EntityEventRefHandler<InformationTagsComponent, ExaminedEvent>(this.OnInfoTagsExamine));
    this.SubscribeLocalEvent<InformationTagsComponent, AfterInteractEvent>(new EntityEventRefHandler<InformationTagsComponent, AfterInteractEvent>(this.OnInfoTagsUse));
    this.SubscribeLocalEvent<RMCMemorialComponent, ExaminedEvent>(new EntityEventRefHandler<RMCMemorialComponent, ExaminedEvent>(this.OnMemorialExamined));
  }

  private void OnTagsExamine(Entity<TakeableTagsComponent> tags, ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner))
      return;
    string name;
    string job;
    string bloodtype;
    this.GetTagInformation((EntityUid) tags, out name, out job, out bloodtype);
    args.PushMarkup(this.Loc.GetString("rmc-dogtags-read", ("name", (object) name), ("assignment", (object) job), ("bloodtype", (object) bloodtype)));
  }

  private void OnInfoTagsExamine(Entity<InformationTagsComponent> tags, ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.PushMarkup(this.Loc.GetString("rmc-dogtags-info-read-start", (nameof (tags), (object) tags.Comp.Tags.Count)), -19);
    int num = 1;
    foreach (InfoTagInfo tag in tags.Comp.Tags)
      args.PushMarkup(this.Loc.GetString("rmc-dogtags-info-read", ("number", (object) num++), ("name", (object) tag.Name), ("assignment", (object) tag.Assignment), ("bloodtype", (object) tag.BloodType)), -19 - num);
  }

  private void OnMemorialExamined(Entity<RMCMemorialComponent> memorial, ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner) || memorial.Comp.Names.Count == 0)
      return;
    string markup = $"{this.Loc.GetString("rmc-memorial-start")} {this.MemorialNamesFormat(memorial.Comp.Names)}";
    args.PushMarkup(markup, -5);
  }

  public string MemorialNamesFormat(List<string> memorialnames)
  {
    string str = "";
    int num = 1;
    foreach (string memorialname in memorialnames)
    {
      str = num != memorialnames.Count ? $"{str}{memorialname}, " : $"{str}{memorialname}.";
      ++num;
    }
    return str;
  }

  private void GetRelayedTags(
    Entity<TakeableTagsComponent> tags,
    ref InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>> args)
  {
    this.OnGetVerbTags(tags, ref args.Args);
  }

  private bool CanTakeTags(
    Entity<TakeableTagsComponent> tags,
    EntityUid wearer,
    EntityUid taker,
    out bool equipped,
    out string reason)
  {
    equipped = true;
    reason = "";
    if (wearer == taker)
    {
      if (!this._hands.IsHolding((Entity<HandsComponent>) taker, new EntityUid?((EntityUid) tags)))
        return false;
      IdCardOwnerComponent comp;
      if (this.TryComp<IdCardOwnerComponent>((EntityUid) tags, out comp) && this.Exists(comp.Id))
      {
        reason = !(comp.Id == taker) ? this.Loc.GetString("rmc-dogtags-still-exists") : this.Loc.GetString("rmc-dogtags-still-exists-self");
        return false;
      }
      equipped = false;
      return true;
    }
    if (!this._mob.IsDead(wearer))
    {
      reason = this.Loc.GetString("rmc-dogtags-still-alive");
      return false;
    }
    if (this._rotting.IsRotten(wearer) || this._unrevivableSystem.IsUnrevivable(wearer) || this.HasComp<RMCDefibrillatorBlockedComponent>(wearer) || this._skills.HasSkill((Entity<SkillsComponent>) taker, this.Skill, this.SkillRequired))
      return true;
    reason = this.Loc.GetString("rmc-dogtags-can-be-saved");
    return false;
  }

  private void OnGetVerbTags(
    Entity<TakeableTagsComponent> tags,
    ref GetVerbsEvent<EquipmentVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess || args.Hands == null || tags.Comp.TagsTaken || this.HasComp<XenoComponent>(args.User))
      return;
    EntityUid wearer = this.Transform((EntityUid) tags).ParentUid;
    EntityUid user = args.User;
    if (!this.CanTakeTags(tags, wearer, user, out bool _, out string _))
      return;
    EquipmentVerb equipmentVerb1 = new EquipmentVerb();
    equipmentVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/_RMC14/Interface/VerbIcons/dogtag.png"));
    equipmentVerb1.Text = this.Loc.GetString("rmc-dogtags-take");
    EquipmentVerb equipmentVerb2 = equipmentVerb1;
    equipmentVerb2.Act = (Action) (() => this.TakeTags(tags, user, wearer));
    args.Verbs.Add(equipmentVerb2);
  }

  private void TakeTags(Entity<TakeableTagsComponent> tags, EntityUid user, EntityUid wearer)
  {
    if (tags.Comp.TagsTaken)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-dogtags-already-taken", ("target", (object) wearer)), new EntityUid?(user));
    }
    else
    {
      bool equipped;
      string reason;
      if (!this.CanTakeTags(tags, wearer, user, out equipped, out reason))
      {
        this._popup.PopupClient(reason, new EntityUid?(user));
      }
      else
      {
        if (!this._interaction.InRangeAndAccessible((Entity<TransformComponent>) user, (Entity<TransformComponent>) wearer))
          return;
        tags.Comp.TagsTaken = true;
        this._appearance.SetData((EntityUid) tags, (Enum) DogtagVisuals.Taken, (object) true);
        if (this._net.IsClient)
          return;
        if (!equipped)
        {
          EntityUid target = this.SpawnAtPosition((string) tags.Comp.FallenTag, user.ToCoordinates());
          IdCardComponent comp;
          if (this.TryComp<IdCardComponent>((EntityUid) tags, out comp))
          {
            this.CopyComp<TakeableTagsComponent>(tags.Owner, target, tags.Comp);
            this.CopyComp<IdCardComponent>(tags.Owner, target, comp);
          }
          this.QueueDel(new EntityUid?((EntityUid) tags));
        }
        EntityUid orDrop = this.SpawnNextToOrDrop((string) tags.Comp.InfoTag, wearer);
        this.Dirty<TakeableTagsComponent>(tags);
        InformationTagsComponent informationTagsComponent = this.EnsureComp<InformationTagsComponent>(orDrop);
        string name;
        string job;
        string bloodtype;
        this.GetTagInformation((EntityUid) tags, out name, out job, out bloodtype);
        InfoTagInfo infoTagInfo = new InfoTagInfo()
        {
          Name = name,
          Assignment = job,
          BloodType = bloodtype
        };
        informationTagsComponent.Tags.Add(infoTagInfo);
        this._hands.TryPickupAnyHand(user, orDrop);
      }
    }
  }

  private void OnInfoTagsUse(Entity<InformationTagsComponent> tags, ref AfterInteractEvent args)
  {
    if (!args.Target.HasValue)
      return;
    InformationTagsComponent comp1;
    if (this.TryComp<InformationTagsComponent>(args.Target, out comp1))
    {
      args.Handled = true;
      this._meta.SetEntityName(args.Target.Value, this.Loc.GetString("rmc-dogtags-info-joined-name"));
      this._meta.SetEntityDescription(args.Target.Value, this.Loc.GetString("rmc-dogtags-info-joined-desc"));
      if (this._net.IsClient)
        return;
      this._popup.PopupEntity(this.Loc.GetString(tags.Comp.Tags.Count != 1 || comp1.Tags.Count != 1 ? "rmc-dogtags-join" : "rmc-dogtags-single-join"), args.User, args.User);
      comp1.Tags.AddRange((IEnumerable<InfoTagInfo>) tags.Comp.Tags);
      this._appearance.SetData(args.Target.Value, (Enum) InfoTagVisuals.Number, (object) Math.Min(comp1.Tags.Count, comp1.MaxDisplayTags));
      this.QueueDel(new EntityUid?((EntityUid) tags));
    }
    else
    {
      RMCMemorialComponent comp2;
      if (!this.TryComp<RMCMemorialComponent>(args.Target, out comp2))
        return;
      args.Handled = true;
      if (this._net.IsClient)
        return;
      this._popup.PopupEntity(this.Loc.GetString("rmc-memorial-add", (nameof (tags), (object) tags), ("slab", (object) args.Target.Value)), args.User, args.User);
      foreach (InfoTagInfo tag in tags.Comp.Tags)
        comp2.Names.Add(tag.Name);
      this.QueueDel(new EntityUid?((EntityUid) tags));
    }
  }

  private void GetTagInformation(
    EntityUid dogtag,
    out string name,
    out string job,
    out string bloodtype)
  {
    name = this.Loc.GetString("rmc-dogtags-unknown");
    job = this.Loc.GetString("rmc-dogtags-unknown");
    bloodtype = "O-";
    IdCardComponent comp;
    if (!this.TryComp<IdCardComponent>(dogtag, out comp))
      return;
    if (comp.FullName != null)
      name = comp.FullName;
    if (comp.LocalizedJobTitle == null)
      return;
    job = comp.LocalizedJobTitle;
  }
}
