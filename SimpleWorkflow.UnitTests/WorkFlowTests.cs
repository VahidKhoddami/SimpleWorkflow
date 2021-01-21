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
            var wf = new WorkFlowBuilder<TestWFStates, WorkFlowConditions>();

            var ex = Assert.Throws<Exception>(() => wf.Add(null));
            Assert.That(ex.Message, Contains.Substring("builder is not defined").IgnoreCase);

        }

        [Test]
        public void Add_BuilderConditionNotDefined_ReturnException()
        {
            var wf = new WorkFlowBuilder<TestWFStates, WorkFlowConditions>();

            var ex = Assert.Throws<Exception>(() =>
            wf.Add(q => q.From(q => q.FormSubmission)
                         .GoTo(q => q.ExpertReview)));
            Assert.That(ex.Message, Contains.Substring("'If' statement is not defined").IgnoreCase);

        }

        [Test]
        public void Add_BuilderCurrentStateNotDefined_ReturnException()
        {
            var wf = new WorkFlowBuilder<TestWFStates, WorkFlowConditions>();

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

            Assert.AreEqual(new SupervisorReview(), nextState);
        }

        [Test]
        public void GetNext_NextStateIsNotDefined_ReturnNull()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(q => q.ExpertReview, q => q.Reject);

            Assert.AreEqual(null, nextState);

        }

        [Test]
        public void GetNext_WorkflowHasNoItem_ReturnNull()
        {
            var wf = new  WorkFlowBuilder<TestWFStates, WorkFlowConditions>().Build();

            var nextState = wf.GetNextState(q => q.FormSubmission, q => q.Reject);

            Assert.AreEqual(null, nextState);

        }

        [Test]
        public void GetNext_CurrentStateIsNull_ReturnNull()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(null, q => q.Reject);

            Assert.AreEqual(null, nextState);

        }

        [Test]
        public void GetNext_ConditionIsNull_ReturnNull()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(q => q.FormSubmission, null);

            Assert.AreEqual(null, nextState);

        }

        [Test]
        public void GetNext_ProvidedWithIntegerStateAndCondition_ReturnNextStateItem()
        {
            var wf = CreateSampleWorkFow();

            var nextState = wf.GetNextState(2, 1);

            Assert.AreEqual(3, nextState.Value);

        }

        [Test]
        public void GetConditions_WorkflowHasNoItem_ReturnNull()
        {
            var wf = new WorkFlowBuilder<TestWFStates, WorkFlowConditions>().Build();

            var conditions = wf.GetConditions();

            Assert.AreEqual(Enumerable.Empty<TransitionItem>(), conditions);

        }

        [Test]
        public void GetConditions_WorkflowHasItems_ReturnConditions()
        {
            var wf = CreateSampleWorkFow();

            var conditions = wf.GetConditions();

            Assert.IsTrue(conditions.Count() == 3);
            Assert.That(
                new TransitionItem[] { new ApproveCondition()
                                     , new RejectCondition()
                                     , new ReturnCondition()}
            , Is.EquivalentTo(conditions));

        }

        [Test]
        public void GetConditions_WorkflowHasItemsAndCurrentStateIsProvided_ReturnConditionsForTheCurrentState()
        {
            var wf = CreateSampleWorkFow();

            var conditions = wf.GetConditions(q => q.ExpertReview);

            Assert.AreEqual(2, conditions.Count());
            Assert.That(
                new TransitionItem[] { new ApproveCondition()
                                     , new ReturnCondition()}
            , Is.EquivalentTo(conditions));

        }

        [Test]
        public void GetStateItem_StateExists_ReturnTheStateItem()
        {
            var wf = CreateSampleWorkFow();

            var state = wf.GetStateItem(2);

            Assert.AreEqual(TestWFStates.Instance.ExpertReview, state);
        }

        [Test]
        public void GetCommandItem_CommandExists_ReturnTheCommandItem()
        {
            var wf = CreateSampleWorkFow();

            var command = wf.GetCommandItem(3);

            Assert.AreEqual(WorkFlowConditions.Instance.ReturnBack, command);
        }

        private WorkFlow<TestWFStates, WorkFlowConditions> CreateSampleWorkFow()
        {
            return new WorkFlowBuilder<TestWFStates, WorkFlowConditions>()
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
            public override string Name => "ثبت فرم اولیه";
            public override int Value => 1;
        }

        public class ExpertReview : TransitionItem
        {
            public override string Name => "ثبت نظر کاربر خبره";
            public override int Value => 2;
        }
        public class SupervisorReview : TransitionItem
        {
            public override string Name => "ثبت نظر ناظر";
            public override int Value => 3;
        }
        public class End : TransitionItem
        {
            public override string Name => "پایان جریان کار";
            public override int Value => 4;
        }



        public class WorkFlowConditions
        {
            public static WorkFlowConditions Instance
            {
                get
                {

                    return new WorkFlowConditions();
                }
            }
            public TransitionItem Approve => new ApproveCondition();
            public TransitionItem Reject => new RejectCondition();
            public TransitionItem ReturnBack => new ReturnCondition();
            public TransitionItem ReturnToInitialState => new ReturnToInitialStateCondition();
        }
        public class ApproveCondition : TransitionItem
        {
            public override string Name => "تایید";
            public override int Value => 1;
        }

        public class RejectCondition : TransitionItem
        {
            public override string Name => "رد";
            public override int Value => 2;
        }

        public class ReturnCondition : TransitionItem
        {
            public override string Name => "بازگشت جهت اصلاح";
            public override int Value => 3;
        }

        public class ReturnToInitialStateCondition : TransitionItem
        {
            public override string Name => "بازگشت به مرحله اول";
            public override int Value => 4;
        }
    }


}