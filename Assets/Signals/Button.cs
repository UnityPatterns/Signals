using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{
	public Signal onPress;

	void OnTriggerEnter(Collider other)
	{
		onPress.Invoke();
	}
}
