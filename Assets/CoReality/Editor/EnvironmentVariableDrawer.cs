using UnityEngine;
using UnityEditor;

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(EnvironmentVariable))]
public class EnvironmentVariableDrawer : PropertyDrawer
{
    string tempValue;
    string currentValue;

    float rowHeight = 20f;
    float spacing = 2f;
    int rowCount = 4;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (rowHeight * rowCount) + ((rowCount - 1) * spacing);
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        SerializedProperty varName = property.FindPropertyRelative("VariableName");
        EnvironmentVariable envVar = fieldInfo.GetValue(property.serializedObject.targetObject) as EnvironmentVariable;
        currentValue = envVar.Get();
        if (currentValue != null) {
            tempValue = currentValue;
        }
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(rect, label, property);

        // Draw label
        label.text = "Environment: " + label.text; 
        EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.indentLevel++;

        rect.y += rowHeight + spacing;
        rect.height = rowHeight;

        // Draw variable name field
 
        if (envVar.lockedName) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(rect, "Variable Name", envVar.name);
            EditorGUI.EndDisabledGroup();
        } else {
            envVar.name = EditorGUI.TextField(rect, "Variable Name", envVar.name);
        }

        rect.y += rowHeight + spacing;

        // Draw value readout field, disabled if there is already a value
        EditorGUI.BeginDisabledGroup(currentValue != null);
        tempValue = EditorGUI.TextField(rect, "Value", tempValue);
        EditorGUI.EndDisabledGroup();

        rect.y += rowHeight + spacing;


        // Draw buttons
        if (currentValue == null) {
            if (GUI.Button(rect, "Set Variable")) {
                Debug.Log("Setting Env Var");
                envVar.Set(tempValue);
            }
        } else {
            if (GUI.Button(rect, "Clear Variable")) {
                envVar.Set(null);
                tempValue = null;
            }
        }

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}
