using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;

public abstract class _BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}
