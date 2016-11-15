using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Corvalius.Common;

namespace System.Reactive
{
    //public static class ObservableHelpers
    //{
    //    public static IObservable<TResult> CatchObserverError<TResult>(this IObservable<TResult> source, Action<Exception> handler)
    //    {
    //        return CatchObserverError<TResult, Exception>(source, handler);
    //    }

    //    public static IObservable<TResult> CatchObserverError<TResult, TException>(this IObservable<TResult> source, Action<TException> handler)
    //        where TException : Exception
    //    {
    //        return Observable.Create<TResult>(o =>
    //            source.Subscribe(
    //                n => { try { o.OnNext(n); } catch (TException ex) { handler(ex); } },
    //                e => { try { o.OnError(e); } catch (TException ex) { handler(ex); } },
    //                () => { try { o.OnCompleted(); } catch (TException ex) { handler(ex); } }
    //            )
    //        );
    //    }

    //    public static IObservable<TResult> CatchObserverError<TResult, TException>(this IObservable<TResult> source, Action<Notification<TResult>, TException> handler)
    //        where TException : Exception
    //    {
    //        return Observable.Create<TResult>(o =>
    //            source.Subscribe(
    //                n =>
    //                {
    //                    try { o.OnNext(n); }
    //                    catch (TException ex)
    //                    { handler(new Notification<TResult>.OnNext(n), ex); }
    //                },
    //                e =>
    //                {
    //                    try { o.OnError(e); }
    //                    catch (TException ex)
    //                    { handler(new Notification<TResult>.OnError(e), ex); }
    //                },
    //                () =>
    //                {
    //                    try { o.OnCompleted(); }
    //                    catch (TException ex)
    //                    { handler(new Notification<TResult>.OnCompleted(), ex); }
    //                }
    //            )
    //        );
    //    }
    //}

    ///// <summary>
    ///// Wraps an scheduler with a safe scheduler that will log exceptions.
    ///// </summary>
    ///// <seealso cref="http://www.jaylee.org/post/2010/09/06/WP7Dev-Reactive-Safer-Reactive-Extensions.aspx"/>
    //public static class SafeSchedulerExtensions
    //{
    //    public static IScheduler AsSafe(this IScheduler scheduler)
    //    {
    //        return new SafeScheduler(scheduler);
    //    }

    //    private class SafeScheduler : IScheduler
    //    {
    //        private readonly IScheduler source;

    //        public SafeScheduler(IScheduler scheduler)
    //        {
    //            this.source = scheduler;
    //        }

    //        public DateTimeOffset Now { get { return source.Now; } }

    //        public IDisposable Schedule(Action action, TimeSpan dueTime)
    //        {
    //            return source.Schedule(Wrap(action), dueTime);
    //        }

    //        public IDisposable Schedule(Action action)
    //        {
    //            return source.Schedule(Wrap(action));
    //        }

    //        private Action Wrap(Action action)
    //        {
    //            return () =>
    //            {
    //                try
    //                {
    //                    action();
    //                }
    //                catch
    //                {
    //                    // Log and report the exception.
    //                    DebuggingHelper.Break();
    //                    // TODO: Report to the Error Reporting Component on Visual Studio.
    //                }
    //            };

    //        }
    //    }
    //}
}
