// Reference from: https://gist.github.com/Farl/e20d22f31203c57a63e9902678f96bf5

/* Example:
var getRequest = UnityWebRequest.Get("http://www.google.com");
await getRequest.SendWebRequest();
var result = getRequest.downloadHandler.text;
*/

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace SS
{
    public static class UnityAsyncOperationAwaiterExtension
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}