// Decompiled with JetBrains decompiler
// Type: Content.Client.Stack.StackSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items;
using Content.Client.Storage.Systems;
using Content.Shared.Stacks;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.Stack;

public sealed class StackSystem : SharedStackSystem
{
  [Dependency]
  private AppearanceSystem _appearanceSystem;
  [Dependency]
  private ItemCounterSystem _counterSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StackComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<StackComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
    this.Subs.ItemStatus<StackComponent>((Func<Entity<StackComponent>, Control>) (ent => (Control) new StackStatusControl(Entity<StackComponent>.op_Implicit(ent))));
  }

  public override void SetCount(EntityUid uid, int amount, StackComponent? component = null)
  {
    if (!this.Resolve<StackComponent>(uid, ref component, false))
      return;
    base.SetCount(uid, amount, component);
    SpriteComponent spriteComponent;
    if (component.Lingering && this.TryComp<SpriteComponent>(uid, ref spriteComponent))
    {
      Color color1;
      if (component.Count != 0 || !component.Lingering)
      {
        color1 = Color.White;
      }
      else
      {
        Color darkGray = Color.DarkGray;
        color1 = ((Color) ref darkGray).WithAlpha(0.65f);
      }
      Color color2 = color1;
      for (int index = 0; index < spriteComponent.AllLayers.Count<ISpriteLayer>(); ++index)
        this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), index, color2);
    }
    if (component.Count <= 0 && !component.Lingering)
      this.Xform.DetachEntity(uid, this.Transform(uid));
    else
      component.UiUpdateNeeded = true;
  }

  private void OnAppearanceChange(
    EntityUid uid,
    StackComponent comp,
    ref AppearanceChangeEvent args)
  {
    int actual;
    if (args.Sprite == null || comp.LayerStates.Count < 1 || !((SharedAppearanceSystem) this._appearanceSystem).TryGetData<int>(uid, (Enum) StackVisuals.Actual, ref actual, args.Component))
      return;
    int count;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<int>(uid, (Enum) StackVisuals.MaxCount, ref count, args.Component))
      count = comp.LayerStates.Count;
    bool hide;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<bool>(uid, (Enum) StackVisuals.Hide, ref hide, args.Component))
      hide = false;
    if (comp.LayerFunction != StackLayerFunction.None)
      this.ApplyLayerFunction(Entity<StackComponent>.op_Implicit((uid, comp)), ref actual, ref count);
    if (comp.IsComposite)
      this._counterSystem.ProcessCompositeSprite(uid, actual, count, comp.LayerStates, hide, args.Sprite);
    else
      this._counterSystem.ProcessOpaqueSprite(uid, comp.BaseLayer, actual, count, comp.LayerStates, hide, args.Sprite);
  }

  private bool ApplyLayerFunction(Entity<StackComponent> ent, ref int actual, ref int maxCount)
  {
    StackLayerThresholdComponent comp;
    if (ent.Comp.LayerFunction != StackLayerFunction.Threshold || !this.TryComp<StackLayerThresholdComponent>(Entity<StackComponent>.op_Implicit(ent), ref comp))
      return false;
    StackSystem.ApplyThreshold(comp, ref actual, ref maxCount);
    return true;
  }

  private static void ApplyThreshold(
    StackLayerThresholdComponent comp,
    ref int actual,
    ref int maxCount)
  {
    maxCount = Math.Min(comp.Thresholds.Count + 1, maxCount);
    int num = 0;
    foreach (int threshold in comp.Thresholds)
    {
      if (actual >= threshold)
      {
        if (num < maxCount)
          ++num;
        else
          break;
      }
      else
        break;
    }
    actual = num;
  }
}
