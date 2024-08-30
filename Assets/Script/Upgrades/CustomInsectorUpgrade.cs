using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(UpgradeSO))]
public class CustomInsectorUpgrade : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Get reference to the target object
        UpgradeSO UpgSO = (UpgradeSO)target;

        // Draw the enum field
        UpgSO.UpgradeType = (UpgradeSO.UpgradeTypeEnum)EditorGUILayout.EnumPopup("Upgrade Type", UpgSO.UpgradeType);
        UpgSO.WeaponType = (UpgradeSO.WeaponTypeEnum)EditorGUILayout.EnumPopup("Weapon Type", UpgSO.WeaponType);
        UpgSO.DamageType = (UpgradeSO.DamageTypeEnum)EditorGUILayout.EnumPopup("Damage Type", UpgSO.DamageType);

        /*// Draw the ScriptableObject reference field
        UpgSO.DamageType = (IntVariable)EditorGUILayout.ObjectField("Damage Type", UpgSO.DamageType, typeof(IntVariable), false);*/

        // Draw the always-visible fields
        UpgSO.UpgradeName = EditorGUILayout.TextField("Upgrade Name", UpgSO.UpgradeName);
        SerializedProperty stringArrayProperty = serializedObject.FindProperty("UpgradeDescription");
        EditorGUILayout.PropertyField(stringArrayProperty, new GUIContent("Upgrade Description"), true);

        UpgSO.MaxLevel = EditorGUILayout.IntField("Max Level", UpgSO.MaxLevel);
        UpgSO.ID = EditorGUILayout.IntField("ID", UpgSO.ID);


        // Conditional field display based on enum selection
        switch (UpgSO.UpgradeType)
        {
            case UpgradeSO.UpgradeTypeEnum.Weapon:
            case UpgradeSO.UpgradeTypeEnum.Abilities:
                // Draw the array of custom class instances
                SerializedProperty arrayProp = serializedObject.FindProperty("ElitePath");
                EditorGUILayout.PropertyField(arrayProp, new GUIContent("Elite Path"), true);
                break;
        }

        // Apply any changes made in the Inspector
        serializedObject.ApplyModifiedProperties();

        // Force update of the inspector
        EditorUtility.SetDirty(target);
        Repaint();
    }
}
#endif