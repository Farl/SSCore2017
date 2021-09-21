// /* (C)2021 Rayark Inc. - All Rights Reserved
//  * Rayark Confidential
//  *
//  * NOTICE: The intellectual and technical concepts contained herein are
//  * proprietary to or under control of Rayark Inc. and its affiliates.
//  * The information herein may be covered by patents, patents in process,
//  * and are protected by trade secret or copyright law.
//  * You may not disseminate this information or reproduce this material
//  * unless otherwise prior agreed by Rayark Inc. in writing.
//  */

namespace JetGen
{
    using UnityEngine;
    using UnityEngine.UI;
    public class LayoutDrawerTestElement : MonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField] private Text _text;
#pragma warning restore IDE0044
        public void Render(string text)
        {
            _text.text = text;
        }
    }
}
