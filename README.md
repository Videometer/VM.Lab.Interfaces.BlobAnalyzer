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

## WaitCondition
Defines how to wait when stopping. 

## IAutofeederControlListener
Interface to control an __autofeeder__ from an external plugin, the plugin could be used to control any granularDevice, but for now it is used for the autofeeder 

## AutofeederControl
Autofeeder communication controller, Types that implement this class will be detected and gives the implementer the ability to control the Autofeeder

Control loop for Blob Analyzers, the diagram shows when the Start,Stop and Flush command are possible, 
```mermaid
stateDiagram-v2 
  direction LR
* --> idle
idle --> Running : Start
Running --> Flushing : Low coverage
Running --> Stopping : Stop
Flushing --> Stopping : Done / Stop
Stopping --> Stopped
Stopped --> Flushing : Flush
Stopped --> idle : Finish (Save results) 
```

Class diagram overview
```mermaid
classDiagram
    
class IAutofeederControlListener {
    <<interface>>
    Task Start(string id, string initials, string comments);
    Task Stop(WaitCondition waitCondition, bool doFlush = false)
    Task Flush();
    Task Finish();
}

class AutofeederControl {
    <<Abstract>>
	#IAutofeederControlListener _listener
    +AutofeederControl(IAutofeederControlListener listener)
    +void StateChanged(AutofeederState oldState, AutofeederState newState, string sampleId, DataTable result)*
    +string GetBlobCollectionSubfolder(DateTime measurementStartTime)*
    +string GetPredictionResultFilename(DateTime measurementStartTime)*
}
AutofeederControl *-- IAutofeederControlListener
AutofeederState --> AutofeederControl
class AutofeederState {
    <<Enum>>
    +Idle
    +Running
    +Flushing
    +Stopping
    +Stopped
}

WaitCondition --> IAutofeederControlListener
class WaitCondition {
    <<Enum>>
	+DontWait,
    +Wait_All_Queues_Empty,
    +Wait_CaptureThread_Done
}
```

