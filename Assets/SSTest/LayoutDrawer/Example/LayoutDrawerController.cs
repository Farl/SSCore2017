using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JetGen
{
    public class LayoutDrawerController : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private ScrollRect _scrollrect;

        [Header("Layout Group 1")]
        [SerializeField] private LayoutGroup _layoutGroup1;
        [SerializeField] private RectTransform _banner;
        [SerializeField] private RectTransform _footer;

        [Header("Layout Group 2")]
        [SerializeField] private LayoutGroup _layoutGroup2;
        [SerializeField] private LayoutDrawerTestElement _prefab;
        [SerializeField] private LayoutDrawerTestElement2 _prefab2;

        [Header("Test")]
        [SerializeField] private Button _button;
        [SerializeField] private int _testElementCount = 80;
        [SerializeField] private int _testElementId = 40;

        private ILayoutDrawer _layoutDrawer;

        private UnityEngine.Events.UnityAction _buttonAction;
        private UnityEngine.Events.UnityAction<Vector2> scrollAction;
        private IHoVLayout _currLayout;
        private Coroutine coroutine;

        private void OnDisable()
        {
            if (_layoutDrawer != null)
            {
                if (_currLayout != null)
                {
                    var layoutBuilder = (new LayoutBuilder(_content)) as ILayoutBuilder;
                    _currLayout = layoutBuilder.GetLayout();
                }
                if (_currLayout != null)
                {
                    _layoutDrawer.UpdateLayout(_currLayout);
                }
                _layoutDrawer.RedrawAlongAxis(_viewport, _content);
                _layoutDrawer = null;
            }

            if (scrollAction != null)
                _scrollrect?.onValueChanged.RemoveListener(scrollAction);
        }

        private void OnEnable()
        {
            CreateLayoutDrawer();
        }

        void CreateLayoutDrawer()
        {
            if (_layoutDrawer != null)
                return;

            _layoutDrawer = new LayoutDrawer();

            // Setup pool handler (LayoutDrawerTestElement)
            _layoutDrawer.SetPoolHandler<LayoutDrawerTestElement, string>(
                _prefab,
                spawner: (parentTransform) =>
                {
                    if (_prefab != null)
                    {
                        var element = Instantiate(_prefab, parentTransform);
                        element.gameObject.SetActive(true);
                        return element;
                    }
                    return null;
                },
                resetter: (view) =>
                {
                },
                updater: (view, text) =>
                {
                    view.Render(text);
                }
            );
            _layoutDrawer.SetPoolHandler<LayoutDrawerTestElement2, string>(
                _prefab2,
                spawner: (parentTransform) =>
                {
                    var element = Instantiate(_prefab2, parentTransform);
                    element.gameObject.SetActive(true);
                    return element;
                },
                resetter: (view) =>
                {
                },
                updater: (view, text) =>
                {
                }
            );

            // Prepare scroll value changed callback
            if (scrollAction == null)
            {
                scrollAction = (pos) =>
                {
                    _layoutDrawer?.RedrawAlongAxis(_viewport, _content);
                };
            }

            _scrollrect?.onValueChanged.AddListener(scrollAction);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            coroutine = StartCoroutine(UpdateLayoutCoroutine());
        }

        void Start()
        {
            _buttonAction = () =>
            {
                var element = _currLayout?.GetElementById(_testElementId);
                if (_scrollrect && element != null)
                {
                    _scrollrect.verticalNormalizedPosition =
                        element.GetVerticallyCenteredNormalizedPosition(_viewport, _content);
                    _scrollrect.horizontalNormalizedPosition =
                        element.GetHorizontallyCenteredNormalizedPosition(_viewport, _content);
                }
            };
            _button?.onClick.AddListener(_buttonAction);
        }

        [ContextMenu("Update Layout")]
        void UpdateLayout()
        {
            if (_currLayout != null)
            {
                _currLayout = null;
            }

            // Recalculate layout
            _currLayout = _GetIHoVLayout();

            if (_layoutDrawer != null && _currLayout != null)
            {
                _layoutDrawer.UpdateLayout(_currLayout);
                _layoutDrawer.RedrawAlongAxis(_viewport, _content);
            }
        }

        IEnumerator UpdateLayoutCoroutine()
        {
            yield return new WaitForEndOfFrame();

            UpdateLayout();
        }

        private IHoVLayout _GetIHoVLayout()
        {
            // Sub layout builder
            var subBuilder = GridLayoutGroupBuilder.CreateLayoutBuilder(_layoutGroup2, false);

            // Add Pooled Layout Element in Grid
            var idx = 0;
            for (; idx <  _testElementCount / 2; idx++)
            {
                if (idx % 2 == 0)
                {
                    var element = PooledLayoutElement.Create(
                        _prefab,
                        idx,
                        idx.ToString()
                    );
                    subBuilder.Add(element, idx);
                }
                else
                {
                    var element = PooledLayoutElement.Create(
                        _prefab2,
                        idx,
                        idx.ToString()
                    );
                    subBuilder.Add(element, idx);
                }
            }

            for (; idx < _testElementCount; idx++)
            {
                if (idx % 2 == 0)
                {
                    var element = PooledLayoutElement.Create(
                        _prefab,
                        idx,
                        idx.ToString()
                    );
                    subBuilder.Add(element, idx);
                }
                else
                {
                    var element = PooledLayoutElement.Create(
                        _prefab2,
                        idx,
                        idx.ToString()
                    );
                    subBuilder.Add(element, idx);
                }
            }
            
            // Main layout builder
            var mainBuilder = GridLayoutGroupBuilder.CreateLayoutBuilder(_layoutGroup1)
            .Add(new StaticLayoutElement(_banner))
            .Add(subBuilder.GetLayout())
            .Add(new RowLayoutElement(400, 100))
            .Add(new StaticLayoutElement(_footer))
            ;

            return mainBuilder.GetLayout();
        }
    }
}
