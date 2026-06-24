using Robust.Shared.Network;

namespace Content.Shared.Chat.V2.Repository;

public struct ChatRecord(string userName, NetUserId userId, IChatEvent storedEvent, string entityName)
{
	public string UserName = userName;

	public NetUserId UserId = userId;

	public string EntityName = entityName;

	public IChatEvent StoredEvent = storedEvent;
}
