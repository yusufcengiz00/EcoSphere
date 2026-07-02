using System.Threading.Tasks;
using EcoSphere.Core.Repositories;

namespace EcoSphere.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EcoSphereDbContext _context;

    public ISustainabilityGoalRepository SustainabilityGoals { get; }
    public IEcoCertificateRepository EcoCertificates { get; }
    public ISustainabilityAuditRepository SustainabilityAudits { get; }
    public IEnergyConsumptionRepository EnergyConsumptions { get; }
    public IWasteManagementRepository WasteManagements { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(EcoSphereDbContext context)
    {
        _context = context;
        SustainabilityGoals = new SustainabilityGoalRepository(context);
        EcoCertificates = new EcoCertificateRepository(context);
        SustainabilityAudits = new SustainabilityAuditRepository(context);
        EnergyConsumptions = new EnergyConsumptionRepository(context);
        WasteManagements = new WasteManagementRepository(context);
        Users = new UserRepository(context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
