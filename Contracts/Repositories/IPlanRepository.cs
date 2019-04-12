using Core.Entities;

namespace Contracts.Repositories
{
    public interface IPlanRepository:IRepositoryBase<Plan>
    {
         void CreatePlan(Plan plan);
    }
}
