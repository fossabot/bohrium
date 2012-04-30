﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumCIL.Int32
{
    using T = System.Int32;
    using InArray = NumCIL.Generic.NdArray<System.Int32>;
    using OutArray = NdArray;

    /// <summary>
    /// A wrapper for a basic NdArray to a similar one with typed operators
    /// </summary>
    public partial class NdArray : IEnumerable<InArray>
    {
        #region NdArray Mimics
        /// <summary>
        /// The value instance that gives access to values
        /// </summary>
        public InArray.ValueAccessor Value { get { return value.Value; } }
        /// <summary>
        /// A reference to the shape instance that describes this view
        /// </summary>
        public Shape Shape { get { return value.Shape; } }
        /// <summary>
        /// A reference to the underlying data storage
        /// </summary>
        public T[] Data { get { return value.Data; } }
        /// <summary>
        /// Gets a subview on the array
        /// </summary>
        /// <param name="index">The element to get the view from</param>
        /// <returns>A view on the selected element</returns>
        public OutArray this[params long[] index] { get { return this.value[index]; } set { this.value[index] = value; } }
        /// <summary>
        /// Gets a subview on the array
        /// </summary>
        /// <param name="ranges">The range get the view from</param>
        /// <returns>A view on the selected element</returns>
        public OutArray this[params Range[] ranges] { get { return this.value[ranges]; } set { this.value[ranges] = value; } }
        /// <summary>
        /// Returns a flattened (1-d copy) of the current data view
        /// </summary>
        /// <returns>A flattened copy</returns>
        public OutArray Flatten() { return this.value.Flatten(); }
        /// <summary>
        /// Returns a copy of the underlying data, shaped as this view
        /// </summary>
        /// <returns>A copy of the view data</returns>
        public OutArray Clone() { return this.value.Clone(); }
        /// <summary>
        /// Generates a new view based on this array
        /// </summary>
        /// <param name="newshape">The new shape</param>
        /// <returns>The reshaped array</returns>
        public OutArray Reshape(Shape newshape) { return this.value.Reshape(newshape); }
        /// <summary>
        /// Returns a view that is a view of a single element
        /// </summary>
        /// <param name="element">The element to view</param>
        /// <returns>The subview</returns>
        public OutArray Subview(long element) { return this.value.Subview(element); }
        /// <summary>
        /// Returns a view that is a view of a range of elements
        /// </summary>
        /// <param name="range">The range to view</param>
        /// <param name="dimension">The dimension to view</param>
        /// <returns>The subview</returns>
        public OutArray Subview(Range range, long dimension) { return this.value.Subview(range, dimension); }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<InArray> GetEnumerator() { return this.value.GetEnumerator(); }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this.value.GetEnumerator(); }
        /// <summary>
        /// A debug tag used for naming a view
        /// </summary>
        public string Name { get { return this.value.Name; } set { this.value.Name = value; } }

        /// <summary>
        /// Constructs a NdArray that is a scalar wrapper,
        /// allows simple scalar operations on arbitrary
        /// NdArrays
        /// </summary>
        /// <param name="value">The scalar value</param>
        public NdArray(T value)
            : this(new T[] { value })
        {
        }

        /// <summary>
        /// Constructs a new NdArray over a pre-allocated array
        /// </summary>
        /// <param name="shape">The shape of the new NdArray</param>
        public NdArray(Shape shape)
            : this(Generate.Empty(shape))
        {
        }

        /// <summary>
        /// Constructs a new NdArray over a pre-allocated array and shapes it
        /// </summary>
        /// <param name="data">The data to wrap in a NdArray</param>
        /// <param name="shape">The shape to view the array in</param>
        public NdArray(T[] data, Shape shape = null)
            : this(new InArray(data, shape))
        {
        }

        /// <summary>
        /// Constructs a new NdArray over a pre-allocated array and shapes it
        /// </summary>
        /// <param name="source">An existing array that will be re-shaped</param>
        /// <param name="newshape">The shape to view the array in</param>
        public NdArray(InArray source, Shape newshape)
            : this(new InArray(source, newshape))
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.value.ToString();
        }

        /// <summary>
        /// Flushes pending operations on the array
        /// </summary>
        public void Flush() { this.value.Flush(); }

        /// <summary>
        /// Returns a transposed view of this array. If <paramref name="out"/> is supplied, the contents are copied into that array.
        /// </summary>
        /// <param name="out">Optional output array</param>
        /// <returns>A transposed view</returns>
        public OutArray Transpose(OutArray @out = null) { return value.Transpose(@out); }

        /// <summary>
        /// Gets or sets data in a transposed view
        /// </summary>
        public OutArray Transposed { get { return value.Transposed; } set { value.Transposed = this.value; } }

        /// <summary>
        /// Repeats elements of the array
        /// </summary>
        /// <param name="repeats">The number of repeats to perform</param>
        /// <param name="axis">The axis to repeat, if not speficied, repeat is done on a flattened array</param>
        /// <returns>A repeated copy of the input data</returns>
        public OutArray Repeat(long repeats, long? axis = null) { return value.Repeat(repeats, axis); }

        /// <summary>
        /// Repeats elements of the array
        /// </summary>
        /// <param name="repeats">The number of repeats to perform in each axis</param>
        /// <param name="axis">The axis to repeat, if not specified, repeat is done on a flattened array</param>
        /// <returns>A repeated copy of the input data</returns>
        public OutArray Repeat(long[] repeats, long? axis = null) { return value.Repeat(repeats, axis); }

        /// <summary>
        /// Concatenates an array onto this array, joined at the axis
        /// </summary>
        /// <param name="arg">The array to join</param>
        /// <param name="axis">The axis to join at</param>
        /// <returns>The joined array</returns>
        public OutArray Concatenate(OutArray arg, long axis = 0) { return this.value.Concatenate(arg, axis); }

        /// <summary>
        /// Performs matrix multiplication on the two elements
        /// </summary>
        /// <param name="arg">The right-hand-side argument in matrix multiplication</param>
        /// <param name="out">Optional target for the multiplication</param>
        /// <returns>The matrix multiplication result</returns>
        public OutArray MatrixMultiply(OutArray arg, OutArray @out = null) { return UFunc.Matmul<T, Add, Mul>(this, arg, @out); }
        #endregion

        /// <summary>
        /// The only data member of the struct is a reference to the underlying view
        /// </summary>
        private readonly InArray value;
        /// <summary>
        /// Constructs a new typed array from a basic one
        /// </summary>
        /// <param name="v">The basic array</param>
        public NdArray(InArray v) { this.value = v; }

        #region Implict conversion operators
        /// <summary>
        /// Implicit operator that returns a typed array from a basic one
        /// </summary>
        /// <param name="v">The basic array to wrap</param>
        /// <returns>A wrapped array</returns>
        public static implicit operator OutArray(InArray v) { return v == null ? null : new OutArray(v); }
        /// <summary>
        /// Implicit operator that returns a basic array from a wrapped one
        /// </summary>
        /// <param name="v">The wrapper array</param>
        /// <returns>The basic array</returns>
        public static implicit operator InArray(NdArray v) { return v == null ? null : v.value; }
        /// <summary>
        /// Implicit operator that returns a wrapped array from a scalar value
        /// </summary>
        /// <param name="v">The scalar value</param>
        /// <returns>A wrapped array representing the scalar value</returns>
        public static implicit operator OutArray(T v) { return new OutArray(v); }
        #endregion

        #region Operator implementations
        /// <summary>
        /// Calculates addition to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of adding the two input operands</returns>
        public static OutArray operator +(OutArray a, OutArray b) { return UFunc.Apply<T, Add>(a, b, null); }
        /// <summary>
        /// Calculates subtraction of the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of subtracting the two input operands</returns>
        public static OutArray operator -(OutArray a, OutArray b) { return UFunc.Apply<T, Sub>(a, b, null); }
        /// <summary>
        /// Calculates multiplication to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of multiplying the two input operands</returns>
        public static OutArray operator *(OutArray a, OutArray b) { return UFunc.Apply<T, Mul>(a, b, null); }
        /// <summary>
        /// Calculates division of to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of dividing the two input operands</returns>
        public static OutArray operator /(OutArray a, OutArray b) { return UFunc.Apply<T, Div>(a, b, null); }
        /// <summary>
        /// Calculates modulo of the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The modulo result of dividing the two input operands</returns>
        public static OutArray operator %(OutArray a, OutArray b) { return UFunc.Apply<T, Mod>(a, b, null); }

        /// <summary>
        /// Applies addition to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of adding the two input operands</returns>
        public static OutArray operator +(OutArray a, T b) { return UFunc.Apply<T, Add>(a, b, null); }
        /// <summary>
        /// Calculates subtraction of the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of subtracting the two input operands</returns>
        public static OutArray operator -(OutArray a, T b) { return UFunc.Apply<T, Sub>(a, b, null); }
        /// <summary>
        /// Calculates multiplication to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of multiplying the two input operands</returns>
        public static OutArray operator *(OutArray a, T b) { return UFunc.Apply<T, Mul>(a, b, null); }
        /// <summary>
        /// Calculates division of to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of dividing the two input operands</returns>
        public static OutArray operator /(OutArray a, T b) { return UFunc.Apply<T, Div>(a, b, null); }
        /// <summary>
        /// Calculates modulo of the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The modulo result of dividing the two input operands</returns>
        public static OutArray operator %(OutArray a, T b) { return UFunc.Apply<T, Mod>(a, b, null); }

        /// <summary>
        /// Applies addition to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of adding the two input operands</returns>
        public static OutArray operator +(T a, OutArray b) { return UFunc.Apply<T, Add>(new NdArray(a), b, null); }
        /// <summary>
        /// Calculates subtraction of the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of subtracting the two input operands</returns>
        public static OutArray operator -(T a, OutArray b) { return UFunc.Apply<T, Sub>(new NdArray(a), b, null); }
        /// <summary>
        /// Calculates multiplication to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of multiplying the two input operands</returns>
        public static OutArray operator *(T a, OutArray b) { return UFunc.Apply<T, Mul>(new NdArray(a), b, null); }
        /// <summary>
        /// Calculates division of to the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The result of dividing the two input operands</returns>
        public static OutArray operator /(T a, OutArray b) { return UFunc.Apply<T, Div>(new NdArray(a), b, null); }
        /// <summary>
        /// Calculates modulo of the two operands
        /// </summary>
        /// <param name="a">One input operand</param>
        /// <param name="b">Another input operand</param>
        /// <returns>The modulo result of dividing the two input operands</returns>
        public static OutArray operator %(T a, OutArray b) { return UFunc.Apply<T, Mod>(new NdArray(a), b, null); }

        /// <summary>
        /// Calculates the incremented value of the operand
        /// </summary>
        /// <param name="a">An input operand</param>
        /// <returns>The result of incrementing each value</returns>
        public static OutArray operator ++(OutArray a) { return UFunc.Apply<T, Inc>(a); }
        /// <summary>
        /// Calculates the decremented value of the operand
        /// </summary>
        /// <param name="a">An input operand</param>
        /// <returns>The result of decrementing each value</returns>
        public static OutArray operator --(OutArray a) { return UFunc.Apply<T, Inc>(a); }
        #endregion

        #region Common function implementations
        /// <summary>
        /// Adds the values of an NdArray to this NdArray and returns the result as a new NdArray
        /// </summary>
        /// <param name="op">The operand to add</param>
        /// <param name="out">Optional target for addition, use to perform in-place addition</param>
        /// <returns>An NdArray which is the result of adding the two NdArrays</returns>
        public OutArray Add(OutArray op, OutArray @out = null) { return UFunc.Apply<T, Add>(this, op, @out); }
        /// <summary>
        /// Adds the values of a scalar to this NdArray and returns the result as a new NdArray
        /// </summary>
        /// <param name="scalar">The operand to add</param>
        /// <param name="out">Optional target for addition, use to perform in-place addition</param>
        /// <returns>An NdArray which is the result of adding the scalar value</returns>
        public OutArray Add(T scalar, OutArray @out = null) { return UFunc.Apply<T, Add>(this, scalar, @out); }
        /// <summary>
        /// Subtracts the values of an NdArray from this NdArray and returns the result as a new NdArray
        /// </summary>
        /// <param name="op">The operand to subtract</param>
        /// <param name="out">Optional target for addition, use to perform in-place subtraction</param>
        /// <returns>An NdArray which is the result of subtracting the two NdArrays</returns>
        public OutArray Sub(OutArray op, OutArray @out = null) { return UFunc.Apply<T, Sub>(this, op, @out); }
        /// <summary>
        /// Subtracts the values of a scalar from this NdArray and returns the result as a new NdArray
        /// </summary>
        /// <param name="scalar">The operand to subtract</param>
        /// <param name="out">Optional target for addition, use to perform in-place subtraction</param>
        /// <returns>An NdArray which is the result of subtracting the scalar value</returns>
        public OutArray Sub(T scalar, OutArray @out = null) { return UFunc.Apply<T, Sub>(this, scalar, @out); }
        /// <summary>
        /// Multiplies the values of an NdArray to this NdArray and returns the result as a new NdArray
        /// </summary>
        /// <param name="op">The operand to multiply with</param>
        /// <param name="out">Optional target for multiplication, use to perform in-place multiplication</param>
        /// <returns>An NdArray which is the result of multiplying the two NdArrays</returns>
        public OutArray Mul(OutArray op, OutArray @out = null) { return UFunc.Apply<T, Mul>(this, op, @out); }
        /// <summary>
        /// Multiplies the values of a scalar to this NdArray and returns the result as a new NdArray
        /// </summary>
        /// <param name="scalar">The operand to multiply with</param>
        /// <param name="out">Optional target for multiplication, use to perform in-place multiplication</param>
        /// <returns>An NdArray which is the result of multiplying the scalar value</returns>
        public OutArray Mul(T scalar, OutArray @out = null) { return UFunc.Apply<T, Mul>(this, scalar, @out); }
        /// <summary>
        /// Divides the values of an NdArray with this NdArray and returns the result as a new NdArray
        /// </summary>
        /// <param name="op">The operand to divide with</param>
        /// <param name="out">Optional target for division, use to perform in-place division</param>
        /// <returns>An NdArray which is the result of dividing the two NdArrays</returns>
        public OutArray Div(OutArray op, OutArray @out = null) { return UFunc.Apply<T, Div>(this, op, @out); }
        /// <summary>
        /// Divides the values of a scalar to this NdArray and returns the result as a new NdArray
        /// </summary>
        /// <param name="scalar">The operand to divide with</param>
        /// <param name="out">Optional target for division, use to perform in-place division</param>
        /// <returns>An NdArray which is the result of dividing with the scalar value</returns>
        public OutArray Div(T scalar, OutArray @out = null) { return UFunc.Apply<T, Div>(this, scalar, @out); }
        /// <summary>
        /// Divides values of an NdArray with this NdArray and returns the modulo result as a new NdArray
        /// </summary>
        /// <param name="op">The operand to divide with</param>
        /// <param name="out">Optional target for modulo result, use to perform in-place modulo</param>
        /// <returns>An NdArray which is the modulo residues of dividing the two NdArrays</returns>
        public OutArray Mod(OutArray op, OutArray @out = null) { return UFunc.Apply<T, Mod>(this, op, @out); }
        /// <summary>
        /// Divides NdArray with a scalar and returns the modulo result as a new NdArray
        /// </summary>
        /// <param name="scalar">The operand to divide with</param>
        /// <param name="out">Optional target for modulo result, use to perform in-place modulo</param>
        /// <returns>An NdArray which is the modulo residues of dividing with the scalar value</returns>
        public OutArray Mod(T scalar, OutArray @out = null) { return UFunc.Apply<T, Mod>(this, scalar, @out); }

        /// <summary>
        /// Calculates the maximum values of each element in two NdArrays and returns the result as a new NdArray
        /// </summary>
        /// <param name="op">The operand to compare with</param>
        /// <param name="out">Optional target for the max operation, use to perform in-place maximum calculation</param>
        /// <returns>An NdArray which is the maximum values of the two NdArrays</returns>
        public OutArray Max(OutArray op, OutArray @out = null) { return UFunc.Apply<T, Max>(this, op, @out); }
        /// <summary>
        /// Calculates the maximum value of a scalar and this NdArray, and returns the result as a new NdArray
        /// </summary>
        /// <param name="scalar">The operand to compare with</param>
        /// <param name="out">Optional target for the max operation, use to perform in-place maximum calculation</param>
        /// <returns>An NdArray which is the maximum of the scalar value and this NdArray</returns>
        public OutArray Max(T scalar, OutArray @out = null) { return UFunc.Apply<T, Max>(this, scalar, @out); }
        /// <summary>
        /// Calculates the minimum values of each element in two NdArrays and returns the result as a new NdArray
        /// </summary>
        /// <param name="op">The operand to compare with</param>
        /// <param name="out">Optional target for the min operation, use to perform in-place minimum calculation</param>
        /// <returns>An NdArray which is the minimum values of the two NdArrays</returns>
        public OutArray Min(OutArray op, OutArray @out = null) { return UFunc.Apply<T, Min>(this, op, @out); }
        /// <summary>
        /// Calculates the minimum value of a scalar and this NdArray, and returns the result as a new NdArray
        /// </summary>
        /// <param name="scalar">The operand to compare with</param>
        /// <param name="out">Optional target for the min operation, use to perform in-place minimum calculation</param>
        /// <returns>An NdArray which is the minimum of the scalar value and this NdArray</returns>
        public OutArray Min(T scalar, OutArray @out = null) { return UFunc.Apply<T, Min>(this, scalar, @out); }

        /// <summary>
        /// Returns a new NdArray that is the result of rounding up all the elements in this NdArray to the nearest integer
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with all values rounded up to the nearest integer</returns>
        public OutArray Ceiling(OutArray @out = null) { return UFunc.Apply<T, Ceiling>(this, @out); }
        /// <summary>
        /// Returns a new NdArray that is the result of rounding down all the elements in this NdArray to the nearest integer
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with all values rounded down to the nearest integer</returns>
        public OutArray Floor(OutArray @out = null) { return UFunc.Apply<T, Floor>(this, @out); }
        /// <summary>
        /// Returns a new NdArray that is the result of rounding all the elements in this NdArray to the nearest integer
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with all values rounded to the nearest integer</returns>
        public OutArray Round(OutArray @out = null) { return UFunc.Apply<T, Round>(this, @out); }
        /// <summary>
        /// Returns a new NdArray that has the absolute value of all elements in this array
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with all absolute values</returns>
        public OutArray Abs(OutArray @out = null) { return UFunc.Apply<T, Abs>(this, @out); }
        /// <summary>
        /// Returns a new NdArray that is the result of calculating the square root for all the elements in this NdArray
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with the square root of all values</returns>
        public OutArray Sqrt(OutArray @out = null) { return UFunc.Apply<T, Sqrt>(this, @out); }
        /// <summary>
        /// Returns a new NdArray that is the result of calculating the exponent for all the elements in this NdArray
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with the exponent of all values</returns>
        public OutArray Exp(OutArray @out = null) { return UFunc.Apply<T, Exp>(this, @out); }
        /// <summary>
        /// Returns a new NdArray with the negated value of all elements in this NdArray
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with all values negated</returns>
        public OutArray Negate(OutArray @out = null) { return UFunc.Apply<T, Negate>(this, @out); }
        /// <summary>
        /// Returns a new NdArray that is the result of calculating the logarithmic value of all the elements in this NdArray
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with the logarithmic of all values</returns>
        public OutArray Log(OutArray @out = null) { return UFunc.Apply<T, Log>(this, @out); }
        /// <summary>
        /// Returns a new NdArray that is the result of calculating the logarithmic-10 value of all the elements in this NdArray
        /// </summary>
        /// <param name="out">Optional target for the operation, use to perform the operation in-place</param>
        /// <returns>An NdArray with the logarithmic-10 of all values</returns>
        public OutArray Log10(OutArray @out = null) { return UFunc.Apply<T, Log10>(this, @out); }
        /// <summary>
        /// Calculates each element in this NdArray as being raised to the power of the mathcing element in the operand NdArray
        /// </summary>
        /// <param name="op">The power-of values</param>
        /// <param name="out">Optional target for the pow operation, use to perform in-place power-of calculation</param>
        /// <returns>An NdArray which is the values of this NdArray raised to the power of the values in the operand NdArray</returns>
        public OutArray Pow(NdArray op, OutArray @out = null) { return UFunc.Apply<T, Pow>(this, op, @out); }
        /// <summary>
        /// Calculates each element in this NdArray as being raised to the power of the scalar value
        /// </summary>
        /// <param name="value">The power-of value</param>
        /// <param name="out">Optional target for the pow operation, use to perform in-place power-of calculation</param>
        /// <returns>An NdArray which is the values of this NdArray raised to the power of the scalar value</returns>
        public OutArray Pow(T value, OutArray @out = null)
        {
            if (value == 2)
                return UFunc.Apply<T, Mul>(this, this, null);
            else
                return UFunc.Apply<T, Pow>(this, value, null);
        }
        #endregion


        #region Appliers for custom UFuncs
        /// <summary>
        /// Applies a unary lambda function to each element in this NdArray
        /// </summary>
        /// <param name="op">The lamda function to apply</param>
        /// <param name="out">An optional output array, use this to perform the function in-place</param>
        /// <returns>An NdArray that is the result of applying the lambda function to each element</returns>
        public OutArray Apply(Func<T, T> op, OutArray @out = null) { return UFunc.Apply(op, this, @out); }
        /// <summary>
        /// Applies a binary lambda function to each element in this NdArray
        /// </summary>
        /// <param name="op">The lamda function to apply</param>
        /// <param name="b">An input operand</param>
        /// <returns>An NdArray that is the result of applying the lambda function to each element</returns>
        /// <param name="out">An optional output array, use this to perform the function in-place</param>
        public OutArray Apply(Func<T, T, T> op, OutArray b, OutArray @out = null) { return UFunc.Apply<T>(op, this, b, @out); }
        /// <summary>
        /// Applies a unary function to each element in this NdArray
        /// </summary>
        /// <typeparam name="C">The function to apply</typeparam>
        /// <param name="out">An optional output array, use this to perform the function in-place</param>
        /// <returns>An NdArray that is the result of applying the function to each element</returns>
        public OutArray Apply<C>(OutArray @out = null) where C : struct, IUnaryOp<T> { return UFunc.Apply<T, C>(this, @out); }
        /// <summary>
        /// Applies a binary function to each element in this NdArray
        /// </summary>
        /// <typeparam name="C">The function to apply</typeparam>
        /// <param name="b">An input operand</param>
        /// <param name="out">An optional output array, use this to perform the function in-place</param>
        /// <returns>An NdArray that is the result of applying the function to each element</returns>
        public OutArray Apply<C>(OutArray b, OutArray @out = null) where C : struct, IBinaryOp<T> { return UFunc.Apply<T, C>(this, b, @out); }
        /// <summary>
        /// Applies a nullary function to each element in this NdArray
        /// Note that Nullary functions are performed in-place
        /// </summary>
        /// <param name="op">The function to apply</param>
        /// <returns>This NdArray</returns>
        public OutArray Apply(INullaryOp<T> op) { UFunc.Apply<T>(op, this); return this; }
        /// <summary>
        /// Applies a unary function to each element in this NdArray
        /// </summary>
        /// <param name="op">The function to apply</param>
        /// <param name="out">An optional output array, use this to perform the function in-place</param>
        /// <returns>An NdArray that is the result of applying the function to each element</returns>
        public OutArray Apply(IUnaryOp<T> op, OutArray @out = null) { return UFunc.Apply<T>(op, this, @out); }
        /// <summary>
        /// Applies a binary function to each element in this NdArray
        /// </summary>
        /// <param name="op">The function to apply</param>
        /// <param name="b">An input operand</param>
        /// <param name="out">An optional output array, use this to perform the function in-place</param>
        /// <returns>An NdArray that is the result of applying the function to each element</returns>
        public OutArray Apply(IBinaryOp<T> op, OutArray b, OutArray @out = null) { return UFunc.Apply<T>(op, this, b, @out); }
        /// <summary>
        /// Applies a binary function to each element in this NdArray
        /// </summary>
        /// <param name="op">The function to apply</param>
        /// <param name="b">An input operand</param>
        /// <param name="out">An optional output array, use this to perform the function in-place</param>
        /// <returns>An NdArray that is the result of applying the function to each element</returns>
        public OutArray Apply(IBinaryOp<T> op, T b, OutArray @out = null) { return UFunc.Apply<T>(op, this, new NdArray(b), @out); }

        /// <summary>
        /// Performs a reduction operation on this array
        /// </summary>
        /// <typeparam name="C">The function to apply reduction with</typeparam>
        /// <param name="axis">The axis to remove</param>
        /// <param name="out">An optional output array, use this to perform the reduction in-place</param>
        /// <returns>An NdArray that is the result of reducing the selected axis</returns>
        public OutArray Reduce<C>(long axis = 0, OutArray @out = null) where C : struct, IBinaryOp<T> { return UFunc.Reduce<T, C>(this, axis, @out); }
        /// <summary>
        /// Performs a reduction operation on this array
        /// </summary>
        /// <param name="op">The function to apply reduction with</param>
        /// <param name="axis">The axis to remove</param>
        /// <param name="out">An optional output array, use this to perform the function in-place</param>
        /// <returns>A reduced array</returns>
        public OutArray Reduce(IBinaryOp<T> op, long axis = 0, OutArray @out = null) { return UFunc.Reduce<T>(op, this, axis, @out); }

        /// <summary>
        /// Calculates the scalar result of applying the binary operation to all elements
        /// </summary>
        /// <typeparam name="C">The operation to perform</typeparam>
        /// <returns>A scalar value that is the result of aggregating all elements</returns>
        public T Aggregate<C>() where C : struct, IBinaryOp<T> { return UFunc.Aggregate<T, C>(this); }

        /// <summary>
        /// Calculates the scalar result of applying the binary operation to all elements
        /// </summary>
        /// <param name="op">The operation to perform</param>
        /// <returns>A scalar value that is the result of aggregating all elements</returns>
        public T Aggregate(IBinaryOp<T> op) { return UFunc.Aggregate<T>(op, this); }

        /// <summary>
        /// Calculates the sum of all elements
        /// </summary>
        /// <returns>A scalar value that is the sum of all elements</returns>
        public T Sum() { return UFunc.Aggregate<T, Add>(this); }

        /// <summary>
        /// Calculates the maximum of all elements
        /// </summary>
        /// <returns>A scalar value that is the maximum of all elements</returns>
        public T Max() { return UFunc.Aggregate<T, Max>(this); }

        /// <summary>
        /// Calculates the minimum of all elements
        /// </summary>
        /// <returns>A scalar value that is the minimum of all elements</returns>
        public T Min() { return UFunc.Aggregate<T, Min>(this); }
        #endregion
    }

    #region Struct instances for common operations
    /// <summary>
    /// Collection of instantiated operation structs
    /// </summary>
    public struct Ops
    {
        /// <summary>
        /// The addition operation
        /// </summary>
        public static readonly Add Add;
        /// <summary>
        /// The subtraction operation
        /// </summary>
        public static readonly Sub Sub;
        /// <summary>
        /// The multiplication operation
        /// </summary>
        public static readonly Mul Mul;
        /// <summary>
        /// The division operation
        /// </summary>
        public static readonly Div Div;
        /// <summary>
        /// The modulo operation
        /// </summary>
        public static readonly Mod Mod;
        /// <summary>
        /// The maximum operation
        /// </summary>
        public static readonly Max Max;
        /// <summary>
        /// The minimum operation
        /// </summary>
        public static readonly Min Min;

        /// <summary>
        /// The increment operation
        /// </summary>
        public static readonly Inc Inc;
        /// <summary>
        /// The decrement operation
        /// </summary>
        public static readonly Dec Dec;
        /// <summary>
        /// The floor operation
        /// </summary>
        public static readonly Floor Floor;
        /// <summary>
        /// The ceiling operation
        /// </summary>
        public static readonly Ceiling Ceiling;
        /// <summary>
        /// The round operation
        /// </summary>
        public static readonly Round Round;
        /// <summary>
        /// The absolute operation
        /// </summary>
        public static readonly Abs Abs;
        /// <summary>
        /// The square root operation
        /// </summary>
        public static readonly Sqrt Sqrt;
        /// <summary>
        /// The exponential operation
        /// </summary>
        public static readonly Exp Exp;
        /// <summary>
        /// The negate operation
        /// </summary>
        public static readonly Negate Negate;
        /// <summary>
        /// The logarithmic operation
        /// </summary>
        public static readonly Log Log;
        /// <summary>
        /// The logarithmic-10 operation
        /// </summary>
        public static readonly Log10 Log10;
        /// <summary>
        /// The power operation
        /// </summary>
        public static readonly Pow Pow;
    }
    #endregion

    #region Operator implementations
    /// <summary>
    /// The addition operator implementation
    /// </summary>
    public struct Add : IBinaryOp<T>
    {
        /// <summary>
        /// Implementation of adding two numbers
        /// </summary>
        /// <param name="a">One operand</param>
        /// <param name="b">Another operand</param>
        /// <returns>The result of adding the two numbers</returns>
        public T Op(T a, T b)
        { return (T)(a + b); }

        /// <summary>
        /// Reduces the input array with the addition operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <param name="axis">The axis to reduce</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>A reduced NdArray</returns>
        public static OutArray Reduce(OutArray arg, long axis = 0, OutArray @out = null)
        { return UFunc.Reduce<T, Add>(arg, axis, @out); }

        /// <summary>
        /// Reduces the input array to a scalar with the addition operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <returns>The scalar result of the aggregation</returns>
        public static T Aggregate(OutArray arg)
        { return UFunc.Aggregate<T, Add>(arg); }

        /// <summary>
        /// Applies the addition operation to the input operands
        /// </summary>
        /// <param name="in1">One input operand</param>
        /// <param name="in2">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray in1, OutArray in2, OutArray @out = null)
        { return UFunc.Apply<T, Add>(in1, in2, @out); }

        /// <summary>
        /// Applies the addition operation to the input operands
        /// </summary>
        /// <param name="in">One input operand</param>
        /// <param name="scalar">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray @in, T scalar, OutArray @out = null)
        { return UFunc.Apply<T, Add>(@in, scalar, @out); }
    }
    /// <summary>
    /// The subtraction operator implementation
    /// </summary>
    public struct Sub : IBinaryOp<T>
    {
        /// <summary>
        /// Implementation of subtracting two numbers
        /// </summary>
        /// <param name="a">One operand</param>
        /// <param name="b">Another operand</param>
        /// <returns>The result of subtracting the two numbers</returns>
        public T Op(T a, T b)
        { return (T)(a - b); }

        /// <summary>
        /// Reduces the input array with the subtraction operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <param name="axis">The axis to reduce</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>A reduced NdArray</returns>
        public static OutArray Reduce(OutArray arg, long axis = 0, OutArray @out = null)
        { return UFunc.Reduce<T, Sub>(arg, axis, @out); }

        /// <summary>
        /// Reduces the input array to a scalar with the subtraction operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <returns>The scalar result of the aggregation</returns>
        public static T Aggregate(OutArray arg)
        { return UFunc.Aggregate<T, Sub>(arg); }

        /// <summary>
        /// Applies the subtraction operation to the input operands
        /// </summary>
        /// <param name="in1">One input operand</param>
        /// <param name="in2">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray in1, OutArray in2, OutArray @out = null)
        { return UFunc.Apply<T, Sub>(in1, in2, @out); }

        /// <summary>
        /// Applies the subtraction operation to the input operands
        /// </summary>
        /// <param name="in">One input operand</param>
        /// <param name="scalar">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray @in, T scalar, OutArray @out = null)
        { return UFunc.Apply<T, Add>(@in, scalar, @out); }
    }
    /// <summary>
    /// The multiplication operator implementation
    /// </summary>
    public struct Mul : IBinaryOp<T>
    {
        /// <summary>
        /// Implementation of multiplying two numbers
        /// </summary>
        /// <param name="a">One operand</param>
        /// <param name="b">Another operand</param>
        /// <returns>The result of multiplying the two numbers</returns>
        public T Op(T a, T b)
        { return (T)(a * b); }

        /// <summary>
        /// Reduces the input array with the multiplication operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <param name="axis">The axis to reduce</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>A reduced NdArray</returns>
        public static OutArray Reduce(OutArray arg, long axis = 0, OutArray @out = null)
        { return UFunc.Reduce<T, Mul>(arg, axis, @out); }

        /// <summary>
        /// Reduces the input array to a scalar with the multiplication operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <returns>The scalar result of the aggregation</returns>
        public static T Aggregate(OutArray arg)
        { return UFunc.Aggregate<T, Mul>(arg); }

        /// <summary>
        /// Applies the multiply operation to the input operands
        /// </summary>
        /// <param name="in1">One input operand</param>
        /// <param name="in2">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray in1, OutArray in2, OutArray @out = null)
        { return UFunc.Apply<T, Mul>(in1, in2, @out); }

        /// <summary>
        /// Applies the multiply operation to the input operands
        /// </summary>
        /// <param name="in">One input operand</param>
        /// <param name="scalar">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray @in, T scalar, OutArray @out = null)
        { return UFunc.Apply<T, Add>(@in, scalar, @out); }
    }
    /// <summary>
    /// The division operator implementation
    /// </summary>
    public struct Div : IBinaryOp<T>
    {
        /// <summary>
        /// Implementation of dividing two numbers
        /// </summary>
        /// <param name="a">One operand</param>
        /// <param name="b">Another operand</param>
        /// <returns>The result of dividing the two numbers</returns>
        public T Op(T a, T b)
        { return (T)(a / b); }

        /// <summary>
        /// Reduces the input array with the division operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <param name="axis">The axis to reduce</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>A reduced NdArray</returns>
        public static OutArray Reduce(OutArray arg, long axis = 0, OutArray @out = null)
        { return UFunc.Reduce<T, Div>(arg, axis, @out); }

        /// <summary>
        /// Reduces the input array to a scalar with the division operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <returns>The scalar result of the aggregation</returns>
        public static T Aggregate(OutArray arg)
        { return UFunc.Aggregate<T, Div>(arg); }

        /// <summary>
        /// Applies the divide operation to the input operands
        /// </summary>
        /// <param name="in1">One input operand</param>
        /// <param name="in2">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray in1, OutArray in2, OutArray @out = null)
        { return UFunc.Apply<T, Div>(in1, in2, @out); }

        /// <summary>
        /// Applies the divide operation to the input operands
        /// </summary>
        /// <param name="in">One input operand</param>
        /// <param name="scalar">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray @in, T scalar, OutArray @out = null)
        { return UFunc.Apply<T, Add>(@in, scalar, @out); }
    }
    /// <summary>
    /// The modulo operation implementation
    /// </summary>
    public struct Mod : IBinaryOp<T>
    {
        /// <summary>
        /// Implementation of the modulo operation on two numbers
        /// </summary>
        /// <param name="a">One operand</param>
        /// <param name="b">Another operand</param>
        /// <returns>The modulo result of dividing the two numbers</returns>
        public T Op(T a, T b)
        { return (T)(a % b); }

        /// <summary>
        /// Reduces the input array with the modulo operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <param name="axis">The axis to reduce</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>A reduced NdArray</returns>
        public static OutArray Reduce(OutArray arg, long axis = 0, OutArray @out = null)
        { return UFunc.Reduce<T, Mod>(arg, axis, @out); }

        /// <summary>
        /// Reduces the input array to a scalar with the modulo operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <returns>The scalar result of the aggregation</returns>
        public static T Aggregate(OutArray arg)
        { return UFunc.Aggregate<T, Mod>(arg); }

        /// <summary>
        /// Applies the modulo operation to the input operands
        /// </summary>
        /// <param name="in1">One input operand</param>
        /// <param name="in2">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray in1, OutArray in2, OutArray @out = null)
        { return UFunc.Apply<T, Mod>(in1, in2, @out); }

        /// <summary>
        /// Applies the modulo operation to the input operands
        /// </summary>
        /// <param name="in">One input operand</param>
        /// <param name="scalar">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray @in, T scalar, OutArray @out = null)
        { return UFunc.Apply<T, Add>(@in, scalar, @out); }
    }

    /// <summary>
    /// The maximum operation implementation
    /// </summary>
    public struct Max : IBinaryOp<T>
    {
        /// <summary>
        /// Implementation of calculating the maximum of two numbers
        /// </summary>
        /// <param name="a">One operand</param>
        /// <param name="b">Another operand</param>
        /// <returns>The largest of the two numbers</returns>
        public T Op(T a, T b)
        { return (T)Math.Max(a, b); }

        /// <summary>
        /// Reduces the input array with the maximum operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <param name="axis">The axis to reduce</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>A reduced NdArray</returns>
        public static OutArray Reduce(OutArray arg, long axis = 0, OutArray @out = null)
        { return UFunc.Reduce<T, Max>(arg, axis, @out); }

        /// <summary>
        /// Reduces the input array to a scalar with the maximum operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <returns>The scalar result of the aggregation</returns>
        public static T Aggregate(OutArray arg)
        { return UFunc.Aggregate<T, Max>(arg); }

        /// <summary>
        /// Applies the maximum operation to the input operands
        /// </summary>
        /// <param name="in1">One input operand</param>
        /// <param name="in2">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray in1, OutArray in2, OutArray @out = null)
        { return UFunc.Apply<T, Max>(in1, in2, @out); }

        /// <summary>
        /// Applies the maximum operation to the input operands
        /// </summary>
        /// <param name="in">One input operand</param>
        /// <param name="scalar">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray @in, T scalar, OutArray @out = null)
        { return UFunc.Apply<T, Add>(@in, scalar, @out); }
    }
    /// <summary>
    /// The minimum operation implementation
    /// </summary>
    public struct Min : IBinaryOp<T>
    {
        /// <summary>
        /// Implementation of calculating the minimum of two numbers
        /// </summary>
        /// <param name="a">One operand</param>
        /// <param name="b">Another operand</param>
        /// <returns>The smallest of the two numbers</returns>
        public T Op(T a, T b)
        { return (T)Math.Min(a, b); }

        /// <summary>
        /// Reduces the input array with the minimum operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <param name="axis">The axis to reduce</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>A reduced NdArray</returns>
        public static OutArray Reduce(OutArray arg, long axis = 0, OutArray @out = null)
        { return UFunc.Reduce<T, Min>(arg, axis, @out); }

        /// <summary>
        /// Reduces the input array to a scalar with the minimum operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <returns>The scalar result of the aggregation</returns>
        public static T Aggregate(OutArray arg)
        { return UFunc.Aggregate<T, Min>(arg); }

        /// <summary>
        /// Applies the minimum operation to the input operands
        /// </summary>
        /// <param name="in1">One input operand</param>
        /// <param name="in2">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray in1, OutArray in2, OutArray @out = null)
        { return UFunc.Apply<T, Min>(in1, in2, @out); }

        /// <summary>
        /// Applies the minimum operation to the input operands
        /// </summary>
        /// <param name="in">One input operand</param>
        /// <param name="scalar">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray @in, T scalar, OutArray @out = null)
        { return UFunc.Apply<T, Add>(@in, scalar, @out); }
    }

    /// <summary>
    /// The increment operation implementation
    /// </summary>
    public struct Inc : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the increment operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The result of incrementing the operand</returns>
        public T Op(T a) { return (T)(a + (T)1); }

        /// <summary>
        /// Applies the increment operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Inc>(@in, @out); }
    }
    /// <summary>
    /// The decrement operation implementation
    /// </summary>
    public struct Dec : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the decrement operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The result of decrementing the operand</returns>
        public T Op(T a) { return (T)(a + (T)1); }

        /// <summary>
        /// Applies the decrement operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Dec>(@in, @out); }
    }

    /// <summary>
    /// The ceil operation implementation
    /// </summary>
    public struct Ceiling : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the ceiling operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The ceiling value of the operand</returns>
        public T Op(T a) { return (T)Math.Ceiling((double)a); }

        /// <summary>
        /// Applies the ceiling operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Ceiling>(@in, @out); }
    }
    /// <summary>
    /// The floor operation implementation
    /// </summary>
    public struct Floor : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the floor operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The floor value of the operand</returns>
        public T Op(T a) { return (T)Math.Floor((double)a); }

        /// <summary>
        /// Applies the floor operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Floor>(@in, @out); }
    }
    /// <summary>
    /// The round operation implementation
    /// </summary>
    public struct Round : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the round operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The rounded value of the operand</returns>
        public T Op(T a) { return (T)Math.Round((double)a); }

        /// <summary>
        /// Applies the round operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Round>(@in, @out); }
    }
    /// <summary>
    /// The absolute operation implementation
    /// </summary>
    public struct Abs : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the absolute operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The absolute value of the operand</returns>
        public T Op(T a) { return (T)Math.Abs(a); }

        /// <summary>
        /// Applies the absolute operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Abs>(@in, @out); }
    }
    /// <summary>
    /// The square root operation implementation
    /// </summary>
    public struct Sqrt : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the square root operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The square root value of the operand</returns>
        public T Op(T a) { return (T)Math.Sqrt(a); }

        /// <summary>
        /// Applies the square root operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Sqrt>(@in, @out); }
    }
    /// <summary>
    /// The exponential operation implementation
    /// </summary>
    public struct Exp : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the exponential operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The exponential value of the operand</returns>
        public T Op(T a) { return (T)Math.Exp(a); }

        /// <summary>
        /// Applies the exponential operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Exp>(@in, @out); }
    }
    /// <summary>
    /// The negate operation implementation
    /// </summary>
    public struct Negate : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the negation operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The negated value of the operand</returns>
        public T Op(T a) { return (T)(-a); }

        /// <summary>
        /// Applies the negate operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Negate>(@in, @out); }
    }
    /// <summary>
    /// The logarithmic operation implementation
    /// </summary>
    public struct Log : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the logarithmic operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The logarithmic value of the operand</returns>
        public T Op(T a) { return (T)Math.Log(a); }

        /// <summary>
        /// Applies the logarithmic operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Log>(@in, @out); }
    }
    /// <summary>
    /// The logarithmic-10 operation implementation
    /// </summary>
    public struct Log10 : IUnaryOp<T>
    {
        /// <summary>
        /// Implementation of the logarithmic-10 operation
        /// </summary>
        /// <param name="a">The input operand</param>
        /// <returns>The logarithmic-10 value of the operand</returns>
        public T Op(T a) { return (T)Math.Log10(a); }

        /// <summary>
        /// Applies the logarithmic-10 operation to the operand
        /// </summary>
        /// <param name="in">The input operand</param>
        /// <param name="out">An optional output operand, use to perform the operation in-place</param>
        /// <returns>The result of applying the operation to the input array</returns>
        public static OutArray Apply(OutArray @in, OutArray @out = null)
        { return UFunc.Apply<T, Log10>(@in, @out); }
    }
    /// <summary>
    /// The power operation implementation
    /// </summary>
    public struct Pow : IBinaryOp<T>
    {
        /// <summary>
        /// Implementation of rasing a number to the power of another number
        /// </summary>
        /// <param name="a">One operand</param>
        /// <param name="b">Another operand</param>
        /// <returns>The result of rasing operand a to the power of operand b</returns>
        public T Op(T a, T b)
        { return (T)Math.Pow(a, b); }

        /// <summary>
        /// Reduces the input array with the power operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <param name="axis">The axis to reduce</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>A reduced NdArray</returns>
        public static OutArray Reduce(OutArray arg, long axis = 0, OutArray @out = null)
        { return UFunc.Reduce<T, Max>(arg, axis, @out); }

        /// <summary>
        /// Reduces the input array to a scalar with the power operation
        /// </summary>
        /// <param name="arg">The input operand</param>
        /// <returns>The scalar result of the aggregation</returns>
        public static T Aggregate(OutArray arg)
        { return UFunc.Aggregate<T, Pow>(arg); }

        /// <summary>
        /// Applies the power operation to the input operands
        /// </summary>
        /// <param name="in1">One input operand</param>
        /// <param name="in2">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray in1, OutArray in2, OutArray @out = null)
        { return UFunc.Apply<T, Add>(in1, in2, @out); }

        /// <summary>
        /// Applies the power operation to the input operands
        /// </summary>
        /// <param name="in">One input operand</param>
        /// <param name="scalar">Another input operand</param>
        /// <param name="out">An optional output array, use to perform the operation in-place</param>
        /// <returns>An NdArray that is the result of applying the operation to the two input operands</returns>
        public static OutArray Apply(OutArray @in, T scalar, OutArray @out = null)
        { return UFunc.Apply<T, Add>(@in, scalar, @out); }
    }
    #endregion

    #region Generate mimics
    /// <summary>
    /// Generates basic initial arrays
    /// </summary>
    public static class Generate
    {
        /// <summary>
        /// The factory class that produces the NdArray instances
        /// </summary>
        public static NumCIL.Generic.Generator<T> Generator = new NumCIL.Generic.Generator<T>();

        /// <summary>
        /// Creates a range sequential integers, starting with zero
        /// </summary>
        /// <param name="shape">The shape of the NdArray</param>
        /// <returns>A shaped array with sequential numbers</returns>
        public static OutArray Arange(Shape shape) { return Generator.Arange(shape); }
        /// <summary>
        /// Creates an array filled with the value 1
        /// </summary>
        /// <param name="shape">The shape of the NdArray</param>
        /// <returns>A shaped array with all values set to 1</returns>
        public static OutArray Ones(Shape shape) { return Generator.Ones(shape); }
        /// <summary>
        /// Creates an array filled with the value 0
        /// </summary>
        /// <param name="shape">The shape of the NdArray</param>
        /// <returns>A shaped array with all values set to 0</returns>
        public static OutArray Zeroes(Shape shape) { return Generator.Zeroes(shape); }
        /// <summary>
        /// Creates an uninitialized array
        /// </summary>
        /// <param name="shape">The shape of the NdArray</param>
        /// <returns>A shaped array with uninitialized data</returns>
        public static OutArray Empty(Shape shape) { return Generator.Empty(shape); }
        /// <summary>
        /// Creates an array filled with the given value
        /// </summary>
        /// <param name="value">The value to fill the array with</param>
        /// <param name="shape">The shape of the NdArray</param>
        /// <returns>A shaped array with all values set to the given value</returns>
        public static OutArray Same(T value, Shape shape) { return Generator.Same(value, shape); }
        /// <summary>
        /// Creates an array filled with the value random numbers
        /// </summary>
        /// <param name="shape">The shape of the NdArray</param>
        /// <returns>A shaped array with all values set to random numbers</returns>
        public static OutArray Random(Shape shape) { return Generator.Random(shape); }

        /// <summary>
        /// Creates a range sequential integers, starting with zero
        /// </summary>
        /// <param name="dimensions">The size of each dimension</param>
        /// <returns>A shaped array with sequential numbers</returns>
        public static OutArray Arange(params long[] dimensions) { return Generator.Arange(dimensions); }
        /// <summary>
        /// Creates an array filled with the value 1
        /// </summary>
        /// <param name="dimensions">The size of each dimension</param>
        /// <returns>A shaped array with all values set to 1</returns>
        public static OutArray Ones(params long[] dimensions) { return Generator.Ones(dimensions); }
        /// <summary>
        /// Creates an array filled with the value 0
        /// </summary>
        /// <param name="dimensions">The size of each dimension</param>
        /// <returns>A shaped array with all values set to 0</returns>
        public static OutArray Zeroes(params long[] dimensions) { return Generator.Zeroes(dimensions); }
        /// <summary>
        /// Creates an uninitialized array
        /// </summary>
        /// <param name="dimensions">The size of each dimension</param>
        /// <returns>A shaped array with uninitialized values</returns>
        public static OutArray Empty(params long[] dimensions) { return Generator.Empty(dimensions); }
        /// <summary>
        /// Creates an array filled with the given value
        /// </summary>
        /// <param name="value">The value to set fill the array with</param>
        /// <param name="dimensions">The size of each dimension</param>
        /// <returns>A shaped array with all values set to the given value</returns>
        public static OutArray Same(T value, params long[] dimensions) { return Generator.Same(value, dimensions); }
        /// <summary>
        /// Creates an array filled with random values
        /// </summary>
        /// <param name="dimensions">The size of each dimension</param>
        /// <returns>A shaped array with all values set to a random value</returns>
        public static OutArray Random(params long[] dimensions) { return Generator.Random(dimensions); }
    }
    #endregion
}


