using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappers
{
    public class MapperInitializer
    {
        public static void Initialize()
        {
            new PlanStepMapper().Configure(); 
        }
    }
}
