// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*============================================================
**
** Purpose: Default IComparer implementation.
**
===========================================================*/

using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Collections
{
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public sealed class Comparer : IComparer, ISerializable
    {
        private readonly CompareInfo _compareInfo;

        public static readonly Comparer Default = new Comparer(CultureInfo.CurrentCulture);
        public static readonly Comparer DefaultInvariant = new Comparer(CultureInfo.InvariantCulture);

        public Comparer(CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(culture);

            _compareInfo = culture.CompareInfo;
        }

        private Comparer(SerializationInfo info, StreamingContext context)
        {
            ArgumentNullException.ThrowIfNull(info);

            _compareInfo = (CompareInfo)info.GetValue("CompareInfo", typeof(CompareInfo))!;
        }

        [Obsolete(Obsoletions.LegacyFormatterImplMessage, DiagnosticId = Obsoletions.LegacyFormatterImplDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ArgumentNullException.ThrowIfNull(info);

            info.AddValue("CompareInfo", _compareInfo);
        }

        // Compares two Objects by calling CompareTo.
        // If a == b, 0 is returned.
        // If a implements IComparable, a.CompareTo(b) is returned.
        // If a doesn't implement IComparable and b does, -(b.CompareTo(a)) is returned.
        // Otherwise an exception is thrown.
        //
        public int Compare(object? a, object? b)
        {
            if (a == b) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            if (a is string sa && b is string sb)
                return _compareInfo.Compare(sa, sb);

            if (a is IComparable ia)
                return ia.CompareTo(b);

            if (b is IComparable ib)
                return -ib.CompareTo(a);

            throw new ArgumentException(SR.Argument_ImplementIComparable);
        }
    }
}
