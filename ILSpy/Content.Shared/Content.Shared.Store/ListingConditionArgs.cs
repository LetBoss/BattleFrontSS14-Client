using Robust.Shared.GameObjects;

namespace Content.Shared.Store;

public readonly record struct ListingConditionArgs(EntityUid Buyer, EntityUid? StoreEntity, ListingData Listing, IEntityManager EntityManager);
