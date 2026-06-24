// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Party.PubgPartyWorldOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Interactable.Components;
using Content.Shared._PUBG.Party;
using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.Party;

public sealed class PubgPartyWorldOverlaySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private PubgPartyClientSystem _party;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private IConfigurationManager _cfg;
  private readonly Dictionary<EntityUid, ShaderInstance> _partyShaders = new Dictionary<EntityUid, ShaderInstance>();
  private readonly Dictionary<EntityUid, ShaderInstance?> _previousShaders = new Dictionary<EntityUid, ShaderInstance>();
  private readonly HashSet<EntityUid> _keep = new HashSet<EntityUid>();
  private readonly List<EntityUid> _removeScratch = new List<EntityUid>();
  private PubgPartySpriteOverlay? _overlay;
  private bool _markersEnabled = true;
  private float _markersOpacity = 1f;

  private static ShaderInstance? GetValidShader(ShaderInstance? shader)
  {
    return shader == null || shader.Disposed ? (ShaderInstance) null : shader;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay = new PubgPartySpriteOverlay((IEntityManager) this.EntityManager, this._player, this._party);
    this._party.PartyStateUpdated += new Action(this.OnPartyStateUpdated);
    this._cfg.OnValueChanged<bool>(CCVars.PubgPartyMarkersEnabled, new Action<bool>(this.OnMarkersEnabledChanged), true);
    this._cfg.OnValueChanged<float>(CCVars.PubgPartyMarkersOpacity, new Action<float>(this.OnMarkersOpacityChanged), true);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._party.PartyStateUpdated -= new Action(this.OnPartyStateUpdated);
    this.ClearOutlines();
    this.RemoveOverlay();
  }

  private void OnPartyStateUpdated()
  {
    if (!this._markersEnabled)
    {
      this.ClearOutlines();
      this.RemoveOverlay();
    }
    else
    {
      this.ApplyOutlines();
      this.UpdateOverlay();
    }
  }

  private void OnMarkersEnabledChanged(bool enabled)
  {
    this._markersEnabled = enabled;
    if (!enabled)
    {
      this.ClearOutlines();
      this.RemoveOverlay();
    }
    else
    {
      this.ApplyOutlines();
      this.UpdateOverlay();
    }
  }

  private void OnMarkersOpacityChanged(float opacity)
  {
    this._markersOpacity = Math.Clamp(opacity, 0.0f, 1f);
    if (this._overlay != null)
      this._overlay.Opacity = this._markersOpacity;
    this.ApplyOutlines();
  }

  private void UpdateOverlay()
  {
    if (this._overlay == null)
      return;
    this._overlay.Opacity = this._markersOpacity;
    if (this._party.Members.Count <= 1)
    {
      this.RemoveOverlay();
    }
    else
    {
      if (this._overlayManager.HasOverlay<PubgPartySpriteOverlay>())
        return;
      this._overlayManager.AddOverlay((Overlay) this._overlay);
    }
  }

  private void RemoveOverlay()
  {
    if (!this._overlayManager.HasOverlay<PubgPartySpriteOverlay>())
      return;
    this._overlayManager.RemoveOverlay<PubgPartySpriteOverlay>();
  }

  private void ApplyOutlines()
  {
    if (!this._markersEnabled)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    NetEntity netEntity = localEntity.HasValue ? this.GetNetEntity(localEntity.Value, (MetaDataComponent) null) : new NetEntity();
    HashSet<EntityUid> keep = this._keep;
    keep.Clear();
    foreach (PubgPartyMemberState member in (IEnumerable<PubgPartyMemberState>) this._party.Members)
    {
      if (!NetEntity.op_Equality(member.Entity, netEntity))
      {
        EntityUid entity = this.GetEntity(member.Entity);
        SpriteComponent spriteComponent;
        if (this.Exists(entity) && this.TryComp<SpriteComponent>(entity, ref spriteComponent))
        {
          keep.Add(entity);
          this.EnsureComp<InteractionOutlineComponent>(entity);
          ShaderInstance shaderInstance = (ShaderInstance) null;
          bool flag = !this._partyShaders.TryGetValue(entity, out shaderInstance) || spriteComponent.PostShader != shaderInstance || shaderInstance != null && shaderInstance.Disposed;
          Color partyColor = PubgPartyWorldOverlaySystem.GetPartyColor(member.SlotIndex);
          Color color = ((Color) ref partyColor).WithAlpha(this._markersOpacity);
          if (!flag)
          {
            if (shaderInstance != null)
            {
              try
              {
                shaderInstance.SetParameter("outline_color", color);
              }
              catch
              {
                flag = true;
              }
            }
          }
          if (flag)
          {
            if (shaderInstance != null)
            {
              try
              {
                shaderInstance.Dispose();
              }
              catch
              {
              }
            }
            ShaderInstance validShader = PubgPartyWorldOverlaySystem.GetValidShader(spriteComponent.PostShader);
            if (validShader != null)
              this._previousShaders.TryAdd(entity, validShader);
            else
              this._previousShaders.Remove(entity);
            shaderInstance = this._prototypes.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCAuraOutline")).InstanceUnique();
            this._partyShaders[entity] = shaderInstance;
            spriteComponent.PostShader = shaderInstance;
            shaderInstance.SetParameter("outline_color", color);
            shaderInstance.SetParameter("outline_width", 2f);
          }
          else
            shaderInstance?.SetParameter("outline_width", 2f);
        }
      }
    }
    this.ClearMissing(keep);
  }

  private void ClearMissing(HashSet<EntityUid> keep)
  {
    this._removeScratch.Clear();
    foreach (EntityUid key in this._partyShaders.Keys)
    {
      if (!keep.Contains(key))
        this._removeScratch.Add(key);
    }
    foreach (EntityUid uid in this._removeScratch)
      this.RestoreOutline(uid);
  }

  private void ClearOutlines()
  {
    foreach (EntityUid uid in this._partyShaders.Keys.ToList<EntityUid>())
      this.RestoreOutline(uid);
  }

  private void RestoreOutline(EntityUid uid)
  {
    ShaderInstance shaderInstance;
    if (this._partyShaders.TryGetValue(uid, out shaderInstance))
    {
      SpriteComponent spriteComponent;
      if (this.Exists(uid) && this.TryComp<SpriteComponent>(uid, ref spriteComponent) && spriteComponent.PostShader == shaderInstance)
      {
        ShaderInstance shader;
        spriteComponent.PostShader = !this._previousShaders.TryGetValue(uid, out shader) || PubgPartyWorldOverlaySystem.GetValidShader(shader) == null ? (ShaderInstance) null : shader;
      }
      try
      {
        shaderInstance.Dispose();
      }
      catch
      {
      }
    }
    this._partyShaders.Remove(uid);
    this._previousShaders.Remove(uid);
  }

  private static Color GetPartyColor(int slotIndex)
  {
    Color partyColor;
    switch (slotIndex)
    {
      case 1:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#00bcd4", new Color?());
        break;
      case 2:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#ffeb3b", new Color?());
        break;
      case 3:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#ff9800", new Color?());
        break;
      default:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#4caf50", new Color?());
        break;
    }
    return partyColor;
  }
}
