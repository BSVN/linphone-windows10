﻿using Windows.ApplicationModel.Background;

namespace LinphoneTasks
{
    public sealed class PhoneCallTask : IBackgroundTask
    {
        private BackgroundTaskDeferral mDeferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            mDeferral = taskInstance.GetDeferral();
            CurrentOperation.PhoneCallTaskDeferral = mDeferral;
            taskInstance.Canceled += TaskInstance_Canceled;
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (mDeferral != null)
            {
                mDeferral.Complete();
            }
            CurrentOperation.PhoneCallTaskDeferral = null;
        }
    }
}
