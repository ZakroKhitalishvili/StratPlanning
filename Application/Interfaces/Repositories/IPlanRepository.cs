using Core.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IPlanRepository:IRepositoryBase<Plan>
    {
         void CreatePlan(Plan plan);
    }
}
