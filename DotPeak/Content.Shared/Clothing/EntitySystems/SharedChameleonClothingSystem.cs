// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.SharedChameleonClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Contraband;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public abstract class SharedChameleonClothingSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private ClothingSystem _clothingSystem;
  [Dependency]
  private ContrabandSystem _contraband;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private SharedItemSystem _itemSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  protected IGameTiming _timing;
  private static readonly SlotFlags[] IgnoredSlots = new SlotFlags[3]
  {
    SlotFlags.All,
    SlotFlags.PREVENTEQUIP,
    SlotFlags.NONE
  };
  private static readonly SlotFlags[] Slots = ((IEnumerable<SlotFlags>) Enum.GetValues<SlotFlags>()).Except<SlotFlags>((IEnumerable<SlotFlags>) SharedChameleonClothingSystem.IgnoredSlots).ToArray<SlotFlags>();
  private readonly Dictionary<SlotFlags, List<EntProtoId>> _data = new Dictionary<SlotFlags, List<EntProtoId>>();
  public readonly Dictionary<SlotFlags, List<string>> ValidVariants = new Dictionary<SlotFlags, List<string>>();
  [Dependency]
  protected SharedUserInterfaceSystem UI;
  private static readonly ProtoId<TagPrototype> WhitelistChameleonTag = ProtoId<TagPrototype>.op_Implicit("WhitelistChameleon");

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonClothingComponent, GotEquippedEvent>(new ComponentEventHandler<ChameleonClothingComponent, GotEquippedEvent>((object) this, __methodptr(OnGotEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<ChameleonClothingComponent, GotUnequippedEvent>((object) this, __methodptr(OnGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonClothingComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<ChameleonClothingComponent, GetVerbsEvent<InteractionVerb>>((object) this, __methodptr(OnVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonClothingComponent, PrototypesReloadedEventArgs>(new ComponentEventHandler<ChameleonClothingComponent, PrototypesReloadedEventArgs>((object) this, __methodptr(OnPrototypeReload)), (Type[]) null, (Type[]) null);
    this.PrepareAllVariants();
  }

  private void OnPrototypeReload(
    EntityUid uid,
    ChameleonClothingComponent component,
    PrototypesReloadedEventArgs args)
  {
    this.PrepareAllVariants();
  }

  private void OnGotEquipped(
    EntityUid uid,
    ChameleonClothingComponent component,
    GotEquippedEvent args)
  {
    component.User = new EntityUid?(args.Equipee);
  }

  private void OnGotUnequipped(
    EntityUid uid,
    ChameleonClothingComponent component,
    GotUnequippedEvent args)
  {
    component.User = new EntityUid?();
  }

  protected void UpdateVisuals(EntityUid uid, ChameleonClothingComponent component)
  {
    EntProtoId? nullable = component.Default;
    EntityPrototype proto;
    if (string.IsNullOrEmpty(nullable.HasValue ? EntProtoId.op_Implicit(nullable.GetValueOrDefault()) : (string) null) || !this._proto.TryIndex(component.Default, ref proto))
      return;
    this.UpdateSprite(uid, proto);
    if (!this.HasComp<IdCardComponent>(uid))
    {
      MetaDataComponent metaDataComponent = this.MetaData(uid);
      this._metaData.SetEntityName(uid, proto.Name, metaDataComponent, true);
      this._metaData.SetEntityDescription(uid, proto.Description, metaDataComponent);
    }
    ItemComponent itemComponent;
    ItemComponent otherItem;
    if (this.TryComp<ItemComponent>(uid, ref itemComponent) && proto.TryGetComponent<ItemComponent>(ref otherItem, this.Factory))
      this._itemSystem.CopyVisuals(uid, otherItem, itemComponent);
    ClothingComponent clothing;
    ClothingComponent otherClothing;
    if (this.TryComp<ClothingComponent>(uid, ref clothing) && proto.TryGetComponent<ClothingComponent>("Clothing", ref otherClothing))
      this._clothingSystem.CopyVisuals(uid, otherClothing, clothing);
    AppearanceComponent appearanceComponent1;
    AppearanceComponent appearanceComponent2;
    if (this.TryComp<AppearanceComponent>(uid, ref appearanceComponent1) && proto.TryGetComponent<AppearanceComponent>("Appearance", ref appearanceComponent2))
    {
      this._appearance.AppendData(appearanceComponent2, Entity<AppearanceComponent>.op_Implicit(uid));
      this.Dirty(uid, (IComponent) appearanceComponent1, (MetaDataComponent) null);
    }
    ContrabandComponent other;
    if (proto.TryGetComponent<ContrabandComponent>("Contraband", ref other))
    {
      ContrabandComponent contraband;
      this.EnsureComp<ContrabandComponent>(uid, ref contraband);
      this._contraband.CopyDetails(uid, other, contraband);
    }
    else
      this.RemComp<ContrabandComponent>(uid);
  }

  private void OnVerb(
    Entity<ChameleonClothingComponent> ent,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid? user1 = ent.Comp.User;
    EntityUid user2 = args.User;
    if ((user1.HasValue ? (EntityUid.op_Inequality(user1.GetValueOrDefault(), user2) ? 1 : 0) : 1) != 0)
      return;
    EntityUid user = args.User;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Text = this.Loc.GetString("chameleon-component-verb-text");
    interactionVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png"));
    interactionVerb.Act = (Action) (() => this.UI.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) ChameleonUiKey.Key, user));
    verbs.Add(interactionVerb);
  }

  protected virtual void UpdateSprite(EntityUid uid, EntityPrototype proto)
  {
  }

  public bool IsValidTarget(EntityPrototype proto, SlotFlags chameleonSlot = SlotFlags.NONE, string? requiredTag = null)
  {
    TagComponent component;
    ClothingComponent clothingComponent;
    return !proto.Abstract && !proto.HideSpawnMenu && proto.TryGetComponent<TagComponent>(ref component, this.Factory) && this._tag.HasTag(component, SharedChameleonClothingSystem.WhitelistChameleonTag) && (requiredTag == null || this._tag.HasTag(component, ProtoId<TagPrototype>.op_Implicit(requiredTag))) && proto.TryGetComponent<ClothingComponent>("Clothing", ref clothingComponent) && clothingComponent.Slots.HasFlag((Enum) chameleonSlot);
  }

  public IEnumerable<EntProtoId> GetValidTargets(SlotFlags slot, string? tag = null)
  {
    List<EntProtoId> validTargets = new List<EntProtoId>();
    if (tag != null)
    {
      foreach (EntProtoId entProtoId in this._data[slot])
      {
        if (this.IsValidTarget(this._proto.Index(entProtoId), slot, tag))
          validTargets.Add(entProtoId);
      }
    }
    else
      validTargets = this._data[slot];
    return (IEnumerable<EntProtoId>) validTargets;
  }

  protected void PrepareAllVariants()
  {
    this._data.Clear();
    foreach (EntityPrototype enumeratePrototype in this._proto.EnumeratePrototypes<EntityPrototype>())
    {
      ClothingComponent clothingComponent;
      if (this.IsValidTarget(enumeratePrototype) && enumeratePrototype.TryGetComponent<ClothingComponent>(ref clothingComponent, this.Factory))
      {
        foreach (SlotFlags slot in SharedChameleonClothingSystem.Slots)
        {
          if (clothingComponent.Slots.HasFlag((Enum) slot))
          {
            if (!this._data.ContainsKey(slot))
              this._data.Add(slot, new List<EntProtoId>());
            this._data[slot].Add(EntProtoId.op_Implicit(enumeratePrototype.ID));
          }
        }
      }
    }
  }
}
