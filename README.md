# Simple Workflow Designer

This is a workflow designer developed using C#.NET, providing a simple and extensible solution for managing workflows.

## Table of Contents

- [Simple Workflow Designer](#simple-workflow-designer)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Classes](#classes)
	- [TransitionItem Class](#transitionitem-class)
	- [WorkFlowTransition Class](#workflowtransition-class)
	- [WorkFlow Class](#workflow-class)
	- [WorkFlowBuilder Class](#workflowbuilder-class)
  - [Save workflow states and commands in a Database](#save-workflow-states-and-commands-in-a-database)


## Overview

In each workflow, there are a series of states and commands that can be executed.
To go from one state to another, a command must be executed. In this workflow designer, each state and command is represented by a class that inherits from the `TransitionItem` abstract base class.
Before we can create a workflow, we must first create the states and commands that will be used in the workflow.
Then we can use Fluent API to create a workflow using the `WorkFlowBuilder` class.
The `WorkFlowBuilder` class provides methods for adding states and commands to the workflow, as well as methods for adding transitions between states.
Once the workflow has been created, we can execute commands and move between states using the `WorkFlow` class. 


The Simple Workflow Designer consists of several key classes, including `TransitionItem`, `WorkFlow`, `WorkFlowBuilder`, `WorkFlowItemBuilder`, `WorkFlowTransition`, and `WorkFlowTests`. These classes provide a foundation for designing and managing workflows in C#.NET.

## Classes

### TransitionItem Class

The `TransitionItem` class is an abstract base class representing individual states or commands within the workflow.

You can add or remove properties as needed to suit your workflow scenario. For example, if you want to add roles to your workflow and restrict access to those roles, you can add a `Role` property to the `TransitionItem` class.
#### Usage Example:

```csharp
public class CustomState : TransitionItem
{
    public override int Value => 5;
    public override string Name => "Custom State";
}
```

### WorkFlowTransition Class

The `WorkFlowTransition` class represents a specific transition in the workflow, including properties for `CurrentState`, `NextState`, and `Command`.
#### Usage Example:

```csharp
public class CustomState : TransitionItem
{
    public override int Value => 5;
    public override string Name => "Custom State";
}
```
### WorkFlow Class

The `WorkFlow` class provides methods such as `GetNextState` and `GetCommands` based on the current state to be used in each form that is part of the workflow.
You can add more functionality to this class as needed to suit your workflow scenario.

### WorkFlowBuilder Class

The `WorkFlowBuilder` class provides methods for adding states and commands to the workflow, as well as methods for adding transitions between states. 
This is the class that will be used to create the workflow.

#### Usage Example:

```csharp
var wf = new WorkFlowBuilder<WFStates, WorkFlowCommands>()
                        .Add(wfItem => wfItem
                             .From(q => q.FormSubmission)
                             .If(q => q.Approve)
                             .GoTo(q => q.ExpertReview)

                        ).Add(wfItem => wfItem
                             .From(q => q.ExpertReview)
                             .If(q => q.Approve)
                             .GoTo(q => q.SupervisorReview)
                         .Build();
```

## Save workflow states and commands in a Database

We can keep track of all the states and commands in a workflow by saving them in a table in a database.
To do this, we can create a table with columns for `Id`, `State`, `Command`, `Date`, and `Description`.(add fields as required)
Then for each state and command in the workflow, we can insert a row into the table with the corresponding values.



