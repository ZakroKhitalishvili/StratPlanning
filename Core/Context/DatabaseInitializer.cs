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
        public static void Initialize(PlanningDbContext context)
        {
            var initializer = new DatabaseInitializer();
            initializer.Seed(context);
        }

        private void Seed(PlanningDbContext context)
        {
            if (!context.StepBlocks.Any())
            {
                SeedPredepartureStep(context);
            }

            if (!context.Plans.Any())
            {
                SeedPlans(context);
            }

            if (!context.Users.Any())
            {
                SeedUsers(context);
            }

        }

        private void SeedUsers(PlanningDbContext context)
        {
            var users = new User[]
            {
                new User
                {
                  UserName="admin",Email="admin@sp.com", Password="8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",FirstName=string.Empty,LastName=string.Empty,PositionId=null,Role=Roles.Admin, CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now,CreatedBy=null, UpdatedBy=null
                }
            };

            context.Users.AddRange(users);

            context.SaveChanges();
        }

        private void SeedPlans(PlanningDbContext context)
        {
            var plans = new Plan[]
            {
                new Plan
                {
                    Name="Initial",Description="Initially generated plan",CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now,CreatedBy=null, UpdatedBy=null,EndDate=null,IsCompleted=false,IsWithActionPlan=null,StartDate=DateTime.Now
                }
            };

            context.Plans.AddRange(plans);

            context.SaveChanges();
        }

        private void SeedPredepartureStep(PlanningDbContext context)
        {
            var blocks = new StepBlock[]
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
                        new Question{ Type=QuestionTypes.MultiSelect, Order=3, Title="Who is in the working group?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false, HasOptions=true, CanSpecifyOther=true,
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
                          new Question{ Type=QuestionTypes.StepTable, Order=6, Title="What steps will you use in your planning process?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="Review these steps with the people who are involved, and refine them as needed.", HasFiles=false, HasOptions=false }
                    }
                }

            };

            context.StepBlocks.AddRange(blocks);

            context.SaveChanges();
        }
    }
}
