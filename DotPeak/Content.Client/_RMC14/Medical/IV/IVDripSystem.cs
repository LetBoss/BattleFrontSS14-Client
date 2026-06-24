// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.IV.IVDripSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Medical.IV;
using Content.Shared.Rounding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Medical.IV;

public sealed class IVDripSystem : SharedIVDripSystem
{
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    if (this._overlay.HasOverlay<IVDripOverlay>())
      return;
    this._overlay.AddOverlay((Overlay) new IVDripOverlay());
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlay.RemoveOverlay<IVDripOverlay>();
  }

  protected override void UpdateIVAppearance(Entity<IVDripComponent> iv)
  {
    base.UpdateIVAppearance(iv);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<IVDripComponent>.op_Implicit(iv), ref spriteComponent))
      return;
    string str1 = !iv.Comp.AttachedTo.HasValue ? iv.Comp.UnattachedState : iv.Comp.AttachedState;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((iv.Owner, spriteComponent)), (Enum) IVDripVisualLayers.Base, RSI.StateId.op_Implicit(str1));
    string str2 = (string) null;
    for (int index = iv.Comp.ReagentStates.Count - 1; index >= 0; --index)
    {
      (int Percentage, string State) = iv.Comp.ReagentStates[index];
      if (Percentage <= iv.Comp.FillPercentage)
      {
        str2 = State;
        break;
      }
    }
    if (str2 == null)
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((iv.Owner, spriteComponent)), (Enum) IVDripVisualLayers.Reagent, false);
    }
    else
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((iv.Owner, spriteComponent)), (Enum) IVDripVisualLayers.Reagent, true);
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((iv.Owner, spriteComponent)), (Enum) IVDripVisualLayers.Reagent, RSI.StateId.op_Implicit(str2));
      this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((iv.Owner, spriteComponent)), (Enum) IVDripVisualLayers.Reagent, iv.Comp.FillColor);
    }
  }

  protected override void UpdatePackAppearance(Entity<BloodPackComponent> pack)
  {
    base.UpdatePackAppearance(pack);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<BloodPackComponent>.op_Implicit(pack), ref spriteComponent))
      return;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((pack.Owner, spriteComponent)), (Enum) BloodPackVisuals.Label, false);
    int num;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((pack.Owner, spriteComponent)), (Enum) BloodPackVisuals.Fill, ref num, false))
      return;
    int levels = ContentHelpers.RoundToLevels((double) pack.Comp.FillPercentage.Float(), 1.0, pack.Comp.MaxFillLevels + 1);
    string str1;
    if (levels <= 0)
      str1 = pack.Comp.FillBaseName;
    else
      str1 = $"{pack.Comp.FillBaseName}{levels}";
    string str2 = str1;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((pack.Owner, spriteComponent)), num, RSI.StateId.op_Implicit(str2));
    this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((pack.Owner, spriteComponent)), num, pack.Comp.FillColor);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((pack.Owner, spriteComponent)), num, true);
  }
}
