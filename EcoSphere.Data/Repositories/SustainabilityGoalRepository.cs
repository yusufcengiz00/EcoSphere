using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;

namespace EcoSphere.Data.Repositories;

public class SustainabilityGoalRepository : Repository<SustainabilityGoal>, ISustainabilityGoalRepository
{
    public SustainabilityGoalRepository(EcoSphereDbContext context) : base(context)
    {
    }
}
