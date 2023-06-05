using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SS.Core
{
    public class CustomSettingTest : MonoBehaviour
    {
        public Toggle toggle;
        public Text text;

        public bool IsLoadAsync => (toggle != null) ? toggle.isOn : false;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            if (IsLoadAsync)
            {
                var oh = MyCustomSettings.LoadAsync((x) =>
                {
                    Debug.Log("Load Complete!");
                });
                while (!oh.IsDone)
                {
                    yield return null;
                }
                Debug.Log("Start() Finished");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (text)
            {
                if (!IsLoadAsync || MyCustomSettings.IsExist)
                {

                    text.text = MyCustomSettings.Instance.Number.ToString();
                }
                else
                {
                    text.text = "Not found";
                }
            }
        }
    }

}
