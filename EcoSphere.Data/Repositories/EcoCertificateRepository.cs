using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;

namespace EcoSphere.Data.Repositories;

public class EcoCertificateRepository : Repository<EcoCertificate>, IEcoCertificateRepository
{
    public EcoCertificateRepository(EcoSphereDbContext context) : base(context)
    {
    }
}
