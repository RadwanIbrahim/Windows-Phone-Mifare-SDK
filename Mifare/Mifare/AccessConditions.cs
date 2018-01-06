using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mifare
{
    /// <summary>
    /// Class that handles the access conditions to a given sector of the card
    /// </summary>
    public class AccessConditions
    {
        #region Constructor
        public AccessConditions()
        {
            DataAreas = new DataAreaAccessCondition[3];
            DataAreas[0] = new DataAreaAccessCondition();
            DataAreas[1] = new DataAreaAccessCondition();
            DataAreas[2] = new DataAreaAccessCondition();

            Trailer = new TrailerAccessCondition();

            MADVersion = MADVersionEnum.NoMAD;
            MultiApplicationCard = false;
        }
        #endregion

        #region Properties

        #region MADVersion
        /// <summary>
        /// Version of the MAD supported by the card. The MAD version is written only in the trailer datablock of sector 0.
        /// For all other sector, this value has no meaning
        /// </summary>
        public enum MADVersionEnum
        {
            NoMAD,
            Version1,
            Version2
        }
        public MADVersionEnum MADVersion;
        #endregion

        #region MultiApplicationCard
        /// <summary>
        /// True if the card supports multiple applications
        /// </summary>
        public bool MultiApplicationCard;
        #endregion

        #region DataAreas
        /// <summary>
        /// Access conditions for each data area. This array has always 3 elements
        /// </summary>
        public DataAreaAccessCondition[] DataAreas;
        #endregion

        #region Trailer
        /// <summary>
        /// Access conditions for the trailer datablock
        /// </summary>
        public TrailerAccessCondition Trailer;
        #endregion

        #endregion
    }
}
