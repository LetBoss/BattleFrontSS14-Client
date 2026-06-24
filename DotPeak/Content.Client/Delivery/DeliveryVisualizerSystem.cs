// Decompiled with JetBrains decompiler
// Type: Content.Client.Delivery.DeliveryVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Delivery;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Delivery;

public sealed class DeliveryVisualizerSystem : VisualizerSystem<DeliveryComponent>
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IPrototypeManager _prototype;
  private static readonly ProtoId<JobIconPrototype> UnknownIcon = ProtoId<JobIconPrototype>.op_Implicit("JobIconUnknown");

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    DeliveryComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    string str;
    this._appearance.TryGetData<string>(uid, (Enum) DeliveryVisuals.JobIcon, ref str, args.Component);
    if (string.IsNullOrEmpty(str))
      str = ProtoId<JobIconPrototype>.op_Implicit(DeliveryVisualizerSystem.UnknownIcon);
    JobIconPrototype jobIconPrototype;
    if (!this._prototype.TryIndex<JobIconPrototype>(str, ref jobIconPrototype))
      this.SpriteSystem.LayerSetTexture(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DeliveryVisualLayers.JobStamp, this.SpriteSystem.Frame0(this._prototype.Index<JobIconPrototype>(DeliveryVisualizerSystem.UnknownIcon).Icon));
    else
      this.SpriteSystem.LayerSetTexture(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DeliveryVisualLayers.JobStamp, this.SpriteSystem.Frame0(jobIconPrototype.Icon));
  }
}
