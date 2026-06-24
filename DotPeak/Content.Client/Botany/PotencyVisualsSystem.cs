// Decompiled with JetBrains decompiler
// Type: Content.Client.Botany.PotencyVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Botany.Components;
using Content.Shared.Botany;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Botany;

public sealed class PotencyVisualsSystem : VisualizerSystem<PotencyVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PotencyVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    float num1;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<float>(uid, (Enum) ProduceVisuals.Potency, ref num1, args.Component))
      return;
    float num2 = MathHelper.Lerp(component.MinimumScale, component.MaximumScale, num1 / 100f);
    this.SpriteSystem.SetScale(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), new Vector2(num2, num2));
  }
}
