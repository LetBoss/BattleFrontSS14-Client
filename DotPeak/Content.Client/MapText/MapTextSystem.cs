// Decompiled with JetBrains decompiler
// Type: Content.Client.MapText.MapTextSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.MapText;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.MapText;

public sealed class MapTextSystem : SharedMapTextSystem
{
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IOverlayManager _overlayManager;
  private MapTextOverlay _overlay;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MapTextComponent, ComponentStartup>(new EntityEventRefHandler<MapTextComponent, ComponentStartup>((object) this, __methodptr(OnComponentStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MapTextComponent, ComponentHandleState>(new EntityEventRefHandler<MapTextComponent, ComponentHandleState>((object) this, __methodptr(HandleCompState)), (Type[]) null, (Type[]) null);
    this._overlay = new MapTextOverlay(this._configManager, (IEntityManager) this.EntityManager, this._uiManager, this._transform, this._resourceCache, this._prototypeManager);
    this._overlayManager.AddOverlay((Overlay) this._overlay);
  }

  private void OnComponentStartup(Entity<MapTextComponent> ent, ref ComponentStartup args)
  {
    this.CacheText(ent.Comp);
  }

  private void HandleCompState(Entity<MapTextComponent> ent, ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is MapTextComponentState current))
      return;
    ent.Comp.Text = current.Text;
    ent.Comp.LocText = current.LocText;
    ent.Comp.Color = current.Color;
    ent.Comp.FontId = current.FontId;
    ent.Comp.FontSize = current.FontSize;
    ent.Comp.Offset = current.Offset;
    this.CacheText(ent.Comp);
  }

  private void CacheText(MapTextComponent component)
  {
    component.CachedFont = (VectorFont) null;
    component.CachedText = string.IsNullOrWhiteSpace(component.Text) ? this.Loc.GetString(LocId.op_Implicit(component.LocText)) : component.Text;
    FontPrototype fontPrototype1;
    if (!this._prototypeManager.TryIndex<FontPrototype>(component.FontId, ref fontPrototype1))
    {
      component.CachedText = this.Loc.GetString("map-text-font-error");
      component.Color = Color.Red;
      FontPrototype fontPrototype2;
      if (!this._prototypeManager.TryIndex<FontPrototype>("Default", ref fontPrototype2))
        return;
      component.CachedFont = new VectorFont(this._resourceCache.GetResource<FontResource>(fontPrototype2.Path, true), 14);
    }
    else
    {
      FontResource resource = this._resourceCache.GetResource<FontResource>(fontPrototype1.Path, true);
      component.CachedFont = new VectorFont(resource, component.FontSize);
    }
  }
}
