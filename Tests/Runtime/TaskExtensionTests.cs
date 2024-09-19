using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime
{
    public class TaskTests
    {
        private const string DummyTaskException = "Dummy Task Exception";
        private const string ExpectedTestException = "ExpectedTestException";
        private const string OnMainThreadException = "OnMainThread";
        private const string NotOnMainThreadException = "NotOnMainThread";

        private const int IntParam = 1;
        private bool _throwException;

        [UnityTest]
        public IEnumerator TestContinueWithMainThreadCompletion()
        {
            var task = DummyTask(false).ContinueWithOnMainThread(t => Debug.Log("Task complete"));

            Debug.Log("Awaiting task completion....");
            yield return new WaitUntil(() => task is { IsCompleted: true });
            
            Assert.False(task.Status == TaskStatus.Faulted);
            Assert.True(task.Status == TaskStatus.RanToCompletion);
        }
        
        [UnityTest]
        public IEnumerator TestContinueWithMainThreadException()
        {
            var task = DummyTask(true).ContinueWithOnMainThread(t => Debug.Log("Task complete"));
            
            Debug.Log("Awaiting task completion....");
            yield return new WaitUntil(() => task.IsCompleted);
            
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {DummyTaskException}*"));
            
            Assert.True(task.Status == TaskStatus.Faulted);
            Assert.False(task.Status == TaskStatus.RanToCompletion);
        }
        
        [UnityTest]
        public IEnumerator TestAction()
        {
            // First run without throwing an exception
            Task task = null;
            RunOnNonMainThread(() => task = MainThreadDispatcher.MainThreadTask(Action_));
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => task = MainThreadDispatcher.MainThreadTask(Action_));
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            _throwException = false;
        }
        
        [UnityTest]
        public IEnumerator TestFunc()
        {
            // First run without throwing an exception
            Task task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(Func_);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(Func_);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            _throwException = false;
        }
        
        [UnityTest]
        public IEnumerator TestFuncAsync()
        {
            // First run without throwing an exception
            Task task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncAsync);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncAsync);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            
            _throwException = false;
        }
        
        [UnityTest]
        public IEnumerator TestFuncWithParam()
        {
            // First run without throwing an exception
            Task task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithParam, IntParam);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithParam, IntParam);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            
            _throwException = false;

        }
        
        [UnityTest]
        public IEnumerator TestFuncWithParamAsync()
        {
            // First run without throwing an exception
            Task task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithParamAsync, IntParam);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithParamAsync, IntParam);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            
            _throwException = false;
        }
        
        [UnityTest]
        public IEnumerator TestFuncWithReturn()
        {
            // First run without throwing an exception
            Task<int> task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithReturn<int>);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithReturn<int>);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            
            _throwException = false;
        }
        
        [UnityTest]
        public IEnumerator TestFuncWithReturnAsync()
        {
            // First run without throwing an exception
            Task<int> task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithReturnAsync<int>);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithReturnAsync<int>);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            
            _throwException = false;
        }
        
        [UnityTest]
        public IEnumerator TestFuncWithParamAndReturn()
        {
            // First run without throwing an exception
            Task<int> task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithParamAndReturn<int, int>, IntParam);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithParamAndReturn<int, int>, IntParam);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            
            _throwException = false;
        }
        
        [UnityTest]
        public IEnumerator TestFuncWithParamAndReturnAsync()
        {
            // First run without throwing an exception
            Task<int> task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithParamAndReturnAsync<int, int>, IntParam);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNull(task.Exception);

            // Second run with throwing an exception
            _throwException = true;
            task = null;
            RunOnNonMainThread(() => { task = MainThreadDispatcher.MainThreadTask(FuncWithParamAndReturnAsync<int, int>, IntParam);});
            yield return new WaitUntil(() => task is { IsCompleted: true });
            Assert.IsNotNull(task.Exception);
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {ExpectedTestException}*"));
            
            _throwException = false;
        }

        private void Action_()
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);
            
            Debug.Log($"_throwException set to : {_throwException}");
            
            if (_throwException)
                throw new Exception(ExpectedTestException);
            
            Debug.Log("Action_ complete");
        }

        private Task Func_()
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);

            if (_throwException)
            {
                Debug.Log($"Throwing exception {ExpectedTestException}");
                throw new Exception(ExpectedTestException);
            }

            Debug.Log("Func_ complete");
            return Task.CompletedTask;
        }

        private Task FuncWithParam<T>(T param)
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);

            if (_throwException)
                throw new Exception(ExpectedTestException);

            Debug.Log("FuncWithParam complete");
            return Task.CompletedTask;  // we don't need a return if task is async
        }

        private Task<T> FuncWithReturn<T>()
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);

            if (_throwException)
                throw new Exception(ExpectedTestException);
            
            Debug.Log("FuncWithReturn complete");
            return Task.FromResult<T>(default);
        }
        
        private Task<TResult> FuncWithParamAndReturn<T, TResult>(object param)
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);

            if (_throwException)
                throw new Exception(ExpectedTestException);
            
            Debug.Log("FuncWithParamAndReturn complete");
            
            return Task.FromResult((TResult)param);
        }
        
        private async Task FuncAsync()
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);

            if (_throwException)
                throw new Exception(ExpectedTestException);
            
            await Task.Delay(1000);
            
            Debug.Log("FuncAsync complete");
        }

        private async Task FuncWithParamAsync<T>(T param)
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);

            if (_throwException)
                throw new Exception(ExpectedTestException);
            
            await Task.Delay(1000);
            Debug.Log("FuncWithParamAsync complete");

        }

        private async Task<T> FuncWithReturnAsync<T>()
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);
            
            await Task.Delay(1000);

            if (_throwException)
                throw new Exception(ExpectedTestException);

            Debug.Log("FuncWithReturnAsync complete");

            return default;
        }

        private async Task<TResult> FuncWithParamAndReturnAsync<T, TResult>(object param)
        {
            // If not running on main thread
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                throw new Exception(NotOnMainThreadException);
            
            await Task.Delay(1000);

            if (_throwException)
                throw new Exception(ExpectedTestException);
            
            Debug.Log("FuncWithParamAndReturnAsync complete");

            return (TResult)param;
        }
        
        private async Task DummyTask(bool throwException)
        {
            await System.Threading.Tasks.Task.Delay(1000);

            if (throwException)
            {
                throw new Exception(DummyTaskException);
            }
        }
        
        private void RunOnNonMainThread(Action action)
        {
            async void ThreadStart()
            {
                // Make sure to await here.
                // Until await this would still be on main thread
                Debug.Log("Waiting for a second in non main thread");
                await Task.Delay(1000);

                // If still running on main thread
                if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    Debug.LogError(OnMainThreadException);

                Debug.Log("Running non thread action");
                action();
            }

            var thread = new Thread(ThreadStart);
            thread.Start();
        }
    }
}
