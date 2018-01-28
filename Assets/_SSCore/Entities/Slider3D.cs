using UnityEngine;
using System.Collections;
using SS;

public class Slider3D : MonoBehaviour {

	public int quantization = 0;
	public int quantizationValue = 0;

	Vector2 prevPos;
	bool draging = false;
	Transform target;

	bool CheckTarget()
	{
		if (target == null && transform.childCount > 0)
			target = transform.GetChild(0);
		return target != null;
	}
	
	// Use this for initialization
	void Start ()
	{
		CheckTarget();
	}

	void Awake()
	{
		EventManager.AddEventListener("UICamera-Press", OnUIPress, gameObject);
		EventManager.AddEventListener("UICamera-Down", OnUIDown, gameObject);
	}

	void OnDestroy()
	{
		EventManager.RemoveEventListener("UICamera-Press", OnUIPress, gameObject);
		EventManager.RemoveEventListener("UICamera-Down", OnUIDown, gameObject);
	}

    protected virtual void OnUIPress(EventMessage em, ref object paramRef)
    {
        if (em.paramBool)
		{
			if (em.paramExtra[0].GetType() == typeof(Vector2))
			{
				Vector2 paramVec2 = (Vector2)em.paramExtra[0];
				prevPos = paramVec2;
			}
		}
	}

    protected virtual void OnUIDown(EventMessage em, ref object paramRef)
    {
        //print (eventID);
        if (em.paramExtra.Length > 0)
		{
			if (em.paramExtra[0].GetType() == typeof(Vector2))
			{
				Vector2 paramVec2 = (Vector2)em.paramExtra[0];
				
				//print(paramVec2);

				Vector2 diff = paramVec2 - prevPos;
				if (target != null)
				{
					float rotateValue = diff.y * 400.0f / Screen.height;
					target.Rotate(new Vector3(rotateValue, 0, 0), Space.Self);
				}
				prevPos = paramVec2;

				draging = true;
			}
		}
	}

	public void SetQuantizationValue(int q)
	{
		if (quantization > 0)
		{
			quantizationValue = q % quantization;
			float angle = 360.0f * quantizationValue / (quantization);
			if (CheckTarget())
			{
				Vector3 newEuler = Vector3.zero;
				newEuler.x = angle;
				target.localEulerAngles = newEuler;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (quantization > 0)
		{
			if (CheckTarget())
			{
				if (!draging)
				{
					int qValue = Mathf.RoundToInt((target.localEulerAngles.x / 360.0f) * quantization);
					float targetX = 360.0f * qValue / (float)quantization;
					//print (targetX);
					Vector3 newEuler = target.localEulerAngles;
					newEuler.x = targetX;

					Quaternion newQuat = Quaternion.Euler(newEuler);
					target.localRotation = Quaternion.Slerp(target.localRotation, newQuat, 0.5f);


					float newValue = (Vector3.Dot(newQuat * Vector3.forward, Quaternion.LookRotation(Vector3.up) * Vector3.forward) > 0)?
						360 - Quaternion.Angle(newQuat, Quaternion.identity):
						Quaternion.Angle(newQuat, Quaternion.identity);
					quantizationValue = Mathf.RoundToInt((newValue / 360.0f) * quantization) % quantization;
				}
			}
		}
		draging = false;
	}
}
