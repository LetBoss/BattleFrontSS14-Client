// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Storage.RMCIconLabelSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.IconLabel;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;
using System.Text;

#nullable enable
namespace Content.Client._RMC14.Storage;

public sealed class RMCIconLabelSystem : SharedRMCIconLabelSystem
{
  [Dependency]
  private IResourceCache _cache;
  [Dependency]
  private IConfigurationManager _config;
  private Font _font;
  private bool _drawStorageIconLabels;

  public virtual void Initialize()
  {
    this._font = (Font) new VectorFont(this._cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._config, RMCCVars.RMCDrawStorageIconLabels, (Action<bool>) (v => this._drawStorageIconLabels = v), true);
  }

  public void DrawStorage(
    EntityUid entity,
    float uiScale,
    Vector2 iconPosition,
    DrawingHandleScreen handle)
  {
    IconLabelComponent iconLabelComponent;
    if (!this._drawStorageIconLabels || !this.TryComp<IconLabelComponent>(entity, ref iconLabelComponent))
      return;
    float num1 = 2f * uiScale;
    if (!iconLabelComponent.LabelTextLocId.HasValue)
      return;
    ILocalizationManager loc = this.Loc;
    LocId? labelTextLocId = iconLabelComponent.LabelTextLocId;
    string str1 = labelTextLocId.HasValue ? LocId.op_Implicit(labelTextLocId.GetValueOrDefault()) : (string) null;
    string str2;
    ref string local = ref str2;
    (string, object)[] array = iconLabelComponent.LabelTextParams.ToArray();
    if (!loc.TryGetString(str1, ref local, array))
      return;
    if (str2.Length > iconLabelComponent.LabelMaxSize)
      str2 = str2.Substring(0, iconLabelComponent.LabelMaxSize);
    Color color;
    Color.TryFromName(iconLabelComponent.TextColor, ref color);
    char[] charArray = str2.ToCharArray();
    Vector2 vector2 = new Vector2(iconPosition.X + num1 * (float) iconLabelComponent.StoredOffset.X, iconPosition.Y + num1 * (float) iconLabelComponent.StoredOffset.Y);
    int textSize = iconLabelComponent.TextSize;
    float num2 = 0.0f;
    foreach (char ch in charArray)
    {
      vector2.X += num2;
      num2 = this._font.DrawChar((DrawingHandleBase) handle, new Rune(ch), vector2, (float) textSize * num1, color, true);
    }
  }
}
