using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;

namespace System.Net
{
    public static class WebRequestExtensions
    {
        public static IObservable<Stream> RequestToStream(this IObservable<string> source, TimeSpan timeout)
        {
            return from wc in source.Select(WebRequest.Create)
                   from s in Observable.FromAsyncPattern<WebResponse>(wc.BeginGetResponse, wc.EndGetResponse)()
                                       .Timeout(timeout, Observable.Empty<WebResponse>())
                                       .Catch(Observable.Empty<WebResponse>())
                   select s.GetResponseStream();
        }

        public static IObservable<Stream> RequestToStream(this IObservable<Uri> source, TimeSpan timeout)
        {
            return from wc in source.Select(WebRequest.Create)
                   from s in Observable.FromAsyncPattern<WebResponse>(wc.BeginGetResponse, wc.EndGetResponse)()
                                       .Timeout(timeout, Observable.Empty<WebResponse>())
                                       .Catch(Observable.Empty<WebResponse>())
                   select s.GetResponseStream();
        }

        public static IObservable<Stream> FromRequestToStream(this WebRequest webRequest, TimeSpan timeout)
        {
            return from s in Observable.FromAsyncPattern<WebResponse>(webRequest.BeginGetResponse, webRequest.EndGetResponse)()
                                       .Timeout(timeout, Observable.Empty<WebResponse>())
                                       .Catch(Observable.Empty<WebResponse>())
                   select s.GetResponseStream();
        }
    }
}