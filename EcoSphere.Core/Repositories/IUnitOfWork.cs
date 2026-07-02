using System;
using System.Threading.Tasks;

namespace EcoSphere.Core.Repositories;

public interface IUnitOfWork : IDisposable
{
    ISustainabilityGoalRepository SustainabilityGoals { get; }
    IEcoCertificateRepository EcoCertificates { get; }
    ISustainabilityAuditRepository SustainabilityAudits { get; }
    IEnergyConsumptionRepository EnergyConsumptions { get; }
    IWasteManagementRepository WasteManagements { get; }
    IUserRepository Users { get; }

    Task<int> CompleteAsync();
}
