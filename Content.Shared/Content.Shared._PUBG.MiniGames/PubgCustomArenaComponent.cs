using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.MiniGames;

[RegisterComponent]
[NetworkedComponent]
public sealed class PubgCustomArenaComponent : Component, ISerializationGenerated<PubgCustomArenaComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string DisplayName { get; set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public NetUserId AuthorUserId { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public string AuthorCkey { get; set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public DateTime DateCreated { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public string FileName { get; set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public bool IsInCustomizationMode { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgCustomArenaComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgCustomArenaComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgCustomArenaComponent>(this, ref target, hookCtx, false, context))
		{
			string DisplayNameTemp = null;
			if (DisplayName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(DisplayName, ref DisplayNameTemp, hookCtx, false, context))
			{
				DisplayNameTemp = DisplayName;
			}
			target.DisplayName = DisplayNameTemp;
			NetUserId AuthorUserIdTemp = default(NetUserId);
			if (!serialization.TryCustomCopy<NetUserId>(AuthorUserId, ref AuthorUserIdTemp, hookCtx, false, context))
			{
				AuthorUserIdTemp = serialization.CreateCopy<NetUserId>(AuthorUserId, hookCtx, context, false);
			}
			target.AuthorUserId = AuthorUserIdTemp;
			string AuthorCkeyTemp = null;
			if (AuthorCkey == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(AuthorCkey, ref AuthorCkeyTemp, hookCtx, false, context))
			{
				AuthorCkeyTemp = AuthorCkey;
			}
			target.AuthorCkey = AuthorCkeyTemp;
			DateTime DateCreatedTemp = default(DateTime);
			if (!serialization.TryCustomCopy<DateTime>(DateCreated, ref DateCreatedTemp, hookCtx, false, context))
			{
				DateCreatedTemp = DateCreated;
			}
			target.DateCreated = DateCreatedTemp;
			string FileNameTemp = null;
			if (FileName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FileName, ref FileNameTemp, hookCtx, false, context))
			{
				FileNameTemp = FileName;
			}
			target.FileName = FileNameTemp;
			bool IsInCustomizationModeTemp = false;
			if (!serialization.TryCustomCopy<bool>(IsInCustomizationMode, ref IsInCustomizationModeTemp, hookCtx, false, context))
			{
				IsInCustomizationModeTemp = IsInCustomizationMode;
			}
			target.IsInCustomizationMode = IsInCustomizationModeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgCustomArenaComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgCustomArenaComponent cast = (PubgCustomArenaComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgCustomArenaComponent cast = (PubgCustomArenaComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgCustomArenaComponent def = (PubgCustomArenaComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgCustomArenaComponent Instantiate()
	{
		return new PubgCustomArenaComponent();
	}
}
