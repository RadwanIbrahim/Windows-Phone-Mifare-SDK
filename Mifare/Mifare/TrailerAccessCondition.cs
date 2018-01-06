using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mifare
{
    public class TrailerAccessCondition
    {
        #region Private fields
        private static Dictionary<TrailerAccessCondition, BitArray> _Templates = null;
        #endregion

        #region Properties

        #region ConditionEnum
        public enum ConditionEnum
        {
            Never,
            KeyA,
            KeyB,
            KeyAOrB
        }
        #endregion

        #region KeyARead
        public ConditionEnum KeyARead;
        #endregion

        #region KeyAWrite
        public ConditionEnum KeyAWrite;
        #endregion

        #region KeyBRead
        public ConditionEnum KeyBRead;
        #endregion

        #region KeyBWrite
        public ConditionEnum KeyBWrite;
        #endregion

        #region AccessBitsRead
        public ConditionEnum AccessBitsRead;
        #endregion

        #region AccessBitsWrite
        public ConditionEnum AccessBitsWrite;
        #endregion

        #endregion

        #region Public functions

        #region Equals
        public override bool Equals(object obj)
        {
            TrailerAccessCondition tac = obj as TrailerAccessCondition;
            if (tac == null)
                return false;

            return ((tac.KeyARead == KeyARead) && (tac.KeyAWrite == KeyAWrite) &&
                (tac.KeyBRead == KeyBRead) && (tac.KeyBWrite == KeyBWrite) &&
                (tac.AccessBitsRead == AccessBitsRead) && (tac.AccessBitsWrite == AccessBitsWrite));
        }
        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #region ToString
        public override String ToString()
        {
            BitArray bits = GetBits();
            if (bits == null)
                return "Invalid";

            return bits.ToString();
        }
        #endregion

        #region GetBits
        public BitArray GetBits()
        {
            InitTemplates();

            foreach (KeyValuePair<TrailerAccessCondition, BitArray> kvp in _Templates)
            {
                if (kvp.Key.Equals(this))
                    return kvp.Value;
            }

            return _Templates.ElementAt(4).Value;
        }
        #endregion

        #region Initialize
        public void Initialize(TrailerAccessCondition access)
        {
            KeyARead = access.KeyARead;
            KeyAWrite = access.KeyAWrite;
            KeyBRead = access.KeyBRead;
            KeyBWrite = access.KeyBWrite;
            AccessBitsRead = access.AccessBitsRead;
            AccessBitsWrite = access.AccessBitsWrite;
        }

        public bool Initialize(BitArray bits)
        {
            InitTemplates();

            foreach (KeyValuePair<TrailerAccessCondition, BitArray> kvp in _Templates)
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

        #region Private functions

        #region InitTemplates
        private void InitTemplates()
        {
            if (_Templates != null)
                return;

            _Templates = new Dictionary<TrailerAccessCondition, BitArray>();

            _Templates.Add(new TrailerAccessCondition()
            {
                KeyARead = ConditionEnum.Never,
                KeyAWrite = ConditionEnum.KeyA,
                AccessBitsRead = ConditionEnum.KeyA,
                AccessBitsWrite = ConditionEnum.Never,
                KeyBRead = ConditionEnum.KeyA,
                KeyBWrite = ConditionEnum.KeyA
            },
            new BitArray(new bool[] { false, false, false }));

            _Templates.Add(new TrailerAccessCondition()
            {
                KeyARead = ConditionEnum.Never,
                KeyAWrite = ConditionEnum.Never,
                AccessBitsRead = ConditionEnum.KeyA,
                AccessBitsWrite = ConditionEnum.Never,
                KeyBRead = ConditionEnum.KeyA,
                KeyBWrite = ConditionEnum.Never
            },
            new BitArray(new bool[] { false, true, false }));

            _Templates.Add(new TrailerAccessCondition()
            {
                KeyARead = ConditionEnum.Never,
                KeyAWrite = ConditionEnum.KeyB,
                AccessBitsRead = ConditionEnum.KeyAOrB,
                AccessBitsWrite = ConditionEnum.Never,
                KeyBRead = ConditionEnum.Never,
                KeyBWrite = ConditionEnum.KeyB
            },
            new BitArray(new bool[] { true, false, false }));

            _Templates.Add(new TrailerAccessCondition()
            {
                KeyARead = ConditionEnum.Never,
                KeyAWrite = ConditionEnum.Never,
                AccessBitsRead = ConditionEnum.KeyAOrB,
                AccessBitsWrite = ConditionEnum.Never,
                KeyBRead = ConditionEnum.Never,
                KeyBWrite = ConditionEnum.Never
            },
            new BitArray(new bool[] { true, true, false }));

            _Templates.Add(new TrailerAccessCondition()
            {
                KeyARead = ConditionEnum.Never,
                KeyAWrite = ConditionEnum.KeyA,
                AccessBitsRead = ConditionEnum.KeyA,
                AccessBitsWrite = ConditionEnum.KeyA,
                KeyBRead = ConditionEnum.KeyA,
                KeyBWrite = ConditionEnum.KeyA
            },
            new BitArray(new bool[] { false, false, true }));

            _Templates.Add(new TrailerAccessCondition()
            {
                KeyARead = ConditionEnum.Never,
                KeyAWrite = ConditionEnum.KeyB,
                AccessBitsRead = ConditionEnum.KeyAOrB,
                AccessBitsWrite = ConditionEnum.KeyB,
                KeyBRead = ConditionEnum.Never,
                KeyBWrite = ConditionEnum.KeyB
            },
            new BitArray(new bool[] { false, true, true }));

            _Templates.Add(new TrailerAccessCondition()
            {
                KeyARead = ConditionEnum.Never,
                KeyAWrite = ConditionEnum.Never,
                AccessBitsRead = ConditionEnum.KeyAOrB,
                AccessBitsWrite = ConditionEnum.KeyB,
                KeyBRead = ConditionEnum.Never,
                KeyBWrite = ConditionEnum.Never
            },
            new BitArray(new bool[] { true, false, true }));

            _Templates.Add(new TrailerAccessCondition()
            {
                KeyARead = ConditionEnum.Never,
                KeyAWrite = ConditionEnum.Never,
                AccessBitsRead = ConditionEnum.KeyAOrB,
                AccessBitsWrite = ConditionEnum.Never,
                KeyBRead = ConditionEnum.Never,
                KeyBWrite = ConditionEnum.Never
            },
            new BitArray(new bool[] { true, true, true }));
        }
        #endregion

        #endregion
    }
}
