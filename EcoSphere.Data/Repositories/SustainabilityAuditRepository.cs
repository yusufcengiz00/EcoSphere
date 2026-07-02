using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;

namespace EcoSphere.Data.Repositories;

public class SustainabilityAuditRepository : Repository<SustainabilityAudit>, ISustainabilityAuditRepository
{
    public SustainabilityAuditRepository(EcoSphereDbContext context) : base(context)
    {
    }
}
