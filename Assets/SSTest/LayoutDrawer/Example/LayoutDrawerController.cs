using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JetGen
{
    [ExecuteInEditMode]
    public class LayoutDrawerController : MonoBehaviour
    {
        [SerializeField] private bool _runInEditor;

        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private ScrollRect _scrollrect;

        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
        [SerializeField] private RectTransform _banner;
        [SerializeField] private RectTransform _footer;

        [SerializeField] private CustomGridLayoutGroup _gridLayoutGroup;
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
            if (!Application.isPlaying && !_runInEditor)
                return;

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
            if (!Application.isPlaying && !_runInEditor)
                return;

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
                        if (!Application.isPlaying)
                        {
                            element.gameObject.hideFlags = HideFlags.DontSave;
                            element.gameObject.name = $"(DontSave){element.gameObject.name}";
                        }
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
                    if (!Application.isPlaying)
                    {
                        element.gameObject.hideFlags = HideFlags.DontSave;
                        element.gameObject.name = $"(DontSave){element.gameObject.name}";
                    }
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

        void DestroyLayoutDrawer()
        {

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
            var rectTrans = _prefab.GetComponent<RectTransform>();
            Debug.Log($"{_prefab.name} {rectTrans.rect}", rectTrans);

            // GridLayoutGroup builder
            var gridBuilder = (new GridLayoutGroupBuilder(_gridLayoutGroup.transform as RectTransform, _gridLayoutGroup, false) as ILayoutBuilder);

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
                    gridBuilder.Add(element, idx);
                }
                else
                {
                    var element = PooledLayoutElement.Create(
                        _prefab2,
                        idx,
                        idx.ToString()
                    );
                    gridBuilder.Add(element, idx);
                }
            }

            gridBuilder.Add(new RowLayoutElement(500, 100));
            gridBuilder.Add(new RowLayoutElement(500, 100));

            for (; idx < _testElementCount; idx++)
            {
                if (idx % 2 == 0)
                {
                    var element = PooledLayoutElement.Create(
                        _prefab,
                        idx,
                        idx.ToString()
                    );
                    gridBuilder.Add(element, idx);
                }
                else
                {
                    var element = PooledLayoutElement.Create(
                        _prefab2,
                        idx,
                        idx.ToString()
                    );
                    gridBuilder.Add(element, idx);
                }
            }

            var gridLayout = gridBuilder.GetLayout();
            
            // VerticalLayoutGroup builder
            var mainBuilder = (new HoVLayoutGroupBuilder(_verticalLayoutGroup.GetComponent<RectTransform>(), _verticalLayoutGroup) as ILayoutBuilder)
            .Add(new StaticLayoutElement(_banner))
            .Add(gridLayout)
            .Add(new RowLayoutElement(400, 100))
            .Add(new StaticLayoutElement(_footer))
            ;

            return mainBuilder.GetLayout();
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <returns></returns>
        private IHoVLayoutV1 _GetLagecyLayout()
        {
            IHoVLayoutV1 trophyLayout = new VerticalLayout(_content);
            for (var i = 0; i < 20; i++)
            {
                var id = i;
                var item = PooledLayoutElement.Create(
                    _prefab,
                    id,
                    id.ToString()
                );
                trophyLayout.Add(item, id);
            }
            trophyLayout.Spacing = 20f;
            trophyLayout.SetPadding(10f);
            trophyLayout.DoLayout();

            return trophyLayout;
        }
    }
}
