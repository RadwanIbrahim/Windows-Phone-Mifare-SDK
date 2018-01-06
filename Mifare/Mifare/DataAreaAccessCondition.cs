using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mifare
{
    /// <summary>
    /// Handle access condition for a generic datablock of a sector
    /// </summary>
    public class DataAreaAccessCondition
    {
        #region Private fields
        /// <summary>
        /// Dictionary that associate an AccessConditionsSet to a bit array of C1-C2-C3 bits
        /// (see MiFare specs for the meaning of C1-C2-C3)
        /// </summary>
        private static Dictionary<DataAreaAccessCondition, BitArray> _Templates = null;
        #endregion

        #region Costructor
        public DataAreaAccessCondition()
        {
            Read = ConditionEnum.KeyAOrB;
            Write = ConditionEnum.KeyAOrB;
            Increment = ConditionEnum.KeyAOrB;
            Decrement = ConditionEnum.KeyAOrB;
        }
        #endregion

        #region Properties

        #region ConditionEnum
        /// <summary>
        /// List of access conditions that may apply to each operation (read, write, inc, dec)
        /// </summary>
        public enum ConditionEnum
        {
            Never,
            KeyA,
            KeyB,
            KeyAOrB
        }
        #endregion

        #region Read
        /// <summary>
        /// Access condition for read operations on the data block
        /// </summary>
        public ConditionEnum Read;
        #endregion

        #region Write
        /// <summary>
        /// Access condition for write operations on the data block
        /// </summary>
        public ConditionEnum Write;
        #endregion

        #region Increment
        /// Access condition for increment operations on the data block
        public ConditionEnum Increment;
        #endregion

        #region Decrement
        /// Access condition for decrement operations on the data block
        public ConditionEnum Decrement;
        #endregion

        #endregion

        #region Public functions

        #region Equals
        public override bool Equals(object obj)
        {
            DataAreaAccessCondition daac = obj as DataAreaAccessCondition;
            if (daac == null)
                return false;

            return ((daac.Read == Read) && (daac.Write == Write) && (daac.Increment == Increment) && (daac.Decrement == Decrement));
        }
        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region ToString
        public override string ToString()
        {
            BitArray bits = GetBits();
            if (bits == null)
                return "Invalid";

            return bits.ToString();
        }
        #endregion

        #endregion

        #region Private fields

        #region InitTemplates
        private void InitTemplates()
        {
            if (_Templates != null)
                return;

            _Templates = new Dictionary<DataAreaAccessCondition, BitArray>();

            _Templates.Add(new DataAreaAccessCondition()
            {
                Read = ConditionEnum.KeyAOrB,
                Write = ConditionEnum.KeyAOrB,
                Increment = ConditionEnum.KeyAOrB,
                Decrement = ConditionEnum.KeyAOrB
            },
            new BitArray(new bool[] { false, false, false }));

            _Templates.Add(new DataAreaAccessCondition()
            {
                Read = ConditionEnum.KeyAOrB,
                Write = ConditionEnum.Never,
                Increment = ConditionEnum.Never,
                Decrement = ConditionEnum.Never
            },
            new BitArray(new bool[] { false, true, false }));

            _Templates.Add(new DataAreaAccessCondition()
            {
                Read = ConditionEnum.KeyAOrB,
                Write = ConditionEnum.KeyB,
                Increment = ConditionEnum.Never,
                Decrement = ConditionEnum.Never
            },
            new BitArray(new bool[] { true, false, false }));

            _Templates.Add(new DataAreaAccessCondition()
            {
                Read = ConditionEnum.KeyAOrB,
                Write = ConditionEnum.KeyB,
                Increment = ConditionEnum.KeyB,
                Decrement = ConditionEnum.KeyAOrB
            },
            new BitArray(new bool[] { true, true, false }));

            _Templates.Add(new DataAreaAccessCondition()
            {
                Read = ConditionEnum.KeyAOrB,
                Write = ConditionEnum.Never,
                Increment = ConditionEnum.Never,
                Decrement = ConditionEnum.KeyAOrB
            },
            new BitArray(new bool[] { false, false, true }));

            _Templates.Add(new DataAreaAccessCondition()
            {
                Read = ConditionEnum.KeyB,
                Write = ConditionEnum.KeyB,
                Increment = ConditionEnum.Never,
                Decrement = ConditionEnum.Never
            },
            new BitArray(new bool[] { false, true, true }));

            _Templates.Add(new DataAreaAccessCondition()
            {
                Read = ConditionEnum.KeyB,
                Write = ConditionEnum.Never,
                Increment = ConditionEnum.Never,
                Decrement = ConditionEnum.Never
            },
            new BitArray(new bool[] { true, false, true }));

            _Templates.Add(new DataAreaAccessCondition()
            {
                Read = ConditionEnum.Never,
                Write = ConditionEnum.Never,
                Increment = ConditionEnum.Never,
                Decrement = ConditionEnum.Never
            },
            new BitArray(new bool[] { true, true, true }));
        }
        #endregion

        #region GetBits
        /// <summary>
        /// convert the object to the corresponding C1-C2-C3 bits
        /// </summary>
        /// <returns>a 3-elements bit array</returns>
        internal BitArray GetBits()
        {
            InitTemplates();

            foreach (KeyValuePair<DataAreaAccessCondition, BitArray> kvp in _Templates)
            {
                if (kvp.Key.Equals(this))
                    return kvp.Value;
            }

            return _Templates.ElementAt(0).Value;
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize the object based on a DataAreaAccessCondition
        /// </summary>
        /// <param name="access">the DataAreaAccessCondition to clone</param>
        internal void Initialize(DataAreaAccessCondition access)
        {
            Read = access.Read;
            Write = access.Write;
            Increment = access.Increment;
            Decrement = access.Decrement;
        }

        /// <summary>
        /// Initialize object based on a bit array of C1-C2-C3
        /// </summary>
        /// <param name="bits">C1-C2-C3 bit array</param>
        /// <returns></returns>
        internal bool Initialize(BitArray bits)
        {
            InitTemplates();

            foreach (KeyValuePair<DataAreaAccessCondition, BitArray> kvp in _Templates)
            {
                if (kvp.Value.IsEqual(bits))
                {
                    Initialize(kvp.Key);
                    return true;
                }
            }

            return false;
        }
        #endregion

        #endregion
    }
}
