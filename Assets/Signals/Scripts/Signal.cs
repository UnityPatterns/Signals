using UnityEngine;
using System.Collections;

[System.Serializable]
public class Signal
{
	public GameObject target;
	public string method;
	public string argType;

	public Signal()
	{

	}
	public Signal(System.Type argType)
	{
		this.argType = argType.FullName;
	}

	public void Invoke()
	{
		if (target != null && !string.IsNullOrEmpty(method))
			target.SendMessage(method, SendMessageOptions.RequireReceiver);
	}
	public void Invoke(object value)
	{
		if (argType != null)
		{
			if (target != null && !string.IsNullOrEmpty(method))
			{
				if (argType.Equals(value.GetType().FullName))
					target.SendMessage(method, value, SendMessageOptions.RequireReceiver);
				else
					Debug.LogError("Incorrect parameter type, expected [" + argType + "], got [" + value.GetType().FullName + "].");
			}
		}
		else
			Invoke();
	}
}

public class SignalAttribute : System.Attribute
{
	public string name;

	public SignalAttribute()
	{

	}
	public SignalAttribute(string name)
	{
		this.name = name;
	}
}