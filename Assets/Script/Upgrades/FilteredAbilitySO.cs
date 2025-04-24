using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
[CustomEditor(typeof(UpgradeVariables))]
public class FilteredAbilitySO : Editor
{
    private SerializedProperty profileProp;
    private SerializedProperty upgradeTableProp;

    private void OnEnable()
    {
        profileProp = serializedObject.FindProperty("AbilityProfile");
        upgradeTableProp = serializedObject.FindProperty("UpgradeTable");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(profileProp);

        var upgradeSO = (UpgradeVariables)target;
        StatVariables[] allowedStats = GetAllowedStats(upgradeSO.AbilityProfile);

        upgradeTableProp.arraySize = EditorGUILayout.IntField("Size", upgradeTableProp.arraySize);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Upgrade Table", EditorStyles.boldLabel);
        

        if (upgradeTableProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox("Upgrade table is empty. Set the array size or initialize in code.", MessageType.Info);
        }

        for (int i = 0; i < upgradeTableProp.arraySize; i++)
        {
            SerializedProperty entry = upgradeTableProp.GetArrayElementAtIndex(i);
            SerializedProperty statProp = entry.FindPropertyRelative("Stat");
            SerializedProperty valueProp = entry.FindPropertyRelative("Value");

            EditorGUILayout.BeginVertical("box");

            // Filtered enum popup
            StatVariables currentStat = (StatVariables)statProp.enumValueIndex;
            int currentIndex = System.Array.IndexOf(allowedStats, currentStat);
            if (currentIndex == -1) currentIndex = 0;

            string[] options = System.Array.ConvertAll(allowedStats, s => s.ToString());
            int selectedIndex = EditorGUILayout.Popup("Stat", currentIndex, options);
            statProp.enumValueIndex = (int)allowedStats[selectedIndex];

            EditorGUILayout.PropertyField(valueProp);

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(upgradeSO);
    }
    private StatVariables[] GetAllowedStats(AbilityProfile profile)
    {
        List<StatVariables> FilteredTable = new List<StatVariables>
        {
            StatVariables.Damage,
            StatVariables.Cooldown,
            StatVariables.Knockback,
            StatVariables.DamageScaling,
            StatVariables.CritRate,
            StatVariables.CritDamage,
        };
        switch (profile)
        {
            case AbilityProfile.EngimaticSaw:
                FilteredTable.AddRange(new List<StatVariables>
                {
                    StatVariables.Scale,
                    StatVariables.RazorbladeCount,
                    StatVariables.DamageInterval,
                    StatVariables.Speed,
                    StatVariables.Duration,
                });
                break;
            case AbilityProfile.HomingMissiles:
                FilteredTable.AddRange(new List<StatVariables>
                {
                    StatVariables.MissileNumbers,
                    StatVariables.InternalExplode,
                    StatVariables.Scale,
                });
                break;
            case AbilityProfile.MagneticField:
                FilteredTable.AddRange(new List<StatVariables>
                {
                    StatVariables.NumbOfOrbs,
                    StatVariables.NumbOfMini,
                    StatVariables.Duration,
                    StatVariables.Piercing,
                    StatVariables.OrbMoveSpeed,
                    StatVariables.OrbRecover,
                    StatVariables.Scale,
                });
                break;
            case AbilityProfile.FieryEruption:
                FilteredTable.AddRange(new List<StatVariables>
                {
                    StatVariables.AdditionalEruptions,
                    StatVariables.GroundDuration,
                    StatVariables.DecreaseScale,
                    StatVariables.IncreaseScale,
                });
                break;
            case AbilityProfile.SplitterShotgun:
                FilteredTable.AddRange(new List<StatVariables>
                {
                    StatVariables.BulletLifetime,
                    StatVariables.BulletNumb,
                    StatVariables.Bounce,
                    StatVariables.Radius,
                });
                break;
            case AbilityProfile.LancerBeam:
                FilteredTable.AddRange(new List<StatVariables>
                {
                    StatVariables.BeamLifetime,
                    StatVariables.DamageInterval,
                    StatVariables.Scale,
                    StatVariables.Radius,
                    StatVariables.BeamCount,
                    StatVariables.Piercing,
                    StatVariables.FractionBeam,
                });
                break;
        }
        StatVariables[] ConvertedTable = FilteredTable.ToArray();
        return ConvertedTable;
    }
}
#endif