using System;
using System.Numerics;
using Content.Shared.Physics;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Storage.Components;

[NetworkedComponent]
public abstract class SharedEntityStorageComponent : Component, ISerializationGenerated<SharedEntityStorageComponent>, ISerializationGenerated
{
	public readonly float MaxSize = 1f;

	public static readonly TimeSpan InternalOpenAttemptDelay = TimeSpan.FromSeconds(0.5);

	public TimeSpan NextInternalOpenAttempt;

	public readonly int MasksToRemove = 28;

	[DataField(null, false, 1, false, false, null)]
	public int RemovedMasks;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Capacity = 30;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool IsCollidableWhenOpen;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool OpenOnMove = true;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Vector2 EnteringOffset = new Vector2(0f, 0f);

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public CollisionGroup EnteringOffsetCollisionFlags = CollisionGroup.TableMask;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float EnteringRange = 0.18f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool ShowContents;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool OccludesLight = true;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool DeleteContentsOnDestruction;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Airtight = true;

	[DataField(null, false, 1, false, false, null)]
	public bool Open;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier CloseSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/closetclose.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier OpenSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/closetopen.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[ViewVariables]
	public Container Contents;

	public SharedEntityStorageComponent()
	{
	}//IL_0066: Unknown result type (might be due to invalid IL or missing references)
	//IL_0070: Expected O, but got Unknown
	//IL_007f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0089: Expected O, but got Unknown


	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedEntityStorageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedEntityStorageComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<SharedEntityStorageComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		int RemovedMasksTemp = 0;
		if (!serialization.TryCustomCopy<int>(RemovedMasks, ref RemovedMasksTemp, hookCtx, false, context))
		{
			RemovedMasksTemp = RemovedMasks;
		}
		target.RemovedMasks = RemovedMasksTemp;
		int CapacityTemp = 0;
		if (!serialization.TryCustomCopy<int>(Capacity, ref CapacityTemp, hookCtx, false, context))
		{
			CapacityTemp = Capacity;
		}
		target.Capacity = CapacityTemp;
		bool IsCollidableWhenOpenTemp = false;
		if (!serialization.TryCustomCopy<bool>(IsCollidableWhenOpen, ref IsCollidableWhenOpenTemp, hookCtx, false, context))
		{
			IsCollidableWhenOpenTemp = IsCollidableWhenOpen;
		}
		target.IsCollidableWhenOpen = IsCollidableWhenOpenTemp;
		bool OpenOnMoveTemp = false;
		if (!serialization.TryCustomCopy<bool>(OpenOnMove, ref OpenOnMoveTemp, hookCtx, false, context))
		{
			OpenOnMoveTemp = OpenOnMove;
		}
		target.OpenOnMove = OpenOnMoveTemp;
		Vector2 EnteringOffsetTemp = default(Vector2);
		if (!serialization.TryCustomCopy<Vector2>(EnteringOffset, ref EnteringOffsetTemp, hookCtx, false, context))
		{
			EnteringOffsetTemp = serialization.CreateCopy<Vector2>(EnteringOffset, hookCtx, context, false);
		}
		target.EnteringOffset = EnteringOffsetTemp;
		CollisionGroup EnteringOffsetCollisionFlagsTemp = CollisionGroup.None;
		if (!serialization.TryCustomCopy<CollisionGroup>(EnteringOffsetCollisionFlags, ref EnteringOffsetCollisionFlagsTemp, hookCtx, false, context))
		{
			EnteringOffsetCollisionFlagsTemp = EnteringOffsetCollisionFlags;
		}
		target.EnteringOffsetCollisionFlags = EnteringOffsetCollisionFlagsTemp;
		float EnteringRangeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(EnteringRange, ref EnteringRangeTemp, hookCtx, false, context))
		{
			EnteringRangeTemp = EnteringRange;
		}
		target.EnteringRange = EnteringRangeTemp;
		bool ShowContentsTemp = false;
		if (!serialization.TryCustomCopy<bool>(ShowContents, ref ShowContentsTemp, hookCtx, false, context))
		{
			ShowContentsTemp = ShowContents;
		}
		target.ShowContents = ShowContentsTemp;
		bool OccludesLightTemp = false;
		if (!serialization.TryCustomCopy<bool>(OccludesLight, ref OccludesLightTemp, hookCtx, false, context))
		{
			OccludesLightTemp = OccludesLight;
		}
		target.OccludesLight = OccludesLightTemp;
		bool DeleteContentsOnDestructionTemp = false;
		if (!serialization.TryCustomCopy<bool>(DeleteContentsOnDestruction, ref DeleteContentsOnDestructionTemp, hookCtx, false, context))
		{
			DeleteContentsOnDestructionTemp = DeleteContentsOnDestruction;
		}
		target.DeleteContentsOnDestruction = DeleteContentsOnDestructionTemp;
		bool AirtightTemp = false;
		if (!serialization.TryCustomCopy<bool>(Airtight, ref AirtightTemp, hookCtx, false, context))
		{
			AirtightTemp = Airtight;
		}
		target.Airtight = AirtightTemp;
		bool OpenTemp = false;
		if (!serialization.TryCustomCopy<bool>(Open, ref OpenTemp, hookCtx, false, context))
		{
			OpenTemp = Open;
		}
		target.Open = OpenTemp;
		SoundSpecifier CloseSoundTemp = null;
		if (CloseSound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(CloseSound, ref CloseSoundTemp, hookCtx, true, context))
		{
			CloseSoundTemp = serialization.CreateCopy<SoundSpecifier>(CloseSound, hookCtx, context, false);
		}
		target.CloseSound = CloseSoundTemp;
		SoundSpecifier OpenSoundTemp = null;
		if (OpenSound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(OpenSound, ref OpenSoundTemp, hookCtx, true, context))
		{
			OpenSoundTemp = serialization.CreateCopy<SoundSpecifier>(OpenSound, hookCtx, context, false);
		}
		target.OpenSound = OpenSoundTemp;
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		target.Whitelist = WhitelistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedEntityStorageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedEntityStorageComponent cast = (SharedEntityStorageComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedEntityStorageComponent cast = (SharedEntityStorageComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedEntityStorageComponent def = (SharedEntityStorageComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedEntityStorageComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
