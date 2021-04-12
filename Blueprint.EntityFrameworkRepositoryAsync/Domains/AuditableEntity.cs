using System;

namespace Blueprint.EntityFrameworkRepositoryAsync
{
    public abstract class AuditableEntity<TId> : Entity<TId>
    {
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
