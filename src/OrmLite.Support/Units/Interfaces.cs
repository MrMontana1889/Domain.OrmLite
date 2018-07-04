// Interfaces.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

namespace OrmLite.Support.Units
{
    /// <summary>
    /// Interface for strategy objects that convert between units.
    /// </summary>
    public interface IUnitConverter
    {
        double FromBaseUnit(double adouble);
        double ToBaseUnit(double adouble);
    }
}
