using NUnit.Framework;
using SimpleWorkflow.Core;
using System;
using System.Linq;

namespace SimpleWorkflow.UnitTests
{
    public class WorkFlowTests
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Add_WhenBuilderIsNull_ReturnException()
        {
            var wf = new WorkFlowBuilder<TestWFStates, WorkFlowCommands>();

            var ex = Assert.Throws<Exception>(() => wf.Add(null));
            Assert.That(ex.Message, Contains.Substring("builder is not defined").IgnoreCase);

        }

        [Test]
        public void Add_BuilderCommandNotDefined_ReturnException()
        {
            var wf = new WorkFlowBuilder<TestWFStates, WorkFlowCommands>();

            var ex = Assert.Throws<Exception>(() =>
            wf.Add(q => q.From(q => q.FormSubmission)
                         .GoTo(q => q.ExpertReview)));
            Assert.That(ex.Message, Contains.Substring("'If' statement is not defined").IgnoreCase);

        }

        [Test]
        public void Add_BuilderCurrentStateNotDefined_ReturnException()
        {
            var wf = new WorkFlowBuilder<TestWFStates, WorkFlowCommands>();

            var ex = Assert.Throws<Exception>(() =>
            wf.Add(q => q.If(q => q.Approve)
                         .GoTo(q => q.ExpertReview)));
            Assert.That(ex.Message, Contains.Substring("'From' statement is not defined").IgnoreCase);

        }

        [Test]
        public void GetNext_NextStateIsDefined_ReturnNextState()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(q => q.ExpertReview, q => q.Approve);

            Assert.That(nextState, Is.EqualTo(new SupervisorReview()));
        }

        [Test]
        public void GetNext_NextStateIsNotDefined_ReturnNull()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(q => q.ExpertReview, q => q.Reject);

            Assert.That(nextState, Is.EqualTo(null));

        }

        [Test]
        public void GetNext_WorkflowHasNoItem_ReturnNull()
        {
            var wf = new  WorkFlowBuilder<TestWFStates, WorkFlowCommands>().Build();

            var nextState = wf.GetNextState(q => q.FormSubmission, q => q.Reject);

            Assert.That(nextState, Is.EqualTo(null));

        }

        [Test]
        public void GetNext_CurrentStateIsNull_ReturnNull()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(null, q => q.Reject);

            Assert.That(nextState, Is.EqualTo(null));

        }

        [Test]
        public void GetNext_CommandIsNull_ReturnNull()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(q => q.FormSubmission, null);

            Assert.That(nextState, Is.EqualTo(null));

        }

        [Test]
        public void GetNext_ProvidedWithIntegerStateAndCommand_ReturnNextStateItem()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(2, 1);

            Assert.That(nextState.Value, Is.EqualTo(3));

        }

        [Test]
        public void GetCommands_WorkflowHasNoItem_ReturnNull()
        {
            var wf = new WorkFlowBuilder<TestWFStates, WorkFlowCommands>().Build();

            var commands = wf.GetCommands();

            Assert.That(commands, Is.EqualTo(Enumerable.Empty<TransitionItem>()));

        }

        [Test]
        public void GetCommands_WorkflowHasItems_ReturnCommands()
        {
            var wf = CreateSampleWorkFow();

            var commands = wf.GetCommands();

            Assert.That(commands.Count() == 3,Is.True);
            Assert.That(
                new TransitionItem[] { new ApproveCommand()
                                     , new RejectCommand()
                                     , new ReturnCommand()}
            , Is.EquivalentTo(commands));

        }

        [Test]
        public void GetCommands_WorkflowHasItemsAndCurrentStateIsProvided_ReturnCommandsForTheCurrentState()
        {
            var wf = CreateSampleWorkFow();

            var commands = wf.GetCommands(q => q.ExpertReview);

            Assert.That(commands.Count(), Is.EqualTo(2));
            Assert.That(
                new TransitionItem[] { new ApproveCommand()
                                     , new ReturnCommand()}
            , Is.EquivalentTo(commands));

        }

        [Test]
        public void GetStateItem_StateExists_ReturnTheStateItem()
        {
            var wf = CreateSampleWorkFow();

            var state = wf.GetStateItem(2);

            Assert.That(state, Is.EqualTo(TestWFStates.Instance.ExpertReview));
        }

        [Test]
        public void GetCommandItem_CommandExists_ReturnTheCommandItem()
        {
            var wf = CreateSampleWorkFow();

            var command = wf.GetCommandItem(3);

            Assert.That(command, Is.EqualTo(WorkFlowCommands.Instance.ReturnBack));
        }

        private WorkFlow<TestWFStates, WorkFlowCommands> CreateSampleWorkFow()
        {
            return new WorkFlowBuilder<TestWFStates, WorkFlowCommands>()
                        .Add(wfItem => wfItem
                             .From(q => q.FormSubmission)
                             .If(q => q.Approve)
                             .GoTo(q => q.ExpertReview)

                        ).Add(wfItem => wfItem
                             .From(q => q.ExpertReview)
                             .If(q => q.Approve)
                             .GoTo(q => q.SupervisorReview)

                        ).Add(wfItem => wfItem
                             .From(q => q.SupervisorReview)
                             .If(q => q.Approve)
                             .GoTo(q => q.End)

                        ).Add(wfItem => wfItem
                             .From(q => q.SupervisorReview)
                             .If(q => q.Reject)
                             .GoTo(q => q.End)

                        ).Add(wfItem => wfItem
                             .From(q => q.SupervisorReview)
                             .If(q => q.ReturnBack)
                             .GoTo(q => q.ExpertReview)

                        ).Add(wfItem => wfItem
                             .From(q => q.ExpertReview)
                             .If(q => q.ReturnBack)
                             .GoTo(q => q.FormSubmission)
                        ).Build();

        }

        public class TestWFStates
        {

            public static TestWFStates Instance
            {
                get
                {

                    return new TestWFStates();
                }
            }

            public TransitionItem FormSubmission => new FormSubmission();

            public TransitionItem ExpertReview => new ExpertReview();

            public TransitionItem SupervisorReview => new SupervisorReview();

            public TransitionItem End => new End();
        }

        public class FormSubmission : TransitionItem
        {
            public override string Name => "Initial submission";
            public override int Value => 1;
        }

        public class ExpertReview : TransitionItem
        {
            public override string Name => "Expert review submission";
            public override int Value => 2;
        }
        public class SupervisorReview : TransitionItem
        {
            public override string Name => "Supervisor review submission";
            public override int Value => 3;
        }
        public class End : TransitionItem
        {
            public override string Name => "Completion";
            public override int Value => 4;
        }



        public class WorkFlowCommands
        {
            public static WorkFlowCommands Instance
            {
                get
                {

                    return new WorkFlowCommands();
                }
            }
            public TransitionItem Approve => new ApproveCommand();
            public TransitionItem Reject => new RejectCommand();
            public TransitionItem ReturnBack => new ReturnCommand();
            public TransitionItem ReturnToInitialState => new ReturnToInitialStateCommand();
        }
        public class ApproveCommand : TransitionItem
        {
            public override string Name => "Approve";
            public override int Value => 1;
        }

        public class RejectCommand : TransitionItem
        {
            public override string Name => "Reject";
            public override int Value => 2;
        }

        public class ReturnCommand : TransitionItem
        {
            public override string Name => "Return";
            public override int Value => 3;
        }

        public class ReturnToInitialStateCommand : TransitionItem
        {
            public override string Name => "Return to initial state";
            public override int Value => 4;
        }
    }


}