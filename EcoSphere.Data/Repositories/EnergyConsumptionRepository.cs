using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;

namespace EcoSphere.Data.Repositories;

public class EnergyConsumptionRepository : Repository<EnergyConsumption>, IEnergyConsumptionRepository
{
    public EnergyConsumptionRepository(EcoSphereDbContext context) : base(context)
    {
    }
}
