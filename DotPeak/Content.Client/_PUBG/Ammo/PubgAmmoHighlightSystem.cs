// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Ammo.PubgAmmoHighlightSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Interactable.Components;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Storage;
using Content.Shared.Tag;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Ammo;

public sealed class PubgAmmoHighlightSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private TagSystem _tags;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private IGameTiming _timing;
  private readonly Dictionary<EntityUid, ShaderInstance> _highlightShaders = new Dictionary<EntityUid, ShaderInstance>();
  private readonly Dictionary<EntityUid, ShaderInstance?> _previousShaders = new Dictionary<EntityUid, ShaderInstance>();
  private readonly HashSet<string> _ammoTags = new HashSet<string>();
  private readonly List<EntityUid> _removeScratch = new List<EntityUid>();
  private TimeSpan _nextUpdate;
  private static readonly Color HighlightColor = Color.FromHex((ReadOnlySpan<char>) "#FFD54F", new Color?());
  private const float HighlightWidth = 2f;
  private const float UpdateInterval = 0.5f;

  private static ShaderInstance? GetValidShader(ShaderInstance? shader)
  {
    return shader == null || shader.Disposed ? (ShaderInstance) null : shader;
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.ClearOutlines();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._timing.CurTime < this._nextUpdate)
      return;
    this._nextUpdate = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
    {
      this.ClearOutlines();
    }
    else
    {
      this.BuildAmmoTags(localEntity.Value);
      if (this._ammoTags.Count == 0)
        this.ClearOutlines();
      else
        this.ApplyOutlines(localEntity.Value);
    }
  }

  private void BuildAmmoTags(EntityUid local)
  {
    this._ammoTags.Clear();
    HandsComponent handsComponent;
    if (this.TryComp<HandsComponent>(local, ref handsComponent))
    {
      foreach (EntityUid entity in this._hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((local, handsComponent))))
        this.AddAmmoTag(entity);
    }
    EntityUid? entityUid1;
    if (this._inventory.TryGetSlotEntity(local, "back", out entityUid1))
    {
      this.AddAmmoTag(entityUid1.Value);
      StorageComponent storageComponent;
      if (this.TryComp<StorageComponent>(entityUid1.Value, ref storageComponent) && storageComponent.Container != null)
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) ((BaseContainer) storageComponent.Container).ContainedEntities)
          this.AddAmmoTag(containedEntity);
      }
    }
    EntityUid? entityUid2;
    if (!this._inventory.TryGetSlotEntity(local, "suitstorage", out entityUid2))
      return;
    this.AddAmmoTag(entityUid2.Value);
  }

  private void AddAmmoTag(EntityUid entity)
  {
    PubgAmmoProviderComponent providerComponent;
    if (!this.TryComp<PubgAmmoProviderComponent>(entity, ref providerComponent) || string.IsNullOrWhiteSpace(providerComponent.AmmoTag))
      return;
    this._ammoTags.Add(providerComponent.AmmoTag);
  }

  private void ApplyOutlines(EntityUid local)
  {
    HashSet<EntityUid> keep = new HashSet<EntityUid>();
    MapId mapId = this.Transform(local).MapID;
    EntityQueryEnumerator<TagComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TagComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    TagComponent tagComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref tagComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, mapId) && !this._containers.IsEntityInContainer(entityUid, (MetaDataComponent) null) && this.HasMatchingAmmoTag(entityUid))
      {
        keep.Add(entityUid);
        this.EnsureComp<InteractionOutlineComponent>(entityUid);
        ShaderInstance shaderInstance1 = (ShaderInstance) null;
        bool flag = !this._highlightShaders.TryGetValue(entityUid, out shaderInstance1) || spriteComponent.PostShader != shaderInstance1 || shaderInstance1 != null && shaderInstance1.Disposed;
        if (!flag)
        {
          if (shaderInstance1 != null)
          {
            try
            {
              shaderInstance1.SetParameter("outline_color", PubgAmmoHighlightSystem.HighlightColor);
            }
            catch
            {
              flag = true;
            }
          }
        }
        if (flag)
        {
          if (shaderInstance1 != null)
          {
            try
            {
              shaderInstance1.Dispose();
            }
            catch
            {
            }
          }
          ShaderInstance validShader = PubgAmmoHighlightSystem.GetValidShader(spriteComponent.PostShader);
          if (validShader != null)
            this._previousShaders.TryAdd(entityUid, validShader);
          else
            this._previousShaders.Remove(entityUid);
          ShaderInstance shaderInstance2 = this._prototypes.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCAuraOutline")).InstanceUnique();
          this._highlightShaders[entityUid] = shaderInstance2;
          spriteComponent.PostShader = shaderInstance2;
          shaderInstance2.SetParameter("outline_color", PubgAmmoHighlightSystem.HighlightColor);
          shaderInstance2.SetParameter("outline_width", 2f);
        }
        else
          shaderInstance1?.SetParameter("outline_width", 2f);
      }
    }
    this.ClearMissing(keep);
  }

  private bool HasMatchingAmmoTag(EntityUid uid)
  {
    foreach (string ammoTag in this._ammoTags)
    {
      if (this._tags.HasTag(uid, ProtoId<TagPrototype>.op_Implicit(ammoTag)))
        return true;
    }
    return false;
  }

  private void ClearMissing(HashSet<EntityUid> keep)
  {
    this._removeScratch.Clear();
    foreach (EntityUid key in this._highlightShaders.Keys)
    {
      if (!keep.Contains(key))
        this._removeScratch.Add(key);
    }
    foreach (EntityUid uid in this._removeScratch)
      this.RestoreOutline(uid);
  }

  private void ClearOutlines()
  {
    this._removeScratch.Clear();
    this._removeScratch.AddRange((IEnumerable<EntityUid>) this._highlightShaders.Keys);
    foreach (EntityUid uid in this._removeScratch)
      this.RestoreOutline(uid);
  }

  private void RestoreOutline(EntityUid uid)
  {
    ShaderInstance shaderInstance;
    if (this._highlightShaders.TryGetValue(uid, out shaderInstance))
    {
      SpriteComponent spriteComponent;
      if (this.Exists(uid) && this.TryComp<SpriteComponent>(uid, ref spriteComponent) && spriteComponent.PostShader == shaderInstance)
      {
        ShaderInstance shader;
        spriteComponent.PostShader = !this._previousShaders.TryGetValue(uid, out shader) || PubgAmmoHighlightSystem.GetValidShader(shader) == null ? (ShaderInstance) null : shader;
      }
      try
      {
        shaderInstance.Dispose();
      }
      catch
      {
      }
    }
    this._highlightShaders.Remove(uid);
    this._previousShaders.Remove(uid);
  }
}
