using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomPropertyDrawer(typeof(Signal))]
public class SignalDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		GUI.Label(position, label.text + " (Signal)", EditorStyles.label);
		var indent = 30f;
		var prefixRect = new Rect(position.x + indent, position.y + position.height / 3, (position.width - indent) * 0.25f, position.height / 3);
		var valueRect = new Rect(prefixRect.x + prefixRect.width, prefixRect.y, (position.width - indent) * 0.75f, position.height / 3);

		var targetProperty = property.FindPropertyRelative("target");
		GUI.Label(prefixRect, "Target", EditorStyles.miniLabel);
		targetProperty.objectReferenceValue = EditorGUI.ObjectField(valueRect, targetProperty.objectReferenceValue, typeof(GameObject), true);

		prefixRect.y += prefixRect.height + 1;
		valueRect.y += valueRect.height + 1;

		var methodProperty = property.FindPropertyRelative("method");
		GUI.Label(prefixRect, "Method", EditorStyles.miniLabel);

		var argType = property.FindPropertyRelative("argType").stringValue;
		var names = new List<string>();
		var methodNames = new List<string>();
		names.Add("None");
		methodNames.Add(null);
		var index = 0;
		var obj = targetProperty.objectReferenceValue as GameObject;
		if (obj != null)
		{
			var components = obj.GetComponents<MonoBehaviour>();
			foreach (var component in components)
			{
				var type = component.GetType();
				var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				foreach (var method in methods)
				{
					var attributes = method.GetCustomAttributes(typeof(SignalAttribute), true);
					if (attributes.Length > 0)
					{
						var parameters = method.GetParameters();
						var addMethod = false;

						if (string.IsNullOrEmpty(argType))
						{
							if (parameters.Length == 0)
								addMethod = true;
						}
						else
						{
							if (parameters.Length == 1 && parameters[0].ParameterType.FullName.Equals(argType))
								addMethod = true;
						}

						if (addMethod)
						{
							var attribute = (SignalAttribute)attributes[0];
							if (string.IsNullOrEmpty(attribute.name))
							{
								var name = ObjectNames.NicifyVariableName(method.Name);
								if (name.Contains("Signal"))
									name = name.Replace("Signal", "");
								names.Add(type.Name + ": " + name);
							}
							else
								names.Add(type.Name + ": " + attribute.name);
							if (methodProperty.stringValue == method.Name)
								index = methodNames.Count;
							methodNames.Add(method.Name);
						}
					}
				}
			}
		
			if (names.Count > 1)
			{
				EditorGUI.BeginChangeCheck();
				index = EditorGUI.Popup(valueRect, index, names.ToArray());
				if (EditorGUI.EndChangeCheck())
					methodProperty.stringValue = methodNames[index];
			}
			else
				GUI.Label(valueRect, "None");
		}
		else
			GUI.Label(valueRect, "None");

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) * 3;
	}
}
