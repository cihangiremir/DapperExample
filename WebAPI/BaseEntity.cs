using System;

namespace WebAPI
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedTime { get; set; }
    }
}
