using System;
using System.Threading;
using System.Threading.Tasks;
using Chartboost.Logging;
using UnityEditor;
using UnityEngine;

namespace Chartboost
{
    /// <summary>
    /// <para>Utility class to dispatch functionality into Unity's main thread.</para>
    /// If your code contains references to any Unity Code and not just raw C#, you will have to dispatch it to the Unity main thread.
    /// </summary>
    public static class MainThreadDispatcher {
        
        /// <summary>
        /// Synchronous; blocks until the callback completes
        /// </summary>
        public static void Send(SendOrPostCallback callback) => _context.Send(callback, null);

        /// <summary>
        /// Asynchronous; send and forget
        /// </summary>
        public static void Post(SendOrPostCallback callback) => _context.Post(callback, null);

        /// <summary>
        /// Creates and dispatches a <see cref="Task"/> to run on the Unity Scheduler.
        /// </summary>
        /// <param name="task">The <see cref="Action"/> to be executed on the Unity Scheduler.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation. This <see cref="Task"/>
        /// will log any exceptions that occur during its execution using the Unity logging system.
        /// </returns>
        public static Task MainThreadTask(Action task)
        {
            var ret = Task.Factory.StartNew(task, CancellationToken.None, TaskCreationOptions.None, _unityScheduler);
            ret.AppendExceptionLogging();
            return ret;
        }
        
        /// <summary>
        /// Creates and dispatches a <see cref="Task"/> to run on the Unity Scheduler.
        /// </summary>
        /// <param name="task">The <see cref="Func{Task}"/> to be executed on the Unity Scheduler.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation. This <see cref="Task"/>
        /// will log any exceptions that occur during its execution using the Unity logging system.
        /// </returns>
        public static Task MainThreadTask(Func<Task> task)
        {
            var ret = Task.Factory.StartNew(task, CancellationToken.None, TaskCreationOptions.None, _unityScheduler).Unwrap();
            ret.AppendExceptionLogging();
            return ret;
        }
        
        /// <summary>
        /// Creates and dispatches a <see cref="Task"/> with a parameter to run on the Unity Scheduler.
        /// </summary>
        /// <typeparam name="T">The type of the parameter passed to the task.</typeparam>
        /// <param name="task">The <see cref="Func{Object, Task}"/> to be executed on the Unity Scheduler.</param>
        /// <param name="parameter">The parameter to pass to the task.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation. This <see cref="Task"/>
        /// will log any exceptions that occur during its execution using the Unity logging system.
        /// </returns>
        public static Task MainThreadTask<T>(Func<object, Task> task, T parameter)
        {
            var ret = Task.Factory.StartNew(task, parameter, CancellationToken.None, TaskCreationOptions.None, _unityScheduler).Unwrap();
            ret.AppendExceptionLogging();
            return ret;
        }
        
        /// <summary>
        /// Creates and dispatches a <see cref="Task{TResult}"/> to run on the Unity Scheduler.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
        /// <param name="task">The <see cref="Func{Task{TResult}}"/> to be executed on the Unity Scheduler.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. This <see cref="Task"/>
        /// will log any exceptions that occur during its execution using the Unity logging system and will 
        /// return the result of type <typeparamref name="TResult"/> upon completion.
        /// </returns>
        public static Task<TResult> MainThreadTask<TResult>(Func<Task<TResult>> task)
        {
            var ret = Task.Factory.StartNew(task, CancellationToken.None, TaskCreationOptions.None, _unityScheduler).Unwrap();
            ret.AppendExceptionLogging();

            return ret;
        }

        /// <summary>
        /// Creates and dispatches a <see cref="Task{TResult}"/> with a parameter to run on the Unity Scheduler.
        /// </summary>
        /// <typeparam name="T">The type of the parameter passed to the task.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
        /// <param name="task">The <see cref="Func{Object, Task{TResult}}"/> to be executed on the Unity Scheduler.</param>
        /// <param name="parameter">The parameter to pass to the task.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. This <see cref="Task"/>
        /// will log any exceptions that occur during its execution using the Unity logging system and will
        /// return the result of type <typeparamref name="TResult"/> upon completion.
        /// </returns>
        public static Task<TResult> MainThreadTask<T, TResult>(Func<object, Task<TResult>> task, T parameter)
        {
            var ret = Task.Factory.StartNew(task, parameter, CancellationToken.None, TaskCreationOptions.None, _unityScheduler).Unwrap();
            ret.AppendExceptionLogging();
            return ret;
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously, on the Unity main thread, when the target <see cref="Task{T}"/> completes.
        /// </summary>
        /// <param name="task">Target <see cref="Task"/>.</param>
        /// <param name="continuation">An action to run when the <see cref="Task"/> completes. </param>
        /// <typeparam name="T">The type of the result produced by the <see cref="Task"/>.</typeparam>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        public static Task ContinueWithOnMainThread<T>(this Task<T> task, Action<Task<T>> continuation)
        {
            var ret = Task.Factory.StartNew(async () =>
            {
                await task;
                continuation.Invoke(task);
            }, CancellationToken.None, TaskCreationOptions.None, _unityScheduler).Unwrap();
            ret.AppendExceptionLogging();
            return ret;
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously, on the Unity main thread, when the target <see cref="Task"/> completes.
        /// </summary>
        /// <param name="task">Target <see cref="Task"/>.</param>
        /// <param name="continuation">An action to run when the <see cref="Task"/> completes. </param>
        /// <typeparam name="T">The type of the result produced by the <see cref="Task"/>.</typeparam>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        public static Task ContinueWithOnMainThread(this Task task, Action<Task> continuation)
        {
            var ret = Task.Factory.StartNew(async () =>
            {
                await task;
                continuation.Invoke(task);
            }, CancellationToken.None, TaskCreationOptions.None, _unityScheduler).Unwrap();
            ret.AppendExceptionLogging();
            return ret;
        }

        private static void AppendExceptionLogging(this Task inputTask) 
            => inputTask.ContinueWith(faultedTask => LogController.LogException(faultedTask.Exception), TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

        private static SynchronizationContext _context;
        private static TaskScheduler _unityScheduler;

        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void _initialize()
        {
            _context ??= SynchronizationContext.Current;
            _unityScheduler ??= TaskScheduler.FromCurrentSynchronizationContext();
        }
    }
}
