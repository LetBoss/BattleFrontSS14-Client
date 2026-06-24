using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Prototypes;

[Prototype(-1)]
public sealed class EntityPrototype : IPrototype, IInheritingPrototype, ISerializationHooks
{
	[DataRecord]
	public record ComponentRegistryEntry(IComponent Component, MappingDataNode Mapping);

	[DataDefinition]
	public sealed class EntityPlacementProperties : ISerializationGenerated<EntityPlacementProperties>, ISerializationGenerated
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
			get
			{
				return _placementMode;
			}
			set
			{
				PlacementOverriden = true;
				_placementMode = value;
			}
		}

		[DataField("offset", false, 1, false, false, null)]
		public Vector2i PlacementOffset
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _placementOffset;
			}
			set
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				PlacementOverriden = true;
				_placementOffset = value;
			}
		}

		[DataField("snap", false, 1, false, false, null)]
		public HashSet<string> SnapFlags
		{
			get
			{
				return _snapFlags;
			}
			set
			{
				SnapOverriden = true;
				_snapFlags = value;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref EntityPlacementProperties target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
			{
				string target2 = null;
				if (PlacementMode == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy(PlacementMode, ref target2, hookCtx, hasHooks: false, context))
				{
					target2 = PlacementMode;
				}
				target.PlacementMode = target2;
				Vector2i target3 = default(Vector2i);
				if (!serialization.TryCustomCopy(PlacementOffset, ref target3, hookCtx, hasHooks: false, context))
				{
					target3 = serialization.CreateCopy<Vector2i>(PlacementOffset, hookCtx, context);
				}
				target.PlacementOffset = target3;
				List<int> target4 = null;
				if (!serialization.TryCustomCopy(MountingPoints, ref target4, hookCtx, hasHooks: true, context))
				{
					target4 = serialization.CreateCopy(MountingPoints, hookCtx, context);
				}
				target.MountingPoints = target4;
				int target5 = 0;
				if (!serialization.TryCustomCopy(PlacementRange, ref target5, hookCtx, hasHooks: false, context))
				{
					target5 = PlacementRange;
				}
				target.PlacementRange = target5;
				HashSet<string> target6 = null;
				if (SnapFlags == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy(SnapFlags, ref target6, hookCtx, hasHooks: true, context))
				{
					target6 = serialization.CreateCopy(SnapFlags, hookCtx, context);
				}
				target.SnapFlags = target6;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref EntityPlacementProperties target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			EntityPlacementProperties target2 = (EntityPlacementProperties)target;
			Copy(ref target2, serialization, hookCtx, context);
			target = target2;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public EntityPlacementProperties Instantiate()
		{
			return new EntityPlacementProperties();
		}
	}

	private ILocalizationManager _loc;

	private static readonly Dictionary<string, string> LocPropertiesDefault = new Dictionary<string, string>();

	private const int DEFAULT_RANGE = 200;

	[DataField("loc", false, 1, false, false, null)]
	private Dictionary<string, string>? _locPropertiesSet;

	[DataField("categories", false, 1, false, false, null)]
	[Access(new Type[] { typeof(PrototypeManager) })]
	[NeverPushInheritance]
	internal HashSet<ProtoId<EntityCategoryPrototype>>? CategoriesInternal;

	[DataField("placement", false, 1, false, false, null)]
	private EntityPlacementProperties PlacementProperties = new EntityPlacementProperties();

	[DataField("components", false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public ComponentRegistry Components = new ComponentRegistry();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string? SetName { get; private set; }

	[DataField("description", false, 1, false, false, null)]
	public string? SetDesc { get; private set; }

	[DataField("suffix", false, 1, false, false, null)]
	public string? SetSuffix { get; private set; }

	[ViewVariables]
	public IReadOnlySet<EntityCategoryPrototype> Categories { get; internal set; } = new HashSet<EntityCategoryPrototype>();

	[ViewVariables]
	public IReadOnlyDictionary<string, string> LocProperties => _locPropertiesSet ?? LocPropertiesDefault;

	[ViewVariables]
	public string Name => _loc.GetEntityData(ID).Name;

	[ViewVariables]
	public string Description => _loc.GetEntityData(ID).Desc;

	[ViewVariables]
	public string? EditorSuffix => _loc.GetEntityData(ID).Suffix;

	[DataField("localizationId", false, 1, false, false, null)]
	public string? CustomLocalizationID { get; private set; }

	[Access(new Type[] { typeof(PrototypeManager) })]
	public bool HideSpawnMenu { get; internal set; }

	[ViewVariables]
	public List<int>? MountingPoints => PlacementProperties.MountingPoints;

	[ViewVariables]
	public string PlacementMode => PlacementProperties.PlacementMode;

	[ViewVariables]
	public int PlacementRange => PlacementProperties.PlacementRange;

	[ViewVariables]
	public Vector2i PlacementOffset => PlacementProperties.PlacementOffset;

	[DataField("save", false, 1, false, false, null)]
	public bool MapSavable { get; set; } = true;

	[ViewVariables]
	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<EntityPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[ViewVariables]
	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	public EntityPrototype()
	{
		Components.Add("Transform", new ComponentRegistryEntry(new TransformComponent(), new MappingDataNode()));
		Components.Add("MetaData", new ComponentRegistryEntry(new MetaDataComponent(), new MappingDataNode()));
	}

	void ISerializationHooks.AfterDeserialization()
	{
		_loc = IoCManager.Resolve<ILocalizationManager>();
	}

	[Obsolete("Pass in IComponentFactory")]
	public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : IComponent, new()
	{
		string componentName = IoCManager.Resolve<IComponentFactory>().GetComponentName<T>();
		return TryGetComponent<T>(componentName, out component);
	}

	public bool TryGetComponent<T>([NotNullWhen(true)] out T? component, IComponentFactory factory) where T : IComponent, new()
	{
		string componentName = factory.GetComponentName<T>();
		return TryGetComponent<T>(componentName, out component);
	}

	public bool TryGetComponent<T>(string name, [NotNullWhen(true)] out T? component) where T : IComponent, new()
	{
		if (!Components.TryGetValue(name, out ComponentRegistryEntry value))
		{
			component = default(T);
			return false;
		}
		if (!(value.Component is T val))
		{
			component = default(T);
			return false;
		}
		component = val;
		return true;
	}

	internal static void LoadEntity(Entity<MetaDataComponent> ent, IComponentFactory factory, IEntityManager entityManager, ISerializationManager serManager, IEntityLoadContext? context)
	{
		Entity<MetaDataComponent> entity = ent;
		entity.Deconstruct(out var owner, out var comp);
		EntityUid entity2 = owner;
		MetaDataComponent metaDataComponent = comp;
		EntityPrototype entityPrototype = metaDataComponent.EntityPrototype;
		ISerializationContext context2 = context as ISerializationContext;
		if (entityPrototype != null)
		{
			foreach (var (text2, componentRegistryEntry2) in entityPrototype.Components)
			{
				if (context != null && context.ShouldSkipComponent(text2))
				{
					continue;
				}
				IComponent component;
				IComponent data = ((context != null && context.TryGetComponent(text2, out component)) ? component : componentRegistryEntry2.Component);
				ComponentRegistration registration = factory.GetRegistration(text2);
				EnsureCompExistsAndDeserialize(entity2, registration, factory, entityManager, serManager, text2, data, context2);
				if (!componentRegistryEntry2.Component.NetSyncEnabled)
				{
					ushort? netID = registration.NetID;
					if (netID.HasValue)
					{
						ushort valueOrDefault = netID.GetValueOrDefault();
						metaDataComponent.NetComponents.Remove(valueOrDefault);
					}
				}
			}
		}
		if (context == null)
		{
			return;
		}
		foreach (string extraComponentType in context.GetExtraComponentTypes())
		{
			if (entityPrototype == null || !entityPrototype.Components.ContainsKey(extraComponentType))
			{
				if (!context.TryGetComponent(extraComponentType, out IComponent component2))
				{
					throw new InvalidOperationException("IEntityLoadContext provided component name " + extraComponentType + " but refused to provide data");
				}
				ComponentRegistration registration2 = factory.GetRegistration(extraComponentType);
				EnsureCompExistsAndDeserialize(entity2, registration2, factory, entityManager, serManager, extraComponentType, component2, context2);
			}
		}
	}

	public static void EnsureCompExistsAndDeserialize(EntityUid entity, ComponentRegistration compReg, IComponentFactory factory, IEntityManager entityManager, ISerializationManager serManager, string compName, IComponent data, ISerializationContext? context)
	{
		if (!entityManager.TryGetComponent(entity, compReg.Idx, out IComponent component))
		{
			IComponent component2 = factory.GetComponent(compName);
			entityManager.AddComponent(entity, component2);
			component = component2;
		}
		if (!(context is EntityDeserializer entityDeserializer))
		{
			serManager.CopyTo(data, ref component, context, skipHook: false, notNullableOverride: true);
			return;
		}
		entityDeserializer.CurrentComponent = compName;
		serManager.CopyTo(data, ref component, context, skipHook: false, notNullableOverride: true);
		entityDeserializer.CurrentComponent = null;
	}

	public override string ToString()
	{
		return "EntityPrototype(" + ID + ")";
	}
}
