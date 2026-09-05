using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Modifier
{
    public enum ModifierType
    {
        Numeric,
        Percentage,
        Multiplier
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
            case ModifierType.Multiplier:
                return baseValue * (1f + value / 100f);
            default:
                return baseValue;
        }
    }

    // Aplica una lista de modificadores sobre un valor base, en este orden:
    // 1) Se suman todos los numéricos.
    // 2) Se suman entre sí todos los porcentuales y se aplican como UN único
    //    multiplicador (p.ej. dos badges de +10% de defensa dan +20%, no +21%).
    // 3) Cada Multiplier se aplica de manera INDEPENDIENTE y compuesta, uno
    //    detrás de otro (p.ej. dos badges de +20% de velocidad dan
    //    1.2 * 1.2 = 1.44, no 1.4).
    public static float ApplyModifiers(float baseValue, List<Modifier> modifiers)
    {
        if (modifiers == null || modifiers.Count == 0) return baseValue;

        float result = baseValue;
        float percentageSum = 0f;

        foreach (Modifier modifier in modifiers)
        {
            if (modifier.type == ModifierType.Numeric)
                result += modifier.value;
            else if (modifier.type == ModifierType.Percentage)
                percentageSum += modifier.value;
        }

        if (percentageSum != 0f)
            result *= (1f + percentageSum / 100f);

        foreach (Modifier modifier in modifiers)
        {
            if (modifier.type == ModifierType.Multiplier)
                result = modifier.ApplyModifier(result);
        }

        return result;
    }
}