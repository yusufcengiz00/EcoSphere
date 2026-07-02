using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;

namespace EcoSphere.Data.Repositories;

public class WasteManagementRepository : Repository<WasteManagement>, IWasteManagementRepository
{
    public WasteManagementRepository(EcoSphereDbContext context) : base(context)
    {
    }
}
