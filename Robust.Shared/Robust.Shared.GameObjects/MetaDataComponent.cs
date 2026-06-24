using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
public sealed class MetaDataComponent : Component, ISerializationGenerated<MetaDataComponent>, ISerializationGenerated
{
	[DataField("name", false, 1, false, false, null)]
	internal string? _entityName;

	[DataField("desc", false, 1, false, false, null)]
	internal string? _entityDescription;

	internal EntityPrototype? _entityPrototype;

	[ViewVariables]
	internal readonly Dictionary<ushort, IComponent> NetComponents = new Dictionary<ushort, IComponent>();

	internal TimeSpan? PauseTime;

	internal MetaDataFlags _flags;

	[ViewVariables]
	internal PvsChunkLocation? LastPvsLocation;

	internal PvsIndex PvsData = PvsIndex.Invalid;

	[ViewVariables]
	[Access(new Type[] { typeof(EntityManager) }, Other = AccessPermissions.ReadExecute)]
	public NetEntity NetEntity { get; internal set; } = NetEntity.Invalid;

	[ViewVariables]
	public GameTick EntityLastModifiedTick { get; internal set; } = GameTick.First;

	[ViewVariables]
	public GameTick LastStateApplied { get; internal set; } = GameTick.Zero;

	[ViewVariables]
	public GameTick LastComponentRemoved { get; internal set; } = GameTick.Zero;

	[ViewVariables(VVAccess.ReadWrite)]
	public string EntityName
	{
		get
		{
			if (_entityName == null)
			{
				if (_entityPrototype == null)
				{
					return string.Empty;
				}
				return _entityPrototype.Name;
			}
			return _entityName;
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public string EntityDescription
	{
		get
		{
			if (_entityDescription == null)
			{
				if (_entityPrototype == null)
				{
					return string.Empty;
				}
				return _entityPrototype.Description;
			}
			return _entityDescription;
		}
	}

	[ViewVariables]
	public EntityPrototype? EntityPrototype
	{
		get
		{
			return _entityPrototype;
		}
		[Obsolete("Use MetaDataSystem.SetEntityPrototype")]
		set
		{
			_entityPrototype = value;
			Dirty();
		}
	}

	[ViewVariables]
	[Access(new Type[] { typeof(EntityManager) }, Other = AccessPermissions.ReadExecute)]
	public EntityLifeStage EntityLifeStage { get; internal set; }

	[ViewVariables(VVAccess.ReadOnly)]
	public MetaDataFlags Flags
	{
		get
		{
			return _flags;
		}
		internal set
		{
			if (_flags != value)
			{
				_flags = value;
			}
		}
	}

	[ViewVariables]
	public ushort VisibilityMask { get; internal set; } = 1;

	[ViewVariables]
	public bool EntityPaused => PauseTime.HasValue;

	public bool EntityInitialized => (int)EntityLifeStage >= 2;

	public bool EntityInitializing => EntityLifeStage == EntityLifeStage.Initializing;

	public bool EntityDeleted => (int)EntityLifeStage >= 5;

	private protected override void ClearTicks()
	{
		ClearCreationTick();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MetaDataComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (MetaDataComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			string target3 = null;
			if (!serialization.TryCustomCopy(_entityName, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = _entityName;
			}
			target._entityName = target3;
			string target4 = null;
			if (!serialization.TryCustomCopy(_entityDescription, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = _entityDescription;
			}
			target._entityDescription = target4;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MetaDataComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MetaDataComponent target2 = (MetaDataComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MetaDataComponent target2 = (MetaDataComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MetaDataComponent target2 = (MetaDataComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MetaDataComponent Instantiate()
	{
		return new MetaDataComponent();
	}
}
