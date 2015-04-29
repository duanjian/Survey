using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DFHE.Survey.API.Handler
{
    public class DirectUrlReqHandler : DelegatingHandler
    {
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {

            if (!request.RequestUri.OriginalString.Contains("api"))
            {
                var hrm = new HttpResponseMessage(HttpStatusCode.Redirect);
                hrm.Headers.Location = new Uri("/index.html", UriKind.Relative);
                return Task.Factory.StartNew<HttpResponseMessage>(() => { return hrm; }, cancellationToken);
            }
            else
            {
                return base.SendAsync(request, cancellationToken);                
            }

            //int matchHeaderCount = request.Headers.Count((item) =>
            //{
            //    if ("key".Equals(item.Key))
            //    {
            //        foreach (var str in item.Value)
            //        {
            //            if ("11234".Equals(str))
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //    return false;
            //});
            //if (matchHeaderCount > 0)
            //{
            //    return base.SendAsync(request, cancellationToken);
            //}
            //return Task.Factory.StartNew<HttpResponseMessage>(() => { return new HttpResponseMessage(HttpStatusCode.Forbidden); });
        }

        
    }
}