using TaskManagement.Server.Domains.Enums;

namespace TaskManagement.Server.Domains.Entities
{
    public sealed class Board : IEntity
    {
        public string Name { get; private set; } = string.Empty;
        private readonly List<TaskStatusEnum> _statuses = new();
        public IReadOnlyList<TaskStatusEnum> Statuses => _statuses;
        private Board() { }
        public Board(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Board name required") : name;
        }        
        public void SetStatuses(IEnumerable<TaskStatusEnum> statuses)
        {
            var list = (statuses ?? Enumerable.Empty<TaskStatusEnum>()).Distinct().ToList();
            if (list.Count == 0)
            {
                throw new InvalidOperationException("At least one status is required.");
            }

            _statuses.Clear();
            _statuses.AddRange(list);
        }
    }
}
