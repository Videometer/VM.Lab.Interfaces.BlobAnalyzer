# VM.Lab.Interfaces.BlobAnalyzer
Defines common available interfaces to VideometerLab Blob analyzers, example blob analyzers are 

<!-- TOC -->
* [VM.Lab.Interfaces.BlobAnalyzer](#vmlabinterfacesblobanalyzer)
  * [Introduction](#introduction)
  * [AutofeederState](#autofeederstate)
  * [WaitCondition](#waitcondition)
  * [IAutofeederControlListener](#iautofeedercontrollistener)
  * [AutofeederControl](#autofeedercontrol)
<!-- TOC -->

## Introduction
Defines common available interfaces to VideometerLab Blob analyzers, example blob analyzers are
* VideometerLab Autofeeder blobAnalyzer
* VideometerLab X/Y blobAnalyzer

Notice, that interface is called Autofeeder* in many cases, this is due to legacy inheritance, the plugin can be used to controll both Autofeeder Blob analyzer and Videometerlab X/Y Blob Analyzer

The interface exposes the basic features that is available from the user interface

## AutofeederState
Represents the state of the autofeeder, used to communicate state transitions to the controller

## IAutofeederControlListener
Interface to control an __autofeeder__ from an external plugin, the plugin could be used to control any granularDevice, but for now it is used for the autofeeder 

## AutofeederControl
Autofeeder communication controller, Types that implement this class will be detected and gives the implementer the ability to control the Autofeeder

Control loop for Blob Analyzers, the diagram shows when the Start,Stop and Flush command are possible, 
```mermaid
stateDiagram-v2
direction LR
None --> LOADING_RECIPE : LoadRecipe
LOADING_RECIPE --> None : FailedLoadingRecipe

LOADING_RECIPE --> IDLE : RecipeLoaded
IDLE --> STARTING_MEASUREMENT : StartMeasurement
STARTING_MEASUREMENT --> IDLE : Stop

IDLE --> FLUSHING_IDLE : BeginFlush
FLUSHING_IDLE --> IDLE : FlushDone

None --> FLUSHING_NONE : BeginFlush
FLUSHING_NONE --> None : FlushDone
STOPPED -->  FLUSHING_STOPPED : BeginFlush

STARTING_MEASUREMENT --> MEASURING : StartingDone
MEASURING --> STOPPING_PIPELINE : Stop
STOPPING_PIPELINE --> STOPPED : Stop

IDLE --> LOADING_RECIPE : LoadRecipe

STOPPED --> RESUMING_MEASUREMENT : StartMeasurement
RESUMING_MEASUREMENT --> MEASURING : StartMeasurement

RESUMING_MEASUREMENT --> STOPPED : Stop

MEASURING --> STOPPING_PIPELINE_FLUSH : ReachedEndPattern
STOPPING_PIPELINE_FLUSH --> FLUSHING_STOPPED : BeginFlush
FLUSHING_STOPPED --> STOPPED : FlushDone
STOPPED --> FINALIZING_MEASUREMENT : FinishMeasurement
FINALIZING_MEASUREMENT --> IDLE : SavingComplete

```

Class diagram overview
```mermaid
classDiagram
    
class IAutofeederControlListener {
    <<interface>>
    Task LoadRecipe(string recipeName)
    Task Start(string sampleId, string initials, string comments, ResultSpecification resultSavingSpecification);
    Task Stop(WaitCondition waitCondition, bool doFlush = false)
    Task Flush();
    Task Finish();
}

AutofeederControl -- ResultSpecification
class ResultSpecification {
  +string PredictionResultFilename
  +string BlobCollectionFolder
}

class AutofeederControl {
    <<Abstract>>
	#IAutofeederControlListener _listener
    +AutofeederControl(IAutofeederControlListener listener)
    +void StateChanged(AutofeederState oldState, AutofeederState newState)*
}
AutofeederControl *-- IAutofeederControlListener
AutofeederState --> AutofeederControl
class AutofeederState {
    <<Enum>>
      +None
      +LOADING_RECIPE
      +IDLE
      +STARTING_MEASUREMENT
      +MEASURING
      +RESUMING_MEASUREMENT
      +STOPPING_PIPELINE
      +STOPPING_PIPELINE_FLUSH
      +STOPPED
      +FINALIZING_MEASUREMENT
      +FLUSHING
      +FLUSHING_IDLE
      +FLUSHING_NONE
      +FLUSHING_STOPPED
}

```

```csharp
public interface IAutofeederControlListener
{
    /// <summary>
    /// Loads a recipe
    /// if an absolute path (local or cloud), the recipe is loaded given the absolute path
    /// if not an absolute path, the recipe is loaded from the current active workspace
    /// </summary>
    /// <param name="recipeName"></param>
    /// <returns></returns>
    Task LoadRecipe(string recipeName);
    
    /// <summary>Start a new measurement</summary>
    /// <param name="id">ID of the sample</param>
    /// <param name="initials">Operator initials</param>
    /// <param name="comments">Operator comments</param>
    Task Start(string id, string initials, string comments, ResultSpecification resultSpecification);

    /// <summary>Stop/Pause the current measurement</summary>
    /// <param name="waitCondition">What to wait on when stopping</param>
    /// <param name="doFlush">Controls if a flush is done during stopping of the autofeeder.
    /// This is wanted e.g. in the case of stopping due to low coverage.</param>
    Task Stop();

    /// <summary>Flush the conveyor</summary>
    Task Flush();

    /// <summary>Finish the actual measurement</summary>
    Task Finish();
}
```
