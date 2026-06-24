using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.GameTicking;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class RoundEndMessageEvent : EntityEventArgs, ISerializationGenerated<RoundEndMessageEvent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	[DataDefinition]
	public struct RoundEndPlayerInfo : ISerializationGenerated<RoundEndPlayerInfo>, ISerializationGenerated
	{
		[DataField(null, false, 1, false, false, null)]
		public string PlayerOOCName = null;

		[DataField(null, false, 1, false, false, null)]
		public string? PlayerICName = null;

		[NonSerialized]
		[DataField(null, false, 1, false, false, null)]
		public NetUserId? PlayerGuid = null;

		public string Role = null;

		[NonSerialized]
		[DataField(null, false, 1, false, false, null)]
		public string[] JobPrototypes = null;

		[NonSerialized]
		[DataField(null, false, 1, false, false, null)]
		public string[] AntagPrototypes = null;

		public NetEntity? PlayerNetEntity = null;

		[DataField(null, false, 1, false, false, null)]
		public bool Antag = false;

		[DataField(null, false, 1, false, false, null)]
		public bool Observer = false;

		public bool Connected = false;

		public RoundEndPlayerInfo()
		{
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref RoundEndPlayerInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy<RoundEndPlayerInfo>(this, ref target, hookCtx, false, context))
			{
				string PlayerOOCNameTemp = null;
				if (PlayerOOCName == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<string>(PlayerOOCName, ref PlayerOOCNameTemp, hookCtx, false, context))
				{
					PlayerOOCNameTemp = PlayerOOCName;
				}
				string PlayerICNameTemp = null;
				if (!serialization.TryCustomCopy<string>(PlayerICName, ref PlayerICNameTemp, hookCtx, false, context))
				{
					PlayerICNameTemp = PlayerICName;
				}
				NetUserId? PlayerGuidTemp = null;
				if (!serialization.TryCustomCopy<NetUserId?>(PlayerGuid, ref PlayerGuidTemp, hookCtx, false, context))
				{
					PlayerGuidTemp = serialization.CreateCopy<NetUserId?>(PlayerGuid, hookCtx, context, false);
				}
				string[] JobPrototypesTemp = null;
				if (JobPrototypes == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<string[]>(JobPrototypes, ref JobPrototypesTemp, hookCtx, true, context))
				{
					JobPrototypesTemp = serialization.CreateCopy<string[]>(JobPrototypes, hookCtx, context, false);
				}
				string[] AntagPrototypesTemp = null;
				if (AntagPrototypes == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<string[]>(AntagPrototypes, ref AntagPrototypesTemp, hookCtx, true, context))
				{
					AntagPrototypesTemp = serialization.CreateCopy<string[]>(AntagPrototypes, hookCtx, context, false);
				}
				bool AntagTemp = false;
				if (!serialization.TryCustomCopy<bool>(Antag, ref AntagTemp, hookCtx, false, context))
				{
					AntagTemp = Antag;
				}
				bool ObserverTemp = false;
				if (!serialization.TryCustomCopy<bool>(Observer, ref ObserverTemp, hookCtx, false, context))
				{
					ObserverTemp = Observer;
				}
				RoundEndPlayerInfo roundEndPlayerInfo = target;
				roundEndPlayerInfo.PlayerOOCName = PlayerOOCNameTemp;
				roundEndPlayerInfo.PlayerICName = PlayerICNameTemp;
				roundEndPlayerInfo.PlayerGuid = PlayerGuidTemp;
				roundEndPlayerInfo.JobPrototypes = JobPrototypesTemp;
				roundEndPlayerInfo.AntagPrototypes = AntagPrototypesTemp;
				roundEndPlayerInfo.Antag = AntagTemp;
				roundEndPlayerInfo.Observer = ObserverTemp;
				target = roundEndPlayerInfo;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref RoundEndPlayerInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			RoundEndPlayerInfo cast = (RoundEndPlayerInfo)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public RoundEndPlayerInfo Instantiate()
		{
			return new RoundEndPlayerInfo();
		}
	}

	public ResolvedSoundSpecifier? RestartSound;

	public string GamemodeTitle { get; }

	public string RoundEndText { get; }

	public TimeSpan RoundDuration { get; }

	public int RoundId { get; }

	public int PlayerCount { get; }

	public RoundEndPlayerInfo[] AllPlayersEndInfo { get; }

	public RoundEndMessageEvent(string gamemodeTitle, string roundEndText, TimeSpan roundDuration, int roundId, int playerCount, RoundEndPlayerInfo[] allPlayersEndInfo, ResolvedSoundSpecifier? restartSound)
	{
		GamemodeTitle = gamemodeTitle;
		RoundEndText = roundEndText;
		RoundDuration = roundDuration;
		RoundId = roundId;
		PlayerCount = playerCount;
		AllPlayersEndInfo = allPlayersEndInfo;
		RestartSound = restartSound;
	}

	public RoundEndMessageEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RoundEndMessageEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<RoundEndMessageEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RoundEndMessageEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RoundEndMessageEvent cast = (RoundEndMessageEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RoundEndMessageEvent Instantiate()
	{
		return new RoundEndMessageEvent();
	}
}
