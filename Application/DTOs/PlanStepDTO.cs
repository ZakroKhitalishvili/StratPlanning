﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class PlanStepDTO
    {
        public string Step { get; set; }

        public IList<StepBlockDTO> StepBlocks { get; set; }

    }
}
