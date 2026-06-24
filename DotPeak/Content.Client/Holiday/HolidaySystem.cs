// Decompiled with JetBrains decompiler
// Type: Content.Client.Holiday.HolidaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Holiday;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Holiday;

public sealed class HolidaySystem : EntitySystem
{
  [Dependency]
  private IResourceCache _rescache;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HolidayRsiSwapComponent, AppearanceChangeEvent>(new EntityEventRefHandler<HolidayRsiSwapComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    Entity<HolidayRsiSwapComponent> ent,
    ref AppearanceChangeEvent args)
  {
    string key;
    string str;
    RSIResource rsiResource;
    if (!this._appearance.TryGetData<string>(Entity<HolidayRsiSwapComponent>.op_Implicit(ent), (Enum) HolidayVisuals.Holiday, ref key, args.Component) || !ent.Comp.Sprite.TryGetValue(key, out str) || args.Sprite == null || !this._rescache.TryGetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, str), ref rsiResource))
      return;
    this._sprite.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, args.Sprite)), rsiResource.RSI);
  }
}
