using log4net;
using System;
using System.Diagnostics;

namespace Abot2.Util
{
    public interface IMemoryMonitor : IDisposable
    {
        int GetCurrentUsageInMb();
    }

    public class GcMemoryMonitor : IMemoryMonitor
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public virtual int GetCurrentUsageInMb()
        {
            var timer = Stopwatch.StartNew();
            var currentUsageInMb = Convert.ToInt32(GC.GetTotalMemory(false) / (1024 * 1024));
            timer.Stop();

            Log.DebugFormat("GC reporting [{0}mb] currently thought to be allocated, took [{1}] millisecs", currentUsageInMb, timer.ElapsedMilliseconds);

            return currentUsageInMb;       
        }

        public void Dispose()
        {
            //do nothing
        }
    }
}
