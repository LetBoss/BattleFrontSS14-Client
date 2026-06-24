using System;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[Reflect(false)]
[ImplicitDataDefinitionForInheritors]
public abstract class Component : IComponent, ISerializationGenerated<IComponent>, ISerializationGenerated, ISerializationGenerated<Component>
{
	[DataField("netsync", false, 1, false, false, null)]
	[ViewVariables(VVAccess.ReadWrite)]
	private bool _netSync { get; set; } = true;

	internal bool Networked { get; set; } = true;

	bool IComponent.Networked
	{
		get
		{
			return Networked;
		}
		set
		{
			Networked = value;
		}
	}

	public bool NetSyncEnabled
	{
		get
		{
			if (Networked)
			{
				return _netSync;
			}
			return false;
		}
		set
		{
			_netSync = value;
		}
	}

	[ViewVariables]
	[Obsolete("Update your API to allow accessing Owner through other means")]
	public EntityUid Owner { get; set; } = EntityUid.Invalid;

	[ViewVariables]
	public ComponentLifeStage LifeStage { get; internal set; }

	ComponentLifeStage IComponent.LifeStage
	{
		get
		{
			return LifeStage;
		}
		set
		{
			LifeStage = value;
		}
	}

	public virtual bool SendOnlyToOwner => false;

	public virtual bool SessionSpecific => false;

	[ViewVariables]
	public bool Initialized => (int)LifeStage >= 3;

	[ViewVariables]
	public bool Running
	{
		get
		{
			if (5 <= (int)LifeStage)
			{
				return (int)LifeStage <= 7;
			}
			return false;
		}
	}

	[ViewVariables]
	public bool Deleted => (int)LifeStage >= 9;

	[ViewVariables]
	public GameTick CreationTick { get; internal set; }

	GameTick IComponent.CreationTick
	{
		get
		{
			return CreationTick;
		}
		set
		{
			CreationTick = value;
		}
	}

	[ViewVariables]
	public GameTick LastModifiedTick { get; internal set; }

	GameTick IComponent.LastModifiedTick
	{
		get
		{
			return LastModifiedTick;
		}
		set
		{
			LastModifiedTick = value;
		}
	}

	[Obsolete]
	public void Dirty(IEntityManager? entManager = null)
	{
		IoCManager.Resolve(ref entManager);
		entManager.Dirty(Owner, this);
	}

	void IComponent.ClearTicks()
	{
		ClearTicks();
	}

	private protected virtual void ClearTicks()
	{
		LastModifiedTick = GameTick.Zero;
		ClearCreationTick();
	}

	void IComponent.ClearCreationTick()
	{
		ClearCreationTick();
	}

	private protected void ClearCreationTick()
	{
		CreationTick = GameTick.Zero;
	}

	public Component()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			bool target2 = false;
			if (!serialization.TryCustomCopy(_netSync, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = _netSync;
			}
			target._netSync = target2;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = (Component)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = (Component)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual Component Instantiate()
	{
		throw new NotImplementedException();
	}

	IComponent IComponent.Instantiate()
	{
		return Instantiate();
	}

	IComponent ISerializationGenerated<IComponent>.Instantiate()
	{
		return Instantiate();
	}
}
