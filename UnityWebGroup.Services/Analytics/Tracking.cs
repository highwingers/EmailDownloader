using Gappalytics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityWebGroup.Services.Analytics
{
    public sealed class Tracking : ITracking
    {
        private AnalyticsSession tracker;
        public Tracking(string sitename, string siteid)
        {
            tracker = new AnalyticsSession(sitename,siteid);
        }


        public  void TrackPage(string page, string title)
        {            
            var p = tracker.CreatePageViewRequest("App-" + page, title);
            p.Send();
        }
    }
}
