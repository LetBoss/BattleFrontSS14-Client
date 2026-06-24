using System;
using System.Collections.Generic;
using Content.Shared.Item;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Storage;

[RegisterComponent]
[NetworkedComponent]
public sealed class StorageComponent : Component, ISerializationGenerated<StorageComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public enum StorageUiKey : byte
	{
		Key
	}

	public static string ContainerId = "storagebase";

	public const byte ChunkSize = 8;

	public Dictionary<Vector2i, ulong> OccupiedGrid = new Dictionary<Vector2i, ulong>();

	[ViewVariables]
	public Container Container;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<EntityUid, ItemStorageLocation> StoredItems = new Dictionary<EntityUid, ItemStorageLocation>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, List<ItemStorageLocation>> SavedLocations = new Dictionary<string, List<ItemStorageLocation>>();

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public List<Box2i> Grid = new List<Box2i>();

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[Access(new Type[] { typeof(SharedStorageSystem) })]
	public ProtoId<ItemSizePrototype>? MaxItemSize;

	[DataField(null, false, 1, false, false, null)]
	public bool QuickInsert;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan QuickInsertCooldown = TimeSpan.FromSeconds(0.5);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan OpenUiCooldown = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	public bool ClickInsert = true;

	[DataField(null, false, 1, false, false, null)]
	public bool AllowStorageTransfer = true;

	[DataField(null, false, 1, false, false, null)]
	public bool OpenOnActivate = true;

	public const int AreaPickupLimit = 10;

	[DataField(null, false, 1, false, false, null)]
	public bool AreaInsert;

	[DataField(null, false, 1, false, false, null)]
	public int AreaInsertRadius = 1;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? StorageInsertSound = (SoundSpecifier?)new SoundCollectionSpecifier("storageRustle", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? StorageRemoveSound;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? StorageOpenSound = (SoundSpecifier?)new SoundCollectionSpecifier("storageRustle", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? StorageCloseSound;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public StorageDefaultOrientation? DefaultStorageOrientation;

	[DataField(null, false, 1, false, false, null)]
	public bool HideStackVisualsWhenClosed = true;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<TagPrototype> SilentStorageUserTag = ProtoId<TagPrototype>.op_Implicit("SilentStorageUser");

	[DataField(null, false, 1, false, false, null)]
	public bool ShowVerb = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StorageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StorageComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<StorageComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		Dictionary<EntityUid, ItemStorageLocation> StoredItemsTemp = null;
		if (StoredItems == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<EntityUid, ItemStorageLocation>>(StoredItems, ref StoredItemsTemp, hookCtx, true, context))
		{
			StoredItemsTemp = serialization.CreateCopy<Dictionary<EntityUid, ItemStorageLocation>>(StoredItems, hookCtx, context, false);
		}
		target.StoredItems = StoredItemsTemp;
		Dictionary<string, List<ItemStorageLocation>> SavedLocationsTemp = null;
		if (SavedLocations == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<string, List<ItemStorageLocation>>>(SavedLocations, ref SavedLocationsTemp, hookCtx, true, context))
		{
			SavedLocationsTemp = serialization.CreateCopy<Dictionary<string, List<ItemStorageLocation>>>(SavedLocations, hookCtx, context, false);
		}
		target.SavedLocations = SavedLocationsTemp;
		List<Box2i> GridTemp = null;
		if (Grid == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<Box2i>>(Grid, ref GridTemp, hookCtx, true, context))
		{
			GridTemp = serialization.CreateCopy<List<Box2i>>(Grid, hookCtx, context, false);
		}
		target.Grid = GridTemp;
		ProtoId<ItemSizePrototype>? MaxItemSizeTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>?>(MaxItemSize, ref MaxItemSizeTemp, hookCtx, false, context))
		{
			MaxItemSizeTemp = serialization.CreateCopy<ProtoId<ItemSizePrototype>?>(MaxItemSize, hookCtx, context, false);
		}
		target.MaxItemSize = MaxItemSizeTemp;
		bool QuickInsertTemp = false;
		if (!serialization.TryCustomCopy<bool>(QuickInsert, ref QuickInsertTemp, hookCtx, false, context))
		{
			QuickInsertTemp = QuickInsert;
		}
		target.QuickInsert = QuickInsertTemp;
		TimeSpan QuickInsertCooldownTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(QuickInsertCooldown, ref QuickInsertCooldownTemp, hookCtx, false, context))
		{
			QuickInsertCooldownTemp = serialization.CreateCopy<TimeSpan>(QuickInsertCooldown, hookCtx, context, false);
		}
		target.QuickInsertCooldown = QuickInsertCooldownTemp;
		TimeSpan OpenUiCooldownTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(OpenUiCooldown, ref OpenUiCooldownTemp, hookCtx, false, context))
		{
			OpenUiCooldownTemp = serialization.CreateCopy<TimeSpan>(OpenUiCooldown, hookCtx, context, false);
		}
		target.OpenUiCooldown = OpenUiCooldownTemp;
		bool ClickInsertTemp = false;
		if (!serialization.TryCustomCopy<bool>(ClickInsert, ref ClickInsertTemp, hookCtx, false, context))
		{
			ClickInsertTemp = ClickInsert;
		}
		target.ClickInsert = ClickInsertTemp;
		bool AllowStorageTransferTemp = false;
		if (!serialization.TryCustomCopy<bool>(AllowStorageTransfer, ref AllowStorageTransferTemp, hookCtx, false, context))
		{
			AllowStorageTransferTemp = AllowStorageTransfer;
		}
		target.AllowStorageTransfer = AllowStorageTransferTemp;
		bool OpenOnActivateTemp = false;
		if (!serialization.TryCustomCopy<bool>(OpenOnActivate, ref OpenOnActivateTemp, hookCtx, false, context))
		{
			OpenOnActivateTemp = OpenOnActivate;
		}
		target.OpenOnActivate = OpenOnActivateTemp;
		bool AreaInsertTemp = false;
		if (!serialization.TryCustomCopy<bool>(AreaInsert, ref AreaInsertTemp, hookCtx, false, context))
		{
			AreaInsertTemp = AreaInsert;
		}
		target.AreaInsert = AreaInsertTemp;
		int AreaInsertRadiusTemp = 0;
		if (!serialization.TryCustomCopy<int>(AreaInsertRadius, ref AreaInsertRadiusTemp, hookCtx, false, context))
		{
			AreaInsertRadiusTemp = AreaInsertRadius;
		}
		target.AreaInsertRadius = AreaInsertRadiusTemp;
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
		EntityWhitelist BlacklistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, false, context))
		{
			if (Blacklist == null)
			{
				BlacklistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, context, false);
			}
		}
		target.Blacklist = BlacklistTemp;
		SoundSpecifier StorageInsertSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(StorageInsertSound, ref StorageInsertSoundTemp, hookCtx, true, context))
		{
			StorageInsertSoundTemp = serialization.CreateCopy<SoundSpecifier>(StorageInsertSound, hookCtx, context, false);
		}
		target.StorageInsertSound = StorageInsertSoundTemp;
		SoundSpecifier StorageRemoveSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(StorageRemoveSound, ref StorageRemoveSoundTemp, hookCtx, true, context))
		{
			StorageRemoveSoundTemp = serialization.CreateCopy<SoundSpecifier>(StorageRemoveSound, hookCtx, context, false);
		}
		target.StorageRemoveSound = StorageRemoveSoundTemp;
		SoundSpecifier StorageOpenSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(StorageOpenSound, ref StorageOpenSoundTemp, hookCtx, true, context))
		{
			StorageOpenSoundTemp = serialization.CreateCopy<SoundSpecifier>(StorageOpenSound, hookCtx, context, false);
		}
		target.StorageOpenSound = StorageOpenSoundTemp;
		SoundSpecifier StorageCloseSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(StorageCloseSound, ref StorageCloseSoundTemp, hookCtx, true, context))
		{
			StorageCloseSoundTemp = serialization.CreateCopy<SoundSpecifier>(StorageCloseSound, hookCtx, context, false);
		}
		target.StorageCloseSound = StorageCloseSoundTemp;
		StorageDefaultOrientation? DefaultStorageOrientationTemp = null;
		if (!serialization.TryCustomCopy<StorageDefaultOrientation?>(DefaultStorageOrientation, ref DefaultStorageOrientationTemp, hookCtx, false, context))
		{
			DefaultStorageOrientationTemp = DefaultStorageOrientation;
		}
		target.DefaultStorageOrientation = DefaultStorageOrientationTemp;
		bool HideStackVisualsWhenClosedTemp = false;
		if (!serialization.TryCustomCopy<bool>(HideStackVisualsWhenClosed, ref HideStackVisualsWhenClosedTemp, hookCtx, false, context))
		{
			HideStackVisualsWhenClosedTemp = HideStackVisualsWhenClosed;
		}
		target.HideStackVisualsWhenClosed = HideStackVisualsWhenClosedTemp;
		ProtoId<TagPrototype> SilentStorageUserTagTemp = default(ProtoId<TagPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<TagPrototype>>(SilentStorageUserTag, ref SilentStorageUserTagTemp, hookCtx, false, context))
		{
			SilentStorageUserTagTemp = serialization.CreateCopy<ProtoId<TagPrototype>>(SilentStorageUserTag, hookCtx, context, false);
		}
		target.SilentStorageUserTag = SilentStorageUserTagTemp;
		bool ShowVerbTemp = false;
		if (!serialization.TryCustomCopy<bool>(ShowVerb, ref ShowVerbTemp, hookCtx, false, context))
		{
			ShowVerbTemp = ShowVerb;
		}
		target.ShowVerb = ShowVerbTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StorageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageComponent cast = (StorageComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageComponent cast = (StorageComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageComponent def = (StorageComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StorageComponent Instantiate()
	{
		return new StorageComponent();
	}
}
