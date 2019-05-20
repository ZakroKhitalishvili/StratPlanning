using Core.Constants;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Context
{
    public class DatabaseInitializer
    {
        private User[] Users { get; set; }
        private Plan[] Plans { get; set; }
        private StepBlock[] Blocks { get; set; }
        private Dictionary[] Positions { get; set; }
        private PlanningDbContext Context { get; set; }

        public static void Initialize(PlanningDbContext context)
        {
            var initializer = new DatabaseInitializer();
            initializer.Seed(context);
        }

        private void Seed(PlanningDbContext context)
        {
            Context = context;
            if (!Context.Users.Any())
            {
                SeedUsers();
            }

            if (!Context.StepBlocks.Any())
            {
                SeedPredepartureStep();
            }

            if (!Context.Plans.Any())
            {
                SeedPlans();
            }

            if (!Context.Dictionaries.Any(x => x.HasPosition))
            {
                SeedPositions();
            }

            if (!Context.StepBlocks.Where(x => x.Step == Steps.Mission).Any())
            {
                SeedMissionStep();
            }

            Context.SaveChanges();

        }

        private void SeedPositions()
        {
            Positions = new Dictionary[]
            {
                new Dictionary
                {
                    HasPosition=true,Title="Chairman",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasPosition=true,Title="Advisor",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                }
            };

            Context.Dictionaries.AddRange(Positions);
        }

        private void SeedUsers()
        {
            Users = new User[]
            {
                new User
                {
                 Email="admin@sp.com", Password="8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",FirstName="Mark",LastName="Andre",PositionId=null,Role=Roles.Admin, CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now,CreatedBy=null, UpdatedBy=null
                }
            };

            Context.Users.AddRange(Users);

        }

        private void SeedPlans()
        {
            Plans = new Plan[]
            {
                new Plan
                {
                    Name="Initial",Description="Initially generated plan",CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now,CreatedBy=null, UpdatedBy=null,EndDate=null,IsCompleted=false,IsWithActionPlan=null,StartDate=DateTime.Now,
                    StepTasks=new List<StepTask>()
                    {
                        new StepTask
                        {
                            Step = Steps.Predeparture,
                            IsCompleted = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        }
                    }
                },
                 new Plan
                {
                    Name="Party Goals Planning",Description="Initially generated plan",CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now,CreatedBy=null, UpdatedBy=null,EndDate=null,IsCompleted=false,IsWithActionPlan=null,StartDate=DateTime.Now,
                    StepTasks=new List<StepTask>()
                    {
                        new StepTask
                        {
                            Step = Steps.Predeparture,
                            IsCompleted = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        }
                    }
                 }
            };

            foreach (var plan in Plans)
            {
                foreach (var user in Users)
                    plan.UsersToPlans.Add(new UserToPlan { User = user });
            }

            Context.Plans.AddRange(Plans);

        }

        private void SeedPredepartureStep()
        {
            Blocks = new StepBlock[]
            {
                new StepBlock{
                    Title ="Ready for takeoff?",
                    Instruction ="sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=1,
                    Step=Steps.Predeparture,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.Boolean, Order=1, Title="Focal point appointed", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.Boolean, Order=2, Title="Resources to do the planning are available (time, people, etc.)", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.Boolean, Order=3, Title="Does the strategic planning group have both the authority and capability to take relevant decisions?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.Boolean, Order=4, Title="Are we sure the planning process will not conflict with other processes in which the party is involved?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.Boolean, Order=5, Title="Now is the right time to initiate the process", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.Boolean, Order=6, Title="Do you believe that all the relevant conditions to make the planning process a success are met?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                    }
                },
                new StepBlock{
                    Title ="Planning the planning",
                    Instruction ="sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=2,
                    Step=Steps.Predeparture,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.Select, Order=1, Title="Whose plan is it?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false, HasOptions=true, CanSpecifyOther=true,
                            Options=new List<Option>()
                            {
                                new Option{ Title="Party", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="Regional unit", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="Sectorial unit", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null}
                            }
                        },
                        new Question{ Type=QuestionTypes.Select, Order=2, Title="What period of time will the plan cover?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="It is recommended to develop a plan for 2 to 5 years.", HasFiles=false, HasOptions=true, CanSpecifyOther=true,
                           Options=new List<Option>()
                            {
                                new Option{ Title="1 year", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="2 year", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="3 year", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="4 year", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null}
                            }},
                        new Question{ Type=QuestionTypes.TagMultiSelect, Order=3, Title="Who is in the working group?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false, HasOptions=true, CanSpecifyOther=true,
                             Options=new List<Option>()
                            {
                                new Option{ Title="Senior leadership", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="Middle leadership", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="Regional branches", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="Members", CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null}
                            }
                        },
                        new Question{ Type=QuestionTypes.PlanTypeSelect, Order=4, Title="What type of written plan do we envision?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false, HasOptions=false},
                        new Question{ Type=QuestionTypes.Select, Order=5, Title="What is the time frame for the planning process?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="On overage, depending on the type of a strategic plan, the process takes 2-4 months.", HasFiles=false, HasOptions=true, CanSpecifyOther=true,
                           Options=new List<Option>()
                            {
                                new Option{ Title="3 months", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                                new Option{ Title="6 months", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now,UpdatedBy=null, CreatedBy=null},
                            }},
                         }
                }

            };

            Context.StepBlocks.AddRange(Blocks);

        }

        private void SeedMissionStep()
        {
            Blocks = new StepBlock[]
            {
                new StepBlock
                {
                    Title = "Mission Statement",
                    Instruction = "sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=1,
                    Step=Steps.Mission,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.TextArea, Order=1, Title="Which of the party documents express your mission statement the best?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="Please, copy the existing mission statement here.", HasFiles=true,
                            Files = new List<File>()
                            {
                                new File { Name="summery", Ext="docx", CreatedAt=DateTime.Now, CreatedBy=null, Path="" },
                                new File { Name="description", Ext="pdf", CreatedAt=DateTime.Now, CreatedBy=null, Path="" }
                            }
                        },
                        new Question{ Type=QuestionTypes.TextArea, Order=2, Title="In general, what does our party offer to society? How our party differs from the competing political parties?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=3, Title="What is our ideology? What are our core values?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=4, Title="Should our mission statement be modified? If yes, why and how?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=5, Title="Examine your answers to the above questions and draft a new mission statement.", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },

                    }
                }
            };

            Context.StepBlocks.AddRange(Blocks);
        }
    }
}
