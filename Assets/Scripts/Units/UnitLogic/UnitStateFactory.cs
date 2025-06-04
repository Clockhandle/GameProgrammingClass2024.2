using System;
using System.Collections.Generic;
using UnityEngine;

public static class UnitStateFactory
{
    private static Dictionary<UnitClass, Func<UnitStates>> stateCreators = new Dictionary<UnitClass, Func<UnitStates>>()
    {
        { UnitClass.Infantry, () => new UnitInfantryState() },
        { UnitClass.Archer, () => new UnitArcherState() },
        { UnitClass.Medic, () => new UnitMedicState() },
        { UnitClass.Workers, () => new UnitWorkerState() },
        { UnitClass.Scout, () => new UnitScoutState() }
    };

    /// <summary>
    /// Returns an appropriate state for the given unit data.
    /// If the unit has multiple classes, you can decide whether to prioritize one or combine behavior.
    /// </summary>
    public static UnitStates CreateState(UnitDataSO unitData)
    {
        // If the unit can have multiple classes, you might decide on a priority order.
        if ((unitData.unitClass & UnitClass.Infantry) != 0)
            return stateCreators[UnitClass.Infantry]();
        if ((unitData.unitClass & UnitClass.Archer) != 0)
            return stateCreators[UnitClass.Archer]();
        if ((unitData.unitClass & UnitClass.Medic) != 0)
            return stateCreators[UnitClass.Medic]();
        if ((unitData.unitClass & UnitClass.Workers) != 0)
            return stateCreators[UnitClass.Workers]();
        if ((unitData.unitClass & UnitClass.Scout) != 0)
            return stateCreators[UnitClass.Scout]();

        // Fallback default state.
        return new UnitAttackState();
    }
}
