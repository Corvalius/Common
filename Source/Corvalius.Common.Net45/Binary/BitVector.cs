﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Corvalius.Binary
{

    /// <summary>
    /// Differently from the numeric representation a BitVector operation is left-aligned.
    /// </summary>
    [DebuggerDisplay("{ToDebugString()}")]
    public class BitVector : IComparable<BitVector>, IEquatable<BitVector>
    {
        public const int BitsPerByte = 8;
        public const int BitsPerWord = sizeof(ulong) * BitsPerByte;
        public const int BytesPerWord = sizeof(ulong) / sizeof(byte);        
        public const uint Log2BitsPerWord = 6; // Math.Log( BitsPerWord, 2 )
        
	    public const uint WordMask = BitsPerWord - 1;
        public const ulong Ones = 0xFFFFFFFFFFFFFFFFL;

        public const uint FirstBitPosition = BitsPerWord - 1;
        public const ulong FirstBitMask = 1UL << (int)FirstBitPosition;

        public const ulong LastBitMask = 1UL;
        public const uint LastBitPosition = 1;
        
        public readonly ulong[] Bits;        

        public string ToDebugString()
        {
            unchecked
            {
                var builder = new StringBuilder();
                if (this.Count <= BitsPerWord)
                {
                    for (int i = 0; i < this.Count; i++)
                        builder.Append(this[i] ? 1 : 0);
                }
                else
                {
                    int words = NumberOfWordsForBits(this.Count);
                    for (int i = 0; i < words; i++)
                    {
                        ulong v = this.GetWord(i);

                        builder.Append(v.ToString("X16"));
                        builder.Append(" ");
                    }
                }

                return builder.ToString();
            }            
        }

        private static ulong Reverse(ulong v)
        {
            int s = BitsPerWord; // bit size; must be power of 2 

            unchecked
            {
                ulong mask = (ulong)~0;
                while ((s >>= 1) > 0)
                {
                    mask ^= (mask << s);
                    v = ((v >> s) & mask) | ((v << s) & ~mask);
                }
            }

            return v;
        }

        public BitVector(int size)
        {
            this.Count = size;
            this.Bits = new ulong[size % BitVector.BitsPerWord == 0 ? size / BitVector.BitsPerWord : size / BitVector.BitsPerWord + 1];            
        }

        public BitVector(int size, params ulong[] values)
        {
            if (size / BitVector.BitsPerWord > values.Length)
                throw new ArgumentException("The values passed as parameters does not have enough bits to fill the vector size.", "values");

            this.Count = size;    
            this.Bits = values;
        }

        public int Count
        {
            get;
            private set;
        }

        public bool this[int idx]
        {
            get { return Get(idx); }
            set { Set(idx, value); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int idx)
        {
            Contract.Requires(idx >= 0 && idx < this.Count);

            uint word = WordForBit(idx);
            ulong mask = BitInWord(idx);

            Bits[word] |= mask;                
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int idx, bool value)
        {
            Contract.Requires(idx >= 0 && idx < this.Count);

            uint word = WordForBit(idx);
            ulong mask = BitInWord(idx);

            bool currentValue = (Bits[word] & mask) != 0;
            if (currentValue != value)
                Bits[word] ^= mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Get(int idx)
        {
            Contract.Requires(idx >= 0 && idx < this.Count);

            uint word = WordForBit(idx);
            ulong mask = BitInWord(idx);
            return (Bits[word] & mask) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetByte(int idx)
        {
            int positionInWord = idx % BitVector.BytesPerWord;
            
            ulong word = GetWord(idx / BitVector.BytesPerWord);
            word <<= BitVector.BitsPerByte * positionInWord;
            word >>= BitVector.BitsPerByte * (BitVector.BytesPerWord - 1);
            
            return (int)word;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong GetWord(int wordIdx)
        {            
            return Bits[wordIdx];
        }

        public void Fill(bool value)
        {
            // TODO: Avoid touching bits outside of the valid ones. (keep them as Zeroes)
            unsafe
            {
                byte x = value ? (byte)0xFF : (byte)0x00;
                fixed (ulong* array = this.Bits)
                    Memory.SetInline((byte*)array, x, this.Bits.Length * sizeof(ulong));
            }
        }

        public void Fill(bool value, int from, int to)
        {
            Contract.Requires(from >= 0 && from < this.Count);
            Contract.Requires(to >= 0 && to < this.Count);
            Contract.Requires(from <= to);

            // TODO: Avoid touching bits outside of the valid ones. (keep them as Zeroes)
            unchecked
            {
                int bFrom = from / BitVector.BitsPerWord;
                int bTo = to / BitVector.BitsPerWord;

                if (bFrom == bTo)
                {
                    ulong fill = (1UL << (to - from) - 1 << from);
                    if (value)
                        Bits[bFrom] |= fill;
                    else
                        Bits[bFrom] &= ~fill;
                }
                else
                {
                    byte x = value ? (byte)0xFF : (byte)0x00;
                    unsafe
                    {
                        fixed (ulong* array = this.Bits)
                            Memory.SetInline((byte*)(array + bFrom + 1), x, bTo);
                    }

                    if (from % BitVector.BitsPerWord != 0)
                    {
                        if (value)
                            Bits[bFrom] |= (ulong)(-1L) << from % BitVector.BitsPerWord;
                        else
                            Bits[bFrom] &= (1UL << from % BitVector.BitsPerWord) - 1;
                    }

                    if (to % BitVector.BitsPerWord != 0)
                    {
                        if (value)
                            Bits[bTo] |= 1UL << to % BitVector.BitsPerWord - 1;
                        else
                            Bits[bTo] &= (ulong)(-1L) << to % BitVector.BitsPerWord;
                    }
                }
            }
        }

        public void Flip()
        {
            // TODO: Avoid touching bits outside of the valid ones. (keep them as Zeroes)

            for (int i = 0; i < Bits.Length; i++)
                Bits[i] ^= BitVector.Ones;
        }

        public void Flip(int idx)
        {
            Contract.Requires(idx >= 0 && idx < this.Count);

            // TODO: Avoid touching bits outside of the valid ones. (keep them as Zeroes)

            unchecked
            {
                uint wPos = WordForBit(idx);
                Bits[wPos] ^= BitInWord((int)idx);
            }
        }

        public void Flip(int from, int to)
        {
            Contract.Requires(from >= 0 && from < this.Count);
            Contract.Requires(to >= 0 && to < this.Count);
            Contract.Requires(from <= to);

            // TODO: Avoid touching bits outside of the valid ones. (keep them as Zeroes)

            unchecked
            {
                int bTo = to / BitVector.BitsPerWord;
                int bFrom = from / BitVector.BitsPerWord;
                if (bTo == bFrom)
                {
                    if (from == to)
                    {
                        Bits[bFrom] ^= BitInWord((int)from);
                    }
                    else
                    {
                        ulong mask = Ones << BitVector.BitsPerWord - (to - from);
                        Bits[bFrom] ^= mask >> from;
                    }
                        
                }
                else
                {
                    int start = (from + BitVector.BitsPerWord - 1) / BitVector.BitsPerWord;

                    ulong mask = BitVector.Ones;
                    for (int i = bTo; i-- != start; )
                        Bits[i] ^= mask;

                    if (from % BitVector.BitsPerWord != 0)
                        Bits[bFrom] ^= (1UL << (BitVector.BitsPerWord - from) % BitVector.BitsPerWord) - 1;
                    if (to % BitVector.BitsPerWord != 0)
                        Bits[bTo] ^= Ones << BitVector.BitsPerWord - (to % BitVector.BitsPerWord);
                }
            }
        }

        public void Clear()
        {
            Array.Clear(this.Bits, 0, this.Bits.Length);
        }

        public BitVector And(BitVector v)
        {
            // TODO: Avoid touching bits outside of the valid ones. (keep them as Zeroes)

            int words = Math.Min(Bits.Length, v.Bits.Length) - 1;
            while (words >= 0)
            {
                Bits[words] &= v.Bits[words];

                words--;
            }

            return this;
        }

        public BitVector Or(BitVector v)
        {
            // TODO: Avoid touching bits outside of the valid ones. (keep them as Zeroes)

            int words = Math.Min(Bits.Length, v.Bits.Length) - 1;
            while (words >= 0)
            {
                Bits[words] |= v.Bits[words];

                words--;
            }

            return this;
        }

        public BitVector Xor(BitVector v)
        {
            // TODO: Avoid touching bits outside of the valid ones. (keep them as Zeroes)

            int words = Math.Min(Bits.Length, v.Bits.Length) - 1;
            while (words >= 0)
            {
                Bits[words] ^= v.Bits[words];

                words--;
            }

            return this;
        }

        public static BitVector And(BitVector x, BitVector y)
        {
            if (x.Count < y.Count)
            {
                var t = new BitVector(x.Count);
                x.CopyTo(t);
                return t.And(y);
            }
            else
            {
                var t = new BitVector(y.Count);
                y.CopyTo(t);
                return t.And(x);
            }
        }

        public static BitVector Or(BitVector x, BitVector y)
        {
            if (x.Count < y.Count)
            {
                var t = new BitVector(x.Count);
                x.CopyTo(t);
                return t.Or(y);
            }
            else
            {
                var t = new BitVector(y.Count);
                y.CopyTo(t);
                return t.Or(x);
            }
        }

        public static BitVector Xor(BitVector x, BitVector y)
        {
            if (x.Count < y.Count)
            {
                var t = new BitVector(x.Count);
                x.CopyTo(t);
                return t.Xor(y);
            }
            else
            {
                var t = new BitVector(y.Count);
                y.CopyTo(t);
                return t.Xor(x);
            }
        }


        public void CopyTo(BitVector dest)
        {
            Copy(this, dest);
        }

        public static void Copy(BitVector src, BitVector dest)
        {
            Contract.Requires(src.Count <= dest.Count);

            dest.Count = src.Count;

            unsafe
            {
                fixed (ulong* destPtr = dest.Bits)
                fixed (ulong* srcPtr = src.Bits)
                {
                    Memory.CopyInline((byte*)destPtr, (byte*)srcPtr, src.Bits.Length * sizeof(ulong));
                }
            }
        }

        private static void BitwiseCopySlow(BitVector src, int srcStart, BitVector dest, int destStart, int length)
        {
            unchecked
            {
                // Very inefficient. If this is a liability we will make it faster. 
                while ( length > 0 )
                {
                    dest[destStart] = src[srcStart];

                    srcStart++;
                    destStart++;
                    length--;
                }                
            }
        }

        public static BitVector OfLength(int size)
        {
            return new BitVector(size);
        }

        public static BitVector Of(params ulong[] values)
        {
            return new BitVector(values.Length * BitVector.BitsPerWord, values);
        }

        public static BitVector Of(bool prefixFree, params long[] values)
        {
            int prefixAdjustment = (prefixFree ? 1 : 0);

            ulong[] newValue = new ulong[values.Length + prefixAdjustment];
            for( int i = 0; i < values.Length; i++ )
                newValue[i] = (ulong) values[i];

            return new BitVector((values.Length + prefixAdjustment) * BitVector.BitsPerWord, newValue);
        }

        public static BitVector Of(params long[] values)
        {
            ulong[] newValue = new ulong[values.Length];
            for( int i = 0; i < values.Length; i++ )
                newValue[i] = (ulong) values[i];

            return new BitVector(values.Length * BitVector.BitsPerWord, newValue);
        }

        public static BitVector Of(params uint[] values)
        {
            int lastLong = values.Length / 2;
            int size = lastLong + values.Length % 2;

            ulong[] newValue = new ulong[size];
            for (int i = 0; i < lastLong; i++)
                newValue[i] = (ulong)values[2 * i] << 32 | (ulong)values[2 * i + 1];

            if (values.Length % 2 == 1)
                newValue[newValue.Length - 1] = (ulong)values[values.Length - 1] << 32;

            return new BitVector(values.Length * BitVector.BitsPerWord / 2, newValue);
        }

        public static BitVector Of(params byte[] values)
        {
            int extraBytes = values.Length % sizeof(ulong);
            int lastLong = values.Length / sizeof(ulong);

            int size = lastLong + ((extraBytes == 0) ? 0 : 1);

            int position;
            ulong[] newValue = new ulong[size];
            for (int i = 0; i < lastLong; i++)
            {
                position = i * sizeof(ulong);
                newValue[i] = (ulong)values[position] << 64 - 8 | 
                              (ulong)values[position + 1] << 64 - 16 | 
                              (ulong)values[position + 2] << 64 - 24 |
                              (ulong)values[position + 3] << 32 |
                              (ulong)values[position + 4] << 24 |
                              (ulong)values[position + 5] << 16 | 
                              (ulong)values[position + 6] << 8 |
                              (ulong)values[position + 7];
            }

            if ( extraBytes != 0 )
            {
                position = lastLong * sizeof(ulong);

                int bytesLeft = extraBytes;
                ulong lastValue = 0;
                do
                {
                    lastValue = lastValue << 8 | values[position];

                    position++;
                    bytesLeft--;
                }
                while (bytesLeft > 0);

                newValue[size - 1] = lastValue << (8 - extraBytes) * BitVector.BitsPerByte;
            }

            return new BitVector(values.Length * BitVector.BitsPerByte, newValue);
        }

        public static BitVector Of(string value, bool prefixFree = false)
        {
            int prefixAdjustment = (prefixFree ? 1 : 0);

            int valueLength = value.Length + prefixAdjustment;            
            int size = valueLength / 4;
            if (valueLength % 4 != 0)
                size++;

            int position = 0;
            int lastLong = value.Length / 4;
            ulong[] newValue = new ulong[size];            
            for (int i = 0; i < lastLong; i++)
            {                
                newValue[i] = (ulong)value[position] << 48 | (ulong)value[position + 1] << 32 | (ulong)value[position + 2] << 16 | (ulong)value[position + 3];
                position += 4;
            }

            switch( value.Length % 4)
            {
                case 3: newValue[newValue.Length - 1] = (ulong)value[position] << 48 | (ulong)value[position + 1] << 32 | (ulong)value[position + 2] << 16; break;
                case 2: newValue[newValue.Length - 1] = (ulong)value[position] << 48 | (ulong)value[position + 1] << 32; break;
                case 1: newValue[newValue.Length - 1] = (ulong)value[position] << 48; break;
                default: break;
            }

            return new BitVector((value.Length + prefixAdjustment) * BitVector.BitsPerWord / 4, newValue);
                
        }

        public static BitVector Parse(string value)
        {
            var vector = new BitVector(value.Length);

            for (int i = 0; i < value.Length; i++ )
            {
                if (value[i] != '0')
                    vector[i] = true;
            }

            return vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong BitInWord(int idx)
        {
            return 0x8000000000000000UL >> (idx % (int)BitVector.BitsPerWord);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Bit(int idx)
        {
            Contract.Requires(idx < BitVector.BitsPerWord);

            return WordMask & (uint)idx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint WordForBit(int idx)
        {
            return (uint)(idx >> (int)Log2BitsPerWord);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfWordsForBits(int size)
        {
            return (int)((size + WordMask) >> (int)Log2BitsPerWord);
        }

        public int CompareTo(BitVector other)
        {
            int bits;
            return CompareToInline(other, out bits);
        }

        public int CompareTo(BitVector other, out int equalBits)
        {
            return CompareToInline(other, out equalBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareToInline(BitVector other, out int equalBits)
        {
            var srcKey = this.Count;
            var otherKey = other.Count;
            var length = Math.Min(srcKey, otherKey);

            unsafe
            {
                fixed (ulong* srcPtr = this.Bits)
                fixed (ulong* destPtr = other.Bits)
                {
                    int wholeBytes = length / BitsPerWord;

                    ulong* bpx = srcPtr;
                    ulong* bpy = destPtr;

                    for (int i = 0; i < wholeBytes; i++, bpx += 1, bpy += 1)
                    {
                        if (*((ulong*)bpx) != *((ulong*)bpy))
                            break;
                    }

                    // We always finish the last extent with a bit-wise comparison (bit vector is stored in big endian).
                    int from = (int)(bpx - srcPtr) * BitsPerWord;
                    int leftover = length - from;
                    if ( leftover == 0 )
                    {
                        equalBits = length;
                        return 0;
                    }

                    if (leftover > BitVector.BitsPerWord)
                        leftover = BitVector.BitsPerWord;

                    int shift = BitVector.BitsPerWord - leftover;

                    ulong thisWord = ((*bpx) >> shift);
                    ulong otherWord = ((*bpy) >> shift);

                    ulong cmp = thisWord ^ otherWord;
                    if (cmp == 0)
                    {
                        equalBits = length;
                        return 0;
                    }

                    int differentBit = Binary.Bits.MostSignificantBit(cmp);

                    equalBits = from + leftover - (differentBit + 1);
                    return thisWord > otherWord ? 1 : -1;
                }
            }
        }

        public int LongestCommonPrefixLength(BitVector other)
        {           
            int differentBit;
            CompareToInline( other, out differentBit );

            return differentBit;
        }

        public BitVector SubVector(int start, int length)
        {
            Contract.Requires(start >= 0 && start < this.Count);
            Contract.Requires(length >= 0 && start + length < this.Count);

            var subVector = new BitVector(length);
            if ( start % BitsPerWord == 0 )
            {
                int startWord = start / BitsPerWord;
                int totalWords = length / BitsPerWord;

                unsafe
                {
                    fixed (ulong* sourcePtr = this.Bits)
                    fixed (ulong* destPtr = subVector.Bits)
                    {
                        Memory.Copy((byte*)destPtr, (byte*)(sourcePtr + startWord), totalWords * sizeof(ulong));
                    }
                }

                int remainder = length % BitsPerWord;
                if ( remainder != 0 )
                {
                    int shift = BitsPerWord - remainder;
                    ulong value = this.Bits[startWord + totalWords];
                    value = (value >> shift) << shift;

                    subVector.Bits[totalWords] = value;
                }                
            }
            else
            {
                // The cost to optimize this case is high and we are not using it. 
                BitVector.BitwiseCopySlow(this, start, subVector, 0, length);
            }            
            return subVector;
        }

        public bool IsPrefix(BitVector other)
        {
            if (this.Count > other.Count)
                return false;

            int equalBits;
            CompareToInline(other, out equalBits);

            return equalBits == this.Count;
        }

        public bool IsPrefix(BitVector other, int length)
        {
            if (length > this.Count || length > other.Count)
                return false;

            int equalBits;
            CompareToInline(other, out equalBits);

            return equalBits >= length;
        }

        public bool IsProperPrefix(BitVector other)
        {
            if (this.Count >= other.Count)
                return false;

            int equalBits;
            CompareToInline(other, out equalBits);

            return equalBits == this.Count && equalBits != other.Count;
        }

        public bool Equals(BitVector other)
        {
            if (other == null) return false;
            if (this == other) return true;

            int dummy;
            return CompareToInline(other, out dummy) == 0;
        }

        public string ToBinaryString()
        {
            var builder = new StringBuilder();
            for ( int i = 0; i < this.Count; i++ )
            {
                if (i != 0 && i % 8 == 0)
                    builder.Append(" ");

                builder.Append( this[i] ? "1" : "0");                   
            }

            return builder.ToString();
        }


    }
}
