// Decompiled with JetBrains decompiler
// Type: Content.Client.Humanoid.HumanoidAppearanceSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.DisplacementMap;
using Content.Client.Items.Systems;
using Content.Shared._RMC14.Humanoid;
using Content.Shared.CCVar;
using Content.Shared.DisplacementMap;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Humanoid;

public sealed class HumanoidAppearanceSystem : SharedHumanoidAppearanceSystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private MarkingManager _markingManager;
  [Dependency]
  private IConfigurationManager _configurationManager;
  [Dependency]
  private DisplacementMapSystem _displacement;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private ItemSystem _item;
  [Dependency]
  private RMCHumanoidAppearanceSystem _rmcHumanoid;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HumanoidAppearanceComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<HumanoidAppearanceComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configurationManager, CCVars.AccessibilityClientCensorNudity, new Action<bool>(this.OnCvarChanged), true);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configurationManager, CCVars.AccessibilityServerCensorNudity, new Action<bool>(this.OnCvarChanged), true);
    this.SubscribeLocalEvent<LocalPlayerAttachedEvent>(new EntityEventHandler<LocalPlayerAttachedEvent>(this.UpdateHiddenSprites<LocalPlayerAttachedEvent>), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<LocalPlayerDetachedEvent>(new EntityEventHandler<LocalPlayerDetachedEvent>(this.UpdateHiddenSprites<LocalPlayerDetachedEvent>), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HiddenAppearanceComponent, ComponentRemove>(new EntityEventRefHandler<HiddenAppearanceComponent, ComponentRemove>((object) this, __methodptr(OnHiddenRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    EntityUid uid,
    HumanoidAppearanceComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateSprite(uid, (IRMCHumanoidAppearance) component, this.Comp<SpriteComponent>(uid));
  }

  private void OnCvarChanged(bool value)
  {
    AllEntityQueryEnumerator<HumanoidAppearanceComponent, SpriteComponent> entityQueryEnumerator = this.AllEntityQuery<HumanoidAppearanceComponent, SpriteComponent>();
    EntityUid entity;
    HumanoidAppearanceComponent humanoid;
    SpriteComponent sprite;
    while (entityQueryEnumerator.MoveNext(ref entity, ref humanoid, ref sprite))
      this.UpdateSprite(entity, (IRMCHumanoidAppearance) humanoid, sprite);
  }

  private void UpdateSprite(
    EntityUid entity,
    IRMCHumanoidAppearance humanoid,
    SpriteComponent sprite)
  {
    this.ClearAllMarkings(entity, humanoid, sprite);
    foreach (KeyValuePair<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> baseLayer in humanoid.BaseLayers)
    {
      int num;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum) baseLayer.Key, ref num, false))
        sprite[num].Visible = false;
    }
    IRMCHumanoidAppearance humanoidAppearance = humanoid;
    IRMCHumanoidAppearance appearance;
    if (this._rmcHumanoid.TryGetLocalHiddenAppearance(entity, out appearance))
      humanoidAppearance = appearance;
    if (appearance != null)
    {
      this.ClearAllMarkings(entity, appearance, sprite);
      foreach (KeyValuePair<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> baseLayer in appearance.BaseLayers)
      {
        int num;
        if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum) baseLayer.Key, ref num, false))
          sprite[num].Visible = false;
      }
    }
    this.UpdateLayers(entity, humanoidAppearance, sprite);
    this.ApplyMarkingSet(entity, humanoidAppearance, sprite);
    sprite[this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum) HumanoidVisualLayers.Eyes)].Color = humanoidAppearance.EyeColor;
  }

  private static bool IsHidden(IRMCHumanoidAppearance humanoid, HumanoidVisualLayers layer)
  {
    return humanoid.HiddenLayers.ContainsKey(layer) || humanoid.PermanentlyHidden.Contains(layer);
  }

  private void UpdateLayers(
    EntityUid entity,
    IRMCHumanoidAppearance component,
    SpriteComponent sprite)
  {
    HashSet<HumanoidVisualLayers> humanoidVisualLayersSet = new HashSet<HumanoidVisualLayers>((IEnumerable<HumanoidVisualLayers>) component.BaseLayers.Keys);
    component.BaseLayers.Clear();
    foreach ((HumanoidVisualLayers key3, string str) in this._prototypeManager.Index<HumanoidSpeciesBaseSpritesPrototype>(this._prototypeManager.Index<SpeciesPrototype>(component.Species).SpriteSet).Sprites)
    {
      HumanoidVisualLayers key2 = key3;
      string protoId = str;
      humanoidVisualLayersSet.Remove(key2);
      if (!component.CustomBaseLayers.ContainsKey(key2))
        this.SetLayerData(entity, component, sprite, key2, protoId, true);
    }
    CustomBaseLayerInfo customBaseLayerInfo2;
    foreach ((key3, customBaseLayerInfo2) in component.CustomBaseLayers)
    {
      HumanoidVisualLayers humanoidVisualLayers = key3;
      CustomBaseLayerInfo customBaseLayerInfo3 = customBaseLayerInfo2;
      humanoidVisualLayersSet.Remove(humanoidVisualLayers);
      EntityUid entity1 = entity;
      IRMCHumanoidAppearance component1 = component;
      SpriteComponent sprite1 = sprite;
      int key4 = (int) humanoidVisualLayers;
      ProtoId<HumanoidSpeciesSpriteLayer>? id = customBaseLayerInfo3.Id;
      string protoId = id.HasValue ? ProtoId<HumanoidSpeciesSpriteLayer>.op_Implicit(id.GetValueOrDefault()) : (string) null;
      Color? color = customBaseLayerInfo3.Color;
      this.SetLayerData(entity1, component1, sprite1, (HumanoidVisualLayers) key4, protoId, color: color);
    }
    foreach (HumanoidVisualLayers humanoidVisualLayers in humanoidVisualLayersSet)
    {
      int num;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum) humanoidVisualLayers, ref num, false))
        sprite[num].Visible = false;
    }
  }

  private void SetLayerData(
    EntityUid entity,
    IRMCHumanoidAppearance component,
    SpriteComponent sprite,
    HumanoidVisualLayers key,
    string? protoId,
    bool sexMorph = false,
    Color? color = null)
  {
    int num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum) key);
    ISpriteLayer ispriteLayer1 = sprite[num];
    ispriteLayer1.Visible = !HumanoidAppearanceSystem.IsHidden(component, key);
    if (color.HasValue)
      ispriteLayer1.Color = color.Value;
    if (protoId == null)
      return;
    if (sexMorph)
      protoId = HumanoidVisualLayersExtension.GetSexMorph(key, component.Sex, protoId);
    HumanoidSpeciesSpriteLayer speciesSpriteLayer = this._prototypeManager.Index<HumanoidSpeciesSpriteLayer>(protoId);
    component.BaseLayers[key] = speciesSpriteLayer;
    if (speciesSpriteLayer.MatchSkin)
    {
      ISpriteLayer ispriteLayer2 = ispriteLayer1;
      Color skinColor = component.SkinColor;
      Color color1 = ((Color) ref skinColor).WithAlpha(speciesSpriteLayer.LayerAlpha);
      ispriteLayer2.Color = color1;
    }
    if (speciesSpriteLayer.BaseSprite == null)
      return;
    this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((entity, sprite)), num, speciesSpriteLayer.BaseSprite);
  }

  public override void LoadProfile(
    EntityUid uid,
    HumanoidCharacterProfile? profile,
    HumanoidAppearanceComponent? humanoid = null)
  {
    if (profile == null || !this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
      return;
    Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> dictionary1 = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();
    MarkingSet markingSet = new MarkingSet(ProtoId<MarkingPointsPrototype>.op_Implicit(this._prototypeManager.Index<SpeciesPrototype>(profile.Species).MarkingPoints), this._markingManager, this._prototypeManager);
    Dictionary<Marking, MarkingPrototype> dictionary2 = new Dictionary<Marking, MarkingPrototype>();
    foreach (Marking marking in profile.Appearance.Markings)
    {
      MarkingPrototype markingResult;
      if (this._markingManager.TryGetMarking(marking, out markingResult))
      {
        if (!markingResult.ForcedColoring)
          markingSet.AddBack(markingResult.MarkingCategory, marking);
        else
          dictionary2.Add(marking, markingResult);
      }
    }
    float alpha1;
    Color skinColor;
    Color color1;
    if (!this._markingManager.MustMatchSkin(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), HumanoidVisualLayers.Hair, out alpha1, this._prototypeManager))
    {
      color1 = profile.Appearance.HairColor;
    }
    else
    {
      skinColor = profile.Appearance.SkinColor;
      color1 = ((Color) ref skinColor).WithAlpha(alpha1);
    }
    Color color2 = color1;
    Marking marking1 = new Marking(profile.Appearance.HairStyleId, (IReadOnlyList<Color>) new Color[1]
    {
      color2
    });
    float alpha2;
    Color color3;
    if (!this._markingManager.MustMatchSkin(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), HumanoidVisualLayers.FacialHair, out alpha2, this._prototypeManager))
    {
      color3 = profile.Appearance.FacialHairColor;
    }
    else
    {
      skinColor = profile.Appearance.SkinColor;
      color3 = ((Color) ref skinColor).WithAlpha(alpha2);
    }
    Color color4 = color3;
    Marking marking2 = new Marking(profile.Appearance.FacialHairStyleId, (IReadOnlyList<Color>) new Color[1]
    {
      color4
    });
    if (this._markingManager.CanBeApplied(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), profile.Sex, marking1, this._prototypeManager))
      markingSet.AddBack(MarkingCategories.Hair, marking1);
    if (this._markingManager.CanBeApplied(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), profile.Sex, marking2, this._prototypeManager))
      markingSet.AddBack(MarkingCategories.FacialHair, marking2);
    foreach ((Marking key, MarkingPrototype prototype) in dictionary2)
    {
      List<Color> markingLayerColors = MarkingColoring.GetMarkingLayerColors(prototype, new Color?(profile.Appearance.SkinColor), new Color?(profile.Appearance.EyeColor), markingSet);
      markingSet.AddBack(prototype.MarkingCategory, new Marking(key.MarkingId, markingLayerColors));
    }
    markingSet.EnsureSpecies(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), new Color?(profile.Appearance.SkinColor), this._markingManager, this._prototypeManager);
    markingSet.EnsureSexes(profile.Sex, this._markingManager);
    markingSet.EnsureDefault(new Color?(profile.Appearance.SkinColor), new Color?(profile.Appearance.EyeColor), this._markingManager);
    humanoid.MarkingSet = markingSet;
    humanoid.PermanentlyHidden = new HashSet<HumanoidVisualLayers>();
    humanoid.HiddenLayers = new Dictionary<HumanoidVisualLayers, SlotFlags>();
    humanoid.CustomBaseLayers = dictionary1;
    humanoid.Sex = profile.Sex;
    humanoid.Gender = profile.Gender;
    humanoid.Age = profile.Age;
    humanoid.Species = profile.Species;
    humanoid.SkinColor = profile.Appearance.SkinColor;
    humanoid.EyeColor = profile.Appearance.EyeColor;
    this.UpdateSprite(uid, (IRMCHumanoidAppearance) humanoid, this.Comp<SpriteComponent>(uid));
  }

  private void ApplyMarkingSet(
    EntityUid entity,
    IRMCHumanoidAppearance humanoid,
    SpriteComponent sprite)
  {
    this.ClearAllMarkings(entity, humanoid, sprite);
    bool undergarmentTop;
    bool undergarmentBottom = undergarmentTop = this._configurationManager.GetCVar<bool>(CCVars.AccessibilityClientCensorNudity) || this._configurationManager.GetCVar<bool>(CCVars.AccessibilityServerCensorNudity);
    foreach (List<Marking> markingList in humanoid.MarkingSet.Markings.Values)
    {
      foreach (Marking marking in markingList)
      {
        MarkingPrototype markingResult;
        if (this._markingManager.TryGetMarking(marking, out markingResult))
        {
          this.ApplyMarking(markingResult, marking.MarkingColors, marking.Visible, entity, humanoid, sprite);
          if (markingResult.BodyPart == HumanoidVisualLayers.UndergarmentTop)
            undergarmentTop = false;
          else if (markingResult.BodyPart == HumanoidVisualLayers.UndergarmentBottom)
            undergarmentBottom = false;
        }
      }
    }
    humanoid.ClientOldMarkings = new MarkingSet(humanoid.MarkingSet);
    this.AddUndergarments(entity, humanoid, sprite, undergarmentTop, undergarmentBottom);
  }

  private void ClearAllMarkings(
    EntityUid entity,
    IRMCHumanoidAppearance humanoid,
    SpriteComponent sprite)
  {
    foreach (List<Marking> markingList in humanoid.ClientOldMarkings.Markings.Values)
    {
      foreach (Marking marking in markingList)
        this.RemoveMarking(marking, Entity<SpriteComponent>.op_Implicit((entity, sprite)));
    }
    humanoid.ClientOldMarkings.Clear();
    foreach (List<Marking> markingList in humanoid.MarkingSet.Markings.Values)
    {
      foreach (Marking marking in markingList)
        this.RemoveMarking(marking, Entity<SpriteComponent>.op_Implicit((entity, sprite)));
    }
  }

  private void RemoveMarking(Marking marking, Entity<SpriteComponent> spriteEnt)
  {
    MarkingPrototype markingResult;
    if (!this._markingManager.TryGetMarking(marking, out markingResult))
      return;
    foreach (SpriteSpecifier sprite in markingResult.Sprites)
    {
      if (sprite is SpriteSpecifier.Rsi rsi)
      {
        string str = $"{marking.MarkingId}-{rsi.RsiState}";
        int num;
        if (this._sprite.LayerMapTryGet(spriteEnt.AsNullable(), str, ref num, false))
        {
          this._sprite.LayerMapRemove(spriteEnt.AsNullable(), str);
          this._sprite.RemoveLayer(spriteEnt.AsNullable(), num, true);
        }
      }
    }
  }

  private void AddUndergarments(
    EntityUid entity,
    IRMCHumanoidAppearance humanoid,
    SpriteComponent sprite,
    bool undergarmentTop,
    bool undergarmentBottom)
  {
    if (undergarmentTop && humanoid.UndergarmentTop.HasValue)
    {
      ProtoId<MarkingPrototype>? undergarmentTop1 = humanoid.UndergarmentTop;
      Marking marking = new Marking(undergarmentTop1.HasValue ? ProtoId<MarkingPrototype>.op_Implicit(undergarmentTop1.GetValueOrDefault()) : (string) null, new List<Color>()
      {
        new Color()
      });
      MarkingPrototype markingResult;
      if (this._markingManager.TryGetMarking(marking, out markingResult))
      {
        humanoid.ClientOldMarkings.Markings.Add(MarkingCategories.UndergarmentTop, new List<Marking>()
        {
          marking
        });
        this.ApplyMarking(markingResult, (IReadOnlyList<Color>) null, true, entity, humanoid, sprite);
      }
    }
    if (!undergarmentBottom || !humanoid.UndergarmentBottom.HasValue)
      return;
    ProtoId<MarkingPrototype>? undergarmentBottom1 = humanoid.UndergarmentBottom;
    Marking marking1 = new Marking(undergarmentBottom1.HasValue ? ProtoId<MarkingPrototype>.op_Implicit(undergarmentBottom1.GetValueOrDefault()) : (string) null, new List<Color>()
    {
      new Color()
    });
    MarkingPrototype markingResult1;
    if (!this._markingManager.TryGetMarking(marking1, out markingResult1))
      return;
    humanoid.ClientOldMarkings.Markings.Add(MarkingCategories.UndergarmentBottom, new List<Marking>()
    {
      marking1
    });
    this.ApplyMarking(markingResult1, (IReadOnlyList<Color>) null, true, entity, humanoid, sprite);
  }

  private void ApplyMarking(
    MarkingPrototype markingPrototype,
    IReadOnlyList<Color>? colors,
    bool visible,
    EntityUid entity,
    IRMCHumanoidAppearance humanoid,
    SpriteComponent sprite)
  {
    int num1;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum) markingPrototype.BodyPart, ref num1, false))
      return;
    visible &= !HumanoidAppearanceSystem.IsHidden(humanoid, markingPrototype.BodyPart);
    HumanoidSpeciesSpriteLayer speciesSpriteLayer;
    visible = ((visible ? 1 : 0) & (!humanoid.BaseLayers.TryGetValue(markingPrototype.BodyPart, out speciesSpriteLayer) ? 0 : (speciesSpriteLayer.AllowsMarkings ? 1 : 0))) != 0;
    for (int index = 0; index < markingPrototype.Sprites.Count; ++index)
    {
      SpriteSpecifier sprite1 = markingPrototype.Sprites[index];
      if (sprite1 is SpriteSpecifier.Rsi rsi)
      {
        string key = $"{markingPrototype.ID}-{rsi.RsiState}";
        int num2;
        if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), key, ref num2, false))
        {
          int num3 = this._sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((entity, sprite)), sprite1, new int?(num1 + index + 1));
          this._sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), key, num3);
          this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((entity, sprite)), key, (SpriteSpecifier) rsi);
        }
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity, sprite)), key, visible);
        if (visible && speciesSpriteLayer != null)
        {
          if (colors != null && index < colors.Count)
            this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((entity, sprite)), key, colors[index]);
          else
            this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((entity, sprite)), key, Color.White);
          DisplacementData data;
          if (humanoid.MarkingsDisplacement.TryGetValue(markingPrototype.BodyPart, out data) && markingPrototype.CanBeDisplaced)
            this._displacement.TryAddDisplacement(data, Entity<SpriteComponent>.op_Implicit((entity, sprite)), num1 + index + 1, (object) key, out string _);
        }
      }
    }
  }

  public override void SetSkinColor(
    EntityUid uid,
    Color skinColor,
    bool sync = true,
    bool verify = true,
    HumanoidAppearanceComponent? humanoid = null)
  {
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || Color.op_Equality(humanoid.SkinColor, skinColor))
      return;
    base.SetSkinColor(uid, skinColor, false, verify, humanoid);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    foreach ((HumanoidVisualLayers key, HumanoidSpeciesSpriteLayer speciesSpriteLayer) in humanoid.BaseLayers)
    {
      if (speciesSpriteLayer.MatchSkin)
      {
        int num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) key);
        spriteComponent[num].Color = ((Color) ref skinColor).WithAlpha(speciesSpriteLayer.LayerAlpha);
      }
    }
  }

  public override void SetLayerVisibility(
    Entity<HumanoidAppearanceComponent> ent,
    HumanoidVisualLayers layer,
    bool visible,
    SlotFlags? slot,
    ref bool dirty)
  {
    base.SetLayerVisibility(ent, layer, visible, slot, ref dirty);
    SpriteComponent sprite = this.Comp<SpriteComponent>(Entity<HumanoidAppearanceComponent>.op_Implicit(ent));
    int num;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) layer, ref num, false))
    {
      if (!visible)
        return;
      num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) layer);
    }
    ISpriteLayer ispriteLayer = sprite[num];
    if (ispriteLayer.Visible == visible)
      return;
    ispriteLayer.Visible = visible;
    IRMCHumanoidAppearance humanoid = (IRMCHumanoidAppearance) ent.Comp;
    IRMCHumanoidAppearance appearance;
    if (this._rmcHumanoid.TryGetLocalHiddenAppearance(Entity<HumanoidAppearanceComponent>.op_Implicit(ent), out appearance))
      humanoid = appearance;
    foreach (List<Marking> markingList in ent.Comp.MarkingSet.Markings.Values)
    {
      foreach (Marking marking in markingList)
      {
        MarkingPrototype markingResult;
        if (this._markingManager.TryGetMarking(marking, out markingResult) && markingResult.BodyPart == layer)
          this.ApplyMarking(markingResult, marking.MarkingColors, marking.Visible, Entity<HumanoidAppearanceComponent>.op_Implicit(ent), humanoid, sprite);
      }
    }
  }

  private void UpdateHiddenSprites<T>(T ev)
  {
    AllEntityQueryEnumerator<HiddenAppearanceComponent, HumanoidAppearanceComponent, SpriteComponent> entityQueryEnumerator = this.AllEntityQuery<HiddenAppearanceComponent, HumanoidAppearanceComponent, SpriteComponent>();
    EntityUid entityUid;
    HiddenAppearanceComponent appearanceComponent;
    HumanoidAppearanceComponent humanoid;
    SpriteComponent sprite;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref appearanceComponent, ref humanoid, ref sprite))
    {
      this.UpdateSprite(entityUid, (IRMCHumanoidAppearance) humanoid, sprite);
      this.UpdatePlayerMedals(entityUid);
    }
  }

  private void OnHiddenRemove(Entity<HiddenAppearanceComponent> ent, ref ComponentRemove args)
  {
    HumanoidAppearanceComponent humanoid;
    SpriteComponent sprite;
    if (!this.TryComp<HumanoidAppearanceComponent>(Entity<HiddenAppearanceComponent>.op_Implicit(ent), ref humanoid) || !this.TryComp<SpriteComponent>(Entity<HiddenAppearanceComponent>.op_Implicit(ent), ref sprite))
      return;
    this.UpdateSprite(Entity<HiddenAppearanceComponent>.op_Implicit(ent), (IRMCHumanoidAppearance) humanoid, sprite);
    this.UpdatePlayerMedals(Entity<HiddenAppearanceComponent>.op_Implicit(ent));
  }

  private void UpdatePlayerMedals(EntityUid player)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(player));
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      EntityUid? containedEntity = container.ContainedEntity;
      if (containedEntity.HasValue)
        this._item.VisualsChanged(containedEntity.GetValueOrDefault());
    }
  }
}
