using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerDataType
{
    public EnumDataType enumType;
    public float value;

    public PlayerDataType(EnumDataType enumType, float value)
    {
        this.enumType = enumType;
        this.value = value;
    }
}
