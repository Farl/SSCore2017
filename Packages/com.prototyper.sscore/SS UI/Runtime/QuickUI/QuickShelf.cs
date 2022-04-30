using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SS
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class QuickShelf : MonoBehaviour
	{
		public enum TransitionType
		{
			None,
			Fade,
			Translate,
			Scale,
			Anim,
		}
		public TransitionType transitionType;
		
		private CanvasGroup canvasGroup;
		private Animator animator;
		private UIBase page;
		
		private float defaultTransitionTime = 0.2f;
		
		public float transitionInTime = -1;
		public float transitionOutTime = -1;
		public string transitionInCurveID;
		public string transitionOutCurveID;
		
		private Vector3 origLocalPos;
		private Vector3 offsetPos;
		private Vector3 targetPos;
		private Vector3 sourcePos;
		
		private Vector3 origLocalScale;
		private Vector3 targetScale;
		private Vector3 sourceScale;
		
		private float origAlpha = 1;
		private float targetAlpha;
		private float sourceAlpha;
		
		private float currTime = 0;
		private float totalTime = 0;
		
		private AnimationCurve curve;
		
		private bool isInit = false;
		
		private bool isInTransition = false;
		
		private bool goOut = false;
		
		CanvasGroup GetOrAddCanvasGroup()
		{
			if (!canvasGroup)
				canvasGroup = GetComponent<CanvasGroup>();
			if (!canvasGroup)
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			return canvasGroup;
		}
		
		public void Init()
		{
			if (!isInit)
			{
				GetOrAddCanvasGroup();
				origLocalPos = transform.localPosition;
				origLocalScale = transform.localScale;
				
				switch (transitionType)
				{
				case TransitionType.Anim:
					InitAnim ();
					break;
				case TransitionType.Fade:
					InitFade();
					break;
				case TransitionType.None:
					break;
				case TransitionType.Scale:
					InitScale();
					break;
				case TransitionType.Translate:
					InitTranslate();
					break;
				}

				if (transitionType != TransitionType.Anim)
				{
					// Immediate out!
					float temp = transitionOutTime;
					transitionOutTime = 0;
					Out();
					transitionOutTime = temp;
					Calc();
					isInTransition = false;
				}
				
				if (transitionInTime < 0)
					transitionInTime = defaultTransitionTime;
				if (transitionOutTime < 0)
					transitionOutTime = defaultTransitionTime;
				
				isInit = true;
			}
		}
		
		void Start ()
		{
			Init ();	
		}
		
		void InitAnim()
		{
			animator = GetComponent<Animator>();
		}
		
		void InitFade()
		{
		}
		
		void InitScale()
		{
		}

        [ContextMenu("Init Translate")]
		void InitTranslate()
		{
			RectTransform[] group = GetComponentsInChildren<RectTransform>();
			if (group != null)
			{
				bool init = false;
				Bounds bound = new Bounds();
				
				Canvas canvas = GetComponentInParent<Canvas>();
				RectTransform canvasRT = canvas.GetComponent<RectTransform>();
				
				foreach (RectTransform rt in group)
				{
					if (rt.gameObject != gameObject)
					{
						Vector3[] corners = new Vector3[4];
						rt.GetWorldCorners(corners);

						foreach (Vector3 corner in corners)
						{
							Debug.Log(rt.name + " " + corner);

							// World to local
							// (Canvas pivot is always center)
							Vector3 vRel = canvasRT.InverseTransformPoint(corner);
							
							if (!init)
							{
								init = true;
								bound = new Bounds(vRel, Vector3.zero);
							}
							else
							{
                                bound.Encapsulate(vRel);
							}
						}
					}
				}
				
				Debug.Log (bound.max + " ~ " + bound.min);
				Debug.Log(canvasRT.rect.size);
				
				RectTransform currRT = GetComponent<RectTransform>();
				offsetPos = (Vector3)((currRT.anchorMax + currRT.anchorMin) / 2.0f);

				Vector2 canvasSize = canvasRT.rect.size;
				
				if (offsetPos.x < 0.5f)
				{
					offsetPos.x = bound.max.x - canvasSize.x / 2.0f;
				}
				else if (offsetPos.x > 0.5f)
				{
					offsetPos.x = - bound.min.x + canvasSize.x / 2.0f;
				}
				
				if (offsetPos.y < 0.5f)
				{
					offsetPos.y = bound.max.y - canvasSize.y / 2.0f;
				}
				else if (offsetPos.y > 0.5f)
				{
					offsetPos.y = - bound.min.y + canvasSize.y / 2.0f;
				}
				
				offsetPos -= origLocalPos - transform.localPosition;
			}
		}
		
		[ContextMenu("In")]
		public void In()
		{
			Lock();

			isInTransition = true;

			var uiSettings = UISettings.Instance;
			if (uiSettings != null)
			{
				curve = UISettings.Instance.GetCurve((!string.IsNullOrEmpty(transitionInCurveID)) ? transitionInCurveID : "in");
			}
			
			goOut = false;
			
			totalTime = transitionInTime;
			
			currTime = 0;
			
			gameObject.SetActive(true);
			
			switch (transitionType)
			{
			case TransitionType.Anim:
				if (animator)
				{
					animator.Play("In", -1, 0);

					// Fix for hiccup
					animator.Update(0);
				}
				break;
			case TransitionType.Fade:
				sourceAlpha = canvasGroup.alpha;
				targetAlpha = origAlpha;
				break;
			case TransitionType.None:
				break;
			case TransitionType.Scale:
				sourceScale = transform.localScale;
				targetScale = origLocalScale;
				break;
			case TransitionType.Translate:
				sourcePos = transform.localPosition;
				targetPos = origLocalPos;
				break;
			}
			enabled = true;
		}
		
		[ContextMenu("Out")]
		public void Out(UIBase _page = null)
		{
			page = _page;

			Lock();

			isInTransition = true;
			
			var uiSettings = UISettings.Instance;
			if (uiSettings)
			{
				curve = UISettings.Instance.GetCurve((!string.IsNullOrEmpty(transitionOutCurveID)) ? transitionOutCurveID : "out");
			}
			
			goOut = true;
			
			totalTime = transitionOutTime;
			
			currTime = 0;
			
			switch (transitionType)
			{
			case TransitionType.Anim:
				if (animator)
				{
					animator.Play("Out", -1, 0);
					animator.Update(0);
				}
				break;
			case TransitionType.Fade:
				sourceAlpha = canvasGroup.alpha;
				targetAlpha = 0;
				break;
			case TransitionType.None:
				break;
			case TransitionType.Scale:
				sourceScale = transform.localScale;
				targetScale = Vector3.zero;
				break;
			case TransitionType.Translate:
				sourcePos = transform.localPosition;
				targetPos = origLocalPos + offsetPos;
				break;
			}
			enabled = true;
		}
		
		void Calc()
		{
			float factor = (totalTime <= 0)? 1: (currTime >= totalTime)? 1: currTime / totalTime;
			float factorCurve = (curve != null)? curve.Evaluate(factor): factor;
			//float restTime = totalTime - currTime;
			
			switch (transitionType)
			{
			case TransitionType.Anim:
				if (animator)
				{

				}
				break;

			case TransitionType.Fade:
			{
				//float alphaDiff = targetAlpha - canvasGroup.alpha;
				//float maxDelta = (totalTime <= 0)? alphaDiff: Time.unscaledDeltaTime * alphaDiff / restTime;
				//canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Mathf.Abs(maxDelta));
				
				float currAlpha = sourceAlpha * (1 - factorCurve) + targetAlpha * (factorCurve);
				canvasGroup.alpha = (factor >= 1)? currAlpha: Mathf.Lerp(canvasGroup.alpha, currAlpha, 0.5f);
			}
				break;
			case TransitionType.None:
				break;
			case TransitionType.Scale:
			{
				//Vector3 scaleDiff = targetScale - transform.localScale;
				//float maxDelta = (totalTime <= 0)? scaleDiff.magnitude: Time.unscaledDeltaTime * scaleDiff.magnitude / restTime;
				//transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Mathf.Abs(maxDelta));
				
				Vector3 currScale = sourceScale * (1 - factorCurve) + targetScale * (factorCurve);
				transform.localScale = (factor >= 1)? currScale: Vector3.Lerp(transform.localScale, currScale, 0.5f);
			}
				break;
			case TransitionType.Translate:
			{
				//Vector3 posDiff = targetPos - transform.localPosition;
				//float maxDelta = (totalTime <= 0)? posDiff.magnitude: Time.unscaledDeltaTime * posDiff.magnitude / restTime;
				//transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, Mathf.Abs(maxDelta));
				
				Vector3 currPos = sourcePos * (1 - factorCurve) + targetPos * (factorCurve);
				transform.localPosition = (factor >= 1)? currPos: Vector3.Lerp(transform.localPosition, currPos, 0.5f);
			}
				break;
			}
		}
		
		void Update ()
		{
			UpdateShelf();
		}

		void Lock()
		{
			UILocker.LockRecursive(gameObject, name + ".QucikShelf", true);
		}

		void Unlock()
		{
			UILocker.LockRecursive(gameObject, name + ".QucikShelf", false);
		}

		void Finish()
		{
			switch (transitionType)
			{
			case TransitionType.Anim:
				break;

			default:
				Calc ();
				currTime = totalTime;

				break;
			}

			enabled = false;
			
			Unlock();

			if (goOut)
			{
				gameObject.SetActive(false);
				
				if (page)
				{
					page.ShelfClosed(this);
				}
			}

			isInTransition = false;
		}

		void UpdateShelf()
		{
			switch (transitionType)
			{
			case TransitionType.Anim:
				if (animator)
				{
					int hashName = Animator.StringToHash((goOut)? "Out": "In");
					AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
					if (info.shortNameHash != hashName || info.normalizedTime >= 1)
					{
						Finish();
					}
				}
				break;
				
			default:
				if (currTime >= totalTime)
				{
					Finish();
				}
				else
				{
					Calc ();
					currTime += Time.unscaledDeltaTime;
				}
				break;
			}
		}
		
		public bool IsInTransition()
		{
			return isInTransition;
		}
	}
}