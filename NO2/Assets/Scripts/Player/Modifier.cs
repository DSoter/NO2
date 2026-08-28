using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Modifier
{
    public enum ModifierType
    {
        Numeric,
        Percentage
    }

    [SerializeField] private ModifierType type;
    [SerializeField] private float value;

    public ModifierType Type => type;
    public float Value => value;

    public Modifier(ModifierType type, float value)
    {
        this.type = type;
        this.value = value;
    }

    // Aplica este modificador de manera aislada sobre un valor
    public float ApplyModifier(float baseValue)
    {
        switch (type)
        {
            case ModifierType.Numeric:
                return baseValue + value;
            case ModifierType.Percentage:
                return baseValue * (1f + value / 100f);
            default:
                return baseValue;
        }
    }

    // Aplica una lista de modificadores sobre un valor base:
    // primero se suman todos los numéricos, después se multiplican todos los
    // porcentuales entre sí y se aplican como un único multiplicador.

    public static float ApplyModifiers(float baseValue, List<Modifier> modifiers)
    {
        if (modifiers == null || modifiers.Count == 0) return baseValue;

        float result = baseValue;
        float percentageMult = 1f;

        foreach (Modifier modifier in modifiers)
        {
            if (modifier.type == ModifierType.Numeric)
                result += modifier.value;
            else if (modifier.type == ModifierType.Percentage)
                percentageMult *= (1 + modifier.value / 100);
        }

        if (percentageMult != 0f)
            result *= percentageMult ;

        return result;
    }
}