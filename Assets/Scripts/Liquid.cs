using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Liquid : ScriptableObject
{ 
    public float amount = 0;
    public Color color = new Color();
    public LiquidType type = LiquidType.Water;

    /// <summary>
    /// Copy ctor for Liquid
    /// </summary>
    /// <param name="other"></param>
    public Liquid (Liquid other)
    {
        amount = other.amount;
        color = other.color;
    }

    public Liquid(float inputAmount, Color inputColor, LiquidType inputType)
    {
        amount = inputAmount;
        color = inputColor;
        type = inputType;
    }


    /// <summary>
    /// Run after creating the instance
    /// </summary>
    /// <param name="inputAmount"></param>
    /// <param name="inputColor"></param>
    /// <param name="inputType"></param>
    public void init(float inputAmount, Color inputColor, LiquidType inputType)
    {
        amount = inputAmount;
        color = inputColor;
        type = inputType;
    }

    /// <summary>
    /// Copy ctor for Liquid
    /// </summary>
    /// <param name="other"></param>
    public void init (Liquid other)
    {
        amount = other.amount;
        color = other.color;
    }
}

public enum LiquidType {Water, Sodium };