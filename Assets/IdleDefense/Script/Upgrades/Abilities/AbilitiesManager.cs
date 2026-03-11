using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    // Every new ability just add it in here no need to be dynamic
    public EnigmaticSawController EnigmaticSawController;
    public LancerBeamController  LancerBeamController;
    public HomingMissilesController HomingMissilesController;
    public FieryEruptionController FieryEruptionController;
    public MagneticFieldController MagneticFieldController;
    public SplitterController SplitterController;

    public void ActivateAbility(int abilityid)
    {
        AbilitiesController abilityController = getAbilityController(abilityid);
        abilityController.enabled = true;
    }

    public void UpgradeAbility(int abilityid, int level)
    {
        AbilitiesController abilityController = getAbilityController(abilityid); 
        abilityController.CheckUpgrade(level);
    }

    public void UnlockElite(int abilityid, int eliteID)
    {
        AbilitiesController abilityController = getAbilityController(abilityid);
        abilityController.EliteUnlock(eliteID);
    }
    
    #region ID assign NOTE
    /*
        ID = 1 => HomingMissiles
        ID = 2 => FieryEruption
        ID = 3 => MagneticField
        ID = 4 => SplitterShotgun
        ID = 5 => EnigmaticSaw
        ID = 6 => LancerBeam
    */
    #endregion
    private AbilitiesController getAbilityController(int id)
    {
        AbilitiesController abilityController;
        switch (id)
        {
            case 1:
                abilityController = HomingMissilesController;
                break;
            case 2:
                abilityController = FieryEruptionController;
                break;
            case 3:
                abilityController = MagneticFieldController;
                break;
            case 4:
                abilityController = SplitterController;
                break;
            case 5:
                abilityController = EnigmaticSawController;
                break;
            case 6:
                abilityController = LancerBeamController;
                break;
            default:
                abilityController = HomingMissilesController;
                break;
        }
        return abilityController;
    }
}
