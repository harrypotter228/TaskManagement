namespace TaskManagement.Server.Domains.Entities
{
    public class IEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        // Can define more common properties or methods for entities here: CreatedAt, UpdatedAt, CreatetdBy, Versioning, etc.
    }
}
