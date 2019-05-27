﻿using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
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

            if (!Context.Plans.Any())
            {
                SeedPlans();
            }


            if (!Context.Dictionaries.Any(x => x.HasPosition))
            {
                SeedPositions();
            }

            if (!Context.Dictionaries.Any(x => x.HasValue))
            {
                SeedValues();
            }

            if (!Context.Dictionaries.Any(x => x.HasStakeholderCategory))
            {
                SeedStakeholderCategories();
            }

            if (!Context.Dictionaries.Any(x => x.HasStakeholderCriteria))
            {
                SeedStakeholderCriterions();
            }

            if (!Context.StepBlocks.Where(x => x.Step == Steps.Predeparture).Any())
            {
                SeedPredepartureStep();
            }

            if (!Context.StepBlocks.Where(x => x.Step == Steps.Mission).Any())
            {
                SeedMissionStep();
            }

            if (!Context.StepBlocks.Where(x => x.Step == Steps.Vision).Any())
            {
                SeedVisionStep();
            }

            if (!Context.StepBlocks.Where(x => x.Step == Steps.Values).Any())
            {
                SeedValuesStep();
            }

            if (!Context.StepBlocks.Where(x => x.Step == Steps.StakeholdersIdentify).Any())
            {
                SeedStakeholdersIdentifyStep();
            }

            if (!Context.StepBlocks.Where(x => x.Step == Steps.SWOT).Any())
            {
                SeedSWOTStep();
            }

            if (!Context.StepBlocks.Where(x => x.Step == Steps.StrategicIssues).Any())
            {
                SeedStrategicIssuesStep();
            }
            if (!Context.StepBlocks.Where(x => x.Step == Steps.StakeholdersAnalysis).Any())
            {
                SeedStakeholdersAnalysisStep();
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

        private void SeedValues()
        {
            Positions = new Dictionary[]
            {
                new Dictionary
                {
                    HasValue=true,Title="Team work",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasValue=true,Title="Openness",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasValue=true,Title="Integrity",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasValue=true,Title="Profesionalism",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
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
                        new StepTask{Step = Steps.ActionPlanDetailed,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.ActionPlanKeyQuestions,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Evalution,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.IssuesDistinguish,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Mission,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Predeparture,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Review,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.StakeholdersAnalysis,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.StakeholdersIdentify,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.StrategicIssues,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.SWOT,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Values,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Vision,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                    }
                },
                 new Plan
                {
                    Name="Party Goals Planning",Description="Initially generated plan",CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now,CreatedBy=null, UpdatedBy=null,EndDate=null,IsCompleted=false,IsWithActionPlan=null,StartDate=DateTime.Now,
                    StepTasks=new List<StepTask>()
                    {
                        new StepTask{Step = Steps.ActionPlanDetailed,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.ActionPlanKeyQuestions,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Evalution,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.IssuesDistinguish,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Mission,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Predeparture,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Review,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.StakeholdersAnalysis,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.StakeholdersIdentify,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.StrategicIssues,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.SWOT,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Values,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                        new StepTask{Step = Steps.Vision,IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now},
                    }
                 }
            };

            foreach (var plan in Plans)
            {
                if (Users != null)
                {
                    foreach (var user in Users)
                        plan.UsersToPlans.Add(new UserToPlan { User = user });
                }
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
                        new Question{ Type=QuestionTypes.TextArea, Order=2, Title="1. In general, what does our party offer to society? How our party differs from the competing political parties?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=3, Title="2. What is our ideology? What are our core values?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=4, Title="3. Should our mission statement be modified? If yes, why and how?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=5, Title="4. Examine your answers to the above questions and draft a new mission statement.", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.File, Order=6, Title="Attach files", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=true }
                    }
                }
            };

            Context.StepBlocks.AddRange(Blocks);
        }

        private void SeedVisionStep()
        {
            Blocks = new StepBlock[]
            {
                new StepBlock
                {
                    Title = "Current mission and mandate of the party",
                    Instruction = "condimentum eu, suscipit sit amet arcu. Donec vel dignissim ligul ",
                    Order=1,
                    Step=Steps.Vision,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.TextArea, Order=1, Title="What is the purpose of your party?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="Please, copy the existing mission statement here." },
                        new Question{ Type=QuestionTypes.TextArea, Order=2, Title="What is the comparative advantage of your party? How is it different from others?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=3, Title="Describe your external legitimacy and support", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false }
                    }
                },
                new StepBlock
                {
                    Title = "Structure, governance and funding of the party",
                    Instruction = "condimentum eu, suscipit sit amet arcu. Donec vel dignissim ligul",
                    Order=2,
                    Step=Steps.Vision,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.TextArea, Order=1, Title="How are important decisions made?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=2, Title="How are different sections of the party related? How do they coordinate? How are they accountable?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=3, Title="How is party membership organized?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=4, Title="What are the party’s current sources of funding? Roughly, how large is the share of each source in the total funding structure?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=5, Title="What services does your party offer to its members?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=6, Title="What activities do you organize?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=7, Title="What is the state of internal party democracy? What are the mechanisms? How do they work?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=8, Title="Briefly describe key aspects of your organizational culture", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                    }
                },
                new StepBlock
                {
                    Title = "Operational aspects of the party",
                    Instruction = "condimentum eu, suscipit sit amet arcu. Donec vel dignissim ligul",
                    Order=3,
                    Step=Steps.Vision,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.TextArea, Order=1, Title="What is the organizational infrastructure of the party? Who is on payroll and how are tasks divided?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=2, Title="How is the party administered? How well are administrative processes working?", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false }
                    }
                },
                new StepBlock
                {
                    Title = "The role",
                    Instruction = "condimentum eu, suscipit sit amet arcu. Donec vel dignissim ligul",
                    Order=4,
                    Step=Steps.Vision,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.TextArea, Order=1, Title="Having all of your previous answers in mind, how would you describe the role(s) of your party", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false }
                    }
                },
                new StepBlock
                {
                    Title = "Sketching a vision",
                    Instruction = "condimentum eu, suscipit sit amet arcu. Donec vel dignissim ligul",
                    Order=5,
                    Step=Steps.Vision,
                    Description="A vision describes how the political party should look after it has successfully implemented its strategies and achieved full potential. A vision statement answers the question: where and what do we want to be? What might the party look like or be in the future, given expected opportunities and challenges, as well as anticipated or conceivable actions? Describe the desired state of affairs in (…) years:",
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.TextArea, Order=1, Title="1. Mission and mandate", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=2, Title="2. Structure, governance and funding", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=3, Title="3. Operational aspects", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=4, Title="4. The role", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false },
                        new Question{ Type=QuestionTypes.TextArea, Order=5, Title="5. Examine your answers to the above questions and draft a vision statement", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false }
                    }
                }
            };

            Context.StepBlocks.AddRange(Blocks);
        }

        private void SeedValuesStep()
        {
            Blocks = new StepBlock[]
            {
                new StepBlock
                {
                    Title = "Value Statement",
                    Instruction = "sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=1,
                    Step=Steps.Values,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.Values, Order=1, Title="Your answer", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="Please, enter values.", HasFiles=false }
                    }
                }
            };

            Context.StepBlocks.AddRange(Blocks);
        }

        private void SeedStakeholdersIdentifyStep()
        {
            Blocks = new StepBlock[]
            {
                new StepBlock
                {
                    Title = "Identifying the party’s stakeholders",
                    Instruction = "sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=1,
                    Step=Steps.StakeholdersIdentify,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.Stakeholder, Order=1, Title="Your answer", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="Please, enter values.", HasFiles=false }
                    }
                }
            };

            Context.StepBlocks.AddRange(Blocks);
        }

        private void SeedSWOTStep()
        {
            Blocks = new StepBlock[]
            {
                new StepBlock
                {
                    Title = "Fill out SWOT table",
                    Instruction = "sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=1,
                    Step=Steps.SWOT,
                    Description="Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled. Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled. Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled. Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled. Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled. Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled.",
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.SWOT, Order=1, Title="Your answer", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false }
                    }
                }
            };

            Context.StepBlocks.AddRange(Blocks);
        }

        private void SeedStakeholdersAnalysisStep()
        {
            Blocks = new StepBlock[]
            {
                new StepBlock
                {
                    Title = "Internal stakeholders rating",
                    Instruction = "sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=1,
                    Step=Steps.StakeholdersAnalysis,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.InternalStakeholdersRating, Order=1, Title="", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="", HasFiles=false }
                    }
                },
                new StepBlock
                {
                    Title = "External stakeholders rating",
                    Instruction = "sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=1,
                    Step=Steps.StakeholdersAnalysis,
                    Description=null,
                    UpdatedAt=DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.ExternalStakeholdersRating, Order=2, Title="", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description="", HasFiles=false }
                    }
                }
            };

            Context.StepBlocks.AddRange(Blocks);
        }

        private void SeedStakeholderCategories()
        {
            Positions = new Dictionary[]
            {
                new Dictionary
                {
                    HasStakeholderCategory=true,Title="Stakeholder category 1",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasStakeholderCategory=true,Title="Stakeholder category 2",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasStakeholderCategory=true,Title="Stakeholder category 3",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
            };

            Context.Dictionaries.AddRange(Positions);
        }

        private void SeedStakeholderCriterions()
        {
            Positions = new Dictionary[]
            {
                new Dictionary
                {
                    HasStakeholderCriteria=true,Title="Professionalism",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasStakeholderCriteria=true,Title="Transparency",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasStakeholderCriteria=true,Title="Democracy",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasStakeholderCriteria=true,Title="Information",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasStakeholderCriteria=true,Title="Input in public",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasStakeholderCriteria=true,Title="Representing societal interest",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                },
                new Dictionary
                {
                    HasStakeholderCriteria=true,Title="Training political leaders",CreatedAt=DateTime.Now, CreatedBy=null,UpdatedAt=DateTime.Now,UpdatedBy=null
                }
            };

            Context.Dictionaries.AddRange(Positions);
        }

        private void SeedStrategicIssuesStep()
        {
            Blocks = new StepBlock[]
            {
                new StepBlock
                {
                    Title = "Strategic issues",
                    Instruction = "sagittis. In dignissim commodo hendrerit. Sed congue purus luctus mi feugiat, ut consequat nisi porttitor",
                    Order=1,
                    Step=Steps.StrategicIssues,
                    Description=null,
                    UpdatedAt =DateTime.Now,
                    CreatedAt=DateTime.Now,
                    CreatedBy=null,
                    UpdatedBy=null,
                    Questions=new List<Question>()
                    {
                        new Question{ Type=QuestionTypes.StrategicIssues, Order=1, Title="Your answer", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, CreatedBy=null, UpdatedBy=null, Description=null, HasFiles=false }
                    }
                }
            };

            Context.StepBlocks.AddRange(Blocks);
        }
    }
}
