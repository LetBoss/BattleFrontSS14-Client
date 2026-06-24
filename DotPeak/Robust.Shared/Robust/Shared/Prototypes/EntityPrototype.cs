// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.EntityPrototype
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.EntitySerialization;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Prototypes;

[Robust.Shared.Prototypes.Prototype(-1)]
public sealed class EntityPrototype : IPrototype, IInheritingPrototype, ISerializationHooks
{
  private ILocalizationManager _loc;
  private static readonly Dictionary<string, string> LocPropertiesDefault = new Dictionary<string, string>();
  private const int DEFAULT_RANGE = 200;
  [DataField("loc", false, 1, false, false, null)]
  private Dictionary<string, string>? _locPropertiesSet;
  [DataField("categories", false, 1, false, false, null)]
  [Access(new Type[] {typeof (PrototypeManager)})]
  [NeverPushInheritance]
  internal HashSet<ProtoId<EntityCategoryPrototype>>? CategoriesInternal;
  [DataField("placement", false, 1, false, false, null)]
  private EntityPrototype.EntityPlacementProperties PlacementProperties = new EntityPrototype.EntityPlacementProperties();
  [DataField("components", false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public ComponentRegistry Components = new ComponentRegistry();

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  public string? SetName { get; private set; }

  [DataField("description", false, 1, false, false, null)]
  public string? SetDesc { get; private set; }

  [DataField("suffix", false, 1, false, false, null)]
  public string? SetSuffix { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlySet<EntityCategoryPrototype> Categories { get; internal set; } = (IReadOnlySet<EntityCategoryPrototype>) new HashSet<EntityCategoryPrototype>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyDictionary<string, string> LocProperties
  {
    get
    {
      return (IReadOnlyDictionary<string, string>) this._locPropertiesSet ?? (IReadOnlyDictionary<string, string>) EntityPrototype.LocPropertiesDefault;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Name => this._loc.GetEntityData(this.ID).Name;

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Description => this._loc.GetEntityData(this.ID).Desc;

  [Robust.Shared.ViewVariables.ViewVariables]
  public string? EditorSuffix => this._loc.GetEntityData(this.ID).Suffix;

  [DataField("localizationId", false, 1, false, false, null)]
  public string? CustomLocalizationID { get; private set; }

  [Access(new Type[] {typeof (PrototypeManager)})]
  public bool HideSpawnMenu { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public List<int>? MountingPoints => this.PlacementProperties.MountingPoints;

  [Robust.Shared.ViewVariables.ViewVariables]
  public string PlacementMode => this.PlacementProperties.PlacementMode;

  [Robust.Shared.ViewVariables.ViewVariables]
  public int PlacementRange => this.PlacementProperties.PlacementRange;

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2i PlacementOffset => this.PlacementProperties.PlacementOffset;

  [DataField("save", false, 1, false, false, null)]
  public bool MapSavable { get; set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<EntityPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  public EntityPrototype()
  {
    this.Components.Add("Transform", new EntityPrototype.ComponentRegistryEntry((IComponent) new TransformComponent(), new MappingDataNode()));
    this.Components.Add("MetaData", new EntityPrototype.ComponentRegistryEntry((IComponent) new MetaDataComponent(), new MappingDataNode()));
  }

  void ISerializationHooks.AfterDeserialization()
  {
    this._loc = IoCManager.Resolve<ILocalizationManager>();
  }

  [Obsolete("Pass in IComponentFactory")]
  public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : IComponent, new()
  {
    return this.TryGetComponent<T>(IoCManager.Resolve<IComponentFactory>().GetComponentName<T>(), out component);
  }

  public bool TryGetComponent<T>([NotNullWhen(true)] out T? component, IComponentFactory factory) where T : IComponent, new()
  {
    return this.TryGetComponent<T>(factory.GetComponentName<T>(), out component);
  }

  public bool TryGetComponent<T>(string name, [NotNullWhen(true)] out T? component) where T : IComponent, new()
  {
    EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
    if (!this.Components.TryGetValue(name, out componentRegistryEntry))
    {
      component = default (T);
      return false;
    }
    if (componentRegistryEntry.Component is T component1)
    {
      component = component1;
      return true;
    }
    component = default (T);
    return false;
  }

  internal static void LoadEntity(
    Entity<MetaDataComponent> ent,
    IComponentFactory factory,
    IEntityManager entityManager,
    ISerializationManager serManager,
    IEntityLoadContext? context)
  {
    (EntityUid entityUid, MetaDataComponent comp) = ent;
    EntityPrototype entityPrototype = comp.EntityPrototype;
    ISerializationContext context1 = context as ISerializationContext;
    if (entityPrototype != null)
    {
      foreach ((string str, EntityPrototype.ComponentRegistryEntry componentRegistryEntry) in (Dictionary<string, EntityPrototype.ComponentRegistryEntry>) entityPrototype.Components)
      {
        if (context == null || !context.ShouldSkipComponent(str))
        {
          IComponent component;
          IComponent data = context == null || !context.TryGetComponent(str, out component) ? componentRegistryEntry.Component : component;
          ComponentRegistration registration = factory.GetRegistration(str);
          EntityPrototype.EnsureCompExistsAndDeserialize(entityUid, registration, factory, entityManager, serManager, str, data, context1);
          if (!componentRegistryEntry.Component.NetSyncEnabled)
          {
            ushort? netId = registration.NetID;
            if (netId.HasValue)
            {
              ushort valueOrDefault = netId.GetValueOrDefault();
              comp.NetComponents.Remove(valueOrDefault);
            }
          }
        }
      }
    }
    if (context == null)
      return;
    foreach (string extraComponentType in context.GetExtraComponentTypes())
    {
      if (entityPrototype == null || !entityPrototype.Components.ContainsKey(extraComponentType))
      {
        IComponent component;
        ComponentRegistration compReg = context.TryGetComponent(extraComponentType, out component) ? factory.GetRegistration(extraComponentType) : throw new InvalidOperationException($"IEntityLoadContext provided component name {extraComponentType} but refused to provide data");
        EntityPrototype.EnsureCompExistsAndDeserialize(entityUid, compReg, factory, entityManager, serManager, extraComponentType, component, context1);
      }
    }
  }

  public static void EnsureCompExistsAndDeserialize(
    EntityUid entity,
    ComponentRegistration compReg,
    IComponentFactory factory,
    IEntityManager entityManager,
    ISerializationManager serManager,
    string compName,
    IComponent data,
    ISerializationContext? context)
  {
    IComponent component1;
    if (!entityManager.TryGetComponent(entity, compReg.Idx, out component1))
    {
      IComponent component2 = factory.GetComponent(compName);
      entityManager.AddComponent<IComponent>(entity, component2);
      component1 = component2;
    }
    if (!(context is EntityDeserializer entityDeserializer))
    {
      serManager.CopyTo<IComponent>(data, ref component1, context, notNullableOverride: true);
    }
    else
    {
      entityDeserializer.CurrentComponent = compName;
      serManager.CopyTo<IComponent>(data, ref component1, context, notNullableOverride: true);
      entityDeserializer.CurrentComponent = (string) null;
    }
  }

  public override string ToString() => $"EntityPrototype({this.ID})";

  [DataRecord]
  public record ComponentRegistryEntry(IComponent Component, MappingDataNode Mapping);

  [DataDefinition]
  public sealed class EntityPlacementProperties : 
    ISerializationGenerated<EntityPrototype.EntityPlacementProperties>,
    ISerializationGenerated
  {
    private string _placementMode = "PlaceFree";
    private Vector2i _placementOffset;
    [DataField("nodes", false, 1, false, false, null)]
    public List<int>? MountingPoints;
    [DataField("range", false, 1, false, false, null)]
    public int PlacementRange = 200;
    private HashSet<string> _snapFlags = new HashSet<string>();

    public bool PlacementOverriden { get; private set; }

    public bool SnapOverriden { get; private set; }

    [DataField("mode", false, 1, false, false, null)]
    public string PlacementMode
    {
      get => this._placementMode;
      set
      {
        this.PlacementOverriden = true;
        this._placementMode = value;
      }
    }

    [DataField("offset", false, 1, false, false, null)]
    public Vector2i PlacementOffset
    {
      get => this._placementOffset;
      set
      {
        this.PlacementOverriden = true;
        this._placementOffset = value;
      }
    }

    [DataField("snap", false, 1, false, false, null)]
    public HashSet<string> SnapFlags
    {
      get => this._snapFlags;
      set
      {
        this.SnapOverriden = true;
        this._snapFlags = value;
      }
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref EntityPrototype.EntityPlacementProperties target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<EntityPrototype.EntityPlacementProperties>(this, ref target, hookCtx, false, context))
        return;
      string target1 = (string) null;
      if (this.PlacementMode == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string>(this.PlacementMode, ref target1, hookCtx, false, context))
        target1 = this.PlacementMode;
      target.PlacementMode = target1;
      Vector2i target2 = new Vector2i();
      if (!serialization.TryCustomCopy<Vector2i>(this.PlacementOffset, ref target2, hookCtx, false, context))
        target2 = serialization.CreateCopy<Vector2i>(this.PlacementOffset, hookCtx, context);
      target.PlacementOffset = target2;
      List<int> target3 = (List<int>) null;
      if (!serialization.TryCustomCopy<List<int>>(this.MountingPoints, ref target3, hookCtx, true, context))
        target3 = serialization.CreateCopy<List<int>>(this.MountingPoints, hookCtx, context);
      target.MountingPoints = target3;
      int target4 = 0;
      if (!serialization.TryCustomCopy<int>(this.PlacementRange, ref target4, hookCtx, false, context))
        target4 = this.PlacementRange;
      target.PlacementRange = target4;
      HashSet<string> target5 = (HashSet<string>) null;
      if (this.SnapFlags == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<HashSet<string>>(this.SnapFlags, ref target5, hookCtx, true, context))
        target5 = serialization.CreateCopy<HashSet<string>>(this.SnapFlags, hookCtx, context);
      target.SnapFlags = target5;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref EntityPrototype.EntityPlacementProperties target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      EntityPrototype.EntityPlacementProperties target1 = (EntityPrototype.EntityPlacementProperties) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public EntityPrototype.EntityPlacementProperties Instantiate()
    {
      return new EntityPrototype.EntityPlacementProperties();
    }
  }
}
