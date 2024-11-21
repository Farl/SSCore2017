// Create by Claude.AI

using System.Threading.Tasks;
using UnityEngine;

namespace SS
{
    // Extension method to handle Task without waiting
    public static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    Debug.LogError($"Unhandled exception in forgotten task: {t.Exception}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}