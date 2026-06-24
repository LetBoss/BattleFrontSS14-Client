// Decompiled with JetBrains decompiler
// Type: Content.Shared.Labels.EntitySystems.LabelSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Examine;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Labels.Components;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Paper;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Labels.EntitySystems;

public sealed class LabelSystem : EntitySystem
{
  [Dependency]
  private NameModifierSystem _nameModifier;
  [Dependency]
  private ItemSlotsSystem _itemSlots;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private CMExamineSystem _rmcExamine;
  public const string ContainerName = "paper_label";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<LabelComponent, MapInitEvent>(new EntityEventRefHandler<LabelComponent, MapInitEvent>(this.OnLabelCompMapInit));
    this.SubscribeLocalEvent<LabelComponent, ExaminedEvent>(new EntityEventRefHandler<LabelComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<LabelComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<LabelComponent, RefreshNameModifiersEvent>(this.OnRefreshNameModifiers));
    this.SubscribeLocalEvent<PaperLabelComponent, ComponentInit>(new EntityEventRefHandler<PaperLabelComponent, ComponentInit>(this.OnComponentInit));
    this.SubscribeLocalEvent<PaperLabelComponent, ComponentRemove>(new EntityEventRefHandler<PaperLabelComponent, ComponentRemove>(this.OnComponentRemove));
    this.SubscribeLocalEvent<PaperLabelComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<PaperLabelComponent, EntInsertedIntoContainerMessage>(this.OnContainerModified));
    this.SubscribeLocalEvent<PaperLabelComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PaperLabelComponent, EntRemovedFromContainerMessage>(this.OnContainerModified));
    this.SubscribeLocalEvent<PaperLabelComponent, ExaminedEvent>(new EntityEventRefHandler<PaperLabelComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnLabelCompMapInit(Entity<LabelComponent> ent, ref MapInitEvent args)
  {
    if (!string.IsNullOrEmpty(ent.Comp.CurrentLabel))
    {
      ent.Comp.CurrentLabel = this.Loc.GetString(ent.Comp.CurrentLabel);
      this.Dirty<LabelComponent>(ent);
    }
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) ent.Owner);
  }

  public void Label(EntityUid uid, string? text, MetaDataComponent? metadata = null, LabelComponent? label = null)
  {
    if (label == null)
      label = this.EnsureComp<LabelComponent>(uid);
    label.CurrentLabel = text;
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) uid);
    this.Dirty(uid, (IComponent) label);
  }

  private void OnExamine(Entity<LabelComponent> ent, ref ExaminedEvent args)
  {
    if (!this._rmcExamine.CanExamine((Entity<BlockExamineComponent>) ent.Owner, args.Examiner) || !ent.Comp.Examinable || ent.Comp.CurrentLabel == null)
      return;
    FormattedMessage message = new FormattedMessage();
    message.AddText(this.Loc.GetString("hand-labeler-has-label", ("label", (object) ent.Comp.CurrentLabel)));
    args.PushMessage(message);
  }

  private void OnRefreshNameModifiers(
    Entity<LabelComponent> entity,
    ref RefreshNameModifiersEvent args)
  {
    if (string.IsNullOrEmpty(entity.Comp.CurrentLabel))
      return;
    args.AddModifier((LocId) "comp-label-format", 0, ("label", (object) entity.Comp.CurrentLabel));
  }

  private void OnComponentInit(Entity<PaperLabelComponent> ent, ref ComponentInit args)
  {
    this._itemSlots.AddItemSlot((EntityUid) ent, "paper_label", ent.Comp.LabelSlot);
    this.UpdateAppearance((Entity<PaperLabelComponent, AppearanceComponent>) ent);
  }

  private void OnComponentRemove(Entity<PaperLabelComponent> ent, ref ComponentRemove args)
  {
    this._itemSlots.RemoveItemSlot((EntityUid) ent, ent.Comp.LabelSlot);
  }

  private void OnExamined(Entity<PaperLabelComponent> ent, ref ExaminedEvent args)
  {
    EntityUid? nullable = ent.Comp.LabelSlot.Item;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    if (!valueOrDefault.Valid)
      return;
    using (args.PushGroup("PaperLabelComponent"))
    {
      if (!args.IsInDetailsRange)
      {
        args.PushMarkup(this.Loc.GetString("comp-paper-label-has-label-cant-read"));
      }
      else
      {
        PaperComponent comp;
        if (!this.TryComp<PaperComponent>(valueOrDefault, out comp))
          return;
        if (string.IsNullOrWhiteSpace(comp.Content))
        {
          args.PushMarkup(this.Loc.GetString("comp-paper-label-has-label-blank"));
        }
        else
        {
          args.PushMarkup(this.Loc.GetString("comp-paper-label-has-label"));
          string content = comp.Content;
          args.PushMarkup(content.TrimEnd());
        }
      }
    }
  }

  private void OnContainerModified(
    EntityUid uid,
    PaperLabelComponent label,
    ContainerModifiedMessage args)
  {
    if (!label.Initialized || args.Container.ID != label.LabelSlot.ID)
      return;
    this.UpdateAppearance((Entity<PaperLabelComponent, AppearanceComponent>) (uid, label));
  }

  private void UpdateAppearance(
    Entity<PaperLabelComponent, AppearanceComponent?> ent)
  {
    if (!this.Resolve<AppearanceComponent>((EntityUid) ent, ref ent.Comp2, false))
      return;
    ItemSlot labelSlot = ent.Comp1.LabelSlot;
    this._appearance.SetData((EntityUid) ent, (Enum) PaperLabelVisuals.HasLabel, (object) labelSlot.HasItem, ent.Comp2);
    PaperLabelTypeComponent comp;
    if (!this.TryComp<PaperLabelTypeComponent>(labelSlot.Item, out comp))
      return;
    this._appearance.SetData((EntityUid) ent, (Enum) PaperLabelVisuals.LabelType, (object) comp.PaperType, ent.Comp2);
  }

  public bool TryGetLabel<T>(Entity<PaperLabelComponent?> ent, [NotNullWhen(true)] out Entity<T>? label) where T : Component
  {
    label = new Entity<T>?();
    if (!this.Resolve<PaperLabelComponent>((EntityUid) ent, ref ent.Comp, false))
      return false;
    EntityUid? nullable = ent.Comp.LabelSlot.Item;
    if (!nullable.HasValue)
      return false;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    T comp;
    if (!this.TryComp<T>(valueOrDefault, out comp))
      return false;
    label = new Entity<T>?((Entity<T>) (valueOrDefault, comp));
    return true;
  }
}
