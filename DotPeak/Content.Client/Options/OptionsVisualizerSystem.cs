// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.OptionsVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using System;

#nullable enable
namespace Content.Client.Options;

public sealed class OptionsVisualizerSystem : EntitySystem
{
  private static readonly (OptionVisualizerOptions, CVarDef<bool>)[] OptionVars = new (OptionVisualizerOptions, CVarDef<bool>)[2]
  {
    (OptionVisualizerOptions.Test, CCVars.DebugOptionVisualizerTest),
    (OptionVisualizerOptions.ReducedMotion, CCVars.ReducedMotion)
  };
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IReflectionManager _reflection;
  [Dependency]
  private SpriteSystem _sprite;
  private OptionVisualizerOptions _currentOptions;

  public virtual void Initialize()
  {
    base.Initialize();
    foreach ((OptionVisualizerOptions, CVarDef<bool>) optionVar in OptionsVisualizerSystem.OptionVars)
      EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._cfg, optionVar.Item2, (Action<bool>) (_ => this.CVarChanged()), false);
    this.UpdateActiveOptions();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<OptionsVisualizerComponent, ComponentStartup>(new ComponentEventHandler<OptionsVisualizerComponent, ComponentStartup>((object) this, __methodptr(OnComponentStartup)), (Type[]) null, (Type[]) null);
  }

  private void CVarChanged()
  {
    this.UpdateActiveOptions();
    this.UpdateAllComponents();
  }

  private void UpdateActiveOptions()
  {
    this._currentOptions = OptionVisualizerOptions.Default;
    foreach ((OptionVisualizerOptions, CVarDef<bool>) optionVar in OptionsVisualizerSystem.OptionVars)
    {
      OptionVisualizerOptions visualizerOptions = optionVar.Item1;
      if (this._cfg.GetCVar<bool>(optionVar.Item2))
        this._currentOptions |= visualizerOptions;
    }
  }

  private void UpdateAllComponents()
  {
    EntityQueryEnumerator<OptionsVisualizerComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<OptionsVisualizerComponent, SpriteComponent>();
    EntityUid uid;
    OptionsVisualizerComponent component;
    SpriteComponent sprite;
    while (entityQueryEnumerator.MoveNext(ref uid, ref component, ref sprite))
      this.UpdateComponent(uid, component, sprite);
  }

  private void OnComponentStartup(
    EntityUid uid,
    OptionsVisualizerComponent component,
    ComponentStartup args)
  {
    SpriteComponent sprite;
    if (!this.TryComp<SpriteComponent>(uid, ref sprite))
      return;
    this.UpdateComponent(uid, component, sprite);
  }

  private void UpdateComponent(
    EntityUid uid,
    OptionsVisualizerComponent component,
    SpriteComponent sprite)
  {
    foreach ((string key, OptionsVisualizerComponent.LayerDatum[] layerDatumArray1) in component.Visuals)
    {
      OptionsVisualizerComponent.LayerDatum[] layerDatumArray2 = layerDatumArray1;
      OptionsVisualizerComponent.LayerDatum layerDatum1 = (OptionsVisualizerComponent.LayerDatum) null;
      layerDatumArray1 = layerDatumArray2;
      for (int index = 0; index < layerDatumArray1.Length; ++index)
      {
        OptionsVisualizerComponent.LayerDatum layerDatum2 = layerDatumArray1[index];
        if ((layerDatum2.Options & this._currentOptions) == layerDatum2.Options)
          layerDatum1 = layerDatum2;
      }
      if (layerDatum1 != null)
      {
        Enum @enum;
        int num = this._reflection.TryParseEnumReference(key, ref @enum, true) ? this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), @enum) : this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key);
        this._sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, layerDatum1.Data);
      }
    }
  }
}
