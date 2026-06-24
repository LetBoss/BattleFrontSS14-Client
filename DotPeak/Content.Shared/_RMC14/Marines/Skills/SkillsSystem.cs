// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.SkillsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Flash;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Content.Shared.Throwing;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Skills;

public sealed class SkillsSystem : EntitySystem
{
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainerSystem;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private RMCReagentSystem _reagent;
  private static readonly EntProtoId<SkillDefinitionComponent> MeleeSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMeleeWeapons";
  private Robust.Shared.GameObjects.EntityQuery<SkillsComponent> _skillsQuery;
  private SortedSet<(string, int)> _skillsSorted = new SortedSet<(string, int)>((IComparer<(string, int)>) Comparer<(string, int)>.Create((Comparison<(string, int)>) ((a, b) => string.Compare(a.Item1, b.Item1, StringComparison.Ordinal))));

  public ImmutableArray<EntProtoId<SkillDefinitionComponent>> Skills { get; private set; }

  public ImmutableDictionary<string, EntProtoId<SkillDefinitionComponent>> SkillNames { get; private set; } = ImmutableDictionary<string, EntProtoId<SkillDefinitionComponent>>.Empty;

  public override void Initialize()
  {
    this._skillsQuery = this.GetEntityQuery<SkillsComponent>();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.SubscribeLocalEvent<GetMeleeDamageEvent>(new EntityEventRefHandler<GetMeleeDamageEvent>(this.OnGetMeleeDamage));
    this.SubscribeLocalEvent<SkillsComponent, MapInitEvent>(new EntityEventRefHandler<SkillsComponent, MapInitEvent>(this.OnSkillsMapInit));
    this.SubscribeLocalEvent<SkillsComponent, GetVerbsEvent<ExamineVerb>>(new EntityEventRefHandler<SkillsComponent, GetVerbsEvent<ExamineVerb>>(this.OnSkillsVerbExamine));
    this.SubscribeLocalEvent<MedicallyUnskilledDoAfterComponent, AttemptHyposprayUseEvent>(new EntityEventRefHandler<MedicallyUnskilledDoAfterComponent, AttemptHyposprayUseEvent>(this.OnAttemptHyposprayUse));
    this.SubscribeLocalEvent<RequiresSkillComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<RequiresSkillComponent, BeforeRangedInteractEvent>(this.OnRequiresSkillBeforeRangedInteract));
    this.SubscribeLocalEvent<RequiresSkillComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<RequiresSkillComponent, ActivatableUIOpenAttemptEvent>(this.OnRequiresSkillActivatableUIOpenAttempt));
    this.SubscribeLocalEvent<RequiresSkillComponent, UseInHandEvent>(new EntityEventRefHandler<RequiresSkillComponent, UseInHandEvent>(this.OnRequiresSkillUseInHand), new Type[2]
    {
      typeof (HypospraySystem),
      typeof (SharedFlashSystem)
    });
    this.SubscribeLocalEvent<MeleeRequiresSkillComponent, AttemptMeleeEvent>(new EntityEventRefHandler<MeleeRequiresSkillComponent, AttemptMeleeEvent>(this.OnMeleeRequiresSkillAttemptMelee));
    this.SubscribeLocalEvent<MeleeRequiresSkillComponent, ThrowItemAttemptEvent>(new EntityEventRefHandler<MeleeRequiresSkillComponent, ThrowItemAttemptEvent>(this.OnMeleeRequiresSkillThrowAttempt));
    this.SubscribeLocalEvent<MeleeRequiresSkillComponent, UseInHandEvent>(new EntityEventRefHandler<MeleeRequiresSkillComponent, UseInHandEvent>(this.OnMeleeRequiresSkillUseInHand), new Type[2]
    {
      typeof (HypospraySystem),
      typeof (SharedFlashSystem)
    });
    this.SubscribeLocalEvent<ItemToggleRequiresSkillComponent, ItemToggleActivateAttemptEvent>(new EntityEventRefHandler<ItemToggleRequiresSkillComponent, ItemToggleActivateAttemptEvent>(this.OnItemToggleRequiresSkill));
    this.SubscribeLocalEvent<ItemToggleDeactivateUnskilledComponent, GotEquippedEvent>(new EntityEventRefHandler<ItemToggleDeactivateUnskilledComponent, GotEquippedEvent>(this.OnItemToggleDeactivateUnskilled));
    this.SubscribeLocalEvent<ReagentExaminationRequiresSkillComponent, ExaminedEvent>(new EntityEventRefHandler<ReagentExaminationRequiresSkillComponent, ExaminedEvent>(this.OnExamineReagentContainer));
    this.SubscribeLocalEvent<ExamineRequiresSkillComponent, ExaminedEvent>(new EntityEventRefHandler<ExamineRequiresSkillComponent, ExaminedEvent>(this.OnExamineRequiresSkill));
    this.ReloadPrototypes();
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
  {
    if (!ev.WasModified<Robust.Shared.Prototypes.EntityPrototype>())
      return;
    this.ReloadPrototypes();
  }

  private void OnGetMeleeDamage(ref GetMeleeDamageEvent args)
  {
    if (args.User == args.Weapon || this.GetSkill((Entity<SkillsComponent>) args.User, SkillsSystem.MeleeSkill) <= 0)
      return;
    args.Damage = this.ApplyMeleeSkillModifier(args.User, args.Damage);
  }

  private void OnSkillsMapInit(Entity<SkillsComponent> ent, ref MapInitEvent args)
  {
    SkillPresetPrototype prototype;
    if (!this._prototypes.TryIndex<SkillPresetPrototype>(ent.Comp.Preset, out prototype))
      return;
    ent.Comp.Skills = prototype.Skills;
    this.Dirty<SkillsComponent>(ent);
  }

  private void OnSkillsVerbExamine(Entity<SkillsComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
  {
    EntityUid user = args.User;
    if (!args.CanInteract || !args.CanAccess || this.HasComp<XenoComponent>(user))
      return;
    this._skillsSorted.Clear();
    foreach ((EntProtoId<SkillDefinitionComponent> entProtoId, int num) in ent.Comp.Skills)
    {
      Robust.Shared.Prototypes.EntityPrototype prototype;
      if (this._prototypes.TryIndex((EntProtoId) entProtoId, out prototype))
        this._skillsSorted.Add((prototype.Name, num));
    }
    FormattedMessage message = new FormattedMessage();
    if (this._skillsSorted.Count == 0)
    {
      message.AddMarkupPermissive(this.Loc.GetString("rmc-skills-examine-none", ("target", (object) ent)));
    }
    else
    {
      foreach ((string str, int num) in this._skillsSorted)
      {
        if (num != 0)
        {
          message.AddMarkupPermissive(this.Loc.GetString("rmc-skills-examine-skill", ("name", (object) str), ("level", (object) num)));
          message.PushNewline();
        }
      }
    }
    this._examine.AddDetailedExamineVerb(args, (Component) (SkillsComponent) ent, message, this.Loc.GetString("rmc-skills-examine", ("target", (object) ent)), "/Textures/Interface/students-cap.svg.192dpi.png", this.Loc.GetString("rmc-skills-examine", ("target", (object) ent)));
  }

  private void OnAttemptHyposprayUse(
    Entity<MedicallyUnskilledDoAfterComponent> ent,
    ref AttemptHyposprayUseEvent args)
  {
    if (this.HasSkill((Entity<SkillsComponent>) args.User, ent.Comp.Skill, ent.Comp.Min))
      return;
    args.MaxDoAfter(ent.Comp.DoAfter);
  }

  private void OnRequiresSkillBeforeRangedInteract(
    Entity<RequiresSkillComponent> ent,
    ref BeforeRangedInteractEvent args)
  {
    if (args.Handled || this.HasAllSkills((Entity<SkillsComponent>) args.User, ent.Comp.Skills))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-use", ("item", (object) args.Used)), new EntityUid?(args.User), PopupType.SmallCaution);
    args.Handled = true;
  }

  private void OnRequiresSkillActivatableUIOpenAttempt(
    Entity<RequiresSkillComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled || this.HasAllSkills((Entity<SkillsComponent>) args.User, ent.Comp.Skills))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-no-training", ("target", (object) ent)), new EntityUid?(args.User), PopupType.SmallCaution);
    args.Cancel();
  }

  private void OnRequiresSkillUseInHand(Entity<RequiresSkillComponent> ent, ref UseInHandEvent args)
  {
    if (this.HasAllSkills((Entity<SkillsComponent>) args.User, ent.Comp.Skills))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-use", ("item", (object) ent)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
    args.Handled = true;
  }

  private void OnMeleeRequiresSkillAttemptMelee(
    Entity<MeleeRequiresSkillComponent> ent,
    ref AttemptMeleeEvent args)
  {
    if (this.HasAllSkills((Entity<SkillsComponent>) args.User, ent.Comp.Skills))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-use", ("item", (object) ent)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
    args.Cancelled = true;
  }

  private void OnMeleeRequiresSkillThrowAttempt(
    Entity<MeleeRequiresSkillComponent> ent,
    ref ThrowItemAttemptEvent args)
  {
    if (this.HasAllSkills((Entity<SkillsComponent>) args.User, ent.Comp.Skills))
      return;
    if (this._net.IsServer)
      this._popup.PopupEntity(this.Loc.GetString("rmc-skills-cant-use", ("item", (object) ent)), args.User, args.User, PopupType.SmallCaution);
    args.Cancelled = true;
  }

  private void OnMeleeRequiresSkillUseInHand(
    Entity<MeleeRequiresSkillComponent> ent,
    ref UseInHandEvent args)
  {
    if (this.HasAllSkills((Entity<SkillsComponent>) args.User, ent.Comp.Skills))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-use", ("item", (object) ent)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
    args.Handled = true;
  }

  private void OnItemToggleRequiresSkill(
    Entity<ItemToggleRequiresSkillComponent> ent,
    ref ItemToggleActivateAttemptEvent args)
  {
    if (!args.User.HasValue || this.HasAllSkills((Entity<SkillsComponent>) args.User.Value, ent.Comp.Skills))
      return;
    args.Popup = this.Loc.GetString("rmc-skills-cant-use", ("item", (object) ent));
    args.Cancelled = true;
  }

  private void OnItemToggleDeactivateUnskilled(
    Entity<ItemToggleDeactivateUnskilledComponent> ent,
    ref GotEquippedEvent args)
  {
    if (this.HasAllSkills((Entity<SkillsComponent>) args.Equipee, ent.Comp.Skills) || !this._toggle.IsActivated((Entity<ItemToggleComponent>) ent.Owner) || !this._toggle.TryDeactivate((Entity<ItemToggleComponent>) ent.Owner, new EntityUid?(args.Equipee)) || !ent.Comp.Popup.HasValue)
      return;
    ILocalizationManager loc = this.Loc;
    LocId? popup = ent.Comp.Popup;
    string valueOrDefault = popup.HasValue ? (string) popup.GetValueOrDefault() : (string) null;
    (string, object) valueTuple = ("item", (object) ent);
    this._popup.PopupClient(loc.GetString(valueOrDefault, valueTuple), args.Equipee, new EntityUid?(args.Equipee), PopupType.SmallCaution);
  }

  private void OnExamineReagentContainer(
    Entity<ReagentExaminationRequiresSkillComponent> ent,
    ref ExaminedEvent args)
  {
    if (!this.HasAllSkills((Entity<SkillsComponent>) args.Examiner, ent.Comp.Skills))
    {
      if (!ent.Comp.UnskilledExamine.HasValue)
        return;
      using (args.PushGroup("ReagentExaminationRequiresSkillComponent"))
      {
        ExaminedEvent examinedEvent = args;
        ILocalizationManager loc = this.Loc;
        LocId? unskilledExamine = ent.Comp.UnskilledExamine;
        string valueOrDefault = unskilledExamine.HasValue ? (string) unskilledExamine.GetValueOrDefault() : (string) null;
        string markup = loc.GetString(valueOrDefault);
        examinedEvent.PushMarkup(markup);
      }
    }
    else
    {
      EntityUid examined = args.Examined;
      if (ent.Comp.ContainerId != null)
      {
        BaseContainer container;
        EntityUid? element;
        if (!this._container.TryGetContainer(args.Examined, ent.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out element))
        {
          if (!ent.Comp.NoContainerExamine.HasValue)
            return;
          using (args.PushGroup("ReagentExaminationRequiresSkillComponent"))
          {
            ExaminedEvent examinedEvent = args;
            ILocalizationManager loc = this.Loc;
            LocId? containerExamine = ent.Comp.NoContainerExamine;
            string valueOrDefault = containerExamine.HasValue ? (string) containerExamine.GetValueOrDefault() : (string) null;
            (string, object) valueTuple = ("target", (object) ent.Owner);
            string markup = loc.GetString(valueOrDefault, valueTuple);
            examinedEvent.PushMarkup(markup);
            return;
          }
        }
        examined = element.Value;
      }
      SolutionContainerManagerComponent comp;
      if (!this.TryComp<SolutionContainerManagerComponent>(examined, out comp))
        return;
      List<ReagentQuantity> source = new List<ReagentQuantity>();
      foreach (string container in comp.Containers)
      {
        Solution solution;
        if (this._solutionContainerSystem.TryGetSolution((Entity<SolutionContainerManagerComponent>) examined, container, out Entity<SolutionComponent>? _, out solution))
        {
          foreach (ReagentQuantity content in solution.Contents)
            source.Add(content);
        }
      }
      if (!source.Any<ReagentQuantity>())
      {
        using (args.PushGroup("ReagentExaminationRequiresSkillComponent"))
          args.PushMarkup(this.Loc.GetString((string) ent.Comp.SkilledExamineNone, ("target", (object) ent.Owner)));
      }
      else
      {
        string str = string.Join("; ", source.Select<ReagentQuantity, string>((Func<ReagentQuantity, string>) (r => $"{this._reagent.Index((ProtoId<ReagentPrototype>) r.Reagent.Prototype).LocalizedName}({r.Quantity}u)")));
        using (args.PushGroup("ReagentExaminationRequiresSkillComponent"))
          args.PushMarkup(this.Loc.GetString((string) ent.Comp.SkilledExamineContains, ("target", (object) ent.Owner), ("reagents", (object) str)));
      }
    }
  }

  private void OnExamineRequiresSkill(
    Entity<ExamineRequiresSkillComponent> ent,
    ref ExaminedEvent args)
  {
    if (!this.HasAllSkills((Entity<SkillsComponent>) args.Examiner, ent.Comp.Skills))
    {
      if (!ent.Comp.UnskilledExamine.HasValue)
        return;
      using (args.PushGroup("ExamineRequiresSkillComponent", ent.Comp.ExaminePriority))
      {
        ExaminedEvent examinedEvent = args;
        ILocalizationManager loc = this.Loc;
        LocId? unskilledExamine = ent.Comp.UnskilledExamine;
        string valueOrDefault = unskilledExamine.HasValue ? (string) unskilledExamine.GetValueOrDefault() : (string) null;
        string markup = loc.GetString(valueOrDefault);
        examinedEvent.PushMarkup(markup);
      }
    }
    else
    {
      using (args.PushGroup("ExamineRequiresSkillComponent", ent.Comp.ExaminePriority))
        args.PushMarkup(this.Loc.GetString((string) ent.Comp.SkilledExamine));
    }
  }

  private void ReloadPrototypes()
  {
    ImmutableArray<EntProtoId<SkillDefinitionComponent>>.Builder builder1 = ImmutableArray.CreateBuilder<EntProtoId<SkillDefinitionComponent>>();
    ImmutableDictionary<string, EntProtoId<SkillDefinitionComponent>>.Builder builder2 = ImmutableDictionary.CreateBuilder<string, EntProtoId<SkillDefinitionComponent>>();
    foreach (Robust.Shared.Prototypes.EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<Robust.Shared.Prototypes.EntityPrototype>())
    {
      if (enumeratePrototype.HasComponent<SkillDefinitionComponent>())
      {
        string id1 = enumeratePrototype.ID;
        string key = enumeratePrototype.Name.Replace(" ", string.Empty);
        builder1.Add((EntProtoId<SkillDefinitionComponent>) id1);
        if (!builder2.TryAdd<string, EntProtoId<SkillDefinitionComponent>>(key, (EntProtoId<SkillDefinitionComponent>) id1))
        {
          string id2 = builder2.GetValueOrDefault(key).Id;
          this.Log.Error($"Duplicate skill name found: {key}, old: {id2}, new: {id1}");
        }
      }
    }
    this.Skills = builder1.ToImmutable();
    this.SkillNames = builder2.ToImmutable();
  }

  public TimeSpan GetDelay(EntityUid user, EntityUid tool)
  {
    MedicallyUnskilledDoAfterComponent comp;
    if (!this.TryComp<MedicallyUnskilledDoAfterComponent>(tool, out comp) || comp.Min <= 0)
      return new TimeSpan();
    return !this.HasSkill((Entity<SkillsComponent>) user, comp.Skill, comp.Min) ? comp.DoAfter : new TimeSpan();
  }

  public int GetSkill(Entity<SkillsComponent?> ent, EntProtoId<SkillDefinitionComponent> skill)
  {
    if (skill == new EntProtoId<SkillDefinitionComponent>())
      this.Log.Error($"Empty skill {skill} passed to {nameof (GetSkill)}!");
    return !this._skillsQuery.Resolve((EntityUid) ent, ref ent.Comp, false) ? 0 : ent.Comp.Skills.GetValueOrDefault<EntProtoId<SkillDefinitionComponent>, int>(skill);
  }

  public bool HasSkills(Entity<SkillsComponent?> ent, SkillWhitelist whitelist)
  {
    return this.HasAllSkills(ent, whitelist.All);
  }

  public bool HasAllSkills(
    Entity<SkillsComponent?> ent,
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> required)
  {
    if (this.HasComp<BypassSkillChecksComponent>((EntityUid) ent))
      return true;
    this._skillsQuery.Resolve((EntityUid) ent, ref ent.Comp, false);
    foreach ((EntProtoId<SkillDefinitionComponent> key, int num1) in required)
    {
      int num2;
      if (num1 > 0 && (ent.Comp == null || !ent.Comp.Skills.TryGetValue(key, out num2) || num2 < num1))
        return false;
    }
    return true;
  }

  public bool HasAllSkills(Entity<SkillsComponent?> ent, List<Skill> allRequired)
  {
    if (this.HasComp<BypassSkillChecksComponent>((EntityUid) ent))
      return true;
    this._skillsQuery.Resolve((EntityUid) ent, ref ent.Comp, false);
    Span<Skill> span = CollectionsMarshal.AsSpan<Skill>(allRequired);
    for (int index = 0; index < span.Length; ++index)
    {
      ref Skill local = ref span[index];
      int num;
      if (local.Level > 0 && (ent.Comp == null || !ent.Comp.Skills.TryGetValue(local.Type, out num) || num < local.Level))
        return false;
    }
    return true;
  }

  public bool HasAnySkills(
    Entity<SkillsComponent?> ent,
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> anyRequired)
  {
    if (this.HasComp<BypassSkillChecksComponent>((EntityUid) ent))
      return true;
    this._skillsQuery.Resolve((EntityUid) ent, ref ent.Comp, false);
    foreach ((EntProtoId<SkillDefinitionComponent> key, int num1) in anyRequired)
    {
      int num2;
      if (num1 > 0 && ent.Comp != null && ent.Comp.Skills.TryGetValue(key, out num2) && num2 >= num1)
        return true;
    }
    return false;
  }

  public bool HasAnySkills(Entity<SkillsComponent?> ent, List<Skill> anyRequired)
  {
    if (this.HasComp<BypassSkillChecksComponent>((EntityUid) ent))
      return true;
    this._skillsQuery.Resolve((EntityUid) ent, ref ent.Comp, false);
    Span<Skill> span = CollectionsMarshal.AsSpan<Skill>(anyRequired);
    for (int index = 0; index < span.Length; ++index)
    {
      ref Skill local = ref span[index];
      int num;
      if (local.Level > 0 && ent.Comp != null && ent.Comp.Skills.TryGetValue(local.Type, out num) && num >= local.Level)
        return false;
    }
    return true;
  }

  public bool HasSkill(
    Entity<SkillsComponent?> ent,
    EntProtoId<SkillDefinitionComponent> skill,
    int required)
  {
    if (this.HasComp<BypassSkillChecksComponent>((EntityUid) ent) || required <= 0)
      return true;
    int num;
    return this._skillsQuery.Resolve((EntityUid) ent, ref ent.Comp, false) && ent.Comp.Skills.TryGetValue(skill, out num) && num >= required;
  }

  public void IncrementSkill(
    Entity<SkillsComponent?> ent,
    EntProtoId<SkillDefinitionComponent> skill,
    int by = 1)
  {
    ref SkillsComponent local = ref ent.Comp;
    if (local == null)
      local = this.EnsureComp<SkillsComponent>((EntityUid) ent);
    this.SetSkill(ent, skill, ent.Comp.Skills.GetValueOrDefault<EntProtoId<SkillDefinitionComponent>, int>(skill) + by);
  }

  public void IncrementSkills(
    Entity<SkillsComponent?> ent,
    List<EntProtoId<SkillDefinitionComponent>> skills,
    int by = 1)
  {
    ref SkillsComponent local1 = ref ent.Comp;
    if (local1 == null)
      local1 = this.EnsureComp<SkillsComponent>((EntityUid) ent);
    Span<EntProtoId<SkillDefinitionComponent>> span = CollectionsMarshal.AsSpan<EntProtoId<SkillDefinitionComponent>>(skills);
    for (int index = 0; index < span.Length; ++index)
    {
      ref EntProtoId<SkillDefinitionComponent> local2 = ref span[index];
      this.SetSkill(ent, local2, ent.Comp.Skills.GetValueOrDefault<EntProtoId<SkillDefinitionComponent>, int>(local2) + by);
    }
  }

  public void IncrementSkills(
    Entity<SkillsComponent?> ent,
    HashSet<EntProtoId<SkillDefinitionComponent>> skills,
    int by = 1)
  {
    ref SkillsComponent local = ref ent.Comp;
    if (local == null)
      local = this.EnsureComp<SkillsComponent>((EntityUid) ent);
    foreach (EntProtoId<SkillDefinitionComponent> skill in skills)
      this.SetSkill(ent, skill, ent.Comp.Skills.GetValueOrDefault<EntProtoId<SkillDefinitionComponent>, int>(skill) + by);
  }

  public void RemoveAllSkills(Entity<SkillsComponent?> ent)
  {
    if (!this._skillsQuery.Resolve((EntityUid) ent, ref ent.Comp, false))
      return;
    ent.Comp.Skills.Clear();
    this.Dirty<SkillsComponent>(ent);
  }

  public void SetSkill(
    Entity<SkillsComponent?> ent,
    EntProtoId<SkillDefinitionComponent> skill,
    int to)
  {
    if (skill == new EntProtoId<SkillDefinitionComponent>())
    {
      this.Log.Error($"Empty skill {skill} passed to {nameof (SetSkill)}!");
    }
    else
    {
      ref SkillsComponent local = ref ent.Comp;
      if (local == null)
        local = this.EnsureComp<SkillsComponent>((EntityUid) ent);
      ent.Comp.Skills[skill] = to;
      this.Dirty<SkillsComponent>(ent);
    }
  }

  public void SetSkills(
    Entity<SkillsComponent?> ent,
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> to)
  {
    ref SkillsComponent local = ref ent.Comp;
    if (local == null)
      local = this.EnsureComp<SkillsComponent>((EntityUid) ent);
    foreach ((EntProtoId<SkillDefinitionComponent> key, int num) in to)
      ent.Comp.Skills[key] = num;
    this.Dirty<SkillsComponent>(ent);
  }

  public void SetSkills(Entity<SkillsComponent?> ent, List<Skill> to)
  {
    ref SkillsComponent local1 = ref ent.Comp;
    if (local1 == null)
      local1 = this.EnsureComp<SkillsComponent>((EntityUid) ent);
    Span<Skill> span = CollectionsMarshal.AsSpan<Skill>(to);
    for (int index = 0; index < span.Length; ++index)
    {
      ref Skill local2 = ref span[index];
      ent.Comp.Skills[local2.Type] = local2.Level;
    }
    this.Dirty<SkillsComponent>(ent);
  }

  public void SetSkills(Entity<SkillsComponent?> ent, HashSet<Skill> to)
  {
    ref SkillsComponent local = ref ent.Comp;
    if (local == null)
      local = this.EnsureComp<SkillsComponent>((EntityUid) ent);
    foreach (Skill skill in to)
      ent.Comp.Skills[skill.Type] = skill.Level;
    this.Dirty<SkillsComponent>(ent);
  }

  public float GetSkillDelayMultiplier(
    Entity<SkillsComponent?> user,
    EntProtoId<SkillDefinitionComponent> definition,
    float[]? multipliers = null)
  {
    SkillDefinitionComponent comp;
    if (!definition.TryGet(out comp, this._prototypes, this._compFactory))
      return 1f;
    if (multipliers == null)
      multipliers = comp.DelayMultipliers;
    if (multipliers.Length == 0)
      return 1f;
    int skill = this.GetSkill(user, definition);
    float skillDelayMultiplier;
    if (!((IList<float>) multipliers).TryGetValue<float>(skill, out skillDelayMultiplier))
    {
      float[] numArray = multipliers;
      skillDelayMultiplier = numArray[numArray.Length - 1];
    }
    return skillDelayMultiplier;
  }

  public DamageSpecifier ApplyMeleeSkillModifier(EntityUid user, DamageSpecifier damage)
  {
    int skill = this.GetSkill((Entity<SkillsComponent>) user, SkillsSystem.MeleeSkill);
    return damage * (FixedPoint2) (1.0 + 0.25 * (double) skill);
  }
}
