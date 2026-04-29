namespace Domain.Entity
{
    public interface IAppEntity
    {
        IReadOnlyCollection<IDomainEvent> Events { get; }

        void ClearEvents();
    }
}