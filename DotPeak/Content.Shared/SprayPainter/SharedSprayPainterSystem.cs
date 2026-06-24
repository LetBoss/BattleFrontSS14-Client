// Decompiled with JetBrains decompiler
// Type: Content.Shared.SprayPainter.SharedSprayPainterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.SprayPainter.Components;
using Content.Shared.SprayPainter.Prototypes;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.SprayPainter;

public abstract class SharedSprayPainterSystem : EntitySystem
{
  [Dependency]
  protected IPrototypeManager Proto;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  protected SharedAudioSystem Audio;
  [Dependency]
  protected SharedDoAfterSystem DoAfter;
  [Dependency]
  private SharedPopupSystem _popup;
  private static readonly ProtoId<AirlockDepartmentsPrototype> Departments = (ProtoId<AirlockDepartmentsPrototype>) nameof (Departments);

  public List<AirlockStyle> Styles { get; private set; } = new List<AirlockStyle>();

  public List<AirlockGroupPrototype> Groups { get; private set; } = new List<AirlockGroupPrototype>();

  public override void Initialize()
  {
    base.Initialize();
    this.CacheStyles();
    this.SubscribeLocalEvent<SprayPainterComponent, MapInitEvent>(new EntityEventRefHandler<SprayPainterComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<SprayPainterComponent, SprayPainterDoorDoAfterEvent>(new EntityEventRefHandler<SprayPainterComponent, SprayPainterDoorDoAfterEvent>(this.OnDoorDoAfter));
    this.Subs.BuiEvents<SprayPainterComponent>((object) SprayPainterUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<SprayPainterComponent>) (subs =>
    {
      subs.Event<SprayPainterSpritePickedMessage>(new EntityEventRefHandler<SprayPainterComponent, SprayPainterSpritePickedMessage>(this.OnSpritePicked));
      subs.Event<SprayPainterColorPickedMessage>(new EntityEventRefHandler<SprayPainterComponent, SprayPainterColorPickedMessage>(this.OnColorPicked));
    }));
    this.SubscribeLocalEvent<PaintableAirlockComponent, InteractUsingEvent>(new EntityEventRefHandler<PaintableAirlockComponent, InteractUsingEvent>(this.OnAirlockInteract));
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
  }

  private void OnMapInit(Entity<SprayPainterComponent> ent, ref MapInitEvent args)
  {
    if (ent.Comp.ColorPalette.Count == 0)
      return;
    this.SetColor(ent, ent.Comp.ColorPalette.First<KeyValuePair<string, Color>>().Key);
  }

  private void OnDoorDoAfter(
    Entity<SprayPainterComponent> ent,
    ref SprayPainterDoorDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target = args.Args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    PaintableAirlockComponent comp;
    if (!this.TryComp<PaintableAirlockComponent>(valueOrDefault, out comp))
      return;
    comp.Department = (ProtoId<DepartmentPrototype>?) args.Department;
    this.Dirty(valueOrDefault, (IComponent) comp);
    this.Audio.PlayPredicted(ent.Comp.SpraySound, (EntityUid) ent, new EntityUid?(args.Args.User));
    this.Appearance.SetData(valueOrDefault, (Enum) DoorVisuals.BaseRSI, (object) args.Sprite);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(9, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Args.User), "user", "ToPrettyString(args.Args.User)");
    logStringHandler.AppendLiteral(" painted ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Args.Target.Value), "target", "ToPrettyString(args.Args.Target.Value)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    args.Handled = true;
  }

  private void OnColorPicked(
    Entity<SprayPainterComponent> ent,
    ref SprayPainterColorPickedMessage args)
  {
    this.SetColor(ent, args.Key);
  }

  private void OnSpritePicked(
    Entity<SprayPainterComponent> ent,
    ref SprayPainterSpritePickedMessage args)
  {
    if (args.Index >= this.Styles.Count)
      return;
    ent.Comp.Index = args.Index;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  private void SetColor(Entity<SprayPainterComponent> ent, string? paletteKey)
  {
    if (paletteKey == null || paletteKey == ent.Comp.PickedColor || !ent.Comp.ColorPalette.ContainsKey(paletteKey))
      return;
    ent.Comp.PickedColor = paletteKey;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  private void OnAirlockInteract(Entity<PaintableAirlockComponent> ent, ref InteractUsingEvent args)
  {
    SprayPainterComponent comp;
    if (args.Handled || !this.TryComp<SprayPainterComponent>(args.Used, out comp))
      return;
    AirlockGroupPrototype airlockGroupPrototype = this.Proto.Index<AirlockGroupPrototype>(ent.Comp.Group);
    AirlockStyle style = this.Styles[comp.Index];
    string sprite;
    if (!airlockGroupPrototype.StylePaths.TryGetValue(style.Name, out sprite))
    {
      this._popup.PopupClient(this.Loc.GetString("spray-painter-style-not-available"), args.User, new EntityUid?(args.User));
    }
    else
    {
      if (!this.DoAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, comp.AirlockSprayTime, (DoAfterEvent) new SprayPainterDoorDoAfterEvent(sprite, style.Department), new EntityUid?(args.Used), new EntityUid?((EntityUid) ent), new EntityUid?(args.Used))
      {
        BreakOnMove = true,
        BreakOnDamage = true,
        NeedHand = true
      }, out DoAfterId? _))
        return;
      args.Handled = true;
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(23, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" is painting ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "target", "ToPrettyString(ent)");
      logStringHandler.AppendLiteral(" to '");
      logStringHandler.AppendFormatted(style.Name);
      logStringHandler.AppendLiteral("' at ");
      logStringHandler.AppendFormatted<EntityCoordinates>(this.Transform((EntityUid) ent).Coordinates, "targetlocation", "Transform(ent).Coordinates");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    }
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
  {
    if (!args.WasModified<AirlockGroupPrototype>() && !args.WasModified<AirlockDepartmentsPrototype>())
      return;
    this.Styles.Clear();
    this.Groups.Clear();
    this.CacheStyles();
    int num = this.Styles.Count - 1;
    AllEntityQueryEnumerator<SprayPainterComponent> entityQueryEnumerator = this.AllEntityQuery<SprayPainterComponent>();
    EntityUid uid;
    SprayPainterComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Index > num)
      {
        comp1.Index = num;
        this.Dirty(uid, (IComponent) comp1);
      }
    }
  }

  protected virtual void CacheStyles()
  {
    SortedSet<string> sortedSet = new SortedSet<string>();
    foreach (AirlockGroupPrototype enumeratePrototype in this.Proto.EnumeratePrototypes<AirlockGroupPrototype>())
    {
      this.Groups.Add(enumeratePrototype);
      foreach (string key in enumeratePrototype.StylePaths.Keys)
        sortedSet.Add(key);
    }
    AirlockDepartmentsPrototype departmentsPrototype = this.Proto.Index<AirlockDepartmentsPrototype>(SharedSprayPainterSystem.Departments);
    this.Styles.Capacity = sortedSet.Count;
    foreach (string str in sortedSet)
    {
      ProtoId<DepartmentPrototype> Department;
      departmentsPrototype.Departments.TryGetValue(str, out Department);
      this.Styles.Add(new AirlockStyle(str, (string) Department));
    }
  }
}
