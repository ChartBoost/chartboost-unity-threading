# Chartboost Threading Utilities for Unity
A way of dispatching functions to the main thread in Unity projects. Useful for functions that Unity limits to the main thread from different threads.

# Installation
This package is meant to be a dependency for other Chartboost Packages; however, if you wish to use it by itself, it can be installed through UPM & NuGet as follows:

## Using the public [npm registry](https://www.npmjs.com/search?q=com.chartboost.unity.threading)
```json
"dependencies": {
    "com.chartboost.unity.threading": "1.0.1",
    ...
},
"scopedRegistries": [
{
    "name": "NpmJS",
    "url": "https://registry.npmjs.org",
    "scopes": [
    "com.chartboost"
    ]
}
]
```

## Using the public [NuGet package](https://www.nuget.org/packages/Chartboost.CSharp.Threading.Unity)

To add the Chartboost Core Unity SDK to your project using the NuGet package, you will first need to add the [NugetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) package into your Unity Project.

This can be done by adding the following to your Unity Project's ***manifest.json***

```json
  "dependencies": {
    "com.github-glitchenzo.nugetforunity": "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity",
    ...
  },
```

Once <code>NugetForUnity</code> is installed, search for `Chartboost.CSharp.Threading.Unity` in the search bar of Nuget Explorer window(Nuget -> Manage Nuget Packages).
You should be able to see the `Chartboost.CSharp.Threading.Unity` package. Choose the appropriate version and install.
# Usage

## Simple Actions
Utilize the following methods to execute calls on the main thread:

```csharp
void TestAction(object state){
    //Execute logic on main thread
    Debug.Log("This is called in the main thread")
}

// Synchronous; blocks until the callback completes
MainThreadDispatcher.Send(TestAction);

// Asynchronous; send and forget
MainThreadDispatcher.Post(TestAction)
```

## Tasks
Taks can be utilized in Unity. However, if they contain code that must run on the Unity main thread, the Task too should also be run in the main thread. Use the following:

```csharp
MainThreadDispatcher.MainThreadTask(async () =>{
    // Mostly useful when calling task initially from outside of the Unity environment
    await myTask();
});
```

## Task Continuations
Task continuations are useful when trying to call asynchronous code from a synchronous environment. The following examples represent the same logic.

```csharp
private void MySyncrhonousMethod(){
    MyAsynchornousTask().ContinueWithOnMainThread(taskContinuationResultTask => {
        // perform any continuation logic here.
        Debug.Log("My task finished!")
    });
}

private async void MyAsyncrhonousMethod(){
    var taskResult = awat MyAsynchronousTask();
    Debug.Log("My task finished!")
}
```