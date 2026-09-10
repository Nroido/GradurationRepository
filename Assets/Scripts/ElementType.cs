using UnityEngine;

public enum ElementType
{
    Fire,   
    Wind,   
    Earth,  
    Water,  
    Light, 
    Dark    
}

public static class ElementAffinity
{
    public static float GetMultiplier(ElementType attacker, ElementType defender)
    {
        if (IsAdvantage(attacker, defender)) return 1.2f;
        if (IsAdvantage(defender, attacker)) return 0.8f;

        return 1.0f;
    }

    private static bool IsAdvantage(ElementType a, ElementType b)
    {
        // ‰Î¨•—¨“y¨…¨‰Î
        if (a == ElementType.Fire && b == ElementType.Wind) return true;
        if (a == ElementType.Wind && b == ElementType.Earth) return true;
        if (a == ElementType.Earth && b == ElementType.Water) return true;
        if (a == ElementType.Water && b == ElementType.Fire) return true;

        // ŒõÌˆÅ‚Í‘ŠŒİã“_
        if (a == ElementType.Light && b == ElementType.Dark) return true;
        if (a == ElementType.Dark && b == ElementType.Light) return true;
        
        return false;
    }
}